//
//      coded by un
//            --------------
//                     mindshifter.com
//

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Mindshifter
{
    public sealed partial class Manipulator
    {
        /// <summary>
        /// Initializes scale-specific manipulator data and functions
        /// </summary>
        private void InitScale()
        {
            // transformation matrices for use in drawing each axis
            Dictionary<AxisFlags, Matrix> axis_transforms = new Dictionary<AxisFlags, Matrix>();

            axis_transforms[AxisFlags.X] = Matrix.CreateRotationZ(-MathHelper.PiOver2);
            axis_transforms[AxisFlags.Y] = Matrix.Identity;
            axis_transforms[AxisFlags.Z] = Matrix.CreateRotationX(MathHelper.PiOver2);
            axis_transforms[AxisFlags.X | AxisFlags.Y] = Matrix.Identity;
            axis_transforms[AxisFlags.X | AxisFlags.Z] = Matrix.CreateRotationX(MathHelper.PiOver2);
            axis_transforms[AxisFlags.Y | AxisFlags.Z] = Matrix.CreateRotationY(-MathHelper.PiOver2);

            // the base draw handler for single-axis draw calls
            Action<Matrix> base_draw_axis = new Action<Matrix>(delegate(Matrix transform)
            {
                GraphicsDevice device = mGraphics.GraphicsDevice;
                ManipulatorSettings.ScaleSettings settings = mSettings.Scale;

                if ((settings.AxisDrawMode & AxisDirections.Positive) == AxisDirections.Positive)
                {
                    Primitives.DrawLine(device, Vector3.Zero, Vector3.UnitY * settings.AxisExtent, transform);
                    Primitives.DrawBox(device, Vector3.UnitY * settings.AxisExtent,
                        Vector3.One * settings.AxisHandleSize, transform);
                }

                if ((settings.AxisDrawMode & AxisDirections.Negative) == AxisDirections.Negative)
                {
                    Primitives.DrawLine(device, Vector3.Zero, -Vector3.UnitY * settings.AxisExtent, transform);
                    Primitives.DrawBox(device, -Vector3.UnitY * settings.AxisExtent,
                        Vector3.One * settings.AxisHandleSize, transform);
                }
            });

            // use the base single-axis draw handler for all single axes, and
            // transform using the appropriate axis transform from the dictionary above
            mDrawFunctions[TransformationMode.ScaleAxis][AxisFlags.X]
                = mDrawFunctions[TransformationMode.ScaleAxis][AxisFlags.Y]
                = mDrawFunctions[TransformationMode.ScaleAxis][AxisFlags.Z]
                    = delegate(AxisFlags axis)
                    {
                        base_draw_axis(axis_transforms[axis]);
                    };

            Action<Matrix> base_draw_plane = new Action<Matrix>(delegate(Matrix transform)
            {
                GraphicsDevice device = mGraphics.GraphicsDevice;
                ManipulatorSettings.ScaleSettings settings = mSettings.Scale;

                bool draw_pos = (settings.AxisDrawMode & AxisDirections.Positive) == AxisDirections.Positive;
                bool draw_neg = (settings.AxisDrawMode & AxisDirections.Negative) == AxisDirections.Negative;

                if (draw_pos)
                {
                    Primitives.DrawLine(device, Vector3.UnitY * settings.AxisExtent,
                        Vector3.UnitX * settings.AxisExtent, transform);
                    Primitives.DrawSphere(device, Vector3.UnitY * settings.AxisExtent * 0.5f 
                        + Vector3.UnitX * settings.AxisExtent * 0.5f, settings.PlaneHandleRadius, 10, 20, transform);
                }

                if (draw_neg)
                {
                    Primitives.DrawLine(device, -Vector3.UnitY * settings.AxisExtent,
                        -Vector3.UnitX * settings.AxisExtent, transform);
                    Primitives.DrawSphere(device, -Vector3.UnitY * settings.AxisExtent * 0.5f
                        + -Vector3.UnitX * settings.AxisExtent * 0.5f, settings.PlaneHandleRadius, 10, 20, transform);
                }

                if (draw_pos && draw_neg)
                {
                    Primitives.DrawLine(device, -Vector3.UnitY * settings.AxisExtent,
                        Vector3.UnitX * settings.AxisExtent, transform);
                    Primitives.DrawSphere(device, -Vector3.UnitY * settings.AxisExtent * 0.5f
                        + Vector3.UnitX * settings.AxisExtent * 0.5f, settings.PlaneHandleRadius, 10, 20, transform);

                    Primitives.DrawLine(device, Vector3.UnitY * settings.AxisExtent,
                        -Vector3.UnitX * settings.AxisExtent, transform);
                    Primitives.DrawSphere(device, Vector3.UnitY * settings.AxisExtent * 0.5f
                        + -Vector3.UnitX * settings.AxisExtent * 0.5f, settings.PlaneHandleRadius, 10, 20, transform);
                }
            });

            mDrawFunctions[TransformationMode.ScalePlane][AxisFlags.XY]
                = mDrawFunctions[TransformationMode.ScalePlane][AxisFlags.XZ]
                = mDrawFunctions[TransformationMode.ScalePlane][AxisFlags.YZ]
                    = delegate(AxisFlags axis)
                    {
                        base_draw_plane(axis_transforms[axis]);
                    };

            mDrawFunctions[TransformationMode.ScaleUniform][AxisFlags.X | AxisFlags.Y | AxisFlags.Z]
                = delegate(AxisFlags axis)
                {
                    ManipulatorSettings.ScaleSettings settings = mSettings.Scale;
                    Primitives.DrawSphere(mGraphics.GraphicsDevice, Vector3.Zero,
                        settings.UniformHandleRadius, 10, 20);
                };

            // all single-axis scaling will use the same manip function
            mManipFunctions[TransformationMode.ScaleAxis][AxisFlags.X]
                = mManipFunctions[TransformationMode.ScaleAxis][AxisFlags.Y]
                = mManipFunctions[TransformationMode.ScaleAxis][AxisFlags.Z]
                    = delegate()
                    {
                        // get the axis for the component being scaled
                        Vector3 axis = GetUnitAxis(mSelectedAxes);

                        // get a translation matrix on which the projection of the above axis
                        // will be based
                        Matrix tmtx = Matrix.CreateTranslation(mTransform.Translation);

                        // project the axis into screen space
                        Vector3 p0 = Viewport.Project(Vector3.Zero, ProjectionMatrix, ViewMatrix, tmtx);
                        Vector3 p1 = Viewport.Project(axis, ProjectionMatrix, ViewMatrix, tmtx);

                        // disregard the z component for 2D calculations
                        p0.Z = p1.Z = 0;

                        // Vector3 versions of the mouse input positions
                        Vector3 ps = new Vector3(mInput.Start, 0);
                        Vector3 pe = new Vector3(mInput.End, 0);

                        // calculate the axis vector and vectors from the translation point
                        // to each of the mouse positions
                        Vector3 v0 = p1 - p0;
                        Vector3 vs = ps - p0;
                        Vector3 ve = pe - p0;

                        // project both mouse positions onto the axis vector and calculate
                        // their scalars
                        float proj_s = Math.Abs(Vector3.Dot(vs, v0) / v0.Length());
                        float proj_e = Math.Abs(Vector3.Dot(ve, v0) / v0.Length());

                        // find the ratio between the projected scalar values
                        Vector3 scale = mTransform.Scale;
                        float ratio = (proj_e / proj_s);

                        // scale the appropriate axis by the ratio
                        switch (mSelectedAxes)
                        {
                            case AxisFlags.X:
                                scale.X *= ratio;
                                break;

                            case AxisFlags.Y:
                                scale.Y *= ratio;
                                break;

                            case AxisFlags.Z:
                                scale.Z *= ratio;
                                break;
                        }

                        // clamp each component of the new scale to a sane value
                        scale.X = MathHelper.Clamp(scale.X, float.Epsilon, float.MaxValue);
                        scale.Y = MathHelper.Clamp(scale.Y, float.Epsilon, float.MaxValue);
                        scale.Z = MathHelper.Clamp(scale.Z, float.Epsilon, float.MaxValue);

                        // scale the transform
                        mTransform.Scale = scale;
                    };

            // all dual-axis scaling will use the same manip function
            mManipFunctions[TransformationMode.ScalePlane][AxisFlags.X | AxisFlags.Y]
                = mManipFunctions[TransformationMode.ScalePlane][AxisFlags.X | AxisFlags.Z]
                = mManipFunctions[TransformationMode.ScalePlane][AxisFlags.Y | AxisFlags.Z]
                    = delegate()
                    {
                        // get the plane that corresponds to the axes on which we are performing the scale
                        Plane p = GetPlane(mSelectedAxes);

                        // cast rays from the mouse start and end positions
                        Ray start_ray = GetPickRay(mInput.Start);
                        Ray end_ray = GetPickRay(mInput.End);

                        // intersect each ray with the scale plane
                        float? start_hit = start_ray.Intersects(p);
                        float? end_hit = end_ray.Intersects(p);

                        // bail out if either of the rays failed to intersect the plane
                        if (!start_hit.HasValue || !end_hit.HasValue)
                            return;

                        // calculate the intersection points of each ray along the plane
                        Vector3 start_pos = start_ray.Position + (start_ray.Direction * start_hit.Value);
                        Vector3 end_pos = end_ray.Position + (end_ray.Direction * end_hit.Value);

                        // find the vectors from the transform's position to each intersection point
                        Vector3 start_to_pos = start_pos - mTransform.Translation;
                        Vector3 end_to_pos = end_pos - mTransform.Translation;

                        // get the lengths of both of these vectors and find the ratio between them
                        float start_len = start_to_pos.Length();
                        float end_len = end_to_pos.Length();

                        Vector3 scale = mTransform.Scale;
                        float ratio = (start_len == 0) 
                            ? (1) 
                            : (end_len / start_len);

                        // scale the selected components by the ratio
                        if ((mSelectedAxes & AxisFlags.X) == AxisFlags.X)
                            scale.X *= ratio;
                        if ((mSelectedAxes & AxisFlags.Y) == AxisFlags.Y)
                            scale.Y *= ratio;
                        if ((mSelectedAxes & AxisFlags.Z) == AxisFlags.Z)
                            scale.Z *= ratio;

                        // clamp each component of the new scale to a sane value
                        scale.X = MathHelper.Clamp(scale.X, float.Epsilon, float.MaxValue);
                        scale.Y = MathHelper.Clamp(scale.Y, float.Epsilon, float.MaxValue);
                        scale.Z = MathHelper.Clamp(scale.Z, float.Epsilon, float.MaxValue);

                        // scale the transform
                        mTransform.Scale = scale;
                    };

            mManipFunctions[TransformationMode.ScaleUniform][AxisFlags.X | AxisFlags.Y | AxisFlags.Z]
                    = delegate()
                    {
                        // get the direction of the transformation's position to the camera position
                        Vector3 pos_to_cam 
                            = Matrix.Invert(ViewMatrix).Translation - mTransform.Translation;

                        // normalize the direction for use in plane construction
                        if(pos_to_cam != Vector3.Zero)
                            pos_to_cam.Normalize();

                        // create a plane with the normal calculated above that passes through
                        // the transform's position
                        Plane p = Plane.Transform(new Plane(pos_to_cam, 0), 
                            Matrix.CreateTranslation(mTransform.Translation));

                        // cast pick rays from the mouse start and end points
                        Ray start_ray = GetPickRay(mInput.Start);
                        Ray end_ray = GetPickRay(mInput.End);

                        // intersect each ray with the plane
                        float? start_hit = start_ray.Intersects(p);
                        float? end_hit = end_ray.Intersects(p);

                        // bail out if either of the rays fails to intersect the plane
                        if (!start_hit.HasValue || !end_hit.HasValue)
                            return;

                        // calculate the intersection points of each ray along the plane
                        Vector3 start_pos = start_ray.Position + (start_ray.Direction * start_hit.Value);
                        Vector3 end_pos = end_ray.Position + (end_ray.Direction * end_hit.Value);

                        // find the vectors from the transform's position to each intersection point
                        Vector3 start_to_pos = start_pos - mTransform.Translation;
                        Vector3 end_to_pos = end_pos - mTransform.Translation;

                        // get the lengths of both of these vectors and find the ratio between them
                        float start_len = start_to_pos.Length();
                        float end_len = end_to_pos.Length();

                        Vector3 scale = mTransform.Scale;
                        float ratio = (start_len == 0) 
                            ? (1) 
                            : (end_len / start_len);

                        // multiply the scale uniformly by the ratio of the start and end vector lengths
                        scale *= ratio;

                        // clamp each component of the new scale to a sane value
                        scale.X = MathHelper.Clamp(scale.X, float.Epsilon, float.MaxValue);
                        scale.Y = MathHelper.Clamp(scale.Y, float.Epsilon, float.MaxValue);
                        scale.Z = MathHelper.Clamp(scale.Z, float.Epsilon, float.MaxValue);

                        // scale the transform
                        mTransform.Scale = scale;
                    };
        }
    }
}
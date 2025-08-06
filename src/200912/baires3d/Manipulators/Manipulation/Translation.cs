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
        /// Initializes translation-specific manipulator data and functions
        /// </summary>
        private void InitTranslation()
        {
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

                ManipulatorSettings.TranslationSettings settings = mSettings.Translation;

                // draw the axis cylinder to represent the axis itself
                if((settings.AxisDrawMode & AxisDirections.Positive) == AxisDirections.Positive)
                {
                    Primitives.DrawCylinder(device, Vector3.UnitY * settings.AxisExtent * 0.5f,
                        settings.AxisExtent * 0.5f, settings.AxisRadius, 8, transform);
                    Primitives.DrawCone(device, Vector3.UnitY * settings.AxisExtent, settings.ConeRadius,
                        settings.ConeHeight, 8, transform);
                }

                if ((settings.AxisDrawMode & AxisDirections.Negative) == AxisDirections.Negative)
                {
                    Matrix y_invert = Matrix.CreateRotationX(MathHelper.Pi);

                    Primitives.DrawCylinder(device, -Vector3.UnitY * settings.AxisExtent * 0.5f,
                        settings.AxisExtent * 0.5f, settings.AxisRadius, 8, transform);
                    Primitives.DrawCone(device, Vector3.UnitY * settings.AxisExtent, settings.ConeRadius,
                        settings.ConeHeight, 8, y_invert * transform);
                }
            });

            // use the base single axis draw function for each of the main axes, and
            // transform using the appropriate axis transform from the array defined aboved
            mDrawFunctions[TransformationMode.TranslationAxis][AxisFlags.X]
                = mDrawFunctions[TransformationMode.TranslationAxis][AxisFlags.Y]
                = mDrawFunctions[TransformationMode.TranslationAxis][AxisFlags.Z]
                    = delegate(AxisFlags axis)
                    {
                        base_draw_axis(axis_transforms[axis]);
                    };

            Action<Matrix> base_draw_plane = new Action<Matrix>(delegate(Matrix transform)
            {
                GraphicsDevice device = mGraphics.GraphicsDevice;
                ManipulatorSettings.TranslationSettings settings = mSettings.Translation;

                Vector3 up = Vector3.UnitY * settings.PlaneQuadrantSize;
                Vector3 right = Vector3.UnitX * settings.PlaneQuadrantSize;

                bool draw_pos = (settings.AxisDrawMode & AxisDirections.Positive) == AxisDirections.Positive;
                bool draw_neg = (settings.AxisDrawMode & AxisDirections.Negative) == AxisDirections.Negative;

                if (draw_pos)
                    Primitives.DrawTriangle(device, up, right, Vector3.Zero, transform);

                if (draw_neg)
                    Primitives.DrawTriangle(device, -up, -right, Vector3.Zero, transform);
                
                if(draw_pos && draw_neg)
                {
                    Primitives.DrawTriangle(device, -up, right, Vector3.Zero, transform);
                    Primitives.DrawTriangle(device, up, -right, Vector3.Zero, transform);
                }
            });

            mDrawFunctions[TransformationMode.TranslationPlane][AxisFlags.X | AxisFlags.Y]
                = mDrawFunctions[TransformationMode.TranslationPlane][AxisFlags.X | AxisFlags.Z]
                = mDrawFunctions[TransformationMode.TranslationPlane][AxisFlags.Y | AxisFlags.Z]
                    = delegate(AxisFlags axis)
                    {
                        base_draw_plane(axis_transforms[axis]);
                    };

            // all single-axis translations will use the same manip function
            mManipFunctions[TransformationMode.TranslationAxis][AxisFlags.X]
                = mManipFunctions[TransformationMode.TranslationAxis][AxisFlags.Y]
                = mManipFunctions[TransformationMode.TranslationAxis][AxisFlags.Z]
                    = delegate()
                    {
                        // get the unit version of the seclected axis
                        Vector3 axis = GetUnitAxis(mSelectedAxes);

                        // we need to project using the translation component of the current
                        // ITransform in order to obtain a projected unit axis originating
                        // from the transform's position
                        Matrix translation = Matrix.CreateTranslation(mTransform.Translation);

                        // project the origin onto the screen at the transform's position
                        Vector3 start_position = Viewport.Project(Vector3.Zero, ProjectionMatrix, ViewMatrix, translation);
                        // project the unit axis onto the screen at the transform's position
                        Vector3 end_position = Viewport.Project(axis, ProjectionMatrix, ViewMatrix, translation);

                        // calculate the normalized direction vector of the unit axis in screen space
                        Vector3 screen_direction = Vector3.Normalize(end_position - start_position);

                        // calculate the projected mouse delta along the screen direction vector
                        end_position = start_position + (screen_direction * (Vector3.Dot(new Vector3(mInput.Delta, 0f), screen_direction)));

                        // unproject the screen points back into world space using the translation transform
                        // to get the world space start and end positions in regard to the mouse delta along
                        // the mouse direction vector
                        start_position = Viewport.Unproject(start_position, ProjectionMatrix, ViewMatrix, translation);
                        end_position = Viewport.Unproject(end_position, ProjectionMatrix, ViewMatrix, translation);

                        // calculate the difference vector between the world space start and end points
                        Vector3 difference = end_position - start_position;

                        // create a view frustum based on the  current view and projection matrices
                        BoundingFrustum frust = new BoundingFrustum(ViewMatrix * ProjectionMatrix);

                        // if the new translation position is within the current frustum, then add the difference
                        // to the current transform's translation component, otherwise the transform would be outside of
                        // the screen
                        if (frust.Contains(mTransform.Translation + difference) == ContainmentType.Contains)
                            mTransform.Translation += difference;
                    };

            // all planetranslations will use the same manip function
            mManipFunctions[TransformationMode.TranslationPlane][AxisFlags.X | AxisFlags.Y]
                = mManipFunctions[TransformationMode.TranslationPlane][AxisFlags.X | AxisFlags.Z]
                = mManipFunctions[TransformationMode.TranslationPlane][AxisFlags.Y | AxisFlags.Z]
                    = delegate()
                    {
                        // get the plane representing the two selected axes
                        Plane p = GetPlane(mSelectedAxes);

                        // cast rays into the scene from the mouse start and end points
                        Ray sray = GetPickRay(mInput.Start);
                        Ray eray = GetPickRay(mInput.End);

                        // intersect the pick rays with the dual axis plane we want to move along
                        float? sisect = sray.Intersects(p);
                        float? eisect = eray.Intersects(p);

                        // if either of the intersections is invalid then bail out as it would
                        // be impossible to calculate the difference
                        if (!sisect.HasValue || !eisect.HasValue)
                            return;

                        // obtain the intersection points of each ray with the dual axis plane
                        Vector3 spos = sray.Position + (sray.Direction * sisect.Value);
                        Vector3 epos = eray.Position + (eray.Direction * eisect.Value);

                        // calculate the difference between the intersection points
                        Vector3 diff = epos - spos;

                        // obtain the current view frustum using the camera's view and projection matrices
                        BoundingFrustum frust = new BoundingFrustum(ViewMatrix * ProjectionMatrix);

                        // if the new translation is within the current camera frustum, then add the difference
                        // to the current transformation's translation component
                        if (frust.Contains(mTransform.Translation + diff) == ContainmentType.Contains)
                            mTransform.Translation += diff;
                    };
        }
    }
}
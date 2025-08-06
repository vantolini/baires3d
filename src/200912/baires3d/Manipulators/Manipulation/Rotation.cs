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
        /// Initializes rotation-specific manipulator data and functions
        /// </summary>
        private void InitRotation()
        {
            // transformation matrices by which to transform each axis when drawing
            Dictionary<AxisFlags, Matrix> axis_transforms = new Dictionary<AxisFlags, Matrix>();

            axis_transforms[AxisFlags.X] = Matrix.CreateRotationZ(MathHelper.PiOver2);
            axis_transforms[AxisFlags.Y] = Matrix.Identity;
            axis_transforms[AxisFlags.Z] = Matrix.CreateRotationX(MathHelper.PiOver2);

            // all rotation axes are drawn using a single circle strip
            mDrawFunctions[TransformationMode.Rotation][AxisFlags.X]
                = mDrawFunctions[TransformationMode.Rotation][AxisFlags.Y]
                = mDrawFunctions[TransformationMode.Rotation][AxisFlags.Z]
                    = delegate(AxisFlags axis)
                    {
                        ManipulatorSettings.RotationSettings settings = mSettings.Rotation;

                        // draw a circle strip around the specified axis
                        Primitives.DrawCircleStrip(mGraphics.GraphicsDevice, Vector3.Zero, settings.InnerRadius, 
                            settings.OuterRadius, 128, axis_transforms[axis]);
                    };

            // we will use the same manip function for all single axes. dual and triple
            // axes are not supported for rotations
            mManipFunctions[TransformationMode.Rotation][AxisFlags.X]
                = mManipFunctions[TransformationMode.Rotation][AxisFlags.Y]
                = mManipFunctions[TransformationMode.Rotation][AxisFlags.Z]
                    = delegate()
                    {
                        // get the plane perpendicular to the rotation axis, transformed by
                        // the current ITransform's world matrix
                        Plane p = GetPlane(mSelectedAxes);

                        // project rays from the start and end points of the mouse
                        Ray start_ray = GetPickRay(mInput.Start);
                        Ray end_ray = GetPickRay(mInput.End);

                        // intersect the rays with the perpendicular rotation plane
                        float? hits = start_ray.Intersects(p);
                        float? hite = end_ray.Intersects(p);

                        // exit if either of the intersections is invalid
                        if (!hits.HasValue || !hite.HasValue)
                            return;

                        // calculate the intersection position of each ray on the plane
                        Vector3 start_position = start_ray.Position + (start_ray.Direction * hits.Value);
                        Vector3 end_position = end_ray.Position + (end_ray.Direction * hite.Value);

                        // get the direction vectors of the rotation origin to the start and end points
                        Vector3 origin_to_start = Vector3.Normalize(start_position - mTransform.Translation);
                        Vector3 origin_to_end = Vector3.Normalize(end_position - mTransform.Translation);

                        Vector3 rotation_axis = GetUnitAxis(mSelectedAxes);

                        // calculate cross products of the direction vectors with the rotation axis
                        Vector3 rot_cross_start = Vector3.Normalize(Vector3.Cross(rotation_axis, origin_to_start));
                        Vector3 rot_cross_end = Vector3.Normalize(Vector3.Cross(rotation_axis, origin_to_end));

                        // calculate the cross product of the above start and end cross products
                        Vector3 start_cross_end = Vector3.Normalize(Vector3.Cross(rot_cross_start, rot_cross_end));

                        // dot the two direction vectors and get the arccos of the dot product to get
                        // the angle between them, then multiply it by the sign of the dot product
                        // of the derived cross product calculated above to obtain the direction
                        // by which we should rotate with the angle
                        float dot = Vector3.Dot(origin_to_start, origin_to_end);
                        float rotation_angle = (float)Math.Acos(dot)
                            * Math.Sign(Vector3.Dot(rotation_axis, start_cross_end));

                        // create a normalized quaternion representing the rotation from the start to end points
                        Quaternion rot = Quaternion.Normalize(Quaternion.CreateFromAxisAngle(rotation_axis, rotation_angle));
                        
                        // add the calculated rotation to the current rotation
                        mTransform.Rotation = rot * mTransform.Rotation;
                    };
        }
    }
}
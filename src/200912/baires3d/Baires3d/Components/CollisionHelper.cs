using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace b3d {
    public static class CollisionHelper {
        public static bool isVisible(BoundingSphere b) {
            if (Constants.Camera.Frustum.Contains(
                    b) != ContainmentType.Disjoint) {
                return true;
            }

            return false;
        }
        public static void isPicked(MeshComponentPart mc) {
            if (Constants.Camera.Picking && !Constants.Camera.Picked) {
                float? pepe = Constants.Camera.CurrentRay.Intersects(mc.BoundingSphere);

                if (pepe != null) {
                    //Constants.Logger.AddLog("Intersects: " + Parts[i].Name + ", GOING DEEPER!!!!!!!");

                    for (int x = 0; x < mc.Indices.Length; x += 3) {
                        float? res = isPickedTriangle(
                            Constants.Camera.CurrentRay,
                            mc.Positions[mc.Indices[x + 2]],
                            mc.Positions[mc.Indices[x + 1]],
                            mc.Positions[mc.Indices[x]]
                            );

                        if (res != null) {
                            Constants.Camera.Picked = true;
                            Constants.Camera.PickedObject = mc.Name;
                            Constants.Camera.PickedPos = mc.Positions[mc.Indices[x]];
                            Constants.Camera.PickedTriangle = new VertexPositionColor[]{
                                new VertexPositionColor(mc.Positions[mc.Indices[x +2]], Color.Lime),
                                new VertexPositionColor(mc.Positions[mc.Indices[x + 1]], Color.Lime),
                                new VertexPositionColor(mc.Positions[mc.Indices[x]], Color.Lime)};
                            break;
                        }
                    }
                }
            }
        }

        public static float? isPickedTriangle(Ray CursorRay,
                                       Vector3 P0,
                                       Vector3 P1,
                                       Vector3 P2) {
            float? IntersectionDistance;
            Vector3 IntersectionPoint, V0, V1, V2;
            float U, V, V1V1, V0V0, V0V1, V0V2, V1V2, Denominator;

            IntersectionDistance = CursorRay.Intersects(new Plane(P0, P1, P2));
            if (IntersectionDistance == null) return null;

            IntersectionPoint = CursorRay.Position +
                                CursorRay.Direction * (float)IntersectionDistance;
            V2 = IntersectionPoint - P0;
            V0 = P2 - P0;
            V1 = P1 - P0;

            V1V1 = Vector3.Dot(V1, V1);
            V1V2 = Vector3.Dot(V1, V2);
            V0V0 = Vector3.Dot(V0, V0);
            V0V1 = Vector3.Dot(V0, V1);
            V0V2 = Vector3.Dot(V0, V2);

            Denominator = V0V0 * V1V1 - V0V1 * V0V1;
            U = (V1V1 * V0V2 - V0V1 * V1V2) / Denominator;
            V = (V0V0 * V1V2 - V0V1 * V0V2) / Denominator;

            if ((U > 0) && (V > 0) && (U + V < 1))
                return IntersectionDistance;
            else return null;
        }

    }
}

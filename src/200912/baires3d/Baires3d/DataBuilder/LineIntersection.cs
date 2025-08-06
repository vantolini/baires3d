using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using b3d;
using Microsoft.Xna.Framework;

namespace b3d
{
    public enum IntersectionResult
    {
        None,
        Point,
        Segment
    }

    public struct LineIntersection
    {
        public static float perp(Vector2 u, Vector2 v)
        {
            return (float) (u.X*v.Y - u.Y*v.X);
        }

        public static float dot(Vector2 u, Vector2 v)
        {
            return (float) (u.X*v.X + u.Y*v.Y);
        }

        private static bool inSegment(Vector2 ptPoint, Vector2 ptSegmentStart, Vector2 ptSegmentEnd)
        {
            if (ptSegmentStart.X != ptSegmentEnd.X)
            {
                // S is not vertical
                if (ptSegmentStart.X <= ptPoint.X && ptPoint.X <= ptSegmentEnd.X)
                    return true;
                if (ptSegmentStart.X >= ptPoint.X && ptPoint.X >= ptSegmentEnd.X)
                    return true;
            }
            else
            {
                // S is vertical, so test y coordinate
                if (ptSegmentStart.Y <= ptPoint.Y && ptPoint.Y <= ptSegmentEnd.Y)
                    return true;
                if (ptSegmentStart.Y >= ptPoint.Y && ptPoint.Y >= ptSegmentEnd.Y)
                    return true;
            }

            return false;
        }

        public IntersectionResult LineStringIntersection(Vector2 ptLine1Start, Vector2 ptLine1End, Vector2 ptLine2Start,
                                                         Vector2 ptLine2End)
        {
            const float SMALL_NUM = 0.00000001F;


            Vector2 u = ptLine1End - ptLine1Start;
            Vector2 v = ptLine2End - ptLine2Start;
            Vector2 w = ptLine1Start - ptLine2Start;
            float D = perp(u, v);

            // test if they are parallel (includes either being a point)
            if (Math.Abs(D) < SMALL_NUM)
            {
                // S1 and S2 are parallel
                if (perp(u, w) != 0 || perp(v, w) != 0)
                {
                    return IntersectionResult.None; // they are NOT collinear
                }
                // they are collinear or degenerate
                // check if they are degenerate points
                float du = dot(u, u);
                float dv = dot(v, v);
                if (du == 0 && dv == 0)
                {
                    // both segments are points
                    if (ptLine1Start != ptLine2Start) // they are distinct points
                        return IntersectionResult.None;
                    //*I0 = S1.P0;                // they are the same point
                    return IntersectionResult.Point;
                }
                if (du == 0)
                {
                    // S1 is a single point
                    if (!inSegment(ptLine1Start, ptLine2Start, ptLine2End)) // but is not in S2
                        return IntersectionResult.None;
                    //*I0 = S1.P0;
                    return IntersectionResult.Point;
                }
                if (dv == 0)
                {
                    // S2 a single point
                    if (!inSegment(ptLine2Start, ptLine1Start, ptLine1End)) // but is not in S1
                        return 0;
                    //*I0 = S2.P0;
                    return IntersectionResult.Point;
                }
                // they are collinear segments - get overlap (or not)
                float t0, t1; // endpoints of S1 in eqn for S2
                Vector2 w2 = ptLine1End - ptLine2Start;
                if (v.X != 0)
                {
                    t0 = (float) w.X/(float) v.X;
                    t1 = (float) w2.X/(float) v.X;
                }
                else
                {
                    t0 = (float) w.Y/(float) v.Y;
                    t1 = (float) w2.Y/(float) v.Y;
                }
                if (t0 > t1)
                {
                    // must have t0 smaller than t1
                    float t = t0;
                    t0 = t1;
                    t1 = t; // swap if not
                }
                if (t0 > 1 || t1 < 0)
                {
                    return IntersectionResult.None; // NO overlap
                }
                t0 = t0 < 0 ? 0 : t0; // clip to min 0
                t1 = t1 > 1 ? 1 : t1; // clip to max 1
                if (t0 == t1)
                {
                    // intersect is a point
                    //*I0 = S2.P0 + t0 * v;
                    return IntersectionResult.Point;
                }

                // they overlap in a valid subsegment
                //*I0 = S2.P0 + t0 * v;
                //*I1 = S2.P0 + t1 * v;
                return IntersectionResult.Segment;
            }

            // the segments are skew and may intersect in a point
            // get the intersect parameter for S1
            float sI = perp(v, w)/D;
            if (sI < 0 || sI > 1) // no intersect with S1
                return IntersectionResult.None;

            // get the intersect parameter for S2
            float tI = perp(u, w)/D;
            if (tI < 0 || tI > 1) // no intersect with S2
                return IntersectionResult.None;

            //*I0 = S1.P0 + sI * u;               // compute S1 intersect point
            return IntersectionResult.Point;
        }
    }
}
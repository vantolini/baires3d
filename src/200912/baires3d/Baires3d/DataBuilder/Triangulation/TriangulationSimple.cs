using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace b3d
{
    public class TriangulationSimple
    {
        // From Wikipedia:
        // One way to triangulate a simple polygon is by using the assertion that any simple polygon
        // without holes has at least two so called 'ears'. An ear is a triangle with two sides on the edge
        // of the polygon and the other one completely inside it. The algorithm then consists of finding
        // such an ear, removing it from the polygon (which results in a new polygon that still meets
        // the conditions) and repeating until there is only one triangle left.

        // the algorithm here aims for simplicity over performance. there are other, more performant
        // algorithms that are much more complex.

        // convert a triangle to a list of triangles. each triangle is represented by a PointF array of length 3.
        public static List<PointF[]> Triangulate(Polygon poly)
        {
            List<PointF[]> triangles = new List<PointF[]>(); // accumulate the triangles here
            // keep clipping ears off of poly until only one triangle remains
            while (poly.PtListOpen.Count > 3) // if only 3 points are left, we have the final triangle
            {
                int midvertex = FindEar(poly); // find the middle vertex of the next "ear"
                triangles.Add(new PointF[]
                                  {poly.PtList[midvertex - 1], poly.PtList[midvertex], poly.PtList[midvertex + 1]});
                // create a new polygon that clips off the ear; i.e., all vertices but midvertex
                List<PointF> newPts = new List<PointF>(poly.PtList);
                newPts.RemoveAt(midvertex); // clip off the ear
                poly = new Polygon(newPts); // poly now has one less point
            }
            // only a single triangle remains, so add it to the triangle list
            triangles.Add(poly.PtListOpen.ToArray());
            return triangles;
        }

        // find an ear (always a triangle) of the polygon and return the index of the middle (second) vertex in the ear
        public static int FindEar(Polygon poly)
        {
            for (int i = 0; i < poly.PtList.Count - 2; i++)
            {
                if (poly.VertexType(i + 1) == PolygonType.Convex)
                {
                    // get the three points of the triangle we are about to test
                    PointF a = poly.PtList[i];
                    PointF b = poly.PtList[i + 1];
                    PointF c = poly.PtList[i + 2];
                    bool foundAPointInTheTriangle = false;
                        // see if any of the other points in the polygon are in this triangle
                    for (int j = 0; j < poly.PtListOpen.Count; j++)
                        // don't check the last point, which is a duplicate of the first
                    {
                        if (j != i && j != i + 1 && j != i + 2 && PointInTriangle(poly.PtList[j], a, b, c))
                            foundAPointInTheTriangle = true;
                    }
                    if (!foundAPointInTheTriangle)
                        // the middle point of this triangle is convex and none of the other points in the polygon are in this triangle, so it is an ear
                        return i + 1; // EXITING HERE!
                }
            }
            throw new ApplicationException("Improperly formed polygon");
        }

        // return true if point p is inside the triangle a,b,c
        public static bool PointInTriangle(PointF p, PointF a, PointF b, PointF c)
        {
            // three tests are required.
            // if p and c are both on the same side of the line a,b
            // and p and b are both on the same side of the line a,c
            // and p and a are both on the same side of the line b,c
            // then p is inside the triangle, o.w., not
            return PointsOnSameSide(p, a, b, c) && PointsOnSameSide(p, b, a, c) && PointsOnSameSide(p, c, a, b);
        }

        // if the two points p1 and p2 are both on the same side of the line a,b, return true
        private static bool PointsOnSameSide(PointF p1, PointF p2, PointF a, PointF b)
        {
            // these are probably the most interesting three lines of code in the algorithm (probably because I don't fully understand them)
            // the concept is nicely described at http://www.blackpawn.com/texts/pointinpoly/default.html
            float cp1 = CrossProduct(VSub(b, a), VSub(p1, a));
            float cp2 = CrossProduct(VSub(b, a), VSub(p2, a));
            return (cp1*cp2) >= 0; // they have the same sign if on the same side of the line
        }

        // subtract the vector (point) b from the vector (point) a
        private static PointF VSub(PointF a, PointF b)
        {
            return new PointF(a.X - b.X, a.Y - b.Y);
        }

        // find the cross product of two x,y vectors, which is always a single value, z, representing the three dimensional vector (0,0,z)
        private static float CrossProduct(PointF p1, PointF p2)
        {
            return (p1.X*p2.Y) - (p1.Y*p2.X);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace b3d
{
    //Useful definititions. Polygons may be characterized by their degree of convexity:
    //Convex: any line drawn through the polygon (and not tangent to an edge or corner) meets its boundary exactly twice. 
    //Non-convex: a line may be found which meets its boundary more than twice. 
    //Simple: the boundary of the polygon does not cross itself. All convex polygons are simple. 
    //Concave: Non-convex and simple. 
    //Star-shaped: the whole interior is visible from a single point, without crossing any edge. The polygon must be simple, and may be convex or concave. 
    //Self-intersecting: the boundary of the polygon crosses itself. 

    //This class deals with Simple polygons only, either Concave or Convex.

    public enum PolygonType
    {
        Convex,
        Concave
    }

    public class Polygon
    {
        public readonly List<PointF> PtList;
                                     // the points making up the Polygon; guaranteed to be Closed, such that the last point is the same as the first

        public readonly List<PointF> PtListOpen;
                                     // the same PtList, but with the last point removed, i.e., an Open polygon

        public readonly float Area;
        public readonly PolygonType Type;

        // create a new polygon with a list of points (which won't change)
        public Polygon(List<PointF> ptlist)
        {
            PolyClose(ptlist);
                // make sure the polygon is closed by duplicating the first point to the end, if necessary
            PtList = ptlist;
            PtListOpen = new List<PointF>(PtList);
            PtListOpen.RemoveAt(PtList.Count - 1); // remove the last point, which is a duplicate of the first
            Area = PolyArea(PtList);
            Type = PolyType(PtList, Area);
        }

        // create a new pointlist that closes the polygon by adding the first point at the end
        private static void PolyClose(List<PointF> pts)
        {
            if (!IsPolyClosed(pts)) pts.Add(pts[0]); // add a point at the end if it is not already closed
        }

        // find the area of a polygon. if the vertices are ordered clockwise, the area is negative, o.w. positive, but
        // the absolute value is the same in either case. (Remember that, in System.Drawing, Y is positive down.
        private static float PolyArea(IList<PointF> ptlist)
        {
            float area = 0;
            for (int i = 0; i < ptlist.Count - 1; i++)
                area += ptlist[i].X*ptlist[i + 1].Y - ptlist[i + 1].X*ptlist[i].Y;
            return area/2;
        }

        // find the type, Concave or Convex, of a Simple polygon
        private static PolygonType PolyType(List<PointF> ptlist, float area)
        {
            int polysign = Math.Sign(area);
            List<PointF> lst = new List<PointF>();


            for (int i = 0; i < ptlist.Count - 2; i++)
            {
                lst.Clear();
                lst.Add(ptlist[i]);
                lst.Add(ptlist[i + 1]);
                lst.Add(ptlist[i + 2]);


                if (Math.Sign((double) PolyArea(lst)) != polysign) return PolygonType.Concave;
            }
            return PolygonType.Convex;
        }

        // find the type of a specific vertex in a polygon, either Concave or Convex.
        public PolygonType VertexType(int vertexNo)
        {
            Polygon triangle;
            List<PointF> lst = new List<PointF>();
            if (vertexNo == 0)
            {
                lst.Add(PtList[PtList.Count - 2]);
                lst.Add(PtList[0]);
                lst.Add(PtList[1]);
                triangle = new Polygon(lst);


                // the polygon is always closed so the last point is the same as the first
            }
            else
            {
                lst.Add(PtList[vertexNo - 1]);
                lst.Add(PtList[vertexNo]);
                lst.Add(PtList[vertexNo + 1]);
                triangle = new Polygon(lst);
            }

            if (Math.Sign(triangle.Area) == Math.Sign(this.Area))
                return PolygonType.Convex;
            else
                return PolygonType.Concave;
        }

        private static bool IsPolyClosed(List<PointF> pts)
        {
            return IsSamePoint(pts[0], pts[pts.Count - 1]);
        }

        private static bool IsSamePoint(PointF pt1, PointF pt2)
        {
            return pt1.X == pt2.X && pt1.Y == pt2.Y;
        }
    }
}
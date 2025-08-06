using System;

namespace baires3d {
    public class RoofVertex
    {
        double x;
        double y;
        double angle;
        double speed;
        bool ear;
        RoofVertex prev;
        RoofVertex next;

        public RoofVertex(double d, double d1, double d2, double d3)
        {
            x = d;
            y = d1;
            angle = d2;
            speed = d3;
        }

        public RoofVertex(double d, double d1, double d2)
        {
            RoofVertex(d, d1, d2, 0.0D);
        }

        public RoofVertex(double d, double d1)
        {
            RoofVertex(d, d1, 0.0D, 0.0D);
        }

        public RoofVertex(RealPoint realpoint, double d)
        {
            RoofVertex(realpoint.x, realpoint.y, d);
        }

        public bool equals(Object obj)
        {
            //System.out.println("in Vertex.equals(Object)");
            //if(obj instanceof Vertex)
            //    return distance((Vertex)obj) < 1.0D;
            //else
            //    return false;
        }

        public double distance(RoofVertex vertex)
        {
            double d = x - vertex.x;
            double d1 = y - vertex.y;
            return Math.Sqrt(d * d + d1 * d1);
        }

        public String toString()
        {
            String s = "(" + x + ", " + y + ";" + angle + ")";
            return s;
        }


    public static double EQUALITY_EPS = 1D;
}


}
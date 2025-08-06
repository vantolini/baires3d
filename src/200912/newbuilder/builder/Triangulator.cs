using System;
using System.Collections.Generic;
using Poly2Tri;
using System.Linq;
using System.Collections;
using EarcutNet;

namespace Builder {

    public class Triangulate {
        private List<Vector2D> m_points = new List<Vector2D>();

        public Triangulate(Vector2D[] points) {
            m_points = new List<Vector2D>(points);
        }

        public int[] doTriangulate() {
            List<int> indices = new List<int>();

            int n = m_points.Count;
            if (n < 3)
                return indices.ToArray();

            int[] V = new int[n];
            if (Area() > 0) {
                for (int v = 0; v < n; v++)
                    V[v] = v;
            }
            else {
                for (int v = 0; v < n; v++)
                    V[v] = (n - 1) - v;
            }

            int nv = n;
            int count = 2 * nv;
            for (int m = 0, v = nv - 1; nv > 2; ) {
                if ((count--) <= 0)
                    return indices.ToArray();

                int u = v;
                if (nv <= u)
                    u = 0;
                v = u + 1;
                if (nv <= v)
                    v = 0;
                int w = v + 1;
                if (nv <= w)
                    w = 0;

                if (Snip(u, v, w, nv, V)) {
                    int a, b, c, s, t;
                    a = V[u];
                    b = V[v];
                    c = V[w];
                    indices.Add(a);
                    indices.Add(b);
                    indices.Add(c);
                    m++;
                    for (s = v, t = v + 1; t < nv; s++, t++)
                        V[s] = V[t];
                    nv--;
                    count = 2 * nv;
                }
            }

            indices.Reverse();
            return indices.ToArray();
        }

        private double Area() {
            int n = m_points.Count;
            double A = 0.0;
            for (int p = n - 1, q = 0; q < n; p = q++) {
                Vector2D pval = m_points[p];
                Vector2D qval = m_points[q];
                A += pval.X * qval.Y - qval.X * pval.Y;
            }
            return (A * 0.5);
        }

        private bool Snip(int u, int v, int w, int n, int[] V) {
            int p;
            Vector2D A = m_points[V[u]];
            Vector2D B = m_points[V[v]];
            Vector2D C = m_points[V[w]];
            if (double.Epsilon > (((B.X - A.X) * (C.Y - A.Y)) - ((B.Y - A.Y) * (C.X - A.X))))
                return false;
            for (p = 0; p < n; p++) {
                if ((p == u) || (p == v) || (p == w))
                    continue;
                Vector2D P = m_points[V[p]];
                if (InsideTriangle(A, B, C, P))
                    return false;
            }
            return true;
        }

        private bool InsideTriangle(Vector2D A, Vector2D B, Vector2D C, Vector2D P) {
            double ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
            double cCROSSap, bCROSScp, aCROSSbp;

            ax = C.X - B.X; ay = C.Y - B.Y;
            bx = A.X - C.X; by = A.Y - C.Y;
            cx = B.X - A.X; cy = B.Y - A.Y;
            apx = P.X - A.X; apy = P.Y - A.Y;
            bpx = P.X - B.X; bpy = P.Y - B.Y;
            cpx = P.X - C.X; cpy = P.Y - C.Y;

            aCROSSbp = ax * bpy - ay * bpx;
            cCROSSap = cx * apy - cy * apx;
            bCROSScp = bx * cpy - by * cpx;

            return ((aCROSSbp >= 0.0) && (bCROSScp >= 0.0) && (cCROSSap >= 0.0));
        }
    }

    public class PolygonTriangulator
    {
        /// <summary>
        /// Calculate list of convex polygons or triangles.
        /// </summary>
        /// <param name="Polygon">Input polygon without self-intersections (it can be checked with SelfIntersection().</param>
        /// <param name="triangulate">true: splitting on triangles; false: splitting on convex polygons.</param>
        /// <returns></returns>
        public static List<List<Vector3>> Triangulate(List<Vector3> Polygon, bool triangulate = false)
        {
            var result = new List<List<Vector3>>();
            var tempPolygon = new List<Vector3>(Polygon);
            var convPolygon = new List<Vector3>();

            int begin_ind = 0;
            int cur_ind;
            int begin_ind1;
            int N = Polygon.Count;
            int Range;

            if (Square(tempPolygon) < 0)
                tempPolygon.Reverse();

            while (N >= 3)
            {
                while ((PMSquare(tempPolygon[begin_ind], tempPolygon[(begin_ind + 1) % N],
                          tempPolygon[(begin_ind + 2) % N]) < 0) ||
                          (Intersect(tempPolygon, begin_ind, (begin_ind + 1) % N, (begin_ind + 2) % N) == true))
                {
                    begin_ind++;
                    begin_ind %= N;
                }
                cur_ind = (begin_ind + 1) % N;
                convPolygon.Add(tempPolygon[begin_ind]);
                convPolygon.Add(tempPolygon[cur_ind]);
                convPolygon.Add(tempPolygon[(begin_ind + 2) % N]);

                if (triangulate == false)
                {
                    begin_ind1 = cur_ind;
                    while ((PMSquare(tempPolygon[cur_ind], tempPolygon[(cur_ind + 1) % N],
                                    tempPolygon[(cur_ind + 2) % N]) > 0) && ((cur_ind + 2) % N != begin_ind))
                    {
                        if ((Intersect(tempPolygon, begin_ind, (cur_ind + 1) % N, (cur_ind + 2) % N) == true) ||
                            (PMSquare(tempPolygon[begin_ind], tempPolygon[(begin_ind + 1) % N],
                                      tempPolygon[(cur_ind + 2) % N]) < 0))
                            break;
                        convPolygon.Add(tempPolygon[(cur_ind + 2) % N]);
                        cur_ind++;
                        cur_ind %= N;
                    }
                }

                Range = cur_ind - begin_ind;
                if (Range > 0)
                {
                    tempPolygon.RemoveRange(begin_ind + 1, Range);
                }
                else
                {
                    tempPolygon.RemoveRange(begin_ind + 1, N - begin_ind - 1);
                    tempPolygon.RemoveRange(0, cur_ind + 1);
                }
                N = tempPolygon.Count;
                begin_ind++;
                begin_ind %= N;

                result.Add(convPolygon);
            }

            return result;
        }

        public static int SelfIntersection(List<Vector3> polygon)
        {
            if (polygon.Count < 3)
                return 0;
            int High = polygon.Count - 1;
            Vector3 O = new Vector3();
            int i;
            for (i = 0; i < High; i++)
            {
                for (int j = i + 2; j < High; j++)
                {
                    if (LineIntersect(polygon[i], polygon[i + 1],
                                      polygon[j], polygon[j + 1], ref O) == 1)
                        return 1;
                }
            }
            for (i = 1; i < High - 1; i++)
                if (LineIntersect(polygon[i], polygon[i + 1], polygon[High], polygon[0], ref O) == 1)
                    return 1;
            return -1;
        }

        public static float Square(List<Vector3> polygon)
        {
            float S = 0;
            if (polygon.Count >= 3)
            {
                for (int i = 0; i < polygon.Count - 1; i++)
                    S += PMSquare((Vector3)polygon[i], (Vector3)polygon[i + 1]);
                S += PMSquare((Vector3)polygon[polygon.Count - 1], (Vector3)polygon[0]);
            }
            return S;
        }

        public int IsConvex(List<Vector3> Polygon)
        {
            if (Polygon.Count >= 3)
            {
                if (Square(Polygon) > 0)
                {
                    for (int i = 0; i < Polygon.Count - 2; i++)
                        if (PMSquare(Polygon[i], Polygon[i + 1], Polygon[i + 2]) < 0)
                            return -1;
                    if (PMSquare(Polygon[Polygon.Count - 2], Polygon[Polygon.Count - 1], Polygon[0]) < 0)
                        return -1;
                    if (PMSquare(Polygon[Polygon.Count - 1], Polygon[0], Polygon[1]) < 0)
                        return -1;
                }
                else
                {
                    for (int i = 0; i < Polygon.Count - 2; i++)
                        if (PMSquare(Polygon[i], Polygon[i + 1], Polygon[i + 2]) > 0)
                            return -1;
                    if (PMSquare(Polygon[Polygon.Count - 2], Polygon[Polygon.Count - 1], Polygon[0]) > 0)
                        return -1;
                    if (PMSquare(Polygon[Polygon.Count - 1], Polygon[0], Polygon[1]) > 0)
                        return -1;
                }
                return 1;
            }
            return 0;
        }

        static bool Intersect(List<Vector3> polygon, int vertex1Ind, int vertex2Ind, int vertex3Ind)
        {
            float s1, s2, s3;
            for (int i = 0; i < polygon.Count; i++)
            {
                if ((i == vertex1Ind) || (i == vertex2Ind) || (i == vertex3Ind))
                    continue;
                s1 = PMSquare(polygon[vertex1Ind], polygon[vertex2Ind], polygon[i]);
                s2 = PMSquare(polygon[vertex2Ind], polygon[vertex3Ind], polygon[i]);
                if (((s1 < 0) && (s2 > 0)) || ((s1 > 0) && (s2 < 0)))
                    continue;
                s3 = PMSquare(polygon[vertex3Ind], polygon[vertex1Ind], polygon[i]);
                if (((s3 >= 0) && (s2 >= 0)) || ((s3 <= 0) && (s2 <= 0)))
                    return true;
            }
            return false;
        }

        static float PMSquare(Vector3 p1, Vector3 p2)
        {
            return (p2.X * p1.Y - p1.X * p2.Y);
        }

        static float PMSquare(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            return (p3.X - p1.X) * (p2.Y - p1.Y) - (p2.X - p1.X) * (p3.Y - p1.Y);
        }

        static int LineIntersect(Vector3 A1, Vector3 A2, Vector3 B1, Vector3 B2, ref Vector3 O)
        {
            float a1 = A2.Y - A1.Y;
            float b1 = A1.X - A2.X;
            float d1 = -a1 * A1.X - b1 * A1.Y;
            float a2 = B2.Y - B1.Y;
            float b2 = B1.X - B2.X;
            float d2 = -a2 * B1.X - b2 * B1.Y;
            float t = a2 * b1 - a1 * b2;

            if (t == 0)
                return -1;

            O.Y = (a1 * d2 - a2 * d1) / t;
            O.X = (b2 * d1 - b1 * d2) / t;

            if (A1.X > A2.X)
            {
                if ((O.X < A2.X) || (O.X > A1.X))
                    return 0;
            }
            else
            {
                if ((O.X < A1.X) || (O.X > A2.X))
                    return 0;
            }

            if (A1.Y > A2.Y)
            {
                if ((O.Y < A2.Y) || (O.Y > A1.Y))
                    return 0;
            }
            else
            {
                if ((O.Y < A1.Y) || (O.Y > A2.Y))
                    return 0;
            }

            if (B1.X > B2.X)
            {
                if ((O.X < B2.X) || (O.X > B1.X))
                    return 0;
            }
            else
            {
                if ((O.X < B1.X) || (O.X > B2.X))
                    return 0;
            }

            if (B1.Y > B2.Y)
            {
                if ((O.Y < B2.Y) || (O.Y > B1.Y))
                    return 0;
            }
            else
            {
                if ((O.Y < B1.Y) || (O.Y > B2.Y))
                    return 0;
            }

            return 1;
        }
    }



    public static class Triangulator {
        //List<Vector3> pos2 = Engine.Triangulate(posss);

        public static List<Vector3> Triangulate2(List<Vector3> points)
        {
            List<Vector3> pols = new List<Vector3>();
            List<double> data = new List<double>();

            List<int> holes = new List<int>();
            foreach (var point in points)
            {
                data.Add(point.X);
                data.Add(point.Z);
            }

            List<int> res = Earcut.Tessellate(data, holes);
            foreach(var re in res)
            {
                pols.Add(points[re]);
            }

            return pols;
        }
        public static List<Vector3> Triangulate(List<Vector3> points) {
            return Triangulator.Triangulate2(points);

            if (points.Count == 3) {
                //points.Reverse();
                return points;
            }
            if (points.Count < 3) {
                return points;
            }
            /*
            Hashtable hashtable = new Hashtable();

            foreach (var point in points)
            {
                if (hashtable.ContainsKey(point.X + "|" + point.Z))
                {
                    Console.WriteLine("DUPLICATE!!! " + point.ToString());
                }else { 
                    hashtable.Add(point.X + "|" + point.Z, point);
                }
            }

            points.Clear();

            foreach (System.Collections.DictionaryEntry point in hashtable)
            {
                points.Add((Vector3)point.Value);
            }

            


           List<Vector2D> vecs = new List<Vector2D>();
           foreach (var point in points) {
               vecs.Add(new Vector2D(point.X, point.Z));
           }

           Triangulate Triangulatr = new Triangulate(vecs.ToArray());
           int[] res = Triangulatr.doTriangulate();
           List<Vector3> retpoly = new List<Vector3>();


           foreach (int ress in res) {
               float hgt = getHeight(points, points[ress].X, points[ress].Z);
               retpoly.Add(new Vector3(points[ress].X, hgt, points[ress].Z));
           }
           //retpoly.Reverse();




           if (retpoly.Count > 0) {
               return retpoly;
           }

            */


            List<Vector3> pols = null;

            //List<List<Vector3>> polst = PolygonTriangulator.Triangulate(points);


            if (true)
            {
                var tess = new LibTessDotNet.Tess();

                var contour = new LibTessDotNet.ContourVertex[points.Count];
                for (int i = 0; i < points.Count; i++)
                {
                    // NOTE : Z is here for convenience if you want to keep a 3D vertex position throughout the tessellation process but only X and Y are important.
                    contour[i].Position = new LibTessDotNet.Vec3 { X = points[i].X, Y = points[i].Z, Z = 0.0f };
                    // Data can contain any per-vertex data, here a constant color.
                    contour[i].Data = Color.Azure;
                }
                // Add the contour with a specific orientation, use "Original" if you want to keep the input orientation.
                tess.AddContour(contour, LibTessDotNet.ContourOrientation.Clockwise);

                // Tessellate!
                // The winding rule determines how the different contours are combined together.
                // See http://www.glprogramming.com/red/chapter11.html (section "Winding Numbers and Winding Rules") for more information.
                // If you want triangles as output, you need to use "Polygons" type as output and 3 vertices per polygon.
                tess.Tessellate(LibTessDotNet.WindingRule.EvenOdd, LibTessDotNet.ElementType.Polygons, 3, VertexCombine);
                int numTriangles = tess.ElementCount;

                List<Vector3> pols3 = new List<Vector3>();



                for (int i = 0; i < numTriangles; i++)
                {
                    var v0 = tess.Vertices[tess.Elements[i * 3]].Position;
                    var v1 = tess.Vertices[tess.Elements[i * 3 + 1]].Position;
                    var v2 = tess.Vertices[tess.Elements[i * 3 + 2]].Position;
                    float hgt2 = getHeight(points, v0.X, v0.Y);
                    pols3.Add(new Vector3(v0.X, hgt2, v0.Y));

                    float hgt3 = getHeight(points, v1.X, v1.Y);
                    pols3.Add(new Vector3(v1.X, hgt3, v1.Y));

                    float hgt4 = getHeight(points, v2.X, v2.Y);
                    pols3.Add(new Vector3(v2.X, hgt4, v2.Y));


                    //Console.WriteLine("#{0} ({1:F1},{2:F1}) ({3:F1},{4:F1}) ({5:F1},{6:F1})", i, v0.X, v0.Y, v1.X, v1.Y, v2.X, v2.Y);
                }
                if (pols3.Count > 0)
                {
                    return pols3;
                }
                else
                {
                    Console.WriteLine("NOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO");
                    return null;
                }


            }
            else
            {

                pols = TriangulateExtra(points);

                List<Vector3> pols2 = new List<Vector3>();
                foreach(Vector3 tri in pols){
                    float hgt2 = getHeight(points, tri.X, tri.Z);
                    pols2.Add(new Vector3(tri.X, hgt2, tri.Z));
                }
                //pols2.Reverse();
                if (pols2.Count > 0) {
                    return pols2;
                }

                //points.Reverse();
                pols = TriangulateExtra(points);
                if (pols.Count == 0) {
                    //pols.Reverse();
                    return points;
                }
                else {
                    return pols;// null;
                }


            }


        }
        private static object VertexCombine(LibTessDotNet.Vec3 position, object[] data, float[] weights)
        {
            // Fetch the vertex data.
            var colors = new Color[] { (Color)data[0], (Color)data[1], (Color)data[2], (Color)data[3] };
            // Interpolate with the 4 weights.
            var rgba = new float[] {
                (float)colors[0].R * weights[0] + (float)colors[1].R * weights[1] + (float)colors[2].R * weights[2] + (float)colors[3].R * weights[3],
                (float)colors[0].G * weights[0] + (float)colors[1].G * weights[1] + (float)colors[2].G * weights[2] + (float)colors[3].G * weights[3],
                (float)colors[0].B * weights[0] + (float)colors[1].B * weights[1] + (float)colors[2].B * weights[2] + (float)colors[3].B * weights[3],
                (float)colors[0].A * weights[0] + (float)colors[1].A * weights[1] + (float)colors[2].A * weights[2] + (float)colors[3].A * weights[3]
            };
            // Return interpolated data for the new vertex.
            return Color.Aquamarine;//FromArgb((int)rgba[3], (int)rgba[0], (int)rgba[1], (int)rgba[2]);
        }

        public static List<Vector3> TriangulateExtra(List<Vector3> originalPoints) {
            List<Vector3> points = new List<Vector3>();
            P2T.CreateContext(TriangulationAlgorithm.DTSweep);
            if (originalPoints.Count > 3) {
                PolygonPoint[] polypoints = new PolygonPoint[originalPoints.Count];
                /*if (points.Length > 200){
                    Console.WriteLine("Triangulating " + points.Length + " vertices...");
                }*/
                for (int ix = 0; ix < polypoints.Length; ix++) {
                    polypoints[ix] = new PolygonPoint(originalPoints[ix].X, originalPoints[ix].Z);
                }

                Polygon polygon = new Polygon(polypoints);
                try {
                    P2T.Triangulate(polygon);

                   // feature.numVertices = polygon.Triangles.Count * 3;

                    float numPrimitives = polygon.Triangles.Count;
                    //feature.Techo = new List<Vector3>();

                   // float alturita = feature.Altura;

                    for (int ix = 0; ix < numPrimitives; ix++) {
                        points.Add(
                            new Vector3(
                                (float)polygon.Triangles[ix].Points[0].X,//polygon.Triangles[ix].X,
                                0,
                                //getHeight(feature.Points, (float)polygon.Triangles[ix].Points[0].X, (float)polygon.Triangles[ix].Points[0].Y) + alturita,
                                (float)polygon.Triangles[ix].Points[0].Y//polygon.Triangles[ix].Points[0].X;
                                )
                            );
                        points.Add(
                            new Vector3(
                                (float)polygon.Triangles[ix].Points[1].X,//polygon.Triangles[ix].X,
                                0, //getHeight(feature.Points, (float)polygon.Triangles[ix].Points[1].X, (float)polygon.Triangles[ix].Points[1].Y) + alturita,
                               (float)polygon.Triangles[ix].Points[1].Y//polygon.Triangles[ix].Points[0].X;
                                )
                            );

                        points.Add(
                           new Vector3(
                              (float)polygon.Triangles[ix].Points[2].X,//polygon.Triangles[ix].X,
                               0, //getHeight(feature.Points, (float)polygon.Triangles[ix].Points[2].X, (float)polygon.Triangles[ix].Points[2].Y) + alturita,
                               (float)polygon.Triangles[ix].Points[2].Y//polygon.Triangles[ix].Points[0].X;
                               )
                           );
                    }
                    /*
                    //feature.Techo.Reverse();
                    int[] newIndx = new int[feature.Techo.Count];
                    for (int ix = 0; ix < feature.Techo.Count; ix++) {
                        newIndx[ix] = ix;
                    }

                    feature.IndicesTecho = newIndx;*/
                }
                catch (Exception exe) {
                    Console.WriteLine("ERROR " + exe.ToString());
                    //continue;
                }
                //feature.Techo.Reverse();
            }
            else {
                if (originalPoints.Count < 3) {
                    throw new Exception();
                }
                else {
                    for (int o = 0; o < originalPoints.Count; o++) {
                        points.Add(
                           new Vector3(
                                originalPoints[o].X,//polygon.Triangles[ix].X,
                                0,
                                originalPoints[o].Z//polygon.Triangles[ix].Points[0].X;
                               )
                           );

                    }
                }

            }
            return points;
        }
        private static float getHeight(List<Vector3> points, float X, float Z) {
            for (int i = 0; i < points.Count; i++) {
                if (points[i].X == X && points[i].Z == Z) {
                    return points[i].Y;
                }
            }
            return 0;
        }
    }
}

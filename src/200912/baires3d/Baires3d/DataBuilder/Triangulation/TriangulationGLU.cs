using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tao.OpenGl;
//using Tao.FreeGlut;
using System.Runtime.InteropServices;

namespace baires3d {
    public class TriangulationGLU {
        private void e() {
            finishTesselation((pVector3[])tessList.ToArray(typeof(pVector3)));
        }
        public ArrayList primList = new ArrayList();
        ArrayList tessList = new ArrayList();
        PrimitiveType m_primitiveType = PrimitiveType.PointList;

        private void b(int which) {
            tessList.Clear();
            switch (which) {
                case 4:
                    m_primitiveType = PrimitiveType.TriangleList;
                    break;
                case 5:
                    m_primitiveType = PrimitiveType.TriangleStrip;
                    break;
                case 6:
                    m_primitiveType = PrimitiveType.TriangleFan;
                    break;
            }
            primTypes.Add(m_primitiveType);
        }

        private void r(int which) {
            
            //Console.Write("error: " + Tao.OpenGl.Gl.glerr Glu.gluErrorString(which).ToString());
        }
        private void f(System.IntPtr vertexData) {
            try {
                double[] v = new double[3];
                System.Runtime.InteropServices.Marshal.Copy(vertexData, v, 0, 3);

                pVector3 p = new pVector3(v[0], 0, v[2]);
                tessList.Add(p);
            }
            catch (Exception ex) {
                Console.Write(ex);
            }
        }

        public delegate void TessCombineCallback(
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] coordinates,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)] IntPtr[] vertexData,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)] float[] weight,
            [Out] IntPtr[] outData
        );

        /*
        private static void Combine(double[] coordinates, IntPtr[] vertexData, float[] weight, IntPtr[] dataOut) {
            double[] vertex = new double[6];
            int i;

            vertex[0] = coordinates[0];
            vertex[1] = coordinates[1];
            vertex[2] = coordinates[2];

            for(i = 3; i < 6; i++) {
                vertex[i] = weight[0] * vertexData[i] + weight[1] * vertexData[i] + weight[2] * vertexData[i] + weight[3] * vertexData[i];
            }

            dataOut = vertex;
        }*/
        /*
        public void combine(

           double[] coordinates, double[] vertexData, float[] weight, double[] dataOut) {
            double[] vertex = new double[3];

            vertex[0] = coordinates[0];
            vertex[1] = coordinates[1];
            vertex[2] = coordinates[2];
            dataOut = vertex;
        }

        */


      private static void combine(double[] coordinates, IntPtr[] ptrVertexData, float[] weight, IntPtr[] dataOut)
      {
        TessCombineVertexData[] vertexData = new TessCombineVertexData[4];
 
        for (int i = 0; i < 4; i++)
        {
          vertexData[i] = (TessCombineVertexData)Marshal.PtrToStructure(ptrVertexData[i], typeof(TessCombineVertexData));
        }
        TessCombineResult vertex = new TessCombineResult();
        vertex.x = coordinates[0];
        vertex.y = coordinates[1];
        vertex.z = coordinates[2];
        vertex.r = weight[0] * vertexData[0].r + weight[1] * vertexData[1].r + weight[2] * vertexData[2].r + weight[3] * vertexData[3].r;
        vertex.g = weight[0] * vertexData[0].g + weight[1] * vertexData[1].g + weight[2] * vertexData[2].g + weight[3] * vertexData[3].g;
        vertex.b = weight[0] * vertexData[0].b + weight[1] * vertexData[1].b + weight[2] * vertexData[2].b + weight[3] * vertexData[3].b;
 
        int sizeVertex = Marshal.SizeOf(vertex);
        IntPtr bufVert = Marshal.AllocHGlobal(sizeVertex);
        memoryFreeAfterCombine.Add(bufVert);
        Marshal.StructureToPtr(vertex, bufVert, false);

        dataOut[0] = bufVert;

      }

      private static List<IntPtr> memoryFreeAfterCombine = new List<IntPtr>();

      [System.Runtime.InteropServices.StructLayout(LayoutKind.Sequential)]
      private struct TessCombineVertexData
      {
        public double x;
        public double y;
        public double z;
        public double r;
        public double g;
        public double b;
        public double a;
      };


      [System.Runtime.InteropServices.StructLayout(LayoutKind.Sequential)]
      private struct TessCombineResult
      {
        public double x;
        public double y;
        public double z;
        public double r;
        public double g;
       public double b;
      };




        public void getTessellation(pVector2[] m_outerRing) {
           // try {

                primList.Clear();
                primTypes.Clear();

                ArrayList pointList = new ArrayList();
                for (int i = 0; i < m_outerRing.Length; i++) {
                    double[] p = new double[3];
                    p[0] = m_outerRing[i].X;
                    p[1] = 0;
                    p[2] = m_outerRing[i].Y;

                    pointList.Add(p);
                }

                //unsafe { tessCombine = this.CombineHandler; }
            /*

                Glu.GLUtesselator tess = Glu.gluNewTess();
                //
                Glu.gluTessProperty(tess, Glu.GLU_TESS_WINDING_RULE, Glu.GLU_TESS_WINDING_POSITIVE);

                Glu.gluTessCallback(tess, Glu.GLU_TESS_BEGIN, new Glu.TessBeginCallback(b));
                Glu.gluTessCallback(tess, Glu.GLU_TESS_END, new Glu.TessEndCallback(e));

                Glu.gluTessCallback(tess, Glu.GLU_TESS_ERROR, new Glu.TessErrorCallback(r));
                Glu.gluTessCallback(tess, Glu.GLU_TESS_VERTEX, new Glu.TessVertexCallback(f));
                Glu.gluTessCallback(tess, Glu.GLU_TESS_COMBINE, new Glu.TessCombineCallback(combine));
                //Glu.gluTessCallback(tess, Glu.GLU_TESS_COMBINE,  tessCombine);

                Glu.gluTessBeginPolygon(tess, IntPtr.Zero);
                Glu.gluTessBeginContour(tess);

                for (int i = 0; i < pointList.Count - 1; i++) {
                    double[] p = (double[])pointList[i];
                    Glu.gluTessVertex(tess, p, p);
                }
                Glu.gluTessEndContour(tess);

                Glu.gluTessEndPolygon(tess);
            */
        }


        ArrayList primTypes = new ArrayList();

        private void finishTesselation(pVector3[] tesselatorList) {
            //int polygonColor = System.Drawing.Color.FromArgb(m_polygonColor.A, m_polygonColor.R, m_polygonColor.G, m_polygonColor.B).ToArgb();
            pVector3[] vertices = new pVector3[tesselatorList.Length];

            for (int i = 0; i < vertices.Length; i++) {
                pVector3 sphericalPoint = tesselatorList[i];

                double terrainHeight = 0;
                
                //vertices[i].Color = polygonColor;
                vertices[i].X = sphericalPoint.X;
                vertices[i].Y = 0;
                vertices[i].Z = sphericalPoint.Z;
            }



            pVector3[] vertos = new pVector3[1];
            switch ((PrimitiveType)primTypes[0]) {
                case PrimitiveType.TriangleFan:

                    vertos = new pVector3[(tesselatorList.Length - 2) * 3];
                    int eex = 0;



                    for (int x = 0; x < (tesselatorList.Length - 2); x++) {
                        vertos[eex] = vertices[0]; eex++;
                        vertos[eex] = vertices[x + 1]; eex++;
                        vertos[eex] = vertices[x + 2]; eex++;
                    }

                    break;
                case PrimitiveType.TriangleList:

                    vertos = new pVector3[tesselatorList.Length];


                    for (int x = 0; x < tesselatorList.Length; x++) {
                        vertos[x] = vertices[x];
                    }

                    break;
                case PrimitiveType.TriangleStrip:



                    int src_pos = 0, dst_pos = 0;
                    pVector3 a, b, c;
                    vertos = new pVector3[(vertices.Length - 2) * 3];

                    a = vertices[src_pos++];
                    b = vertices[src_pos++];
                    for (int i = 0; i < vertices.Length - 2; i++) {
                        c = vertices[src_pos++];
                        if ((i & 1) == 0) {
                            vertos[dst_pos++] = a;
                            vertos[dst_pos++] = b;
                            vertos[dst_pos++] = c;
                        }
                        else {
                            vertos[dst_pos++] = b;
                            vertos[dst_pos++] = a;
                            vertos[dst_pos++] = c;

                        }
                        a = b;
                        b = c;
                    }


                    break;

                default:
                    Console.WriteLine(((PrimitiveType)primTypes[0]).ToString());
                    break;

            }

            primList.Add(vertos);
        }
    }
}

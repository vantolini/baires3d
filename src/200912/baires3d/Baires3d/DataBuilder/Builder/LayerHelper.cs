using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FileMode=System.IO.FileMode;
using FileStream=System.IO.FileStream;
using MemoryStream=System.IO.MemoryStream;
using System.IO;

namespace b3d
{
    public static class LayerHelper
    {
        public static float startingPoint = 2;

        public static float divisor = 200f;

        public static Vector2 textureUpperLeft = new Vector2(0.0f, 0.0f);
        public static Vector2 textureUpperRight = new Vector2(1.0f, 0.0f);
        public static Vector2 textureLowerLeft = new Vector2(0.0f, 1.0f);
        public static Vector2 textureLowerRight = new Vector2(1.0f, 1.0f);
        public static Random r = new Random(23051983);

        public static void SerializeIndex(string name, string path, string[] layers, byte layerType)
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter BinaryWriter = new BinaryWriter(memoryStream);
            BinaryWriter.Write(LayerFields.LayerGroup);
            BinaryWriter.Write(layerType);
            BinaryWriter.Write(layers.Length);

            for (int xx = 0; xx < layers.Length; xx++)
            {
                BinaryWriter.Write(layers[xx]);
            }

            BinaryWriter.Flush();


            FileStream fs = new FileStream(path + name + ".bin", FileMode.Create);
            fs.Write(memoryStream.GetBuffer(), 0, Convert.ToInt32(memoryStream.Position));
            fs.Flush();
            fs.Close();
            memoryStream.Close();
            BinaryWriter.Close();
        }


        public static void GenerateCoords(List<Feature> Features)
        {
            for (int i = 0; i < Features.Count; i++)
            {
                int[] tmpindx = Features[i].IndicesTecho;
                Features[i].Indices = new int[tmpindx.Length + Features[i].Paredes.Length];
                for (int x = 0; x < tmpindx.Length; x++)
                {
                    Features[i].Indices[x] = tmpindx[x];
                }
                //Features[i].Paredes.Reverse();

                int[] lala = new int[Features[i].Paredes.Length];

                for (int x = 0; x < Features[i].Paredes.Length; x++)
                {
                    lala[x] = x;
                }

                for (int x = 0; x < (tmpindx.Length + Features[i].Paredes.Length); x++)
                {
                    Features[i].Indices[x] = x;
                }

                Features[i].Vertices =
                    new PositionNormalTextureColor[Features[i].Techo.Count + Features[i].Paredes.Length];

                for (int g = 0; g < Features[i].Techo.Count; g++)
                {
                    Features[i].Vertices[g] =
                        new PositionNormalTextureColor(
                            new Vector3(
                                (float) Features[i].Techo[g].X,
                                (float) Features[i].Techo[g].Y,
                                (float) Features[i].Techo[g].Z
                                )
                            );
                }

                for (int g = 0; g < Features[i].Paredes.Length; g++)
                {
                    Features[i].Vertices[Features[i].Techo.Count + g] =
                        new PositionNormalTextureColor(
                            new Vector3(
                                (float) Features[i].Paredes[g].X,
                                (float) Features[i].Paredes[g].Y,
                                (float) Features[i].Paredes[g].Z
                                )
                            );
                }
            }


            for (int i = 0; i < Features.Count; i++)
            {
                for (int g = 0; g < Features[i].Vertices.Length; g++)
                    Features[i].Vertices[g].Normal = new Vector3(0, 0, 0);

                for (int g = 0; g < Features[i].Indices.Length/3; g++)
                {
                    Vector3 firstvec = Features[i].Vertices[Features[i].Indices[g*3 + 1]].Position -
                                       Features[i].Vertices[Features[i].Indices[g*3]].Position;
                    Vector3 secondvec = Features[i].Vertices[Features[i].Indices[g*3]].Position -
                                        Features[i].Vertices[Features[i].Indices[g*3 + 2]].Position;
                    Vector3 normal = Vector3.Cross(firstvec, secondvec);
                    normal.Normalize();
                    Features[i].Vertices[Features[i].Indices[g*3]].Normal += normal;
                    Features[i].Vertices[Features[i].Indices[g*3 + 1]].Normal += normal;
                    Features[i].Vertices[Features[i].Indices[g*3 + 2]].Normal += normal;
                }
                if (Features[i].Data.Count > 8)
                {
                    if (Features[i].Data[1] == "plaza")
                    {
                        for (int g = 0; g < Features[i].Vertices.Length; g++)
                        {
                            Features[i].Vertices[g].Color = Color.LightGreen;
                        }
                    }
                    else
                    {
                        if (Features[i].Data[1] == "parque")
                        {
                            for (int g = 0; g < Features[i].Vertices.Length; g++)
                            {
                                Features[i].Vertices[g].Color = Color.DarkGreen;
                            }
                        }
                        else
                        {
                            for (int g = 0; g < Features[i].Vertices.Length; g++)
                            {
                                Features[i].Vertices[g].Color = GetRandomColor();
                            }
                        }
                    }
                }
                else
                {
                    for (int g = 0; g < Features[i].Vertices.Length; g++)
                    {
                        Features[i].Vertices[g].Color = GetRandomColor();
                    }
                }


                for (int x = 0; x < Features[i].Vertices.Length - 5; x += 6)
                {
                    Features[i].Vertices[x].TextureCoordinate = textureUpperLeft;
                    Features[i].Vertices[x + 1].TextureCoordinate = textureUpperRight;
                    Features[i].Vertices[x + 2].TextureCoordinate = textureLowerLeft;
                    Features[i].Vertices[x + 3].TextureCoordinate = textureLowerLeft;
                    Features[i].Vertices[x + 4].TextureCoordinate = textureUpperRight;
                    Features[i].Vertices[x + 5].TextureCoordinate = textureLowerRight;
                }
            }
        }

        public static Color GetRandomColor()
        {
            return new Color((byte) r.Next(0, 256), (byte) r.Next(0, 256), (byte) r.Next(0, 256));
        }

        public static void Build2d(List<Feature> Features)
        {
            for (int i = 0; i < Features.Count; i++)
            {
                Features[i].Indices = new int[Features[i].IndicesTecho.Length];
                for (int x = 0; x < Features[i].IndicesTecho.Length; x++)
                {
                    Features[i].Indices[x] = Features[i].IndicesTecho[x];
                }

                Features[i].Vertices = new PositionNormalTextureColor[Features[i].Techo.Count];

                for (int g = 0; g < Features[i].Techo.Count; g++)
                {
                    Features[i].Vertices[g] =
                        new PositionNormalTextureColor(
                            new Vector3(
                                (float) Features[i].Techo[g].X,
                                (float) Features[i].Techo[g].Y,
                                (float) Features[i].Techo[g].Z
                                )
                            );
                }
            }

            for (int i = 0; i < Features.Count; i++)
            {
                for (int g = 0; g < Features[i].Vertices.Length; g++)
                    Features[i].Vertices[g].Normal = new Vector3(0, 0, 0);

                for (int g = 0; g < Features[i].Indices.Length/3; g++)
                {
                    Vector3 firstvec = Features[i].Vertices[Features[i].Indices[g*3 + 1]].Position -
                                       Features[i].Vertices[Features[i].Indices[g*3]].Position;
                    Vector3 secondvec = Features[i].Vertices[Features[i].Indices[g*3]].Position -
                                        Features[i].Vertices[Features[i].Indices[g*3 + 2]].Position;
                    Vector3 normal = Vector3.Cross(firstvec, secondvec);
                    normal.Normalize();
                    Features[i].Vertices[Features[i].Indices[g*3]].Normal += normal;
                    Features[i].Vertices[Features[i].Indices[g*3 + 1]].Normal += normal;
                    Features[i].Vertices[Features[i].Indices[g*3 + 2]].Normal += normal;
                }
                for (int g = 0; g < Features[i].Vertices.Length; g++)
                {
                    Features[i].Vertices[g].Color = Color.Silver;
                }
                for (int x = 0; x < Features[i].Vertices.Length - 5; x += 6)
                {
                    Features[i].Vertices[x].TextureCoordinate = textureUpperLeft;
                    Features[i].Vertices[x + 1].TextureCoordinate = textureUpperRight;
                    Features[i].Vertices[x + 2].TextureCoordinate = textureLowerLeft;
                    Features[i].Vertices[x + 3].TextureCoordinate = textureLowerLeft;
                    Features[i].Vertices[x + 4].TextureCoordinate = textureUpperRight;
                    Features[i].Vertices[x + 5].TextureCoordinate = textureLowerRight;
                }
            }
        }


        public static void Build3d(List<Feature> Features, float current_height)
        {
            float altura_inicial;
            for (int i = 0; i < Features.Count; i++)
            {
                int currentpared = 0;
                Features[i].Points.Reverse();
                Features[i].Paredes = new Vector3[Features[i].Points.Count*6];

                altura_inicial = 0;
                for (int g = 0; g < Features[i].Points.Count; g++)
                {
                    Features[i].Paredes[currentpared] = new Vector3(Features[i].Points[g].X, altura_inicial,
                                                                     Features[i].Points[g].Z);
                    //Features[i].Paredes[currentpared].Color = Microsoft.Xna.Framework.Graphics.Color.HotPink;
                    currentpared++;

                    Features[i].Paredes[currentpared] = new Vector3(Features[i].Points[g].X,
                                                                     altura_inicial + current_height,
                                                                     Features[i].Points[g].Z);
                    //Features[i].Paredes[currentpared].Color = Microsoft.Xna.Framework.Graphics.Color.HotPink;
                    currentpared++;


                    if (g == (Features[i].Points.Count - 1))
                    {
                        Features[i].Paredes[currentpared] = new Vector3(Features[i].Points[0].X, altura_inicial,
                                                                         Features[i].Points[0].Z);
                    }
                    else
                    {
                        Features[i].Paredes[currentpared] = new Vector3(Features[i].Points[g + 1].X, altura_inicial,
                                                                         Features[i].Points[g + 1].Z);
                    }

                    //Features[i].Paredes[currentpared].Color = Microsoft.Xna.Framework.Graphics.Color.HotPink;
                    currentpared++;

                    /***********/

                    if (g == (Features[i].Points.Count - 1))
                    {
                        Features[i].Paredes[currentpared] = new Vector3(Features[i].Points[0].X, altura_inicial,
                                                                         Features[i].Points[0].Z);
                    }
                    else
                    {
                        Features[i].Paredes[currentpared] = new Vector3(Features[i].Points[g + 1].X, altura_inicial,
                                                                         Features[i].Points[g + 1].Z);
                    }

                    //Features[i].Paredes[currentpared].Color = Microsoft.Xna.Framework.Graphics.Color.Orange;
                    currentpared++;

                    Features[i].Paredes[currentpared] = new Vector3(Features[i].Points[g].X,
                                                                     altura_inicial + current_height,
                                                                     Features[i].Points[g].Z);
                    //Features[i].Paredes[currentpared].Color = Microsoft.Xna.Framework.Graphics.Color.Orange;
                    currentpared++;


                    if (g == (Features[i].Points.Count - 1))
                    {
                        Features[i].Paredes[currentpared] = new Vector3(Features[i].Points[0].X, altura_inicial,
                                                                         Features[i].Points[0].Z);
                    }
                    else
                    {
                        Features[i].Paredes[currentpared] = new Vector3(Features[i].Points[g + 1].X, altura_inicial,
                                                                         Features[i].Points[g + 1].Z);
                    }

                    if (g == (Features[i].Points.Count - 1))
                    {
                        Features[i].Paredes[currentpared] = new Vector3(Features[i].Points[0].X,
                                                                         altura_inicial + current_height,
                                                                         Features[i].Points[0].Z);
                    }
                    else
                    {
                        Features[i].Paredes[currentpared] = new Vector3(Features[i].Points[g + 1].X,
                                                                         altura_inicial + current_height,
                                                                         Features[i].Points[g + 1].Z);
                    }


                    //Features[i].Paredes[currentpared].Color = Microsoft.Xna.Framework.Graphics.Color.Orange;
                    currentpared++;
                }
                List<Vector3> pr = Features[i].Paredes.ToList();
                //pr.Reverse();
                for (int xx = 0; xx < pr.Count; xx++)
                {
                    Features[i].Paredes[xx] = pr[xx];
                }
            }
        }

        public static void Build3d(List<Feature> Features)
        {
            float current_height = 0;
            float altura_inicial;
            for (int i = 0; i < Features.Count; i++)
            {
                int currentpared = 0;
                Features[i].Points.Reverse();
                Features[i].Paredes = new Vector3[Features[i].Points.Count*6];

                altura_inicial = 0;
                current_height = Features[i].Height;
                for (int g = 0; g < Features[i].Points.Count; g++)
                {
                    Features[i].Paredes[currentpared] = new Vector3(Features[i].Points[g].X, altura_inicial,
                                                                     Features[i].Points[g].Z);
                    //Features[i].Paredes[currentpared].Color = Microsoft.Xna.Framework.Graphics.Color.HotPink;
                    currentpared++;

                    Features[i].Paredes[currentpared] = new Vector3(Features[i].Points[g].X,
                                                                     altura_inicial + current_height,
                                                                     Features[i].Points[g].Z);
                    //Features[i].Paredes[currentpared].Color = Microsoft.Xna.Framework.Graphics.Color.HotPink;
                    currentpared++;


                    if (g == (Features[i].Points.Count - 1))
                    {
                        Features[i].Paredes[currentpared] = new Vector3(Features[i].Points[0].X, altura_inicial,
                                                                         Features[i].Points[0].Z);
                    }
                    else
                    {
                        Features[i].Paredes[currentpared] = new Vector3(Features[i].Points[g + 1].X, altura_inicial,
                                                                         Features[i].Points[g + 1].Z);
                    }

                    //Features[i].Paredes[currentpared].Color = Microsoft.Xna.Framework.Graphics.Color.HotPink;
                    currentpared++;

                    /***********/

                    if (g == (Features[i].Points.Count - 1))
                    {
                        Features[i].Paredes[currentpared] = new Vector3(Features[i].Points[0].X, altura_inicial,
                                                                         Features[i].Points[0].Z);
                    }
                    else
                    {
                        Features[i].Paredes[currentpared] = new Vector3(Features[i].Points[g + 1].X, altura_inicial,
                                                                         Features[i].Points[g + 1].Z);
                    }

                    //Features[i].Paredes[currentpared].Color = Microsoft.Xna.Framework.Graphics.Color.Orange;
                    currentpared++;

                    Features[i].Paredes[currentpared] = new Vector3(Features[i].Points[g].X,
                                                                     altura_inicial + current_height,
                                                                     Features[i].Points[g].Z);
                    //Features[i].Paredes[currentpared].Color = Microsoft.Xna.Framework.Graphics.Color.Orange;
                    currentpared++;


                    if (g == (Features[i].Points.Count - 1))
                    {
                        Features[i].Paredes[currentpared] = new Vector3(Features[i].Points[0].X, altura_inicial,
                                                                         Features[i].Points[0].Z);
                    }
                    else
                    {
                        Features[i].Paredes[currentpared] = new Vector3(Features[i].Points[g + 1].X, altura_inicial,
                                                                         Features[i].Points[g + 1].Z);
                    }

                    if (g == (Features[i].Points.Count - 1))
                    {
                        Features[i].Paredes[currentpared] = new Vector3(Features[i].Points[0].X,
                                                                         altura_inicial + current_height,
                                                                         Features[i].Points[0].Z);
                    }
                    else
                    {
                        Features[i].Paredes[currentpared] = new Vector3(Features[i].Points[g + 1].X,
                                                                         altura_inicial + current_height,
                                                                         Features[i].Points[g + 1].Z);
                    }


                    //Features[i].Paredes[currentpared].Color = Microsoft.Xna.Framework.Graphics.Color.Orange;
                    currentpared++;
                }
                /*
                List<Vector3> pr = Features[i].Paredes.ToList();
                //pr.Reverse();
                for (int xx = 0; xx < pr.Count; xx++) {
                    Features[i].Paredes[xx] = pr[xx];
                }
*/
            }
        }

        public static List<Vector3> GetTriangleStrip(Vector3[] points, float thickness)
        {
            if (points.Length == 0)
            {
                //return null;
            }
            Vector3 lastPoint = Vector3.Zero;
            List<Vector3> list = new List<Vector3>();
            for (int i = 0; i < points.Length; i++)
            {
                if (i == 0)
                {
                    lastPoint = points[i];
                    continue;
                }
                //the direction of the current line
                Vector3 direction = lastPoint - points[i];
                direction.Normalize();
                //the perpendiculat to the current line
                Vector3 normal = Vector3.Cross(direction, Vector3.Down);
                normal.Normalize();
                Vector3 p1 = lastPoint + normal*thickness;
                Vector3 p2 = lastPoint - normal*thickness;
                Vector3 p3 = points[i] + normal*thickness;
                Vector3 p4 = points[i] - normal*thickness;

                list.Add(new Vector3(p4.X, p4.Y, p4.Z));
                list.Add(new Vector3(p3.X, p3.Y, p3.Z));
                list.Add(new Vector3(p2.X, p2.Y, p2.Z));
                list.Add(new Vector3(p1.X, p1.Y, p1.Z));
                lastPoint = points[i];
            }
            return list;
        }
        public static Vector3 getVector(double x, double y, double z) {
            string[] vals = new string[2];
            vals[0] = x.ToString().Substring(4);
            vals[1] = z.ToString().Substring(4);

            //vals[0] = x.ToString();
            //vals[1] = z.ToString();
            if(vals[0].Length < 9){
                vals[0] = vals[0].PadRight(8, '0');
                vals[0] = vals[0].PadRight(9, '1');
            }

            if (vals[1].Length < 9) {
                vals[1] = vals[1].PadRight(8, '0');
                vals[1] = vals[1].PadRight(9, '1');
            }
            float pX = ((float)double.Parse(vals[0].Substring(0, 9))) / 10000;
            float pY = Convert.ToSingle(y);
            float pZ = ((float)double.Parse(vals[1].Substring(0, 9))) / 10000;
            pX -= 35608f;
            
            pZ -= 35608f;
            return new Vector3(pX, pY, pZ);
        }

        public static Vector3 getVector2(double x, double y, double z)
        {
            string[] vals = new string[2];
            vals[0] = x.ToString().Substring(4);
            vals[1] = z.ToString().Substring(4);

            
            int add = 7 - vals[0].Length;
            if (add < 0)
            {
                vals[0] = vals[0].Substring(0, 7);
            }
            if (add > 0)
            {
                for (int e = 0; e < add; e++)
                {
                    if (e == (add - 1))
                    {
                        vals[0] += "1";
                    }
                    else
                    {
                        vals[0] += "0";
                    }
                }
            }
            int add2 = 7 - vals[1].Length;
            if (add2 < 0)
            {
                vals[1] = vals[1].Substring(0,7);
            }
            if (add2 > 0)
            {
                for (int e = 0; e < add2; e++)
                {
                    if (e == (add2 - 1))
                    {
                        vals[1] += "1";
                    }
                    else
                    {
                        vals[1] += "0";
                    }
                }
            }

            float pX = (float)double.Parse(vals[0]) / 1000;
            float pY = Convert.ToSingle(y);
            float pZ = (float)double.Parse(vals[1]) / 1000;

            return new Vector3(pX, pY, pZ);
        }

        public static float restar = 4855000f;
        public static Vector3 getFinalVector(Vector3 vec)
        {
            return new Vector3(vec.X, vec.Y, vec.Z);
        }

        public static Vector3 getFinalVector(float Z, float Y, float X)
        {
            string[] vals = new string[2];
            vals[0] = X.ToString().Replace(",", "").Replace(" ", " ");
            vals[1] = Z.ToString().Replace(",", "").Replace(" ", " ");
         

            int add = 7 - vals[0].Length;
            if (add < 0)
            {
                vals[0] = vals[0].Substring(0, 7);
            }
            if (add > 0)
            {
                for (int e = 0; e < add; e++)
                {
                    if (e == (add - 1))
                    {
                        vals[0] += "0";
                    }
                    else
                    {
                        vals[0] += "0";
                    }
                }
            }
            int add2 = 7 - vals[1].Length;
            if (add2 < 0)
            {
                vals[1] = vals[1].Substring(0, 7);
            }
            if (add2 > 0)
            {
                for (int e = 0; e < add2; e++)
                {
                    if (e == (add2 - 1))
                    {
                        vals[1] += "0";
                    }
                    else
                    {
                        vals[1] += "0";
                    }
                }
            }
            

            float fX = float.Parse(vals[0]) - 478500f;
            float fY = Convert.ToSingle(Y);
            float fZ = float.Parse(vals[1]) - 478500f;


            if (startingPoint == 0)
            {
                startingPoint = 35277.7734f;
                //startingPoint = (fZ /divisor);
            }

            Vector3 vc = new Vector3(fZ / 10, fY, fX / 10);
            //vc.Normalize();
            return vc;
        }

        public static List<string> getUniqueValue(List<Feature> Features)
        {
            List<string> strings = new List<string>();

            for (int i = 0; i < Features.Count; i++)
            {
                if (!strings.Contains(Features[i].Data[2]))
                {
                    //Console.WriteLine("\"" + Polygons[i].Data[7] + "\", ");
                    strings.Add(Features[i].Data[2]);
                }
            }
            return strings;
        }

        internal static void OptimizeTriangles(PositionNormalTextureColor[] UnoptimizedVertices, int[] UnoptimizedIndices, out PositionNormalTextureColor[] OptimizedVertices, out int[] OptimizedIndices) {


            //OptimizedIndices = UnoptimizedIndices;
            //OptimizedVertices = UnoptimizedVertices;
            //return;
            /*
             Off the top of my head - I'd start by finding adjacent triangles using something like 
             vertex hashing to see which vertices are on the same spot (or within some tiny distance)
             and share texture coords/normals etc to build up a basic unoptimal index list. 
             To do this the idea is to have two arrays, final_verts and final_indices.
             * For each vertex you check to see if it exists in final_verts 
             * already (this is where hashing comes in handy) and if it does, add an index 
             * to final_indices pointing to it.  If it's not in the list already, add the new 
             * vertex to final_verts and add a new index to 
             * final_indices (i.e final_indices.push_back(final_verts.size() - 1) etc).
             */

            /*
            OptimizedIndices = UnoptimizedIndices;
            OptimizedVertices = UnoptimizedVertices;
            return;*/
            List<PositionNormalTextureColor> final_verts = new List<PositionNormalTextureColor>();
            List<int> final_indices = new List<int>();


            Dictionary<string, int> vertices = new Dictionary<string, int>();


            int currentIndex = -1;

            for (int v = 0; v < UnoptimizedVertices.Length; v++) {
                string str = (
                  UnoptimizedVertices[v].Position.X.ToString()
                + UnoptimizedVertices[v].Position.Y.ToString()
                + UnoptimizedVertices[v].Position.Z.ToString()
                + UnoptimizedVertices[v].Normal.X.ToString()
                + UnoptimizedVertices[v].Normal.Y.ToString()
                + UnoptimizedVertices[v].Normal.Z.ToString()
                );
                str = str.Replace("-", "").Replace(",", "").Replace(".", "");

                string vertexHash = str;

                if (vertices.ContainsKey(vertexHash)) {
                    int vertexIndex = vertices[vertexHash];
                    final_indices.Add(vertexIndex);
                }
                else {
                    final_verts.Add(UnoptimizedVertices[v]);

                    currentIndex = final_verts.Count - 1;
                    vertices.Add(vertexHash, currentIndex);
                    final_indices.Add(currentIndex);
                }
            }

            OptimizedIndices = final_indices.ToArray();
            OptimizedVertices = final_verts.ToArray();
        }

        internal static void OptimizeTriangles22<T>(T[] UnoptimizedVertices, int[] UnoptimizedIndices, out T[] OptimizedVertices, out int[] OptimizedIndices) where T : struct {


            //OptimizedIndices = UnoptimizedIndices;
            //OptimizedVertices = UnoptimizedVertices;
            //return;
            /*
             Off the top of my head - I'd start by finding adjacent triangles using something like 
             vertex hashing to see which vertices are on the same spot (or within some tiny distance)
             and share texture coords/normals etc to build up a basic unoptimal index list. 
             To do this the idea is to have two arrays, final_verts and final_indices.
             * For each vertex you check to see if it exists in final_verts 
             * already (this is where hashing comes in handy) and if it does, add an index 
             * to final_indices pointing to it.  If it's not in the list already, add the new 
             * vertex to final_verts and add a new index to 
             * final_indices (i.e final_indices.push_back(final_verts.size() - 1) etc).
             */

            /*
            OptimizedIndices = UnoptimizedIndices;
            OptimizedVertices = UnoptimizedVertices;
            return;*/
            List<T> final_verts = new List<T>();
            List<int> final_indices = new List<int>();


            Dictionary<double, int> vertices = new Dictionary<double, int>();


            int currentIndex = -1;

            for (int v = 0; v < UnoptimizedVertices.Length; v++) {

                FieldInfo field = UnoptimizedVertices[v].GetType().GetField("Position");
                Vector3 vx = Vector3.Zero;

                FieldInfo[] fields = UnoptimizedVertices[v].GetType().GetFields();


                vx = (Vector3)field.GetValue(vx);

                FieldInfo field2 = UnoptimizedVertices[v].GetType().GetField("Normal");
                Vector3 no = Vector3.Zero; 
                no = (Vector3)field.GetValue(vx);
                
                string str = (
                  vx.X.ToString()
                + vx.Y.ToString()
                + vx.Z.ToString()
                + no.X.ToString()
                + no.Y.ToString()
                + no.Z.ToString()
                );
            str = str.Replace("-", "").Replace(",", "").Replace(".", "");
            
                double vertexHash = UnoptimizedVertices[v].GetHashCode();

                if (vertices.ContainsKey(vertexHash)) {
                    int vertexIndex = vertices[vertexHash];
                    final_indices.Add(vertexIndex);
                }
                else {
                    final_verts.Add(UnoptimizedVertices[v]);

                    currentIndex = final_verts.Count - 1;
                    vertices.Add(vertexHash, currentIndex);
                    final_indices.Add(currentIndex);


                }
            }


            OptimizedIndices = final_indices.ToArray();
            OptimizedVertices = final_verts.ToArray();
        }




        internal static void OptimizeTriangles2(VertexPositionNormalTexture[] UnoptimizedVertices, int[] UnoptimizedIndices) {
            /*
             Off the top of my head - I'd start by finding adjacent triangles using something like 
             vertex hashing to see which vertices are on the same spot (or within some tiny distance)
             and share texture coords/normals etc to build up a basic unoptimal index list. 
             To do this the idea is to have two arrays, final_verts and final_indices.
             * For each vertex you check to see if it exists in final_verts 
             * already (this is where hashing comes in handy) and if it does, add an index 
             * to final_indices pointing to it.  If it's not in the list already, add the new 
             * vertex to final_verts and add a new index to 
             * final_indices (i.e final_indices.push_back(final_verts.size() - 1) etc).
             */

            /*
            OptimizedIndices = UnoptimizedIndices;
            OptimizedVertices = UnoptimizedVertices;
            return;*/
            List<VertexPositionNormalTexture> final_verts = new List<VertexPositionNormalTexture>();
            List<int> final_indices = new List<int>();


            Dictionary<double, int> vertices = new Dictionary<double, int>();


            int currentIndex = -1;

            for (int v = 0; v < UnoptimizedVertices.Length; v++) {
                double vertexHash = UnoptimizedVertices[v].GetHashCode();

                if (vertices.ContainsKey(vertexHash)) {
                    int vertexIndex = vertices[vertexHash];
                    final_indices.Add(vertexIndex);
                }
                else {
                    final_verts.Add(UnoptimizedVertices[v]);

                    currentIndex = final_verts.Count - 1;
                    vertices.Add(vertexHash, currentIndex);
                    final_indices.Add(currentIndex);


                }
            }


            //OptimizedIndices = final_indices.ToArray();
            //OptimizedVertices2 = final_verts.ToArray();
        }



        internal static void OptimizeTriangles22(PositionNormalTextureColor[] UnoptimizedVertices, int[] UnoptimizedIndices) {
            /*
             Off the top of my head - I'd start by finding adjacent triangles using something like 
             vertex hashing to see which vertices are on the same spot (or within some tiny distance)
             and share texture coords/normals etc to build up a basic unoptimal index list. 
             To do this the idea is to have two arrays, final_verts and final_indices.
             * For each vertex you check to see if it exists in final_verts 
             * already (this is where hashing comes in handy) and if it does, add an index 
             * to final_indices pointing to it.  If it's not in the list already, add the new 
             * vertex to final_verts and add a new index to 
             * final_indices (i.e final_indices.push_back(final_verts.size() - 1) etc).
             */

            /*
            OptimizedIndices = UnoptimizedIndices;
            OptimizedVertices = UnoptimizedVertices;
            return;*/
            List<PositionNormalTextureColor> final_verts = new List<PositionNormalTextureColor>();
            List<int> final_indices = new List<int>();


            Dictionary<double, int> vertices = new Dictionary<double, int>();


            int currentIndex = -1;

            for (int v = 0; v < UnoptimizedVertices.Length;v++){
                //double vertexHash = UnoptimizedVertices[v].GetHashCode();
                double vertexHash = double.Parse(
                    (
                    UnoptimizedVertices[v].Position.X.ToString() + 
                    UnoptimizedVertices[v].Position.Y.ToString() +
                    UnoptimizedVertices[v].Position.Z.ToString() +
                    UnoptimizedVertices[v].TextureCoordinate.X.ToString() +
                    UnoptimizedVertices[v].TextureCoordinate.Y.ToString()
                    ).Replace(",", "").Replace("E", "").Replace("-", "")
                    
                    );
                if (vertices.ContainsKey(vertexHash)) {
                    int vertexIndex = vertices[vertexHash];
                    final_indices.Add(vertexIndex);
                }
                else{
                    final_verts.Add(UnoptimizedVertices[v]);
                    
                    currentIndex = final_verts.Count - 1;
                    vertices.Add(vertexHash, currentIndex);
                    final_indices.Add(currentIndex);
                }
            }
        }
    }
}
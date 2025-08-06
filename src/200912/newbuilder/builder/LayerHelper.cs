using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Builder
{
    [System.Diagnostics.DebuggerDisplay("{Position} C: {Color}")]
    public struct PositionNormalTextureColor {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TextureCoordinate;
        public Color Color;
        public bool Techo;

        public PositionNormalTextureColor(Vector3 position) {
            this.Position = position;
            this.Normal = Vector3.UnitY;
            this.TextureCoordinate = Vector2.Zero;
            this.Color = Color.White;
            this.Techo = false;
        }

        public PositionNormalTextureColor(Vector3 position, Color color)
        {
            this.Position = position;
            this.Normal = Vector3.UnitY;
            this.TextureCoordinate = Vector2.Zero;
            this.Color = color;
            this.Techo = false;
        }
        public PositionNormalTextureColor(Vector3 position, Color color, bool techo)
        {
            this.Position = position;
            this.Normal = Vector3.UnitY;
            this.TextureCoordinate = Vector2.Zero;
            this.Color = color;
            this.Techo = techo;
        }
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct HashedVertex
    {
        public int Index;
        public int HashCode;
        public HashedVertex(int index, int hashcode)
        {
            this.Index = index;
            this.HashCode = hashcode;
        }
    }



    public abstract class VertexChannelHelper {
        // Methods
        protected VertexChannelHelper() {
        }

        public abstract void AccumulateHashCodes(int[] hashCodes);
        public abstract bool CompareVertices(int i, int j);
        //public abstract void StoreOutput(VertexChannelCollection channelCollection, List<int> indices);
    }






    public static class LayerHelper
    {

        public static float startingPoint = 2;

        public static float divisor = 200f;

        public static Vector2 textureUpperLeft = new Vector2(0.0f, 0.0f);
        public static Vector2 textureUpperRight = new Vector2(1.0f, 0.0f);
        public static Vector2 textureLowerLeft = new Vector2(0.0f, 1.0f);
        public static Vector2 textureLowerRight = new Vector2(1.0f, 1.0f);


        public static Random r = new Random(23051983);

        public static void GenerateCoords(Feature Feature)
        {

            int[] tmpindx = Feature.IndicesTecho;
            Feature.Indices = new int[Feature.IndicesTecho.Length + Feature.Paredes.Length];
            if (false)
            {
                int xx = 0;
                for (int x = Feature.Indices.Length - 1; x > 0; x--)
                {
                    Feature.Indices[xx] = x;
                    xx++;
                }
            }
            else
            {
                for (int x = 0; x < Feature.Indices.Length; x++)
                {
                    Feature.Indices[x] = x;
                }
            }
            //Feature.Paredes.Reverse();

            Feature.Vertices = new PositionNormalTextureColor[Feature.Techo.Count + Feature.Paredes.Length];

            //Feature.Techo.Reverse();
            for (int g = 0; g < Feature.Techo.Count; g++)
            {
                Feature.Vertices[g] =
                    new PositionNormalTextureColor(
                        new Vector3(
                            (float)Feature.Techo[g].X,
                            (float)Feature.Techo[g].Y,
                            (float)Feature.Techo[g].Z
                        ),
                        Color.White,
                        true
                );
            }

            if (false)
            {
                List<Vector3> test = new List<Vector3>();
                test.Add(new Vector3(-1, -1, -1));
                test.Add(new Vector3(3, 3, 3));
                test.Add(new Vector3(1, 1, 1));

                BoundingBox bb = BoundingBox.CreateFromPoints(test);

                Vector3 siz = bb.Max + bb.Min;
            }

            //Feature.boundingBox = new BoundingBox();
            Feature.boundingBox = BoundingBox.CreateFromPoints(Feature.Techo);

            Vector3 size = Feature.boundingBox.Max;// + Feature.boundingBox.Min;
            //size.

            //Vector2[] uvs = LayerHelper.tryProcessUv(Feature);

            Vector2[] uvs = LayerHelper.uvear(Feature);
            for (int g = 0; g < Feature.Techo.Count; g++)
            {
                //Vector3 size = Feature.boundingBox.Max + Feature.boundingBox.Min;

                //uvs[i] = new Vector2(Feature.Vertices[i].Position.X / size.X, Feature.Vertices[i].Position.Z / size.Z);

                Feature.Vertices[g].TextureCoordinate = uvs[g];//new Vector2(Feature.Vertices[g].Position.X / size.X, Feature.Vertices[g].Position.Z / size.Z);

            }

            //Array.Reverse(Feature.Paredes);
            for (int g = 0; g < Feature.Paredes.Length; g++)
            {
                PositionNormalTextureColor vx = new PositionNormalTextureColor(
                new Vector3(
                    Feature.Paredes[g].X,
                    Feature.Paredes[g].Y,
                    Feature.Paredes[g].Z
                    )
                );
                vx.TextureCoordinate = Feature.Coords[g];
                Feature.Vertices[Feature.Techo.Count + g] = vx;
            }

            /*
            for (int x = 0; x < Feature.Paredes.Length - 5; x += 6) {
                Feature.Vertices[Feature.Techo.Count + x].TextureCoordinate = textureUpperLeft;
                Feature.Vertices[Feature.Techo.Count + x + 1].TextureCoordinate = textureUpperRight;
                Feature.Vertices[Feature.Techo.Count + x + 2].TextureCoordinate = textureLowerLeft;
                Feature.Vertices[Feature.Techo.Count + x + 3].TextureCoordinate = textureLowerLeft;
                Feature.Vertices[Feature.Techo.Count + x + 4].TextureCoordinate = textureUpperRight;
                Feature.Vertices[Feature.Techo.Count + x + 5].TextureCoordinate = textureLowerRight;
            }
            Console.WriteLine("asdasd");

            */
        }
        
        public static void GenerateFloorCoords(Feature Feature)
        {

            Feature.Vertices = new PositionNormalTextureColor[Feature.Paredes.Length];

            //Array.Reverse(Feature.Paredes);
            for (int g = 0; g < Feature.Paredes.Length; g++)
            {
                PositionNormalTextureColor vx = new PositionNormalTextureColor(
                new Vector3(
                    Feature.Paredes[g].X,
                    Feature.Paredes[g].Y,
                    Feature.Paredes[g].Z
                    )
                );
                vx.TextureCoordinate = Feature.Coords[g];
                Feature.Vertices[ g] = vx;
            }


            Feature.Indices = new int[Feature.Paredes.Length];
            
            for (int x = 0; x < Feature.Indices.Length; x++)
            {
                Feature.Indices[x] = x;
            }
        }

        public static Vector2[] uvear(Feature Feature)
        {
            Vector2[] uvs = new Vector2[Feature.Techo.Count];
            float scaleFactor = 0.5f;


            for (int index = 0; index < Feature.Techo.Count; index += 3)
            {
                // Get the three vertices bounding this triangle.
                Vector3 v1 = Feature.Techo[index];
                Vector3 v2 = Feature.Techo[index + 1];
                Vector3 v3 = Feature.Techo[index + 2];

                // Compute a vector perpendicular to the face.
                Vector3 normal = Vector3.Cross2(v3 - v1, v2 - v1);

                // Form a rotation that points the z+ axis in this perpendicular direction.
                // Multiplying by the inverse will flatten the triangle into an xy plane.
                Quaternion q1 = Quaternion.LookRotation(normal);
                Quaternion rotation = Quaternion.Inverse(q1);

                // Assign the uvs, applying a scale factor to control the texture tiling.
                uvs[index] = (Vector2)(rotation * v1) * scaleFactor;
                uvs[index + 1] = (Vector2)(rotation * v2) * scaleFactor;
                uvs[index + 2] = (Vector2)(rotation * v3) * scaleFactor;
            }

            return uvs;
        }


        public static Vector2[] tryProcessUv(Feature Feature)
        {
            Vector2[] uvs = new Vector2[Feature.Techo.Count];
            for (int i = 0; i < Feature.Techo.Count; i++)
            {

                //uvs[i] = new Vector2(Feature.Vertices[i].Position.X / size.X, Feature.Vertices[i].Position.Z / size.Z);
            }
            return uvs;
        }
        public static Color GetRandomColor()
        {
            int[] rgb = new int[3];
           rgb[0] = r.Next(256);  // red
           rgb[1] = r.Next(256);  // green
           rgb[2] = r.Next(256);  // blue

           // Console.WriteLine("{0},{1},{2}", rgb[0], rgb[1], rgb[2]);

           // find max and min indexes.
           int max, min;

           if (rgb[0] > rgb[1])
           {
               max = (rgb[0] > rgb[2]) ? 0 : 2;
               min = (rgb[1] < rgb[2]) ? 1 : 2;
           }
           else
           {
               max = (rgb[1] > rgb[2]) ? 1 : 2;
               int notmax = 1 + max % 2;
               min = (rgb[0] < rgb[notmax]) ? 0 : notmax;
           }
           rgb[max] = 255;
           rgb[min] = 0;

           return new Color((byte)rgb[0], (byte)rgb[1], (byte)rgb[2]);

            //return new Color((byte)r.Next(0, 256), (byte)r.Next(0, 256), (byte)r.Next(0, 256));
        }
        public static Color GetRandomBuilding()
        {
            return new Color((byte)r.Next(150, 180), (byte)r.Next(100, 120), (byte)r.Next(10, 40));
        }

        private static float getHeight(List<Vector3> points, double X, double Z)
        {
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].X == X && points[i].Z == Z)
                {
                    return points[i].Y;
                }
            }
            return 0;
        }

        private static float getFloorHeight(List<Vector3> points, double X, double Z)
        {
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].X == X && points[i].Z == Z)
                {
                    return points[i].Y;
                }
            }
            return 0;
        }

        private static Dictionary<string, int> vertices = new Dictionary<string, int>(50);

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

            
            //OptimizedIndices = UnoptimizedIndices;
            //OptimizedVertices = UnoptimizedVertices;
            //return;
            List<PositionNormalTextureColor> final_verts = new List<PositionNormalTextureColor>();
            List<int> final_indices = new List<int>();

            vertices.Clear();
            //vertices = new Dictionary<string, int>();


            int currentIndex = -1;

            for (int v = 0; v < UnoptimizedVertices.Length; v++) {
                string str = (
                  UnoptimizedVertices[v].Position.X.ToString()
                + UnoptimizedVertices[v].Position.Y.ToString()
                + UnoptimizedVertices[v].Position.Z.ToString()
                + UnoptimizedVertices[v].Normal.X.ToString()
                + UnoptimizedVertices[v].Normal.Y.ToString()
                + UnoptimizedVertices[v].Normal.Z.ToString()
                + UnoptimizedVertices[v].TextureCoordinate.X.ToString()
                + UnoptimizedVertices[v].TextureCoordinate.Y.ToString()
                
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
            vertices.Clear();
        }

        public static void Build3dFeature(Feature feature, float altura)
        {
            List<Vector3> Paredes = new List<Vector3>(feature.Points.Count * 6);
            List<Vector2> Coords = new List<Vector2>(feature.Points.Count * 6);

            for (int g = 0; g < feature.Points.Count; g++)
            {

                float alturaLowCurrent = 0;
                float alturaLowNext = 0;

                float alturaHighCurrent = 0;
                float alturaHighNext = 0;

                Vector3 lowCurrent = Vector3.Zero;
                Vector3 highCurrent = Vector3.Zero;
                Vector3 lowNext = Vector3.Zero;
                Vector3 highNext = Vector3.Zero;

                if (g == (feature.Points.Count - 1))
                {
                    alturaLowCurrent = getHeight(feature.Points, feature.Points[g].X, feature.Points[g].Z);
                    alturaLowNext = getHeight(feature.Points, feature.Points[0].X, feature.Points[0].Z);

                    alturaHighCurrent = getHeight(feature.Points, feature.Points[g].X, feature.Points[g].Z) + feature.Altura;
                    alturaHighNext = getHeight(feature.Points, feature.Points[0].X, feature.Points[0].Z) + feature.Altura;


                    lowCurrent = new Vector3(feature.Points[g].X, alturaLowCurrent, feature.Points[g].Z);
                    highCurrent = new Vector3(feature.Points[g].X, alturaHighCurrent, feature.Points[g].Z);
                    lowNext = new Vector3(feature.Points[0].X, alturaLowNext, feature.Points[0].Z);
                    highNext = new Vector3(feature.Points[0].X, alturaHighNext, feature.Points[0].Z);

                    Paredes.Add(lowCurrent);
                    Paredes.Add(highCurrent);
                    Paredes.Add(highNext);

                    Paredes.Add(lowCurrent);
                    Paredes.Add(highNext);
                    Paredes.Add(lowNext);

                    Coords.Add(textureLowerLeft);
                    Coords.Add(textureUpperLeft);
                    Coords.Add(textureUpperRight);

                    Coords.Add(textureLowerLeft);
                    Coords.Add(textureUpperRight);
                    Coords.Add(textureLowerRight);


                }
                else
                {
                    alturaLowCurrent = getHeight(feature.Points, feature.Points[g].X, feature.Points[g].Z);
                    alturaLowNext = getHeight(feature.Points, feature.Points[g + 1].X, feature.Points[g + 1].Z);


                    alturaHighCurrent = getHeight(feature.Points, feature.Points[g].X, feature.Points[g].Z) + feature.Altura;
                    alturaHighNext = getHeight(feature.Points, feature.Points[g + 1].X, feature.Points[g + 1].Z) + feature.Altura;

                    lowCurrent = new Vector3(feature.Points[g].X, alturaLowCurrent, feature.Points[g].Z);
                    highCurrent = new Vector3(feature.Points[g].X, alturaHighCurrent, feature.Points[g].Z);
                    lowNext = new Vector3(feature.Points[g + 1].X, alturaLowNext, feature.Points[g + 1].Z);
                    highNext = new Vector3(feature.Points[g + 1].X, alturaHighNext, feature.Points[g + 1].Z);

                    Paredes.Add(lowCurrent);
                    Paredes.Add(highCurrent);
                    Paredes.Add(highNext);

                    Paredes.Add(lowCurrent);
                    Paredes.Add(highNext);
                    Paredes.Add(lowNext);


                    Coords.Add(textureLowerLeft);
                    Coords.Add(textureUpperLeft);
                    Coords.Add(textureUpperRight);

                    Coords.Add(textureLowerLeft);
                    Coords.Add(textureUpperRight);
                    Coords.Add(textureLowerRight);
                }
            }
            feature.Paredes = Paredes.ToArray();
            feature.Coords = Coords.ToArray();

            //feature.Paredes.Reverse();
            /*
            List<Vector3> pr = feature.Paredes.ToList();
            for (int xx = 0; xx < pr.Count; xx++) {
                feature.Paredes[xx] = pr[xx];
            }*/
        }

        public static Feature BuildFloor(List<Vector3> Points, float startHeight, float Altura)
        {
            Feature feature = new Feature();
            feature.Altura = Altura;



            List<Vector3> Paredes = new List<Vector3>(Points.Count * 6);
            List<Vector2> Coords = new List<Vector2>(Points.Count * 6);

            for (int g = 0; g < Points.Count; g++)
            {

                float alturaLowCurrent = 0;
                float alturaLowNext = 0;

                float alturaHighCurrent = 0;
                float alturaHighNext = 0;

                Vector3 lowCurrent = Vector3.Zero;
                Vector3 highCurrent = Vector3.Zero;
                Vector3 lowNext = Vector3.Zero;
                Vector3 highNext = Vector3.Zero;

                if (g == (Points.Count - 1))
                {


                    alturaLowCurrent = getFloorHeight(Points, Points[g].X, Points[g].Z) + startHeight;
                    alturaLowNext = getFloorHeight(Points, Points[0].X, Points[0].Z) + startHeight;

                    alturaHighCurrent = getFloorHeight(Points, Points[g].X, Points[g].Z) + startHeight +  Altura;
                    alturaHighNext = getFloorHeight(Points, Points[0].X, Points[0].Z) + startHeight + Altura;

                    lowCurrent = new Vector3(Points[g].X, alturaLowCurrent, Points[g].Z);
                    highCurrent = new Vector3(Points[g].X, alturaHighCurrent, Points[g].Z);
                    lowNext = new Vector3(Points[0].X, alturaLowNext, Points[0].Z);
                    highNext = new Vector3(Points[0].X, alturaHighNext, Points[0].Z);


                }
                else
                {
                    alturaLowCurrent = getFloorHeight(Points, Points[g].X, Points[g].Z) + startHeight;
                    alturaLowNext = getFloorHeight(Points, Points[g + 1].X, Points[g + 1].Z) + startHeight;


                    alturaHighCurrent = getFloorHeight(Points, Points[g].X, Points[g].Z) + startHeight + Altura;
                    alturaHighNext = getFloorHeight(Points, Points[g + 1].X, Points[g + 1].Z) + startHeight + Altura;

                    lowCurrent = new Vector3(Points[g].X, alturaLowCurrent, Points[g].Z);
                    highCurrent = new Vector3(Points[g].X, alturaHighCurrent, Points[g].Z);
                    lowNext = new Vector3(Points[g + 1].X, alturaLowNext, Points[g + 1].Z);
                    highNext = new Vector3(Points[g + 1].X, alturaHighNext, Points[g + 1].Z);


                }


                Paredes.Add(lowCurrent);
                Paredes.Add(highCurrent);
                Paredes.Add(highNext);

                Paredes.Add(highNext);
                Paredes.Add(lowNext);
                Paredes.Add(lowCurrent);


                Coords.Add(textureLowerLeft);
                Coords.Add(textureUpperLeft);
                Coords.Add(textureUpperRight);

                Coords.Add(textureUpperRight);
                Coords.Add(textureLowerRight);
                Coords.Add(textureLowerLeft);

            }
            feature.Paredes = Paredes.ToArray();
            feature.Coords = Coords.ToArray();

            LayerHelper.GenerateFloorCoords(feature);

            return feature;
        }


        public static Vector2 PointLine(Vector2 point, Vector2 lineOrigin, Vector2 lineDirection, out float projectedX) {
            // In theory, sqrMagnitude should be 1, but in practice this division helps with numerical stability
            projectedX = Vector2.Dot(lineDirection, point - lineOrigin) / lineDirection.sqrMagnitude;
            return lineOrigin + lineDirection * projectedX;
        }
        public static float Distance_PointLine(Vector2 point, Vector2 lineOrigin, Vector2 lineDirection) {
            return Vector2.Distance(point, Closest_PointLine(point, lineOrigin, lineDirection));
        }
        public static Vector2 Closest_PointLine(Vector2 point, Vector2 lineOrigin, Vector2 lineDirection) {
            float projectedX;
            return PointLine(point, lineOrigin, lineDirection, out projectedX);
        }

        public static float CalculateVertexHeight(Vector2 vertex, Vector2 edgeA, Vector2 edgeDirection, float roofPitch) {
            float distance = Distance_PointLine(vertex, edgeA, edgeDirection);
            return Mathf.Tan(roofPitch * Mathf.Deg2Rad) * distance;
        }
        public static Vector2 PointSegment(Vector2 point, Vector2 segmentA, Vector2 segmentB, out float projectedX) {
            Vector2 segmentDirection = segmentB - segmentA;
            float sqrSegmentLength = segmentDirection.sqrMagnitude;
            if (sqrSegmentLength < 0.00001f) {
                // The segment is a point
                projectedX = 0;
                return segmentA;
            }

            float pointProjection = Vector2.Dot(segmentDirection, point - segmentA);
            if (pointProjection <= 0) {
                projectedX = 0;
                return segmentA;
            }
            if (pointProjection >= sqrSegmentLength) {
                projectedX = 1;
                return segmentB;
            }

            projectedX = pointProjection / sqrSegmentLength;
            return segmentA + segmentDirection * projectedX;
        }


        public static Vector2 Closest_PointSegment(Vector2 point, Vector2 segmentA, Vector2 segmentB) {
            float projectedX;
            return PointSegment(point, segmentA, segmentB, out projectedX);
        }


        public static Vector3 CalculateRoofNormal(Vector2 edgeDirection2, float roofPitch) {
            return Quaternion.AngleAxis(roofPitch, edgeDirection2.ToVector3XZ(edgeDirection2)) * Vector3.Up;
        }

        public static List<Vector3> ConstructGableDraft(List<Vector2> skeletonPolygon2, float roofPitch) {
            Vector2 edgeA2 = skeletonPolygon2[0];
            Vector2 edgeB2 = skeletonPolygon2[1];
            Vector2 peak2 = skeletonPolygon2[2];
            Vector2 edgeDirection2 = (edgeB2 - edgeA2).normalized;

            float peakHeight = CalculateVertexHeight(peak2, edgeA2, edgeDirection2, roofPitch);
            Vector3 edgeA3 = edgeA2.ToVector3XZ(edgeA2);
            Vector3 edgeB3 = edgeB2.ToVector3XZ(edgeA2);
            Vector3 peak3 = new Vector3(peak2.X, peakHeight, peak2.Y);
            Vector2 gableTop2 = Closest_PointSegment(peak2, edgeA2, edgeB2);
            Vector3 gableTop3 = new Vector3(gableTop2.X, peakHeight, gableTop2.Y);


            List<Vector3> vertices = new List<Vector3>();
            //return new MeshDraft().AddTriangle(edgeA3, edgeB3, gableTop3, true)
            vertices.Add(edgeA3);
            vertices.Add(gableTop3);
            vertices.Add(peak3);

            vertices.Add(edgeB3);
            vertices.Add(peak3);
            vertices.Add(gableTop3);
            return vertices;
            
        }

        public static Feature BuildRoof2(List<Vector3> Techo) {

            Feature feature = new Feature();
            feature.Techo = Techo;
            //feature.Altura = Altura;
            List<Vector2> techuli = new List<Vector2>();
            foreach(Vector3 v3 in feature.Techo) {
                techuli.Add(new Vector2(v3.X, v3.Z));
            }
            List<Vector3> verts = ConstructGableDraft(techuli, 3);


            feature.Vertices = new PositionNormalTextureColor[feature.Techo.Count];

            //Feature.Techo.Reverse();
            feature.Indices = new int[feature.Techo.Count];
            for (int g = 0; g < feature.Techo.Count; g++) {
                feature.Vertices[g] =
                    new PositionNormalTextureColor(
                        new Vector3(
                            (float)feature.Techo[g].X,
                            (float)feature.Techo[g].Y,
                            (float)feature.Techo[g].Z
                        ),
                        Color.White,
                        true
                );

                feature.Indices[g] = g;

            }


            //Feature.boundingBox = new BoundingBox();
            feature.boundingBox = BoundingBox.CreateFromPoints(feature.Techo);

            Vector3 size = feature.boundingBox.Max;// + Feature.boundingBox.Min;
            //size.

            //Vector2[] uvs = LayerHelper.tryProcessUv(Feature);

            Vector2[] uvs = LayerHelper.uvear(feature);
            for (int g = 0; g < feature.Techo.Count; g++) {
                //Vector3 size = Feature.boundingBox.Max + Feature.boundingBox.Min;

                //uvs[i] = new Vector2(Feature.Vertices[i].Position.X / size.X, Feature.Vertices[i].Position.Z / size.Z);

                feature.Vertices[g].TextureCoordinate = uvs[g];//new Vector2(Feature.Vertices[g].Position.X / size.X, Feature.Vertices[g].Position.Z / size.Z);


            }

            return feature;
        }
        public static Feature BuildRoof(List<Vector3> Techo)
        {
            Feature feature = new Feature();
            feature.Techo = Techo;
            //feature.Altura = Altura;

            feature.Vertices = new PositionNormalTextureColor[feature.Techo.Count];

            //Feature.Techo.Reverse();
            feature.Indices = new int[feature.Techo.Count];
            for (int g = 0; g < feature.Techo.Count; g++)
            {
                feature.Vertices[g] =
                    new PositionNormalTextureColor(
                        new Vector3(
                            (float)feature.Techo[g].X,
                            (float)feature.Techo[g].Y,
                            (float)feature.Techo[g].Z
                        ),
                        Color.White,
                        true
                );

                feature.Indices[g] = g;

            }

            //Feature.boundingBox = new BoundingBox();
            feature.boundingBox = BoundingBox.CreateFromPoints(feature.Techo);

            Vector3 size = feature.boundingBox.Max;// + Feature.boundingBox.Min;
            //Vector2[] uvs = LayerHelper.tryProcessUv(Feature);

            Vector2[] uvs = LayerHelper.uvear(feature);
            for (int g = 0; g < feature.Techo.Count; g++)
            {
                //Vector3 size = Feature.boundingBox.Max + Feature.boundingBox.Min;

                //uvs[i] = new Vector2(Feature.Vertices[i].Position.X / size.X, Feature.Vertices[i].Position.Z / size.Z);

                feature.Vertices[g].TextureCoordinate = uvs[g];//new Vector2(Feature.Vertices[g].Position.X / size.X, Feature.Vertices[g].Position.Z / size.Z);


            }

            return feature;
        }

        public static void Build2d(Feature feature){
            feature.Indices = new int[feature.IndicesTecho.Length];
            for (int x = 0; x < feature.IndicesTecho.Length; x++) {
                feature.Indices[x] = feature.IndicesTecho[x];
            }

            //feature.Techo.Reverse();
            feature.Vertices = new PositionNormalTextureColor[feature.Techo.Count];

            for (int g = 0; g < feature.Techo.Count; g++) {
                feature.Vertices[g] =
                    new PositionNormalTextureColor(
                        new Vector3(
                            feature.Techo[g].X,
                            feature.Techo[g].Y,
                            feature.Techo[g].Z
                            )
                        );
            }

            Vector2[] uvs = LayerHelper.uvear(feature);
            for (int g = 0; g < feature.Techo.Count; g++)
            {
                //Vector3 size = Feature.boundingBox.Max + Feature.boundingBox.Min;

                //uvs[i] = new Vector2(Feature.Vertices[i].Position.X / size.X, Feature.Vertices[i].Position.Z / size.Z);

                feature.Vertices[g].TextureCoordinate = uvs[g];//new Vector2(Feature.Vertices[g].Position.X / size.X, Feature.Vertices[g].Position.Z / size.Z);


            }
        }
    }
}
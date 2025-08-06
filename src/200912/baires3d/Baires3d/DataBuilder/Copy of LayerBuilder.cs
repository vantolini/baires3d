using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SevenZip;
using SharpMap.Data;
using SharpMap.Data.Providers.ShapeFile;
using SharpMap.Geometries;

namespace ar3d {
    public class LayerBuilder {
        public float ManzanaHeight = 0.03f;
        public string Altura = "";
        public int HeightField = -1;

        Vector2 textureUpperLeft = new Vector2(0.0f, 0.0f);
        Vector2 textureUpperRight = new Vector2(1.0f, 0.0f);
        Vector2 textureLowerLeft = new Vector2(0.0f, 1.0f);
        Vector2 textureLowerRight = new Vector2(1.0f, 1.0f);

        Random r = new Random(23);
        private float startingPoint = 0;
        public Vector3 getFinalVector(Vector3 vec)
        {
            return getFinalVector(vec.Z, vec.Y, vec.X);
        }
        public Vector3 getFinalVector(float Z, float Y, float X){
            

            string[] vals = new string[2];
            vals[0] = X.ToString().Replace(",", "").Replace("-", "");
            vals[1] = Z.ToString().Replace(",", "").Replace("-", "");


            int add = 7 - vals[0].Length;
            if (add < 0) {
                vals[0] = vals[0].Substring(0, 7);
            }
            if (add > 0) {
                for (int e = 0; e < add; e++) {
                    if (e == (add - 1)) {
                        vals[0] += "1";
                    }
                    else {
                        vals[0] += "0";
                    }
                }
            }
            int add2 = 7 - vals[1].Length;
            if (add2 < 0) {
                vals[1] = vals[1].Substring(0, 7);
            }
            if (add2 > 0) {
                for (int e = 0; e < add2; e++) {
                    if (e == (add2 - 1)) {
                        vals[1] += "1";
                    }
                    else {
                        vals[1] += "0";
                    }
                }
            }



            float fX = float.Parse(vals[0]);
            float fY = Convert.ToSingle(Y);
            float fZ = float.Parse(vals[1]);


            float divisor = 300f;

            if (startingPoint == 0) {
                startingPoint = (fZ /divisor);
            }
            return new Vector3(
                (fZ/ divisor) - startingPoint,
                fY,
                (fX / divisor) - startingPoint
            );
            /*return new Vector3(
                X,
                Y,
                Z
            );
            
             return new Vector3(
                (fZ / divisor) - startingPoint,
                fY,
                (fX / divisor) - startingPoint
            );

             */
            //return new Vector3(fX, fY, fZ);
        }

        private static MapProjection mp = new MapProjection();
        public static Vector3 getVector(double x, double y, double z) {

            return new Vector3((float)x, (float)y, (float)z);
            return mp.Project(x, y, z);
        }
        




        public List<string> getUniqueValue(List<Feature> Features)
        {
            List<string> strings = new List<string>();

            for (int i = 0; i < Features.Count; i++) {
                if (!strings.Contains(Features[i].Data[3])) {
                    //Console.WriteLine("\"" + Polygons[i].Data[7] + "\", ");
                    strings.Add(Features[i].Data[3]);
                }
            }
            return strings;
        }

        
            
        public void BuildProvincias()
        {
            MeshLayerWriter meshWriter = new MeshLayerWriter();
            SerializedComponentInfo nfo = new SerializedComponentInfo();
            nfo.Author = "Victor";
            nfo.Type = ComponentType.Mesh;
            nfo.DateCreation = "23/05/1983";


            HeightField = -1;
            Altura = ManzanaHeight.ToString();

            List<Feature> Polygons = ParsePolygons(@"C:\b3d\Work\shp\Mesh\Provincias\Provincias.shp");
            //Build3d(Polygons);
            //GenerateCoords(Polygons);
            Build2d(Polygons);

            /*
            List<string> Provincias = getUniqueValue(Polygons);
            */


            Color CurrentColor = new Color(255, 128, 0);
            for (int p = 0; p < Constants.Provincias.Length; p++) {
                SerializedMesh smProvincias = new SerializedMesh();
                smProvincias.Info = nfo;
                int VxAmount = 0;
                List<PositionNormalTextureColor> ExtraVertices = new List<PositionNormalTextureColor>();
                List<int> ExtraIndices = new List<int>();
                CurrentColor = GetRandomColor();

                for (int m = 0; m < Polygons.Count; m++){
                    if (Polygons[m].Data[7] == Constants.Provincias[p]) {
                        Polygons[m].BoundingSphere = BoundingSphere.CreateFromPoints(Polygons[m].Points);

                        Polygons[m].Name = Polygons[m].Data[4];
                        Polygons[m].VertexStart = ExtraVertices.Count;
                        
                        for (int x = 0; x < Polygons[m].Vertices.Length; x++){
                            PositionNormalTextureColor pc = new PositionNormalTextureColor();
                            pc.Position = getFinalVector(Polygons[m].Vertices[x].Position);
                            pc.TextureCoordinate = Polygons[m].Vertices[x].TextureCoordinate;
                            pc.Color = CurrentColor;
                            pc.Normal = Polygons[m].Vertices[x].Normal;
                            ExtraVertices.Add(pc);
                        }
                        for (int xx = 0; xx < Polygons[m].Indices.Length; xx++){
                            ExtraIndices.Add(VxAmount + Polygons[m].Indices[xx]);
                        }
                        VxAmount += Polygons[m].Vertices.Length;
                        Polygons[m].VertexEnd = ExtraVertices.Count;
                    }
                }

                
                smProvincias.Name = Constants.Provincias[p];

                meshWriter.Serialize(smProvincias, 
                    @"C:\b3d\build\Data\Provincias\", 
                    ExtraVertices.ToArray(), 
                    ExtraIndices.ToArray(),
                    false,
                    true
                );
            }

            SerializeIndex(
                "Provincias",
                @"C:\b3d\build\Data\",
                Constants.Provincias,
                LayerFields.Mesh
            );
        }
        public  void SerializeIndex(string name, string path,  string[] layers, byte layerType) {
            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter BinaryWriter = new BinaryWriter(memoryStream);
            BinaryWriter.Write(LayerFields.LayerGroup);
            BinaryWriter.Write(layerType);
            BinaryWriter.Write(layers.Length);

            for (int xx = 0; xx < layers.Length; xx++) {
                
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
        public void BuildProvincias22()
        {
            SerializedComponentInfo nfo = new SerializedComponentInfo();
            nfo.Author = "Victor";
            nfo.Type = ComponentType.Mesh;
            nfo.DateCreation = "23/05/1983";


            HeightField = -1;
            Altura = ManzanaHeight.ToString();

            List<Feature> Polygons = ParsePolygons(@"C:\b3d\Work\shp\Mesh\Provincias\Provincias.shp");
            //Build3d(Polygons);
            //GenerateCoords(Polygons);
            Build2d(Polygons);

            /*
            List<string> Provincias = getUniqueValue(Polygons);
            */
            SerializedMesh smProvincias = new SerializedMesh();
            smProvincias.Info = nfo;

            Color CurrentColor = new Color(255, 128, 0);
            for (int p = 0; p < Constants.Provincias.Length; p++) {
                int VxAmount = 0;
                List<PositionNormalTextureColor> ExtraVertices = new List<PositionNormalTextureColor>();
                List<int> ExtraIndices = new List<int>();
                CurrentColor = GetRandomColor();

                for (int m = 0; m < Polygons.Count; m++){
                    if (Polygons[m].Data[7] == Constants.Provincias[p]) {
                        Polygons[m].BoundingSphere = BoundingSphere.CreateFromPoints(Polygons[m].Points);

                        Polygons[m].Name = Polygons[m].Data[4];
                        Polygons[m].VertexStart = ExtraVertices.Count;
                        
                        for (int x = 0; x < Polygons[m].Vertices.Length; x++){
                            PositionNormalTextureColor pc = new PositionNormalTextureColor();
                            pc.Position = getFinalVector(Polygons[m].Vertices[x].Position);
                            pc.TextureCoordinate = Polygons[m].Vertices[x].TextureCoordinate;
                            pc.Color = CurrentColor;
                            pc.Normal = Polygons[m].Vertices[x].Normal;
                            ExtraVertices.Add(pc);
                        }
                        for (int xx = 0; xx < Polygons[m].Indices.Length; xx++){
                            ExtraIndices.Add(VxAmount + Polygons[m].Indices[xx]);
                        }
                        VxAmount += Polygons[m].Vertices.Length;
                        Polygons[m].VertexEnd = ExtraVertices.Count;
                    }
                }

                SerializeMeshStream(@"C:\b3d\build\Data\Mesh\Provincias\" + Constants.Provincias[p],
                    ExtraVertices.ToArray(), ExtraIndices.ToArray());
                smProvincias.Meshes.Add(
                    new SerializedMeshPartDescriptor(
                        @"Provincias\" + Constants.Provincias[p] + ".vx.bin",
                        @"Provincias\" + Constants.Provincias[p] + ".ix.bin",
                        ExtraVertices.Count,
                        ExtraIndices.Count,
                        MeshType.Provincias,
                        Constants.Provincias[p]
                    )
                );
            }

            smProvincias.Name = "Provincias";
            SerializeLayer(smProvincias, @"C:\b3d\build\Data\Mesh\Provincias.bin");
            Console.WriteLine("Wrote Provincias.bin");
        }

        public void BuildPartidos() {
            SerializedComponentInfo nfo = new SerializedComponentInfo();
            nfo.Author = "Victor";
            nfo.Type = ComponentType.Mesh;
            nfo.DateCreation = "23/05/1983";


            HeightField = -1;
            Altura = ManzanaHeight.ToString();

            List<Feature> Polygons = ParsePolygons(@"C:\b3d\Work\shp\Mesh\Partidos\Partidos.shp");
            //Build3d(Polygons);
            //GenerateCoords(Polygons);
            Build2d(Polygons);

            /*
            List<string> Provincias = getUniqueValue(Polygons);
            */
            SerializedMesh smProvincias = new SerializedMesh();
            smProvincias.Info = nfo;

            Color CurrentColor = new Color(255, 128, 0);
            for (int p = 0; p < Constants.Provincias.Length; p++) {
                int VxAmount = 0;
                List<PositionNormalTextureColor> ExtraVertices = new List<PositionNormalTextureColor>();
                List<int> ExtraIndices = new List<int>();
                CurrentColor = GetRandomColor();

                for (int m = 0; m < Polygons.Count; m++) {
                    if (Polygons[m].Data[7] == Constants.Provincias[p]) {
                        Polygons[m].BoundingSphere = BoundingSphere.CreateFromPoints(Polygons[m].Points);

                        Polygons[m].Name = Polygons[m].Data[4];
                        Polygons[m].VertexStart = ExtraVertices.Count;

                        for (int x = 0; x < Polygons[m].Vertices.Length; x++) {
                            PositionNormalTextureColor pc = new PositionNormalTextureColor();
                            pc.Position = getFinalVector(Polygons[m].Vertices[x].Position);
                            pc.TextureCoordinate = Polygons[m].Vertices[x].TextureCoordinate;
                            pc.Color = CurrentColor;
                            pc.Normal = Polygons[m].Vertices[x].Normal;
                            ExtraVertices.Add(pc);
                        }
                        for (int xx = 0; xx < Polygons[m].Indices.Length; xx++) {
                            ExtraIndices.Add(VxAmount + Polygons[m].Indices[xx]);
                        }
                        VxAmount += Polygons[m].Vertices.Length;
                        Polygons[m].VertexEnd = ExtraVertices.Count;
                    }
                }

                SerializeMeshStream(@"C:\b3d\build\Data\Mesh\Partidos\" + Constants.Provincias[p],
                    ExtraVertices.ToArray(), ExtraIndices.ToArray());
                smProvincias.Meshes.Add(
                    new SerializedMeshPartDescriptor(
                        @"Partidos\" + Constants.Provincias[p] + ".vx.bin",
                        @"Partidos\" + Constants.Provincias[p] + ".ix.bin",
                        ExtraVertices.Count,
                        ExtraIndices.Count,
                        MeshType.Provincias,
                        Constants.Provincias[p]
                    )
                );
            }

            smProvincias.Name = "Partidos";
            SerializeLayer(smProvincias, @"C:\b3d\build\Data\Mesh\Partidos.bin");
            Console.WriteLine("Wrote Partidos.bin");
        }
        public void BuildStreets() {
            StreetLayerWriter StreetWriter = new StreetLayerWriter();

            HeightField = -1;
            Altura = ManzanaHeight.ToString();
            AnchoCamino = "0.1";
            AnchoAvenida = "0.12";
            List<Feature> Streets = ParsePolyLine(@"C:\b3d\Work\shp\Mesh\Calles\Argentina_sp2.shp");
            //Build3d(Polygons);
            //GenerateCoords(Streets);
            //Build2d(Streets);

            StreetWriter.Serialize("Argentina", @"C:\b3d\build\Data\Calles\", Streets);

            SerializeIndex(
                "Calles",
                @"C:\b3d\build\Data\",
                new string[] { "Argentina" },
                LayerFields.Streets
            );
            /*GenerateCoordsStreet(Streets);

            Color CurrentColor = new Color(255, 128, 0);
            SerializedMesh smCalles = new SerializedMesh();
            smCalles.Info = nfo;
            int VxAmount = 0;
            List<PositionNormalTextureColor> ExtraVertices = new List<PositionNormalTextureColor>();
            List<int> ExtraIndices = new List<int>();
            

            for (int m = 0; m < Streets.Count; m++) {
                CurrentColor = GetRandomColor();
                Streets[m].BoundingSphere = BoundingSphere.CreateFromPoints(Streets[m].Points);

                Streets[m].Name = Streets[m].Data[2];
                Streets[m].VertexStart = ExtraVertices.Count;

                for (int x = 0; x < Streets[m].Vertices.Length; x++) {
                    PositionNormalTextureColor pc = new PositionNormalTextureColor();
                    pc.Position = getFinalVector(Streets[m].Vertices[x].Position);
                    pc.TextureCoordinate = Streets[m].Vertices[x].TextureCoordinate;
                    pc.Color = CurrentColor;
                    pc.Normal = Streets[m].Vertices[x].Normal;
                    ExtraVertices.Add(pc);
                }
                for (int xx = 0; xx < Streets[m].Indices.Length; xx++) {
                    ExtraIndices.Add(VxAmount + Streets[m].Indices[xx]);
                }
                VxAmount += Streets[m].Vertices.Length;
                Streets[m].VertexEnd = ExtraVertices.Count;
            }
                


            smCalles.Name ="Calles";

            meshWriter.Serialize(smCalles, @"C:\b3d\build\Data\Mesh\Calles\", ExtraVertices.ToArray(), ExtraIndices.ToArray());
        */


        }
        public void GenerateCoordsStreet(List<Feature> Features) {
            for (int i = 0; i < Features.Count; i++) {
                Features[i].Indices = new int[Features[i].Vertices.Length];
                for (int x = 0; x < Features[i].Indices.Length; x++) {
                    Features[i].Indices[x] = x;
                }


                for (int g = 0; g < Features[i].Vertices.Length; g++)
                    Features[i].Vertices[g].Normal = new Vector3(0, 0, 0);

                for (int g = 0; g < Features[i].Indices.Length / 3; g++) {
                    Vector3 firstvec = Features[i].Vertices[Features[i].Indices[g * 3 + 1]].Position -
                                       Features[i].Vertices[Features[i].Indices[g * 3]].Position;
                    Vector3 secondvec = Features[i].Vertices[Features[i].Indices[g * 3]].Position -
                                        Features[i].Vertices[Features[i].Indices[g * 3 + 2]].Position;
                    Vector3 normal = Vector3.Cross(firstvec, secondvec);
                    normal.Normalize();
                    Features[i].Vertices[Features[i].Indices[g * 3]].Normal += normal;
                    Features[i].Vertices[Features[i].Indices[g * 3 + 1]].Normal += normal;
                    Features[i].Vertices[Features[i].Indices[g * 3 + 2]].Normal += normal;
                }

                for (int g = 0; g < Features[i].Vertices.Length; g++) {
                    Features[i].Vertices[g].Color = GetRandomColor();
                }

                for (int x = 0; x < Features[i].Vertices.Length - 5; x += 6) {
                    Features[i].Vertices[x].TextureCoordinate = textureUpperLeft;
                    Features[i].Vertices[x + 1].TextureCoordinate = textureUpperRight;
                    Features[i].Vertices[x + 2].TextureCoordinate = textureLowerLeft;
                    Features[i].Vertices[x + 3].TextureCoordinate = textureLowerLeft;
                    Features[i].Vertices[x + 4].TextureCoordinate = textureUpperRight;
                    Features[i].Vertices[x + 5].TextureCoordinate = textureLowerRight;
                }
            }
        }

        public void BuildPoints(string nombre) {
            SerializedComponentInfo nfo = new SerializedComponentInfo();
            nfo.Author = "Victor";
            nfo.Type = ComponentType.Point;
            nfo.DateCreation = "23/05/1983";


            HeightField = -1;
            Altura = ManzanaHeight.ToString();

            List<Feature> Points = ParsePoints(@"C:\b3d\Work\shp\Point\" + nombre + ".shp");
            //Build3d(Polygons);
            //GenerateCoords(Polygons);
            //Build2d(Polygons);



            SerializedPoints pointlayer = new SerializedPoints();
            pointlayer.Info = nfo;
            List<string> pts = getUniqueValue(Points);

            foreach (Feature feature in Points) {
                SerializedPoint point = new SerializedPoint();
                point.TextureIndex = pts.IndexOf(feature.Data[3]);

                point.Position = getFinalVector(feature.Points[0]);
                point.Name = feature.Data[2];
                point.DisplayLower = 20;
                point.DisplayHigher = 20;

                //pointlayer.Points = new List<SerializedPoint>();
                pointlayer.Points.Add(point);
            }

            pointlayer.Name = nombre;
            PointLayerWriter plw = new PointLayerWriter();
            plw.Serialize(pointlayer, @"C:\b3d\build\Data\" + nombre + "\\");
            SerializeIndex(
                nombre,
                @"C:\b3d\build\Data\",
                new[] { nombre },
                LayerFields.Points
            );
            Console.WriteLine("Wrote " + nombre);
        }


        public void BuildPolygonLayer(string layerPath)
        {
            SerializedComponentInfo nfo = new SerializedComponentInfo();
            nfo.Author = "Victor";
            nfo.Type = ComponentType.Mesh;
            nfo.DateCreation = "23/05/1983";


            HeightField = -1;
            Altura = ManzanaHeight.ToString();

            List<Feature> Manzanas = ParseHeightmapPolygons(layerPath);
            //Build3d(Manzanas);
            //GenerateCoords(Manzanas);
            Build2d(Manzanas);

                SerializedMesh smManzanas = new SerializedMesh();
            smManzanas.Info = nfo;


            int VxAmount = 0;

            List<PositionNormalTextureColor> ManzanaVertices = new List<PositionNormalTextureColor>();
            List<int> ManzanaIndices = new List<int>();

            for (int m = 0; m < Manzanas.Count; m++) {
                Manzanas[m].BoundingSphere = BoundingSphere.CreateFromPoints(Manzanas[m].Points);

                Manzanas[m].Name = Manzanas[m].Data[2];
                Manzanas[m].VertexStart = ManzanaVertices.Count;
                Color CurrentColor = new Color(Manzanas[m].Vertices[0].Position.Y, Manzanas[m].Vertices[0].Position.Y, 0.3f);

                for (int x = 0; x < Manzanas[m].Vertices.Length; x++) {
                    PositionNormalTextureColor pc = new PositionNormalTextureColor();
                    pc.Position = getFinalVector(Manzanas[m].Vertices[x].Position);
                    pc.Color = CurrentColor;
                    pc.TextureCoordinate = Manzanas[m].Vertices[x].TextureCoordinate;
                    pc.Normal = Manzanas[m].Vertices[x].Normal;
                    ManzanaVertices.Add(pc);
                }
                for (int xx = 0; xx < Manzanas[m].Indices.Length; xx++) {
                    ManzanaIndices.Add(VxAmount + Manzanas[m].Indices[xx]);
                }
                VxAmount += Manzanas[m].Vertices.Length;
                Manzanas[m].VertexEnd = ManzanaVertices.Count;
            }
            
            smManzanas.Name = "Terrain";
            MeshLayerWriter meshWriter = new MeshLayerWriter();

            meshWriter.Serialize(smManzanas,
                @"C:\b3d\build\Data\Terrain\",
                ManzanaVertices.ToArray(),
                ManzanaIndices.ToArray(),
                false,
                true
            );

            SerializeIndex(
"Terrain",
@"C:\b3d\build\Data\",
new string[] { "Terrain" },
LayerFields.Mesh
);


/*
            SerializeIndex(
                "Manzanas",
                @"C:\b3d\build\Data\Ciudades\Capital Federal\",
                Constants.Barrios,
                LayerFields.Mesh
            );*/

        }

        public List<Feature> ParseTerrainPoints(string LayerPath) {
            IEnumerable<uint> indexes;
            ShapeFileProvider sf;
            float current_height = 0;
            List<Feature> Features = new List<Feature>();
            Feature feature;
            Features.Clear();
            sf = new ShapeFileProvider(LayerPath);
            sf.Open(false);

            

            //indexes = sf.GetIntersectingObjectIds(sf.GetExtents());

            int current_feature = 0;
            int featureCount = sf.GetFeatureCount();
            for(uint idx = 0; idx < featureCount ;idx++) {
                feature = new Feature();
                FeatureDataRow Data = sf.GetFeature(idx);
                for (int q = 0; q < Data.Table.Columns.Count; q++) {
                    feature.Data.Add(Data[q].ToString());
                }

                Geometry geometry = sf.GetGeometryById(idx);
                feature.Points = new List<Vector3>();

                switch (sf.ShapeType) {
                    case ShapeType.Point:
                        #region Point
                        SharpMap.Geometries.Point pnt = geometry as SharpMap.Geometries.Point;

                        if (pnt != null) {
                            feature.Points.Add(
                                getVector(
                                    pnt.X,
                                    0,
                                    pnt.Y
                                )
                            );
                        }
                        else {
                            continue;
                        }
                        break;
                        #endregion
                    default:
                        Console.Write(sf.ShapeType.ToString());
                        break;
                }
                Features.Add(feature);
                current_feature++;
            }
            sf.Close();
            return Features;
        }


        public void BuildTerrain(string layerPath) {
            SerializedComponentInfo nfo = new SerializedComponentInfo();
            nfo.Author = "Victor";
            nfo.Type = ComponentType.Mesh;
            nfo.DateCreation = "23/05/1983";


            HeightField = -1;
            Altura = ManzanaHeight.ToString();

            List<Feature> Features = ParseTerrainPoints(layerPath);



            int rows = 640;
            int row_width = 640;
            int yinc = 0;
            int y = 0;
            int cuanto = 0;
            List<Vector3> VertexSoup = new List<Vector3>();

            while (yinc < 610) {
                for (int x = 0; x < rows - 1; x++) {
                    /*Console.Write(
                        "Armo " + x + ":\n" +
                        Features[y + x].Data[0] + ", " +
                        Features[y + x + 1].Data[0] + ", " +
                        Features[y + x + 7].Data[0] + "\n" +
                        Features[y + x + 1].Data[0] + ", " +
                        Features[y + x + 8].Data[0] + ", " +
                        Features[y + x + 7].Data[0] + "\n\n"
                        );*/
                    VertexSoup.Add(
                        getFinalVector(
                            Features[y + x].Points[0].Z,
                            -(float.Parse(Features[y + x].Data[2]) / 100),
                            Features[y + x].Points[0].X
                        )
                    );
                    VertexSoup.Add(
                        getFinalVector(
                            Features[y + x + 1].Points[0].Z,
                            -(float.Parse(Features[y + x + 1].Data[2]) / 100),
                            Features[y + x + 1].Points[0].X
                        )
                    );
                    VertexSoup.Add(
                        getFinalVector(
                            Features[y + x + row_width].Points[0].Z,
                            -(float.Parse(Features[y + x + row_width].Data[2]) / 100),
                            Features[y + x + row_width].Points[0].X
                        )
                    );
                    ///////////
                    VertexSoup.Add(
                        getFinalVector(
                            Features[y + x + 1].Points[0].Z,
                            -(float.Parse(Features[y + x + 1].Data[2]) / 100),
                            Features[y + x + 1].Points[0].X
                        )
                    );
                    VertexSoup.Add(
                        getFinalVector(
                            Features[y + x + row_width + 1].Points[0].Z,
                            -(float.Parse(Features[y + x + row_width + 1].Data[2]) / 100),
                            Features[y + x + row_width + 1].Points[0].X
                        )
                    );
                    VertexSoup.Add(
                        getFinalVector(
                            Features[y + x + row_width].Points[0].Z,
                            -(float.Parse(Features[y + x + row_width].Data[2]) / 100),
                            Features[y + x + row_width].Points[0].X
                        )
                    );
                }
                y += 640;
                yinc++;
            }

            int[] Indices = new int[VertexSoup.Count];
            for (int i = 0; i < VertexSoup.Count; i++) {
                Indices[i] = i;
            }
            PositionNormalTextureColor[] Vertices = new PositionNormalTextureColor[VertexSoup.Count];

            for (int i = 0; i < Vertices.Length; i++) {
                Vertices[i].Position = VertexSoup[i];
            }


            for (int g = 0; g < Vertices.Length; g++)
                Vertices[g].Normal = new Vector3(0, 0, 0);

            for (int g = 0; g < Indices.Length / 3; g++) {
                Vector3 firstvec = Vertices[Indices[g * 3 + 1]].Position -
                                   Vertices[Indices[g * 3]].Position;
                Vector3 secondvec = Vertices[Indices[g * 3]].Position -
                                   Vertices[Indices[g * 3 + 2]].Position;
                Vector3 normal = Vector3.Cross(firstvec, secondvec);
                normal.Normalize();
                Vertices[Indices[g * 3]].Normal += normal;
                Vertices[Indices[g * 3 + 1]].Normal += normal;
                Vertices[Indices[g * 3 + 2]].Normal += normal;
            }


            for (int g = 0; g < Vertices.Length; g++) {
                Vertices[g].Color = Color.Silver;
            }

            for (int x = 0; x < Vertices.Length - 5; x += 6) {
                Vertices[x].TextureCoordinate = textureUpperLeft;
                Vertices[x + 1].TextureCoordinate = textureUpperRight;
                Vertices[x + 2].TextureCoordinate = textureLowerLeft;
                Vertices[x + 3].TextureCoordinate = textureLowerLeft;
                Vertices[x + 4].TextureCoordinate = textureUpperRight;
                Vertices[x + 5].TextureCoordinate = textureLowerRight;
            }
          
            SerializedMesh smManzanas = new SerializedMesh();
            smManzanas.Info = nfo;

            smManzanas.Name = "Terrain";
            MeshLayerWriter meshWriter = new MeshLayerWriter();

            meshWriter.Serialize(smManzanas,
                @"C:\b3d\build\Data\Terrain\",
                Vertices.ToArray(),
                Indices.ToArray(),
                false,
                false
            );

            VerticesTerrain = Vertices;
            IndicesTerrain = Indices;

            SerializeIndex(
                "Terrain",
                @"C:\b3d\build\Data\",
                new string[] { "Terrain" },
                LayerFields.Mesh
                );

        }

        private PositionNormalTextureColor[] VerticesTerrain;
        private int[] IndicesTerrain;

        public void BuildLayers(){
            if(true){
                ///BuildPolygonLayer(@"C:\b3d\Work\shp\Mesh\DEM\Export_Output.shp");
                BuildTerrain(@"C:\b3d\Work\shp\Mesh\DEM\rastert_bsass3.shp");
                BuildCapital();
                
                BuildStreets();
                BuildPoints("OSM");
                BuildProvincias();
            }
        }

        public void BuildLotes(SerializedComponentInfo nfo)
        {
            for (int b = 0; b < Constants.Barrios.Length; b++) {
                SerializedMesh smLotes = new SerializedMesh();
                smLotes.Info = nfo;
                HeightField = 12;
                Altura = ManzanaHeight.ToString();
                List<Feature> Lotes = ParsePolygons(
                    @"C:\b3d\Work\shp\Mesh\Lotes\lotes_" + Constants.Barrios[b] + ".shp"
                    );
                Build3d(Lotes);
                GenerateCoords(Lotes);

                int VxAmount = 0;
                List<PositionNormalTextureColor> LotesVertices = new List<PositionNormalTextureColor>();
                List<int> LotesIndices = new List<int>();

                for (int m = 0; m < Lotes.Count; m++) {
                    if (Lotes[m].Data[2] == Constants.Barrios[b] || (
                        Lotes[m].Data[2] == "Parque Saavedra" && Constants.Barrios[b] == "Parque Avellaneda"
                        )) {
                        Lotes[m].BoundingSphere = BoundingSphere.CreateFromPoints(Lotes[m].Points);

                        Lotes[m].Name = Lotes[m].Data[4];
                        Lotes[m].VertexStart = LotesVertices.Count;
                        Color CurrentColor = new Color(255, 128, 0);

                        for (int x = 0; x < Lotes[m].Vertices.Length; x++) {
                            PositionNormalTextureColor pc = new PositionNormalTextureColor();
                            pc.Position = getFinalVector(Lotes[m].Vertices[x].Position);


                            pc.Color = GetRandomColor();
                            pc.TextureCoordinate = Lotes[m].Vertices[x].TextureCoordinate;
                            pc.Normal = Lotes[m].Vertices[x].Normal;
                            LotesVertices.Add(pc);
                        }
                        for (int xx = 0; xx < Lotes[m].Indices.Length; xx++) {
                            LotesIndices.Add(VxAmount + Lotes[m].Indices[xx]);
                        }
                        VxAmount += Lotes[m].Vertices.Length;
                        Lotes[m].VertexEnd = LotesVertices.Count;
                    }
                }

                smLotes.Name = Constants.Barrios[b];
                MeshLayerWriter meshWriter = new MeshLayerWriter();
                meshWriter.Serialize(smLotes,
                    @"C:\b3d\build\Data\Ciudades\Capital Federal\Lotes\",
                    LotesVertices.ToArray(),
                    LotesIndices.ToArray(),
                    false,
                    false
                );

            }

        }

        public void BuildManzanasOverTerrain(SerializedComponentInfo nfo) {
            List<Feature> Manzanas = ParsePolygons(@"C:\b3d\Work\shp\Mesh\Manzanas\Manzanas.shp");
            Build3d(Manzanas);
            GenerateCoords(Manzanas);

            for (int b = 0; b < Constants.Barrios.Length; b++){
                SerializedMesh smManzanas = new SerializedMesh();
                smManzanas.Info = nfo;


                int VxAmount = 0;

                List<PositionNormalTextureColor> ManzanaVertices = new List<PositionNormalTextureColor>();
                List<int> ManzanaIndices = new List<int>();

                for (int m = 0; m < Manzanas.Count; m++){
                    if (Manzanas[m].Data[8] == Constants.Barrios[b]){
                        Manzanas[m].BoundingSphere = BoundingSphere.CreateFromPoints(Manzanas[m].Points);

                        Manzanas[m].Name = Manzanas[m].Data[4];
                        Manzanas[m].VertexStart = ManzanaVertices.Count;
                        Color CurrentColor = new Color(255, 128, 0);
                        if (Manzanas[m].Data[9] == "plaz11a"){
                            CurrentColor = Color.LightGreen;
                        }
                        else{
                            if (Manzanas[m].Data[9] == "parque" || Manzanas[m].Data[9] == "plaza"){
                                CurrentColor = Color.DarkGreen;
                            }
                        }

                        for (int x = 0; x < Manzanas[m].Vertices.Length; x++){
                            PositionNormalTextureColor pc = new PositionNormalTextureColor();
                            pc.Position = getFinalVector(Manzanas[m].Vertices[x].Position);
                            pc.Color = CurrentColor;
                            pc.TextureCoordinate = Manzanas[m].Vertices[x].TextureCoordinate;
                            pc.Normal = Manzanas[m].Vertices[x].Normal;
                            ManzanaVertices.Add(pc);
                        }
                        for (int xx = 0; xx < Manzanas[m].Indices.Length; xx++){
                            ManzanaIndices.Add(VxAmount + Manzanas[m].Indices[xx]);
                        }
                        VxAmount += Manzanas[m].Vertices.Length;
                        Manzanas[m].VertexEnd = ManzanaVertices.Count;
                    }
                }
                Ray r;
                for (int i = 0; i < ManzanaVertices.Count; i++){
                    r = new Ray(ManzanaVertices[i].Position, Vector3.Down);

                    float fClosestPoly = float.MaxValue;
                    int primitiveCount = VerticesTerrain.Length / 3;
                    int polyIndex = 0;

                    for (int x = 0; x < primitiveCount; x++) {
                        
                        float mydist = Vector3.Distance(
                            VerticesTerrain[IndicesTerrain[x * 3 + 0]].Position,
                            r.Position);

                        if (mydist > 0.41f) {
                            continue;
                        }
                        float fDist;

                        if (Intersection.RayTriangleIntersect(
                            r,
                            VerticesTerrain[IndicesTerrain[x * 3 + 0]].Position,
                            VerticesTerrain[IndicesTerrain[x * 3 + 1]].Position,
                            VerticesTerrain[IndicesTerrain[x * 3 + 2]].Position,
                            out fDist)) {
                            if (fDist < fClosestPoly) {
                                polyIndex = x;
                                fClosestPoly = fDist;
                            }
                        }
                    }
                    

                    //Console.Write(fClosestPoly + ", ");
                    float height = VerticesTerrain[IndicesTerrain[polyIndex*3 + 0]].Position.Y;


                    PositionNormalTextureColor pntc = ManzanaVertices[i];
                    pntc.Position = new Vector3(
                        ManzanaVertices[i].Position.X,
                        ManzanaVertices[i].Position.Y + height,
                        ManzanaVertices[i].Position.Z
                    );

                    /*pntc.Position = new Vector3(
                        ManzanaVertices[i].Position.X,
                        ManzanaVertices[i].Position.Y + height,
                        ManzanaVertices[i].Position.Z
                    );
                    pntc.Color = ManzanaVertices[i].Color;
                    pntc.TextureCoordinate = ManzanaVertices[i].TextureCoordinate;
                    pntc.Normal = ManzanaVertices[i].Normal;
                    */
                    ManzanaVertices[i] = pntc;
                }


                smManzanas.Name = Constants.Barrios[b];
                MeshLayerWriter meshWriter = new MeshLayerWriter();

                meshWriter.Serialize(smManzanas,
                    @"C:\b3d\build\Data\Ciudades\Capital Federal\Manzanas\",
                    ManzanaVertices.ToArray(),
                    ManzanaIndices.ToArray(),
                    false,
                    true
                );
            }
        }

        public Vector3[] FindTerrainCollision(Ray r)
        {
            float fClosestPoly = float.MaxValue;
            int primitiveCount = VerticesTerrain.Length/3;
            int polyIndex = 0;

            for (int x = 0; x < primitiveCount; x++) {
                float fDist;
                float mydist = Vector3.Distance(
                    VerticesTerrain[IndicesTerrain[x * 3 + 0]].Position,
                    r.Position);

                if (mydist > 0.41f) {
                    continue;
                }
                if (Intersection.RayTriangleIntersect(
                    r,
                    VerticesTerrain[IndicesTerrain[x * 3 + 0]].Position,
                    VerticesTerrain[IndicesTerrain[x * 3 + 1]].Position,
                    VerticesTerrain[IndicesTerrain[x * 3 + 2]].Position,
                    out fDist)) {
                    if (fDist < fClosestPoly) {
                        polyIndex = x;
                        fClosestPoly = fDist;
                    }
                }
            }
            //Console.Write(fClosestPoly + ", ");
            Vector3[] polys = new Vector3[3];
            polys[0] = VerticesTerrain[IndicesTerrain[polyIndex*3 + 0]].Position;
            polys[1] = VerticesTerrain[IndicesTerrain[polyIndex*3 + 1]].Position;
            polys[2] = VerticesTerrain[IndicesTerrain[polyIndex*3 + 2]].Position;
            return polys;
        }


        public float? CheckRayTriangleIntersection(Ray CursorRay,
                       Vector3 P0,
                       Vector3 P1,
                       Vector3 P2) {
            //Use barycentric technique to check for the 
            //intersection of a ray and a triangle.
            //Returns null if no intersection, 
            //otherwise returns distance.
            float? IntersectionDistance;
            Vector3 IntersectionPoint, V0, V1, V2;
            float U, V, V1V1, V0V0, V0V1, V0V2, V1V2, Denominator;

            //First check to see if it lies within the plane defined by the points. 
            //This also gives us the distance.
            IntersectionDistance = CursorRay.Intersects(new Plane(P0, P1, P2));
            if (IntersectionDistance == null) return null;

            //If it's in the plane then we check to see if it's in the triangle.
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

            //Using the barycentric method, if U>0 and V>0 and U+V<1 
            //then it must be within the triangle
            if ((U > 0) && (V > 0) && (U + V < 1))
                return IntersectionDistance;
            else return null;
        }
        public void BuildManzanas(SerializedComponentInfo nfo) {
            List<Feature> Manzanas = ParsePolygons(@"C:\b3d\Work\shp\Mesh\Manzanas\Manzanas.shp");
            Build3d(Manzanas);
            GenerateCoords(Manzanas);

            for (int b = 0; b < Constants.Barrios.Length; b++){
                SerializedMesh smManzanas = new SerializedMesh();
                smManzanas.Info = nfo;


                int VxAmount = 0;

                List<PositionNormalTextureColor> ManzanaVertices = new List<PositionNormalTextureColor>();
                List<int> ManzanaIndices = new List<int>();

                for (int m = 0; m < Manzanas.Count; m++) {
                    if (Manzanas[m].Data[8] == Constants.Barrios[b]) {
                        Manzanas[m].BoundingSphere = BoundingSphere.CreateFromPoints(Manzanas[m].Points);

                        Manzanas[m].Name = Manzanas[m].Data[4];
                        Manzanas[m].VertexStart = ManzanaVertices.Count;
                        Color CurrentColor = new Color(255, 128, 0);
                        if (Manzanas[m].Data[9] == "plaz11a") {
                            CurrentColor = Color.LightGreen;
                        }
                        else {
                            if (Manzanas[m].Data[9] == "parque" || Manzanas[m].Data[9] == "plaza") {
                                CurrentColor = Color.DarkGreen;
                            }
                        }

                        for (int x = 0; x < Manzanas[m].Vertices.Length; x++) {
                            PositionNormalTextureColor pc = new PositionNormalTextureColor();
                            pc.Position = getFinalVector(Manzanas[m].Vertices[x].Position);
                            pc.Color = CurrentColor;
                            pc.TextureCoordinate = Manzanas[m].Vertices[x].TextureCoordinate;
                            pc.Normal = Manzanas[m].Vertices[x].Normal;
                            ManzanaVertices.Add(pc);
                        }
                        for (int xx = 0; xx < Manzanas[m].Indices.Length; xx++) {
                            ManzanaIndices.Add(VxAmount + Manzanas[m].Indices[xx]);
                        }
                        VxAmount += Manzanas[m].Vertices.Length;
                        Manzanas[m].VertexEnd = ManzanaVertices.Count;
                    }
                }




                smManzanas.Name = Constants.Barrios[b];
                MeshLayerWriter meshWriter = new MeshLayerWriter();

                meshWriter.Serialize(smManzanas,
                    @"C:\b3d\build\Data\Ciudades\Capital Federal\Manzanas\",
                    ManzanaVertices.ToArray(),
                    ManzanaIndices.ToArray(),
                    false,
                    true
                );
            }
        }

        public void BuildBarrios(SerializedComponentInfo nfo) {
            List<Feature> Low = ParsePolygons(@"C:\b3d\Work\shp\Mesh\Piso\Piso.shp");
            Build2d(Low);
            for (int b = 0; b < Constants.Barrios.Length; b++){
                SerializedMesh smBarrios = new SerializedMesh();
                smBarrios.Info = nfo;

                int VxAmount = 0;
                List<PositionNormalTextureColor> LowVertices = new List<PositionNormalTextureColor>();
                List<int> LowIndices = new List<int>();
                Color CurrentLowColor = GetRandomColor();
                for (int m = 0; m < Low.Count; m++) {
                    if (Low[m].Data[2] == Constants.Barrios[b]) {
                        Low[m].BoundingSphere = BoundingSphere.CreateFromPoints(Low[m].Points);

                        Low[m].Name = Low[m].Data[4];
                        Low[m].VertexStart = LowVertices.Count;


                        for (int x = 0; x < Low[m].Vertices.Length; x++) {
                            PositionNormalTextureColor pc = new PositionNormalTextureColor();
                            pc.Position = getFinalVector(Low[m].Vertices[x].Position);
                            pc.Color = CurrentLowColor;
                            pc.TextureCoordinate = Low[m].Vertices[x].TextureCoordinate;
                            pc.Normal = Low[m].Vertices[x].Normal;
                            LowVertices.Add(pc);
                        }
                        for (int xx = 0; xx < Low[m].Indices.Length; xx++) {
                            LowIndices.Add(VxAmount + Low[m].Indices[xx]);
                        }
                        VxAmount += Low[m].Vertices.Length;
                        Low[m].VertexEnd = LowVertices.Count;
                    }
                }


                smBarrios.Name = Constants.Barrios[b];

                MeshLayerWriter meshWriter = new MeshLayerWriter();


                meshWriter.Serialize(smBarrios,
                    @"C:\b3d\build\Data\Ciudades\Capital Federal\Barrios\",
                    LowVertices.ToArray(),
                    LowIndices.ToArray(),
                    true,
                    true
                );

            }
        }

        public void BuildCapital()
        {
            SerializedComponentInfo nfo = new SerializedComponentInfo();
            nfo.Author = "Victor";
            nfo.Type = ComponentType.Mesh;
            nfo.DateCreation = "23/05/1983";


            HeightField = -1;
            Altura = ManzanaHeight.ToString();

            Console.WriteLine("Building manzanassss");
            BuildManzanasOverTerrain(nfo);

            Console.WriteLine("Building Barriosssss");
            BuildBarrios(nfo);

            //Console.WriteLine("Building Lotessss");
            //BuildLotes(nfo);
            
                /*SevenZipCompressor tmp = new SevenZipCompressor();
                tmp.ArchiveFormat = OutArchiveFormat.SevenZip;
                tmp.CompressionLevel = CompressionLevel.Ultra;
                tmp.CompressionMethod = CompressionMethod.Ppmd;
                string[] filez = new string[6];
                filez[0] = @"C:\b3d\build\Data\Mesh\Barrios\" + Constants.Barrios[b] + ".Manzanas.ix.bin";
                filez[1] = @"C:\b3d\build\Data\Mesh\Barrios\" + Constants.Barrios[b] + ".Lotes.ix.bin";
                filez[2] = @"C:\b3d\build\Data\Mesh\Barrios\" + Constants.Barrios[b] + ".Manzanas.vx.bin";
                filez[3] = @"C:\b3d\build\Data\Mesh\Barrios\" + Constants.Barrios[b] + ".Lotes.vx.bin";
                filez[4] = @"C:\b3d\build\Data\Mesh\Barrios\" + Constants.Barrios[b] + ".Low.vx.bin";
                filez[5] = @"C:\b3d\build\Data\Mesh\Barrios\" + Constants.Barrios[b] + ".Low.ix.bin";


                //tmp.CompressFiles(@"C:\b3d\build\Data\Mesh\Barrios\z\" + Constants.Barrios[b] + ".7z", filez);
                
                */
            
            Console.WriteLine("Wrote Capital");

            SerializeIndex(
                "Barrios",
                @"C:\b3d\build\Data\Ciudades\Capital Federal\",
                Constants.Barrios,
                LayerFields.Mesh
            );
            SerializeIndex(
                "Lotes",
                @"C:\b3d\build\Data\Ciudades\Capital Federal\",
                Constants.Barrios,
                LayerFields.Mesh
            );
            SerializeIndex(
                "Manzanas",
                @"C:\b3d\build\Data\Ciudades\Capital Federal\",
                Constants.Barrios,
                LayerFields.Mesh
            );


        }
        #region build and generate coords

        public void Build2d(List<Feature> Features) {
            for (int i = 0; i < Features.Count; i++) {

                Features[i].Indices = new int[Features[i].IndicesTecho.Length];
                for (int x = 0; x < Features[i].IndicesTecho.Length; x++) {
                    Features[i].Indices[x] = Features[i].IndicesTecho[x];
                }

                Features[i].Vertices = new PositionNormalTextureColor[Features[i].Techo.Count];

                for (int g = 0; g < Features[i].Techo.Count; g++) {
                    Features[i].Vertices[g] =
                        new PositionNormalTextureColor(
                            new Vector3(
                                (float)Features[i].Techo[g].X,
                                (float)Features[i].Techo[g].Y,
                                (float)Features[i].Techo[g].Z
                            )
                        );
                }
            }


            for (int i = 0; i < Features.Count; i++) {
                for (int g = 0; g < Features[i].Vertices.Length; g++)
                    Features[i].Vertices[g].Normal = new Vector3(0, 0, 0);

                for (int g = 0; g < Features[i].Indices.Length / 3; g++) {
                    Vector3 firstvec = Features[i].Vertices[Features[i].Indices[g * 3 + 1]].Position -
                                       Features[i].Vertices[Features[i].Indices[g * 3]].Position;
                    Vector3 secondvec = Features[i].Vertices[Features[i].Indices[g * 3]].Position -
                                        Features[i].Vertices[Features[i].Indices[g * 3 + 2]].Position;
                    Vector3 normal = Vector3.Cross(firstvec, secondvec);
                    normal.Normalize();
                    Features[i].Vertices[Features[i].Indices[g * 3]].Normal += normal;
                    Features[i].Vertices[Features[i].Indices[g * 3 + 1]].Normal += normal;
                    Features[i].Vertices[Features[i].Indices[g * 3 + 2]].Normal += normal;
                }


                for (int g = 0; g < Features[i].Vertices.Length; g++) {
                    Features[i].Vertices[g].Color = Color.Silver;
                }



                for (int x = 0; x < Features[i].Vertices.Length - 5; x += 6) {
                    Features[i].Vertices[x].TextureCoordinate = textureUpperLeft;
                    Features[i].Vertices[x + 1].TextureCoordinate = textureUpperRight;
                    Features[i].Vertices[x + 2].TextureCoordinate = textureLowerLeft;
                    Features[i].Vertices[x + 3].TextureCoordinate = textureLowerLeft;
                    Features[i].Vertices[x + 4].TextureCoordinate = textureUpperRight;
                    Features[i].Vertices[x + 5].TextureCoordinate = textureLowerRight;
                }
            }

        }
        public void Build3d(List<Feature> Features) {
            double current_height = 0;
            double altura_inicial;
            for (int i = 0; i < Features.Count; i++) {
                int currentpared = 0;
                Features[i].Points.Reverse();
                Features[i].Paredes = new pVector3[Features[i].Points.Count * 6];

                altura_inicial = 0;
                current_height = Features[i].Height;
                for (int g = 0; g < Features[i].Points.Count; g++) {
                    Features[i].Paredes[currentpared] = new pVector3(Features[i].Points[g].X, altura_inicial, Features[i].Points[g].Z);
                    //Features[i].Paredes[currentpared].Color = Microsoft.Xna.Framework.Graphics.Color.HotPink;
                    currentpared++;

                    Features[i].Paredes[currentpared] = new pVector3(Features[i].Points[g].X, altura_inicial + current_height, Features[i].Points[g].Z);
                    //Features[i].Paredes[currentpared].Color = Microsoft.Xna.Framework.Graphics.Color.HotPink;
                    currentpared++;


                    if (g == (Features[i].Points.Count - 1)) {
                        Features[i].Paredes[currentpared] = new pVector3(Features[i].Points[0].X, altura_inicial, Features[i].Points[0].Z);
                    }
                    else {
                        Features[i].Paredes[currentpared] = new pVector3(Features[i].Points[g + 1].X, altura_inicial, Features[i].Points[g + 1].Z);
                    }

                    //Features[i].Paredes[currentpared].Color = Microsoft.Xna.Framework.Graphics.Color.HotPink;
                    currentpared++;

                    /***********/

                    if (g == (Features[i].Points.Count - 1)) {
                        Features[i].Paredes[currentpared] = new pVector3(Features[i].Points[0].X, altura_inicial, Features[i].Points[0].Z);
                    }
                    else {
                        Features[i].Paredes[currentpared] = new pVector3(Features[i].Points[g + 1].X, altura_inicial, Features[i].Points[g + 1].Z);
                    }

                    //Features[i].Paredes[currentpared].Color = Microsoft.Xna.Framework.Graphics.Color.Orange;
                    currentpared++;

                    Features[i].Paredes[currentpared] = new pVector3(Features[i].Points[g].X, altura_inicial + current_height, Features[i].Points[g].Z);
                    //Features[i].Paredes[currentpared].Color = Microsoft.Xna.Framework.Graphics.Color.Orange;
                    currentpared++;


                    if (g == (Features[i].Points.Count - 1)) {
                        Features[i].Paredes[currentpared] = new pVector3(Features[i].Points[0].X, altura_inicial, Features[i].Points[0].Z);
                    }
                    else {
                        Features[i].Paredes[currentpared] = new pVector3(Features[i].Points[g + 1].X, altura_inicial, Features[i].Points[g + 1].Z);
                    }

                    if (g == (Features[i].Points.Count - 1)) {
                        Features[i].Paredes[currentpared] = new pVector3(Features[i].Points[0].X, altura_inicial + current_height, Features[i].Points[0].Z);
                    }
                    else {
                        Features[i].Paredes[currentpared] = new pVector3(Features[i].Points[g + 1].X, altura_inicial + current_height, Features[i].Points[g + 1].Z);
                    }


                    //Features[i].Paredes[currentpared].Color = Microsoft.Xna.Framework.Graphics.Color.Orange;
                    currentpared++;

                }
                List<pVector3> pr = Features[i].Paredes.ToList();
                //pr.Reverse();
                for (int xx = 0; xx < pr.Count; xx++) {
                    Features[i].Paredes[xx] = pr[xx];
                }

            }

        }

        public void GenerateCoords(List<Feature> Features) {

            /*
            Vector3[] vertsbb = new Vector3[vertos.Count];
            for (int e = 0; e < vertos.Count; e++) {
                vertsbb[e] = vertos[e];
            }
            */



            for (int i = 0; i < Features.Count; i++) {
                int[] tmpindx = Features[i].IndicesTecho;
                Features[i].Indices = new int[tmpindx.Length + Features[i].Paredes.Length];
                for (int x = 0; x < tmpindx.Length; x++) {
                    Features[i].Indices[x] = tmpindx[x];
                }
                //Features[i].Paredes.Reverse();

                int[] lala = new int[Features[i].Paredes.Length];

                for (int x = 0; x < Features[i].Paredes.Length; x++) {
                    lala[x] = x;
                }

                /*              int xx = 0;
                              for (int x = Features[i].Paredes.Length - 1; x > -1; x--) {
                                  lala[xx] = x;
                                  xx++;
                              }
              */

                for (int x = 0; x < (tmpindx.Length + Features[i].Paredes.Length); x++) {
                    //Features[i].Indices[tmpindx.Length + x] = lala[x];
                    Features[i].Indices[x] = x;
                }

                Features[i].Vertices = new PositionNormalTextureColor[Features[i].Techo.Count + Features[i].Paredes.Length];

                for (int g = 0; g < Features[i].Techo.Count; g++) {
                    Features[i].Vertices[g] =
                        new PositionNormalTextureColor(
                            new Vector3(
                                (float)Features[i].Techo[g].X,
                                (float)Features[i].Techo[g].Y,
                                (float)Features[i].Techo[g].Z
                            )
                        );
                }


                for (int g = 0; g < Features[i].Paredes.Length; g++) {
                    Features[i].Vertices[Features[i].Techo.Count + g] =
                        new PositionNormalTextureColor(
                            new Vector3(
                                (float)Features[i].Paredes[g].X,
                                (float)Features[i].Paredes[g].Y,
                                (float)Features[i].Paredes[g].Z
                            )
                        );
                    /*Features[i].Vertices[Features[i].Techo.Count + g] = new PositionNormalTextureColor(
                        Features[i].Paredes[g].ConvertToVector3()
                        );*/
                }
            }


            for (int i = 0; i < Features.Count; i++) {
                for (int g = 0; g < Features[i].Vertices.Length; g++)
                    Features[i].Vertices[g].Normal = new Vector3(0, 0, 0);

                for (int g = 0; g < Features[i].Indices.Length / 3; g++) {
                    Vector3 firstvec = Features[i].Vertices[Features[i].Indices[g * 3 + 1]].Position -
                                       Features[i].Vertices[Features[i].Indices[g * 3]].Position;
                    Vector3 secondvec = Features[i].Vertices[Features[i].Indices[g * 3]].Position -
                                        Features[i].Vertices[Features[i].Indices[g * 3 + 2]].Position;
                    Vector3 normal = Vector3.Cross(firstvec, secondvec);
                    normal.Normalize();
                    Features[i].Vertices[Features[i].Indices[g * 3]].Normal += normal;
                    Features[i].Vertices[Features[i].Indices[g * 3 + 1]].Normal += normal;
                    Features[i].Vertices[Features[i].Indices[g * 3 + 2]].Normal += normal;
                }
                if (Features[i].Data.Count > 8) {
                    if (Features[i].Data[9] == "plaza") {
                        for (int g = 0; g < Features[i].Vertices.Length; g++) {
                            Features[i].Vertices[g].Color = Color.LightGreen;
                        }
                    }
                    else {
                        if (Features[i].Data[9] == "parque") {
                            for (int g = 0; g < Features[i].Vertices.Length; g++) {
                                Features[i].Vertices[g].Color = Color.DarkGreen;
                            }
                        }
                        else {
                            for (int g = 0; g < Features[i].Vertices.Length; g++) {
                                Features[i].Vertices[g].Color = GetRandomColor();
                            }
                        }
                    }
                }
                else {
                    for (int g = 0; g < Features[i].Vertices.Length; g++) {
                        Features[i].Vertices[g].Color = GetRandomColor();
                    }
                }


                for (int x = 0; x < Features[i].Vertices.Length - 5; x += 6) {
                    Features[i].Vertices[x].TextureCoordinate = textureUpperLeft;
                    Features[i].Vertices[x + 1].TextureCoordinate = textureUpperRight;
                    Features[i].Vertices[x + 2].TextureCoordinate = textureLowerLeft;
                    Features[i].Vertices[x + 3].TextureCoordinate = textureLowerLeft;
                    Features[i].Vertices[x + 4].TextureCoordinate = textureUpperRight;
                    Features[i].Vertices[x + 5].TextureCoordinate = textureLowerRight;
                }
            }
        }

        public Color GetRandomColor() {
            return new Color((byte)r.Next(0, 256), (byte)r.Next(0, 256), (byte)r.Next(0, 256));
        }
        #endregion


        public void SerializeMeshStream(string FileName, PositionNormalTextureColor[] Vertices, int[] Indices) {

            MemoryStream ms = new MemoryStream();
            BinaryWriter w = new BinaryWriter(ms);
            for (int xx = 0; xx < Vertices.Length; xx++) {


                w.Write(Vertices[xx].Position.X);
                w.Write(Vertices[xx].Position.Y);
                w.Write(Vertices[xx].Position.Z);

                w.Write(Vertices[xx].Normal.X);
                w.Write(Vertices[xx].Normal.Y);
                w.Write(Vertices[xx].Normal.Z);
                w.Write(Vertices[xx].TextureCoordinate.X);
                w.Write(Vertices[xx].TextureCoordinate.Y);

                w.Write(Vertices[xx].Color.R);
                w.Write(Vertices[xx].Color.G);
                w.Write(Vertices[xx].Color.B);
                w.Flush();
            }

            Directory.CreateDirectory(Path.GetDirectoryName(FileName + ".vx.bin"));
            FileStream fs = new FileStream(FileName + ".vx.bin", FileMode.Create);
            fs.Write(ms.GetBuffer(), 0, Convert.ToInt32(ms.Position));
            fs.Flush();
            fs.Close();
            fs.Dispose();



            MemoryStream ms2 = new MemoryStream();
            BinaryWriter ww = new BinaryWriter(ms2);
            for (int xx = 0; xx < Indices.Length; xx++) {
                ww.Write(Indices[xx]);
                ww.Flush();
            }

            Directory.CreateDirectory(Path.GetDirectoryName(FileName + ".ix.bin"));
            FileStream fs2 = new FileStream(FileName + ".ix.bin", FileMode.Create);
            fs2.Write(ms2.GetBuffer(), 0, Convert.ToInt32(ms2.Position));
            fs2.Flush();
            fs2.Close();
            fs2.Dispose();



            //scdm.Descriptors = descriptors;
        }
        public void SerializeLayer(object component, string path) {
            IFormatter formatter2 = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);

            formatter2.Serialize(stream, component);
            stream.Close();
        }
        #region parseFeatures
        public List<Feature> ParsePoints(string LayerPath) {
            IEnumerable<uint> indexes;
            ShapeFileProvider sf;
            //output = new StringBuilder(147483000);
            float current_height = 0;
            List<Feature> Features = new List<Feature>();
            Feature feature;
            Features.Clear();
            sf = new ShapeFileProvider(LayerPath);
            sf.Open(false);
            indexes = sf.GetIntersectingObjectIds(sf.GetExtents());
            
            int current_feature = 0;

            foreach (uint idx in indexes) {
                feature = new Feature();
                FeatureDataRow Data = sf.GetFeature(idx);
                for (int q = 0; q < Data.Table.Columns.Count; q++) {
                    feature.Data.Add(Data[q].ToString());
                }

                //label_feature = feature[7].ToString();//.Columns[IndxLabel].Table.Rows[0]..ToString();
               

                Geometry geometry = sf.GetGeometryById(idx);
                feature.Points = new List<Vector3>();

                switch (sf.ShapeType) {
                    case ShapeType.Point:
                        #region Point
                        SharpMap.Geometries.Point pnt = geometry as SharpMap.Geometries.Point;

                        if (pnt != null) {
                            feature.Points.Add(
                                getVector(
                                    pnt.X,
                                    0,
                                    pnt.Y
                                )
                            );

                        }
                        else {
                            continue;
                        }
                        break;
                        #endregion
                    default:
                        Console.Write(sf.ShapeType.ToString());
                        break;
                }

                Features.Add(feature);
                //Console.WriteLine(GC.GetTotalMemory(false)/1024);  GC.Collect();  Console.WriteLine(GC.GetTotalMemory(false) / 1024);

                current_feature++;
            }
            sf.Close();
            return Features;
        }


        public List<Feature> ParseHeightmapPolygons(string LayerPath) {
            IEnumerable<uint> indexes;
            ShapeFileProvider sf;

            //output = new StringBuilder(147483000);
            float current_height = 0;
            List<Feature> Features = new List<Feature>();
            Feature feature;
            Features.Clear();
            sf = new ShapeFileProvider(LayerPath);
            sf.Open(false);
            indexes = sf.GetIntersectingObjectIds(sf.GetExtents());
            int current_feature = 0;



            //StreamWriter pp = new StreamWriter(@"lallalalala.txt");
            List<string> poronga = new List<string>();
            foreach (uint idx in indexes) {
                feature = new Feature();
                FeatureDataRow Data = sf.GetFeature(idx);
                for (int q = 0; q < Data.Table.Columns.Count; q++) {
                    feature.Data.Add(Data[q].ToString());
                }

                Geometry geometry = sf.GetGeometryById(idx);
                //HeightField = 11;
                float altt = float.Parse(feature.Data[2]);
                if (altt == 0) {
                    current_height = -0.1f;
                }
                else {
                    current_height = -(altt / 100);  //Convert.ToDouble(Altura);
                }
                feature.Height = current_height;
                feature.Points = new List<Vector3>();

                switch (sf.ShapeType) {
                    case ShapeType.Polygon:
                        #region Polygon
                        SharpMap.Geometries.Polygon polygon = geometry as SharpMap.Geometries.Polygon;

                        int cnt = 0;
                        
                        if (polygon != null) {
                            foreach (SharpMap.Geometries.Point vertex in polygon.ExteriorRing.Vertices) {
                                //pp.WriteLine(vertex.X.ToString().Replace(",", ".") + ", " + vertex.Y.ToString().Replace(",", "."));
                                //
                                //current_height = 0.0000000001f;
                                if (cnt == (polygon.ExteriorRing.Vertices.Count - 1)) {
                                    break;
                                }
                                cnt++;

                                feature.Points.Add(getVector(vertex.X, current_height, vertex.Y));

                            }
                            feature.Points.Reverse();
                        }
                        else {
                            MultiPolygon multipolygon = geometry as MultiPolygon;
                            if (multipolygon != null) {
                                foreach (SharpMap.Geometries.Polygon p in multipolygon.Polygons) {
                                    foreach (SharpMap.Geometries.Point vertex2 in p.ExteriorRing.Vertices) {
                                        if (cnt == (p.ExteriorRing.Vertices.Count - 1)) {
                                            break;
                                        }
                                        cnt++;

                                        feature.Points.Add(getVector(vertex2.X, current_height, vertex2.Y));

                                    }
                                    feature.Points.Reverse();
                                }
                            }
                            else {
                                throw new Exception();
                            }
                        }
                        break;
                        #endregion
                    default:
                        Console.Write(sf.ShapeType.ToString());
                        break;
                }
                if (feature.Points.Count > 3) {

                    Vector2[] ptlist = new Vector2[feature.Points.Count];
                    for (int i = 0; i < feature.Points.Count; i++) {
                        ptlist[i] = new Vector2((float)feature.Points[i].X, (float)feature.Points[i].Z);
                    }
                    //TriangulationGLU glu = new TriangulationGLU();
                    //glu.getTessellation(ptlist);
                    int[] sourceIndices;
                    Triangulator.Triangulate(
                        ptlist,
                        WindingOrder.CounterClockwise,
                        out ptlist,
                        out sourceIndices);

                    feature.numVertices = ptlist.Length;
                    feature.numPrimitives = ptlist.Length / 3;
                    feature.Techo = new List<Vector3>();
                    for (int i = 0; i < sourceIndices.Length; i++) {
                        feature.Techo.Add(
                            new Vector3(
                                ptlist[sourceIndices[i]].X,
                                current_height,
                                ptlist[sourceIndices[i]].Y
                            )
                        );
                    }
                    if (sourceIndices.Length != ptlist.Length) {
                        //throw new Exception();
                    }

                    int[] newIndx = new int[feature.Techo.Count];
                    for (int i = 0; i < feature.Techo.Count; i++) {
                        newIndx[i] = i;
                    }

                    feature.IndicesTecho = newIndx;

                }
                else {
                    if (feature.Points.Count < 3) {
                        throw new Exception();
                    }
                    else {
                        feature.IndicesTecho = new int[feature.Points.Count];
                        for (int i = 0; i < feature.IndicesTecho.Length; i++) {
                            feature.IndicesTecho[i] = i;
                        }
                        feature.Techo = feature.Points;
                    }

                }

                Features.Add(feature);

                current_feature++;
            }
            sf.Close();


            //pp.Flush();
            //pp.Close();
            return Features;

        }

        public List<Feature> ParsePolygons(string LayerPath) {
            IEnumerable<uint> indexes;
            ShapeFileProvider sf;

            //output = new StringBuilder(147483000);
            float current_height = 0;
            List<Feature> Features = new List<Feature>();
            Feature feature;
            Features.Clear();
            sf = new ShapeFileProvider(LayerPath);
            sf.Open(false);
            indexes = sf.GetIntersectingObjectIds(sf.GetExtents());
            int current_feature = 0;



            //StreamWriter pp = new StreamWriter(@"lallalalala.txt");
            List<string> poronga = new List<string>();
            foreach (uint idx in indexes) {
                feature = new Feature();
                FeatureDataRow Data = sf.GetFeature(idx);
                for (int q = 0; q < Data.Table.Columns.Count; q++) {
                    feature.Data.Add(Data[q].ToString());
                }

                Geometry geometry = sf.GetGeometryById(idx);
                //HeightField = 11;
                if (HeightField == -1) {
                    current_height = float.Parse(Altura);  //Convert.ToDouble(Altura);
                }
                else {

                    if (feature.Data[HeightField].ToString() == "" || feature.Data[HeightField].ToString() == "0" || feature.Data[HeightField].ToString() == "1") {
                        feature.Data[HeightField] = "0.2";
                    }
                    current_height = Convert.ToSingle(feature.Data[HeightField].ToString());
                    if (current_height == 0) {
                        current_height = 1.02f;
                    }
                    current_height *= 1.02f;
                }
                feature.Height = current_height;
                feature.Points = new List<Vector3>();

                switch (sf.ShapeType) {
                    case ShapeType.Polygon:
                        #region Polygon
                        SharpMap.Geometries.Polygon polygon = geometry as SharpMap.Geometries.Polygon;

                        int cnt = 0;
                        
                        if (polygon != null) {
                            foreach (SharpMap.Geometries.Point vertex in polygon.ExteriorRing.Vertices) {
                                //pp.WriteLine(vertex.X.ToString().Replace(",", ".") + ", " + vertex.Y.ToString().Replace(",", "."));
                                //
                                //current_height = 0.0000000001f;
                                if (cnt == (polygon.ExteriorRing.Vertices.Count - 1)) {
                                    break;
                                }
                                cnt++;

                                feature.Points.Add(getVector(vertex.X, current_height, vertex.Y));

                            }
                            feature.Points.Reverse();
                        }
                        else {
                            MultiPolygon multipolygon = geometry as MultiPolygon;
                            if (multipolygon != null) {
                                foreach (SharpMap.Geometries.Polygon p in multipolygon.Polygons) {
                                    foreach (SharpMap.Geometries.Point vertex2 in p.ExteriorRing.Vertices) {
                                        if (cnt == (p.ExteriorRing.Vertices.Count - 1)) {
                                            break;
                                        }
                                        cnt++;

                                        feature.Points.Add(getVector(vertex2.X, current_height, vertex2.Y));

                                    }
                                    feature.Points.Reverse();
                                }
                            }
                            else {
                                throw new Exception();
                            }
                        }
                        break;
                        #endregion
                    default:
                        Console.Write(sf.ShapeType.ToString());
                        break;
                }
                if (LayerPath.Contains("Veredas")) {
                    //feature.Points.Reverse();
                }
                if (feature.Points.Count > 3) {

                    Vector2[] ptlist = new Vector2[feature.Points.Count];
                    for (int i = 0; i < feature.Points.Count; i++) {
                        ptlist[i] = new Vector2((float)feature.Points[i].X, (float)feature.Points[i].Z);
                    }
                    //TriangulationGLU glu = new TriangulationGLU();
                    //glu.getTessellation(ptlist);
                    int[] sourceIndices;
                    Triangulator.Triangulate(
                        ptlist,
                        WindingOrder.CounterClockwise,
                        out ptlist,
                        out sourceIndices);

                    feature.numVertices = ptlist.Length;
                    feature.numPrimitives = ptlist.Length / 3;
                    feature.Techo = new List<Vector3>();
                    for (int i = 0; i < sourceIndices.Length; i++) {
                        feature.Techo.Add(
                            new Vector3(
                                ptlist[sourceIndices[i]].X,
                                current_height,
                                ptlist[sourceIndices[i]].Y
                            )
                        );
                    }
                    if (sourceIndices.Length != ptlist.Length) {
                        //throw new Exception();
                    }

                    int[] newIndx = new int[feature.Techo.Count];
                    for (int i = 0; i < feature.Techo.Count; i++) {
                        newIndx[i] = i;
                    }

                    feature.IndicesTecho = newIndx;

                }
                else {
                    if (feature.Points.Count < 3) {
                        throw new Exception();
                    }
                    else {
                        feature.IndicesTecho = new int[feature.Points.Count];
                        for (int i = 0; i < feature.IndicesTecho.Length; i++) {
                            feature.IndicesTecho[i] = i;
                        }
                        feature.Techo = feature.Points;
                    }

                }

                Features.Add(feature);

                current_feature++;
            }
            sf.Close();


            //pp.Flush();
            //pp.Close();
            return Features;

        }
        public List<Feature> ParsePolyLine(string LayerPath) {
            IEnumerable<uint> indexes;
            ShapeFileProvider sf;
            //output = new StringBuilder(147483000);
            float current_height = 0;
            List<Feature> Features = new List<Feature>();

            Feature feature;
            Features.Clear();
            sf = new ShapeFileProvider(LayerPath);
            sf.Open(false);
            indexes = sf.GetIntersectingObjectIds(sf.GetExtents());
            List<Vector3> points = new List<Vector3>();
            int current_feature = 0;
            foreach (uint idx in indexes) {
                feature = new Feature();
                FeatureDataRow Data = sf.GetFeature(idx);
                for (int q = 0; q < Data.Table.Columns.Count; q++) {
                    feature.Data.Add(Data[q].ToString());
                }

                points.Clear();

                Geometry geometry = sf.GetGeometryById(idx);

                feature.Points = new List<Vector3>();

                switch (sf.ShapeType) {
                    case ShapeType.PolyLine:
                        #region PolyLine
                        LineString pntLine = geometry as LineString;
                        if (pntLine != null) {
                            List<Vector3> vecs = new List<Vector3>();
                            for (int i = 0; i < pntLine.Vertices.Count; i++) {
                                SharpMap.Geometries.Point vertex = pntLine.Vertices[i];

                                vecs.Add(
                                    getFinalVector(getVector(vertex.X, 0,
                                        (vertex.Y)))
                                    );

                                feature.OrigLine.Add(getFinalVector(getVector(vertex.X, 0,
                                        (vertex.Y)))
                                    );
                            }
                            feature.OrigLine.Reverse();
                            Features.Add(feature);
                            continue;


                            vecs.Reverse();
                            for (int xx = 0; xx < vecs.Count; xx++) {

                            }

                            for (int pp = 0; pp < vecs.Count - 1; pp++) {
                                if (vecs[pp].X == vecs[pp + 1].X && vecs[pp].Z == vecs[pp + 1].Z) {
                                    Vector3 veccc = vecs[pp];

                                    veccc.X = veccc.X - 0.00001f;
                                    //vecs[pp] = veccc;
                                }

                            }
                            Vector3[] vecs2 = new Vector3[vecs.Count];

                            for (int xx = 0; xx < vecs.Count; xx++) {
                                vecs2[xx] = new Vector3(vecs[xx].X, vecs[xx].Y, vecs[xx].Z);
                            }
                            List<Vector3> vertitos;
                            if (feature.Data[1].Contains("AVDA") || feature.Data[1].Contains("AV ")) {
                                vertitos =
                                    GetTriangleStrip(vecs2, Convert.ToSingle(AnchoAvenida));
                            }
                            else {
                                vertitos =
                                    GetTriangleStrip(vecs2, Convert.ToSingle(AnchoCamino));
                            }

                            int src_pos = 0, dst_pos = 0;
                            Vector3 a, b, c;
                            Vector3[] vertos = new Vector3[(vertitos.Count - 2) * 3];

                            a = vertitos[src_pos++];
                            b = vertitos[src_pos++];
                            for (int i = 0; i < vertitos.Count - 2; i++) {
                                c = vertitos[src_pos++];
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
                            for (int xx = 0; xx < vertos.Length; xx++) {
                                feature.Points.Add(vertos[xx]);
                            }
                            feature.Vertices = new PositionNormalTextureColor[feature.Points.Count];
                            feature.Points.Reverse();
                            for (int xx = 0; xx < feature.Points.Count; xx++) {
                                feature.Vertices[xx] = new PositionNormalTextureColor(
                                    feature.Points[xx]
                                    );
                            }
                        }
                        else {
                            MultiLineString pntMLine = geometry as MultiLineString;
                            for (int ee = 0; ee < pntMLine.NumGeometries; ee++) {
                                Geometry geom = pntMLine.Geometry(ee);
                                LineString pntLine2 = geom as LineString;
                                //for (int xx = 0; xx < pntLine2.Vertices.Count; xx++) {
                                    List<Vector3> vecs = new List<Vector3>();
                                    for (int i = 0; i < pntLine2.Vertices.Count; i++) {
                                        SharpMap.Geometries.Point vertex2 = pntLine2.Vertices[i];

                                        vecs.Add(
                                            getVector(
                                                vertex2.X,
                                                0,
                                                vertex2.Y)
                                            );

                                        //vecs.Add(new Vector3((float)pntLine2.Vertices[i].Y, 0, (float)pntLine2.Vertices[i].X));

                                    }
                                    Vector3[] vecs2 = new Vector3[vecs.Count];

                                    for (int xx1 = 0; xx1 < vecs.Count; xx1++) {
                                        vecs2[xx1] = vecs[xx1];
                                    }
                                    List<Vector3> vertitos =
                                        GetTriangleStrip(vecs2, Convert.ToSingle(AnchoCamino));

                                    /*
                                    for (int xxx = 0; xxx < vertitos.Count; xxx++)
                                    {
                                        feature.Points.Add(vertitos[xxx]);
                                    }
                                    */

                                    int src_pos = 0, dst_pos = 0;
                                    Vector3 a, b, c;
                                    Vector3[] vertos = new Vector3[(vertitos.Count - 2) * 3];

                                    a = vertitos[src_pos++];
                                    b = vertitos[src_pos++];
                                    for (int i = 0; i < vertitos.Count - 2; i++) {
                                        c = vertitos[src_pos++];
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



                                    for (int gxx = 0; gxx < vertos.Length; gxx++) {
                                        feature.Points.Add(vertos[gxx]);
                                    }
                                    feature.Vertices = new PositionNormalTextureColor[feature.Points.Count];
                                    for (int zxx = 0; zxx < feature.Points.Count; zxx++) {
                                        feature.Vertices[zxx] = new PositionNormalTextureColor(
                                            feature.Points[zxx]
                                            );
                                    }
                                //}
                            }
                        }

                        break;
                        #endregion
                    default:
                        Console.Write(sf.ShapeType.ToString());
                        break;
                }

                
                current_feature++;
            }
            sf.Close();
            return Features;
        }

        public int triangleCount = -2;
        public string AnchoCamino = "1";
        public string AnchoAvenida = "1";
        public List<Vector3> GetTriangleStrip(Vector3[] points, float thickness) {
            if (points.Length == 0) {
                //return null;
            }
            Vector3 lastPoint = Vector3.Zero;
            List<Vector3> list = new List<Vector3>();
            for (int i = 0; i < points.Length; i++) {
                if (i == 0) {
                    lastPoint = points[i];
                    continue;
                }
                //the direction of the current line
                Vector3 direction = lastPoint - points[i];
                direction.Normalize();
                //the perpendiculat to the current line
                Vector3 normal = Vector3.Cross(direction, Vector3.Up);
                normal.Normalize();
                Vector3 p1 = lastPoint + normal * thickness;
                triangleCount++;
                Vector3 p2 = lastPoint - normal * thickness;
                triangleCount++;
                Vector3 p3 = points[i] + normal * thickness;
                triangleCount++;
                Vector3 p4 = points[i] - normal * thickness;
                triangleCount++;

                list.Add(new Vector3(p4.X, p4.Y, p4.Z));
                list.Add(new Vector3(p3.X, p3.Y, p3.Z));
                list.Add(new Vector3(p2.X, p2.Y, p2.Z));
                list.Add(new Vector3(p1.X, p1.Y, p1.Z));
                lastPoint = points[i];
            }
            return list;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Amib.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace b3d
{
    public static class Logger{

        public static void LogLine(string s) {
            Console.WriteLine(s);
        }
    }
    public class CityBuilder : LayerBuilder
    {
        static object syncLock = new object();

        public CityBuilder(string name) {
            Name = name;
        }

        private object BuildVeredas(object barriobjekt) {
            int barriobject = (int)barriobjekt;

            string barrio = Constants.Barrios[barriobject];
            List<Feature> Manzanas;
            lock(syncLock){
                ShapefileLoader Shapefile = new ShapefileLoader(@"..\Work\shp\Mesh\Veredas\Veredas_" + barrio + ".shp");
                Shapefile.Altura = 80f;
                Shapefile.HeightField = -1;
                Manzanas = Shapefile.ParsePolygons();
                Shapefile.Dispose();
            }
            
            LayerHelper.Build3d(Manzanas);
            LayerHelper.GenerateCoords(Manzanas);

            SerializedMesh smManzanas =  new SerializedMesh(new SerializedComponentInfo("Victor", ComponentType.Mesh, "23/05/1983"));

            int VxAmount = 0;

            List<PositionNormalTextureColor> ManzanaVertices = new List<PositionNormalTextureColor>();
            List<int> ManzanaIndices = new List<int>();

            for (int m = 0; m < Manzanas.Count; m++) {
                Manzanas[m].BoundingSphere = BoundingSphere.CreateFromPoints(Manzanas[m].Points);

                Manzanas[m].Name = Manzanas[m].Data[4];
                Manzanas[m].VertexStart = ManzanaVertices.Count;
                Color CurrentColor = new Color(230, 182, 93);
                if (Manzanas[m].Data[1] == "plaza") {
                    CurrentColor = new Color(0, 150, 255);
                }
                else {
                    if (Manzanas[m].Data[1] == "parque" || Manzanas[m].Data[1] == "plaza") {
                        CurrentColor = Color.DarkGreen;
                    }
                }
                CurrentColor = new Color(140, 140, 140);
                for (int x = 0; x < Manzanas[m].Vertices.Length; x++) {
                    PositionNormalTextureColor pc = new PositionNormalTextureColor();
                    pc.Position = Manzanas[m].Vertices[x].Position;
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

            PositionNormalTextureColor[] Manzanillas = ManzanaVertices.ToArray();
            List<PositionNormalTextureColor> ManzanillasSoup = new List<PositionNormalTextureColor>(Manzanillas.Length);

            ManzanaVertices.Clear();

            for (int i = 0; i < Manzanillas.Length; i++) {
                Manzanillas[i].Position = LayerHelper.getFinalVector(Manzanillas[i].Position);
                PositionNormalTextureColor pntc = Manzanillas[i];
                ManzanaVertices.Add(pntc);
            }

            PositionNormalTextureColor[] Vertices = ManzanaVertices.ToArray();

            for (int g = 0; g < Vertices.Length; g++)
                Vertices[g].Normal = new Vector3(0, 0, 0);

            for (int g = 0; g < Vertices.Length / 3; g++) {
                Vector3 firstvec = Vertices[ManzanaIndices[g * 3 + 1]].Position -
                                   Vertices[ManzanaIndices[g * 3]].Position;
                Vector3 secondvec = Vertices[ManzanaIndices[g * 3]].Position -
                                    Vertices[ManzanaIndices[g * 3 + 2]].Position;
                Vector3 normal = Vector3.Cross(firstvec, secondvec);
                normal.Normalize();
                Vertices[ManzanaIndices[g * 3]].Normal += normal;
                Vertices[ManzanaIndices[g * 3 + 1]].Normal += normal;
                Vertices[ManzanaIndices[g * 3 + 2]].Normal += normal;
            }

            for (int g = 0; g < Vertices.Length; g++) {
                Vertices[g].Color = Color.LimeGreen;
            }

            for (int x = 0; x < Vertices.Length - 5; x += 6) {
                Vertices[x].TextureCoordinate = LayerHelper.textureUpperLeft;
                Vertices[x + 1].TextureCoordinate = LayerHelper.textureUpperRight;
                Vertices[x + 2].TextureCoordinate = LayerHelper.textureLowerLeft;
                Vertices[x + 3].TextureCoordinate = LayerHelper.textureLowerLeft;
                Vertices[x + 4].TextureCoordinate = LayerHelper.textureUpperRight;
                Vertices[x + 5].TextureCoordinate = LayerHelper.textureLowerRight;
            }

            smManzanas.Name = barrio;
            CiudadWriter meshWriter = new CiudadWriter();

            /*meshWriter.Serialize(Manzanas, 
                smManzanas,
                                 @"Data\Ciudades\Capital Federal\Veredas\",
                                 ManzanaVertices.ToArray(),
                                 ManzanaIndices.ToArray(),
                                 false,
                                 true
                );*/

            Logger.LogLine(barriobject + ": " + barrio + " written as vereda");

            return null;
        }
        
        private object BuildBarrio(object barriobjekt) {
            int barriobject = (int)barriobjekt;

            string barrio = Constants.Barrios[barriobject];
            List<Feature> Manzanas;
            lock (syncLock) {
                ShapefileLoader Shapefile = new ShapefileLoader(@"..\Work\shp\Mesh\Barrios\Out\Barrio_" + barrio + ".shp");

                Shapefile.Altura = 0;
                Shapefile.HeightField = -1;
                Manzanas = Shapefile.ParsePolygons();

                Shapefile.Dispose();
            }

            LayerHelper.Build2d(Manzanas);

            SerializedMesh smManzanas = new SerializedMesh(new SerializedComponentInfo("Victor", ComponentType.Mesh, "23/05/1983"));

            int VxAmount = 0;

            List<PositionNormalTextureColor> ManzanaVertices = new List<PositionNormalTextureColor>();
            List<int> ManzanaIndices = new List<int>();

            for (int m = 0; m < Manzanas.Count; m++) {
                Manzanas[m].BoundingSphere = BoundingSphere.CreateFromPoints(Manzanas[m].Points);
                Manzanas[m].Name = Manzanas[m].Data[0];
                Manzanas[m].VertexStart = ManzanaVertices.Count;
                Color CurrentColor = Color.Orange;

                for (int x = 0; x < Manzanas[m].Vertices.Length; x++) {
                    PositionNormalTextureColor pc = new PositionNormalTextureColor();
                    pc.Position = Manzanas[m].Vertices[x].Position;
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


            PositionNormalTextureColor[] Manzanillas = ManzanaVertices.ToArray();

            ManzanaVertices.Clear();

            for (int i = 0; i < Manzanillas.Length; i++) {
                Manzanillas[i].Position = LayerHelper.getFinalVector(Manzanillas[i].Position);
                PositionNormalTextureColor pntc = Manzanillas[i];
                ManzanaVertices.Add(pntc);
            }


            PositionNormalTextureColor[] Vertices = ManzanaVertices.ToArray();

            for (int g = 0; g < Vertices.Length; g++)
                Vertices[g].Normal = new Vector3(0, 0, 0);

            for (int g = 0; g < Vertices.Length / 3; g++) {
                Vector3 firstvec = Vertices[ManzanaIndices[g * 3 + 1]].Position -
                                   Vertices[ManzanaIndices[g * 3]].Position;
                Vector3 secondvec = Vertices[ManzanaIndices[g * 3]].Position -
                                    Vertices[ManzanaIndices[g * 3 + 2]].Position;
                Vector3 normal = Vector3.Cross(firstvec, secondvec);
                normal.Normalize();
                Vertices[ManzanaIndices[g * 3]].Normal += normal;
                Vertices[ManzanaIndices[g * 3 + 1]].Normal += normal;
                Vertices[ManzanaIndices[g * 3 + 2]].Normal += normal;
            }


            for (int g = 0; g < Vertices.Length; g++) {
                //Vertices[g].Color = Color.LimeGreen;
            }

            for (int x = 0; x < Vertices.Length - 5; x += 6) {
                Vertices[x].TextureCoordinate = LayerHelper.textureUpperLeft;
                Vertices[x + 1].TextureCoordinate = LayerHelper.textureUpperRight;
                Vertices[x + 2].TextureCoordinate = LayerHelper.textureLowerLeft;
                Vertices[x + 3].TextureCoordinate = LayerHelper.textureLowerLeft;
                Vertices[x + 4].TextureCoordinate = LayerHelper.textureUpperRight;
                Vertices[x + 5].TextureCoordinate = LayerHelper.textureLowerRight;
            }

            smManzanas.Name = barrio;
            CiudadWriter meshWriter = new CiudadWriter();

            meshWriter.SerializeBarrios(Manzanas, smManzanas,
                     @"Data\Ciudades\Capital Federal\Barrios\",
                     ManzanaVertices.ToArray(),
                     ManzanaIndices.ToArray(),
                     false,
                     true
            );

            Logger.LogLine(barriobject + ": " + barrio + " written as barrio");
            return null;
        }

        public float ManzanaHeight = 20f;

        private object BuildManzanas(object barriobjekt) {
            int barriobject = (int)barriobjekt;

            string barrio = Constants.Barrios[barriobject];
            List<Feature> Manzanas;
            lock (syncLock) {
                ShapefileLoader Shapefile = new ShapefileLoader(@"..\Work\shp\Mesh\Manzanas\Out\Manzanas_" + barrio + ".shp");

                Shapefile.Altura = ManzanaHeight;
                Shapefile.HeightField = -1;
                Manzanas = Shapefile.ParsePolygons();

                Shapefile.Dispose();
            }

            LayerHelper.Build3d(Manzanas);
            LayerHelper.GenerateCoords(Manzanas);

            SerializedMesh smManzanas = new SerializedMesh(new SerializedComponentInfo("Victor", ComponentType.Mesh, "23/05/1983"));

            int VxAmount = 0;

            List<PositionNormalTextureColor> ManzanaVertices = new List<PositionNormalTextureColor>();
            List<int> ManzanaIndices = new List<int>();

            for (int m = 0; m < Manzanas.Count; m++) {
                Manzanas[m].Code = Manzanas[m].Data[3];
                Manzanas[m].BoundingSphere = BoundingSphere.CreateFromPoints(Manzanas[m].Points);
                Manzanas[m].Name = Manzanas[m].Data[4];
                Manzanas[m].VertexStart = ManzanaVertices.Count;
                Color CurrentColor = Color.Orange;
                if (Manzanas[m].Data[8] == "plaza") {
                    CurrentColor = new Color(187, 239, 158);
                }
                else {
                    if (Manzanas[m].Data[8] == "parque" || Manzanas[m].Data[8] == "plaza") {
                        CurrentColor = new Color(0, 156, 0);
                    }
                }

                for (int g = 0; g < Manzanas[m].Vertices.Length; g++)
                    Manzanas[m].Vertices[g].Normal = new Vector3(0, 0, 0);

                for (int g = 0; g < Manzanas[m].Vertices.Length / 3; g++) {
                    Vector3 firstvec = Manzanas[m].Vertices[Manzanas[m].Indices[g * 3 + 1]].Position -
                                       Manzanas[m].Vertices[Manzanas[m].Indices[g * 3]].Position;
                    Vector3 secondvec = Manzanas[m].Vertices[Manzanas[m].Indices[g * 3]].Position -
                                        Manzanas[m].Vertices[Manzanas[m].Indices[g * 3 + 2]].Position;
                    Vector3 normal = Vector3.Cross(firstvec, secondvec);
                    normal.Normalize();
                    Manzanas[m].Vertices[Manzanas[m].Indices[g * 3]].Normal += normal;
                    Manzanas[m].Vertices[Manzanas[m].Indices[g * 3 + 1]].Normal += normal;
                    Manzanas[m].Vertices[Manzanas[m].Indices[g * 3 + 2]].Normal += normal;
                }

                for (int x = 0; x < Manzanas[m].Vertices.Length - 5; x += 6) {
                    Manzanas[m].Vertices[x].TextureCoordinate = LayerHelper.textureUpperLeft;
                    Manzanas[m].Vertices[x + 1].TextureCoordinate = LayerHelper.textureUpperRight;
                    Manzanas[m].Vertices[x + 2].TextureCoordinate = LayerHelper.textureLowerLeft;
                    Manzanas[m].Vertices[x + 3].TextureCoordinate = LayerHelper.textureLowerLeft;
                    Manzanas[m].Vertices[x + 4].TextureCoordinate = LayerHelper.textureUpperRight;
                    Manzanas[m].Vertices[x + 5].TextureCoordinate = LayerHelper.textureLowerRight;
                }

                for (int x = 0; x < Manzanas[m].Vertices.Length; x++) {
                    Manzanas[m].Vertices[x].Color = CurrentColor;
                    PositionNormalTextureColor pc = new PositionNormalTextureColor();
                    pc.Position = Manzanas[m].Vertices[x].Position;
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

            PositionNormalTextureColor[] Manzanillas = ManzanaVertices.ToArray();

            ManzanaVertices.Clear();

            for (int i = 0; i < Manzanillas.Length; i++) {
                Manzanillas[i].Position = LayerHelper.getFinalVector(Manzanillas[i].Position);
                PositionNormalTextureColor pntc = Manzanillas[i];
                ManzanaVertices.Add(pntc);
            }

            PositionNormalTextureColor[] Vertices = ManzanaVertices.ToArray();

            for (int g = 0; g < Vertices.Length; g++)
                Vertices[g].Normal = new Vector3(0, 0, 0);

            for (int g = 0; g < Vertices.Length / 3; g++) {
                Vector3 firstvec = Vertices[ManzanaIndices[g * 3 + 1]].Position -
                                   Vertices[ManzanaIndices[g * 3]].Position;
                Vector3 secondvec = Vertices[ManzanaIndices[g * 3]].Position -
                                    Vertices[ManzanaIndices[g * 3 + 2]].Position;
                Vector3 normal = Vector3.Cross(firstvec, secondvec);
                normal.Normalize();
                Vertices[ManzanaIndices[g * 3]].Normal += normal;
                Vertices[ManzanaIndices[g * 3 + 1]].Normal += normal;
                Vertices[ManzanaIndices[g * 3 + 2]].Normal += normal;
            }


            for (int g = 0; g < Vertices.Length; g++) {
                //Vertices[g].Color = Color.LimeGreen;
            }

            for (int x = 0; x < Vertices.Length - 5; x += 6) {
                Vertices[x].TextureCoordinate = LayerHelper.textureUpperLeft;
                Vertices[x + 1].TextureCoordinate = LayerHelper.textureUpperRight;
                Vertices[x + 2].TextureCoordinate = LayerHelper.textureLowerLeft;
                Vertices[x + 3].TextureCoordinate = LayerHelper.textureLowerLeft;
                Vertices[x + 4].TextureCoordinate = LayerHelper.textureUpperRight;
                Vertices[x + 5].TextureCoordinate = LayerHelper.textureLowerRight;
            }

            smManzanas.Name = barrio;
            CiudadWriter meshWriter = new CiudadWriter();

            meshWriter.SerializeManzanas(Manzanas, smManzanas,
                                 @"Data\Ciudades\Capital Federal\Manzanas\",
                                 ManzanaVertices.ToArray(),
                                 ManzanaIndices.ToArray(),
                                 false,
                                 true
                );

            Logger.LogLine(barriobject + ": " + barrio + " written as manzana");
            return null;
        }

        public string[] OutPoints;
        private void BuildPoints(string FilePath) {
            string[] FileName = Directory.GetFiles(FilePath, "*.shp");
            OutPoints = new string[FileName.Length];
            for(int i = 0; i < FileName.Length;i++){
                bool isOSM = false;
                if (FileName[i].Contains("Points_")) {
                    isOSM = true;
                }
                
                string OutName = Path.GetFileNameWithoutExtension(FileName[i]).Replace("Points_", "");
                OutPoints[i] = OutName;
                ShapefileLoader Shapefile = new ShapefileLoader(FileName[i]);
                Shapefile.ManzanaHeight = 11;
                Shapefile.HeightField = -1;

                List<Feature> Points = Shapefile.ParsePoints();

                SerializedPoints pointlayer = new SerializedPoints(new SerializedComponentInfo("Victor", ComponentType.Point, "23/05/1983"));


                //List<string> pts = LayerHelper.getUniqueValue(Points);

                foreach (Feature feature in Points)
                {
                    SerializedPoint point = new SerializedPoint();
                    //point.TextureIndex = pts.IndexOf(feature.Data[2]);
                    point.Position = LayerHelper.getFinalVector(feature.Points[0]);
                    if (isOSM) {
                        point.Name = feature.Data[1];
                    }else{
                        point.Name = feature.Data[0];
                    }
                    pointlayer.Points.Add(point);
                }

                pointlayer.Name = OutName;
                CiudadWriter plw = new CiudadWriter();
                plw.SerializePoints(pointlayer, @"Data\Ciudades\" + Name + @"\Puntos\" + OutName);
                Logger.LogLine(OutName + " written as puntos");
            }
            /*LayerHelper.SerializeIndex(
                Name,
                @"Data\",
                new[] {Name},
                LayerFields.Points
                );

            */
        }
        bool useThreading = true;
        private void BuildExtraMesh(string pt){
            
            ShapefileLoader Shapefile = new ShapefileLoader(pt);

            Shapefile.Altura = 0;
            Shapefile.HeightField = -1;
            List<Feature> Extra = Shapefile.ParsePolygons();

            Shapefile.Dispose();

            LayerHelper.Build2d(Extra);

            SerializedMesh smManzanas = new SerializedMesh(new SerializedComponentInfo("Victor", ComponentType.Mesh, "23/05/1983"));

            int VxAmount = 0;

            List<PositionNormalTextureColor> ManzanaVertices = new List<PositionNormalTextureColor>();
            List<int> ManzanaIndices = new List<int>();

            for (int m = 0; m < Extra.Count; m++) {
                Extra[m].BoundingSphere = BoundingSphere.CreateFromPoints(Extra[m].Points);
                Extra[m].Name = Extra[m].Data[0];
                Extra[m].VertexStart = ManzanaVertices.Count;
                Color CurrentColor = Color.Orange;

                for (int x = 0; x < Extra[m].Vertices.Length; x++) {
                    PositionNormalTextureColor pc = new PositionNormalTextureColor();
                    pc.Position = Extra[m].Vertices[x].Position;
                    pc.Color = CurrentColor;
                    pc.TextureCoordinate = Extra[m].Vertices[x].TextureCoordinate;
                    pc.Normal = Extra[m].Vertices[x].Normal;
                    ManzanaVertices.Add(pc);
                }
                for (int xx = 0; xx < Extra[m].Indices.Length; xx++) {
                    ManzanaIndices.Add(VxAmount + Extra[m].Indices[xx]);
                }
                VxAmount += Extra[m].Vertices.Length;
                Extra[m].VertexEnd = ManzanaVertices.Count;
            }


            PositionNormalTextureColor[] Manzanillas = ManzanaVertices.ToArray();

            ManzanaVertices.Clear();

            for (int i = 0; i < Manzanillas.Length; i++) {
                Manzanillas[i].Position = LayerHelper.getFinalVector(Manzanillas[i].Position);
                PositionNormalTextureColor pntc = Manzanillas[i];
                ManzanaVertices.Add(pntc);
            }


            PositionNormalTextureColor[] Vertices = ManzanaVertices.ToArray();

            for (int g = 0; g < Vertices.Length; g++)
                Vertices[g].Normal = new Vector3(0, 0, 0);

            for (int g = 0; g < Vertices.Length / 3; g++) {
                Vector3 firstvec = Vertices[ManzanaIndices[g * 3 + 1]].Position -
                                   Vertices[ManzanaIndices[g * 3]].Position;
                Vector3 secondvec = Vertices[ManzanaIndices[g * 3]].Position -
                                    Vertices[ManzanaIndices[g * 3 + 2]].Position;
                Vector3 normal = Vector3.Cross(firstvec, secondvec);
                normal.Normalize();
                Vertices[ManzanaIndices[g * 3]].Normal += normal;
                Vertices[ManzanaIndices[g * 3 + 1]].Normal += normal;
                Vertices[ManzanaIndices[g * 3 + 2]].Normal += normal;
            }


            for (int g = 0; g < Vertices.Length; g++) {
                //Vertices[g].Color = Color.LimeGreen;
            }

            for (int x = 0; x < Vertices.Length - 5; x += 6) {
                Vertices[x].TextureCoordinate = LayerHelper.textureUpperLeft;
                Vertices[x + 1].TextureCoordinate = LayerHelper.textureUpperRight;
                Vertices[x + 2].TextureCoordinate = LayerHelper.textureLowerLeft;
                Vertices[x + 3].TextureCoordinate = LayerHelper.textureLowerLeft;
                Vertices[x + 4].TextureCoordinate = LayerHelper.textureUpperRight;
                Vertices[x + 5].TextureCoordinate = LayerHelper.textureLowerRight;
            }

            smManzanas.Name = Path.GetFileNameWithoutExtension(pt);
            CiudadWriter meshWriter = new CiudadWriter();

            meshWriter.SerializeExtraMesh(Extra, smManzanas,
                     @"Data\Ciudades\Capital Federal\Extra\",
                     ManzanaVertices.ToArray(),
                     ManzanaIndices.ToArray(),
                     false,
                     true
    );

            Logger.LogLine(smManzanas.Name + " written as extramesh");

            //Console.WriteLine("Serialized " + smManzanas.Name);

        }
        public void BuildExtra(string path){
            CiudadWriter ciudadWriter = new CiudadWriter();

            string[] extra = Directory.GetFiles(path, "*.shp");
            List<string> final_list = new List<string>();
            for (int e = 0; e < extra.Length; e++) {
                if (extra[e].Contains("Calles") || extra[e].Contains("Provinci") || extra[e].Contains("ttt")) {
                    continue;
                }

                BuildExtraMesh(extra[e]);
                final_list.Add(extra[e]);
            }
            ciudadWriter.SerializeExtra(@"Data\Ciudades\Capital Federal\", final_list.ToArray());
        }
        public override void Process()
        {
            return;
            //BuildExtra(@"..\Work\shp\Mesh\Extra\");
            //return;
            Logger.LogLine("CiudadBuilder::Process()");
            
            STPStartInfo stpStartInfo = new STPStartInfo();
            stpStartInfo.StartSuspended = true;
            stpStartInfo.MaxWorkerThreads = 6;
            
            SmartThreadPool smartThreadPool = new SmartThreadPool(stpStartInfo);

            BuildPoints(@"..\Work\shp\Mesh\Point\");
            useThreading = false;
            for (int b = 0; b < Constants.Barrios.Length; b++)
            {
                if(!Constants.Barrios[b].Contains("Almagro")){
                   // continue;
                }
                if (!useThreading){
                    BuildLotes(b);
                    BuildBarrio(b);
                    BuildManzanas(b);
                    
                    //BuildVeredas(b);
                }else{
                    smartThreadPool.QueueWorkItem(new WorkItemCallback(this.BuildBarrio),b,WorkItemPriority.Highest);
                    //smartThreadPool.QueueWorkItem(new WorkItemCallback(this.BuildVeredas), b, WorkItemPriority.Highest);
                    smartThreadPool.QueueWorkItem(new WorkItemCallback(this.BuildManzanas), b, WorkItemPriority.Highest);
                    smartThreadPool.QueueWorkItem(new WorkItemCallback(this.BuildLotes), b, WorkItemPriority.Highest);
                }
            }

            if (useThreading){
                smartThreadPool.Start();
                smartThreadPool.WaitForIdle();
                smartThreadPool.Shutdown();
            }

            BuildCalles(false);
            CiudadWriter ciudadWriter = new CiudadWriter();
            string[] barrios = Constants.Barrios;
            string[] manzanas = Constants.Barrios;
            string[] lotes = Constants.Barrios;
            string[] call = new []{"Calles"};

            ciudadWriter.SerializeCiudad("Capital Federal", @"Data\Ciudades\", barrios, manzanas, lotes, call, OutPoints);
        }


        private void BuildCalles(bool buildIndersections){

            Logger.LogLine("BuildCalles()");
            StreetLayerWriter StreetWriter = new StreetLayerWriter();

            ShapefileLoader Shapefile = new ShapefileLoader(@"..\Work\shp\Mesh\Calles\CallesGOOD.shp");

            Shapefile.HeightField = -1;
            Shapefile.Altura = 1;
            Shapefile.AnchoCamino = "0.1";
            Shapefile.AnchoAvenida = "0.12";
            List<Feature> Streets = Shapefile.ParsePolyLine();


            for (int f = 0; f < Streets.Count; f++){
                Streets[f].ID = f;
                for (int t = 0; t < Streets[f].Tramos.Count; t++){
                    for (int xx = 0; xx < Streets[f].Tramos[t].OrigLine.Count; xx++){
                        Streets[f].Tramos[t].OrigLine[xx] = LayerHelper.getFinalVector(Streets[f].Tramos[t].OrigLine[xx]);
                    }
                }
            }

            this.calles = Streets;
            if (buildIndersections){
                Logger.LogLine("BuildCalles():ProcessIntersections()");
                ProcessIntersections();
            }

            CiudadWriter ciudadWriter = new CiudadWriter();
            Logger.LogLine("writing Calles.bin");
            ciudadWriter.SerializeCalles("Calles", @"Data\Ciudades\Capital Federal\Calles\", Streets);
            Logger.LogLine("wrote Calles.bin");


        }
        private List<Feature> calles = new List<Feature>();
        public void ProcessIntersections() {
            StreetLayerWriter StreetWriter = new StreetLayerWriter();
            List<string> uniqueStrings = new List<string>();
            for (int i = 0; i < calles.Count; i++) {
                if (!uniqueStrings.Contains(calles[i].Data[0])) {
                    uniqueStrings.Add(calles[i].Data[0]);
                }
            }

            List<Vector3> coords = new List<Vector3>();

            List<StreetCoord> scs = new List<StreetCoord>();


            for (int i = 0; i < calles.Count; i++) {
                for (int t = 0; t < calles[i].Tramos.Count; t++) {
                    for (int p = 0; p < calles[i].Tramos[t].OrigLine.Count; p++) {
                        StreetCoord sc = new StreetCoord(calles[i].Tramos[t].OrigLine[p]);
                        if (!scs.Contains(sc)) {
                            scs.Add(sc);
                        }
                    }
                }
            }


            for (int co = 0; co < scs.Count; co++) {
                for (int i = 0; i < calles.Count; i++) {
                    for (int t = 0; t < calles[i].Tramos.Count; t++) {
                        for (int p = 0; p < calles[i].Tramos[t].OrigLine.Count; p++) {
                            if (scs[co].Coord == calles[i].Tramos[t].OrigLine[p]) {
                                scs[co].addID(calles[i].ID);
                                break;
                            }
                        }
                    }
                }
            }

            for (int co = 0; co < scs.Count; co++) {
                scs[co].Streets = scs[co].Streets.Distinct().ToList();
            }


            for (int co = 0; co < scs.Count; co++) {
                for (int i = 0; i < calles.Count; i++) {
                    if (scs[co].Streets.Contains(calles[i].ID)) {
                        calles[i].AgregarInterseccion(scs[co].Streets);
                    }
                }
            }

            CiudadWriter ciudadWriter = new CiudadWriter();
            ciudadWriter.SerializeIntersections("Intersecciones", @"Data\Ciudades\Capital Federal\Calles\", calles, uniqueStrings);


        }

        private object BuildLotes(object barriobjekt) {
            int barriobject = (int)barriobjekt;
            string barrio = Constants.Barrios[barriobject];

            List<Feature> Lotes;
            lock (syncLock) {
                ShapefileLoader shapefileLoader = new ShapefileLoader(@"..\Work\shp\Mesh\Lotes\lotes_" + barrio + ".shp");
                shapefileLoader.ManzanaHeight = 9;
                shapefileLoader.HeightField = 11;
                Lotes = shapefileLoader.ParsePolygons();
                shapefileLoader.Dispose();
            }
            LayerHelper.Build3d(Lotes);


            LayerHelper.GenerateCoords(Lotes);


            SerializedMesh smManzanas =
                new SerializedMesh(new SerializedComponentInfo("Victor", ComponentType.Mesh, "23/05/1983"));
            Stopwatch sWatch = new Stopwatch();

            int VxAmount = 0;

            List<PositionNormalTextureColor> ManzanaVertices = new List<PositionNormalTextureColor>();
            List<int> ManzanaIndices = new List<int>();

            for (int m = 0; m < Lotes.Count; m++) {
                Lotes[m].Code = Lotes[m].Data[4] + "-" + Lotes[m].Data[5];
                //if (Lotes[m].Data[8] == barrio) {
                Lotes[m].BoundingSphere = BoundingSphere.CreateFromPoints(Lotes[m].Points);

                Lotes[m].Name = Lotes[m].Data[4];
                Lotes[m].VertexStart = ManzanaVertices.Count;

                Color CurrentColor = LayerHelper.GetRandomColor();
                for (int x = 0; x < Lotes[m].Vertices.Length; x++) {
                    PositionNormalTextureColor pc = new PositionNormalTextureColor();
                    pc.Position = Lotes[m].Vertices[x].Position;
                    pc.Color = CurrentColor;
                    pc.TextureCoordinate = Lotes[m].Vertices[x].TextureCoordinate;
                    pc.Normal = Lotes[m].Vertices[x].Normal;
                    ManzanaVertices.Add(pc);
                }
                for (int xx = 0; xx < Lotes[m].Indices.Length; xx++) {
                    ManzanaIndices.Add(VxAmount + Lotes[m].Indices[xx]);
                }
                VxAmount += Lotes[m].Vertices.Length;
                Lotes[m].VertexEnd = ManzanaVertices.Count;
                //}
            }


            PositionNormalTextureColor[] Lotitos = ManzanaVertices.ToArray();


            Vector3 vec = Vector3.Zero;

            int ii = 0;

            ManzanaVertices.Clear();

            for (int i = 0; i < Lotitos.Length; i++) {
                Lotitos[i].Position = LayerHelper.getFinalVector(Lotitos[i].Position);
                PositionNormalTextureColor pntc = Lotitos[i];
                ManzanaVertices.Add(pntc);
            }


            smManzanas.Name = barrio;
            CiudadWriter meshWriter = new CiudadWriter();
            lock (syncLock) {
                meshWriter.SerializeLotes(Lotes
                ,
                smManzanas,
                                     @"Data\Ciudades\Capital Federal\Lotes\",
                                     ManzanaVertices.ToArray(),
                                     ManzanaIndices.ToArray(),
                                     false,
                                     true
                    );
            }
            Logger.LogLine(barriobject + ": " + barrio + " written as lote");

            //Console.WriteLine("Serialized " + smManzanas.Name);
            return null;
        }

        public override void Dispose()
        {
            //Shapefile.Dispose();
        }
    }
}
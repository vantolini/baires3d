using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace Builder
{

    public static class CityFields {
        public const byte Name = 1;
        public const byte Version = 2;
        public const byte Date = 3;


        public const byte Lotes = 6;
        public const byte Manzanas = 7;
        public const byte Barrios = 8;
        public const byte Puntos = 9;
        public const byte Calles = 10;
        public const byte Veredas = 11;
        

    }

    public class CiudadWriter {
        private MemoryStream memoryStream;
        private BinaryWriter BinaryWriter;

        public void SerializeCalles(string name, string path, List<Calle> Features) {
            memoryStream = new MemoryStream();
            BinaryWriter = new BinaryWriter(memoryStream);
            //WriteHeader(component);


            Color CurrentColor = new Color(255, 128, 0);

            //BinaryWriter.Write(LayerFields.Streets);
            BinaryWriter.Write(name);
            BinaryWriter.Write((int)LayerTypes.Calles);

            BinaryWriter.Write(Features.Count);
            for (int f = 0; f < Features.Count; f++) {
                BinaryWriter.Write(f);
                BinaryWriter.Write(Features[f].Nombre);
                BinaryWriter.Write(Features[f].Segmentos.Count);
                for (int t = 0; t < Features[f].Segmentos.Count; t++) {
                    // tipo, sentido, altiniimpar, altfiniimpar, altinipar, altfinipar

                    /*
                    BinaryWriter.Write(Features[f].Tramos[t].Data[2]);
                    BinaryWriter.Write(Features[f].Tramos[t].Data[3]);
                    BinaryWriter.Write(Features[f].Tramos[t].Data[4]);
                    BinaryWriter.Write(Features[f].Tramos[t].Data[5]);
                */

                    BinaryWriter.Write(Features[f].Segmentos[t].Tramos.Count);

                    for (int xx = 0; xx < Features[f].Segmentos[t].Tramos.Count; xx++) {

                        BinaryWriter.Write(Features[f].Segmentos[t].Tramos[xx].AlturaInicialPar);
                        BinaryWriter.Write(Features[f].Segmentos[t].Tramos[xx].AlturaFinalPar);
                        BinaryWriter.Write(Features[f].Segmentos[t].Tramos[xx].AlturaInicialImpar);
                        BinaryWriter.Write(Features[f].Segmentos[t].Tramos[xx].AlturaFinalImpar);
                        BinaryWriter.Write(Features[f].Segmentos[t].Tramos[xx].Sentido);
                        BinaryWriter.Write(Features[f].Segmentos[t].Tramos[xx].Tipo);

                        BinaryWriter.Write(Features[f].Segmentos[t].Tramos[xx].Puntos.Count);

                        for (int pt = 0; pt< Features[f].Segmentos[t].Tramos[xx].Puntos.Count; pt++){
                            BinaryWriter.Write(Features[f].Segmentos[t].Tramos[xx].Puntos[pt].X);
                            BinaryWriter.Write(Features[f].Segmentos[t].Tramos[xx].Puntos[pt].Z);
                        }
                    }
                }
            }

            BinaryWriter.Flush();

            Directory.CreateDirectory(Path.GetDirectoryName(path));
            FileStream fs = new FileStream(path + name + ".bin", FileMode.Create);
            fs.Write(memoryStream.GetBuffer(), 0, Convert.ToInt32(memoryStream.Position));
            fs.Flush();
            fs.Close();
            memoryStream.Close();
            BinaryWriter.Close();
        }



        public void SerializeCallesOLD(string name, string path, List<Feature> Features) {
            memoryStream = new MemoryStream();
            BinaryWriter = new BinaryWriter(memoryStream);
            //WriteHeader(component);


            Color CurrentColor = new Color(255, 128, 0);

            //BinaryWriter.Write(LayerFields.Streets);
            BinaryWriter.Write(name);
            BinaryWriter.Write((int)LayerTypes.Calles);





            BinaryWriter.Write(Features.Count);
            for (int f = 0; f < Features.Count; f++) {
                BinaryWriter.Write(Features[f].ID);
                BinaryWriter.Write(Features[f].Name);
                BinaryWriter.Write(Features[f].Tramos.Count);
                for (int t = 0; t < Features[f].Tramos.Count; t++) {
                    // tipo, sentido, altiniimpar, altfiniimpar, altinipar, altfinipar
                    BinaryWriter.Write(Features[f].Tramos[t].Data[8]);
                    BinaryWriter.Write(Features[f].Tramos[t].Data[3]);
                    BinaryWriter.Write(Features[f].Tramos[t].Data[4]);
                    BinaryWriter.Write(Features[f].Tramos[t].Data[5]);
                    BinaryWriter.Write(Features[f].Tramos[t].Data[6]);
                    BinaryWriter.Write(Features[f].Tramos[t].Data[7]);
                    /*
                    BinaryWriter.Write(Features[f].Tramos[t].Data[2]);
                    BinaryWriter.Write(Features[f].Tramos[t].Data[3]);
                    BinaryWriter.Write(Features[f].Tramos[t].Data[4]);
                    BinaryWriter.Write(Features[f].Tramos[t].Data[5]);
                */

                    BinaryWriter.Write(Features[f].Tramos[t].OrigLine.Count);
                    for (int xx = 0; xx < Features[f].Tramos[t].OrigLine.Count; xx++) {
                        BinaryWriter.Write(Features[f].Tramos[t].OrigLine[xx].X);
                        BinaryWriter.Write(Features[f].Tramos[t].OrigLine[xx].Y);
                        BinaryWriter.Write(Features[f].Tramos[t].OrigLine[xx].Z);
                    }
                }
            }

            BinaryWriter.Flush();

            Directory.CreateDirectory(Path.GetDirectoryName(path));
            FileStream fs = new FileStream(path + name + ".bin", FileMode.Create);
            fs.Write(memoryStream.GetBuffer(), 0, Convert.ToInt32(memoryStream.Position));
            fs.Flush();
            fs.Close();
            memoryStream.Close();
            BinaryWriter.Close();

            //CompressionHelper.CompressFile(path + name + ".bin");
        }




        public void SerializePoints(object component, string path) 
        {
            memoryStream = new MemoryStream();
            BinaryWriter = new BinaryWriter(memoryStream);
            //WriteHeader(component);

            SerializedPoints sm = (SerializedPoints)component;

            //BinaryWriter.Write(LayerFields.Points);
            BinaryWriter.Write(sm.Name);

            BinaryWriter.Write((int)LayerTypes.Puntos);

           // BinaryWriter.Write(sm.Info.Author);
           // BinaryWriter.Write(sm.Info.DateCreation);


            BinaryWriter.Write(sm.Points.Count);
            for (int xx = 0; xx < sm.Points.Count; xx++) {
                BinaryWriter.Write((float)sm.Points[xx].Position.X);
                BinaryWriter.Write((float)sm.Points[xx].Position.Y);
                BinaryWriter.Write((float)sm.Points[xx].Position.Z);
                BinaryWriter.Write(sm.Points[xx].TextureIndex);
                BinaryWriter.Write(sm.Points[xx].Name);
            }
            BinaryWriter.Flush();


            Directory.CreateDirectory(Path.GetDirectoryName(path));
            FileStream fs = new FileStream(path + ".bin", FileMode.Create);
            fs.Write(memoryStream.GetBuffer(), 0, Convert.ToInt32(memoryStream.Position));
            fs.Flush();
            fs.Close();
            memoryStream.Close();
            BinaryWriter.Close();

            //CompressionHelper.CompressFile(path + ".bin");



        }
        
        public void BuildVertices(List<Feature> features, out PositionNormalTextureColor[] vertices, out int[] indices){

            int VxAmount = 0;

            List<PositionNormalTextureColor> ManzanaVertices = new List<PositionNormalTextureColor>();
            List<int> ManzanaIndices = new List<int>();

            for (int m = 0; m < features.Count; m++) {
                features[m].Code = m.ToString();
                features[m].Name = m.ToString();
                features[m].VertexStart = ManzanaVertices.Count;
                /*
                Color CurrentColor = new Color(255, 174, 0);// Color.Orange;

                for (int g = 0; g < features[m].Vertices.Length; g++)
                    features[m].Vertices[g].Normal = new Vector3(0, 0, 0);

                for (int g = 0; g < features[m].Vertices.Length / 3; g++) {
                    Vector3 firstvec =
                        features[m].Vertices[features[m].Indices[g * 3 + 1]].Position - features[m].Vertices[features[m].Indices[g * 3]].Position;
                    Vector3 secondvec =
                        features[m].Vertices[features[m].Indices[g * 3]].Position - features[m].Vertices[features[m].Indices[g * 3 + 2]].Position;

                    Vector3 normal = Vector3.Cross(firstvec, secondvec);
                    normal.Normalize();

                    features[m].Vertices[features[m].Indices[g * 3]].Normal += normal;
                    features[m].Vertices[features[m].Indices[g * 3 + 1]].Normal += normal;
                    features[m].Vertices[features[m].Indices[g * 3 + 2]].Normal += normal;
                }


                for (int g = 0; g < features[m].Vertices.Length; g++) {
                    //features[m].Vertices[g].Normal.Normalize();
                }
                for (int x = 0; x < features[m].Vertices.Length - 5; x += 6) {
                    features[m].Vertices[x].TextureCoordinate = LayerHelper.textureUpperLeft;
                    features[m].Vertices[x + 1].TextureCoordinate = LayerHelper.textureUpperRight;
                    features[m].Vertices[x + 2].TextureCoordinate = LayerHelper.textureLowerLeft;
                    features[m].Vertices[x + 3].TextureCoordinate = LayerHelper.textureLowerLeft;
                    features[m].Vertices[x + 4].TextureCoordinate = LayerHelper.textureUpperRight;
                    features[m].Vertices[x + 5].TextureCoordinate = LayerHelper.textureLowerRight;
                }

                for (int x = 0; x < features[m].Vertices.Length; x++) {
                    features[m].Vertices[x].Color = CurrentColor;
                    PositionNormalTextureColor pc = new PositionNormalTextureColor();
                    pc.Position = features[m].Vertices[x].Position;
                    pc.Color = CurrentColor;
                    pc.TextureCoordinate = features[m].Vertices[x].TextureCoordinate;
                    pc.Normal = features[m].Vertices[x].Normal;
                    ManzanaVertices.Add(pc);
                }
                for (int xx = 0; xx < features[m].Indices.Length; xx++) {
                    ManzanaIndices.Add(VxAmount + features[m].Indices[xx]);
                }*/
                //VxAmount += features[m].Vertices.Length;
                features[m].VertexEnd = ManzanaVertices.Count;
            }



            PositionNormalTextureColor[] Vertices = ManzanaVertices.ToArray();
            int[] Indices = new int[Vertices.Length];
            for(int x = 0; x < Indices.Length;x++){
                Indices[x] = x;
            }

            CalculateNormals(Vertices, Indices);

            LayerHelper.OptimizeTriangles(Vertices, Indices, out vertices, out indices);
            GC.Collect();

        }


        private void WriteInts(BinaryWriter BinaryWriter, int[] array){
            BinaryWriter.Write(array.Length);
            for (int xx = 0; xx < array.Length; xx++) {
                BinaryWriter.Write(array[xx]);
            }
        }
        private void WriteVertices(BinaryWriter BinaryWriter, PositionNormalTextureColor[] array) {
            BinaryWriter.Write(array.Length);
            for (int xx = 0; xx < array.Length; xx++) {
                BinaryWriter.Write(array[xx].Position.X);
                BinaryWriter.Write(array[xx].Position.Y);
                BinaryWriter.Write(array[xx].Position.Z);
            }
        }
        private void WriteNormals(BinaryWriter BinaryWriter, PositionNormalTextureColor[] array) {
            BinaryWriter.Write(array.Length);
            for (int xx = 0; xx < array.Length; xx++) {
                BinaryWriter.Write(array[xx].Normal.X);
                BinaryWriter.Write(array[xx].Normal.Y);
                BinaryWriter.Write(array[xx].Normal.Z);
            }
        }
        private void WriteTextureCoords(BinaryWriter BinaryWriter, PositionNormalTextureColor[] array) {
            BinaryWriter.Write(array.Length);
            for (int xx = 0; xx < array.Length; xx++) {
                BinaryWriter.Write(array[xx].TextureCoordinate.X);
                BinaryWriter.Write(array[xx].TextureCoordinate.Y);
            }
        }
        private void WriteColors(BinaryWriter BinaryWriter, PositionNormalTextureColor[] array) {
            Color clr = array[0].Color;
            bool hasSingleColor = true;
            for (int xx = 1; xx < array.Length; xx++) {
                if (clr != array[xx].Color) {
                    hasSingleColor = false;
                    break;
                }
            }

            if (hasSingleColor) {
                BinaryWriter.Write(0);
                BinaryWriter.Write(array[0].Color.R);
                BinaryWriter.Write(array[0].Color.G);
                BinaryWriter.Write(array[0].Color.B);
            } else {
                BinaryWriter.Write(array.Length);
                for (int xx = 0; xx < array.Length; xx++) {
                    BinaryWriter.Write(array[xx].Color.R);
                    BinaryWriter.Write(array[xx].Color.G);
                    BinaryWriter.Write(array[xx].Color.B);
                }
            }
        }
        private void WriteVertexPositions(BinaryWriter BinaryWriter, List<Feature> array) {
            BinaryWriter.Write(array.Count);
            for (int xx = 0; xx < array.Count; xx++) {
                BinaryWriter.Write(array[xx].VertexStart);
                BinaryWriter.Write(array[xx].VertexEnd);
            }
        }
        private void WriteVertices(BinaryWriter BinaryWriter, List<Feature> features) {
            int paredesCount = 0;
            int techosCount = 0;
            int vertexCount = 0;

            for (int xx = 0; xx < features.Count; xx++) {
                /*if (features[xx].Paredes != null) {
                    paredesCount += features[xx].Paredes.Length;
                    vertexCount += features[xx].Paredes.Length;
                }
                techosCount += features[xx].Techo.Count;*/
                vertexCount += features[xx].Vertices.Length;
            }

            BinaryWriter.Write(features.Count);
            for (int xx = 0; xx < features.Count; xx++) {
                BinaryWriter.Write(features[xx].Vertices.Length);
            }

            BinaryWriter.Write(vertexCount);
            for (int xx = 0; xx < features.Count; xx++) {
                CalculateNormals(features[xx].Vertices, features[xx].Indices);

                for (int xp = 0; xp < features[xx].Vertices.Length; xp++) {
                    BinaryWriter.Write((float)features[xx].Vertices[xp].Position.X);
                    BinaryWriter.Write((float)features[xx].Vertices[xp].Position.Y);
                    BinaryWriter.Write((float)features[xx].Vertices[xp].Position.Z);
                }
            }
        }

        private void WriteVertices2(BinaryWriter BinaryWriter, List<Feature> features) {
            int paredesCount = 0;
            int techosCount = 0;
            int vertexCount = 0;

            for (int xx = 0; xx < features.Count; xx++) {
                /*if (features[xx].Paredes != null) {
                    paredesCount += features[xx].Paredes.Length;
                    vertexCount += features[xx].Paredes.Length;
                }
                techosCount += features[xx].Techo.Count;*/
                vertexCount += features[xx].Vertices.Length;
            }

            BinaryWriter.Write(features.Count);
            for (int xx = 0; xx < features.Count; xx++) {
                BinaryWriter.Write(features[xx].Vertices.Length);
            }

            BinaryWriter.Write(vertexCount);
            for (int xx = 0; xx < features.Count; xx++) {
                CalculateNormals(features[xx].Vertices, features[xx].Indices);

                for (int xp = 0; xp < features[xx].Vertices.Length; xp++) {
                    BinaryWriter.Write((float)features[xx].Vertices[xp].Position.X);
                    BinaryWriter.Write((float)features[xx].Vertices[xp].Position.Y);
                    BinaryWriter.Write((float)features[xx].Vertices[xp].Position.Z);
                    /*
                    BinaryWriter.Write((float)features[xx].Vertices[xp].Normal.X);
                    BinaryWriter.Write((float)features[xx].Vertices[xp].Normal.Y);
                    BinaryWriter.Write((float)features[xx].Vertices[xp].Normal.Z);*/
                }



                /*if (features[xx].Paredes == null) {
                    continue;
                }
                for (int xp = 0; xp < features[xx].Paredes.Length; xp++){
                    BinaryWriter.Write(features[xx].Paredes[xp].X);
                    BinaryWriter.Write(features[xx].Paredes[xp].Y);
                    BinaryWriter.Write(features[xx].Paredes[xp].Z);
                }*/
            }
            //BinaryWriter.Write(techosCount);
            //for (int xx = 0; xx < features.Count; xx++) {
               /* for (int xp = 0; xp < features[xx].Techo.Count; xp++) {
                    BinaryWriter.Write(features[xx].Techo[xp].X);
                    BinaryWriter.Write(features[xx].Techo[xp].Y);
                    BinaryWriter.Write(features[xx].Techo[xp].Z);
                }*/
            //}
        }
        public enum LayerTypes{
            Manzanas,
            Veredas,
            Cordones,
            Silueta,
            Lotes,
            Calles,
            Subte,
            Puntos
        }
        private void WriteMeshHeader(BinaryWriter writer, SerializedComponentInfo Info) {
            //writer.Write((int)Info.DistanceMinimum);
            //writer.Write((int)Info.DistanceMaximum);
        }

        public void SerializeBarrio(
                string name, 
                string path, 
                List<Feature> Manzanas,
                List<Feature> Veredas,
                List<Feature> Cordones,
                List<Feature> Silueta,
                List<Feature> Lotes
            ) {

            SerializedComponentInfo nfo = new SerializedComponentInfo();
            //nfo.LOD = LOD.Far

            nfo.DistanceMinimum = 0.1f;
            nfo.DistanceMaximum = 1f;

            if(name == "Nu�ez"){
                name = "Nunez";
            }

            if(Lotes != null){

                SerializeBarrioMesh(name + "_lotes", path, LayerTypes.Lotes, Lotes, nfo);

            }

            SerializeBarrioMesh(name + "_manzanas", path, LayerTypes.Manzanas, Manzanas, nfo);

            nfo.DistanceMinimum = 0.1f;
            nfo.DistanceMaximum = 0.4f;
            SerializeBarrioMesh(name + "_veredas", path, LayerTypes.Veredas, Veredas, nfo);


            /*
            nfo.DistanceMinimum = 0.1f;
            nfo.DistanceMaximum = 0.4f;
            SerializeBarrioMesh(name + "_veredas", path, LayerTypes.Veredas, Veredas, nfo);

            nfo.DistanceMinimum = 0.01f;
            nfo.DistanceMaximum = 0.1f;
            SerializeBarrioMesh(name + "_cordones", path, LayerTypes.Cordones, Cordones, nfo);
            */
            nfo.DistanceMinimum = 0.4f;
            nfo.DistanceMaximum = 1.0f;
            SerializeBarrioMesh(name + "_silueta", path, LayerTypes.Silueta, Silueta, nfo);

        }

        public void SerializeSubte(
                string name,
                string path,
                List<Feature> Features
            ) {

            SerializedComponentInfo nfo = new SerializedComponentInfo();


            SerializeSubteMesh(name, path, LayerTypes.Subte, Features, nfo);

        }



        public void SerializeSubteMesh(
                string name,
                string path,
                LayerTypes lt,
                List<Feature> Items,
            SerializedComponentInfo nfo) {


            memoryStream = new MemoryStream();
            BinaryWriter = new BinaryWriter(memoryStream);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            FileStream fs = new FileStream(path + name + ".bin", FileMode.Create);

            BinaryWriter.Write(name);
            BinaryWriter.Write((int)lt);


            WriteMeshHeader(BinaryWriter, nfo);
            WriteVertices(BinaryWriter, Items);

            BinaryWriter.Flush();
            fs.Write(memoryStream.GetBuffer(), 0, Convert.ToInt32(memoryStream.Position));
            BinaryWriter.BaseStream.Close();
            fs.Flush();
            fs.Close();
            memoryStream.Close();
            BinaryWriter.Close();
            //CompressionHelper.CompressFile(path + name + ".bin");
        }




        public void SerializeBarrioMesh(
                string name,
                string path,
                LayerTypes lt,
                List<Feature> Items,
            SerializedComponentInfo nfo){


            memoryStream = new MemoryStream();
            BinaryWriter = new BinaryWriter(memoryStream);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            FileStream fs = new FileStream(path + name + ".bin", FileMode.Create);

            BinaryWriter.Write(name);
            BinaryWriter.Write((int)lt);


            WriteMeshHeader(BinaryWriter, nfo);
            WriteVertices(BinaryWriter, Items);

            BinaryWriter.Flush();
            fs.Write(memoryStream.GetBuffer(), 0, Convert.ToInt32(memoryStream.Position));
            BinaryWriter.BaseStream.Close();
            fs.Flush();
            fs.Close();
            memoryStream.Close();
            BinaryWriter.Close();
            //CompressionHelper.CompressFile(path + name + ".bin");
        }

        public void SerializeCiudad2222(string nombre, string path, string[] Barrios, string[] Manzanas, string[] Lotes, string[] Calles, string[] Puntos, string[] Veredas) {

            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter BinaryWriter = new BinaryWriter(memoryStream);

            Directory.CreateDirectory(Path.GetDirectoryName(path));
            FileStream fs = new FileStream(path + nombre + ".bin", FileMode.Create);


            BinaryWriter.Write(CityFields.Name);
            BinaryWriter.Write(nombre);
            BinaryWriter.Write("PITOR");
            BinaryWriter.Write(DateTime.Now.ToUniversalTime().ToString());

            int packAmount = 0;

            if (Barrios != null){
                packAmount++;
            }

            if (Manzanas != null) {
                packAmount++;
            }
            if (Lotes != null) {
                packAmount++;
            }
            if (Calles != null) {
                packAmount++;
            }
            if (Puntos != null) {
                packAmount++;
            }
            if (Veredas != null) {
                packAmount++;
            }
            if(packAmount == 0){
                throw new Exception("WTF???");
            }

            packAmount = 6;
            BinaryWriter.Write(packAmount);
            
            BinaryWriter.Write(CityFields.Barrios);
            if (Barrios == null){
                BinaryWriter.Write(0);
            }else{
                BinaryWriter.Write(Barrios.Length);
                for (int xx = 0; xx < Barrios.Length; xx++) {
                    BinaryWriter.Write(Barrios[xx]);
                }
            }

            BinaryWriter.Write(CityFields.Manzanas);
            if (Manzanas == null) {
                BinaryWriter.Write(0);
            }else{
                BinaryWriter.Write(Manzanas.Length);
                for (int xx = 0; xx < Manzanas.Length; xx++){
                    BinaryWriter.Write(Manzanas[xx]);
                }
            }

            BinaryWriter.Write(CityFields.Lotes);
            if (Lotes == null){
                BinaryWriter.Write(0);
            }else{
                BinaryWriter.Write(Lotes.Length);
                for (int xx = 0; xx < Lotes.Length; xx++){
                    BinaryWriter.Write(Lotes[xx]);
                }
            }

            BinaryWriter.Write(CityFields.Puntos);
            if (Puntos == null){
                BinaryWriter.Write(0);
            }else{
                BinaryWriter.Write(Puntos.Length);
                for (int xx = 0; xx < Puntos.Length; xx++){
                    BinaryWriter.Write(Puntos[xx]);
                }
            }

            BinaryWriter.Write(CityFields.Calles);
            if (Calles == null) {
                BinaryWriter.Write(0);
            }
            else {
                BinaryWriter.Write(Calles.Length);
                for (int xx = 0; xx < Calles.Length; xx++) {
                    BinaryWriter.Write(Calles[xx]);
                }
            }


            BinaryWriter.Write(CityFields.Veredas);
            if (Veredas == null) {
                BinaryWriter.Write(0);
            }
            else {
                BinaryWriter.Write(Veredas.Length);
                for (int xx = 0; xx < Veredas.Length; xx++) {
                    BinaryWriter.Write(Veredas[xx]);
                }
            }


            BinaryWriter.Flush();
            fs.Write(memoryStream.GetBuffer(), 0, Convert.ToInt32(memoryStream.Position));



            BinaryWriter.BaseStream.Close();
            fs.Flush();
            fs.Close();
            memoryStream.Close();
            BinaryWriter.Close();
        }



        static void AddLog(string log) {
            Console.Write(log);
        }

        private void CalculateNormals(PositionNormalTextureColor[] vertices, int[] indices){

            Vector3[] vertexNormals = new Vector3[vertices.Length];
            
            AccumulateTriangleNormals(indices, vertices, vertexNormals);

            for (int i = 0; i < vertexNormals.Length; i++) {
                vertexNormals[i] = SafeNormalize(vertexNormals[i]);
            }

            for (int i = 0; i < vertices.Length; i++) {
                vertices[i].Normal = vertexNormals[i];
            }

            
        }

        private static Vector3 SafeNormalize(Vector3 value) {
            float num = value.Length();
            if (num == 0) {
                return Vector3.Zero;
            }
            return (Vector3)(value / num);
        }

        private static void AccumulateTriangleNormals(int[] indices, PositionNormalTextureColor[] vertices, Vector3[] vertexNormals) {
            
            PositionNormalTextureColor[] positions = vertices;
            int[] positionIndices = indices;
            for (int i = 0; i < indices.Length - 3; i += 3) {
                Vector3 vector4 = vertices[indices[i]].Position;
                Vector3 vector = vertices[indices[i + 1]].Position;
                Vector3 vector3 = vertices[indices[i + 2]].Position;
                Vector3 vector2 = SafeNormalize(Vector3.Cross(vector3 - vector, vector - vector4));
                for (int j = 0; j < 3; j++) {
                    vertexNormals[positionIndices[indices[i + j]]] += vector2;
                }
            }
        }






        public void SerializeLotes(SerializedMesh sm, string path, PositionNormalTextureColor[] UnoptimizedVertices, int[] UnoptimizedIndices) {
            CalculateNormals(UnoptimizedVertices, UnoptimizedIndices);
            PositionNormalTextureColor[] Vertices = UnoptimizedVertices;
            int[] Indices = UnoptimizedIndices;




           // LayerHelper.OptimizeTriangles(UnoptimizedVertices, UnoptimizedIndices, out Vertices, out Indices);


            memoryStream = new MemoryStream();
            BinaryWriter = new BinaryWriter(memoryStream);


            Directory.CreateDirectory(Path.GetDirectoryName(path));
            FileStream fs = new FileStream(path + sm.Name + ".bin", FileMode.Create);
            //AddLog("writing " + path + sm.Name + ".bin");

            //BinaryWriter.Write(LayerFields.Mesh);
            //BinaryWriter.Write(sm.Name);
            WriteMeshHeader(BinaryWriter, sm);
            BinaryWriter.Write(sm.Features.Count);
            for (int xx = 0; xx < sm.Features.Count; xx++) {
                BinaryWriter.Write(sm.Features[xx].VertexStart);
                BinaryWriter.Write(sm.Features[xx].VertexEnd);
                BinaryWriter.Write(sm.Features[xx].Techo.Count);
            }

            /*
            for (int pp = 0; pp < sm.Features.Count; pp++) {
                BinaryWriter.Write(sm.Features[pp].Code);

            }*/

            BinaryWriter.Write(Indices.Length);
            for (int xx = 0; xx < Indices.Length; xx++) {
                BinaryWriter.Write(Indices[xx]);
            }

            BinaryWriter.Write(Vertices.Length);
            for (int xx = 0; xx < Vertices.Length; xx++) {
                BinaryWriter.Write(Vertices[xx].Position.X);
                BinaryWriter.Write(Vertices[xx].Position.Y);
                BinaryWriter.Write(Vertices[xx].Position.Z);
            }



            if (sm.Info.EnableNormals) {
                if (sm.Info.BuildNormals) {
                    BinaryWriter.Write(-1);
                }
                else {
                    BinaryWriter.Write(Vertices.Length);
                    for (int xx = 0; xx < Vertices.Length; xx++) {
                        BinaryWriter.Write(Vertices[xx].Normal.X);
                        BinaryWriter.Write(Vertices[xx].Normal.Y);
                        BinaryWriter.Write(Vertices[xx].Normal.Z);
                    }
                }
            }
            else {
                BinaryWriter.Write(0);
            }

            if (sm.Info.EnableTexture) {
                if (sm.Info.BuildTextureCoords) {
                    BinaryWriter.Write(-1);
                }
                else {
                    BinaryWriter.Write(Vertices.Length);
                    for (int xx = 0; xx < Vertices.Length; xx++) {
                        BinaryWriter.Write(Vertices[xx].TextureCoordinate.X);
                        BinaryWriter.Write(Vertices[xx].TextureCoordinate.Y);
                    }
                }
            }
            else {
                BinaryWriter.Write(0);
            }


            Color clr = Vertices[0].Color;
            bool hasSingleColor = true;
            for (int xx = 1; xx < Vertices.Length; xx++) {
                if (clr != Vertices[xx].Color) {
                    hasSingleColor = false;
                    break;
                }
            }

            if (hasSingleColor) {
                BinaryWriter.Write(0);
                BinaryWriter.Write(Vertices[0].Color.R);
                BinaryWriter.Write(Vertices[0].Color.G);
                BinaryWriter.Write(Vertices[0].Color.B);
            }
            else {
                BinaryWriter.Write(Vertices.Length);
                for (int xx = 0; xx < Vertices.Length; xx++) {
                    BinaryWriter.Write(Vertices[xx].Color.R);
                    BinaryWriter.Write(Vertices[xx].Color.G);
                    BinaryWriter.Write(Vertices[xx].Color.B);
                }

            }


            BinaryWriter.Flush();
            fs.Write(memoryStream.GetBuffer(), 0, Convert.ToInt32(memoryStream.Position));
            BinaryWriter.BaseStream.Close();
            fs.Flush();
            fs.Close();
            memoryStream.Close();
            BinaryWriter.Close();
            if (!sm.Info.DontCompress) {
                CompressionHelper.CompressFile(path + sm.Name + ".bin");
            }
        }





        public void SerializeMesh(SerializedMesh sm, string path, PositionNormalTextureColor[] UnoptimizedVertices, int[] UnoptimizedIndices){
            CalculateNormals(UnoptimizedVertices, UnoptimizedIndices);
            PositionNormalTextureColor[] Vertices = UnoptimizedVertices;
            int[] Indices = UnoptimizedIndices;




            LayerHelper.OptimizeTriangles(UnoptimizedVertices, UnoptimizedIndices, out Vertices, out Indices);
            GC.Collect();

            memoryStream = new MemoryStream();
            BinaryWriter = new BinaryWriter(memoryStream);


            Directory.CreateDirectory(Path.GetDirectoryName(path));
            FileStream fs = new FileStream(path + sm.Name + ".bin", FileMode.Create);
            //AddLog("writing " + path + sm.Name + ".bin");

            //BinaryWriter.Write(LayerFields.Mesh);
            //BinaryWriter.Write(sm.Name);
            WriteMeshHeader(BinaryWriter, sm);
            BinaryWriter.Write(sm.Features.Count);
            if (sm.Features.Count > 1) {
               // return;
            }

            for (int xx = 0; xx < sm.Features.Count; xx++) {
                BinaryWriter.Write(sm.Features[xx].VertexStart);
                BinaryWriter.Write(sm.Features[xx].VertexEnd);
                BinaryWriter.Write(sm.Features[xx].Techo.Count);
            }

            /*
            for (int pp = 0; pp < sm.Features.Count; pp++) {
                BinaryWriter.Write(sm.Features[pp].Code);

            }*/

            BinaryWriter.Write(Indices.Length);
            for (int xx = 0; xx < Indices.Length; xx++) {
                BinaryWriter.Write(Indices[xx]);
            }

            BinaryWriter.Write(Vertices.Length);
            for (int xx = 0; xx < Vertices.Length; xx++) {
                BinaryWriter.Write(Vertices[xx].Position.X);
                BinaryWriter.Write(Vertices[xx].Position.Y);
                BinaryWriter.Write(Vertices[xx].Position.Z);
            }



            if (sm.Info.EnableNormals) {
                if (sm.Info.BuildNormals){
                    BinaryWriter.Write(-1);
                }
                else{
                    BinaryWriter.Write(Vertices.Length);
                    for (int xx = 0; xx < Vertices.Length; xx++){
                        BinaryWriter.Write( Vertices[xx].Normal.X);
                        BinaryWriter.Write( Vertices[xx].Normal.Y);
                        BinaryWriter.Write( Vertices[xx].Normal.Z);
                    }
                }
            }
            else {
                BinaryWriter.Write(0);
            }

            if (sm.Info.EnableTexture) {
                if (sm.Info.BuildTextureCoords) {
                    BinaryWriter.Write(-1);
                }
                else{
                    BinaryWriter.Write(Vertices.Length);
                    for (int xx = 0; xx < Vertices.Length; xx++){
                        BinaryWriter.Write( Vertices[xx].TextureCoordinate.X);
                        BinaryWriter.Write( Vertices[xx].TextureCoordinate.Y);
                    }
                }
            }
            else {
                BinaryWriter.Write(0);
            }


            Color clr = Vertices[0].Color;
            bool hasSingleColor = true;
            for (int xx = 1; xx < Vertices.Length; xx++) {
                if(clr != Vertices[xx].Color){
                    hasSingleColor = false;
                    break;
                }
            }

            if (hasSingleColor){
                BinaryWriter.Write(0);
                BinaryWriter.Write(Vertices[0].Color.R);
                BinaryWriter.Write(Vertices[0].Color.G);
                BinaryWriter.Write(Vertices[0].Color.B);
            }else{
                BinaryWriter.Write(Vertices.Length);
                for (int xx = 0; xx < Vertices.Length; xx++) {
                    BinaryWriter.Write(Vertices[xx].Color.R);
                    BinaryWriter.Write(Vertices[xx].Color.G);
                    BinaryWriter.Write(Vertices[xx].Color.B);
                }
                
            }
            

            BinaryWriter.Flush();
            fs.Write(memoryStream.GetBuffer(), 0, Convert.ToInt32(memoryStream.Position));
            BinaryWriter.BaseStream.Close();
            fs.Flush();
            fs.Close();
            memoryStream.Close();
            BinaryWriter.Close();
            if(!sm.Info.DontCompress){
                CompressionHelper.CompressFile(path + sm.Name + ".bin");
            }
        }

        public void Se111rialize111Lotes1(List<Feature> Features, object component, string path, PositionNormalTextureColor[] UnoptimizedVertices, int[] UnoptimizedIndices, bool disableNormals, bool disableTextureCoordinates) {

            //LayerHelper.OptimizeTriangles(UnoptimizedVertices, UnoptimizedIndices);

            //LayerHelper.OptimizedIndices = UnoptimizedIndices;
            //LayerHelper.OptimizedVertices = UnoptimizedVertices;
            //PositionNormalTextureColor[] Vertices = LayerHelper.OptimizedVertices;
            //int[] Indices = LayerHelper.OptimizedIndices;

            PositionNormalTextureColor[] Vertices = null;
            int[] Indices;
            LayerHelper.OptimizeTriangles(UnoptimizedVertices, UnoptimizedIndices, out Vertices, out Indices);

            memoryStream = new MemoryStream();
            BinaryWriter = new BinaryWriter(memoryStream);
            //WriteHeader(component);


            SerializedMesh sm = (SerializedMesh)component;


            Directory.CreateDirectory(Path.GetDirectoryName(path));
            FileStream fs = new FileStream(path + sm.Name + ".bin", FileMode.Create);


            //BinaryWriter.Write(LayerFields.Mesh);
            //BinaryWriter.Write(sm.Name);
            //WriteHeader(BinaryWriter, sm.Info);
            BinaryWriter.Write(Features.Count);

            for (int pp = 0; pp < Features.Count; pp++) {
                BinaryWriter.Write(Features[pp].Code);
            }


            BinaryWriter.Write(Vertices.Length);
            for (int xx = 0; xx < Vertices.Length; xx++) {
                BinaryWriter.Write(Vertices[xx].Position.X);
                BinaryWriter.Write(Vertices[xx].Position.Y);
                BinaryWriter.Write(Vertices[xx].Position.Z);
            }

            BinaryWriter.Write(Indices.Length);
            for (int xx = 0; xx < Indices.Length; xx++) {
                BinaryWriter.Write(Indices[xx]);
            }

            if (!disableNormals) {
                BinaryWriter.Write(Vertices.Length);
                for (int xx = 0; xx < Vertices.Length; xx++) {
                    BinaryWriter.Write(Vertices[xx].Normal.X);
                    BinaryWriter.Write(Vertices[xx].Normal.Y);
                    BinaryWriter.Write(Vertices[xx].Normal.Z);
                }
            }
            else {
                BinaryWriter.Write(0);
            }

            if (!disableTextureCoordinates) {
                BinaryWriter.Write(Vertices.Length);
                for (int xx = 0; xx < Vertices.Length; xx++) {
                    BinaryWriter.Write(Vertices[xx].TextureCoordinate.X);
                    BinaryWriter.Write(Vertices[xx].TextureCoordinate.Y);
                }
            }
            else {
                BinaryWriter.Write(0);
            }

            BinaryWriter.Write(Vertices.Length);
            for (int xx = 0; xx < Vertices.Length; xx++) {
                BinaryWriter.Write(Vertices[xx].Color.R);
                BinaryWriter.Write(Vertices[xx].Color.G);
                BinaryWriter.Write(Vertices[xx].Color.B);
            }



            BinaryWriter.Flush();
            fs.Write(memoryStream.GetBuffer(), 0, Convert.ToInt32(memoryStream.Position));
            BinaryWriter.BaseStream.Close();
            fs.Flush();
            fs.Close();
            memoryStream.Close();
            BinaryWriter.Close();

            CompressionHelper.CompressFile(path + sm.Name + ".bin");
        }

        public void SerializeVeredas(List<Feature> Features, object component, string path, PositionNormalTextureColor[] UnoptimizedVertices, int[] UnoptimizedIndices, bool disableNormals, bool disableTextureCoordinates) {



            //LayerHelper.OptimizedIndices = UnoptimizedIndices;
            //LayerHelper.OptimizedVertices = UnoptimizedVertices;
            PositionNormalTextureColor[] Vertices = null;
            int[] Indices;
            LayerHelper.OptimizeTriangles(UnoptimizedVertices, UnoptimizedIndices, out Vertices, out Indices);
            //Vertices = UnoptimizedVertices;

            //Indices = UnoptimizedIndices;


            memoryStream = new MemoryStream();
            BinaryWriter = new BinaryWriter(memoryStream);
            //WriteHeader(component);


            SerializedMesh sm = (SerializedMesh)component;


            Directory.CreateDirectory(Path.GetDirectoryName(path));
            FileStream fs = new FileStream(path + sm.Name + ".bin", FileMode.Create);


            //BinaryWriter.Write(LayerFields.Mesh);
            BinaryWriter.Write(sm.Name);
            BinaryWriter.Write(sm.Info.Author);
            BinaryWriter.Write(sm.Info.DateCreation);
            //BinaryWriter.Write(Features.Count);

            BinaryWriter.Write(Vertices.Length);
            for (int xx = 0; xx < Vertices.Length; xx++) {
                BinaryWriter.Write(Vertices[xx].Position.X);
                BinaryWriter.Write(Vertices[xx].Position.Y);
                BinaryWriter.Write(Vertices[xx].Position.Z);
            }

            BinaryWriter.Write(Indices.Length);
            for (int xx = 0; xx < Indices.Length; xx++) {
                BinaryWriter.Write(Indices[xx]);
            }

            if (!disableNormals) {
                BinaryWriter.Write(Vertices.Length);
                for (int xx = 0; xx < Vertices.Length; xx++) {
                    BinaryWriter.Write(Vertices[xx].Normal.X);
                    BinaryWriter.Write(Vertices[xx].Normal.Y);
                    BinaryWriter.Write(Vertices[xx].Normal.Z);
                }
            }
            else {
                BinaryWriter.Write(0);
            }

            if (!disableTextureCoordinates) {
                BinaryWriter.Write(Vertices.Length);
                for (int xx = 0; xx < Vertices.Length; xx++) {
                    BinaryWriter.Write(Vertices[xx].TextureCoordinate.X);
                    BinaryWriter.Write(Vertices[xx].TextureCoordinate.Y);
                }
            }
            else {
                BinaryWriter.Write(0);
            }

            BinaryWriter.Write(Vertices.Length);
            for (int xx = 0; xx < Vertices.Length; xx++) {
                BinaryWriter.Write(Vertices[xx].Color.R);
                BinaryWriter.Write(Vertices[xx].Color.G);
                BinaryWriter.Write(Vertices[xx].Color.B);
            }
            BinaryWriter.Flush();
            fs.Write(memoryStream.GetBuffer(), 0, Convert.ToInt32(memoryStream.Position));



            BinaryWriter.BaseStream.Close();
            fs.Flush();
            fs.Close();
            memoryStream.Close();
            BinaryWriter.Close();

            CompressionHelper.CompressFile(path + sm.Name + ".bin");
        }

        public void SerializeMesh2(List<Feature> Features, object component, string path, PositionNormalTextureColor[] UnoptimizedVertices, int[] UnoptimizedIndices, bool disableNormals, bool disableTextureCoordinates) {



            //LayerHelper.OptimizedIndices = UnoptimizedIndices;
            //LayerHelper.OptimizedVertices = UnoptimizedVertices;
            PositionNormalTextureColor[] Vertices = null;
            int[] Indices;
            LayerHelper.OptimizeTriangles(UnoptimizedVertices, UnoptimizedIndices, out Vertices, out Indices);
            //Vertices = UnoptimizedVertices;

            //Indices = UnoptimizedIndices;


            memoryStream = new MemoryStream();
            BinaryWriter = new BinaryWriter(memoryStream);
            //WriteHeader(component);


            SerializedMesh sm = (SerializedMesh)component;


            Directory.CreateDirectory(Path.GetDirectoryName(path));
            FileStream fs = new FileStream(path + sm.Name + ".bin", FileMode.Create);


            //BinaryWriter.Write(LayerFields.Mesh);
            BinaryWriter.Write(sm.Name);
            BinaryWriter.Write(sm.Info.Author);
            BinaryWriter.Write(sm.Info.DateCreation);
            //BinaryWriter.Write(Features.Count);

            BinaryWriter.Write(Vertices.Length);
            for (int xx = 0; xx < Vertices.Length; xx++) {
                BinaryWriter.Write(Vertices[xx].Position.X);
                BinaryWriter.Write(Vertices[xx].Position.Y);
                BinaryWriter.Write(Vertices[xx].Position.Z);
            }

            BinaryWriter.Write(Indices.Length);
            for (int xx = 0; xx < Indices.Length; xx++) {
                BinaryWriter.Write(Indices[xx]);
            }

            if (!disableNormals) {
                BinaryWriter.Write(Vertices.Length);
                for (int xx = 0; xx < Vertices.Length; xx++) {
                    BinaryWriter.Write(Vertices[xx].Normal.X);
                    BinaryWriter.Write(Vertices[xx].Normal.Y);
                    BinaryWriter.Write(Vertices[xx].Normal.Z);
                }
            }
            else {
                BinaryWriter.Write(0);
            }

            if (!disableTextureCoordinates) {
                BinaryWriter.Write(Vertices.Length);
                for (int xx = 0; xx < Vertices.Length; xx++) {
                    BinaryWriter.Write(Vertices[xx].TextureCoordinate.X);
                    BinaryWriter.Write(Vertices[xx].TextureCoordinate.Y);
                }
            }
            else {
                BinaryWriter.Write(0);
            }

            BinaryWriter.Write(Vertices.Length);
            for (int xx = 0; xx < Vertices.Length; xx++) {
                BinaryWriter.Write(Vertices[xx].Color.R);
                BinaryWriter.Write(Vertices[xx].Color.G);
                BinaryWriter.Write(Vertices[xx].Color.B);
            }
            BinaryWriter.Flush();
            fs.Write(memoryStream.GetBuffer(), 0, Convert.ToInt32(memoryStream.Position));



            BinaryWriter.BaseStream.Close();
            fs.Flush();
            fs.Close();
            memoryStream.Close();
            BinaryWriter.Close();

            CompressionHelper.CompressFile(path + sm.Name + ".bin");
        }

        public void SerializeSiluetas(List<Feature> Features, object component, string path, PositionNormalTextureColor[] UnoptimizedVertices, int[] UnoptimizedIndices, bool disableNormals, bool disableTextureCoordinates) {



            //LayerHelper.OptimizedIndices = UnoptimizedIndices;
            //LayerHelper.OptimizedVertices = UnoptimizedVertices;
            PositionNormalTextureColor[] Vertices = null;
            int[] Indices;
            LayerHelper.OptimizeTriangles(UnoptimizedVertices, UnoptimizedIndices, out Vertices, out Indices);

            //Vertices = UnoptimizedVertices;

            //Indices = UnoptimizedIndices;

            memoryStream = new MemoryStream();
            BinaryWriter = new BinaryWriter(memoryStream);
            //WriteHeader(component);


            SerializedMesh sm = (SerializedMesh)component;


            Directory.CreateDirectory(Path.GetDirectoryName(path));
            FileStream fs = new FileStream(path + sm.Name + ".bin", FileMode.Create);


            //BinaryWriter.Write(LayerFields.Mesh);
            BinaryWriter.Write(sm.Name);
            BinaryWriter.Write(sm.Info.Author);
            BinaryWriter.Write(sm.Info.DateCreation);
            //BinaryWriter.Write(Features.Count);

            BinaryWriter.Write(Vertices.Length);
            for (int xx = 0; xx < Vertices.Length; xx++) {
                BinaryWriter.Write(Vertices[xx].Position.X);
                BinaryWriter.Write(Vertices[xx].Position.Y);
                BinaryWriter.Write(Vertices[xx].Position.Z);
            }

            BinaryWriter.Write(Indices.Length);
            for (int xx = 0; xx < Indices.Length; xx++) {
                BinaryWriter.Write(Indices[xx]);
            }

            if (!disableNormals) {
                BinaryWriter.Write(Vertices.Length);
                for (int xx = 0; xx < Vertices.Length; xx++) {
                    BinaryWriter.Write(Vertices[xx].Normal.X);
                    BinaryWriter.Write(Vertices[xx].Normal.Y);
                    BinaryWriter.Write(Vertices[xx].Normal.Z);
                }
            }
            else {
                BinaryWriter.Write(0);
            }

            if (!disableTextureCoordinates) {
                BinaryWriter.Write(Vertices.Length);
                for (int xx = 0; xx < Vertices.Length; xx++) {
                    BinaryWriter.Write(Vertices[xx].TextureCoordinate.X);
                    BinaryWriter.Write(Vertices[xx].TextureCoordinate.Y);
                }
            }
            else {
                BinaryWriter.Write(0);
            }

            BinaryWriter.Write(Vertices.Length);
            for (int xx = 0; xx < Vertices.Length; xx++) {
                BinaryWriter.Write(Vertices[xx].Color.R);
                BinaryWriter.Write(Vertices[xx].Color.G);
                BinaryWriter.Write(Vertices[xx].Color.B);
            }
            BinaryWriter.Flush();
            fs.Write(memoryStream.GetBuffer(), 0, Convert.ToInt32(memoryStream.Position));



            BinaryWriter.BaseStream.Close();
            fs.Flush();
            fs.Close();
            memoryStream.Close();
            BinaryWriter.Close();

            CompressionHelper.CompressFile(path + sm.Name + ".bin");
        }

        public void SerializeManzanas(List<Feature> Features, object component, string path, PositionNormalTextureColor[] UnoptimizedVertices, int[] UnoptimizedIndices, bool disableNormals, bool disableTextureCoordinates) {
            //LayerHelper.OptimizedIndices = UnoptimizedIndices;
            //LayerHelper.OptimizedVertices = UnoptimizedVertices;
            PositionNormalTextureColor[] Vertices = null;
            int[] Indices;
            LayerHelper.OptimizeTriangles(UnoptimizedVertices, UnoptimizedIndices, out Vertices, out Indices);

            // Vertices = UnoptimizedVertices;

            //Indices = UnoptimizedIndices;


            memoryStream = new MemoryStream();
            BinaryWriter = new BinaryWriter(memoryStream);
            


            SerializedMesh sm = (SerializedMesh)component;
            //WriteHeader(BinaryWriter, sm.Info);

            Directory.CreateDirectory(Path.GetDirectoryName(path));
            FileStream fs = new FileStream(path + sm.Name + ".bin", FileMode.Create);


            //BinaryWriter.Write(LayerFields.Mesh);
            //WriteHeader(BinaryWriter, sm.Info);
            
            //BinaryWriter.Write(sm.Info.DateCreation);

            BinaryWriter.Write(Features.Count);

            for (int pp = 0; pp < Features.Count; pp++) {
                BinaryWriter.Write(Features[pp].Code);
                BinaryWriter.Write(Features[pp].VertexStart);
                BinaryWriter.Write(Features[pp].VertexEnd);
            }

            BinaryWriter.Write(Vertices.Length);
            for (int xx = 0; xx < Vertices.Length; xx++) {
                BinaryWriter.Write(Vertices[xx].Position.X);
                BinaryWriter.Write(Vertices[xx].Position.Y);
                BinaryWriter.Write(Vertices[xx].Position.Z);
            }

            BinaryWriter.Write(Indices.Length);
            for (int xx = 0; xx < Indices.Length; xx++) {
                BinaryWriter.Write(Indices[xx]);
            }

            if (!disableNormals) {
                BinaryWriter.Write(Vertices.Length);
                for (int xx = 0; xx < Vertices.Length; xx++) {
                    BinaryWriter.Write(Vertices[xx].Normal.X);
                    BinaryWriter.Write(Vertices[xx].Normal.Y);
                    BinaryWriter.Write(Vertices[xx].Normal.Z);
                }
            }
            else {
                BinaryWriter.Write(0);
            }

            if (!disableTextureCoordinates) {
                BinaryWriter.Write(Vertices.Length);
                for (int xx = 0; xx < Vertices.Length; xx++) {
                    BinaryWriter.Write(Vertices[xx].TextureCoordinate.X);
                    BinaryWriter.Write(Vertices[xx].TextureCoordinate.Y);
                }
            }
            else {
                BinaryWriter.Write(0);
            }
            
            BinaryWriter.Write(Vertices.Length);
            for (int xx = 0; xx < Vertices.Length; xx++) {
                BinaryWriter.Write(Vertices[xx].Color.R);
                BinaryWriter.Write(Vertices[xx].Color.G);
                BinaryWriter.Write(Vertices[xx].Color.B);
            }
            BinaryWriter.Flush();
            fs.Write(memoryStream.GetBuffer(), 0, Convert.ToInt32(memoryStream.Position));



            BinaryWriter.BaseStream.Close();
            fs.Flush();
            fs.Close();
            memoryStream.Close();
            BinaryWriter.Close();

            CompressionHelper.CompressFile(path + sm.Name + ".bin");
        }

        private void WriteMeshHeader(BinaryWriter writer, SerializedMesh mesh){

            writer.Write(mesh.Name);

            writer.Write((byte)mesh.Info.LayerType);

            writer.Write(mesh.Info.DateCreation);
            writer.Write(mesh.Info.Author);
            writer.Write(mesh.Info.BaseLayer);

            writer.Write(mesh.Info.BuildNormals);
            writer.Write(mesh.Info.BuildTextureCoords);

            writer.Write((int)mesh.Info.LOD);
            writer.Write(mesh.Info.CreateVB);
        }
    }
}
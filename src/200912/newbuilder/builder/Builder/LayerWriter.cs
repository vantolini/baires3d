using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
//using Microsoft.DirectX.Direct3D;

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

    public enum LayerTypes {
        Manzanas,
        Veredas,
        Cordones,
        Silueta,
        Lotes,
        Calles,
        Subte,
        Puntos,
        Terrain
    }

    public class CiudadWriter {

        private MemoryStream memoryStream;
        private BinaryWriter BinaryWriter;

        public void SerializeCalles(string name, string path, List<Calle> Features) {
            memoryStream = new MemoryStream();
            BinaryWriter = new BinaryWriter(memoryStream);

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
                            BinaryWriter.Write(Features[f].Segmentos[t].Tramos[xx].Puntos[pt].Y);
                            BinaryWriter.Write(Features[f].Segmentos[t].Tramos[xx].Puntos[pt].Z);
                        }
                    }
                }
            }

            BinaryWriter.Flush();

           // Directory.CreateDirectory(Path.GetDirectoryName(path));
            FileStream fs = new FileStream(path + name + ".bin", FileMode.Create);
            fs.Write(memoryStream.GetBuffer(), 0, Convert.ToInt32(memoryStream.Position));
            fs.Flush();
            fs.Close();
            memoryStream.Close();
            BinaryWriter.Close();


            CompressionHelper.CompressFile(path + name + ".bin");
        }



        public void SerializePoints(SerializedPoints sm, string path) 
        {
            memoryStream = new MemoryStream();
            BinaryWriter = new BinaryWriter(memoryStream);
            //WriteHeader(component);

            //BinaryWriter.Write(LayerFields.Points);
            //BinaryWriter.Write("Points");

            //BinaryWriter.Write((int)LayerTypes.Puntos);

            // BinaryWriter.Write(sm.Info.Author);
            // BinaryWriter.Write(sm.Info.DateCreation);



            BinaryWriter.Write(sm.Points.Count);
            for (int xx = 0; xx < sm.Points.Count; xx++)
            {
                BinaryWriter.Write(sm.Points[xx].Name);
                BinaryWriter.Write(sm.Points[xx].Position.Z);
                BinaryWriter.Write(sm.Points[xx].Position.Y);
                BinaryWriter.Write(sm.Points[xx].Position.X);
                //BinaryWriter.Write(sm.Points[xx].Icon);
            }
            BinaryWriter.Flush();


            //Directory.CreateDirectory(Path.GetDirectoryName(path));
            FileStream fs = new FileStream(path + ".bin", FileMode.Create);
            fs.Write(memoryStream.GetBuffer(), 0, Convert.ToInt32(memoryStream.Position));
            fs.Flush();
            fs.Close();
            memoryStream.Close();
            BinaryWriter.Close();

            //CompressionHelper.CompressFile(path + ".bin");
            //CompressionHelper.CompressFile(path + ".bin");
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

            List<Color> cols = new List<Color>();
            for (int xx = 0; xx < features.Count; xx++) {
                for (int xp = 0; xp < features[xx].Vertices.Length; xp++) {
                    if (!cols.Contains(features[xx].Vertices[xp].Color)){
                        cols.Add(features[xx].Vertices[xp].Color);   
                    }
                }
            }
            BinaryWriter.Write(cols.Count);
            for (int cc = 0; cc < cols.Count; cc++) {
                BinaryWriter.Write(cols[cc].R);
                BinaryWriter.Write(cols[cc].G);
                BinaryWriter.Write(cols[cc].B);
                BinaryWriter.Write(cols[cc].A);
            }
            BinaryWriter.Write(vertexCount);
            for (int xx = 0; xx < features.Count; xx++) {
                CalculateNormals(features[xx].Vertices, features[xx].Indices);

                for (int xp = 0; xp < features[xx].Vertices.Length; xp++) {
                    BinaryWriter.Write((float)features[xx].Vertices[xp].Position.X);
                    BinaryWriter.Write((float)features[xx].Vertices[xp].Position.Y);
                    BinaryWriter.Write((float)features[xx].Vertices[xp].Position.Z);
                    BinaryWriter.Write(cols.IndexOf(features[xx].Vertices[xp].Color));
                }
            }
        }

        public void SerializeTerrain(
                string name, 
                string path, 
                List<Feature> terrain
            ) {

            if(name == "Nu�ez"){
                name = "Nunez";
            }

            SerializeBarrioMesh(name + "_terrain", path, LayerTypes.Terrain, terrain);

        }

        public void SerializeBarrio(
                string name, 
                string path, 
                List<Feature> Manzanas,
                List<Feature> Veredas,
                List<Feature> Silueta,
                List<Feature> Lotes
            ) {

            if(name == "Nu�ez"){
                name = "Nunez";
            }

            if(Lotes != null)
                SerializeBarrioMesh(name + "_lotes", path, LayerTypes.Lotes, Lotes);

            if (Manzanas != null)
                SerializeBarrioMesh(name + "_manzanas", path, LayerTypes.Manzanas, Manzanas);

            if (Veredas != null)
                SerializeBarrioMesh(name + "_veredas", path, LayerTypes.Veredas, Veredas);

            if (Silueta != null)
                SerializeBarrioMesh(name + "_silueta", path, LayerTypes.Silueta, Silueta);

        }

        public void SerializeSubte(
                string name,
                string path,
                List<Feature> Lineas,
                List<Feature> Estaciones

            ) {
            SerializeSubteMesh(name, path, LayerTypes.Subte, Lineas, Estaciones);

        }

        public void SerializeSubteMesh(
                string name,
                string path,
                LayerTypes lt,
                List<Feature> Items,
                List<Feature> Estaciones
            ) {


            memoryStream = new MemoryStream();
            BinaryWriter = new BinaryWriter(memoryStream);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            FileStream fs = new FileStream(path + name + ".bin", FileMode.Create);

            BinaryWriter.Write(name);
            BinaryWriter.Write((int)lt);


            WriteVertices(BinaryWriter, Items);

            BinaryWriter.Write(Items[0].Color.R);
            BinaryWriter.Write(Items[0].Color.G);
            BinaryWriter.Write(Items[0].Color.B);


            BinaryWriter.Write(Estaciones.Count);
            for (int xx = 0; xx < Estaciones.Count; xx++) {
                BinaryWriter.Write(Estaciones[xx].Name);
                for (int xp = 0; xp < Estaciones[xx].Points.Count; xp++) {
                    BinaryWriter.Write(Estaciones[xx].Points[xp].X);
                    BinaryWriter.Write(Estaciones[xx].Points[xp].Y);
                    BinaryWriter.Write(Estaciones[xx].Points[xp].Z);
                }
            }

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
                List<Feature> Items){

            //SerializeBarrioMeshX(name, path, lt, Items);
            //return;
            memoryStream = new MemoryStream();
            BinaryWriter = new BinaryWriter(memoryStream);
            //Directory.CreateDirectory(Path.GetDirectoryName(path));
            FileStream fs = new FileStream(path + name + ".bin", FileMode.Create);

            BinaryWriter.Write(name);
            BinaryWriter.Write((int)lt);



            int vertexCount = 0;

            for (int xx = 0; xx < Items.Count; xx++) {
                vertexCount += Items[xx].Vertices.Length;
            }

            //BinaryWriter.Write(Items.Count);
            for (int xx = 0; xx < Items.Count; xx++) {
                if (lt == LayerTypes.Silueta) {
                     //BinaryWriter.Write(Items[xx].Name);
                }
                //BinaryWriter.Write(Items[xx].Vertices.Length);
            }
             List<Color> cols = new List<Color>();
             for (int xx = 0; xx < Items.Count; xx++) {
                 for (int xp = 0; xp < Items[xx].Vertices.Length; xp++) {
                     if (!cols.Contains(Items[xx].Vertices[xp].Color)) {
                         cols.Add(Items[xx].Vertices[xp].Color);
                     }
                 }
             }
             BinaryWriter.Write(cols.Count);

             for (int cc = 0; cc < cols.Count; cc++) {
                 BinaryWriter.Write(cols[cc].R);
                 BinaryWriter.Write(cols[cc].G);
                 BinaryWriter.Write(cols[cc].B);
                 //BinaryWriter.Write((int)cols[cc].A);
             }
             

            int wrote = 0;
            BinaryWriter.Write(vertexCount);
            for (int xx = 0; xx < Items.Count; xx++) {
                //CalculateNormals(Items[xx].Vertices, Items[xx].Indices);                
                if (lt == LayerTypes.Silueta) {
                    //BinaryWriter.Write(Items[xx].Name);
                   // BinaryWriter.Write(Items[xx].Name);
                    BinaryWriter.Write(Items[xx].Vertices.Length);
                }
                for (int xp = 0; xp < Items[xx].Vertices.Length; xp++) {
                    //byte ix = (byte)MathHelper.Clamp(Items[xx].Vertices[xp].Position.X * 127.0f + 128.0f, 0.0f, 255.0f);
                    //byte iy = (byte)MathHelper.Clamp(Items[xx].Vertices[xp].Position.Y * 127.0f + 128.0f, 0.0f, 255.0f);
                    //byte iz = (byte)MathHelper.Clamp(Items[xx].Vertices[xp].Position.Z * 127.0f + 128.0f, 0.0f, 255.0f);

                    BinaryWriter.Write((float)Items[xx].Vertices[xp].Position.Z);
                    BinaryWriter.Write((float)Items[xx].Vertices[xp].Position.Y);
                    BinaryWriter.Write((float)Items[xx].Vertices[xp].Position.X);
                    //BinaryWriter.Write((float)Items[xx].Vertices[xp].Normal.Z);
                    //BinaryWriter.Write((float)Items[xx].Vertices[xp].Normal.Y);
                    //BinaryWriter.Write((float)Items[xx].Vertices[xp].Normal.X);
                    BinaryWriter.Write(cols.IndexOf(Items[xx].Vertices[xp].Color));
                }
                wrote += Items[xx].Vertices.Length;
            }
           
            BinaryWriter.Flush();
            fs.Write(memoryStream.GetBuffer(), 0, Convert.ToInt32(memoryStream.Position));
            BinaryWriter.BaseStream.Close();
            fs.Flush();
            fs.Close();
            memoryStream.Close();
            BinaryWriter.Close();
            //CompressionHelper.CompressFile(path + name + ".bin");
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
    }
}
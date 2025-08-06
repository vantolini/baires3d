using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace b3d
{

    public static class LayerFields {
        public const byte Mesh = 1;
        public const byte Points = 2;
        public const byte Streets = 3;

        public const byte Positions = 4;
        public const byte Normals = 5;
        public const byte TextureCoordinates = 6;
        public const byte Colors = 7;
        public const byte Indices = 8;
        public const byte Point = 9;
        public const byte StreetFeatureCount = 10;
        public const byte StreetFeature = 11;
        public const byte ColumnCount = 12;
        public const byte ColumnData = 13;
        public const byte LayerGroup = 14;
        public const byte Intersections = 14;
        public const byte StreetFeatureTramo = 15;
        public const byte StreetFeatureIntersection = 16;
        public const byte StreetFeatureNames = 17;
    }


    public static class CityFields {
        public const byte Name = 1;
        public const byte Version = 2;
        public const byte Date = 3;
        public const byte Author = 4;
        public const byte URL = 5;


        public const byte Lotes = 6;
        public const byte Manzanas = 7;
        public const byte Barrios = 8;
        public const byte Puntos = 9;
        public const byte Calles = 10;
    }

    public class CiudadWriter {
        private MemoryStream memoryStream;
        private BinaryWriter BinaryWriter;


        public void SerializeCiudad(string nombre, string path, string[] Barrios, string[] Manzanas, string[] Lotes, string[] Calles, string[] Puntos) {

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

            if(packAmount == 0){
                throw new Exception("WTF???");
            }

            packAmount = 5;
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


            BinaryWriter.Flush();
            fs.Write(memoryStream.GetBuffer(), 0, Convert.ToInt32(memoryStream.Position));



            BinaryWriter.BaseStream.Close();
            fs.Flush();
            fs.Close();
            memoryStream.Close();
            BinaryWriter.Close();
        }

        public void SerializeLotes(List<Feature> Features, object component, string path, PositionNormalTextureColor[] UnoptimizedVertices, int[] UnoptimizedIndices, bool disableNormals, bool disableTextureCoordinates) {

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


            BinaryWriter.Write(LayerFields.Mesh);
            BinaryWriter.Write(sm.Name);
            BinaryWriter.Write(sm.Info.Author);
            BinaryWriter.Write(sm.Info.DateCreation);
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
        public void SerializeManzanas(List<Feature> Features, object component, string path, PositionNormalTextureColor[] UnoptimizedVertices, int[] UnoptimizedIndices, bool disableNormals, bool disableTextureCoordinates) {
            PositionNormalTextureColor[] Vertices = null;
            int[] Indices;
            LayerHelper.OptimizeTriangles(UnoptimizedVertices, UnoptimizedIndices, out Vertices, out Indices);

            memoryStream = new MemoryStream();
            BinaryWriter = new BinaryWriter(memoryStream);
            //WriteHeader(component);


            SerializedMesh sm = (SerializedMesh)component;


            Directory.CreateDirectory(Path.GetDirectoryName(path));
            FileStream fs = new FileStream(path + sm.Name + ".bin", FileMode.Create);


            BinaryWriter.Write(LayerFields.Mesh);
            BinaryWriter.Write(sm.Name);
            BinaryWriter.Write(sm.Info.Author);
            BinaryWriter.Write(sm.Info.DateCreation);
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
        public void SerializeBarrios(List<Feature> Features, object component, string path, PositionNormalTextureColor[] UnoptimizedVertices, int[] UnoptimizedIndices, bool disableNormals, bool disableTextureCoordinates) {
            PositionNormalTextureColor[] Vertices = null;
            int[] Indices;
            LayerHelper.OptimizeTriangles(UnoptimizedVertices, UnoptimizedIndices, out Vertices, out Indices);



            memoryStream = new MemoryStream();
            BinaryWriter = new BinaryWriter(memoryStream);
            //WriteHeader(component);


            SerializedMesh sm = (SerializedMesh)component;


            Directory.CreateDirectory(Path.GetDirectoryName(path));
            FileStream fs = new FileStream(path + sm.Name + ".bin", FileMode.Create);


            BinaryWriter.Write(LayerFields.Mesh);
            BinaryWriter.Write(sm.Name);
            BinaryWriter.Write(sm.Info.Author);
            BinaryWriter.Write(sm.Info.DateCreation);
            BinaryWriter.Write(Vertices.Length);
            for (int xx = 0; xx < Vertices.Length; xx++) {
                BinaryWriter.Write(Vertices[xx].Position.X);
                BinaryWriter.Write(Vertices[xx].Position.Y);
                BinaryWriter.Write(Vertices[xx].Position.Z);
            }

            BinaryWriter.Write(LayerFields.Indices);
            BinaryWriter.Write(Indices.Length);
            for (int xx = 0; xx < Indices.Length; xx++) {
                BinaryWriter.Write(Indices[xx]);
            }

            if (!disableNormals) {
                BinaryWriter.Write(LayerFields.Normals);
                BinaryWriter.Write(Vertices.Length);
                for (int xx = 0; xx < Vertices.Length; xx++) {
                    BinaryWriter.Write(Vertices[xx].Normal.X);
                    BinaryWriter.Write(Vertices[xx].Normal.Y);
                    BinaryWriter.Write(Vertices[xx].Normal.Z);
                }
            }
            else {
                BinaryWriter.Write(LayerFields.Normals);
                BinaryWriter.Write(0);
            }

            if (!disableTextureCoordinates) {
                BinaryWriter.Write(LayerFields.TextureCoordinates);
                BinaryWriter.Write(Vertices.Length);
                for (int xx = 0; xx < Vertices.Length; xx++) {
                    BinaryWriter.Write(Vertices[xx].TextureCoordinate.X);
                    BinaryWriter.Write(Vertices[xx].TextureCoordinate.Y);
                }
            }
            else {
                BinaryWriter.Write(LayerFields.TextureCoordinates);
                BinaryWriter.Write(0);
            }

            BinaryWriter.Write(LayerFields.Colors);
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


        public void SerializeIntersections(string name, string path, List<Feature> Features, List<string> uniqueStreets) {
            memoryStream = new MemoryStream();
            BinaryWriter = new BinaryWriter(memoryStream);
            //WriteHeader(component);


            Color CurrentColor = new Color(255, 128, 0);

            BinaryWriter.Write(LayerFields.Intersections);
            BinaryWriter.Write(name);
            BinaryWriter.Write("Victor");
            BinaryWriter.Write("23/05/1983");


            BinaryWriter.Write(uniqueStreets.Count);
            for (int f = 0; f < uniqueStreets.Count; f++) {
                BinaryWriter.Write(uniqueStreets[f]);
            }
            BinaryWriter.Write(Features.Count);
            for (int f = 0; f < Features.Count; f++) {
                BinaryWriter.Write(Features[f].ID);
                BinaryWriter.Write(Features[f].Intersections.Count);
                for (int t = 0; t < Features[f].Intersections.Count; t++) {
                    BinaryWriter.Write(Features[f].Intersections[t]);
                }
            }


            BinaryWriter.Write(LayerFields.ColumnCount);
            BinaryWriter.Write(Features[0].Data.Count);
            for (int f = 0; f < Features.Count; f++) {
                for (int xx = 0; xx < Features[f].Data.Count; xx++) {
                    BinaryWriter.Write(Features[f].Data[xx]);
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

            CompressionHelper.CompressFile(path + name + ".bin");
        }


        public void SerializePoints(object component, string path) {
            memoryStream = new MemoryStream();
            BinaryWriter = new BinaryWriter(memoryStream);
            //WriteHeader(component);

            SerializedPoints sm = (SerializedPoints)component;

            BinaryWriter.Write(LayerFields.Points);
            BinaryWriter.Write(sm.Name);
            BinaryWriter.Write(sm.Info.Author);
            BinaryWriter.Write(sm.Info.DateCreation);


            BinaryWriter.Write(sm.Points.Count);
            for (int xx = 0; xx < sm.Points.Count; xx++) {
                BinaryWriter.Write(sm.Points[xx].Position.X);
                BinaryWriter.Write(sm.Points[xx].Position.Y);
                BinaryWriter.Write(sm.Points[xx].Position.Z);
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
        }
        public void SerializeCalles(string name, string path, List<Feature> Features) {
            memoryStream = new MemoryStream();
            BinaryWriter = new BinaryWriter(memoryStream);
            //WriteHeader(component);


            Color CurrentColor = new Color(255, 128, 0);

            BinaryWriter.Write(LayerFields.Streets);
            BinaryWriter.Write(name);
            BinaryWriter.Write("Calles");
            BinaryWriter.Write("??????");


            BinaryWriter.Write(Features.Count);
            for (int f = 0; f < Features.Count; f++) {
                BinaryWriter.Write(LayerFields.StreetFeature);
                BinaryWriter.Write(Features[f].ID);
                BinaryWriter.Write(Features[f].Tramos.Count);
                for (int t = 0; t < Features[f].Tramos.Count; t++) {
                    BinaryWriter.Write(Features[f].Tramos[t].OrigLine.Count);
                    for (int xx = 0; xx < Features[f].Tramos[t].OrigLine.Count; xx++) {
                        BinaryWriter.Write(Features[f].Tramos[t].OrigLine[xx].X);
                        BinaryWriter.Write(Features[f].Tramos[t].OrigLine[xx].Y);
                        BinaryWriter.Write(Features[f].Tramos[t].OrigLine[xx].Z);
                    }
                }
            }

            BinaryWriter.Write(LayerFields.ColumnCount);
            BinaryWriter.Write(Features[0].Data.Count);
            for (int f = 0; f < Features.Count; f++) {
                for (int xx = 0; xx < Features[f].Data.Count; xx++) {
                    BinaryWriter.Write(Features[f].Data[xx]);
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

            CompressionHelper.CompressFile(path + name + ".bin");
        }

        internal void SerializeExtra(string path, string[] extra) {
            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter BinaryWriter = new BinaryWriter(memoryStream);

            Directory.CreateDirectory(Path.GetDirectoryName(path));
            FileStream fs = new FileStream(path +  "Extra.bin", FileMode.Create);


            BinaryWriter.Write(CityFields.Name);
            BinaryWriter.Write("Capital Extra");
            BinaryWriter.Write("PITOR");
            BinaryWriter.Write(DateTime.Now.ToUniversalTime().ToString());
            BinaryWriter.Write(1);
            for (int xx = 0; xx < extra.Length; xx++) {
                BinaryWriter.Write(Path.GetFileNameWithoutExtension(extra[xx]));
            }
            
            BinaryWriter.Flush();
            fs.Write(memoryStream.GetBuffer(), 0, Convert.ToInt32(memoryStream.Position));
            BinaryWriter.BaseStream.Close();
            fs.Flush();
            fs.Close();
            memoryStream.Close();
            BinaryWriter.Close();
        }

        public void SerializeExtraMesh(List<Feature> Features, object component, string path, PositionNormalTextureColor[] UnoptimizedVertices, int[] UnoptimizedIndices, bool disableNormals, bool disableTextureCoordinates) {
            PositionNormalTextureColor[] Vertices = UnoptimizedVertices;
            int[] Indices = UnoptimizedIndices;


            memoryStream = new MemoryStream();
            BinaryWriter = new BinaryWriter(memoryStream);
            //WriteHeader(component);


            SerializedMesh sm = (SerializedMesh)component;


            Directory.CreateDirectory(Path.GetDirectoryName(path));
            FileStream fs = new FileStream(path + sm.Name + ".bin", FileMode.Create);


            BinaryWriter.Write(LayerFields.Mesh);
            BinaryWriter.Write(sm.Name);
            BinaryWriter.Write(sm.Info.Author);
            BinaryWriter.Write(sm.Info.DateCreation);


            BinaryWriter.Write(Features[0].VertexStart);
            BinaryWriter.Write(Features[Features.Count - 1].VertexEnd);
            for (int xx = 0; xx < Vertices.Length; xx++) {
                BinaryWriter.Write(Vertices[xx].Position.X);
                BinaryWriter.Write(Vertices[xx].Position.Y);
                BinaryWriter.Write(Vertices[xx].Position.Z);
            }

            BinaryWriter.Write(LayerFields.Indices);
            BinaryWriter.Write(Indices.Length);
            for (int xx = 0; xx < Indices.Length; xx++) {
                BinaryWriter.Write(Indices[xx]);
            }

            if (!disableNormals) {
                BinaryWriter.Write(LayerFields.Normals);
                BinaryWriter.Write(Vertices.Length);
                for (int xx = 0; xx < Vertices.Length; xx++) {
                    BinaryWriter.Write(Vertices[xx].Normal.X);
                    BinaryWriter.Write(Vertices[xx].Normal.Y);
                    BinaryWriter.Write(Vertices[xx].Normal.Z);
                }
            }
            else {
                BinaryWriter.Write(LayerFields.Normals);
                BinaryWriter.Write(0);
            }

            if (!disableTextureCoordinates) {
                BinaryWriter.Write(LayerFields.TextureCoordinates);
                BinaryWriter.Write(Vertices.Length);
                for (int xx = 0; xx < Vertices.Length; xx++) {
                    BinaryWriter.Write(Vertices[xx].TextureCoordinate.X);
                    BinaryWriter.Write(Vertices[xx].TextureCoordinate.Y);
                }
            }
            else {
                BinaryWriter.Write(LayerFields.TextureCoordinates);
                BinaryWriter.Write(0);
            }

            BinaryWriter.Write(LayerFields.Colors);
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
    }

    public class StreetLayerWriter
    {
        private MemoryStream memoryStream;
        private BinaryWriter BinaryWriter;


    }


}
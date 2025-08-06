using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ar3d {
    public class LayerReader {
        private byte type;
        public string[] DeserializeIndex(string p) {
            FileStream op = new FileStream(p, FileMode.Open);
            BinaryReader rdr = new BinaryReader(op);
            byte lyrgrp = rdr.ReadByte();
            type = rdr.ReadByte();

            int len = rdr.ReadInt32();
            string[] ret = new string[len];
            for(int i = 0; i < len; i++){
                 ret[i] = rdr.ReadString();
            }
            rdr.Close();
            op.Close();
            return ret;
        }

        private FileStream op;
        private BinaryReader rdr;


        public IntersectionsComponent DeserializeIntersections(string path) {

            string[] Layers = DeserializeIndex(path);

            IntersectionsComponent ic = new IntersectionsComponent();
            ic.Name = Path.GetFileNameWithoutExtension(path);
            string pt3 = Path.GetDirectoryName(path);
            for (int i = 0; i < Layers.Length; i++) {
                op = new FileStream(pt3 + "\\" + ic.Name + "\\" + Layers[i] + ".bin", FileMode.Open);

                rdr = new BinaryReader(op);
                byte typ = rdr.ReadByte();

                //ic.Calles = ReadIntersections(ref rdr, ic);


               // ic.Calles = ReadIntersections(ref rdr);
            }
            Constants.StreetManager = ic;
            return ic;

        }

        public RenderableComponent Deserialize(string path){

            string[] Layers = DeserializeIndex(path);

            switch (type) {
                case LayerFields.Mesh:
                    MeshComponent mc = new MeshComponent();
                    mc.Name = Path.GetFileNameWithoutExtension(path);
                    string pt = Path.GetDirectoryName(path);
                    for(int i = 0; i < Layers.Length;i++){
                        if (File.Exists(pt + "\\" + mc.Name + "\\" + Layers[i] + ".bin")){
                            op = new FileStream(pt + "\\" + mc.Name + "\\" + Layers[i] + ".bin", FileMode.Open);
                            
                            rdr = new BinaryReader(op);
                            byte typ = rdr.ReadByte();
                            string name = rdr.ReadString();
                            string Author = rdr.ReadString();
                            string DateCreation = rdr.ReadString();
                            
                            mc.Parts.Add(ReadMesh(ref rdr, ref name));
                        }
                    }
                    mc.BuildBigBuffer();
                    mc.CreateBounds();
                    
                    return mc;
                case LayerFields.Points:
                    PointsComponent pc = new PointsComponent();
                    pc.Name = Path.GetFileNameWithoutExtension(path);
                    string pt3 = Path.GetDirectoryName(path);
                    for (int i = 0; i < Layers.Length; i++) {
                        op = new FileStream(pt3 + "\\" + pc.Name + "\\" + Layers[i] + ".bin", FileMode.Open);

                        rdr = new BinaryReader(op);
                        byte typ = rdr.ReadByte();
                        string name = rdr.ReadString();
                        string Author = rdr.ReadString();
                        string DateCreation = rdr.ReadString();

                        pc.Points = ReadPoints(ref rdr);
                    }
                    pc.CreateBounds();
                    return pc;
                case LayerFields.Streets:
                    CallesComponent cc = new CallesComponent();
                    cc.Name = Path.GetFileNameWithoutExtension(path);
                    string pt2 = Path.GetDirectoryName(path);
                    for (int i = 0; i < Layers.Length; i++) {
                        op = new FileStream(pt2 + "\\" + cc.Name + "\\" + Layers[i] + ".bin", FileMode.Open);

                        rdr = new BinaryReader(op);
                        byte typ = rdr.ReadByte();
                        string name = rdr.ReadString();
                        string Author = rdr.ReadString();
                        string DateCreation = rdr.ReadString();

                        cc.Calles = ReadStreets(ref rdr);
                    }
                    cc.InitRoundLines();
                    cc.CreateBounds();
                    return cc;
            }
            
            return null;
        }

        private List<PointComponent> ReadIntersections(ref  BinaryReader rdr, IntersectionsComponent ic) {

            int ptcount = rdr.ReadInt32();
            List<PointComponent> pnts = new List<PointComponent>(ptcount);

            for (int i = 0; i < ptcount; i++) {
                PointComponent pt = new PointComponent();

                pt.Position = new Vector3(
                    rdr.ReadSingle(),
                    rdr.ReadSingle(),
                    rdr.ReadSingle()
                );
                pt.TextureIndex = rdr.ReadInt32();
                pt.Name = rdr.ReadString();
                pt.CreateBounds();
                pnts.Add(pt);
            }
            return pnts;
        }

        private List<CalleComponent> ReadStreets(ref BinaryReader rdr) {
            int calles_count = rdr.ReadInt32();
            List<CalleComponent> calles = new List<CalleComponent>(calles_count);

            for (int i = 0; i < calles_count; i++) {
                CalleComponent pt = new CalleComponent();
                byte soycalle = rdr.ReadByte();
                int cant = rdr.ReadInt32();
                //BinaryWriter.Write(LayerFields.StreetFeature);
                //BinaryWriter.Write(Features[f].OrigLine.Count);
                pt.Puntos = new List<Vector3>(cant);
                for (int xx = 0; xx < cant; xx++) {
                    float fx = rdr.ReadSingle();
                    float fy = rdr.ReadSingle();
                    float fz = rdr.ReadSingle();

                        pt.Puntos.Add(new Vector3(
                       fx,
                       fy,
                       fz
                   ));
                    
                }
                calles.Add(pt);
            }

            byte cc = rdr.ReadByte();
            int cantcols = rdr.ReadInt32();
            for (int xx = 0; xx < calles.Count; xx++) {
                for (int c = 0; c < cantcols; c++) {
                    calles[xx].Data.Add(rdr.ReadString());
                }
            }
            
            for (int xx = 0; xx < calles.Count; xx++){
                int level = 0;
                if (calles[xx].Data.Count > 4) {
                    switch (calles[xx].Data[4]) {
                        case "path":
                        case "pedestrian":
                        case "unclassified":
                        case "service":
                        case "steps":
                        case "residential":

                            level = 0;
                            break;
                        case "tertiary":
                        case "track":
                        case "motorway":
                        case "motorway_link":
                            level = 1;
                            break;
                        case "primary":
                        case "primary_link":
                            level = 2;
                            break;
                        case "trunk":
                        case "trunk_link":
                        case "secondary":
                        case "secondary_link":
                            level = 3;
                            break;

                    }
                }
                else {
                    level = 3;
                }
                calles[xx].Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(calles[xx].Data[0].ToLower());
                if (calles[xx].Name.Contains("Avda") || calles[xx].Name.Contains("Avenida") || calles[xx].Name.Contains("Av.") || calles[xx].Name.Contains("Av ") || calles[xx].Name.Contains("Autopist")) {
                    calles[xx].Avenida = true;
                    calles[xx].StreetType = "Avenida";
                }
                else {
                    calles[xx].StreetType = "Calle";
                }
                calles[xx].Importance = level;
                
                calles[xx].CreateBounds();
             }
            return calles;
        }
        private MeshComponentPart ReadMesh(ref BinaryReader rdr, ref string name) {

            bool hasNormals = false;
            bool hasTx = false;

            byte pos = rdr.ReadByte();
            int vxlenght = rdr.ReadInt32();
            
            Vector3[] Positions = new Vector3[vxlenght];
            Color[] Colors = new Color[vxlenght];

            for (int i = 0; i < vxlenght; i++){
                float pX = rdr.ReadSingle();
                float pY = rdr.ReadSingle();
                float pZ = rdr.ReadSingle();

                Vector3 Position = new Vector3(
                    pX,
                    pY,
                    pZ
                    );
                Positions[i] = Position;
            }

            byte nr = rdr.ReadByte();
            int xxx = rdr.ReadInt32();
            Vector3[] Normals = null;
            
            if (xxx != 0){
                hasNormals = true;
                Normals = new Vector3[vxlenght];
                for (int i = 0; i < vxlenght; i++){

                    float nX = rdr.ReadSingle();
                    float nY = rdr.ReadSingle();
                    float nZ = rdr.ReadSingle();

                    Vector3 Normal = new Vector3(
                        nX,
                        nY,
                        nZ
                        );
                    Normals[i] = Normal;
                }
            }

            byte tx = rdr.ReadByte();
            int xxxx = rdr.ReadInt32();
            Vector2[] TextureCoordinates = null;

            if (xxxx != 0){
                hasTx = true;
                TextureCoordinates = new Vector2[vxlenght];

                for (int i = 0; i < vxlenght; i++){
                    float tX = rdr.ReadSingle();
                    float tY = rdr.ReadSingle();
                    Vector2 TextureCoordinate = new Vector2(
                        tX,
                        tY
                        );

                    TextureCoordinates[i] = TextureCoordinate;
                }
            }

            byte cl = rdr.ReadByte();
            int xxxxx = rdr.ReadInt32();
            for (int i = 0; i < vxlenght; i++){
                Colors[i] = new Color(rdr.ReadByte(), rdr.ReadByte(), rdr.ReadByte());
            }





            byte ix = rdr.ReadByte();
            int xxxxxx = rdr.ReadInt32();
            int[] indices = new int[xxxxxx];
            for (int i = 0; i < xxxxxx; i++){
                indices[i] = rdr.ReadInt32();
            }

            MeshComponentPart mcp;
            mcp = new MeshComponentPart(name, Positions, Colors, Normals, TextureCoordinates, indices);
            
            mcp.CreateBounds();
            return mcp;
        }
        private List<PointComponent> ReadPoints(ref  BinaryReader rdr) {

            int ptcount = rdr.ReadInt32();
            List<PointComponent> pnts = new  List<PointComponent>(ptcount);

            for (int i = 0; i < ptcount; i++ ){
                PointComponent pt = new PointComponent();

                pt.Position = new Vector3(
                    rdr.ReadSingle(),
                    rdr.ReadSingle(),
                    rdr.ReadSingle()
                );
                pt.TextureIndex = rdr.ReadInt32();
                pt.Name = rdr.ReadString();
                pt.CreateBounds();
                pnts.Add(pt);
            }
            return pnts;
        }

    }
}
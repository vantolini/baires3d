using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace b3d
{
    public class CityReader {
        private byte type;
        public string Name;

        private FileStream op;
        private BinaryReader rdr;
        public CallesCollection Calles;
        public PuntosCollection Points;
        public BarriosCollection Barrios;
        public bool SkipBarrios = false;
        public bool SkipManzanas = false;
        public bool SkipLotes = false;
        public bool SkipPuntos = false;
        public CityReader(string name) {
            Name = name;
        }

        
        public void Process() {
            FileStream op = new FileStream(Name, FileMode.Open);
            BinaryReader rdr = new BinaryReader(op);

            rdr.ReadByte();
            string cityName = rdr.ReadString();
            string author = rdr.ReadString();
            string date = rdr.ReadString();

            List<Barrio> barrios = new  List<Barrio>();

            Barrios = new BarriosCollection();
            for(int x = 0; x < Constants.Barrios.Length;x++){
                barrios.Add(new Barrio(Constants.Barrios[x]));
                Barrios.Add(barrios[x]);
            }
            
            int packAmounts = rdr.ReadInt32();
            if (packAmounts > 0){
                for (int i = 0; i < packAmounts; i++){
                    byte packType = rdr.ReadByte();
                    int lenPack = rdr.ReadInt32();
                    if(lenPack == 0){
                        continue;
                    }
                    string[] packs = new string[lenPack];

                    switch (packType){
                        case CityFields.Barrios:
                            Console.WriteLine("Loading barrios layer...");
                            int currr = 0;
                            for (int p = 0; p < lenPack; p++) {
                                packs[p] = rdr.ReadString();
                                string fn = Path.GetFileNameWithoutExtension(Name);
                                //Console.WriteLine("Loading barrio " + packs[p]);
                                string path = Constants.DataPath +  @"\Ciudades\" + fn + @"\Barrios\" + packs[p] + ".7z";

                                MeshComponent manzanas = ReadBarrios(ref path);
                                manzanas.CreateBounds();
                                /*
                                if (meshPart == null || meshPart.Name == "") {

                                    throw new Exception();
                                    break;
                                }
                                meshPart.Enabled = true;*/
                                currr += manzanas.MeshParts[0].Positions.Length;

                                manzanas.VertexStart = currr - manzanas.MeshParts[0].Positions.Length;
                                manzanas.VertexEnd = currr;
                                Barrios[packs[p]].Mesh = manzanas;
                                Barrios[packs[p]].Mesh.BuildBigBuffer();
                                Barrios[packs[p]].Mesh.CreateBounds();
                            }
                            break;
                        case CityFields.Manzanas:
                            Console.WriteLine("Loading manzanas layer...");
                            for (int p = 0; p < lenPack; p++) {
                                packs[p] = rdr.ReadString();
                                string fn = Path.GetFileNameWithoutExtension(Name);
                                //Console.WriteLine("Loading manzanas " + packs[p]);
                                string path = Constants.DataPath + @"\Ciudades\" + fn + @"\Manzanas\" + packs[p] + ".7z";
                                mz = null;
                                List<Manzana> manzanas = ReadManzanas(ref path);

                                /*if (manzanas == null || manzanas.Name == "") {

                                    throw new Exception();
                                    break;
                                }
                                manzanas.Enabled = true;*/
                                Barrios[packs[p]].AddManzanas(manzanas, mz);
                            }
                            break;
                        case CityFields.Lotes:
                            Console.WriteLine("Loading lotes layer...");
                            for (int p = 0; p < lenPack; p++) {
                                packs[p] = rdr.ReadString();
                                if(packs[p].Contains("Almagro") ||
                                    packs[p].Contains("Balvanera") ||
                                    packs[p].Contains("San Nicolas")
                                    
                                    ){
                                    
                               
                                    string fn = Path.GetFileNameWithoutExtension(Name);
                                    //Console.WriteLine("Loading lotes " + packs[p]);
                                    string path = Constants.DataPath + @"\Ciudades\" + fn + @"\Lotes\" + packs[p] + ".7z";
                                    MeshComponent mc = new MeshComponent();
                                    Barrios[packs[p]].Lotes = mc;

                                    MeshComponentPart lotes = ReadLotes(ref path);
                                    /*
                                    if (meshPart == null || meshPart.Name == "") {

                                        throw new Exception();
                                        break;
                                    }
                                    meshPart.Enabled = true;*/


                                    Barrios[packs[p]].Lotes.MeshParts.Add(lotes);

                                    Barrios[packs[p]].Lotes.BuildBigBuffer();

                                    Barrios[packs[p]].Lotes.CreateBounds();
                                }
                            }
                            
                            break;
                        case CityFields.Puntos:
                           
                            for (int p = 0; p < lenPack; p++) {
                                
                                packs[p] = rdr.ReadString();


                                PointsComponent pc = new PointsComponent();
                                pc.Name = packs[p];


                                string fn = Path.GetFileNameWithoutExtension(Name);
                                string path = Constants.DataPath + @"\Ciudades\" + fn + @"\Puntos\" + packs[p] + ".bin";

                                pc.Points = ReadPoints(ref path);
                                pc.CreateBounds();

                                if (Points == null) {
                                    Points = new PuntosCollection();
                                }

                                Points.Add(pc);
                            }
                            
                            break;
                        case CityFields.Calles:
                            for (int p = 0; p < lenPack; p++) {
                                packs[p] = rdr.ReadString();
                                Calles = new CallesCollection();

                                string fn = Path.GetFileNameWithoutExtension(Name);
                                string path = Constants.DataPath + @"\Ciudades\" + fn + @"\Calles\" + packs[p] + ".7z";
                                List<CalleComponent> Call = new List<CalleComponent>();
                                Call = ReadStreets(ref path);

                                for (int c = 0; c < Call.Count;c++ ){
                                    Calles.Add(Call[c]);
                                }
                                Calles.Init();
                                Calles.CreateBounds();
                            }
                            break;

                    }
                }
            }

            for (int x = 0; x < Barrios.Count; x++) {
                Barrios[x].CreateBounds();
            }

            
            rdr.Close();
            op.Close();
        }


        private MeshComponentPart ReadLotes(ref string name) {
            MemoryStream op = CompressionHelper.DecompressFile(name); //new FileStream(name, FileMode.Open);
            BinaryReader rdr = new BinaryReader(op);


            byte pos = rdr.ReadByte();
            string cityName = rdr.ReadString();
            string author = rdr.ReadString();
            string date = rdr.ReadString();

            List<Lote> lotes = new List<Lote>();
            int featuresLenght = rdr.ReadInt32();


            for (int fl = 0; fl < featuresLenght; fl++){
                string codigo = rdr.ReadString();
                Lote lot = new Lote();
                lot.Codigo = codigo;
                lotes.Add(lot);
            }
            int vxlenght = rdr.ReadInt32();

            Vector3[] Positions = new Vector3[vxlenght];
            for (int i = 0; i < vxlenght; i++) {
                float pX = rdr.ReadSingle();
                float pY = rdr.ReadSingle();
                float pZ = rdr.ReadSingle();

                Vector3 Position = new Vector3(pZ, pY, pX);
                Positions[i] = Position;
            }

            int xxxxxx = rdr.ReadInt32();
            int[] indices = new int[xxxxxx];
            for (int i = 0; i < xxxxxx; i++) {
                indices[i] = rdr.ReadInt32();
            }


            int xxx = rdr.ReadInt32();
            Vector3[] Normals = null;

            if (xxx != 0) {
                Normals = new Vector3[vxlenght];
                for (int i = 0; i < vxlenght; i++) {
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

            int xxxx = rdr.ReadInt32();
            Vector2[] TextureCoordinates = null;

            if (xxxx != 0) {
                TextureCoordinates = new Vector2[vxlenght];

                for (int i = 0; i < vxlenght; i++) {
                    float tX = rdr.ReadSingle();
                    float tY = rdr.ReadSingle();
                    Vector2 TextureCoordinate = new Vector2(
                        tX,
                        tY
                        );

                    TextureCoordinates[i] = TextureCoordinate;
                }
            }

            Color[] Colors = new Color[vxlenght];
            int xxxxx = rdr.ReadInt32();
            for (int i = 0; i < vxlenght; i++) {
                byte b1 = rdr.ReadByte();
                byte b2 = rdr.ReadByte();
                byte b3 = rdr.ReadByte();

                Colors[i] = new Color(b1, b2, b3);
            }

            MeshComponentPart mcp = new MeshComponentPart(Path.GetFileNameWithoutExtension(name), Positions, Colors, Normals, TextureCoordinates, indices);
            mcp.Color = LayerHelper.GetRandomColor();

            return mcp;
        }



        private List<Manzana> ReadManzanas(ref string name) {
            MemoryStream op = CompressionHelper.DecompressFile(name); //new FileStream(name, FileMode.Open);
            BinaryReader rdr = new BinaryReader(op);


            byte pos = rdr.ReadByte();
            string cityName = rdr.ReadString();
            string author = rdr.ReadString();
            string date = rdr.ReadString();

            List<Manzana> manzanas = new List<Manzana>();
            int featuresLenght = rdr.ReadInt32();


            for (int fl = 0; fl < featuresLenght; fl++) {

                string codigo = rdr.ReadString();
                int vxstart = rdr.ReadInt32();
                int vxend = rdr.ReadInt32();
                
                Manzana manzana = new Manzana();

                manzana.Name = codigo;
                manzana.VertexStart = vxstart;
                manzana.VertexEnd = vxend;


                //manzana.Silueta = mcp;
                manzanas.Add(manzana);

            }

            int vxlenght = rdr.ReadInt32();
            Vector3[] Positions = new Vector3[vxlenght];
            for (int i = 0; i < vxlenght; i++) {
                float pX = rdr.ReadSingle();
                float pY = rdr.ReadSingle();
                float pZ = rdr.ReadSingle();

                Vector3 Position = new Vector3(pZ, pY, pX);
                Positions[i] = Position;
            }

            int xxxxxx = rdr.ReadInt32();
            int[] indices = new int[xxxxxx];
            for (int i = 0; i < xxxxxx; i++) {
                indices[i] = rdr.ReadInt32();
            }


            int xxx = rdr.ReadInt32();
            Vector3[] Normals = null;

            if (xxx != 0) {
                Normals = new Vector3[vxlenght];
                for (int i = 0; i < vxlenght; i++) {
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

            int xxxx = rdr.ReadInt32();
            Vector2[] TextureCoordinates = null;

            if (xxxx != 0) {
                TextureCoordinates = new Vector2[vxlenght];

                for (int i = 0; i < vxlenght; i++) {
                    float tX = rdr.ReadSingle();
                    float tY = rdr.ReadSingle();
                    Vector2 TextureCoordinate = new Vector2(
                        tX,
                        tY
                        );

                    TextureCoordinates[i] = TextureCoordinate;
                }
            }

            Color[] Colors = new Color[vxlenght];
            int xxxxx = rdr.ReadInt32();
            for (int i = 0; i < vxlenght; i++) {
                byte b1 = rdr.ReadByte();
                byte b2 = rdr.ReadByte();
                byte b3 = rdr.ReadByte();

                Colors[i] = new Color(b1, b2, b3);
            }



            MeshComponentPart manzanasMesh = new MeshComponentPart(Path.GetFileNameWithoutExtension(name), Positions, Colors, Normals, TextureCoordinates, indices);
            manzanasMesh.Color = LayerHelper.GetRandomColor();
            
            mz = new MeshComponent(manzanasMesh.Name);
            mz.MeshParts.Add(manzanasMesh);
            mz.CreateBounds();

            return manzanas;
        }

        private MeshComponent mz; 
        public StreetManager DeserializeIntersections(string path) {
            //string[] Layers = DeserializeIndex(path);

            StreetManager ic = new StreetManager();
            ic.Name = Path.GetFileNameWithoutExtension(path);
            string pt3 = Path.GetDirectoryName(path);
            //for (int i = 0; i < Layers.Length; i++) {


            MemoryStream op = CompressionHelper.DecompressFile(pt3 + "\\" + ic.Name + ".bin"); //new FileStream(name, FileMode.Open);
            BinaryReader rdr = new BinaryReader(op);
            byte typ = rdr.ReadByte();
            string name = rdr.ReadString();
            string Author = rdr.ReadString();
            string DateCreation = rdr.ReadString();

            ic.Intersections = ReadIntersections(ref rdr, ic);


            // ic.Calles = ReadIntersections(ref rdr);
            //Constants.StreetManager = ic;
            return ic;
        }


        public RenderableComponent Deserialize(string path) {
            return null;
        }

        private Dictionary<int, List<int>> ReadIntersections(ref BinaryReader rdr, StreetManager ic) {
            int namescount = rdr.ReadInt32();
            List<string> names = new List<string>();
            for (int i = 0; i < namescount; i++) {
                names.Add(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(rdr.ReadString().ToLower()));
            }

            ic.CalleNames = names;


            int ptcount = rdr.ReadInt32();


            Dictionary<int, List<int>> intersections = new Dictionary<int, List<int>>();

            List<int> ints = new List<int>();
            for (int i = 0; i < ptcount; i++) {
                ints = new List<int>();

                int featureID = rdr.ReadInt32();

                int intcount = rdr.ReadInt32();
                for (int c = 0; c < intcount; c++) {
                    ints.Add(rdr.ReadInt32());
                }
                intersections.Add(featureID, ints);
            }

            List<int> ids = new List<int>();
            for (int i = 0; i < intersections.Count; i++) {
                ids.Add(i);
            }
            ic.CalleIds = ids;

            return intersections;
        }



        private List<PointComponent> ReadPoints(ref string name) {
            FileStream op = new FileStream(name, FileMode.Open);
            BinaryReader rdr = new BinaryReader(op);


            byte pos = rdr.ReadByte();
            string cityName = rdr.ReadString();
            string author = rdr.ReadString();
            string date = rdr.ReadString();


            int ptcount = rdr.ReadInt32();
            List<PointComponent> pnts = new List<PointComponent>(ptcount);

            for (int i = 0; i < ptcount; i++) {
                PointComponent pt = new PointComponent();
                float pX = rdr.ReadSingle();
                float pY = rdr.ReadSingle();
                float pZ = rdr.ReadSingle();
                pt.Position = new Vector3(pZ, pY, pX);
                pt.TextureIndex = rdr.ReadInt32();
                pt.Name = rdr.ReadString();
                pt.CreateBounds();
                pnts.Add(pt);
            }
            return pnts;
        }


        private List<CalleComponent> ReadStreets(ref string name) {
            MemoryStream op = CompressionHelper.DecompressFile(name); //new FileStream(name, FileMode.Open);
            BinaryReader rdr = new BinaryReader(op);


            byte pos = rdr.ReadByte();
            string cityName = rdr.ReadString();
            string author = rdr.ReadString();
            string date = rdr.ReadString();


            int calles_count = rdr.ReadInt32();
            List<CalleComponent> calles = new List<CalleComponent>(calles_count);

            for (int i = 0; i < calles_count; i++) {
                CalleComponent pt = new CalleComponent();
                byte soycalle = rdr.ReadByte();

                int calleid = rdr.ReadInt32();

                pt.ID = calleid;

                int cant = rdr.ReadInt32();

                List<CalleTramoComponent> tramos = new List<CalleTramoComponent>(cant);
                //BinaryWriter.Write(LayerFields.StreetFeature);
                //BinaryWriter.Write(Features[f].OrigLine.Count);
                for (int t = 0; t < cant; t++) {
                    CalleTramoComponent ct = new CalleTramoComponent();
                    int cantpnt = rdr.ReadInt32();

                    ct.Puntos = new List<Vector3>(cantpnt);
                    for (int xx = 0; xx < cantpnt; xx++) {
                        float fx = rdr.ReadSingle();
                        float fy = rdr.ReadSingle();
                        float fz = rdr.ReadSingle();

                        ct.Puntos.Add(new Vector3(
                                          fz,
                                          fy,
                                          fx
                                          ));
                    }
                    ct.Init();
                    ct.CreateBounds();
                    tramos.Add(ct);
                }

                pt.Tramos = tramos;
                pt.Init();
                calles.Add(pt);
            }

            byte cc = rdr.ReadByte();
            int cantcols = rdr.ReadInt32();
            for (int xx = 0; xx < calles.Count; xx++) {
                for (int c = 0; c < cantcols; c++) {
                    calles[xx].Data.Add(rdr.ReadString());
                }
            }
            

            for (int xx = 0; xx < calles.Count; xx++) {

                calles[xx].Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(calles[xx].Data[0].ToLower());
                Color clr = Color.LimeGreen;
                bool Avenida = false;
                if (calles[xx].Name.Contains("Avda") || calles[xx].Name.Contains("Avenida") ||
                    calles[xx].Name.Contains("Av.") || calles[xx].Name.Contains("Av ") ||
                    calles[xx].Name.Contains("Autopist")) {
                    clr = Color.HotPink;
                    Avenida = true;
                }

                calles[xx].Color = clr;
                for (int tt = 0; tt < calles[xx].Tramos.Count; tt++) {
                    calles[xx].Tramos[tt].Name = calles[xx].Name;
                    calles[xx].Tramos[tt].Avenida = Avenida;
                    

                }
                
                calles[xx].CreateBounds();
            }

            return calles;
        }

        private MeshComponent ReadBarrios(ref string name) {
            MemoryStream op = CompressionHelper.DecompressFile(name); //new FileStream(name, FileMode.Open);
            BinaryReader rdr = new BinaryReader(op);


            byte pos = rdr.ReadByte();
            string cityName = rdr.ReadString();
            string author = rdr.ReadString();
            string date = rdr.ReadString();

 
            int vxlenght = rdr.ReadInt32();


            //int vxlenght = rdr.ReadInt32();
            Vector3[] Positions = new Vector3[vxlenght];
            for (int i = 0; i < vxlenght; i++) {
                float pX = rdr.ReadSingle();
                float pY = rdr.ReadSingle();
                float pZ = rdr.ReadSingle();
                Positions[i] = new Vector3(pZ,pY,pX);
            }

            byte ix = rdr.ReadByte();
            int xxxxxx = rdr.ReadInt32();
            int[] indices = new int[xxxxxx];
            for (int i = 0; i < xxxxxx; i++) {
                indices[i] = rdr.ReadInt32();
            }


            byte nr = rdr.ReadByte();
            int xxx = rdr.ReadInt32();
            Vector3[] Normals = null;

            if (xxx != 0) {
                Normals = new Vector3[vxlenght];
                for (int i = 0; i < vxlenght; i++) {
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

            if (xxxx != 0) {
                TextureCoordinates = new Vector2[vxlenght];

                for (int i = 0; i < vxlenght; i++) {
                    float tX = rdr.ReadSingle();
                    float tY = rdr.ReadSingle();
                    Vector2 TextureCoordinate = new Vector2(
                        tX,
                        tY
                        );

                    TextureCoordinates[i] = TextureCoordinate;
                }
            }

            Color[] Colors = new Color[vxlenght];
            byte cl = rdr.ReadByte();
            int xxxxx = rdr.ReadInt32();
            for (int i = 0; i < vxlenght; i++) {
                byte b1 = rdr.ReadByte();
                byte b2 = rdr.ReadByte();
                byte b3 = rdr.ReadByte();

                Colors[i] = new Color(b1, b2, b3);
            }

            MeshComponentPart mcp;
            mcp = new MeshComponentPart(Path.GetFileNameWithoutExtension(name), Positions, Colors, Normals, TextureCoordinates, indices);
            mcp.Color = LayerHelper.GetRandomColor();

            MeshComponent mc = new MeshComponent(Path.GetFileNameWithoutExtension(name));
            mc.MeshParts.Add(mcp);
            return mc;
        }

        private MeshComponentPart ReadMesh(string name) {
            MemoryStream op = CompressionHelper.DecompressFile(name); //new FileStream(name, FileMode.Open);
            BinaryReader rdr = new BinaryReader(op);


            byte pos = rdr.ReadByte();
            string cityName = rdr.ReadString();
            string author = rdr.ReadString();
            string date = rdr.ReadString();

            int vxstart = rdr.ReadInt32();
            int vxend = rdr.ReadInt32();
            int vxlenght = vxend - vxstart;

            Vector3[] Positions = new Vector3[vxlenght];
            Color[] Colors = new Color[vxlenght];

            for (int i = 0; i < vxlenght; i++) {
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
            byte ix = rdr.ReadByte();
            int xxxxxx = rdr.ReadInt32();
            int[] indices = new int[xxxxxx];
            for (int i = 0; i < xxxxxx; i++) {
                indices[i] = rdr.ReadInt32();
            }

            byte nr = rdr.ReadByte();
            int xxx = rdr.ReadInt32();
            Vector3[] Normals = null;

            if (xxx != 0) {
                Normals = new Vector3[vxlenght];
                for (int i = 0; i < vxlenght; i++) {
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

            if (xxxx != 0) {
                TextureCoordinates = new Vector2[vxlenght];

                for (int i = 0; i < vxlenght; i++) {
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
            for (int i = 0; i < vxlenght; i++) {
                byte b1 = rdr.ReadByte();
                byte b2 = rdr.ReadByte();
                byte b3 = rdr.ReadByte();

                Colors[i] = new Color(b1, b2, b3);
            }



            MeshComponentPart mcp;
            mcp = new MeshComponentPart(Path.GetFileNameWithoutExtension(name), Positions, Colors, Normals, TextureCoordinates, indices);
            mcp.Color = LayerHelper.GetRandomColor();
            return mcp;
        }

        internal List<MeshComponentPart> DeserializeExtra(string p) {
            List<MeshComponentPart> Meshes = new List<MeshComponentPart>();

            //for (int i = 0; i < Layers.Length; i++) {
            FileStream op = new FileStream(p, FileMode.Open);

            BinaryReader rdr = new BinaryReader(op);
            byte typ = rdr.ReadByte();
            string name = rdr.ReadString();
            string Author = rdr.ReadString();
            string DateCreation = rdr.ReadString();
            int cant = rdr.ReadInt32();
            string path = Path.GetDirectoryName(p);

            for(int i = 0; i < cant;i++){
                string extramesh = rdr.ReadString();
                if(File.Exists(path + "\\Extra\\" + extramesh + ".bin")){
                    MeshComponentPart mc = ReadMesh(path + "\\Extra\\" + extramesh + ".bin");

                    Meshes.Add(mc);
                }
            }
            return Meshes;
        }
    }

}
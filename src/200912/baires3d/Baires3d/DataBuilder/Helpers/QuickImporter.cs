using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace b3d
{
    public static class QuickImporter
    {
        private static FileStream op;
        private static BinaryReader rdr;


        public static PositionNormalTextureColor[] DeserializeOBJ(string fileName)
        {
            op = new FileStream(fileName, FileMode.Open);

            rdr = new BinaryReader(op);
            byte typ = rdr.ReadByte();
            string name = rdr.ReadString();
            string Author = rdr.ReadString();
            string DateCreation = rdr.ReadString();


            byte pos = rdr.ReadByte();
            int vxlenght = rdr.ReadInt32();

            Vector3[] Positions = new Vector3[vxlenght];
            Color[] Colors = new Color[vxlenght];

            for (int i = 0; i < vxlenght; i++)
            {
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

            if (xxx != 0)
            {
                Normals = new Vector3[vxlenght];
                for (int i = 0; i < vxlenght; i++)
                {
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

            if (xxxx != 0)
            {
                TextureCoordinates = new Vector2[vxlenght];

                for (int i = 0; i < vxlenght; i++)
                {
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
            for (int i = 0; i < vxlenght; i++)
            {
                Colors[i] = new Color(rdr.ReadByte(), rdr.ReadByte(), rdr.ReadByte());
            }


            byte ix = rdr.ReadByte();
            int xxxxxx = rdr.ReadInt32();
            int[] indices = new int[xxxxxx];
            for (int i = 0; i < xxxxxx; i++)
            {
                indices[i] = rdr.ReadInt32();
            }


            PositionNormalTextureColor[] vxs = new PositionNormalTextureColor[Positions.Length];

            for (int i = 0; i < vxs.Length; i++)
            {
                vxs[i] = new PositionNormalTextureColor();
                vxs[i].Position = Positions[i];
                vxs[i].Normal = Normals[i];
                vxs[i].Color = Colors[i];
                vxs[i].TextureCoordinate = TextureCoordinates[i];
            }
            return vxs;
        }
    }
}
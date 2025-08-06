using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace b3d
{
    public static class QuickSerializer
    {
        private static FileStream op;
        private static BinaryReader rdr;

        public static void SerializeTerrainMesh(string fileName, Vector3[] Vertices)
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter BinaryWriter = new BinaryWriter(memoryStream);
            string filePath = Constants.DataPath + "\\dumps\\" + fileName;

            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            FileStream fs = new FileStream(filePath, FileMode.Create);

            BinaryWriter.Write(Vertices.Length);
            for (int xx = 0; xx < Vertices.Length; xx++)
            {
                BinaryWriter.Write(Vertices[xx].X);
                BinaryWriter.Write(Vertices[xx].Y);
                BinaryWriter.Write(Vertices[xx].Z);
            }

            BinaryWriter.Flush();
            fs.Write(memoryStream.GetBuffer(), 0, Convert.ToInt32(memoryStream.Position));

            BinaryWriter.BaseStream.Close();
            GC.Collect();

            fs.Flush();
            fs.Close();
            memoryStream.Close();
            BinaryWriter.Close();
        }

        public static Vector3[] DeserializeTerrainMesh(string fileName)
        {
            op = new FileStream("Data\\dumps\\" + fileName, FileMode.Open);

            rdr = new BinaryReader(op);

            int vxlenght = rdr.ReadInt32();

            Vector3[] Positions = new Vector3[vxlenght];


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

            return Positions;
        }


    }
}
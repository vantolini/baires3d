using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace b3d
{
    public static class QuickExporter
    {

        public static void SerializeOBJ(string fileName, Vector3[] Vertices, Vector3[] Normals,
                                        Vector2[] TextureCoordinates)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            TextWriter tw = new StreamWriter(fileName);

            for (int xx = 0; xx < Vertices.Length; xx++)
            {
                tw.WriteLine("v " + Vertices[xx].X.ToString().Replace(",", ".") + " " +
                             Vertices[xx].Y.ToString().Replace(",", ".") + " " +
                             Vertices[xx].Z.ToString().Replace(",", "."));
            }

            for (int xx = 0; xx < Normals.Length; xx++)
            {
                tw.WriteLine("vn " + Normals[xx].X.ToString().Replace(",", ".") + " " +
                             Normals[xx].Y.ToString().Replace(",", ".") + " " +
                             Normals[xx].Z.ToString().Replace(",", "."));
            }


            for (int xx = 0; xx < TextureCoordinates.Length; xx++)
            {
                tw.WriteLine("vt " + TextureCoordinates[xx].X.ToString().Replace(",", ".") + " " +
                             TextureCoordinates[xx].Y.ToString().Replace(",", "."));
            }

            //tw.Flush();

            for (int xx = 0; xx < Vertices.Length - 2; xx += 3)
            {
                //Console.WriteLine("f " + (xx + 1) + " " + (xx + 2) + " " + (xx + 3));
                //Console.WriteLine("v " + Vertices[xx].X.ToString().Replace(",", ".") + " " + Vertices[xx].Y.ToString().Replace(",", ".") + " " + Vertices[xx].Z.ToString().Replace(",", "."));
                tw.WriteLine(
                    "f " +
                    (xx + 1) + " " + (xx + 2) + " " + (xx + 3)
                    );
            }

            tw.Flush();
            tw.Close();
        }
    }
}
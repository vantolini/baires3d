
using Builder.Exporters;
using Fbx;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using StraightSkeletonNet.Primitives;
using StraightSkeletonNet;

using System.Linq;
namespace Builder {
    class Program {
        static void AddLog(string log) {
            Console.Write(log);
        }

        public static bool EqualEpsilon(double d1, double d2) {
            return Math.Abs(d1 - d2) < 5E-6;
        }
        public static bool ContainsEpsilon(List<Vector2d> list, Vector2d p) {
            return list.Any(l => EqualEpsilon(l.X, p.X) && EqualEpsilon(l.Y, p.Y));
        }
        public static List<Vector2d> GetFacePoints(Skeleton sk) {
            List<Vector2d> ret = new List<Vector2d>();

            foreach (EdgeResult edgeOutput in sk.Edges) {
                List<Vector2d> points = edgeOutput.Polygon;
                foreach (Vector2d vector2d in points) {
                    if (!ContainsEpsilon(ret, vector2d))
                        ret.Add(vector2d);
                }
            }
            return ret;
        }

        static void Main(string[] args) {
            //ColladaExporter ce = new ColladaExporter();

            //var reader = new FbxAsciiReader(new FileStream(inputFileName, FileMode.Open));
            //var doc = reader.Read();

            //FbxIO.WriteAscii(doc, "C:\\dev\\cube2.fbx");

            var polygon = new List<Vector2d>
            {
                new Vector2d(111.9233f, 2318.726f),
                new Vector2d(111.8474f, 2319.908f),
                new Vector2d(118.332f, 2320.176f),
                new Vector2d(118.3706f, 2319.641f),
                new Vector2d(118.4465f, 2319.641f),
                new Vector2d(118.4849f, 2319.184f),
                new Vector2d(116.7681f, 2319.069f),
                new Vector2d(116.7302f, 2319.107f)
                /*
                new Vector2d(50, 50),
                new Vector2d(100, 50),
                new Vector2d(100, 100),
                
                new Vector2d(50, 100)*/
            };

           // var expected = new List<Vector2d> { new Vector2d(75.000000, 75.000000) };
           // expected.AddRange(polygon);

            var sk = SkeletonBuilder.Build(polygon);

            List<Vector2d>  aa = GetFacePoints(sk);

            //ce.Export();
            //var documentNode = FbxIO.Read(testFile);

            string name = Environment.MachineName;
            //Console.WriteLine(name);
            //return;
            Constants.r = new Random(23051983);
            //Constants.BuildPath = @"C:\dev\git\unity\Piquete\Assets\";
            BarrioBuilder[] Barrios = new BarrioBuilder[48];
            for(int x = 0; x < barrios.Length; x++)
            {
                Barrios[x] = new BarrioBuilder();
            }


            for (int xx = 0; xx < 48; xx++)
            {
                //if (barrios[xx] == "Montserrat" || barrios[xx] == "San Nicolas")
                if (barrios[xx] == "San Nicolas")
                {
                    Console.Write("Process " + barrios[xx] + "\n");
                    Barrios[xx].Start(barrios[xx]);

                }
            }


            Constants.ConnectToDB("gis");

            bool runCalles = false;

            if (runCalles)
            {
                PolylineBuilder callesBuilder = new PolylineBuilder(Constants.Connection);
                Console.WriteLine("Procesando calles...");
                callesBuilder.Process("");
            }
        }

        static string[] barrios = new[]{
            "Agronomia",
            "Almagro",
            "Balvanera",
            "Barracas",
            "Belgrano",
            "Boedo",
            "Caballito",
            "Chacarita",
            "Coghlan",
            "Colegiales",
            "Constitucion",
            "Flores",
            "Floresta",
            "La Boca",
            "Liniers",
            "Mataderos",
            "Monte Castro",
            "Montserrat",
            "Nueva Pompeya",
            "Nuñez",
            "Palermo",
            "Parque Avellaneda",
            "Parque Chacabuco",
            "Parque Chas",
            "Parque Patricios",
            "Paternal",
            "Puerto Madero",
            "Recoleta",
            "Retiro",
            "Saavedra",
            "San Cristobal",
            "San Nicolas",
            "San Telmo",
            "Velez Sarsfield",
            "Versalles",
            "Villa Crespo",
            "Villa Devoto",
            "Villa General Mitre",
            "Villa Lugano",
            "Villa Luro",
            "Villa Ortuzar",
            "Villa Pueyrredon",
            "Villa Real",
            "Villa Riachuelo",
            "Villa Santa Rita",
            "Villa Soldati",
            "Villa Urquiza",
            "Villa del Parque"
        };
    }
}

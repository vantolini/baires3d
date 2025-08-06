using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using Npgsql;
using System.Security.Cryptography;

namespace Builder {
    public static class Downloader {

        public static string DownloadString(string URL) {
            Stopwatch timer = new Stopwatch();
            StringBuilder respBody = new StringBuilder();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);


            string responseBody = respBody.ToString();
            HttpStatusCode statusCode = 0;
            float responseTime = 0;
            try {
                timer.Start();
                var response = (HttpWebResponse)request.GetResponse();
                byte[] buf = new byte[8192];
                Stream respStream = response.GetResponseStream();
                int count = 0;
                do {
                    count = respStream.Read(buf, 0, buf.Length);
                    if (count != 0)
                        respBody.Append(Encoding.ASCII.GetString(buf, 0, count));
                }
                while (count > 0);
                timer.Stop();

                responseBody = respBody.ToString();
                responseTime = timer.ElapsedMilliseconds / 1000.0f;
            }
            catch (WebException ex) {
                responseBody = "No Server Response";

            }
            return responseBody;
        }

    }


    public static class Constants {

        /*
        public static string[] GetUnique(string table, string field) {
            List<string> strings = new List<string>();
            //Constants.ConnectToDB("capas");
            NpgsqlDataReader dr =
            GetRows("SELECT DISTINCT " + field + " FROM " + table + " ORDER BY " + field + " ASC");

            while (dr.Read()) {
                strings.Add(dr[field].ToString());
            }
            dr.Close();
            //Constants.CloseDBConnection();
            return strings.ToArray();
        }
        */






        //public const string BuildPath = @"";
        //public const string BuildPath = @"E:\NEW\build\";
        //public const string BuildPath = @"E:\NEW\sdx\slimdx\samples\Direct3D9\SimpleTriangle9\bin\x86\Debug\";
        //public const string BuildPath = @"E:\NEW\b3d\src\baires3d\bin\Debug\";
        //public const string BuildPath = @"C:\uamp\www\b3d\Capas\0.8\";
        
        public static string BairesVersion = @"0.8/";
        public static string BuildPath = @"C:\dev\git\b3d\build\";
        //public static string BuildPath = @"C:\dev\git\unity\Piquete\Assets\";


        //public static string MeshLabPath = @"C:\dev\git\b3d\MeshLab\meshlabserver.exe";
        public static string MeshLabPath = @"C:\Program Files\VCG\MeshLab\meshlabserver.exe";
        

        //public static string BuildPath = @"../site/downloads/" + BairesVersion;

        //public const string BuildPath = @"E:\dev\b3d\build\";

        private const string SERVER_HOST = "127.0.0.1";
        //private const string SERVER_HOST = "192.168.219.128";

        // LAST VMWAREprivate const string SERVER_HOST = "192.168.202.130";
        //private const string SERVER_HOST = "server.baires3d.com";


        //private const string SERVER_HOST = "www.baires3d.com";
        private const string SERVER_PORT = "5432";
        private const string SERVER_USERNAME = "postgres";
        private const string SERVER_PASSWORD = "*********";
        //public static Release[] Releases;
        //public static List<Layer> ReleaseLayers;
        public static NpgsqlConnection Connection;
        public static string CurrentDb;
        public static void ConnectToDB(string db) {
            Connection = new NpgsqlConnection(
                "Server=" + SERVER_HOST +
                ";Port=" + SERVER_PORT +
                ";User Id=" + SERVER_USERNAME +
                ";Password=" + SERVER_PASSWORD +
                ";Database=" + db + ";" +
                //";Sslmode=Allow;" +
                "Preload Reader=True;" +
                "Encoding=True;"
                );
            CurrentDb = db;
            Connection.Open();
        }
        public static Random r;
        public static float NormalNext(int Steps, int MaxValue)
        {

            int count = 0;
            int val = 0;

            if (Steps < 1) return 0;

            while (++count * Steps <= MaxValue) val += r.Next(Steps);

            return val;
        }




        public static List<Feature> getLotes(NpgsqlDataReader dr)
        {
            List<Feature> features = new List<Feature>();
            while (dr.Read())
            {
                Feature feature = new Feature();
                string gid = dr[0].ToString();
                string poly = dr[1].ToString().Replace("MULTIPOLYGON(((", "").Replace(")))", "");
                poly = poly.Replace("SRID=4326;", "");
                poly = poly.Replace("POLYGON((", "").Replace("))", "");
                if (poly == "POLYGON EMPTY")
                {
                    continue;
                }

                feature.Points = Constants.parsePolygon(poly);

                float mult = 0.12f;

                string alt = dr[2].ToString();

                if (alt == "-1" || alt == "-2")
                {
                    alt = "1";

                    //feature.Altura = 0.1f;
                    feature.EspacioVerde = true;
                }
                
                if (alt == "" || alt == "0")
                {

                    alt = "1";

                } 

                feature.sr = Convert.ToSingle(alt);
                feature.FloorCount = Convert.ToInt32(alt);



                if (dr[3].ToString() == "1")
                {
                    feature.Altura = 0.02f;
                    feature.EspacioVerde = true;
                }

                feature.Seccion = dr[4].ToString();
                feature.Manzana = dr[6].ToString();
                feature.Parcela = dr[7].ToString();
                if (dr[5].ToString() == "1")
                {
                    feature.EspacioVerde = true;
                    feature.Vereda = true;
                    feature.Altura = 0.01f;
                }

                

                feature.Techo = Triangulator.Triangulate(feature.Points);

                feature.IndicesTecho = new int[feature.Techo.Count];
                for (int i = 0; i < feature.Techo.Count; i++)
                {
                    feature.Techo[i] = new Vector3(feature.Techo[i].X, feature.Techo[i].Y + feature.Altura, feature.Techo[i].Z);
                    feature.IndicesTecho[i] = i;
                }
                //feature.Techo.Reverse();
                float floorHeight = 0.31f;// Convert.ToSingle(0.01f + r.NextDouble()) / 1.6f;
                float startHeight = 0f;

                for (int f = 0; f < feature.FloorCount; f++)
                {
                    Feature floor = LayerHelper.BuildFloor(feature.Points, startHeight, floorHeight);
                    floor.FloorNumber = f;
                    feature.Floors.Add(floor);
                    startHeight += floorHeight;
                }

                if (feature.FloorCount == 1) {
                    feature.Floors[0].isSingle = true;
                }
                else
                {
                    feature.Floors[0].isFirst = true;
                    feature.Floors[feature.FloorCount - 1].isLast = true;
                }

                for (int g = 0; g < feature.Techo.Count; g++)
                {
                    Vector3 te = feature.Techo[g];
                    te.Y = startHeight;
                    feature.Techo[g] = te;
                }
                Feature roof = LayerHelper.BuildRoof(feature.Techo);
                roof.isRoof = true;
                feature.Floors.Add(roof);


                //LayerHelper.GenerateCoords(features);
                features.Add(feature);
            }

            return features;
        }


        public static List<Feature> getFeatures(NpgsqlDataReader dr, bool Flat, float Altura, bool isVerde)
        {
            List<Feature> features = new List<Feature>();
            while (dr.Read())
            {
                Feature feature = new Feature();
                string gid = dr[0].ToString();
                string poly = dr[1].ToString().Replace("MULTIPOLYGON(((", "").Replace(")))", "");
                poly = poly.Replace("SRID=4326;", "");
                poly = poly.Replace("POLYGON((", "").Replace("))", "");
                if (poly == "POLYGON EMPTY")
                {
                    continue;
                }
                int isEspacioVerde = 0;
                if (isVerde)
                {
                    string ev = dr[2].ToString();
                    if (ev == "1")
                    {
                        isEspacioVerde = 1;
                    }
                }
                feature.Points = Constants.parsePolygon(poly);

                feature.EspacioVerde = isEspacioVerde == 1 ? true : false;

                float mult = 0.12f;
                if (Altura == -1.0)
                {
                    string alt = dr[2].ToString();

                    if (alt == "-1" || alt == "-2")
                    {
                        feature.Altura = 0.1f;
                        feature.EspacioVerde = true;
                    }
                    else
                    {
                        if (alt == "" || alt == "0")
                        {
                            //Altura = ((float)r.NextDouble() + 0.01f) * 3;
                            //Altura = 1;
                            alt = "1";
                            //alt = r.Next(5, 50) + (((float)r.NextDouble() + 1f) * ((float)r.NextDouble() + 1f)).ToString();
                            //alt = (r.NextDouble() + 1f) + (((float)r.NextDouble() + 1f) * ((float)r.NextDouble() + 1f)).ToString();
                            //alt = "" + Constants.NormalNext(r.Next(10, 20), 100) / 2;
                            alt = "0.5f";
                            //alt = r.NextDouble(0.2f, 0.7f).ToString();
                            //alt = "60";
                            alt = "1";


                            feature.Altura = 0.6f;// Convert.ToSingle(alt);//Convert.ToSingle(alt) * mult;//((float)r.NextDouble() + 0.0001f) + Convert.ToSingle(alt);
                        }
                        else
                        {
                            feature.sr = Convert.ToSingle(alt);
                            feature.Altura = 0.01f + (Convert.ToSingle(alt) * mult);//((float)r.NextDouble() + 0.0001f) + Convert.ToSingle(alt);
                        }
                    }
                    if (dr[3].ToString() == "1")
                    {
                        feature.Altura = 0.02f;
                        feature.EspacioVerde = true;
                    }

                    feature.Manzana = dr[4].ToString();
                    if (dr[5].ToString()   == "1")
                    {
                        feature.EspacioVerde = true;
                        feature.Vereda = true;
                        feature.Altura = 0.01f;
                    }

                }
                else
                {
                    if (feature.EspacioVerde)
                    {
                        feature.Altura = 0.01f;
                    }
                    else
                    {
                        feature.Altura = Altura;
                    }
                }

                feature.Techo = Triangulator.Triangulate(feature.Points);

                feature.IndicesTecho = new int[feature.Techo.Count];
                for (int i = 0; i < feature.Techo.Count; i++)
                {
                    feature.Techo[i] = new Vector3(feature.Techo[i].X, feature.Techo[i].Y + feature.Altura, feature.Techo[i].Z);
                    feature.IndicesTecho[i] = i;
                }
                //feature.Techo.Reverse();

                if (Flat)
                {
                    LayerHelper.Build2d(feature);
                }
                else
                {
                    //LayerHelper.Build3d2(features, Altura);

                    LayerHelper.Build3dFeature(feature, feature.Altura);
                    LayerHelper.GenerateCoords(feature);
                }


                //LayerHelper.GenerateCoords(features);
                features.Add(feature);
                if (features.Count == 40)
                {
                    //break;
                }
            }

            return features;
        }

        public static List<Vector3> parsePolygon(string polygon)
        {
            List<Vector3> lista = new List<Vector3>();
            string[] ll = polygon.Split(',');
            for (int i = 0; i < ll.Length; i++)
            {
                string[] pp = ll[i].Split(' ');
                //Point pt = pp[0] polygon.ExteriorRing.Point(i);
                //float height = getHeight((float)pt.X, (float)pt.Y);
                System.Globalization.CultureInfo ci = System.Globalization.CultureInfo.InstalledUICulture;
                /*float[] dbl = CoordinateConverter.OriginalToConverted(
                            float.Parse(pp[0].Replace("(", "").Replace(")", ""), ci),
                            float.Parse(pp[2].Replace("(", "").Replace(")", ""), ci),
                            float.Parse(pp[1].Replace("(", "").Replace(")", ""), ci)
                    );*/

                float elev = (((float)r.NextDouble() + 1f) * 30);// this.getElevation(float.Parse(pp[0].Replace("(", "").Replace(")", ""), ci), float.Parse(pp[1].Replace("(", "").Replace(")", ""), ci));
                //System.Console.WriteLine("elev: " + elev);
                elev = 10;
                float[] dbl = CoordinateConverter.OriginalToConverted(
                            float.Parse(pp[0].Replace("(", "").Replace(")", ""), ci),
                            elev,
                            float.Parse(pp[1].Replace("(", "").Replace(")", ""), ci)
                    );


                //dbl[1] = height;
                lista.Add(new Vector3(dbl[0], dbl[1], dbl[2]));
            }

            if (lista[lista.Count - 1].X == lista[0].X && lista[lista.Count - 1].Z == lista[0].Z)
            {
                lista.RemoveAt(lista.Count - 1);
            }
            return lista;

        }
        public static NpgsqlDataReader GetRows(string query)
        {
            NpgsqlCommand command = new NpgsqlCommand(query, Constants.Connection);

            NpgsqlDataReader dr = command.ExecuteReader();
            // command.Dispose();
            return dr;
        }
        public static NpgsqlConnection GetConnection(string db) {
            NpgsqlConnection Connection2 = new NpgsqlConnection(
                "Server=" + SERVER_HOST +
                ";Port=" + SERVER_PORT +
                ";User Id=" + SERVER_USERNAME +
                ";Password=" + SERVER_PASSWORD +
                ";Database=" + db + ";" +
                ";Sslmode=Allow;" +
                "Preload Reader=True;"
                );
            //CurrentDb = db;
            Connection2.Open();
            return Connection2; 
        }
        public static void CloseDBConnection() {
            Connection.Close();
        }

        public static string GetMD5HashFile(string input) {
            using (MD5 md5 = new MD5CryptoServiceProvider()) {
                using (FileStream file = new FileStream(input, FileMode.Open)) {
                    byte[] retVal = md5.ComputeHash(file);
                    return BitConverter.ToString(retVal).Replace("-", ""); // hex string
                }
            }
        }

    }
}

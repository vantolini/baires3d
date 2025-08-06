
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Npgsql;
namespace Builder {
    public class PointBuilder
    {
        private NpgsqlConnection Connection;


        public PointBuilder(NpgsqlConnection connection)
        {
            this.Connection = connection;
        }
        public void Process(string name)
        {
            NpgsqlDataReader dr = GetRows("SELECT st_asewkt(geom), nombre_cie, altura_tot, diametro FROM arbolado_publico WHERE barrio = '" + name.ToUpper() + "'");
            NpgsqlDataReader dr2 = GetRows("SELECT st_asewkt(geom), nombre_cie, altura_tot, diametro FROM arbolado_verdes WHERE barrio = '" + name.ToUpper() + "'");
            List<Feature> features = new List<Feature>();


            while (dr.Read())
            {
                Feature feature = new Feature();

                
                string poly = dr[0].ToString().Replace("POINT(", "").Replace(")", "").Replace("SRID=4326;", "");
                string[] ll = poly.Split(',');
                string[] pp = ll[0].Split(' ');
                System.Globalization.CultureInfo ci = System.Globalization.CultureInfo.InstalledUICulture;
                float[] dbl = CoordinateConverter.OriginalToConverted(
                        float.Parse(pp[0].Replace("(", "").Replace(")", ""), ci),
                        1,
                        float.Parse(pp[1].Replace("(", "").Replace(")", ""), ci)
                );
                feature.Points = new List<Vector3>();


                feature.Points.Add(new Vector3(dbl[0], dbl[1], dbl[2]));
                if (feature.Points.Count > 0)
                {
                    feature.Name = dr[1].ToString();
                    //feature.Icon = dr[2].ToString();
                    if (feature.Icon == "")
                    {
                        feature.Icon = "cancel.png";

                    }
                    features.Add(feature);
                }
            }



            while (dr2.Read())
            {
                Feature feature = new Feature();


                string poly = dr2[0].ToString().Replace("POINT(", "").Replace(")", "").Replace("SRID=4326;", "");
                string[] ll = poly.Split(',');
                string[] pp = ll[0].Split(' ');
                System.Globalization.CultureInfo ci = System.Globalization.CultureInfo.InstalledUICulture;
                float[] dbl = CoordinateConverter.OriginalToConverted(
                        float.Parse(pp[0].Replace("(", "").Replace(")", ""), ci),
                        1,
                        float.Parse(pp[1].Replace("(", "").Replace(")", ""), ci)
                );
                feature.Points = new List<Vector3>();


                feature.Points.Add(new Vector3(dbl[0], dbl[1], dbl[2]));
                if (feature.Points.Count > 0)
                {
                    feature.Name = dr2[1].ToString();
                    //feature.Icon = dr[2].ToString();
                    if (feature.Icon == "")
                    {
                        feature.Icon = "cancel.png";

                    }
                    features.Add(feature);
                }
            }


            dr.Close();
            dr2.Close();
            if (features.Count == 0)
            {
                return;
            }
            SerializedPoints pointlayer = new SerializedPoints(new SerializedComponentInfo("Victor", "23/05/1983"));

            foreach (Feature feature in features)
            {
                SerializedPoint point = new SerializedPoint();

                //point.Icon = feature.Icon;
                //point.TextureIndex = pts.IndexOf(feature.Data[2]);
                point.Position = feature.Points[0];
                point.Name = feature.Name;
                pointlayer.Points.Add(point);
            }

            pointlayer.Name = name;
            //pointlayers.Categories.Add(pointlayer);

            CiudadWriter plw = new CiudadWriter();
            plw.SerializePoints(pointlayer, Constants.BuildPath + "Trees\\" + name + "_trees");

        }

        public void Process2(string name)
        {
            string[] uniq = getUnique("puntos3d", "tipo");
            //
            SerializedPoints pointlayers = new SerializedPoints();

            for (int i = 0; i < uniq.Length; i++)
            {
                NpgsqlDataReader dr = GetRows("SELECT st_asewkt(geom), nombre, icon FROM puntos3d WHERE tipo = '" + uniq[i] + "'");
                List<Feature> features = new List<Feature>();

                while (dr.Read())
                {
                    Feature feature = new Feature(uniq[i]);

                    //var geom = SharpMap.Converters.WellKnownText.GeometryFromWKT.Parse(dr[0].ToString());

                    string poly = dr[0].ToString().Replace("POINT(", "").Replace(")", "");
                    string[] ll = poly.Split(',');
                    string[] pp = ll[0].Split(' ');
                    System.Globalization.CultureInfo ci = System.Globalization.CultureInfo.InstalledUICulture;
                    float[] dbl = CoordinateConverter.OriginalToConverted(
                            float.Parse(pp[0].Replace("(", "").Replace(")", ""), ci),
                            float.Parse(pp[2].Replace("(", "").Replace(")", ""), ci) + 2,
                            float.Parse(pp[1].Replace("(", "").Replace(")", ""), ci)
    );
                    feature.Points = new List<Vector3>();

                    feature.Points.Add(new Vector3(dbl[0], dbl[1], dbl[2]));
                    if (feature.Points.Count > 0)
                    {
                        feature.Name = dr[1].ToString();
                        feature.Icon = dr[2].ToString();
                        if (feature.Icon == "")
                        {
                            feature.Icon = "cancel.png";

                        }
                        features.Add(feature);
                    }


                }
                dr.Close();
                if (features.Count == 0)
                {
                    return;
                }
                SerializedPoints pointlayer = new SerializedPoints(new SerializedComponentInfo("Victor", "23/05/1983"));

                foreach (Feature feature in features)
                {
                    SerializedPoint point = new SerializedPoint();

                    point.Icon = feature.Icon;
                    //point.TextureIndex = pts.IndexOf(feature.Data[2]);
                    point.Position = feature.Points[0];
                    point.Name = feature.Name;
                    pointlayer.Points.Add(point);
                }

                pointlayer.Name = uniq[i];
                pointlayers.Categories.Add(pointlayer);

            }

            CiudadWriter plw = new CiudadWriter();
            plw.SerializePoints(pointlayers, Constants.BuildPath + "Points");
        }

        private NpgsqlDataReader GetRows(string query)
        {
            NpgsqlCommand command = new NpgsqlCommand(query, Connection);

            NpgsqlDataReader dr = command.ExecuteReader();
            command.Dispose();
            return dr;
        }


        public string[] getUnique(string table, string field) {
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
    }
}
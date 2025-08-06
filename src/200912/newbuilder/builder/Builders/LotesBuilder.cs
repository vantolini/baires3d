using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder
{
    class LotesBuilder
    {
        string query = "";
        NpgsqlDataReader dr = null;

        internal List<Feature> Process(string name)
        {


            Color clr = LayerHelper.GetRandomColor();

            query = "SELECT gid, st_asewkt(st_geometryn(geom, 1)), sr as alturita, espacio_ve as verde FROM parcelas WHERE barrios = '" + name.ToUpper() + "' LIMIT 10";
            query = "SELECT gid, st_asewkt(st_geometryn(geom, 1)), sr as alturita, espacio_ve as verde FROM parcelas WHERE barrios = '" + name.ToUpper() + "'";
            query = "SELECT gid, st_asewkt(ST_SimplifyPreserveTopology(st_geometryn(geom, 1), 0.000003)), sr as alturita, espacio_ve as verde, seccion, vereda, manzana,  parcela FROM parcelas WHERE barrios = '" + name.ToUpper() + "'";
            query += " AND vereda = 0";
            query += " AND unity = 1";
            //query += " LIMIT 50";
            query += " LIMIT 1";
            dr = Constants.GetRows(query);
            //LotesBuilder lb = new LotesBuilder();
            //lb.Process(dr);
            List<Feature> lotesFeatures = Constants.getLotes(dr);
            dr.Close();

            Random r = new Random(2311);

            List<Color> clrs = new List<Color>();

            List<Color> clrs2 = new List<Color>();

            if (lotesFeatures != null)
            {
                int texCount = 20;
                for (int i = 0; i <= texCount; i++)
                {
                    Color cl = LayerHelper.GetRandomColor();
                    if (!clrs.Contains(cl))
                    {
                        clrs.Add(cl);
                    }
                }


                Color DarkGray = Color.DarkGray;

                clrs2.Add(Color.Gray);
                clrs2.Add(Color.DarkGray);
                //clrs2.Add(Color.LightGray);

                for (int f = 0; f < lotesFeatures.Count; f++)
                {
                    //break;
                    //clr = new Color((byte)val, (byte)val, (byte)val);
                    clr = clrs[r.Next(0, clrs.Count)];


                // clr = LayerHelper.GetRandomColor();
                    lotesFeatures[f].Color = clr;
                    lotesFeatures[f].Texture = r.Next(0, texCount);

                    for (int ff = 0; ff < lotesFeatures[f].Floors.Count; ff++)
                    {
                        if (lotesFeatures[f].Floors[ff].isRoof)
                        {
                            int roo = r.Next(0, clrs2.Count);
                            Color clrRoof = clrs2[roo];

                            lotesFeatures[f].Floors[ff].Color = clrRoof;
                            lotesFeatures[f].Floors[ff].Texture = roo;
                            for (int i = 0; i < lotesFeatures[f].Floors[ff].Vertices.Length; i++)
                            {
                                lotesFeatures[f].Floors[ff].Vertices[i].Color = clrRoof;
                            }
                        }
                        else{

                            lotesFeatures[f].Floors[ff].Color = clr;
                            for (int i = 0; i < lotesFeatures[f].Floors[ff].Vertices.Length; i++)
                            {
                                lotesFeatures[f].Floors[ff].Vertices[i].Color = clr;
                            }
                        }
                        
                    }
                }

                /*
                 
             

                for (int f = 0; f < lotesFeatures.Count; f++)
                {
                    //break;
                    int val = r.Next(100, 180);
                    //clr = new Color((byte)val, (byte)val, (byte)val);
                    clr = clrs[r.Next(0, clrs.Count - 1)];
                    // clr = LayerHelper.GetRandomColor();
                    lotesFeatures[f].Color = clr;
                    lotesFeatures[f].Texture = r.Next(0, texCount);

                    if (lotesFeatures[f].Vereda)
                    {
                        lotesFeatures[f].Color = DarkGray;
                        for (int i = 0; i < lotesFeatures[f].Vertices.Length; i++)
                        {
                            lotesFeatures[f].Vertices[i].Color = DarkGray;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < lotesFeatures[f].Vertices.Length; i++)
                        {
                            lotesFeatures[f].Vertices[i].Color = clr;
                        }
                    }
                }
             */
            }

            return lotesFeatures;
        }




        private List<Feature> getFeat12ures(NpgsqlDataReader dr, bool Flat, float Altura, bool isVerde)
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

                }
                else
                {
                    if (feature.EspacioVerde)
                    {
                        feature.Altura = 0.1f;
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



    }
}

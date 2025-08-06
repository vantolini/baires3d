using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql;
using Poly2Tri;
using SharpMap.Geometries;
using Polygon=Poly2Tri.Polygon;

namespace Builder {

    public class PolygonBuilder {
        public float Altura;
        public bool Flat;
        private float getHeight(List<Vector3> points, float X, float Z) {
            for (int i = 0; i < points.Count; i++) {
                if (points[i].X == X && points[i].Z == Z) {
                    return points[i].Y;
                }
            }
            return 0;
        }
        
        public string LayerType = "";
        public string AlturaField = "";

        public string TableName = "";


        public void ProcessSubte(string linea)
        {
            string query = "SELECT gid, asewkt(st_buffer(the_geom, 0.00010)) FROM subte_lineas WHERE linea = '" + linea + "'";


            //query = "SELECT gid, asewkt(the_geom), sr as alturita FROM lotes_fin WHERE barrio = '" + name + "'";
           // dr = DB.GetRows(query);


            if (Altura == -1)
            {
                //query = "SELECT gid, asewkt(the_geom), " + AlturaField + " FROM " + TableName + " WHERE " + FieldName + " = '" + uniq[i] + "'";
            }
            else {
                Altura = Altura += 0.03f;
            }
            NpgsqlDataReader dr = DB.GetRows(query);
            List<Feature> features = new List<Feature>();
            features = getFeatures(dr, false, Altura, false);
            dr.Close();

            if (features.Count == 0)
            {
                return;
            }

            if (Flat)
            {
                LayerHelper.Build2d(features);
                //LayerHelper.Build3d2(features, -1);
            }
            else
            {
                LayerHelper.Build3d2(features, Altura);
                LayerHelper.GenerateCoords(features);

            }


            Color CurrentColor = new Color(255, 204, 51);// Color.Orange;
            switch(linea){
                case "A":
                    CurrentColor = Color.LightBlue;
                    break;
                case "B":
                    CurrentColor = Color.Red;
                    break;
                case "C":
                    CurrentColor = Color.Blue;
                    break;
                case "D":
                    CurrentColor = Color.Green;
                    break;
                case "E":
                    CurrentColor = Color.Purple;
                    break;
                case "H":
                    CurrentColor = Color.Yellow;
                    break;
                case "P":
                    CurrentColor = Color.Magenta;
                    break;
                default:
                    break;
            }
            features[0].Color = CurrentColor;
                NpgsqlDataReader dr2 = DB.GetRows("SELECT asewkt(the_geom), estacion FROM subte_estaciones WHERE linea = '" + linea + "'");
                List<Feature> estacionesFeatures = new List<Feature>();

                while (dr2.Read()) {
                    Feature feature = new Feature(dr2[1].ToString());

                    string pnt = dr2[0].ToString().Replace("POINT(", "").Replace(")", "");
                    string[] ex = pnt.Split(',');

                    feature.Points = new List<Vector3>();
                    for (int e = 0; e < ex.Length; e++) {
                        if(ex[e] == ""){
                            continue;
                        }
                        string[] pol = ex[e].Split(' ');

                        
                        float[] dbl = CoordinateConverter.OriginalToConverted(Convert.ToDouble(pol[0]), 0, Convert.ToDouble(pol[1]));


                        dbl[1] = 0;
                        feature.Points.Add(new Vector3(dbl[0], dbl[1], dbl[2]));



                    }
                    if (feature.Points.Count > 0){
                        //feature.Name = dr2[1].ToString();
                        estacionesFeatures.Add(feature);
                    }


                }
                dr2.Close();

            CiudadWriter meshWriter = new CiudadWriter();

            meshWriter.SerializeSubte("Subte_" + linea, Constants.BuildPath, features, estacionesFeatures);
        }


        private List<Vector3> parsePolygon(SharpMap.Geometries.Polygon polygon) {
            List<Vector3> lista = new List<Vector3>();
            for (int i = 0; i < polygon.ExteriorRing.NumPoints; i++ ){
                Point pt = polygon.ExteriorRing.Point(i);
                float[] dbl = CoordinateConverter.OriginalToConverted(pt.X, 0, pt.Y);

                lista.Add(new Vector3(dbl[0], dbl[1], dbl[2]));
            }
            if (lista[lista.Count - 1].X == lista[0].X && lista[lista.Count - 1].Z == lista[0].Z) {
                lista.RemoveAt(lista.Count - 1);
            }
            return lista;

        }
        private List<Feature> getFeatures(NpgsqlDataReader dr, bool Flat, float Altura, bool isVerde) {
            Random r = new Random(666);
            List<Feature> features = new List<Feature>();
            while (dr.Read()) {
                Feature feature = new Feature();
                string gid = dr[0].ToString();
                string poly = dr[1].ToString().Replace("MULTIPOLYGON(((", "").Replace(")))", "");

                int isEspacioVerde = 0;
                if(isVerde){
                    isEspacioVerde  = (int) dr[2];
                }
            

                var geom = SharpMap.Converters.WellKnownText.GeometryFromWKT.Parse(dr[1].ToString());
                if (geom is MultiPolygon) {
                    var geom2 = (SharpMap.Geometries.Polygon)geom;
                    feature.Points = parsePolygon(geom2);

                }
                else {
                    if (geom is SharpMap.Geometries.Polygon) {

                        feature.Points = parsePolygon((SharpMap.Geometries.Polygon)geom);

                    }
                    else {
                        throw new Exception();
                    }
                }
               // if (geom.NumInteriorRing > 0) {
                    //return null;
                //}

       

                feature.EspacioVerde = isEspacioVerde == 1 ? true : false;

                if (Altura == -1.0) {
                    string alt = dr[2].ToString();
                    if (alt == "" || alt == "0") {
                        //Altura = ((float)r.NextDouble() + 0.01f) * 3;
                        //Altura = 1;
                        alt = "1";
                    }

                    feature.Altura = ((float)r.NextDouble() + 0.001f) +  Convert.ToSingle(alt);
                    if (dr[2].ToString() == "1"){
                        feature.Altura = 0.2f;
                        feature.EspacioVerde = true;
                    }

                        //Altura = Convert.ToSingle(alt);
                    
                }
                else {
                    if (feature.EspacioVerde) {
                        feature.Altura = 0.1f;
                    }
                    else {
                        feature.Altura = Altura;
                    }
                }

                feature.Techo = Triangulator.Triangulate(feature.Points);
                feature.IndicesTecho = new int[feature.Techo.Count];
                for (int i = 0; i < feature.Techo.Count; i++) {
                    feature.Techo[i] = new Vector3(feature.Techo[i].X, feature.Altura, feature.Techo[i].Z);
                    feature.IndicesTecho[i] = i;
                }
                //feature.Techo.Reverse();
                
                if (Flat) {
                    LayerHelper.Build2d(feature);
                } else {
                    //LayerHelper.Build3d2(features, Altura);
                    
                    LayerHelper.Build3dFeature(feature, feature.Altura);
                    LayerHelper.GenerateCoords(feature);
                }

                
                //LayerHelper.GenerateCoords(features);
                features.Add(feature);
            }

            return features;
        }


        public void ProcessCiudadNew(string name) {
            string query = "";
            float ManzanaAltura = 0.3f;
            float VeredaAltura = 0.04f;
            float CordonAltura = 0.2f;
            float SiluetaAltura = 2.5f;
            float VerdeAltura = 0.2f;

            List<Feature> manzanaFeatures = new List<Feature>();
            List<Feature> veredasFeatures = new List<Feature>();
            List<Feature> cordonesFeatures = new List<Feature>();
            List<Feature> siluetaFeatures = new List<Feature>();
            List<Feature> lotesFeatures = new List<Feature>();
            List<Feature> verdesFeatures = new List<Feature>();

            query = "SELECT gid, asewkt(the_geom), espacio_verde FROM manzanas WHERE barrio = '" + name + "'";
            NpgsqlDataReader dr = DB.GetRows(query);
            manzanaFeatures = getFeatures(dr, false, ManzanaAltura, true);
            dr.Close();


            for (int f = 0; f < manzanaFeatures.Count; f++) {
                for (int i = 0; i < manzanaFeatures[f].Vertices.Length; i++) {
                    manzanaFeatures[f].Vertices[i].Color = new Color(255, 200, 0);
                }
            }


            query = "SELECT gid, asewkt(the_geom) FROM veredas WHERE barrio = '" + name + "'";
            dr = DB.GetRows(query);
            veredasFeatures = getFeatures(dr, true, VeredaAltura, false);
            dr.Close();

            for (int f = 0; f < veredasFeatures.Count; f++) {
                for (int i = 0; i < veredasFeatures[f].Vertices.Length; i++) {
                    veredasFeatures[f].Vertices[i].Color = new Color(200, 200, 200);
                }
            }



            Color clr = LayerHelper.GetRandomColor();

            query = "SELECT gid, asewkt(st_buffer(the_geom, -0.00021)) FROM siluetas WHERE barrio = '" + name + "'";
            dr = DB.GetRows(query);
            siluetaFeatures = getFeatures(dr, true, SiluetaAltura, false);
            dr.Close();

            for (int f = 0; f < manzanaFeatures.Count; f++) {
                for (int i = 0; i < manzanaFeatures[f].Vertices.Length; i++) {
                    manzanaFeatures[f].Vertices[i].Color = new Color(255, 200, 0);
                }
            }

            for (int f = 0; f < manzanaFeatures.Count; f++) {
                /* if (manzanaFeatures[f].EspacioVerde) {
                      for (int i = 0; i < manzanaFeatures[f].Vertices.Length; i++) {
                          manzanaFeatures[f].Vertices[i].Color = new Color(0, 102, 0);
                      }
                  }
                  else {
                      for (int i = 0; i < manzanaFeatures[f].Vertices.Length; i++) {
                       //   manzanaFeatures[f].Vertices[i].Color = clr;
                      }
                  }*/
            }


            //query = "SELECT lotes.gid, asewkt(lotes.the_geom), sr as alturita FROM lotes WHERE lotes.barrio = '" + name + "'";
            //if(name.Contains("Avellaneda")){
            /*  query = "SELECT gid, asewkt(the_geom), sr as alturita FROM lotes_fin WHERE barrio = '" + name + "'";
              dr = DB.GetRows(query);
              lotesFeatures = getFeatures(dr, false, -1, false);
              dr.Close();
          */

            query = "SELECT gid, asewkt(st_simplify(the_geom, 0.000005)) FROM espacios_verdes WHERE \"BARRIO\" = '" + name + "'";
            dr = DB.GetRows(query);
            //LotesBuilder lb = new LotesBuilder();
            //lb.Process(dr);
            verdesFeatures = getVerdes(dr, VerdeAltura);
            dr.Close();

            for (int f = 0; f < verdesFeatures.Count; f++) {
                for (int i = 0; i < verdesFeatures[f].Vertices.Length; i++) {
                    verdesFeatures[f].Vertices[i].Color = new Color(0, 102, 0);
                }
            }
            if (name.Contains("Patricio")) {
                //   return;
            }
            manzanaFeatures.AddRange(verdesFeatures);

            /*
             
             182 PARQUE PAT
             */
            /*

            query = "SELECT gid, asewkt(the_geom), sr as alturita, espacio_verde FROM lotes_fin WHERE barrio = '" + name + "'";
            dr = DB.GetRows(query);
            //LotesBuilder lb = new LotesBuilder();
            //lb.Process(dr);
            lotesFeatures = getFeatures(dr, false, -1, false);
            dr.Close();

            */
            lotesFeatures = null;
            if (lotesFeatures != null) {
                for (int f = 0; f < lotesFeatures.Count; f++) {
                    if (lotesFeatures[f].EspacioVerde) {
                        for (int i = 0; i < lotesFeatures[f].Vertices.Length; i++) {
                            lotesFeatures[f].Vertices[i].Color = new Color(0, 102, 0);
                        }
                    }
                    else {
                        for (int i = 0; i < lotesFeatures[f].Vertices.Length; i++) {
                            lotesFeatures[f].Vertices[i].Color = clr;
                        }
                    }
                }
            }

            //}
            CiudadWriter meshWriter = new CiudadWriter();
            meshWriter.SerializeBarrio(
                name,
                Constants.BuildPath + @"\",
                manzanaFeatures,
                veredasFeatures,
                cordonesFeatures,
                siluetaFeatures,
                lotesFeatures
            );
        }

        public void ProcessCiudad(string name) {
            string query = "";
            float ManzanaAltura = 0.3f;
            float VeredaAltura = 0.02f;
            float CordonAltura = 0.2f;
            float SiluetaAltura = 2.5f;
            float VerdeAltura = 0.2f;

            List<Feature> manzanaFeatures = new List<Feature>();
            List<Feature> veredasFeatures = new List<Feature>();
            List<Feature> cordonesFeatures = new List<Feature>();
            List<Feature> siluetaFeatures = new List<Feature>();
            List<Feature> lotesFeatures = new List<Feature>();
            List<Feature> verdesFeatures = new List<Feature>();
            
            query = "SELECT gid, asewkt(the_geom), espacio_verde FROM manzanas WHERE barrio = '" + name + "'";
            NpgsqlDataReader dr = DB.GetRows(query);
            manzanaFeatures = getFeatures(dr, false, ManzanaAltura, true);
            dr.Close();


            for (int f = 0; f < manzanaFeatures.Count; f++) {
                for (int i = 0; i < manzanaFeatures[f].Vertices.Length; i++) {
                    manzanaFeatures[f].Vertices[i].Color = new Color(255, 200, 0);
                }
            }


            query = "SELECT gid, asewkt(the_geom) FROM veredas WHERE barrio = '" + name + "'";
            dr = DB.GetRows(query);
            veredasFeatures = getFeatures(dr, true, VeredaAltura, false);
            dr.Close();

            for (int f = 0; f < veredasFeatures.Count; f++) {
                for (int i = 0; i < veredasFeatures[f].Vertices.Length; i++) {
                    veredasFeatures[f].Vertices[i].Color = new Color(200, 200, 200);
                }
            }



            Color clr = LayerHelper.GetRandomColor();

            query = "SELECT gid, asewkt(st_buffer(the_geom, -0.00021)) FROM siluetas WHERE barrio = '" + name + "'";
            dr = DB.GetRows(query);
            siluetaFeatures = getFeatures(dr, true, SiluetaAltura, false);
            dr.Close();

            for (int f = 0; f < manzanaFeatures.Count; f++) {
                for (int i = 0; i < manzanaFeatures[f].Vertices.Length; i++) {
                    manzanaFeatures[f].Vertices[i].Color = new Color(255, 200, 0);
                }
            }

            for (int f = 0; f < manzanaFeatures.Count; f++) {
              /* if (manzanaFeatures[f].EspacioVerde) {
                    for (int i = 0; i < manzanaFeatures[f].Vertices.Length; i++) {
                        manzanaFeatures[f].Vertices[i].Color = new Color(0, 102, 0);
                    }
                }
                else {
                    for (int i = 0; i < manzanaFeatures[f].Vertices.Length; i++) {
                     //   manzanaFeatures[f].Vertices[i].Color = clr;
                    }
                }*/
            }


            //query = "SELECT lotes.gid, asewkt(lotes.the_geom), sr as alturita FROM lotes WHERE lotes.barrio = '" + name + "'";
            //if(name.Contains("Avellaneda")){
              /*  query = "SELECT gid, asewkt(the_geom), sr as alturita FROM lotes_fin WHERE barrio = '" + name + "'";
                dr = DB.GetRows(query);
                lotesFeatures = getFeatures(dr, false, -1, false);
                dr.Close();
            */

            query = "SELECT gid, asewkt(st_simplify(the_geom, 0.000005)) FROM espacios_verdes WHERE \"BARRIO\" = '" + name + "'";
            dr = DB.GetRows(query);
            //LotesBuilder lb = new LotesBuilder();
            //lb.Process(dr);
            verdesFeatures = getVerdes(dr, VerdeAltura);
            dr.Close();

            for (int f = 0; f < verdesFeatures.Count; f++) {
                for (int i = 0; i < verdesFeatures[f].Vertices.Length; i++) {
                    verdesFeatures[f].Vertices[i].Color = new Color(0, 102, 0);
                }
            }
            if(name.Contains("Patricio")){
             //   return;
            }
            manzanaFeatures.AddRange(verdesFeatures);

            /*
             
             182 PARQUE PAT
             */
            /*

            query = "SELECT gid, asewkt(the_geom), sr as alturita, espacio_verde FROM lotes_fin WHERE barrio = '" + name + "'";
            dr = DB.GetRows(query);
            //LotesBuilder lb = new LotesBuilder();
            //lb.Process(dr);
            lotesFeatures = getFeatures(dr, false, -1, false);
            dr.Close();

            */
            lotesFeatures = null;
            if (lotesFeatures != null) {
                for (int f = 0; f < lotesFeatures.Count; f++) {
                    if (lotesFeatures[f].EspacioVerde) {
                        for (int i = 0; i < lotesFeatures[f].Vertices.Length; i++) {
                            lotesFeatures[f].Vertices[i].Color = new Color(0, 102, 0);
                        }
                    }
                    else {
                        for (int i = 0; i < lotesFeatures[f].Vertices.Length; i++) {
                            lotesFeatures[f].Vertices[i].Color = clr;
                        }
                    }
                }
            }

            //}
            CiudadWriter meshWriter = new CiudadWriter();
            meshWriter.SerializeBarrio(
                name,
                Constants.BuildPath + @"\",
                manzanaFeatures,
                veredasFeatures,
                cordonesFeatures,
                siluetaFeatures,
                lotesFeatures
            );
        }

        private List<Feature> getVerdes(NpgsqlDataReader dr, float Altura) {
            Random r = new Random(666);
            List<Feature> features = new List<Feature>();
            while (dr.Read()) {
                Feature feature = new Feature();
                string gid = dr[0].ToString();

                if(dr[1].ToString() == ""){
                    Console.WriteLine(gid);
                    continue;
                }
                var geom = SharpMap.Converters.WellKnownText.GeometryFromWKT.Parse(dr[1].ToString());
                if (geom is MultiPolygon) {
                    var geom2 = (SharpMap.Geometries.Polygon)geom;
                    feature.Points = parsePolygon(geom2);
                }
                else {
                    if (geom is SharpMap.Geometries.Polygon) {
                        feature.Points = parsePolygon((SharpMap.Geometries.Polygon)geom);
                    }
                    else {
                        throw new Exception();
                    }
                }

                feature.Altura = Altura;

                feature.Techo = Triangulator.Triangulate(feature.Points);
                feature.IndicesTecho = new int[feature.Techo.Count];
                for (int i = 0; i < feature.Techo.Count; i++) {
                    feature.Techo[i] = new Vector3(feature.Techo[i].X, feature.Altura, feature.Techo[i].Z);
                    feature.IndicesTecho[i] = i;
                }
                //feature.Techo.Reverse();


                LayerHelper.Build3dFeature(feature, feature.Altura);
                LayerHelper.GenerateCoords(feature);
                for (int i = 0; i < feature.Vertices.Length; i++) {
                    feature.Vertices[i].Color = new Color(0, 102, 0);
                }
                feature.IsVerde = true;
                //LayerHelper.GenerateCoords(features);
                features.Add(feature);
            }

            return features;
        }
    }
}

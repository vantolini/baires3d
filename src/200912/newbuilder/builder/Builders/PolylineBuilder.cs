
using System;
using System.Collections.Generic;
using Npgsql;

namespace Builder {
    public class PolylineBuilder {
        public PolylineBuilder(NpgsqlConnection connection)
        {
            this.Connection = connection;
        }
        private NpgsqlConnection Connection;
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

        private double getHeight(List<Vector3> points, double X, double Z) {
            for (int i = 0; i < points.Count; i++) {
                if (points[i].X == X && points[i].Z == Z) {
                    return points[i].Y;
                }
            }
            return 0;
        }


        /*public void BuildSubte3D() {
            CiudadWriter ciudadWriter = new CiudadWriter();

            Logger.LogLine("BuildSubte3D()");
            string response = Utils.Downloader.DownloadString("http://192.168.1.102/php/server/builder/3d/custom/getsubte.php");
            if (response == "NO ES ESQUINA!") {
                //System.Windows.Forms.MessageBox.Show("NO ES ESQUINA!");
                return;

            }

            List<Feature> Lineas = new List<Feature>();
            string[] features = response.Split('\n');
            Feature feature = null;
            Feature featureEstacion = null;

            bool inEstacion = false;
            string nombre2 = "";
            string nombre_estacion = "";
            for (int ff = 0; ff < features.Length; ff++) {
                string[] ss = features[ff].Split(' ');


                if (ss[0] == "LINEA") {
                    nombre2 = "";
                    for (int nn = 1; nn < ss.Length; nn++) {
                        nombre2 += ss[nn] + " ";
                    }
                    nombre2 = nombre2.Substring(0, nombre2.Length - 1);


                    inEstacion = false;
                    if (feature != null) {
                        //feature.Points.Reverse();
                        Lineas.Add(feature);
                    }

                    feature = new Feature();
                    feature.Points = new List<Vector3>();
                    feature.Name = nombre2;
                }
                else {

                    if (ss[0] == "ESTACION") {
                        nombre_estacion = "";
                        for (int nn = 1; nn < ss.Length; nn++) {
                            nombre_estacion += ss[nn] + " ";
                        }
                        nombre_estacion = nombre_estacion.Substring(0, nombre_estacion.Length - 1);
                        inEstacion = true;
                        if (featureEstacion != null) {
                            //featureEstacion = new Feature();
                            //featureEstacion.Points = new List<Vector3>();
                            //featureEstacion.Name = ss[1];
                        }

                        featureEstacion = new Feature();
                        featureEstacion.Points = new List<Vector3>();
                        featureEstacion.Name = nombre_estacion;
                    }
                    else {
                        if (inEstacion == false) {
                            if (ss[0] == "") {
                                continue;
                            }
                            feature.Points.Add(
                                //LayerHelper.getFinalVector(
                                    Utils.CoordinateConverter.OriginalToConverted(
                                        Convert.ToDouble(ss[0].Replace('.', ',')), Convert.ToDouble(ss[2].Replace('.', ',')), Convert.ToDouble(ss[1].Replace('.', ','))
                                    )
                                //)
                            );
                        }
                        else {
                            if (ss[0] == "") {
                                continue;
                            }
                            featureEstacion.Points.Add(
                                //LayerHelper.getFinalVector(
                                    Utils.CoordinateConverter.OriginalToConverted(
                                        Convert.ToDouble(ss[0].Replace('.', ',')), Convert.ToDouble(ss[2].Replace('.', ',')), Convert.ToDouble(ss[1].Replace('.', ','))
                                    )
                                //)
                            );
                            featureEstacion.Data.Add(nombre_estacion);
                            featureEstacion.Data.Add(nombre2);

                            feature.Features.Add(featureEstacion);
                            inEstacion = false;
                        }
                    }
                }
            }



            Lineas.Add(feature);
            Logger.LogLine("writing Subte.bin");
            ciudadWriter.SerializeSubte("Subte", @"Data\Ciudades\Capital Federal\", Lineas);
            Logger.LogLine("wrote Subte.bin");

        }

        */
        /*
        public List<Feature> getEstaciones(string linea) {

            Constants.ConnectToDB("routing");
            NpgsqlDataReader dr = DB.GetRows("SELECT asewkt(the_geom), estacion from subteestaciones3d where linea = '" + linea + "'");
                // + " WHERE " + layer.FieldName + " = '" + uniq[i] + "'");
            List<Feature> features = new List<Feature>();

            while (dr.Read()){
                Feature feature = new Feature();

                string poly = dr[0].ToString().Replace("MULTILINESTRING((", "").Replace("))", "");
                poly = poly.Replace("LINESTRING((", "").Replace("))", "");
                feature.Name = dr[1].ToString();
                feature.Data = new List<string>();

                string[] ex = poly.Split(',');
                feature.Points = new List<Vector3>();
                for (int e = 0; e < ex.Length; e++) {

                    string[] pol = ex[e].Replace("(", "").Replace(")", "").Replace("POINT", "").Split(' ');
                    if (pol.Length == 2) {
                        double[] dbl = Utils.CoordinateConverter.OriginalToConverted(
                            Convert.ToDouble(pol[0].Replace(',', '.')),
                            Convert.ToDouble(21),
                            Convert.ToDouble(pol[1].Replace(',', '.'))
                            );
                        feature.OrigLine.Add(new Vector3(dbl[0], 21, dbl[2]));
                    }
                    else {

                        double[] dbl = Utils.CoordinateConverter.OriginalToConverted(
    Convert.ToDouble(pol[0].Replace('.', ',')),
    Convert.ToDouble(pol[2].Replace('.', ',')) + 21,
    Convert.ToDouble(pol[1].Replace('.', ','))
    );

                        feature.OrigLine.Add(new Vector3(dbl[0], dbl[1], dbl[2])
                        );
                    }
                }
                feature.OrigLine.Reverse();
                features.Add(feature);
            }
            dr.Close();
            Constants.CloseDBConnection();
            return features;
        }

        public void ProcessSubte() {

            Constants.ConnectToDB("routing");
            Console.WriteLine("Procesando subte...");
            NpgsqlDataReader dr = DB.GetRows("SELECT asewkt(the_geom), linea from subtelineas3d where linea != 'P'");// + " WHERE " + layer.FieldName + " = '" + uniq[i] + "'");
            List<Feature> Lineas = new List<Feature>();

            while (dr.Read()) {
                Feature feature = new Feature();

                string poly = dr[0].ToString().Replace("MULTILINESTRING((", "").Replace("))", "");
                poly = poly.Replace("LINESTRING((", "").Replace("))", "");
                feature.Name = dr[1].ToString();
                feature.Data = new List<string>();

                feature.Data.Add(dr[1].ToString());

                string[] ex = poly.Split(',');
                feature.Points = new List<Vector3>();
                for (int e = 0; e < ex.Length; e++) {
                    string[] pol = ex[e].Replace("(", "").Replace(")", "").Split(' ');
                    if (pol.Length == 2) {
                        double[] dbl = Utils.CoordinateConverter.OriginalToConverted(
                            Convert.ToDouble(pol[0].Replace(',', '.')),
                            Convert.ToDouble(21),
                            Convert.ToDouble(pol[1].Replace(',', '.'))
                            );
                        feature.OrigLine.Add(new Vector3(dbl[0], 21, dbl[2]));
                    }
                    else {

                        double[] dbl = Utils.CoordinateConverter.OriginalToConverted(
    Convert.ToDouble(pol[0].Replace('.', ',')),
    Convert.ToDouble(pol[2].Replace('.', ',')) + 21,
    Convert.ToDouble(pol[1].Replace('.', ','))
    );

                        feature.OrigLine.Add(new Vector3(dbl[0], dbl[1], dbl[2])
                        );
                    }

                }

                feature.OrigLine.Reverse();
                feature.Features = getEstaciones(feature.Name);
                Lineas.Add(feature);


            }
            dr.Close();

            CiudadWriter ciudadWriter = new CiudadWriter();
            Logger.LogLine("writing Subte.bin");


            ciudadWriter.SerializeSubte("Subte", Constants.BuildPath + @"Data\Subte\", Lineas);
            Logger.LogLine("wrote Subte.bin");


            Constants.CloseDBConnection();

        }
        */
       /*
        public void ProcessSubte3D() {

            Constants.ConnectToDB("routing");
            Console.WriteLine("Procesando subte...");
            NpgsqlDataReader dr = DB.GetRows("SELECT asewkt(st_buffer(the_geom, 0.00012)), linea from subtelineas3d where linea != 'P'");// + " WHERE " + layer.FieldName + " = '" + uniq[i] + "'");
            List<Feature> Lineas = new List<Feature>();

            while (dr.Read()) {
                Feature feature = new Feature();

                string poly = dr[0].ToString().Replace("MULTILINESTRING((", "").Replace("))", "");
                poly = poly.Replace("POLYGON((", "").Replace("))", "");
                feature.Name = dr[1].ToString();
                feature.Data = new List<string>();

                feature.Data.Add(dr[1].ToString());

                string[] ex = poly.Split(',');
                feature.Points = new List<Vector3>();
                for (int e = 0; e < ex.Length; e++){
                    string[] pol = ex[e].Replace("(", "").Replace(")", "").Split(' ');
                    if (pol.Length == 2){
                        double[] dbl = Utils.CoordinateConverter.OriginalToConverted(
                            Convert.ToDouble(pol[0].Replace(',', '.')),
                            Convert.ToDouble(21),
                            Convert.ToDouble(pol[1].Replace(',', '.'))
                            );
                        feature.OrigLine.Add(new Vector3(dbl[0], 21, dbl[2]));
                    }
                    else{

                        double[] dbl = Utils.CoordinateConverter.OriginalToConverted(
                            Convert.ToDouble(pol[0].Replace('.', ',')),
                            Convert.ToDouble(pol[2].Replace('.', ',')) + 21,
                            Convert.ToDouble(pol[1].Replace('.', ','))
                            );

                        feature.OrigLine.Add(new Vector3(dbl[0], dbl[1], dbl[2])
                            );
                    }
                }


                feature.Points = feature.OrigLine;

                    if (feature.Points.Count > 3) {
                        PolygonPoint[] points = new PolygonPoint[feature.Points.Count];
                        for (int ix = 0; ix < feature.Points.Count; ix++) {
                            points[ix] = new PolygonPoint(feature.Points[ix].X, feature.Points[ix].Z);
                        }

                        Polygon polygon = new Polygon(points);
                        //try
                        //{
                        P2T.Triangulate(polygon);

                        feature.numVertices = polygon.Triangles.Count * 3;

                        feature.numPrimitives = polygon.Triangles.Count;
                        feature.Techo = new List<Vector3>();

                        double alturita = feature.Altura;

                        for (int ix = 0; ix < feature.numPrimitives; ix++) {
                            feature.Techo.Add(
                                new Vector3(
                                    polygon.Triangles[ix].Points[0].X,//polygon.Triangles[ix].X,
                                    getHeight(feature.Points, polygon.Triangles[ix].Points[0].X, polygon.Triangles[ix].Points[0].Y) + alturita,
                                    polygon.Triangles[ix].Points[0].Y//polygon.Triangles[ix].Points[0].X;
                                    )
                                );
                            feature.Techo.Add(
                                new Vector3(
                                    polygon.Triangles[ix].Points[1].X,//polygon.Triangles[ix].X,
                                    getHeight(feature.Points, polygon.Triangles[ix].Points[1].X, polygon.Triangles[ix].Points[1].Y) + alturita,
                                    polygon.Triangles[ix].Points[1].Y//polygon.Triangles[ix].Points[0].X;
                                    )
                                );

                            feature.Techo.Add(
                               new Vector3(
                                   polygon.Triangles[ix].Points[2].X,//polygon.Triangles[ix].X,
                                   getHeight(feature.Points, polygon.Triangles[ix].Points[2].X, polygon.Triangles[ix].Points[2].Y) + alturita,
                                   polygon.Triangles[ix].Points[2].Y//polygon.Triangles[ix].Points[0].X;
                                   )
                               );
                        }

                        int[] newIndx = new int[feature.Techo.Count];
                        for (int ix = 0; ix < feature.Techo.Count; ix++) {
                            newIndx[ix] = ix;
                        }

                        feature.IndicesTecho = newIndx;
                        //} catch (Exception exe){
                        //     Console.WriteLine("ERROR: " + gid);
                        //}
                        //feature.Techo.Reverse();
                    }
                    else {
                        if (feature.Points.Count < 3) {
                            throw new Exception();
                        }
                        else {
                            feature.IndicesTecho = new int[feature.Points.Count];
                            feature.Techo = new List<Vector3>();
                            for (int o = 0; o < feature.IndicesTecho.Length; o++) {
                                feature.IndicesTecho[o] = o;
                                double alturita2 = feature.Altura;


                                feature.Techo.Add(
                                   new Vector3(
                                        feature.Points[o].X,//polygon.Triangles[ix].X,
                                        getHeight(feature.Points, feature.Points[o].X, feature.Points[o].Z) + alturita2,
                                        feature.Points[o].Z//polygon.Triangles[ix].Points[0].X;
                                       )
                                   );

                            }
                            // feature.Techo = feature.Points;
                            feature.Techo.Reverse();
                        }
                    }

                


                feature.Points = feature.Techo;
                feature.OrigLine = feature.Points;
                feature.OrigLine.Reverse();
                feature.Features = getEstaciones(feature.Name);
                Lineas.Add(feature);


            }
            dr.Close();

            CiudadWriter ciudadWriter = new CiudadWriter();
            Logger.LogLine("writing Subte.bin");


            ciudadWriter.SerializeSubte("Subte", Constants.BuildPath + @"Data\Subte\", Lineas);
            Logger.LogLine("wrote Subte.bin");

            Constants.CloseDBConnection();
        }

        */

        /*
        public List<Feature> getEstaciones(string linea)
        {

            NpgsqlDataReader dr = DB.GetRows("SELECT asewkt(the_geom), estacion from subteestaciones3d where linea = '" + linea + "'");
            // + " WHERE " + layer.FieldName + " = '" + uniq[i] + "'");
            List<Feature> features = new List<Feature>();

            while (dr.Read())
            {
                Feature feature = new Feature();

                string poly = dr[0].ToString().Replace("MULTILINESTRING((", "").Replace("))", "");
                poly = poly.Replace("LINESTRING((", "").Replace("))", "");
                feature.Name = dr[1].ToString();
                feature.Data = new List<string>();

                string[] ex = poly.Split(',');
                feature.Points = new List<Vector3>();
                for (int e = 0; e < ex.Length; e++)
                {

                    string[] pol = ex[e].Replace("(", "").Replace(")", "").Replace("POINT", "").Split(' ');
                    if (pol.Length == 2)
                    {
                        double[] dbl = Utils.CoordinateConverter.OriginalToConverted(
                            Convert.ToDouble(pol[0].Replace(',', '.')),
                            Convert.ToDouble(21),
                            Convert.ToDouble(pol[1].Replace(',', '.'))
                            );
                        feature.OrigLine.Add(new Vector3(dbl[0], 21, dbl[2]));
                    }
                    else
                    {

                        double[] dbl = Utils.CoordinateConverter.OriginalToConverted(
    Convert.ToDouble(pol[0].Replace('.', ',')),
    Convert.ToDouble(pol[2].Replace('.', ',')) + 21,
    Convert.ToDouble(pol[1].Replace('.', ','))
    );

                        feature.OrigLine.Add(new Vector3(dbl[0], dbl[1], dbl[2])
                        );
                    }
                }
                feature.OrigLine.Reverse();
                features.Add(feature);
            }
            dr.Close();
            Constants.CloseDBConnection();
            return features;
        }
        
        public void ProcessSubte()
        {

            Console.WriteLine("Procesando subte...");
            NpgsqlDataReader dr = DB.GetRows("SELECT asewkt(the_geom), linea from subtelineas3d where linea != 'P'");// + " WHERE " + layer.FieldName + " = '" + uniq[i] + "'");
            List<Feature> Lineas = new List<Feature>();

            while (dr.Read())
            {
                Feature feature = new Feature();

                string poly = dr[0].ToString().Replace("MULTILINESTRING((", "").Replace("))", "");
                poly = poly.Replace("LINESTRING((", "").Replace("))", "");
                feature.Name = dr[1].ToString();
                feature.Data = new List<string>();

                feature.Data.Add(dr[1].ToString());

                string[] ex = poly.Split(',');
                feature.Points = new List<Vector3>();
                for (int e = 0; e < ex.Length; e++)
                {
                    string[] pol = ex[e].Replace("(", "").Replace(")", "").Split(' ');
                    if (pol.Length == 2)
                    {
                        double[] dbl = Utils.CoordinateConverter.OriginalToConverted(
                            Convert.ToDouble(pol[0].Replace(',', '.')),
                            Convert.ToDouble(21),
                            Convert.ToDouble(pol[1].Replace(',', '.'))
                            );
                        feature.OrigLine.Add(new Vector3(dbl[0], 21, dbl[2]));
                    }
                    else
                    {

                        double[] dbl = Utils.CoordinateConverter.OriginalToConverted(
    Convert.ToDouble(pol[0].Replace('.', ',')),
    Convert.ToDouble(pol[2].Replace('.', ',')) + 21,
    Convert.ToDouble(pol[1].Replace('.', ','))
    );

                        feature.OrigLine.Add(new Vector3(dbl[0], dbl[1], dbl[2])
                        );
                    }

                }

                feature.OrigLine.Reverse();
                feature.Features = getEstaciones(feature.Name);
                Lineas.Add(feature);


            }
            dr.Close();

            CiudadWriter ciudadWriter = new CiudadWriter();
            Logger.LogLine("writing Subte.bin");


            ciudadWriter.SerializeSubte("Subte", Constants.BuildPath + @"Data\Subte\", Lineas);
            Logger.LogLine("wrote Subte.bin");


        }
        */

        public void Process(string name) {
            //List<Feature> Streets = new List<Feature>();
            List<Calle> Calles = new List<Calle>();

            string[] uniq = getUnique("calles3d", "nombre");

            int ca = 0;
            for (int i = 0; i < uniq.Length; i++){

                Calle calle = new Calle(uniq[i]);




                /*Feature feature = new Feature(uniq[i]);
                feature.Data = new List<string>();
                feature.Tramos = new List<Tramo>();*/

                NpgsqlDataReader dr =
                    GetRows(
                        "SELECT st_asewkt(st_geometryn(geom, 1)), sentido, tipo, altinid, altfind, altinii, altfini FROM calles3d WHERE nombre = '" +
                        uniq[i].Replace("'", "\'") + "'");
                //List<Feature> features = new List<Feature>();
                //Dictionary<string, List<LineString>> dict = new Dictionary<string, List<LineString>>();


                while (dr.Read()){
                    string poly = dr[0].ToString().Replace("LINESTRING(", "").Replace(")", "");
                    string[] ll = poly.Split(',');

                    CalleTramo calleTramo = new CalleTramo();

                    for (int x = 0; x < ll.Length; x++){
                        string[] pp = ll[x].Split(' ');

                        System.Globalization.CultureInfo ci = System.Globalization.CultureInfo.InstalledUICulture;
                        float[] dbl = CoordinateConverter.OriginalToConverted(
                            float.Parse(pp[0].Replace("(", "").Replace(")", ""), ci),
                            float.Parse(pp[2].Replace("(", "").Replace(")", ""), ci),
                            float.Parse(pp[1].Replace("(", "").Replace(")", ""), ci)
                            );

                        calleTramo.Puntos.Add(new Vector3(dbl[0], dbl[1], dbl[2]));

                    }



                    /*LineString geomm = (LineString)SharpMap.Converters.WellKnownText.GeometryFromWKT.Parse(dr[0].ToString());
                    
                    
                    foreach (Point pt in geomm.Vertices){


                    }*/
                    calleTramo.Sentido = dr[1].ToString();
                    calleTramo.Tipo = dr[2].ToString();
                    calleTramo.AlturaInicialPar = dr[5].ToString();
                    calleTramo.AlturaInicialImpar = dr[3].ToString();
                    calleTramo.AlturaFinalPar = dr[6].ToString();
                    calleTramo.AlturaFinalImpar = dr[4].ToString();


                    calle.TramosOriginales.Add(calleTramo);
                    //dict.Add();
                }

                dr.Close();
                //feature.ID = ca;
                ca++;
                calle.BuildSegments();
                Calles.Add(calle);

                //Streets.Add(feature);

            }


            
            
            CiudadWriter ciudadWriter = new CiudadWriter();
            Logger.LogLine("writing Calles.bin");
            ciudadWriter.SerializeCalles("Calles",  Constants.BuildPath, Calles);
            Logger.LogLine("wrote Calles.bin");


            //Constants.CloseDBConnection();

        }

        private List<Tramo> findConnected(Tramo Tramo, List<Tramo> Tramos, int type) {
            bool keep = false;
            Tramo current = Tramo;
            List<Tramo> tramos = new List<Tramo>();
            //tramos.Add(current);
            int vx1 = 1;
            int vx2 = 0;
            if(type == 1){
                vx1 = 0;
                vx2 = 1;

            }
            int ppp = 0;
            while(true){
                keep = false;
                for(int i = 0; i < Tramos.Count;i++){
                    
                    if (current == Tramos[i]) {
                        if (i > 0) {
                            //tramos.Add(current);
                        }
                        continue;
                    }
                    if (tramos.Contains(Tramos[i])) {
                        continue;
                    }
                    if (type == 1) {
                        vx1 = 0;
                        vx2 = Tramos[i].OrigLine.Count - 1;
                        if (current.OrigLine[vx1].X == Tramos[i].OrigLine[vx2].X && current.OrigLine[vx1].Z == Tramos[i].OrigLine[vx2].Z) {
                            tramos.Insert(0, Tramos[i]);
                            current = Tramos[i];
                            keep = true;
                        }
                    }
                    else {

                        vx1 = current.OrigLine.Count - 1;
                        vx2 = 0;
                        if (current.OrigLine[vx1].X == Tramos[i].OrigLine[vx2].X && current.OrigLine[vx1].Z == Tramos[i].OrigLine[vx2].Z) {
                            tramos.Add(Tramos[i]);
                            current = Tramos[i];
                            keep = true;
                        }
                    }
                    
                    /*else { 
                    
                    
                        if (i == (Tramos.Count - 1)) {

                           
                        }
                        keep = true;
                    }*/
                    /*if (i == Tramos.Count - 1) {
                        if (type == 1) {
                            current.Previous = Tramos[i];
                        }
                        else {
                            current.Next = Tramos[i];
                        }
                        tramos.Add(current);
                        return tramos;
                    }*/

                }
                if (!keep) { 
                    ppp++;
                }

                if (tramos.Count == Tramos.Count  - 1) {

                    return tramos;
                }

                if (ppp > Tramos.Count * 2) {
                    return tramos;
                }
                   // return tramos;
                //}
                if (Tramos.Count == tramos.Count) {
                   
                    return tramos;
                }

            }


            return tramos;
        }

        private Dictionary<int, List<Tramo>> findLinked(List<Tramo> Tramos) {
            List<Tramo> finals = new List<Tramo>();
            bool found = false;

            Dictionary<int, List<Tramo>> ListaTramos = new Dictionary<int, List<Tramo>>();
            ListaTramos.Clear();
            int current = 0;
            int currentDict = 0;
            bool insertedCrazy = false;
            List<Tramo> ListaTramosU = new List<Tramo>();
            bool getout = false;
            int curtram = 0;
            while (true) {

                if (Tramos.Count  == 0) {
                    break;
                }
                if (ListaTramosU.Count == Tramos.Count || curtram == Tramos.Count) {
                    break;
                }

                
                var tram = Tramos[curtram];
                if (tram == null) {
                    break;
                }
                curtram++;
                    //foreach(Tramo tram in Tramos){
                /*if (Tramos.Count > 0) {
                     tram = Tramos[0];
                }
                else {
                    break;
                }
                */
                if (ListaTramosU.Contains(tram)) {
                    continue;
                }
                /*if (currentDict > 0) {
                    if (ListaTramos[currentDict - 1].Contains(tram)) {
                        continue;
                    }
                }*/
                /*
                    if(tram.Next == null || tram.Previous== null ){
                        if (currentDict > 0) {
                            if (ListaTramos[currentDict - 1].Contains(tram)) {
                                continue;
                            }
                        }*/
                        List<Tramo> coneste = findConnected(tram, Tramos, 0);
                        List<Tramo> coneste2 = findConnected(tram, Tramos, 1);
                        List<Tramo> fin = new List<Tramo>();
                        //if (coneste2.Count == 0) {
                            
                        //}

                        fin.AddRange(coneste2);
                        fin.Add(tram);
                        fin.AddRange(coneste);
                        if (coneste2.Count == 1 && coneste2[0] == tram) {
                            Console.WriteLine("pepe");
                        }
                        else {
                            //fin.AddRange(coneste2);
                        }
                        

                        foreach(Tramo tr in fin){
                           // Tramos.Remove(tr);
                        }

                        if (fin.Count > 0) {
                            ListaTramos.Add(currentDict, new List<Tramo>(fin));
                            ListaTramosU.AddRange(fin);
                            currentDict++;
                        }

                        
                        /*int cntt = 0;
                        foreach (var lista in ListaTramos) {
                            cntt += lista.Value.Count;
                        }


                        if (fin.Count == Tramos.Count || cntt == Tramos.Count) {

                            return ListaTramos;
                        }*/
                    /*}else{
                        getout = true;
                    }*/
                    
                //}

                if (getout) {
                    break;
                }
                

            }


            return ListaTramos;
        }

        private Feature CleanTramos(string nombre, List<Tramo> Tramos){
            Feature Call = new Feature(nombre);

            if (Call.Name == "PAZ, GRAL., AV." || Call.Name == "SANCHEZ, FLORENCIO, AV.") {
                Call.Name = nombre;
            }

            if (Tramos.Count == 1) {
                //Tramos[0].OrigLine.Reverse();
                Call.Tramos.AddRange(Tramos);
                return Call;
            }
            for (int tramou = 0; tramou < Tramos.Count; tramou++) {
                //Tramos[tramou].OrigLine.Reverse();
            }


            Dictionary<int, List<Tramo>> ListaTramos = findLinked(Tramos);
            /*
            int tramoactual = 0;
            //Dictionary<int, List<Tramo>> ListaTramos = new Dictionary<int, List<Tramo>>();
            ListaTramos.Clear();
            int current = 0;
            int currentDict = 0;
            List<Tramo> ListaTramosU = new List<Tramo>();*/
            /*while (current < (Tramos.Count - 1)) {
                if (ListaTramosU.Count == 0) {
                    ListaTramosU.Add(Tramos[current]);
                }

                if (Tramos[current].OrigLine[Tramos[current].OrigLine.Count - 1] == Tramos[current + 1].OrigLine[0]) {
                    ListaTramosU.Add(Tramos[current + 1]);
                }
                else {
                    ListaTramos.Add(currentDict, new List<Tramo>(ListaTramosU));
                    currentDict++;
                    ListaTramosU.Clear();
                }
                current++;
            }
            while (true) {
                if (Call.Name == "FONROUGE") {
                    Call.Name = nombre;
                }
                foreach(Tramo tramo in Tramos){
                    if (tramo.Disabled){
                        //continue;
                    }
                    if (!ListaTramosU.Contains(tramo) && current == 0) {
                        //current++;
                        tramo.Disabled = true;
                        ListaTramosU.Add(tramo);
                    }
                    bool found = false;
                    bool insertedCrazy = false;
                    foreach (Tramo tramo2 in Tramos){
                        if (ListaTramosU.Contains(tramo2)) {
                            continue;
                        }
                        if (tramo != tramo2){
                            if(tramo.OrigLine[1] == tramo2.OrigLine[0]){
                                if (!ListaTramosU.Contains(tramo2)) {
                                    tramo2.Disabled = true;
                                    found = true;
                                    current++;
                                    ListaTramosU.Add(tramo2);
                                    break;
                                }
                            }else{

                                if (tramo.OrigLine[0] == tramo2.OrigLine[1]) {
                                    if (!ListaTramosU.Contains(tramo2)) {
                                        tramo2.Disabled = true;
                                        found = true;
                                        current++;
                                        insertedCrazy = true;
                                        ListaTramosU.Insert(0, tramo2);
                                        break;
                                    }
                                }
                            }
                            
                        }
                    }
                    if (!found) {

                        if (insertedCrazy && ListaTramosU.Count == Tramos.Count) {
                            ListaTramos.Add(currentDict, new List<Tramo>(ListaTramosU));
                            currentDict++;
                            ListaTramosU.Clear();
                            break;
                        }
                        ListaTramos.Add(currentDict, new List<Tramo>(ListaTramosU));
                        currentDict++;
                        ListaTramosU.Clear();

                    }
                    
                    int cntt = 0;
                    foreach (var lista in ListaTramos) {
                        cntt += lista.Value.Count;
                    }
                    if(cntt >= Tramos.Count){
                        if (cntt > Tramos.Count){
                            break;
                        }
                        current = cntt;
                        break;
                    }
                    string pepe = "";
                }

                if (current == Tramos.Count) {
                    break;
                }
                if (true) {
          //          break;
                }


            }
            */
            if (ListaTramos.Count == 0 && Tramos.Count > 0) {
                //ListaTramos.Add(currentDict, new List<Tramo>(ListaTramosU));
                Console.WriteLine("aa");
            }
            for (int ttt = 0; ttt < ListaTramos.Count; ttt++) {
                List<Tramo> Tramin = ListaTramos[ttt];
                Tramo tramoso = new Tramo();

                
                for (int xxx = 0; xxx < ListaTramos[ttt].Count; xxx++) {
                    Call.Tramos.Add(ListaTramos[ttt][xxx]);
                    //tramoso.OrigLine.AddRange(ListaTramos[ttt][xxx].OrigLine);
                    //tramoso.Data.AddRange(ListaTramos[ttt][xxx].Data);
                }
                // tramoso.Data = feature.Data
               
            }

            return Call;
            
        }

        private Dictionary<int, List<Tramo>> findLinked2(List<Tramo> Tramos) {
            List<Tramo> finals = new List<Tramo>();
            bool found = false;

            Dictionary<int, List<Tramo>> ListaTramos = new Dictionary<int, List<Tramo>>();
            ListaTramos.Clear();
            int current = 0;
            int currentDict = 0;
            bool insertedCrazy = false;
            List<Tramo> ListaTramosU = new List<Tramo>();
            bool getout = false;
            while (true) {
                foreach (Tramo tram in Tramos) {
                    if (tram.Next == null || tram.Previous == null) {

                        if (currentDict > 0) {
                            if (ListaTramos[currentDict - 1].Contains(tram)) {
                                continue;
                            }
                        }
                        List<Tramo> coneste = findConnected(tram, Tramos, 0);
                        List<Tramo> coneste2 = findConnected(tram, Tramos, 1);
                        List<Tramo> fin = new List<Tramo>();
                        if (coneste2.Count == 0) {
                            fin.Add(tram);
                        }

                        fin.AddRange(coneste);
                        if (coneste2.Count == 1 && coneste2[0] == tram) {
                            Console.WriteLine("pepe");
                        }
                        else {
                            fin.AddRange(coneste2);
                        }


                        if (fin.Count > 0) {
                            ListaTramos.Add(currentDict, new List<Tramo>(coneste));
                            currentDict++;
                        }

                        int cntt = 0;
                        foreach (var lista in ListaTramos) {
                            cntt += lista.Value.Count;
                        }


                        if (fin.Count == Tramos.Count || cntt == Tramos.Count) {

                            return ListaTramos;
                        }
                    }
                    else {
                        getout = true;
                    }

                }

                if (getout) {
                    break;
                }
            }

            return ListaTramos;
        }

    }
}
using System;
using System.Collections.Generic;
using System.IO;
using Ionic.Zip;
using Npgsql;
using System.Diagnostics;

namespace Builder {

    public class PolygonBuilder {
        string query = "";
        float ManzanaAltura = 0.21f;
        float VeredaAltura = 0.019f;
        float SiluetaAltura = 0.1f;
        float VerdeAltura = 0.02f;

        List<Feature> manzanaFeatures = null;
        List<Feature> veredasFeatures = null;
        List<Feature> siluetaFeatures = null;
        List<Feature> lotesFeatures = null;
        List<Feature> verdesFeatures = null;

        bool runLotes = true;
        bool runManzanas = true;
        bool runSiluetas = true;
        bool runVeredas = true;
        NpgsqlDataReader dr = null;
        public void ProcessManzanas(string name)
        {
        }

        public void ProcessCiudad(string name)
        {
            Color clr = LayerHelper.GetRandomColor();
            List<Color> clrs = new List<Color>();


            if (runManzanas)
            {
                query = "SELECT gid, st_asewkt(geom), espacio_ve FROM manzanas WHERE barrio = '" + name.ToUpper() + "'";
                dr = Constants.GetRows(query);
                manzanaFeatures = getFeatures(dr, false, ManzanaAltura, true);
                dr.Close();

                Color cll = new Color(255, 200, 0);

                for (int f = 0; f < manzanaFeatures.Count; f++)
                {
                    manzanaFeatures[f].Color = cll;
                    for (int i = 0; i < manzanaFeatures[f].Vertices.Length; i++)
                    {
                        manzanaFeatures[f].Vertices[i].Color = cll;
                    }
                }
                if (true)
                {
                    verdesFeatures = new List<Feature>();
                    NpgsqlDataReader drverdes = null;
                    query = "SELECT gid, ST_NumGeometries(geom) as cant FROM verdes_publico WHERE barrio = '" + name.ToUpper() + "'";
                    drverdes = Constants.GetRows(query);

                    while (drverdes.Read()){
                        string gidverde = drverdes[0].ToString();
                        int cantverdes = int.Parse(drverdes[1].ToString());

                        for (int v = 0; v < cantverdes; v++)
                        {
                            query = "SELECT gid, st_asewkt(ST_SimplifyPreserveTopology(st_geometryn(geom, " +  (v + 1) + "), 0.000006)) FROM verdes_publico WHERE gid = " + gidverde + "";
                            dr = Constants.GetRows(query);
                            List<Feature> verdesFeatures2 = getVerdes(dr, VerdeAltura);
                            dr.Close();

                            verdesFeatures.AddRange(verdesFeatures2);
                        }
                    }



                    cll = new Color(0, 102, 0);
                    for (int f = 0; f < verdesFeatures.Count; f++)
                    {
                        verdesFeatures[f].Color = cll;
                        for (int i = 0; i < verdesFeatures[f].Vertices.Length; i++)
                        {
                            verdesFeatures[f].Vertices[i].Color = new Color(0, 102, 0);
                        }
                    }

                    manzanaFeatures = null;
                    manzanaFeatures = new List<Feature>();
                    manzanaFeatures.AddRange(verdesFeatures);

                }
                if (false)
                {

                    query = "SELECT gid, st_asewkt(st_geometryn(geom, 1)) FROM verdes_catastro WHERE barrio = '" + name.ToUpper() + "'";
                    dr = Constants.GetRows(query);
                    verdesFeatures = getVerdes(dr, VerdeAltura);
                    dr.Close();
                    cll = new Color(0, 102, 0);
                    for (int f = 0; f < verdesFeatures.Count; f++)
                    {
                        verdesFeatures[f].Color = cll;
                        for (int i = 0; i < verdesFeatures[f].Vertices.Length; i++)
                        {
                            verdesFeatures[f].Vertices[i].Color = new Color(0, 102, 0);
                        }
                    }
                    manzanaFeatures.AddRange(verdesFeatures);

                }
            }

            if (runVeredas)
            {
                query = "SELECT gid, st_asewkt(ST_SimplifyPreserveTopology(st_geometryn(geom, 1), 0.000007)) FROM veredas WHERE barrio = '" + name.ToUpper() + "' ";
                dr = Constants.GetRows(query);
                veredasFeatures = getFeatures(dr, false, VeredaAltura, false);
                dr.Close();

                Color cll2 = new Color(200, 200, 200);

                Random r = new Random(2311);
                int texCount = 6;
                for (int f = 0; f < veredasFeatures.Count; f++)
                {
                    veredasFeatures[f].Texture = r.Next(0, texCount);
                    veredasFeatures[f].Color = cll2;
                    for (int i = 0; i < veredasFeatures[f].Vertices.Length; i++)
                    {
                        veredasFeatures[f].Vertices[i].Color = cll2;
                    }
                }
            }

            if (runSiluetas)
            {
                query = "SELECT gid, st_asewkt(st_geometryn(geom, 1)) FROM siluetas WHERE barrio = '" + name.ToUpper() + "'";
                dr = Constants.GetRows(query);
                siluetaFeatures = getFeatures(dr, true, SiluetaAltura, false);
                dr.Close();
                siluetaFeatures[0].Name = name;
                clr = LayerHelper.GetRandomColor();

                for (int f = 0; f < siluetaFeatures.Count; f++)
                {

                    siluetaFeatures[f].Color = clr;
                    for (int i = 0; i < siluetaFeatures[f].Vertices.Length; i++)
                    {
                        siluetaFeatures[f].Vertices[i].Color = clr;
                    }
                }

                Siluetas.AddRange(siluetaFeatures);
            }

            List<Feature> lotesFeatures = null;
            if (runLotes)
            {

                LotesBuilder lb = new LotesBuilder();
                lotesFeatures = lb.Process(name);

            }
            //siluetaFeatures = null;
            //}

            /*
            CiudadWriter meshWriter = new CiudadWriter();
            meshWriter.*/
            SerializeBarrio(
                name,
                Constants.BuildPath,
                manzanaFeatures,
                veredasFeatures,
                siluetaFeatures,
                lotesFeatures
            );
        }

        private List<Feature> getFeatures(NpgsqlDataReader dr, bool Flat, float Altura, bool isVerde)
        {
            this.r = new Random(23051983);
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
                feature.Points = parsePolygon(poly);

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
                            alt = r.Next(5, 50) + (((float)r.NextDouble() + 1f) * ((float)r.NextDouble() + 1f)).ToString();
                            alt = (r.NextDouble() + 1f) + (((float)r.NextDouble() + 1f) * ((float)r.NextDouble() + 1f)).ToString();
                            alt = "" + Constants.NormalNext(r.Next(10, 20), 100) / 2;
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
        public void SerializeBarrioMesh(
                string name,
                string path,
                LayerTypes lt,
                List<Feature> Items)
        {
            bool dumpObj = true;
            if (dumpObj)
            {
                List<Vector3> normals = new List<Vector3>();
                if (name.Contains("dddlotes"))
                {
                    List<Vector3> vertices = new List<Vector3>();
                    List<Vector2> uvs = new List<Vector2>();
                    List<Color> colors = new List<Color>();
                    colors = null;

                    for (int xx = 0; xx < Items.Count; xx++)
                    {
                        CalculateNormals(Items[xx].Vertices, Items[xx].Indices);
                        for (int xp = 0; xp < Items[xx].Vertices.Length; xp++)
                        {
                            vertices.Add(Items[xx].Vertices[xp].Position);
                            normals.Add(Items[xx].Vertices[xp].Normal);
                            uvs.Add(Items[xx].Vertices[xp].TextureCoordinate);
                            //colors.Add(Items[xx].Vertices[xp].Color);
                        }
                    }

                    ObjExporter.MeshToFile(name, path, vertices.ToArray(), normals.ToArray(), uvs.ToArray());
                }
                else
                {
                    if (lt == LayerTypes.Lotes)
                    {

                        for (int xx = 0; xx < Items.Count; xx++)
                        {
                            for (int ff = 0; ff < Items[xx].Floors.Count; ff++)
                            {
                                CalculateNormals(Items[xx].Floors[ff].Vertices, Items[xx].Floors[ff].Indices);
                            }
                        }
                        //ObjExporter.ExportLotesRoof(name, path, Items, lt.ToString(), "facade");
                        ObjExporter.ExportLotesRoofMerged(name, path, Items, lt.ToString(), "facade");
                    }
                    else
                    {
                        if (name.Contains("vered"))
                        {

                            for (int xx = 0; xx < Items.Count; xx++)
                            {
                                CalculateNormals(Items[xx].Vertices, Items[xx].Indices);
                            }
                            ObjExporter.ExportVeredas(name, path, Items, lt.ToString(), "vereda");
                        }
                        else
                        {
                            ObjExporter.ExportLotes2(name, path, Items, lt.ToString());
                        }
                    }
                    /*
                    for (int xx = 0; xx < Items.Count; xx++)
                    {
                        CalculateNormals(Items[xx].Vertices, Items[xx].Indices);
                        for (int xp = 0; xp < Items[xx].Vertices.Length; xp++)
                        {
                            normals.Add(Items[xx].Vertices[xp].Normal);
                        }
                    }

                    ObjExporter.MeshToFileLotes(name, path, Items, normals.ToArray());*/
                }

                if (false)
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.CreateNoWindow = true;
                    startInfo.UseShellExecute = false;
                    startInfo.FileName = Constants.MeshLabPath;
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    startInfo.Arguments = "-i " + path + name + ".obj -o " + path + "\\processed\\" + name + ".obj -m vc fq wt -s C:\\dev\\git\\b3d\\clean.mlx";

                    try
                    {
                        // Start the process with the info we specified.
                        // Call WaitForExit and then the using statement will close.
                        using (Process exeProcess = Process.Start(startInfo))
                        {
                            exeProcess.WaitForExit();
                        }
                    }
                    catch
                    {
                        throw new Exception();
                    }
                }
            }

        }























        
        private float getHeight(List<Vector3> points, float X, float Z) {
            for (int i = 0; i < points.Count; i++) {
                if (points[i].X == X && points[i].Z == Z) {
                    return points[i].Y;
                }
            }
            return 0;
        }
      
/*
        private double getHeight(List<Vector3> points, double X, double Z) {
            for(int i = 0; i < points.Count;i++){
                if (points[i].X == X && points[i].Z == Z) {
                    return points[i].Y;
                }
            }
            return 0;
        }  */

        public float Altura;
        public bool Flat;
        /*
        public void ProcessLotes(string name) {
            string query =  "SELECT lotes.gid, asewkt(lotes.the_geom), sr as alturita FROM lotes WHERE lotes.barrio = '" + name + "'";

            NpgsqlDataReader dr = GetRows(query);
            List<Feature> features = new List<Feature>();
            Random r = new Random(666);
            while (dr.Read()) {
                Feature feature = new Feature();
                string gid = dr[0].ToString();
                string poly = dr[1].ToString().Replace("MULTIPOLYGON(((", "").Replace(")))", "");
                poly = poly.Replace("POLYGON((", "").Replace("))", "");
                poly = poly.Replace("(((", "").Replace(")))", "");
                poly = poly.Replace("((", "").Replace("))", "");
                poly = poly.Replace("(", "").Replace(")", "");

                if (Altura == -1) {
                    string alt = dr[2].ToString();

                    if (alt == "" || alt == "0") {
                        feature.Altura = 1;
                        feature.Altura = ((float)r.NextDouble() + 0.2f) * 22;
                    } else{
                        feature.Altura = Convert.ToSingle(alt) * 22;
                    }
                    
                }
                else {
                    feature.Altura = Altura;
                }

                string[] ex = poly.Split(',');
                feature.Points = new List<Vector3>();
                for (int e = 0; e < ex.Length; e++) {
                    string[] pol = ex[e].Split(' ');

                    if (pol.Length == 2) {
                        float[] dbl = CoordinateConverter.OriginalToConverted(
                            Convert.ToDouble(pol[0]),
                            0,
                            Convert.ToDouble(pol[1])
                            );

                        feature.Points.Add(new Vector3(dbl[0], dbl[1], dbl[2])
                        );
                    }
                    else {
                        if (pol.Length == 4) {
                            float[] dbl = CoordinateConverter.OriginalToConverted(
                                Convert.ToDouble(pol[0]),
                                Convert.ToDouble(pol[2]),
                                Convert.ToDouble(pol[1])
                                );
                            dbl[1] = 0;
                            feature.Points.Add(new Vector3(dbl[0], dbl[1], dbl[2])
                            );
                        }
                        else {
                            float[] dbl = CoordinateConverter.OriginalToConverted(
                                Convert.ToDouble(pol[0]),
                                Convert.ToDouble(pol[2]),
                                Convert.ToDouble(pol[1])
                                );

                            dbl[1] = 0;
                            feature.Points.Add(new Vector3(dbl[0], dbl[1], dbl[2])
                                );
                        }
                    }
                }
                if (feature.Points[feature.Points.Count - 1].X == feature.Points[0].X && feature.Points[feature.Points.Count - 1].Z == feature.Points[0].Z) {
                    feature.Points.RemoveAt(feature.Points.Count - 1);
                }

                //feature.Points.Reverse();
                if (feature.Points.Count > 3) {
                    PolygonPoint[] points = new PolygonPoint[feature.Points.Count];
                    for (int ix = 0; ix < feature.Points.Count; ix++) {
                        points[ix] = new PolygonPoint(feature.Points[ix].X, feature.Points[ix].Z);
                    }

                    Polygon polygon = new Polygon(points);
                    try{
                    P2T.Triangulate(polygon);

                    feature.numVertices = polygon.Triangles.Count * 3;

                    feature.numPrimitives = polygon.Triangles.Count;
                    feature.Techo = new List<Vector3>();

                    double alturita = feature.Altura;

                    int[] newIndx = new int[feature.Techo.Count];
                    for (int ix = 0; ix < feature.Techo.Count; ix++) {
                        newIndx[ix] = ix;
                    }

                    feature.IndicesTecho = newIndx;
                    } catch (Exception exe){
                         Console.Write(" !!ERROR " + gid + "!! ");
                        continue;
                    }
                    //feature.Techo.Reverse();
                }
                else {
                    if (feature.Points.Count < 3) {
                        throw new Exception();
                    }else {
                        feature.IndicesTecho = new int[feature.Points.Count];
                        feature.Techo = new List<Vector3>();
                        for (int o = 0; o < feature.IndicesTecho.Length; o++) {
                            feature.IndicesTecho[o] = o;
                            float alturita2 = feature.Altura;

                            feature.Techo.Add(new Vector3(feature.Points[o].X, 
                                getHeight(feature.Points, feature.Points[o].X, feature.Points[o].Z) + alturita2, feature.Points[o].Z));

                        }

                        //feature.Techo.Reverse();
                    }
                }
                //feature.Techo.Reverse();
                features.Add(feature);
            }
            dr.Close();
            if(features.Count == 0)
                return;

            if (Flat) {
                LayerHelper.Build2d(features);
                //LayerHelper.Build3d2(features, -1);
            }
            else {
                LayerHelper.Build3d2(features, Altura);
                LayerHelper.GenerateCoords(features);

            }

            SerializedComponentInfo nfo = new SerializedComponentInfo("Victor", "23/05/1983");
            nfo.CreateVB = true;
            nfo.LOD = 0;
            nfo.EnableNormals = true;
            nfo.EnableTexture = true;
            nfo.BuildNormals = false;
            nfo.BuildTextureCoords = false;
            nfo.BaseLayer = true;

            nfo.DontCompress = true;

            nfo.LayerType = LayerTypes.Lotes;

            SerializedMesh smManzanas = new SerializedMesh(nfo);
            int VxAmount = 0;

            List<PositionNormalTextureColor> ManzanaVertices = new List<PositionNormalTextureColor>();
            List<int> ManzanaIndices = new List<int>();

            for (int m = 0; m < features.Count; m++) {
                features[m].Code = m.ToString();
                features[m].Name = m.ToString();
                features[m].VertexStart = ManzanaVertices.Count;

                //Color CurrentColor = LayerHelper.GetRandomColor();//new Color(200, 200, 200);// Color.Orange;
                byte col = (byte) r.Next(140, 180);
                Color CurrentColor = new Color(col, col, col);
                
                for (int x = 0; x < features[m].Vertices.Length; x++) {
                    features[m].Vertices[x].Color = CurrentColor;
                    PositionNormalTextureColor pc = new PositionNormalTextureColor();
                    pc.Position = features[m].Vertices[x].Position;
                    pc.Color = CurrentColor;
                    pc.TextureCoordinate = features[m].Vertices[x].TextureCoordinate;
                    pc.Normal = features[m].Vertices[x].Normal;
                    ManzanaVertices.Add(pc);
                }
                for (int xx = 0; xx < features[m].Indices.Length; xx++) {
                    ManzanaIndices.Add(VxAmount + features[m].Indices[xx]);
                }
                VxAmount += features[m].Vertices.Length;
                features[m].VertexEnd = ManzanaVertices.Count;
            }

            smManzanas.Name = name;
            smManzanas.Features = features;
            

            CiudadWriter meshWriter = new CiudadWriter();
            meshWriter.SerializeMesh(smManzanas, Constants.BuildPath + @"Data\" + LayerType + @"\", ManzanaVertices.ToArray(), ManzanaIndices.ToArray());


        }
        */
        public string LayerType = "";
        public string AlturaField = "";

        public string TableName = "";


        public void ProcessSubte(string linea)
        {
            string query = "SELECT gid, asewkt(st_buffer(the_geom, 0.00010)) FROM subte_lineas WHERE linea = '" + linea + "'";


            //query = "SELECT gid, asewkt(the_geom), sr as alturita FROM lotes_fin WHERE barrio = '" + name + "'";
           // dr = GetRows(query);


            if (Altura == -1)
            {
                //query = "SELECT gid, asewkt(the_geom), " + AlturaField + " FROM " + TableName + " WHERE " + FieldName + " = '" + uniq[i] + "'";
            }
            else {
                Altura = Altura += 0.03f;
            }
            NpgsqlDataReader dr = Constants.GetRows(query);
            List<Feature> features = new List<Feature>();
            features = getFeatures(dr, false, Altura, false);
            dr.Close();

            if (features.Count == 0)
            {
                return;
            }

            if (Flat)
            {
                //LayerHelper.Build2d(features);
                //LayerHelper.Build3d2(features, -1);
            }
            else
            {
                //LayerHelper.Build3d2(features, Altura);
                //LayerHelper.GenerateCoords(features);

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
                NpgsqlDataReader dr2 = Constants.GetRows("SELECT asewkt(the_geom), estacion FROM subte_estaciones WHERE linea = '" + linea + "'");
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

        private float getHeight(float x, float y) {

            string query = "SELECT st_value(rast, 1, st_setsrid(ST_geomfromtext('POINT(";
            query += x.ToString().Replace(",", ".") + " " + y.ToString().Replace(",", ".") + ")'), 4326)) as val FROM aster WHERE ";
            query += "ST_Intersects(aster.rast, 1, st_setsrid(ST_geomfromtext('POINT(" + x.ToString().Replace(",", ".") + " " + y.ToString().Replace(",", ".") + ")'), 4326))";

            NpgsqlDataReader dr = Constants.GetRows(query);
            dr.Read();
                
            string gid = dr[0].ToString();
            dr.Dispose();
            return Convert.ToSingle(gid);
             
            //return 0;
        }
        Random r = new Random(2311);
        private List<Vector3> parsePolygon(string polygon) {
            List<Vector3> lista = new List<Vector3>();
            string[] ll = polygon.Split(',');
            for (int i = 0; i < ll.Length; i++) {
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

            if (lista[lista.Count - 1].X == lista[0].X && lista[lista.Count - 1].Z == lista[0].Z) {
                lista.RemoveAt(lista.Count - 1);
            }
            return lista;

        }
        /*
        private List<Vector3> parsePolygon(SharpMap.Geometries.Polygon polygon) {
            List<Vector3> lista = new List<Vector3>();
            for (int i = 0; i < polygon.ExteriorRing.NumPoints; i++ ){
                Point pt = polygon.ExteriorRing.Point(i);
                float height = getHeight((float) pt.X, (float) pt.Y);

                float[] dbl = CoordinateConverter.OriginalToConverted(pt.X, height, pt.Y);
                

                dbl[1] = height;
                lista.Add(new Vector3(dbl[0], dbl[1], dbl[2]));
            }

            if (lista[lista.Count - 1].X == lista[0].X && lista[lista.Count - 1].Z == lista[0].Z) {
                lista.RemoveAt(lista.Count - 1);
            }
            return lista;

        }*/

        private float getElevation(float x, float z)
        {
            float ret = 0;
            
            string query = "SELECT ST_Value(rast, st_setsrid(st_makepoint(" + x.ToString().Replace(",", ".") + "," + z.ToString().Replace(",", ".") + "), 4326)) val FROM newlevation, parcelas WHERE ST_Intersects(rast, st_setsrid(st_makepoint(" + x.ToString().Replace(",", ".") + "," + z.ToString().Replace(",", ".") + "), 4326)) LIMIT 1";

            NpgsqlDataReader dr = Constants.GetRows(query);

            while (dr.Read())
            {
                ret = float.Parse(dr[0].ToString());
                break;
            }
            dr.Close();
            return ret;
        }

        
        private List<Feature> getTerrain(NpgsqlDataReader dr) {
            Random r = new Random(666);
            List<Feature> features = new List<Feature>();
            while (dr.Read()) {
                Feature feature = new Feature();
                string gid = dr[0].ToString();
                string poly = dr[1].ToString().Replace("MULTIPOLYGON(((", "").Replace(")))", "");
                /*
                 -58.513376713 -34.650106430002 26 0,
                 -58.5132346460052 -34.6501351482778 26 0,
                 -58.5133165923654 -34.6502214251688 26 0,
                 -58.513376713 -34.650106430002 26 0
                 
                 */
                string[] ptos = poly.Split(',');
                feature.Points = new List<Vector3>();
                for(int i = 0; i < ptos.Length - 1;i++){
                    string[] parts = ptos[i].Split(' ');
                    double xx = Convert.ToSingle(parts[0].Replace(".", ","));
                    double yy = Convert.ToSingle(parts[2].Replace(".", ","));
                    double zz = Convert.ToSingle(parts[1].Replace(".", ","));

                    float[] dbl = CoordinateConverter.OriginalToConverted(xx, yy, zz);


                    if (dbl[1] > 1.1f) {
                       // return null;
                    }


                    Vector3 vc = new Vector3(dbl[0], dbl[1], dbl[2]);
                    feature.Points.Add(vc);
                }
                //feature.Points.Reverse();
                string alts = "";// dr[2].ToString();
                if (alts == "") {
                    //continue;

                }
                /*
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
                */
                // if (geom.NumInteriorRing > 0) {
                //return null;
                //}
                
                Flat = true;
                
                /*string[] alturitas = alts.Split(',');
                alturitas.Reverse();
                for (int i = 0; i < alturitas.Length; i++) {
                    feature.Points[i] = new Vector3(feature.Points[i].X, (Convert.ToSingle(alturitas[i].Replace(" ", "")) / 5)  + 0.001f, feature.Points[i].Z);
                }
                
                */

                feature.Techo = Triangulator.Triangulate(feature.Points);
                feature.IndicesTecho = new int[feature.Techo.Count];
                for (int i = 0; i < feature.Techo.Count; i++) {
                    feature.Techo[i] = new Vector3(feature.Techo[i].X, feature.Techo[i].Y, feature.Techo[i].Z);
                    feature.IndicesTecho[i] = i;
                }
                //feature.Techo.Reverse();

                if (Flat) {
                    LayerHelper.Build2d(feature);
                }
                
                else {
                    //LayerHelper.Build3d2(features, Altura);

                    LayerHelper.Build3dFeature(feature, feature.Altura);
                    LayerHelper.GenerateCoords(feature);
                }

                for (int i = 0; i < feature.Vertices.Length; i++) {
                    feature.Vertices[i].Color = new Color(125, 125, 125);

                }

                //LayerHelper.GenerateCoords(features);
                features.Add(feature);
            }

            return features;
        }


        public void ProcessTerrain(string name){
            string query = "";
            List<Feature> terrainFeatures = new List<Feature>();

            query = "SELECT gid, st_asewkt(the_geom) FROM terrain WHERE barrio = '" + name + "'";
            NpgsqlDataReader dr = Constants.GetRows(query);
            terrainFeatures = getTerrain(dr);
            dr.Close();

            if (terrainFeatures.Count > 0) { 
                CiudadWriter meshWriter = new CiudadWriter();
                meshWriter.SerializeTerrain(
                    name,
                    Constants.BuildPath + @"\",
                    terrainFeatures
                );
            }
        }

        public List<Feature> Siluetas = new List<Feature>();
        
        public PolygonBuilder()
        {
        }

        public void SerializeBarrio(
                string name,
                string path,
                List<Feature> Manzanas,
                List<Feature> Veredas,
                List<Feature> Silueta,
                List<Feature> Lotes
            )
        {

            if (name == "Nuñez")
            {
                name = "Nunez";
            }

            if (Lotes != null)
                SerializeBarrioMesh(name, path, LayerTypes.Lotes, Lotes);

            if (Manzanas != null)
                SerializeBarrioMesh(name + "_manzanas", path, LayerTypes.Manzanas, Manzanas);

            if (Veredas != null)
                SerializeBarrioMesh(name + "_veredas", path, LayerTypes.Veredas, Veredas);

            if (Silueta != null)
                SerializeBarrioMesh(name + "_silueta", path, LayerTypes.Silueta, Silueta);

        }
        public void SerializeBarrioOBJ(
                string name,
                string path,
                LayerTypes lt,
                List<Feature> Items)
        {

            using (StreamWriter writetext = new StreamWriter(path + name + ".obj"))
            {
                int inc = 1;
                for (int xx = 0; xx < Items.Count; xx++)
                {
                    //CalculateNormals(Items[xx].Vertices, Items[xx].Indices);                

                    for (int xp = 0; xp < Items[xx].Vertices.Length; xp++)
                    {
                        //byte ix = (byte)MathHelper.Clamp(Items[xx].Vertices[xp].Position.X * 127.0f + 128.0f, 0.0f, 255.0f);
                        //byte iy = (byte)MathHelper.Clamp(Items[xx].Vertices[xp].Position.Y * 127.0f + 128.0f, 0.0f, 255.0f);
                        //byte iz = (byte)MathHelper.Clamp(Items[xx].Vertices[xp].Position.Z * 127.0f + 128.0f, 0.0f, 255.0f);

                        //BinaryWriter.Write((float)Items[xx].Vertices[xp].Position.Z);
                        //BinaryWriter.Write((float)Items[xx].Vertices[xp].Position.Y);
                        //BinaryWriter.Write((float)Items[xx].Vertices[xp].Position.X);

                        string data = (float)Items[xx].Vertices[xp].Position.Z + " " + (float)Items[xx].Vertices[xp].Position.Y + " " + (float)Items[xx].Vertices[xp].Position.X;
                        data = data.Replace(",", ".");
                        writetext.WriteLine("v " + data);

                    }

                }
                for (int xx = 0; xx < Items.Count; xx++)
                {
                    //CalculateNormals(Items[xx].Vertices, Items[xx].Indices);
                    
                    int xxx = 0;
                    for (int xp = 0; xp <= Items[xx].Indices.Length - 3; xp +=3)
                    {
                        //byte ix = (byte)MathHelper.Clamp(Items[xx].Vertices[xp].Position.X * 127.0f + 128.0f, 0.0f, 255.0f);
                        //byte iy = (byte)MathHelper.Clamp(Items[xx].Vertices[xp].Position.Y * 127.0f + 128.0f, 0.0f, 255.0f);
                        //byte iz = (byte)MathHelper.Clamp(Items[xx].Vertices[xp].Position.Z * 127.0f + 128.0f, 0.0f, 255.0f);

                        //BinaryWriter.Write((float)Items[xx].Vertices[xp].Position.Z);
                        //BinaryWriter.Write((float)Items[xx].Vertices[xp].Position.Y);
                        //BinaryWriter.Write((float)Items[xx].Vertices[xp].Position.X);
                        writetext.Write("f ");
                        writetext.Write("" + (inc + Items[xx].Indices[xp]) + " ");
                        writetext.Write("" + (inc + Items[xx].Indices[xp + 1]) + " ");
                        writetext.Write("" + (inc + Items[xx].Indices[xp + 2]) + " ");
                        writetext.Write("\n");
                        /*
                        if (xxx == 2)
                        {
                            xxx = 0;
                            writetext.Write("\n");
                            if(xp < (Items[xx].Indices.Length - 1)) { 
                                writetext.Write("f ");
                            }
                        }
                        else { 
                            xxx++;
                        }*/

                    }

                    inc += Items[xx].Indices.Length;
                    writetext.Write("\n");

                }
                
            }
        }






        private void CalculateNormals(PositionNormalTextureColor[] vertices, int[] indices)
        {

            Vector3[] vertexNormals = new Vector3[vertices.Length];

            AccumulateTriangleNormals(indices, vertices, vertexNormals);

            for (int i = 0; i < vertexNormals.Length; i++)
            {
                vertexNormals[i] = SafeNormalize(vertexNormals[i]);
            }

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Normal = vertexNormals[i];
            }


        }

        private static void AccumulateTriangleNormals(int[] indices, PositionNormalTextureColor[] vertices, Vector3[] vertexNormals)
        {

            PositionNormalTextureColor[] positions = vertices;
            int[] positionIndices = indices;
            for (int i = 0; i < indices.Length - 3; i += 3)
            {
                Vector3 vector4 = vertices[indices[i]].Position;
                Vector3 vector = vertices[indices[i + 1]].Position;
                Vector3 vector3 = vertices[indices[i + 2]].Position;
                Vector3 vector2 = SafeNormalize(Vector3.Cross(vector3 - vector, vector - vector4));
                for (int j = 0; j < 3; j++)
                {
                    vertexNormals[positionIndices[indices[i + j]]] += vector2;
                }
            }
        }

        private static Vector3 SafeNormalize(Vector3 value)
        {
            float num = value.Length();
            if (num == 0)
            {
                return Vector3.Zero;
            }
            return (Vector3)(value / num);
        }


        public void SerializeBarrioMesh2(
                string name,
                string path,
                LayerTypes lt,
                List<Feature> Items)
        {
            bool dumpObj = true;
            if (dumpObj)
            {
                List<Vector3> vertices = new List<Vector3>();
                List<Vector3> normals = new List<Vector3>();
                List<Vector2> uvs = new List<Vector2>();
                List<Color> colors = new List<Color>();
                for (int xx = 0; xx < Items.Count; xx++)
                {
                    CalculateNormals(Items[xx].Vertices, Items[xx].Indices);
                    for (int xp = 0; xp < Items[xx].Vertices.Length; xp++)
                    {
                        vertices.Add(Items[xx].Vertices[xp].Position);
                        normals.Add(Items[xx].Vertices[xp].Normal);
                        uvs.Add(Items[xx].Vertices[xp].TextureCoordinate);
                        colors.Add(Items[xx].Vertices[xp].Color);
                    }
                }

                ObjExporter.MeshToFile(name, path, vertices.ToArray(), normals.ToArray(), uvs.ToArray(), colors.ToArray());


                if (false) { 
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.CreateNoWindow = true;
                    startInfo.UseShellExecute = false;
                    startInfo.FileName = Constants.MeshLabPath;
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    startInfo.Arguments = "-i " + path + name + ".obj -o " + path + "\\processed\\" + name + ".obj -m vc fq wt -s C:\\dev\\git\\b3d\\clean.mlx";

                    try
                    {
                        // Start the process with the info we specified.
                        // Call WaitForExit and then the using statement will close.
                        using (Process exeProcess = Process.Start(startInfo))
                        {
                            exeProcess.WaitForExit();
                        }
                    }
                    catch
                    {
                        throw new Exception();
                    }
                }
                /**/

            }
            //SerializeBarrioOBJ(name, path, lt, Items);
            return;


            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter BinaryWriter = new BinaryWriter(memoryStream);
            //Directory.CreateDirectory(Path.GetDirectoryName(path));
            FileStream fs = new FileStream(path + name + ".bin", FileMode.Create);

            BinaryWriter.Write(name);
            BinaryWriter.Write((int)lt);

            int vertexCount = 0;

            for (int xx = 0; xx < Items.Count; xx++)
            {
                vertexCount += Items[xx].Vertices.Length;
            }

            //BinaryWriter.Write(Items.Count);
            /*for (int xx = 0; xx < Items.Count; xx++)
            {
                if (lt == LayerTypes.Silueta)
                {
                    //BinaryWriter.Write(Items[xx].Name);
                }
                //BinaryWriter.Write(Items[xx].Vertices.Length);
            }*/
            List<Color> cols = new List<Color>();
            if(lt != LayerTypes.Lotes)
            {
                for (int xx = 0; xx < Items.Count; xx++)
                {
                    for (int xp = 0; xp < Items[xx].Vertices.Length; xp++)
                    {
                        if (!cols.Contains(Items[xx].Vertices[xp].Color))
                        {
                            cols.Add(Items[xx].Vertices[xp].Color);
                        }
                    }
                }
            }else
            {
                if(name.Contains("Almagro"))
                {
                    string pepe = "a";
                    pepe = "b";
                }
                for (int xx = 0; xx < Items.Count; xx++)
                {
                    cols.Add(Items[xx].Vertices[0].Color);
                    Items[xx].ColorIndex = cols.Count - 1;
                }
            }
            BinaryWriter.Write(cols.Count);
            for (int cc = 0; cc < cols.Count; cc++)
            {
                BinaryWriter.Write(cols[cc].R);
                BinaryWriter.Write(cols[cc].G);
                BinaryWriter.Write(cols[cc].B);
                //BinaryWriter.Write((int)cols[cc].A);
            }


            int wrote = 0;
            BinaryWriter.Write(vertexCount);
            for (int xx = 0; xx < Items.Count; xx++)
            {
                //CalculateNormals(Items[xx].Vertices, Items[xx].Indices);                
                if (lt == LayerTypes.Silueta)
                {
                    //BinaryWriter.Write(Items[xx].Name);
                    // BinaryWriter.Write(Items[xx].Name);
                    BinaryWriter.Write(Items[xx].Vertices.Length);
                }
                //if(lt == LayerTypes.Lotes)
                //{
                   // Items[xx].Color = 
               // }
                for (int xp = 0; xp < Items[xx].Vertices.Length; xp++)
                {
                    //byte ix = (byte)MathHelper.Clamp(Items[xx].Vertices[xp].Position.X * 127.0f + 128.0f, 0.0f, 255.0f);
                    //byte iy = (byte)MathHelper.Clamp(Items[xx].Vertices[xp].Position.Y * 127.0f + 128.0f, 0.0f, 255.0f);
                    //byte iz = (byte)MathHelper.Clamp(Items[xx].Vertices[xp].Position.Z * 127.0f + 128.0f, 0.0f, 255.0f);

                    BinaryWriter.Write((float)Items[xx].Vertices[xp].Position.Z);
                    BinaryWriter.Write((float)Items[xx].Vertices[xp].Position.Y);
                    BinaryWriter.Write((float)Items[xx].Vertices[xp].Position.X);
                    //BinaryWriter.Write((float)Items[xx].Vertices[xp].Normal.Z);
                    //BinaryWriter.Write((float)Items[xx].Vertices[xp].Normal.Y);
                    //BinaryWriter.Write((float)Items[xx].Vertices[xp].Normal.X);
                    if(lt == LayerTypes.Lotes){
                        BinaryWriter.Write(Items[xx].ColorIndex);
                   // Items[xx].Color = 
                    }else{
                        BinaryWriter.Write(cols.IndexOf(Items[xx].Vertices[xp].Color));
                    }

                    
                }
                wrote += Items[xx].Vertices.Length;
            }

            BinaryWriter.Flush();
            fs.Write(memoryStream.GetBuffer(), 0, Convert.ToInt32(memoryStream.Position));
            BinaryWriter.BaseStream.Close();
            fs.Flush();
            fs.Close();
            memoryStream.Close();
            BinaryWriter.Close();
            CompressFile(path + name + ".bin");
        }
        private void CompressFile(string fileName)
        {

            using (ZipFile zip = new ZipFile())
            {
                // note: this does not recurse directories! 
                // string[] filenames = System.IO.Directory.GetFiles(DirectoryToZip);

                // This is just a sample, provided to illustrate the DotNetZip interface.  
                // This logic does not recurse through sub-directories.
                // If you are zipping up a directory, you may want to see the AddDirectory() method, 
                // which operates recursively. 
                //foreach (string filename in filenames) {
                //Console.WriteLine("Adding {0}...", fileName);

                zip.CompressionLevel = Ionic.Zlib.CompressionLevel.Level2;

                ZipEntry e = zip.AddFile(fileName, "");
                e.Comment = "Baires 3D - Victor Antolini";
                //}

                zip.Comment = String.Format("Baires 3D - Victor Antolini '{0}'", "lalala" /*System.Net.Dns.GetHostName()*/);

                string filePath2 = Path.GetDirectoryName(fileName);
                string name2 = Path.GetFileNameWithoutExtension(fileName);
                zip.Save(filePath2 + "/" + name2 + ".zip");
            }
            File.Delete(fileName);
        }


        private List<Feature> getVerdes(NpgsqlDataReader dr, float Altura) {
            Random r = new Random(666);
            List<Feature> features = new List<Feature>();
            while (dr.Read()) {
                Feature feature = new Feature();
                string gid = dr[0].ToString();
                if (dr[1].ToString().Replace("SRID=4326;", "") == "POLYGON EMPTY") {
                    continue;
                }
                if(dr[1].ToString() == ""){
                    Console.WriteLine(gid);
                    continue;
                }
                string poly = dr[1].ToString().Replace("MULTIPOLYGON(((", "").Replace(")))", "");
                poly = poly.Replace("SRID=4326;", "");
                poly = poly.Replace("POLYGON((", "").Replace("))", "");
                if (poly == "POLYGON EMPTY") {
                    continue;
                }

                feature.Points = parsePolygon(poly);
                /*var geom = SharpMap.Converters.WellKnownText.GeometryFromWKT.Parse(dr[1].ToString().Replace("SRID=4326;", ""));
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
                */
                feature.Altura = Altura;

                feature.Techo = Triangulator.Triangulate(feature.Points);

                feature.IndicesTecho = new int[feature.Techo.Count];
                for (int i = 0; i < feature.Techo.Count; i++) {
                    feature.Techo[i] = new Vector3(feature.Techo[i].X, feature.Techo[i].Y + feature.Altura, feature.Techo[i].Z);
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

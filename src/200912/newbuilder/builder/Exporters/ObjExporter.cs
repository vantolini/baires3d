using System.Collections;
using System.IO;
using System.Text;
using Builder;
using System.Collections.Generic;
using System;

public static class ObjExporter {

    internal static void ExportLotesRoofMerged(string name, string path, List<Feature> Items, string groupname, string texturename) {


        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();

        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();





        List<string> manzas = new List<string>();
        List<string> secciones = new List<string>();

        for (int xx = 0; xx < Items.Count; xx++) {

            if (!secciones.Contains(Items[xx].Seccion)) {
                secciones.Add(Items[xx].Seccion);

            }
            if (!manzas.Contains(Items[xx].Manzana)) {
                manzas.Add(Items[xx].Manzana);

            }
        }
        bool dumpTechos = true;

        for (int ss = 0; ss < secciones.Count; ss++) {
            string baseSeccion = path + groupname + "\\" + name.Replace(" ", "_") + "\\seccion_" + secciones[ss] + "\\";

            Directory.CreateDirectory(Path.GetDirectoryName(baseSeccion));


            Dictionary<string, List<Feature>> Parcelas = new Dictionary<string, List<Feature>>();
            for (int mm = 0; mm < manzas.Count; mm++) {


                for (int xx = 0; xx < Items.Count; xx++) {
                    if (secciones[ss] != Items[xx].Seccion) {
                        continue;
                    }
                    if (manzas[mm] != Items[xx].Manzana) {
                        continue;
                    }

                    if (Parcelas.ContainsKey(manzas[mm])) {
                        Parcelas[manzas[mm]].Add(Items[xx]);
                    } else {
                        List<Feature> ls = new List<Feature>();
                        ls.Add(Items[xx]);

                        Parcelas.Add(manzas[mm], ls);
                    }

                }

                dumpTechos = false;
            }

            List<Color> cantText = new List<Color>();
            List<Color> cantRoof = new List<Color>();

            string mtlName = "material.mtl";//manza + ".mtl";

            foreach (KeyValuePair<string, List<Feature>> manzana in Parcelas) {
                string manza = manzana.Key;
                string baseManzana = baseSeccion + "\\manzana_" + manza + ".obj";

                StreamWriter sw = new StreamWriter(baseManzana);

                StringBuilder sb = new StringBuilder();

                sb.Append("mtllib ").Append(mtlName).Append("\n");


                int cur = 1;

                foreach (Feature feature in manzana.Value) {


                    sb.Append("o parcela_" + feature.Parcela).Append("\n");

                    for (int f = 0; f < feature.Floors.Count; f++) {

                        int ixcol = 0;

                        if (feature.Floors[f].isRoof) {
                            if (!cantRoof.Contains(feature.Floors[f].Color)) {
                                cantRoof.Add(feature.Floors[f].Color);

                            }
                            ixcol = cantRoof.IndexOf(feature.Floors[f].Color);


                            sb.Append("g parcela_" + feature.Parcela + "_roof").Append("\n");
                        } else {
                            if (!cantText.Contains(feature.Floors[f].Color)) {
                                cantText.Add(feature.Floors[f].Color);

                            }
                            ixcol = cantText.IndexOf(feature.Floors[f].Color);


                            sb.Append("g parcela_" + feature.Parcela + "_floor_").Append(f.ToString()).Append("\n");
                        }

                        if (feature.Floors[f].isRoof) {

                            sb.Append("usemtl ").Append("roof_mtl_" + ixcol.ToString()).Append("\n");
                        } else {
                            if (feature.Floors[f].isSingle) {

                                sb.Append("usemtl ").Append("floor_single_mtl_" + ixcol.ToString()).Append("\n");
                            } else {
                                if (feature.Floors[f].isFirst) {
                                    sb.Append("usemtl ").Append("floor_first_mtl_" + ixcol.ToString()).Append("\n");
                                } else if (feature.Floors[f].isLast) {
                                    sb.Append("usemtl ").Append("floor_last_mtl_" + ixcol.ToString()).Append("\n");
                                } else {

                                    sb.Append("usemtl ").Append("floor_middle_mtl_" + ixcol.ToString()).Append("\n");
                                }
                            }
                        }


                        if (!feature.Floors[f].isRoof) {
                            for (int i = 0; i < feature.Floors[f].Vertices.Length; i++) {
                                PositionNormalTextureColor v = feature.Floors[f].Vertices[i];
                                sb.Append(string.Format("v {0} {1} {2}\n", v.Position.Z.ToString().Replace(",", "."), v.Position.Y.ToString().Replace(",", "."), v.Position.X.ToString().Replace(",", ".")));
                            }
                            for (int i = 0; i < feature.Floors[f].Vertices.Length; i++) {
                                PositionNormalTextureColor v = feature.Floors[f].Vertices[i];
                                sb.Append(string.Format("vn {0} {1} {2}\n", v.Normal.Z.ToString().Replace(",", "."), v.Normal.Y.ToString().Replace(",", "."), v.Normal.X.ToString().Replace(",", ".")));
                            }
                            for (int i = 0; i < feature.Floors[f].Vertices.Length; i++) {
                                PositionNormalTextureColor v = feature.Floors[f].Vertices[i];
                                sb.Append(string.Format("vt {0} {1}\n", v.TextureCoordinate.X.ToString().Replace(",", "."), v.TextureCoordinate.Y.ToString().Replace(",", ".")));
                            }

                            for (int i = cur; i < (cur + feature.Floors[f].Vertices.Length); i += 3) {
                                sb.Append("f " + i + "/" + i + "/" + i + " " + (i + 1) + "/" + (i + 1) + "/" + (i + 1) + " " + (i + 2) + "/" + (i + 2) + "/" + (i + 2) + "\n");
                            }
                            cur += feature.Floors[f].Vertices.Length;


                            sb.Append("\n");
                            sb.Append("\n");
                        } else {

                            for (int i = 0; i < feature.Floors[f].Techo.Count; i++) {
                                PositionNormalTextureColor v = feature.Floors[f].Vertices[i];
                                sb.Append(string.Format("v {0} {1} {2}\n", v.Position.Z.ToString().Replace(",", "."), v.Position.Y.ToString().Replace(",", "."), v.Position.X.ToString().Replace(",", ".")));
                            }
                            for (int i = 0; i < feature.Floors[f].Techo.Count; i++) {
                                PositionNormalTextureColor v = feature.Floors[f].Vertices[i];
                                sb.Append(string.Format("vn {0} {1} {2}\n", v.Normal.Z.ToString().Replace(",", "."), v.Normal.Y.ToString().Replace(",", "."), v.Normal.X.ToString().Replace(",", ".")));
                            }
                            for (int i = 0; i < feature.Floors[f].Techo.Count; i++) {
                                PositionNormalTextureColor v = feature.Floors[f].Vertices[i];
                                sb.Append(string.Format("vt {0} {1}\n", v.TextureCoordinate.X.ToString().Replace(",", "."), v.TextureCoordinate.Y.ToString().Replace(",", ".")));
                            }
                            sb.Append("\n");
                            sb.Append("\n");


                            for (int i = cur; i < (cur + feature.Floors[f].Techo.Count); i += 3) {
                                sb.Append("f " + i + "/" + i + "/" + i + " " + (i + 1) + "/" + (i + 1) + "/" + (i + 1) + " " + (i + 2) + "/" + (i + 2) + "/" + (i + 2) + "\n");
                            }

                            cur += feature.Floors[f].Techo.Count;


                        }

                        sb.Append("\n");
                    }

                    sb.Append("\n");
                    sb.Append("\n");

                }


                sw.Write(sb.ToString());
                sw.Close();
                //mtks
            }

            StringBuilder sb2 = new StringBuilder();
            for (int xx = 0; xx < cantRoof.Count; xx++) {
                Vector3 colorito = cantRoof[xx].ToVector3();
                sb2.Append("newmtl roof_mtl_" + (xx).ToString() + " \n");
                sb2.Append("Ka  " + colorito.X.ToString().Replace(",", ".") + " " + colorito.Y.ToString().Replace(",", ".") + " " + colorito.Z.ToString().Replace(",", ".") + "\n");
                sb2.Append("Kd  " + colorito.X.ToString().Replace(",", ".") + " " + colorito.Y.ToString().Replace(",", ".") + " " + colorito.Z.ToString().Replace(",", ".") + "\n");
                //sb2.Append("Kd  0.0922  0.0166  0.9000 \n");
                sb2.Append("Ks  1 1 1 \n");
                sb2.Append("illum 1 \n");
                sb2.Append("Ns 20 \n\n");
            }

            for (int xx = 0; xx < cantText.Count; xx++) {
                Vector3 colorito = cantText[xx].ToVector3();

                sb2.Append("newmtl floor_single_mtl_" + (xx).ToString() + " \n");
                sb2.Append("Ka  " + colorito.X.ToString().Replace(",", ".") + " " + colorito.Y.ToString().Replace(",", ".") + " " + colorito.Z.ToString().Replace(",", ".") + "\n");
                sb2.Append("Kd  " + colorito.X.ToString().Replace(",", ".") + " " + colorito.Y.ToString().Replace(",", ".") + " " + colorito.Z.ToString().Replace(",", ".") + "\n");
                sb2.Append("Ks  1 1 1 \nillum 1 \nNs 20 \n");
                sb2.Append("\n");

                sb2.Append("newmtl floor_first_mtl_" + (xx).ToString() + " \n");
                sb2.Append("Ka  " + colorito.X.ToString().Replace(",", ".") + " " + colorito.Y.ToString().Replace(",", ".") + " " + colorito.Z.ToString().Replace(",", ".") + "\n");
                sb2.Append("Kd  " + colorito.X.ToString().Replace(",", ".") + " " + colorito.Y.ToString().Replace(",", ".") + " " + colorito.Z.ToString().Replace(",", ".") + "\n");
                sb2.Append("Ks  1 1 1 \nillum 1 \nNs 20 \n");
                sb2.Append("\n");
                /*
                sb2.Append("Kd  " + colorito.X.ToString().Replace(",", ".") + " " + colorito.Y.ToString().Replace(",", ".") + " " + colorito.Z.ToString().Replace(",", ".") + "\n");
                sb2.Append("Ks  1 1 1 \nillum 1 \nNs 20 \n\n");
                */

                sb2.Append("newmtl floor_middle_mtl_" + (xx).ToString() + " \n");
                sb2.Append("Ka  " + colorito.X.ToString().Replace(",", ".") + " " + colorito.Y.ToString().Replace(",", ".") + " " + colorito.Z.ToString().Replace(",", ".") + "\n");
                sb2.Append("Kd  " + colorito.X.ToString().Replace(",", ".") + " " + colorito.Y.ToString().Replace(",", ".") + " " + colorito.Z.ToString().Replace(",", ".") + "\n");
                sb2.Append("Ks  1 1 1 \nillum 1 \nNs 20 \n");
                sb2.Append("\n");
                /*
                sb2.Append("Kd  " + colorito.X.ToString().Replace(",", ".") + " " + colorito.Y.ToString().Replace(",", ".") + " " + colorito.Z.ToString().Replace(",", ".") + "\n");
                sb2.Append("Ks  1 1 1 \n");
                sb2.Append("illum 1 \n");
                sb2.Append("Ns 20 \n\n");
                */


                sb2.Append("newmtl floor_last_mtl_" + (xx).ToString() + " \n");
                sb2.Append("Ka  " + colorito.X.ToString().Replace(",", ".") + " " + colorito.Y.ToString().Replace(",", ".") + " " + colorito.Z.ToString().Replace(",", ".") + "\n");
                sb2.Append("Kd  " + colorito.X.ToString().Replace(",", ".") + " " + colorito.Y.ToString().Replace(",", ".") + " " + colorito.Z.ToString().Replace(",", ".") + "\n");
                sb2.Append("Ks  1 1 1 \nillum 1 \nNs 20 \n");
                sb2.Append("\n");
                /*
                sb2.Append("Kd  " + colorito.X.ToString().Replace(",", ".") + " " + colorito.Y.ToString().Replace(",", ".") + " " + colorito.Z.ToString().Replace(",", ".") + "\n");
                sb2.Append("Ks  1 1 1 \n");
                sb2.Append("illum 1 \n");
                sb2.Append("Ns 20 \n\n");
                */
            }

            using (StreamWriter swmtl = new StreamWriter(baseSeccion + "\\" + mtlName)) {
                swmtl.Write(sb2.ToString());
            }

        }

    }


    internal static void ExportLotesRoofSplit(string name, string path, List<Feature> Items, string groupname, string texturename)
    {


        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();

        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();





        List<string> manzas = new List<string>();
        List<string> secciones = new List<string>();

        for (int xx = 0; xx < Items.Count; xx++)
        {

            if (!secciones.Contains(Items[xx].Seccion))
            {
                secciones.Add(Items[xx].Seccion);

            }
            if (!manzas.Contains(Items[xx].Manzana))
            {
                manzas.Add(Items[xx].Manzana);

            }
        }
        bool dumpTechos = true;

        for (int ss = 0; ss < secciones.Count; ss++)
        {
            string baseSeccion = path + groupname + "\\" + name.Replace(" ", "_") + "\\seccion_" + secciones[ss] + "\\";

            Directory.CreateDirectory(Path.GetDirectoryName(baseSeccion));


            Dictionary<string, List<Feature>> Parcelas = new Dictionary<string, List<Feature>>();
            for (int mm = 0; mm < manzas.Count; mm++)
            {


                for (int xx = 0; xx < Items.Count; xx++)
                {
                    if (secciones[ss] != Items[xx].Seccion)
                    {
                        continue;
                    }
                    if (manzas[mm] != Items[xx].Manzana)
                    {
                        continue;
                    }

                    if (Parcelas.ContainsKey(manzas[mm]))
                    {
                        Parcelas[manzas[mm]].Add(Items[xx]);
                    }
                    else
                    {
                        List<Feature> ls = new List<Feature>();
                        ls.Add(Items[xx]);

                        Parcelas.Add(manzas[mm], ls);
                    }

                }

                dumpTechos = false;
            }

            List<Color> cantText = new List<Color>();
            List<Color> cantRoof = new List<Color>();

            string mtlName = "material.mtl";//manza + ".mtl";

            foreach (KeyValuePair<string, List<Feature>> manzana in Parcelas)
            {
                string manza = manzana.Key;
                string baseManzana = baseSeccion + "\\manzana_" + manza + "\\";

                Directory.CreateDirectory(Path.GetDirectoryName(baseManzana));




                foreach (Feature feature in manzana.Value)
                {

                    StreamWriter sw = new StreamWriter(baseManzana + "\\" + feature.Parcela + ".obj");

                    StringBuilder sb = new StringBuilder();

                    sb.Append("mtllib ../").Append(mtlName).Append("\n");


                    sb.Append("o parcela_" + feature.Parcela).Append("\n");

                    int cur = 1;

                    for (int f = 0; f < feature.Floors.Count; f++)
                    {

                        int ixcol = 0;

                        if (feature.Floors[f].isRoof)
                        {
                            if (!cantRoof.Contains(feature.Floors[f].Color))
                            {
                                cantRoof.Add(feature.Floors[f].Color);

                            }
                            ixcol = cantRoof.IndexOf(feature.Floors[f].Color);


                            sb.Append("g roof").Append("\n");
                        }
                        else
                        {
                            if (!cantText.Contains(feature.Floors[f].Color))
                            {
                                cantText.Add(feature.Floors[f].Color);

                            }
                            ixcol = cantText.IndexOf(feature.Floors[f].Color);


                            sb.Append("g floor_").Append(f.ToString()).Append("\n");
                        }

                        if (feature.Floors[f].isRoof)
                        {

                            sb.Append("usemtl ").Append("roof_mtl_" + ixcol.ToString()).Append("\n");
                        }
                        else
                        {
                            if (feature.Floors[f].isSingle)
                            {

                                sb.Append("usemtl ").Append("floor_single_mtl_" + ixcol.ToString()).Append("\n");
                            }
                            else
                            {
                                if (feature.Floors[f].isFirst)
                                {
                                    sb.Append("usemtl ").Append("floor_first_mtl_" + ixcol.ToString()).Append("\n");
                                } else if (feature.Floors[f].isLast) {
                                    sb.Append("usemtl ").Append("floor_last_mtl_" + ixcol.ToString()).Append("\n");
                                } else {

                                    sb.Append("usemtl ").Append("floor_middle_mtl_" + ixcol.ToString()).Append("\n");
                                }
                            }
                        }


                        if (!feature.Floors[f].isRoof)
                        {
                            for (int i = 0; i < feature.Floors[f].Vertices.Length; i++)
                            {
                                PositionNormalTextureColor v = feature.Floors[f].Vertices[i];
                                sb.Append(string.Format("v {0} {1} {2}\n", v.Position.Z.ToString().Replace(",", "."), v.Position.Y.ToString().Replace(",", "."), v.Position.X.ToString().Replace(",", ".")));
                            }
                            for (int i = 0; i < feature.Floors[f].Vertices.Length; i++)
                            {
                                PositionNormalTextureColor v = feature.Floors[f].Vertices[i];
                                sb.Append(string.Format("vn {0} {1} {2}\n", v.Normal.Z.ToString().Replace(",", "."), v.Normal.Y.ToString().Replace(",", "."), v.Normal.X.ToString().Replace(",", ".")));
                            }
                            for (int i = 0; i < feature.Floors[f].Vertices.Length; i++)
                            {
                                PositionNormalTextureColor v = feature.Floors[f].Vertices[i];
                                sb.Append(string.Format("vt {0} {1}\n", v.TextureCoordinate.X.ToString().Replace(",", "."), v.TextureCoordinate.Y.ToString().Replace(",", ".")));
                            }

                            for (int i = cur; i < (cur + feature.Floors[f].Vertices.Length); i += 3)
                            {
                                sb.Append("f " + i + "/" + i + "/" + i + " " + (i + 1) + "/" + (i + 1) + "/" + (i + 1) + " " + (i + 2) + "/" + (i + 2) + "/" + (i + 2) + "\n");
                            }
                            cur += feature.Floors[f].Vertices.Length;


                            sb.Append("\n");
                            sb.Append("\n");
                        }
                        else
                        {

                            for (int i = 0; i < feature.Floors[f].Techo.Count; i++)
                            {
                                PositionNormalTextureColor v = feature.Floors[f].Vertices[i];
                                sb.Append(string.Format("v {0} {1} {2}\n", v.Position.Z.ToString().Replace(",", "."), v.Position.Y.ToString().Replace(",", "."), v.Position.X.ToString().Replace(",", ".")));
                            }
                            for (int i = 0; i < feature.Floors[f].Techo.Count; i++)
                            {
                                PositionNormalTextureColor v = feature.Floors[f].Vertices[i];
                                sb.Append(string.Format("vn {0} {1} {2}\n", v.Normal.Z.ToString().Replace(",", "."), v.Normal.Y.ToString().Replace(",", "."), v.Normal.X.ToString().Replace(",", ".")));
                            }
                            for (int i = 0; i < feature.Floors[f].Techo.Count; i++)
                            {
                                PositionNormalTextureColor v = feature.Floors[f].Vertices[i];
                                sb.Append(string.Format("vt {0} {1}\n", v.TextureCoordinate.X.ToString().Replace(",", "."), v.TextureCoordinate.Y.ToString().Replace(",", ".")));
                            }
                            sb.Append("\n");
                            sb.Append("\n");


                            for (int i = cur; i < (cur + feature.Floors[f].Techo.Count); i += 3)
                            {
                                sb.Append("f " + i + "/" + i + "/" + i + " " + (i + 1) + "/" + (i + 1) + "/" + (i + 1) + " " + (i + 2) + "/" + (i + 2) + "/" + (i + 2) + "\n");
                            }

                            cur += feature.Floors[f].Techo.Count;


                        }

                        sb.Append("\n");
                    }

                    sb.Append("\n");
                    sb.Append("\n");

                    sw.Write(sb.ToString());
                    sw.Close();
                }


                //mtks
            }




            StringBuilder sb2 = new StringBuilder();
            for (int xx = 0; xx < cantRoof.Count; xx++)
            {
                Vector3 colorito = cantRoof[xx].ToVector3();
                sb2.Append("newmtl roof_mtl_" + (xx).ToString() + " \n");
                sb2.Append("Ka  " + colorito.X.ToString().Replace(",", ".") + " " + colorito.Y.ToString().Replace(",", ".") + " " + colorito.Z.ToString().Replace(",", ".") + "\n");
                sb2.Append("Kd  " + colorito.X.ToString().Replace(",", ".") + " " + colorito.Y.ToString().Replace(",", ".") + " " + colorito.Z.ToString().Replace(",", ".") + "\n");
                //sb2.Append("Kd  0.0922  0.0166  0.9000 \n");
                sb2.Append("Ks  1 1 1 \n");
                sb2.Append("illum 1 \n");
                sb2.Append("Ns 20 \n\n");
            }

            for (int xx = 0; xx < cantText.Count; xx++)
            {
                Vector3 colorito = cantText[xx].ToVector3();

                sb2.Append("newmtl floor_single_mtl_" + (xx).ToString() + " \n");
                sb2.Append("Ka  " + colorito.X.ToString().Replace(",", ".") + " " + colorito.Y.ToString().Replace(",", ".") + " " + colorito.Z.ToString().Replace(",", ".") + "\n");
                sb2.Append("Kd  " + colorito.X.ToString().Replace(",", ".") + " " + colorito.Y.ToString().Replace(",", ".") + " " + colorito.Z.ToString().Replace(",", ".") + "\n");
                sb2.Append("Ks  1 1 1 \nillum 1 \nNs 20 \n");
                sb2.Append("\n");

                sb2.Append("newmtl floor_first_mtl_" + (xx).ToString() + " \n");
                sb2.Append("Ka  " + colorito.X.ToString().Replace(",", ".") + " " + colorito.Y.ToString().Replace(",", ".") + " " + colorito.Z.ToString().Replace(",", ".") + "\n");
                sb2.Append("Kd  " + colorito.X.ToString().Replace(",", ".") + " " + colorito.Y.ToString().Replace(",", ".") + " " + colorito.Z.ToString().Replace(",", ".") + "\n");
                sb2.Append("Ks  1 1 1 \nillum 1 \nNs 20 \n");
                sb2.Append("\n");
                /*
                sb2.Append("Kd  " + colorito.X.ToString().Replace(",", ".") + " " + colorito.Y.ToString().Replace(",", ".") + " " + colorito.Z.ToString().Replace(",", ".") + "\n");
                sb2.Append("Ks  1 1 1 \nillum 1 \nNs 20 \n\n");
                */

                sb2.Append("newmtl floor_middle_mtl_" + (xx).ToString() + " \n");
                sb2.Append("Ka  " + colorito.X.ToString().Replace(",", ".") + " " + colorito.Y.ToString().Replace(",", ".") + " " + colorito.Z.ToString().Replace(",", ".") + "\n");
                sb2.Append("Kd  " + colorito.X.ToString().Replace(",", ".") + " " + colorito.Y.ToString().Replace(",", ".") + " " + colorito.Z.ToString().Replace(",", ".") + "\n");
                sb2.Append("Ks  1 1 1 \nillum 1 \nNs 20 \n");
                sb2.Append("\n");
                /*
                sb2.Append("Kd  " + colorito.X.ToString().Replace(",", ".") + " " + colorito.Y.ToString().Replace(",", ".") + " " + colorito.Z.ToString().Replace(",", ".") + "\n");
                sb2.Append("Ks  1 1 1 \n");
                sb2.Append("illum 1 \n");
                sb2.Append("Ns 20 \n\n");
                */


                sb2.Append("newmtl floor_last_mtl_" + (xx).ToString() + " \n");
                sb2.Append("Ka  " + colorito.X.ToString().Replace(",", ".") + " " + colorito.Y.ToString().Replace(",", ".") + " " + colorito.Z.ToString().Replace(",", ".") + "\n");
                sb2.Append("Kd  " + colorito.X.ToString().Replace(",", ".") + " " + colorito.Y.ToString().Replace(",", ".") + " " + colorito.Z.ToString().Replace(",", ".") + "\n");
                sb2.Append("Ks  1 1 1 \nillum 1 \nNs 20 \n");
                sb2.Append("\n");
                /*
                sb2.Append("Kd  " + colorito.X.ToString().Replace(",", ".") + " " + colorito.Y.ToString().Replace(",", ".") + " " + colorito.Z.ToString().Replace(",", ".") + "\n");
                sb2.Append("Ks  1 1 1 \n");
                sb2.Append("illum 1 \n");
                sb2.Append("Ns 20 \n\n");
                */
            }

            using (StreamWriter swmtl = new StreamWriter(baseSeccion + "\\" + mtlName))
            {
                swmtl.Write(sb2.ToString());
            }





        }




    }





    internal static void ExportLotesRoof(string name, string path, List<Feature> Items, string groupname, string texturename)
    {

        StringBuilder sb2 = new StringBuilder();


        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();

        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();



        List<Color> cantText = new List<Color>();
        List<Color> cantRoof = new List<Color>();
        

        List<string> manzas = new List<string>();



        for (int xx = 0; xx < Items.Count; xx++)
        {

            if (!manzas.Contains(Items[xx].Manzana))
            {
                manzas.Add(Items[xx].Manzana);

            }
        }
        bool dumpTechos = true;

        for (int mm = 0; mm < manzas.Count; mm++)
        {
            StreamWriter sw = new StreamWriter(path + groupname + "\\" +name.Replace(" ", "_") + "\\seccion_" + manzas[mm].Replace(" ", "_") + ".obj");

            StringBuilder sb = new StringBuilder();

            sb.Append("mtllib ").Append(name.Replace(" ", "_") + ".mtl").Append("\n");


            int cur = 1;

            for (int xx = 0; xx < Items.Count; xx++)
            {
                if (manzas[mm] != Items[xx].Manzana)
                {
                    continue;
                }
                sb.Append("o " + groupname + "_lote_").Append(xx.ToString()).Append("\n");


                for (int f = 0; f < Items[xx].Floors.Count; f++)
                {
                    
                    int ixcol = 0;

                    if (Items[xx].Floors[f].isRoof)
                    {
                        if (!cantRoof.Contains(Items[xx].Floors[f].Color))
                        {
                            cantRoof.Add(Items[xx].Floors[f].Color);

                        }
                        ixcol = cantRoof.IndexOf(Items[xx].Floors[f].Color);


                        sb.Append("g " + groupname + "_" + xx.ToString() + "_roof").Append("\n");
                    }
                    else
                    {
                        if (!cantText.Contains(Items[xx].Floors[f].Color))
                        {
                            cantText.Add(Items[xx].Floors[f].Color);

                        }
                        ixcol = cantText.IndexOf(Items[xx].Floors[f].Color);


                        sb.Append("g " + groupname + "_" + xx.ToString() + "_floor_").Append(f.ToString()).Append("\n");
                    }

                    if (Items[xx].Floors[f].isRoof)
                    {

                        sb.Append("usemtl ").Append(groupname + "_roof_mtl_" + ixcol.ToString()).Append("\n");
                    }
                    else
                    {

                        sb.Append("usemtl ").Append(groupname + "_floor_mtl_" + ixcol.ToString()).Append("\n");
                    }


                    if (!Items[xx].Floors[f].isRoof)
                    {
                        for (int i = 0; i < Items[xx].Floors[f].Vertices.Length; i++)
                        {
                            PositionNormalTextureColor v = Items[xx].Floors[f].Vertices[i];
                            sb.Append(string.Format("v {0} {1} {2}\n", v.Position.Z.ToString().Replace(",", "."), v.Position.Y.ToString().Replace(",", "."), v.Position.X.ToString().Replace(",", ".")));
                        }
                        for (int i = 0; i < Items[xx].Floors[f].Vertices.Length; i++)
                        {
                            PositionNormalTextureColor v = Items[xx].Floors[f].Vertices[i];
                            sb.Append(string.Format("vn {0} {1} {2}\n", v.Normal.Z.ToString().Replace(",", "."), v.Normal.Y.ToString().Replace(",", "."), v.Normal.X.ToString().Replace(",", ".")));
                        }
                        for (int i = 0; i < Items[xx].Floors[f].Vertices.Length; i++)
                        {
                            PositionNormalTextureColor v = Items[xx].Floors[f].Vertices[i];
                            sb.Append(string.Format("vt {0} {1}\n", v.TextureCoordinate.X.ToString().Replace(",", "."), v.TextureCoordinate.Y.ToString().Replace(",", ".")));
                        }

                        for (int i = cur; i < (cur + Items[xx].Floors[f].Vertices.Length); i += 3)
                        {
                            sb.Append("f " + i + "/" + i + "/" + i + " " + (i + 1) + "/" + (i + 1) + "/" + (i + 1) + " " + (i + 2) + "/" + (i + 2) + "/" + (i + 2) + "\n");
                        }
                        cur += Items[xx].Floors[f].Vertices.Length;


                        sb.Append("\n");
                        sb.Append("\n");
                    }
                    else
                    {

                        //sb.Append("g " + groupname + "_roof_").Append(xx.ToString()).Append("\n");
                        //sb.Append("usemtl ").Append(groupname + "_roof_mtl").Append("\n");

                        //}
                        for (int i = 0; i < Items[xx].Floors[f].Techo.Count; i++)
                        {
                            PositionNormalTextureColor v = Items[xx].Floors[f].Vertices[i];
                            sb.Append(string.Format("v {0} {1} {2}\n", v.Position.Z.ToString().Replace(",", "."), v.Position.Y.ToString().Replace(",", "."), v.Position.X.ToString().Replace(",", ".")));
                        }
                        for (int i = 0; i < Items[xx].Floors[f].Techo.Count; i++)
                        {
                            PositionNormalTextureColor v = Items[xx].Floors[f].Vertices[i];
                            sb.Append(string.Format("vn {0} {1} {2}\n", v.Normal.Z.ToString().Replace(",", "."), v.Normal.Y.ToString().Replace(",", "."), v.Normal.X.ToString().Replace(",", ".")));
                        }
                        for (int i = 0; i < Items[xx].Floors[f].Techo.Count; i++)
                        {
                            PositionNormalTextureColor v = Items[xx].Floors[f].Vertices[i];
                            sb.Append(string.Format("vt {0} {1}\n", v.TextureCoordinate.X.ToString().Replace(",", "."), v.TextureCoordinate.Y.ToString().Replace(",", ".")));
                        }
                        sb.Append("\n");
                        sb.Append("\n");


                        for (int i = cur; i < (cur + Items[xx].Floors[f].Techo.Count); i += 3)
                        {
                            sb.Append("f " + i + "/" + i + "/" + i + " " + (i + 1) + "/" + (i + 1) + "/" + (i + 1) + " " + (i + 2) + "/" + (i + 2) + "/" + (i + 2) + "\n");
                        }

                        cur += Items[xx].Floors[f].Techo.Count;


                    }




                    sb.Append("\n");


                   // sw.Write(sb.ToString());
                   // sw.Close();



                }
            }
            sb.Append("\n");
            sb.Append("\n");

            sw.Write(sb.ToString());
            sw.Close();

        }
        
        /*

        Vector3 colorito1 = Color.DarkGray.ToVector3();

        sb2.Append("newmtl " + groupname + "_roof_mtl" + " \n");
        sb2.Append("Ka  " + colorito1.X.ToString().Replace(",", ".") + " " + colorito1.Y.ToString().Replace(",", ".") + " " + colorito1.Z.ToString().Replace(",", ".") + "\n");
        sb2.Append("Kd  " + colorito1.X.ToString().Replace(",", ".") + " " + colorito1.Y.ToString().Replace(",", ".") + " " + colorito1.Z.ToString().Replace(",", ".") + "\n");
        //sb2.Append("Kd  0.0922  0.0166  0.9000 \n");
        sb2.Append("Ks  1 1 1 \n");
        sb2.Append("illum 1 \n");
        sb2.Append("Ns 20 \n\n");

        */


        for (int xx = 0; xx < cantRoof.Count; xx++)
        {
            Vector3 colorito = cantRoof[xx].ToVector3();
            sb2.Append("newmtl " + groupname + "_roof_mtl_" + (xx).ToString() + " \n");
            sb2.Append("Ka  " + colorito.X.ToString().Replace(",", ".") + " " + colorito.Y.ToString().Replace(",", ".") + " " + colorito.Z.ToString().Replace(",", ".") + "\n");
            sb2.Append("Kd  " + colorito.X.ToString().Replace(",", ".") + " " + colorito.Y.ToString().Replace(",", ".") + " " + colorito.Z.ToString().Replace(",", ".") + "\n");
            //sb2.Append("Kd  0.0922  0.0166  0.9000 \n");
            sb2.Append("Ks  1 1 1 \n");
            sb2.Append("illum 1 \n");
            sb2.Append("Ns 20 \n\n");
        }

        for (int xx = 0; xx < cantText.Count; xx++)
        {
            Vector3 colorito = cantText[xx].ToVector3();
            sb2.Append("newmtl " + groupname + "_floor_mtl_" + (xx).ToString() + " \n");
            sb2.Append("Ka  " + colorito.X.ToString().Replace(",", ".") + " " + colorito.Y.ToString().Replace(",", ".") + " " + colorito.Z.ToString().Replace(",", ".") + "\n");
            sb2.Append("Kd  " + colorito.X.ToString().Replace(",", ".") + " " + colorito.Y.ToString().Replace(",", ".") + " " + colorito.Z.ToString().Replace(",", ".") + "\n");
            //sb2.Append("Kd  0.0922  0.0166  0.9000 \n");
            sb2.Append("Ks  1 1 1 \n");
            sb2.Append("illum 1 \n");
            sb2.Append("Ns 20 \n\n");
        }

        using (StreamWriter swmtl = new StreamWriter(path + groupname + "\\" + name.Replace(" ", "_") + "\\" + name.Replace(" ", "_") + ".mtl"))
        {
            swmtl.Write(sb2.ToString());
        }




    }




















































    internal static void ExportLotesSingleManzanasSplit(string name, string path, List<Feature> Items, string groupname, string texturename)
    {

        StringBuilder sb2 = new StringBuilder();


        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();

        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();



        List<Color> cantText = new List<Color>();


        List<string> manzas = new List<string>();



        for (int xx = 0; xx < Items.Count; xx++)
        {

            if (!manzas.Contains(Items[xx].Manzana))
            {
                manzas.Add(Items[xx].Manzana);

            }
        }
        bool dumpTechos = true;

        for (int mm = 0; mm < manzas.Count; mm++)
        {
            StreamWriter sw = new StreamWriter(path + groupname + "\\" + name.Replace(" ", "_") + "\\seccion_" + manzas[mm].Replace(" ", "_") + ".obj");

            StringBuilder sb = new StringBuilder();

            sb.Append("mtllib ").Append(name.Replace(" ", "_") + ".mtl").Append("\n");


            int cur = 1;

            for (int xx = 0; xx < Items.Count; xx++)
            {
                if (manzas[mm] != Items[xx].Manzana)
                {
                    continue;
                }
                sb.Append("o " + groupname + "_lote_").Append(xx.ToString()).Append("\n");

                sb.Append("g " + groupname + "_pared_").Append(xx.ToString()).Append("\n");
                if (!cantText.Contains(Items[xx].Color))
                {
                    cantText.Add(Items[xx].Color);

                }
                int ixcol = cantText.IndexOf(Items[xx].Color);

                if (Items[xx].Vereda)
                {
                    sb.Append("#veredita\n");

                }
                sb.Append("usemtl ").Append(groupname + "_pared_mtl_" + ixcol.ToString()).Append("\n");

                if (dumpTechos)
                {


                    for (int i = Items[xx].Techo.Count; i < Items[xx].Vertices.Length; i++)
                    {
                        PositionNormalTextureColor v = Items[xx].Vertices[i];
                        sb.Append(string.Format("v {0} {1} {2}\n", v.Position.Z.ToString().Replace(",", "."), v.Position.Y.ToString().Replace(",", "."), v.Position.X.ToString().Replace(",", ".")));
                    }
                    for (int i = Items[xx].Techo.Count; i < Items[xx].Vertices.Length; i++)
                    {
                        PositionNormalTextureColor v = Items[xx].Vertices[i];
                        sb.Append(string.Format("vn {0} {1} {2}\n", v.Normal.Z.ToString().Replace(",", "."), v.Normal.Y.ToString().Replace(",", "."), v.Normal.X.ToString().Replace(",", ".")));
                    }
                    for (int i = Items[xx].Techo.Count; i < Items[xx].Vertices.Length; i++)
                    {
                        PositionNormalTextureColor v = Items[xx].Vertices[i];
                        sb.Append(string.Format("vt {0} {1}\n", v.TextureCoordinate.X.ToString().Replace(",", "."), v.TextureCoordinate.Y.ToString().Replace(",", ".")));
                    }

                    for (int i = cur; i < (cur + Items[xx].Paredes.Length); i += 3)
                    {
                        sb.Append("f " + i + "/" + i + "/" + i + " " + (i + 1) + "/" + (i + 1) + "/" + (i + 1) + " " + (i + 2) + "/" + (i + 2) + "/" + (i + 2) + "\n");
                    }
                    cur += Items[xx].Paredes.Length;


                    sb.Append("\n");
                    sb.Append("\n");

                    sb.Append("g " + groupname + "_techo_").Append(xx.ToString()).Append("\n");
                    /*if (Items[xx].Vereda)
                    {
                        sb.Append("usemtl ").Append(groupname + "_mtl_" + ixcol.ToString()).Append("\n");

                    }
                    else
                    {*/

                    sb.Append("usemtl ").Append(groupname + "_techo_mtl").Append("\n");

                    //}
                    for (int i = 0; i < Items[xx].Techo.Count; i++)
                    {
                        PositionNormalTextureColor v = Items[xx].Vertices[i];
                        sb.Append(string.Format("v {0} {1} {2}\n", v.Position.Z.ToString().Replace(",", "."), v.Position.Y.ToString().Replace(",", "."), v.Position.X.ToString().Replace(",", ".")));
                    }
                    for (int i = 0; i < Items[xx].Techo.Count; i++)
                    {
                        PositionNormalTextureColor v = Items[xx].Vertices[i];
                        sb.Append(string.Format("vn {0} {1} {2}\n", v.Normal.Z.ToString().Replace(",", "."), v.Normal.Y.ToString().Replace(",", "."), v.Normal.X.ToString().Replace(",", ".")));
                    }
                    for (int i = 0; i < Items[xx].Techo.Count; i++)
                    {
                        PositionNormalTextureColor v = Items[xx].Vertices[i];
                        sb.Append(string.Format("vt {0} {1}\n", v.TextureCoordinate.X.ToString().Replace(",", "."), v.TextureCoordinate.Y.ToString().Replace(",", ".")));
                    }
                    sb.Append("\n");
                    sb.Append("\n");


                    for (int i = cur; i < (cur + Items[xx].Techo.Count); i += 3)
                    {
                        sb.Append("f " + i + "/" + i + "/" + i + " " + (i + 1) + "/" + (i + 1) + "/" + (i + 1) + " " + (i + 2) + "/" + (i + 2) + "/" + (i + 2) + "\n");
                    }

                    cur += Items[xx].Techo.Count;
                }
                else
                {
                    for (int i = 0; i < Items[xx].Vertices.Length; i++)
                    {
                        PositionNormalTextureColor v = Items[xx].Vertices[i];
                        sb.Append(string.Format("v {0} {1} {2}\n", v.Position.Z.ToString().Replace(",", "."), v.Position.Y.ToString().Replace(",", "."), v.Position.X.ToString().Replace(",", ".")));
                    }
                    for (int i = 0; i < Items[xx].Vertices.Length; i++)
                    {
                        PositionNormalTextureColor v = Items[xx].Vertices[i];
                        sb.Append(string.Format("vn {0} {1} {2}\n", v.Normal.Z.ToString().Replace(",", "."), v.Normal.Y.ToString().Replace(",", "."), v.Normal.X.ToString().Replace(",", ".")));
                    }
                    for (int i = 0; i < Items[xx].Vertices.Length; i++)
                    {
                        PositionNormalTextureColor v = Items[xx].Vertices[i];
                        sb.Append(string.Format("vt {0} {1}\n", v.TextureCoordinate.X.ToString().Replace(",", "."), v.TextureCoordinate.Y.ToString().Replace(",", ".")));
                    }


                    for (int i = cur; i < (cur + Items[xx].Vertices.Length); i += 3)
                    {
                        sb.Append("f " + i + "/" + i + "/" + i + " " + (i + 1) + "/" + (i + 1) + "/" + (i + 1) + " " + (i + 2) + "/" + (i + 2) + "/" + (i + 2) + "\n");
                    }
                    cur += Items[xx].Vertices.Length;
                }

                sb.Append("\n");

            }
            sb.Append("\n");
            sb.Append("\n");

            sw.Write(sb.ToString());
            sw.Close();

        }

        Vector3 colorito1 = Color.DarkGray.ToVector3();

        sb2.Append("newmtl " + groupname + "_techo_mtl" + " \n");
        sb2.Append("Ka  " + colorito1.X.ToString().Replace(",", ".") + " " + colorito1.Y.ToString().Replace(",", ".") + " " + colorito1.Z.ToString().Replace(",", ".") + "\n");
        sb2.Append("Kd  " + colorito1.X.ToString().Replace(",", ".") + " " + colorito1.Y.ToString().Replace(",", ".") + " " + colorito1.Z.ToString().Replace(",", ".") + "\n");
        //sb2.Append("Kd  0.0922  0.0166  0.9000 \n");
        sb2.Append("Ks  1 1 1 \n");
        sb2.Append("illum 1 \n");
        sb2.Append("Ns 20 \n\n");


        for (int xx = 0; xx < cantText.Count; xx++)
        {
            Vector3 colorito = cantText[xx].ToVector3();
            sb2.Append("newmtl " + groupname + "_pared_mtl_" + (xx).ToString() + " \n");
            sb2.Append("Ka  " + colorito.X.ToString().Replace(",", ".") + " " + colorito.Y.ToString().Replace(",", ".") + " " + colorito.Z.ToString().Replace(",", ".") + "\n");
            sb2.Append("Kd  " + colorito.X.ToString().Replace(",", ".") + " " + colorito.Y.ToString().Replace(",", ".") + " " + colorito.Z.ToString().Replace(",", ".") + "\n");
            //sb2.Append("Kd  0.0922  0.0166  0.9000 \n");
            sb2.Append("Ks  1 1 1 \n");
            sb2.Append("illum 1 \n");
            sb2.Append("Ns 20 \n\n");
        }

        using (StreamWriter swmtl = new StreamWriter(path + groupname + "\\" + name.Replace(" ", "_") + "\\" + name.Replace(" ", "_") + ".mtl"))
        {
            swmtl.Write(sb2.ToString());
        }




    }
    internal static void ExportLotesSingleManzanas(string name, string path, List<Feature> Items, string groupname, string texturename)
    {

        using (StreamWriter sw = new StreamWriter(path + name.Replace(" ", "_") + ".obj"))
        {

            StringBuilder sb = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();


            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();

            List<Vector3> normals = new List<Vector3>();
            List<Vector2> uv = new List<Vector2>();



            List<Color> cantText = new List<Color>();


            List<string> manzas = new List<string>();


            int cur = 1;

            sb.Append("mtllib ").Append(name.Replace(" ", "_") + ".mtl").Append("\n");

            for (int xx = 0; xx < Items.Count; xx++)
            {

                if (!manzas.Contains(Items[xx].Manzana))
                {
                    manzas.Add(Items[xx].Manzana);

                }
            }

            for (int mm = 0; mm < manzas.Count; mm++){

                sb.Append("o " + groupname + "_manzana_").Append(mm.ToString()).Append("\n");

                for (int xx = 0; xx < Items.Count; xx++){
                    if (manzas[mm] !=  Items[xx].Manzana)
                    {
                        continue;
                    }


                    sb.Append("g " + groupname + "_").Append(xx.ToString()).Append("\n");
                    if (!cantText.Contains(Items[xx].Color))
                    {
                        cantText.Add(Items[xx].Color);

                    }
                    int ixcol = cantText.IndexOf(Items[xx].Color);

                    sb.Append("usemtl ").Append(groupname + "_mtl_" + ixcol.ToString()).Append("\n");
                    for (int i = 0; i < Items[xx].Vertices.Length; i++)
                    {
                        PositionNormalTextureColor v = Items[xx].Vertices[i];
                        sb.Append(string.Format("v {0} {1} {2}\n", v.Position.Z.ToString().Replace(",", "."), v.Position.Y.ToString().Replace(",", "."), v.Position.X.ToString().Replace(",", ".")));
                    }
                    for (int i = 0; i < Items[xx].Vertices.Length; i++)
                    {
                        PositionNormalTextureColor v = Items[xx].Vertices[i];
                        sb.Append(string.Format("vn {0} {1} {2}\n", v.Normal.Z.ToString().Replace(",", "."), v.Normal.Y.ToString().Replace(",", "."), v.Normal.X.ToString().Replace(",", ".")));
                    }
                    for (int i = 0; i < Items[xx].Vertices.Length; i++)
                    {
                        PositionNormalTextureColor v = Items[xx].Vertices[i];
                        sb.Append(string.Format("vt {0} {1}\n", v.TextureCoordinate.X.ToString().Replace(",", "."), v.TextureCoordinate.Y.ToString().Replace(",", ".")));
                    }


                    for (int i = cur; i < (cur + Items[xx].Vertices.Length); i += 3)
                    {
                        sb.Append("f " + i + "/" + i + "/" + i + " " + (i + 1) + "/" + (i + 1) + "/" + (i + 1) + " " + (i + 2) + "/" + (i + 2) + "/" + (i + 2) + "\n");
                    }
                    cur += Items[xx].Vertices.Length;


                    sb.Append("\n");
                    sb.Append("\n");


                }
                sb.Append("\n");
                sb.Append("\n");
            }

            for (int xx = 0; xx < cantText.Count; xx++)
            {
                Vector3 colorito = cantText[xx].ToVector3();
                sb2.Append("newmtl " + groupname + "_mtl_" + (xx).ToString() + " \n");
                sb2.Append("Ka  " + colorito.X.ToString().Replace(",", ".") + " " + colorito.Y.ToString().Replace(",", ".") + " " + colorito.Z.ToString().Replace(",", ".") + "\n");
                sb2.Append("Kd  " + colorito.X.ToString().Replace(",", ".") + " " + colorito.Y.ToString().Replace(",", ".") + " " + colorito.Z.ToString().Replace(",", ".") + "\n");
                //sb2.Append("Kd  0.0922  0.0166  0.9000 \n");
                sb2.Append("Ks  1 1 1 \n");
                sb2.Append("illum 1 \n");
                sb2.Append("Ns 20 \n\n");
            }

            using (StreamWriter swmtl = new StreamWriter(path + name.Replace(" ", "_") + ".mtl"))
            {
                swmtl.Write(sb2.ToString());
            }


            sw.Write(sb.ToString());

        }
    }

    internal static void ExportLotesSingle(string name, string path, List<Feature> Items, string groupname, string texturename)
    {

        using (StreamWriter sw = new StreamWriter(path + name.Replace(" ", "_") + ".obj"))
        {

            StringBuilder sb = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();


            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();

            List<Vector3> normals = new List<Vector3>();
            List<Vector2> uv = new List<Vector2>();



            List<Color> cantText = new List<Color>();


            int cur = 1;

            sb.Append("mtllib ").Append(name.Replace(" ", "_") + ".mtl").Append("\n");

            for (int xx = 0; xx < Items.Count; xx++)
            {

                sb.Append("g " + groupname + "_").Append(xx.ToString()).Append("\n");

                if (!cantText.Contains(Items[xx].Color))
                {
                    cantText.Add(Items[xx].Color);

                }
                int ixcol = cantText.IndexOf(Items[xx].Color);

                sb.Append("usemtl ").Append(groupname + "_mtl_" + ixcol.ToString()).Append("\n");
                for (int i = 0; i < Items[xx].Vertices.Length; i++)
                {
                    PositionNormalTextureColor v = Items[xx].Vertices[i];
                    sb.Append(string.Format("v {0} {1} {2}\n", v.Position.Z.ToString().Replace(",", "."), v.Position.Y.ToString().Replace(",", "."), v.Position.X.ToString().Replace(",", ".")));
                }
                for (int i = 0; i < Items[xx].Vertices.Length; i++)
                {
                    PositionNormalTextureColor v = Items[xx].Vertices[i];
                    sb.Append(string.Format("vn {0} {1} {2}\n", v.Normal.Z.ToString().Replace(",", "."), v.Normal.Y.ToString().Replace(",", "."), v.Normal.X.ToString().Replace(",", ".")));
                }
                for (int i = 0; i < Items[xx].Vertices.Length; i++)
                {
                    PositionNormalTextureColor v = Items[xx].Vertices[i];
                    sb.Append(string.Format("vt {0} {1}\n", v.TextureCoordinate.X.ToString().Replace(",", "."), v.TextureCoordinate.Y.ToString().Replace(",", ".")));
                }


                for (int i = cur; i < (cur + Items[xx].Vertices.Length); i += 3)
                {
                    sb.Append("f " + i + "/" + i + "/" + i + " " + (i + 1) + "/" + (i + 1) + "/" + (i + 1) + " " + (i + 2) + "/" + (i + 2) + "/" + (i + 2) + "\n");
                }
                cur += Items[xx].Vertices.Length;


                sb.Append("\n");
                sb.Append("\n");



            }

            for (int xx = 0; xx < cantText.Count; xx++)
            {
                Vector3 colorito = cantText[xx].ToVector3();
                sb2.Append("newmtl " + groupname + "_mtl_" + (xx).ToString() + " \n");
                sb2.Append("Ka  " + colorito.X.ToString().Replace(",", ".") + " " + colorito.Y.ToString().Replace(",", ".") + " " + colorito.Z.ToString().Replace(",", ".") + "\n");
                sb2.Append("Kd  " + colorito.X.ToString().Replace(",", ".") + " " + colorito.Y.ToString().Replace(",", ".") + " " + colorito.Z.ToString().Replace(",", ".") + "\n");
                //sb2.Append("Kd  0.0922  0.0166  0.9000 \n");
                sb2.Append("Ks  1 1 1 \n");
                sb2.Append("illum 1 \n");
                sb2.Append("Ns 20 \n\n");
            }

            using (StreamWriter swmtl = new StreamWriter(path + name.Replace(" ", "_") + ".mtl"))
            {
                swmtl.Write(sb2.ToString());
            }


            sw.Write(sb.ToString());

        }
    }

    internal static void ExportLotes(string name, string path, List<Feature> Items, string groupname, string texturename)
    {

        using (StreamWriter sw = new StreamWriter(path + name.Replace(" ", "_") + ".obj"))
        {

            StringBuilder sb = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();


            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();

            List<Vector3> normals = new List<Vector3>();
            List<Vector2> uv = new List<Vector2>();



            List<int> cantText = new List<int>();


            int cur = 1;


            sb.Append("mtllib ").Append(name.Replace(" ", "_") + ".mtl").Append("\n");

            for (int xx = 0; xx < Items.Count; xx++)
            {

                sb.Append("g " + groupname + "_paredes_").Append(xx.ToString()).Append("\n");

                if (!cantText.Contains(Items[xx].Texture))
                {
                    cantText.Add(Items[xx].Texture);

                }
                sb.Append("usemtl ").Append(groupname + "_texture_mtl" + Items[xx].Texture.ToString()).Append("\n");
                for (int i = Items[xx].Techo.Count; i < Items[xx].Vertices.Length; i++)
                {
                    PositionNormalTextureColor v = Items[xx].Vertices[i];
                    sb.Append(string.Format("v {0} {1} {2}\n", v.Position.Z.ToString().Replace(",", "."), v.Position.Y.ToString().Replace(",", "."), v.Position.X.ToString().Replace(",", ".")));
                }
                for (int i = Items[xx].Techo.Count; i < Items[xx].Vertices.Length; i++)
                {
                    PositionNormalTextureColor v = Items[xx].Vertices[i];
                    sb.Append(string.Format("vn {0} {1} {2}\n", v.Normal.Z.ToString().Replace(",", "."), v.Normal.Y.ToString().Replace(",", "."), v.Normal.X.ToString().Replace(",", ".")));
                }
                for (int i = Items[xx].Techo.Count; i < Items[xx].Vertices.Length; i++)
                {
                    PositionNormalTextureColor v = Items[xx].Vertices[i];
                    sb.Append(string.Format("vt {0} {1}\n", v.TextureCoordinate.X.ToString().Replace(",", "."), v.TextureCoordinate.Y.ToString().Replace(",", ".")));
                }

                for (int i = cur; i < (cur + Items[xx].Paredes.Length); i += 3)
                {
                    sb.Append("f " + i + "/" + i + "/" + i + " " + (i + 1) + "/" + (i + 1) + "/" + (i + 1) + " " + (i + 2) + "/" + (i + 2) + "/" + (i + 2) + "\n");
                }

                cur += Items[xx].Paredes.Length;



                sb.Append("g " + groupname + "_techo_").Append(xx.ToString()).Append("\n");

                sb.Append("usemtl ").Append(groupname + "_techo_mtl").Append("\n");
                for (int i = 0; i < Items[xx].Techo.Count; i++)
                {
                    PositionNormalTextureColor v = Items[xx].Vertices[i];
                    sb.Append(string.Format("v {0} {1} {2}\n", v.Position.Z.ToString().Replace(",", "."), v.Position.Y.ToString().Replace(",", "."), v.Position.X.ToString().Replace(",", ".")));
                }
                for (int i = 0; i < Items[xx].Techo.Count; i++)
                {
                    PositionNormalTextureColor v = Items[xx].Vertices[i];
                    sb.Append(string.Format("vn {0} {1} {2}\n", v.Normal.Z.ToString().Replace(",", "."), v.Normal.Y.ToString().Replace(",", "."), v.Normal.X.ToString().Replace(",", ".")));
                }
                for (int i = 0; i < Items[xx].Techo.Count; i++)
                {
                    PositionNormalTextureColor v = Items[xx].Vertices[i];
                    sb.Append(string.Format("vt {0} {1}\n", v.TextureCoordinate.X.ToString().Replace(",", "."), v.TextureCoordinate.Y.ToString().Replace(",", ".")));
                }
                sb.Append("\n");
                sb.Append("\n");


                for (int i = cur; i < (cur + Items[xx].Techo.Count); i += 3)
                {
                    sb.Append("f " + i + "/" + i + "/" + i + " " + (i + 1) + "/" + (i + 1) + "/" + (i + 1) + " " + (i + 2) + "/" + (i + 2) + "/" + (i + 2) + "\n");
                }

                cur += Items[xx].Techo.Count;


            }


            if (true)
            {

                Vector3 colorito = Color.DarkGray.ToVector3();

                sb2.Append("newmtl " + groupname + "_techo_mtl" + " \n");
                sb2.Append("Ka  " + colorito.X.ToString().Replace(",", ".") + " " + colorito.Y.ToString().Replace(",", ".") + " " + colorito.Z.ToString().Replace(",", ".") + "\n");
                sb2.Append("Kd  " + colorito.X.ToString().Replace(",", ".") + " " + colorito.Y.ToString().Replace(",", ".") + " " + colorito.Z.ToString().Replace(",", ".") + "\n");
                //sb2.Append("Kd  0.0922  0.0166  0.9000 \n");
                sb2.Append("Ks  1 1 1 \n");
                sb2.Append("illum 1 \n");
                sb2.Append("Ns 20 \n\n");

                for (int xx = 0; xx < cantText.Count; xx++)
                {
                    colorito = Color.Magenta.ToVector3();

                    sb2.Append("newmtl " + groupname + "_texture_mtl" + xx.ToString() + " \n");
                    sb2.Append("Ka  1.000 1.000 1.000\n");
                    sb2.Append("Kd  1.000 1.000 1.000\n");
                    sb2.Append("Ks  0.000 0.000 0.000\n");
                    sb2.Append("map_Ka " + texturename + (xx + 1) + ".jpg\n\n");
                }
            }

            using (StreamWriter swmtl = new StreamWriter(path + name.Replace(" ", "_") + ".mtl"))
            {
                swmtl.Write(sb2.ToString());
            }


            sw.Write(sb.ToString());



        }
    }


    public static void ExportVeredas(string name, string path, List<Feature> Items, string groupname, string texturename)
    {

        using (StreamWriter sw = new StreamWriter(path + name.Replace(" ", "_") + ".obj"))
        {

            StringBuilder sb = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();


            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();

            List<Vector3> normals = new List<Vector3>();
            List<Vector2> uv = new List<Vector2>();



            List<int> cantText = new List<int>();


            int cur = 1;


            sb.Append("mtllib ").Append(name.Replace(" ", "_") + ".mtl").Append("\n");

            for (int xx = 0; xx < Items.Count; xx++)
            {

                sb.Append("g " + groupname + "_cordon_").Append(xx.ToString()).Append("\n");

                if (!cantText.Contains(Items[xx].Texture))
                {
                    cantText.Add(Items[xx].Texture);

                }
                sb.Append("usemtl ").Append(groupname + "_cordon_mtl").Append("\n");
                for (int i = Items[xx].Techo.Count; i < Items[xx].Vertices.Length; i++)
                {
                    PositionNormalTextureColor v = Items[xx].Vertices[i];
                    sb.Append(string.Format("v {0} {1} {2}\n", v.Position.Z.ToString().Replace(",", "."), v.Position.Y.ToString().Replace(",", "."), v.Position.X.ToString().Replace(",", ".")));
                }
                for (int i = Items[xx].Techo.Count; i < Items[xx].Vertices.Length; i++)
                {
                    PositionNormalTextureColor v = Items[xx].Vertices[i];
                    sb.Append(string.Format("vn {0} {1} {2}\n", v.Normal.Z.ToString().Replace(",", "."), v.Normal.Y.ToString().Replace(",", "."), v.Normal.X.ToString().Replace(",", ".")));
                }
                for (int i = Items[xx].Techo.Count; i < Items[xx].Vertices.Length; i++)
                {
                    PositionNormalTextureColor v = Items[xx].Vertices[i];
                    sb.Append(string.Format("vt {0} {1}\n", v.TextureCoordinate.X.ToString().Replace(",", "."), v.TextureCoordinate.Y.ToString().Replace(",", ".")));
                }

                for (int i = cur; i < (cur + Items[xx].Paredes.Length); i += 3)
                {
                    sb.Append("f " + i + "/" + i + "/" + i + " " + (i + 1) + "/" + (i + 1) + "/" + (i + 1) + " " + (i + 2) + "/" + (i + 2) + "/" + (i + 2) + "\n");
                }

                cur += Items[xx].Paredes.Length;



                sb.Append("g " + groupname + "_").Append(xx.ToString()).Append("\n");

                sb.Append("usemtl ").Append(groupname + "_texture_mtl_" + Items[xx].Texture.ToString()).Append("\n");
                for (int i = 0; i < Items[xx].Techo.Count; i++)
                {
                    PositionNormalTextureColor v = Items[xx].Vertices[i];
                    sb.Append(string.Format("v {0} {1} {2}\n", v.Position.Z.ToString().Replace(",", "."), v.Position.Y.ToString().Replace(",", "."), v.Position.X.ToString().Replace(",", ".")));
                }
                for (int i = 0; i < Items[xx].Techo.Count; i++)
                {
                    PositionNormalTextureColor v = Items[xx].Vertices[i];
                    sb.Append(string.Format("vn {0} {1} {2}\n", v.Normal.Z.ToString().Replace(",", "."), v.Normal.Y.ToString().Replace(",", "."), v.Normal.X.ToString().Replace(",", ".")));
                }
                for (int i = 0; i < Items[xx].Techo.Count; i++)
                {
                    PositionNormalTextureColor v = Items[xx].Vertices[i];
                    sb.Append(string.Format("vt {0} {1}\n", v.TextureCoordinate.X.ToString().Replace(",", "."), v.TextureCoordinate.Y.ToString().Replace(",", ".")));
                }
                sb.Append("\n");
                sb.Append("\n");


                for (int i = cur; i < (cur + Items[xx].Techo.Count); i += 3)
                {
                    sb.Append("f " + i + "/" + i + "/" + i + " " + (i + 1) + "/" + (i + 1) + "/" + (i + 1) + " " + (i + 2) + "/" + (i + 2) + "/" + (i + 2) + "\n");
                }

                cur += Items[xx].Techo.Count;


            }

            
            Vector3 colorito = Color.DarkGray.ToVector3();

            sb2.Append("newmtl " + groupname + "_cordon_mtl" + " \n");
            sb2.Append("Ka  " + colorito.X.ToString().Replace(",", ".") + " " + colorito.Y.ToString().Replace(",", ".") + " " + colorito.Z.ToString().Replace(",", ".") + "\n");
            sb2.Append("Kd  " + colorito.X.ToString().Replace(",", ".") + " " + colorito.Y.ToString().Replace(",", ".") + " " + colorito.Z.ToString().Replace(",", ".") + "\n");
            //sb2.Append("Kd  0.0922  0.0166  0.9000 \n");
            sb2.Append("Ks  1 1 1 \n");
            sb2.Append("illum 1 \n");
            sb2.Append("Ns 20 \n\n");

            for (int xx = 0; xx < cantText.Count; xx++)
            {
                colorito = Color.Magenta.ToVector3();

                sb2.Append("newmtl " + groupname + "_texture_mtl_" + xx.ToString() + " \n");
                sb2.Append("Ka  1.000 1.000 1.000\n");
                sb2.Append("Kd  1.000 1.000 1.000\n");
                sb2.Append("Ks  0.000 0.000 0.000\n");
                sb2.Append("map_Ka " + texturename + (xx + 1) + ".jpg\n\n");
            }

            using (StreamWriter swmtl = new StreamWriter(path + name.Replace(" ", "_") + ".mtl"))
            {
                swmtl.Write(sb2.ToString());
            }


            sw.Write(sb.ToString());



        }
    }





    internal static void ExportLotes2(string name, string path, List<Feature> Items, string groupname)
    {

        using (StreamWriter sw = new StreamWriter(path + name.Replace(" ", "_") + ".obj"))
        {

            StringBuilder sb = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();


            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<Color> colors = new List<Color>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> uv = new List<Vector2>();

            colors.Add(new Color(0, 0, 0));

            for (int xx = 0; xx < Items.Count; xx++)
            {
                if (!colors.Contains(Items[xx].Color))
                {
                    colors.Add(Items[xx].Color);
                }
            }



            int cur = 1;


            sb.Append("mtllib ").Append(name.Replace(" ", "_") + ".mtl").Append("\n");

            for (int xx = 0; xx < Items.Count; xx++)
            {
                int colorIx = colors.IndexOf(Items[xx].Color);

                if (groupname == "Lotes")
                {

                    sb.Append("g " + groupname + "_paredes_").Append(xx.ToString()).Append("\n");


                    sb.Append("usemtl ").Append(groupname + "_tex_mtl" + colorIx.ToString()).Append("\n");
                    for (int i = Items[xx].Techo.Count; i < Items[xx].Vertices.Length; i++)
                    {
                        PositionNormalTextureColor v = Items[xx].Vertices[i];
                        sb.Append(string.Format("v {0} {1} {2}\n", v.Position.Z.ToString().Replace(",", "."), v.Position.Y.ToString().Replace(",", "."), v.Position.X.ToString().Replace(",", ".")));
                    }
                    for (int i = Items[xx].Techo.Count; i < Items[xx].Vertices.Length; i++)
                    {
                        PositionNormalTextureColor v = Items[xx].Vertices[i];
                        sb.Append(string.Format("vn {0} {1} {2}\n", v.Normal.Z.ToString().Replace(",", "."), v.Normal.Y.ToString().Replace(",", "."), v.Normal.X.ToString().Replace(",", ".")));
                    }
                    for (int i = Items[xx].Techo.Count; i < Items[xx].Vertices.Length; i++)
                    {
                        PositionNormalTextureColor v = Items[xx].Vertices[i];
                        sb.Append(string.Format("vt {0} {1}\n", v.TextureCoordinate.X.ToString().Replace(",", "."), v.TextureCoordinate.Y.ToString().Replace(",", ".")));
                    }

                    for (int i = cur; i < (cur + Items[xx].Paredes.Length); i += 3)
                    {
                        sb.Append("f " + i + "/" + i + "/" + i + " " + (i + 1) + "/" + (i + 1) + "/" + (i + 1) + " " + (i + 2) + "/" + (i + 2) + "/" + (i + 2) + "\n");
                    }

                    cur += Items[xx].Paredes.Length;



                    sb.Append("g " + groupname + "_techo_").Append(xx.ToString()).Append("\n");

                    sb.Append("usemtl ").Append(groupname + "mtl" + colorIx.ToString()).Append("\n");
                    for (int i = 0; i < Items[xx].Techo.Count; i++)
                    {
                        PositionNormalTextureColor v = Items[xx].Vertices[i];
                        sb.Append(string.Format("v {0} {1} {2}\n", v.Position.Z.ToString().Replace(",", "."), v.Position.Y.ToString().Replace(",", "."), v.Position.X.ToString().Replace(",", ".")));
                    }
                    for (int i = 0; i < Items[xx].Techo.Count; i++)
                    {
                        PositionNormalTextureColor v = Items[xx].Vertices[i];
                        sb.Append(string.Format("vn {0} {1} {2}\n", v.Normal.Z.ToString().Replace(",", "."), v.Normal.Y.ToString().Replace(",", "."), v.Normal.X.ToString().Replace(",", ".")));
                    }
                    for (int i = 0; i < Items[xx].Techo.Count; i++)
                    {
                        PositionNormalTextureColor v = Items[xx].Vertices[i];
                        sb.Append(string.Format("vt {0} {1}\n", v.TextureCoordinate.X.ToString().Replace(",", "."), v.TextureCoordinate.Y.ToString().Replace(",", ".")));
                    }
                    sb.Append("\n");
                    sb.Append("\n");


                    for (int i = cur; i < (cur + Items[xx].Techo.Count); i += 3)
                    {
                        sb.Append("f " + i + "/" + i + "/" + i + " " + (i + 1) + "/" + (i + 1) + "/" + (i + 1) + " " + (i + 2) + "/" + (i + 2) + "/" + (i + 2) + "\n");
                    }

                    cur += Items[xx].Techo.Count;


                }
                else
                {

                    sb.Append("g " + groupname).Append(xx.ToString()).Append("\n");


                    sb.Append("usemtl ").Append(groupname + "mtl" + colorIx.ToString()).Append("\n");
                    foreach (PositionNormalTextureColor v in Items[xx].Vertices)
                    {
                        sb.Append(string.Format("v {0} {1} {2}\n", v.Position.Z.ToString().Replace(",", "."), v.Position.Y.ToString().Replace(",", "."), v.Position.X.ToString().Replace(",", ".")));
                    }
                    sb.Append("\n");
                    foreach (PositionNormalTextureColor v in Items[xx].Vertices)
                    {
                        sb.Append(string.Format("vn {0} {1} {2}\n", v.Normal.X.ToString().Replace(",", "."), v.Normal.Y.ToString().Replace(",", "."), v.Normal.Z.ToString().Replace(",", ".")));
                    }

                    foreach (PositionNormalTextureColor v in Items[xx].Vertices)
                    {
                        sb.Append(string.Format("vt {0} {1}\n", v.TextureCoordinate.X.ToString().Replace(",", "."), v.TextureCoordinate.Y.ToString().Replace(",", ".")));
                    }
                    sb.Append("\n");
                    sb.Append("\n");


                    for (int i = cur; i < (cur + Items[xx].Vertices.Length); i += 3)
                    {
                        sb.Append("f " + i + "/" + i + "/" + i + " " + (i + 1) + "/" + (i + 1) + "/" + (i + 1) + " " + (i + 2) + "/" + (i + 2) + "/" + (i + 2) + "\n");
                    }

                    cur += Items[xx].Vertices.Length;

                }


                /*
                foreach (PositionNormalTextureColor v in Items[xx].Vertices)
                {
                    sb.Append(string.Format("v {0} {1} {2}\n", v.Position.Z.ToString().Replace(",", "."), v.Position.Y.ToString().Replace(",", "."), v.Position.X.ToString().Replace(",", ".")));
                }
                sb.Append("\n");
                foreach (PositionNormalTextureColor v in Items[xx].Vertices)
                {
                    sb.Append(string.Format("vn {0} {1} {2}\n", v.Normal.X.ToString().Replace(",", "."), v.Normal.Y.ToString().Replace(",", "."), v.Normal.Z.ToString().Replace(",", ".")));
                }

                foreach (PositionNormalTextureColor v in Items[xx].Vertices)
                {
                    sb.Append(string.Format("vt {0} {1}\n", v.TextureCoordinate.X.ToString().Replace(",", "."), v.TextureCoordinate.Y.ToString().Replace(",", ".")));
                }
                sb.Append("\n");


                for (int i = cur; i < (cur +  Items[xx].Vertices.Length); i += 3)
                {
                    sb.Append("f " + i + "/" + i + "/" + i + " " + (i + 1) + "/" + (i + 1) + "/" + (i + 1) + " " + (i + 2) + "/" + (i + 2) + "/" + (i + 2) + "\n");
                }

                cur += Items[xx].Vertices.Length;

                */

                //sb.Append("usemtl ").Append("mtl" + colorIx.ToString()).Append("\n");

            }


            if (true)
            {

                for (int xx = 0; xx < colors.Count; xx++)
                {
                    Vector3 colorito = colors[xx].ToVector3();

                    sb2.Append("newmtl " + groupname + "mtl" + xx.ToString() + " \n");
                    sb2.Append("Ka  " + colorito.X.ToString().Replace(",", ".") + " " + colorito.Y.ToString().Replace(",", ".") + " " + colorito.Z.ToString().Replace(",", ".") + "\n");
                    sb2.Append("Kd  " + colorito.X.ToString().Replace(",", ".") + " " + colorito.Y.ToString().Replace(",", ".") + " " + colorito.Z.ToString().Replace(",", ".") + "\n");
                    //sb2.Append("Kd  0.0922  0.0166  0.9000 \n");
                    sb2.Append("Ks  1 1 1 \n");
                    sb2.Append("illum 1 \n");
                    sb2.Append("Ns 20 \n");

                }
                if (groupname == "Lotes")
                {
                    for (int xx = 0; xx < colors.Count; xx++)
                    {
                        Vector3 colorito = colors[xx].ToVector3();

                        sb2.Append("newmtl " + groupname + "_tex_mtl" + xx.ToString() + " \n");
                        sb2.Append("Ka  1.000 1.000 1.000\n");
                        sb2.Append("Kd  1.000 1.000 1.000\n");
                        sb2.Append("Ks  0.000 0.000 0.000\n");
                        sb2.Append("map_Ka facade.jpg\n");

                    }
                }

            }
            else
            {

                for (int xx = 0; xx < Items.Count; xx++)
                {

                    int colorIx = colors.IndexOf(Items[xx].Color);
                    Vector3 colorito = Items[xx].Color.ToVector3();

                    sb2.Append("newmtl " + groupname + "mtl" + colorIx.ToString() + " \n");
                    sb2.Append("Ka  " + colorito.X.ToString().Replace(",", ".") + " " + colorito.Y.ToString().Replace(",", ".") + " " + colorito.Z.ToString().Replace(",", ".") + "\n");
                    sb2.Append("Kd  " + colorito.X.ToString().Replace(",", ".") + " " + colorito.Y.ToString().Replace(",", ".") + " " + colorito.Z.ToString().Replace(",", ".") + "\n");
                    //sb2.Append("Kd  0.0922  0.0166  0.9000 \n");
                    sb2.Append("Ks  1 1 1 \n");
                    sb2.Append("illum 1 \n");
                    sb2.Append("Ns 20 \n");

                }
                if (groupname == "Lotes")
                {
                    for (int xx = 0; xx < Items.Count; xx++)
                    {

                        int colorIx = colors.IndexOf(Items[xx].Color);
                        Vector3 colorito = Items[xx].Color.ToVector3();

                        sb2.Append("newmtl " + groupname + "_tex_mtl" + colorIx.ToString() + " \n");
                        sb2.Append("Ka  1.000 1.000 1.000\n");
                        sb2.Append("Kd  1.000 1.000 1.000\n");
                        sb2.Append("Ks  0.000 0.000 0.000\n");
                        sb2.Append("map_Ka facade.jpg\n");

                    }
                }

            }
            using (StreamWriter swmtl = new StreamWriter(path + name.Replace(" ", "_") + ".mtl"))
            {
                swmtl.Write(sb2.ToString());
            }


            sw.Write(sb.ToString());



        }
    }



























    public static string MeshToString(string name, string path, Vector3[] vertices, Vector3[] normals, Vector2[] uv, Color[] colors) {
        //Mesh m = mf.mesh;
        //Material[] mats = mf.renderer.sharedMaterials;

        StringBuilder sb = new StringBuilder();
        StringBuilder sb2 = new StringBuilder();
        int cur = 0;
        if (colors != null) {
            List<Color> cols = new List<Color>();
            if (name.Contains("lotes"))
            {
                cols.Clear();

                Color clr = LayerHelper.GetRandomColor();
                List<Color> clrs = new List<Color>();
                for (int i = 0; i < 10; i++)
                {
                    Color cl = LayerHelper.GetRandomColor();
                    if (!clrs.Contains(cl))
                    {
                        cols.Add(cl);
                    }
                }


                //cols.Add(new Color(245, 0, 10));
            } else {
                for (int xx = 0; xx < colors.Length; xx++) {
                    if (!cols.Contains(colors[xx])) {
                        cols.Add(colors[xx]);
                    }
                }
            }

            sb.Append("mtllib ").Append(name.Replace(" ", "_") + ".mtl").Append("\n");

            for (int xx = 0; xx < cols.Count; xx++) {
                sb.Append("g group").Append(xx.ToString()).Append("\n");
                sb.Append("usemtl ").Append("mtl" + xx.ToString()).Append("\n");
                List<Vector3> newvxs = new List<Vector3>();
                List<Vector3> newnormals = new List<Vector3>();


                for (int c = 0; c < colors.Length; c++) {
                    //if (colors[c] == cols[xx] || name.Contains("lotes")) {
                        newvxs.Add(vertices[c]);
                        newnormals.Add(vertices[c]);
                    //}
                }
                foreach (Vector3 v in newvxs) {
                    sb.Append(string.Format("v {0} {1} {2}\n", v.Z.ToString().Replace(",", "."), v.Y.ToString().Replace(",", "."), v.X.ToString().Replace(",", ".")));
                }
                sb.Append("\n");
                foreach (Vector3 v in newnormals) {
                    sb.Append(string.Format("vn {0} {1} {2}\n", v.X.ToString().Replace(",", "."), v.Y.ToString().Replace(",", "."), v.Z.ToString().Replace(",", ".")));
                }
                sb.Append("\n");

                for (int i = 1; i < newvxs.Count; i += 3) {
                    sb.Append("f " + (i + cur) + "/" + (i + cur) + "/" + (i + cur) + " " + (i + cur + 1) + "/" + (i + cur + 1) + "/" + (i + cur + 1) + " " + (i + cur + 2) + "/" + (i + cur + 2) + "/" + (i + cur +  2) + "\n");
                }

                cur += newvxs.Count;

                sb2.Append("newmtl mtl" + xx.ToString() + " \n");
                
                Random rnd = new Random();
                double dbl = rnd.NextDouble();
                Vector3 vc = cols[xx].ToVector3();
                vc.Normalize();
                sb2.Append("Ka " + vc.X.ToString().Replace(",", ".") + " " + vc.Y.ToString().Replace(",", ".") + " " + vc.Z.ToString().Replace(",", ".") + "\n");
                sb2.Append("Kd ");
                sb2.Append(rnd.NextDouble().ToString().Replace(",", ".")+ " ");
                sb2.Append(rnd.NextDouble().ToString().Replace(",", ".")+ " ");
                sb2.Append(rnd.NextDouble().ToString().Replace(",", ".")+ "");
                sb2.Append("\n");



                sb2.Append("Ks ");
                sb2.Append(rnd.NextDouble().ToString().Replace(",", ".")+ " ");
                sb2.Append(rnd.NextDouble().ToString().Replace(",", ".")+ " ");
                sb2.Append(rnd.NextDouble().ToString().Replace(",", ".")+ "");
                sb2.Append("\n");
                sb2.Append("d 1.000000\n");
                
                sb2.Append("illum 2 \n");
                sb2.Append("Ns 100.2237 \n\n");


            } 
        } else {
            sb.Append("g ").Append(name).Append("\n");
            sb.Append("mtllib ").Append(name.Replace(" ", "_") + ".mtl").Append("\n");
            sb.Append("usemtl ").Append("shinyred").Append("\n");
            foreach (Vector3 v in vertices) {
                sb.Append(string.Format("v {0} {1} {2}\n", v.Z.ToString().Replace(",", "."), v.Y.ToString().Replace(",", "."), v.X.ToString().Replace(",", ".")));
            }
            sb.Append("\n");
            foreach (Vector3 v in normals) {
                sb.Append(string.Format("vn {0} {1} {2}\n", v.X.ToString().Replace(",", "."), v.Y.ToString().Replace(",", "."), v.Z.ToString().Replace(",", ".")));
            }
            sb.Append("\n");
            foreach (Vector2 v in uv) {
                // sb.Append(string.Format("vt {0} {1}\n", v.X.ToString().Replace(",", "."), v.Y.ToString().Replace(",", ".")));
            }
            for (int i = 1; i < vertices.Length ; i += 3) {
                sb.Append("f " + i + "/" + i + "/" + i + " " + (i + 1) + "/" + (i + 1) + "/" + (i + 1) + " " + (i + 2) + "/" + (i + 2) + "/" + (i + 2) + "\n");
            }

            //f v1//vn1 v2//vn2 v3//vn3
            /*
            for (int material = 0; material < m.subMeshCount; material++) {
                sb.Append("\n");
                sb.Append("usemtl ").Append(mats[material].name).Append("\n");
                sb.Append("usemap ").Append(mats[material].name).Append("\n");

                int[] triangles = m.GetTriangles(material);
                for (int i = 0; i < triangles.Length; i += 3) {
                    sb.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
                        triangles[i] + 1, triangles[i + 1] + 1, triangles[i + 2] + 1));
                }
            }*/

            sb2.Append("newmtl shinyred \n");
            sb2.Append("Ka  0.9986  0.9000  0.9000 \n");
            sb2.Append("Kd  0.0922  0.0166  0.9000 \n");
            sb2.Append("Ks  0.5974  0.9084  0.2084 \n");
            sb2.Append("illum 2 \n");
            sb2.Append("Ns 0.5237 \n");
        }
        using (StreamWriter sw = new StreamWriter(path + name.Replace(" ", "_") + ".mtl")) {
            sw.Write(sb2.ToString());
        }

        return sb.ToString();
    }

    public static void MeshToFile(string name, string path, Vector3[] vertices, Vector3[] normals, Vector2[] uv) {
        using (StreamWriter sw = new StreamWriter(path + name + ".obj")) {
            sw.Write(MeshToString(name, path, vertices, normals, uv, null));
        }
    }
    public static void MeshToFile(string name, string path, Vector3[] vertices, Vector3[] normals, Vector2[] uv, Color[] colors)
    {
        using (StreamWriter sw = new StreamWriter(path + name + ".obj"))
        {
            sw.Write(MeshToString(name, path, vertices, normals, uv, colors));
        }
    }
    public static void MeshToFileLotes(string name, string path, List<Feature> Items, Vector3[] normals)
    {
        using (StreamWriter sw = new StreamWriter(path + name + ".obj"))
        {

            StringBuilder sb = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();
            int cur = 0;

            List<Color> cols = new List<Color>();

            for (int i = 0; i < Items.Count; i++)
            {
                if (!cols.Contains(Items[i].Color))
                {
                    cols.Add(Items[i].Color);
                }
            }

            sb.Append("mtllib ").Append(name + ".mtl").Append("\n");

            for (int xx = 0; xx < cols.Count; xx++)
            {
                List<Vector3> newvxs = new List<Vector3>();
                List<Vector3> newnormals = new List<Vector3>();


                for (int c = 0; c < Items.Count; c++)
                {
                    if (cols[xx] == Items[c].Color) {


                        for (int xp = 0; xp < Items[xx].Vertices.Length; xp++)
                        {
                            newvxs.Add(Items[xx].Vertices[xp].Position);
                            newnormals.Add(Items[xx].Vertices[xp].Normal);
                            //uvs.Add(Items[xx].Vertices[xp].TextureCoordinate);
                            //colors.Add(Items[xx].Vertices[xp].Color);
                        }
                    }
                }
                if(newvxs.Count == 0)
                {
                    continue;
                }


                sb.Append("g group").Append(xx.ToString()).Append("\n");
                sb.Append("usemtl ").Append("mtl" + xx.ToString()).Append("\n");
                foreach (Vector3 v in newvxs)
                {
                    sb.Append(string.Format("v {0} {1} {2}\n", v.Z.ToString().Replace(",", "."), v.Y.ToString().Replace(",", "."), v.X.ToString().Replace(",", ".")));
                }
                sb.Append("\n");
                foreach (Vector3 v in newnormals)
                {
                    sb.Append(string.Format("vn {0} {1} {2}\n", v.X.ToString().Replace(",", "."), v.Y.ToString().Replace(",", "."), v.Z.ToString().Replace(",", ".")));
                }
                sb.Append("\n");

                for (int i = 1; i < newvxs.Count; i += 3)
                {
                    sb.Append("f " + (i + cur) + "/" + (i + cur) + "/" + (i + cur) + " " + (i + cur + 1) + "/" + (i + cur + 1) + "/" + (i + cur + 1) + " " + (i + cur + 2) + "/" + (i + cur + 2) + "/" + (i + cur + 2) + "\n");
                }

                //cur += newvxs.Count;
                cur = 0;

                sb2.Append("newmtl mtl" + xx.ToString() + " \n");

                Random rnd = new Random();
                double dbl = rnd.NextDouble();
                Vector3 vc = cols[xx].ToVector3();
                vc.Normalize();
                sb2.Append("Ka " + vc.X.ToString().Replace(",", ".") + " " + vc.Y.ToString().Replace(",", ".") + " " + vc.Z.ToString().Replace(",", ".") + "\n");
                sb2.Append("Kd " + vc.X.ToString().Replace(",", ".") + " " + vc.Y.ToString().Replace(",", ".") + " " + vc.Z.ToString().Replace(",", ".") + "\n");
                /*sb2.Append("Kd ");
                sb2.Append(rnd.NextDouble().ToString().Replace(",", ".") + " ");
                sb2.Append(rnd.NextDouble().ToString().Replace(",", ".") + " ");
                sb2.Append(rnd.NextDouble().ToString().Replace(",", ".") + "");
                */
                sb2.Append("\n");



                sb2.Append("Ks 1000");
                //sb2.Append(rnd.NextDouble().ToString().Replace(",", ".") + " ");
                //sb2.Append(rnd.NextDouble().ToString().Replace(",", ".") + " ");
                //sb2.Append(rnd.NextDouble().ToString().Replace(",", ".") + "");
                sb2.Append("\n");
                sb2.Append("d 1.000000\n");

                sb2.Append("illum 1 \n");
                sb2.Append("Ns 100.2237 \n\n");


            }
            using (StreamWriter sw2 = new StreamWriter(path + name + ".mtl"))
            {
                sw2.Write(sb2.ToString());
            }

            sw.Write(sb.ToString());
        }
    }

}
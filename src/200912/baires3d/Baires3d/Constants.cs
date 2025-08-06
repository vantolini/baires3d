using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace b3d
{
    public static class Intersection
    {
        // triangle intersect from http://www.graphics.cornell.edu/pubs/1997/MT97.pdf
        public static bool RayTriangleIntersect(Ray r,
                                                Vector3 vert0, Vector3 vert1, Vector3 vert2,
                                                out float distance)
        {
            distance = 0;

            Vector3 edge1 = vert1 - vert0;
            Vector3 edge2 = vert2 - vert0;

            Vector3 tvec, pvec, qvec;
            float det, inv_det;

            pvec = Vector3.Cross(r.Direction, edge2);

            det = Vector3.Dot(edge1, pvec);

            if (det > -0.00001f)
                return false;

            inv_det = 1.0f/det;

            tvec = r.Position - vert0;

            float u = Vector3.Dot(tvec, pvec)*inv_det;
            if (u < -0.001f || u > 1.001f)
                return false;

            qvec = Vector3.Cross(tvec, edge1);

            float v = Vector3.Dot(r.Direction, qvec)*inv_det;
            if (v < -0.001f || u + v > 1.001f)
                return false;

            distance = Vector3.Dot(edge2, qvec)*inv_det;

            if (distance <= 0)
                return false;

            return true;
        }
    }

    public static class Constants
    {
        /*public static float Divisor = 500f;
        public static float Restar = 128959.6f;
        */

        public static RoundLineManager LineManager;
        

        public static float AngleAdd = 0.175f;
        public static float Angle2 = 0.175f;


        public static float Divisor = 100f;
        public static float Restar = 1f; //346084.125


        public static float pitch = 0f;
        public static float heading = 0f;

        public static Render2D Render2D;
        public static Render3D Render3D;
        public static Logger2d Logger;


        public static string DataPath = @"Data\";
        public static Settings Settings;

        public static void LoadSettings()
        {
            //if(!LoadSettingsFromFile()){
            LoadDefaultSettings();
            //}

            SaveSettings();
        }

        private static void LoadDefaultSettings()
        {
            Settings = new Settings();
            string[] BarriosNames = Constants.Barrios;
            for (int b = 0; b < Barrios.Length; b++)
            {
                //Settings.Layers.Add(new SettingsLayer(DataPath + @"Mesh\\Ciudades\\Capital Federal\\Manzanas\\" + Barrios[b] + ".bin", true, true, ComponentType.Mesh));
            }
            Settings.Components.Camera.Position = new Vector3( -3.45312953f, 0, -5.829659f);

            Settings.Layers.Add(new SettingsLayer(DataPath + @"Ciudades\\Capital Federal\\Manzanas.bin", true, true, ComponentType.Mesh));
            Settings.Layers.Add(new SettingsLayer(DataPath + @"Ciudades\\Capital Federal\\Lotes.bin", true, true, ComponentType.Mesh));
            Settings.Layers.Add(new SettingsLayer(DataPath + @"Ciudades\\Capital Federal\\Barrios.bin", true, true, ComponentType.Mesh));
            Settings.Layers.Add(new SettingsLayer(DataPath + @"Ciudades\\Capital Federal\\Veredas.bin", true, true, ComponentType.Mesh));
            //Settings.Layers.Add(new SettingsLayer(DataPath + @"Provincias.bin", true, true, ComponentType.Mesh));
            Settings.Layers.Add(new SettingsLayer(DataPath + @"Points.bin", true, true, ComponentType.Point));

            //Settings.Layers.Add(new SettingsLayer(DataPath + @"Terrain.bin", true, true, ComponentType.Mesh));

            Settings.Layers.Add(new SettingsLayer(DataPath + @"Calles.bin", true, true, ComponentType.Streets));


            //Settings.Layers.Add(new SettingsLayer(DataPath + @"Mesh\\Veredas\\Veredas.Info.bin"));
            //Settings.Layers.Add(new SettingsLayer(DataPath + @"Mesh\\Algramo\\Algramo.Info.bin"));
            //Settings.Layers.Add(new SettingsLayer(DataPath + @"Mesh\\Piso\\Piso.Info.bin"));
            //Settings.Layers.Add(new SettingsLayer(DataPath + @"Mesh\\Veredas_Almagro\\Veredas_Almagro.Info.bin"));
            //Settings.Layers.Add(new SettingsLayer(DataPath + @"Point\\Bomberos\\Bomberos.Info.bin", true, true, ComponentType.Point));
        }

        private static bool LoadSettingsFromFile()
        {
            if (File.Exists("Settings.bin"))
            {
                Settings = new Settings();
                IFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream("Settings.bin", FileMode.Open, FileAccess.Read, FileShare.None);

                Settings = (Settings) formatter.Deserialize(stream);
                stream.Close();

                return true;
            }
            else
            {
                return false;
            }
        }

        public static void SaveSettings()
        {
            IFormatter formatter2 = new BinaryFormatter();
            FileStream stream = new FileStream("Settings.bin", FileMode.Create, FileAccess.Write, FileShare.None);

            formatter2.Serialize(stream, Settings);
            stream.Close();
        }


        private static Random r = new Random(666);

        public static Color GetRandomColor()
        {
            byte ColorR = (byte) r.Next(0, 256);
            byte ColorG = (byte) r.Next(0, 256);
            byte ColorB = (byte) r.Next(0, 256);
            return new Color(ColorR, ColorG, ColorB);
        }

        public static Vector2 Get2DCoords(Vector3 myPosition)
        {
            Matrix ViewProjectionMatrix = Constants.Camera.View*Constants.Camera.Projection;

            Vector4 result4 = Vector4.Transform(myPosition, ViewProjectionMatrix);
            if (result4.W == 0)
                result4.W = 0.000001f;

            var result = new Vector3(result4.X/result4.W, result4.Y/result4.W, result4.Z/result4.W);

            var retVal =
                new Vector2(
                    (int) Math.Round(+result.X*(Constants.Window.ClientBounds.Width/2)) +
                    (Constants.Window.ClientBounds.Width/2),
                    (int) Math.Round(-result.Y*(Constants.Window.ClientBounds.Height/2)) +
                    (Constants.Window.ClientBounds.Height/2));
            return retVal;
        }

        public static TextureManager TextureManager;
        public static CameraManager Camera;
        public static GameWindow Window;
        public static GraphicsDevice GraphicsDevice;
        public static b3d ar3d;


        public static string[] Provincias = {
            "Buenos Aires",
            "Catamarca",
            "Chaco",
            "Chubut",
            "Córdoba",
            "Corrientes",
            "Entre Ríos",
            "Formosa",
            "Jujuy",
            "La Pampa",
            "La Rioja",
            "Mendoza",
            "Misiones",
            "Neuquén",
            "Río Negro",
            "Salta",
            "San Juan",
            "San Luis",
            "Santa Cruz",
            "Santa Fe",
            "Santiago del Estero", "Tucumán",
            "Tierra del Fuego"
        };


        public static string[] Barrios = {
            // "lotes_Retiro", "Calles",
            "Agronomia", "Almagro", "Balvanera", "Barracas", "Belgrano", "Boedo",
            "Caballito", "Chacarita", "Coghlan", "Colegiales", "Constitucion",
            "Flores", "Floresta", "La Boca", "Liniers", "Mataderos",
            "Monte Castro", "Montserrat", "Nueva Pompeya", "Nuñez", "Palermo",
            "Parque Avellaneda", "Parque Chacabuco", "Parque Chas",
            "Parque Patricios", "Paternal", "Puerto Madero", "Recoleta", "Retiro",
            "Saavedra", "San Cristobal", "San Nicolas", "San Telmo", "Velez Sarsfield",
            "Versalles", "Villa Crespo", "Villa del Parque", "Villa Devoto",
            "Villa General Mitre", "Villa Lugano", "Villa Luro", "Villa Ortuzar",
            "Villa Pueyrredon", "Villa Real", "Villa Riachuelo",
            "Villa Santa Rita", "Villa Soldati", "Villa Urquiza"
        };


        public static string[] Lotes = {
            "lotes_Agronomia",
            "lotes_Almagro",
            "lotes_Balvanera",
            "lotes_Barracas",
            "lotes_Belgrano",
            "lotes_Boedo",
            "lotes_Caballito",
            "lotes_Chacarita",
            "lotes_Coghlan",
            "lotes_Colegiales",
            "lotes_Constitucion",
            "lotes_Flores",
            "lotes_Floresta",
            "lotes_La Boca",
            "lotes_Liniers",
            "lotes_Mataderos",
            "lotes_Monte Castro",
            "lotes_Montserrat",
            "lotes_Nueva Pompeya",
            "lotes_Nuñez",
            "lotes_Palermo",
            "lotes_Parque Chacabuco",
            "lotes_Parque Chas",
            "lotes_Parque Patricios",
            "lotes_Parque Avellaneda",
            "lotes_Paternal",
            "lotes_Puerto Madero",
            "lotes_Recoleta",
            "lotes_Retiro",
            "lotes_Saavedra",
            "lotes_San Cristobal",
            "lotes_San Nicolas",
            "lotes_San Telmo",
            "lotes_Velez Sarsfield",
            "lotes_Versalles",
            "lotes_Villa Crespo",
            "lotes_Villa del Parque",
            "lotes_Villa Devoto",
            "lotes_Villa General Mitre",
            "lotes_Villa Lugano",
            "lotes_Villa Luro",
            "lotes_Villa Ortuzar",
            "lotes_Villa Pueyrredon",
            "lotes_Villa Real",
            "lotes_Villa Riachuelo",
            "lotes_Villa Santa Rita",
            "lotes_Villa Soldati",
            "lotes_Villa Urquiza"
        };
    }
}
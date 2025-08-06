using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace b3d
{
    public class Icono{
        public string Name;
        public Texture2D Image;
    }

    public class TextureManager
    {
        public List<IconoCollection> Iconos = new List<IconoCollection>();
        public enum TextureColor
        {
            Red,
            Blue,
            Orange
        } ;

        public Texture2D noiseImage;

        public Texture2D[] coloredTextures = new Texture2D[5];
        public Texture2D[] imgEdificios;
        public Texture2D[] imgNegocios;

        public Texture2D[] imgIconos;

        public Texture2D nullTexture;
        public Texture2D getIconByName(string name) {
            if(Iconos[0].Exists(name.ToLower())){
                return Iconos[0][name.ToLower()].Image;
            }
            return Iconos[0]["unknown"].Image;
        }

        public void LoadTextures()
        {
            string[] defaultCollectionFiles = Directory.GetFiles("Content\\Iconos\\Default\\", "*.xnb");
            IconoCollection defaultCollection = new IconoCollection();
            for (int i = 0; i < defaultCollectionFiles.Length; i++)
            {
                Icono icono = new Icono();
                icono.Image = Constants.ar3d.Content.Load<Texture2D>(defaultCollectionFiles[i].Replace(".xnb", "").Replace("Content\\", "")); // 14
                icono.Name = Path.GetFileNameWithoutExtension(defaultCollectionFiles[i]);
                defaultCollection.Add(icono);
            }

            Iconos.Add(defaultCollection);
            
            /*
             \\Bomberos.shp"
    [1]: "..\\Work\\shp\\Mesh\\Point\\Clinicas.shp"
    [2]: "..\\Work\\shp\\Mesh\\Point\\Escuelas.shp"
    [3]: "..\\Work\\shp\\Mesh\\Point\\Iglesias.shp"
    [4]: "..\\Work\\shp\\Mesh\\Point\\Points_attraction.shp"
    *[5]: "..\\Work\\shp\\Mesh\\Point\\Points_bank.shp"
    *[6]: "..\\Work\\shp\\Mesh\\Point\\Points_bar.shp"
    [7]: "..\\Work\\shp\\Mesh\\Point\\Points_bus_station.shp"
    [8]: "..\\Work\\shp\\Mesh\\Point\\Points_bus_stop.shp"
    [9]: "..\\Work\\shp\\Mesh\\Point\\Points_cafe.shp"
    [10]: "..\\Work\\shp\\Mesh\\Point\\Points_education.shp"
    [11]: "..\\Work\\shp\\Mesh\\Point\\Points_embassy.shp"
    [12]: "..\\Work\\shp\\Mesh\\Point\\Points_fast_food.shp"
    [13]: "..\\Work\\shp\\Mesh\\Point\\Points_fire_station.shp"
    [14]: "..\\Work\\shp\\Mesh\\Point\\Points_fuel.shp"
    *[15]: "..\\Work\\shp\\Mesh\\Point\\Points_hospital.shp"
    *[16]: "..\\Work\\shp\\Mesh\\Point\\Points_hostel.shp"
    *[17]: "..\\Work\\shp\\Mesh\\Point\\Points_hotel.shp"
    [18]: "..\\Work\\shp\\Mesh\\Point\\Points_memorial.shp"
    [19]: "..\\Work\\shp\\Mesh\\Point\\Points_monument.shp"
    [20]: "..\\Work\\shp\\Mesh\\Point\\Points_museum.shp"
    [21]: "..\\Work\\shp\\Mesh\\Point\\Points_parking.shp"
    [22]: "..\\Work\\shp\\Mesh\\Point\\Points_pharmacy.shp"
    [23]: "..\\Work\\shp\\Mesh\\Point\\Points_place_of_worship.shp"
    [24]: "..\\Work\\shp\\Mesh\\Point\\Points_police.shp"
    [25]: "..\\Work\\shp\\Mesh\\Point\\Points_public_building.shp"
    *[26]: "..\\Work\\shp\\Mesh\\Point\\Points_restaurant.shp"
    [27]: "..\\Work\\shp\\Mesh\\Point\\Points_school.shp"
    [28]: "..\\Work\\shp\\Mesh\\Point\\Points_station.shp"
    [29]: "..\\Work\\shp\\Mesh\\Point\\Points_theatre.shp"
    [30]: "..\\Work\\shp\\Mesh\\Point\\Points_university.shp"

             */
            /*
           nullTexture = new Texture2D(Constants.GraphicsDevice, 1, 1, 0,
                       TextureUsage.None, SurfaceFormat.Color);

           Color[] pixels = { Color.Black };

           nullTexture.SetData(pixels);

           


           coloredTextures[0] = new Texture2D(Constants.GraphicsDevice, 1, 1, 0,
       TextureUsage.None, SurfaceFormat.Color);
           coloredTextures[1] = new Texture2D(Constants.GraphicsDevice, 1, 1, 0,
                   TextureUsage.None, SurfaceFormat.Color);
           coloredTextures[2] = new Texture2D(Constants.GraphicsDevice, 1, 1, 0,
               TextureUsage.None, SurfaceFormat.Color);
           coloredTextures[3] = new Texture2D(Constants.GraphicsDevice, 1, 1, 0,
               TextureUsage.None, SurfaceFormat.Color);
           coloredTextures[4] = new Texture2D(Constants.GraphicsDevice, 1, 1, 0,
           TextureUsage.None, SurfaceFormat.Color);

           Color[] red = { Color.Red };
           Color[] blue = { Color.Blue };
           Color[] green = { Color.Green };
           Color[] yellow = { Color.Yellow };
           Color[] orange = { new Color(255, 178, 28) };

           //Color pepe = new Color(255, 218, 163);
           coloredTextures[0].SetData(red);
           coloredTextures[1].SetData(blue);
           coloredTextures[2].SetData(green);
           coloredTextures[3].SetData(yellow);
           coloredTextures[4].SetData(orange);
                
            
           Random rand = new Random();
           int resolution = 512;
           Color[] noisyColors = new Color[resolution * resolution];
           for (int x = 0; x < resolution; x++)
               for (int y = 0; y < resolution; y++)
                   noisyColors[x + y * resolution] = new Color(new Vector3((float)rand.Next(1000) / 1000.0f, 0, 0));

           noiseImage = new Texture2D(Constants.GraphicsDevice, resolution, resolution, 1, TextureUsage.None, SurfaceFormat.Color);
           noiseImage.SetData(noisyColors);*/
        }
    }
}
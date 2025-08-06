using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace b3d {
    public class CapitalFederal : Ciudad {

        public CapitalFederal(){
            Barrios = new BarriosCollection();
        }

        public CapitalFederal(string name) {
            Barrios = new BarriosCollection();
            Name = name;
        }

        public CapitalFederal(string name, string loadFrom) {
            Barrios = new BarriosCollection();
            Name = name;
            LoadFrom(loadFrom);
        }

        public override void Draw(GameTime gameTime){
            Barrios.Draw(gameTime);

            if (Extra != null){
                for (int i = 0; i < Extra.Count; i++){
                    Extra[i].Draw(gameTime);
                }
            }
        }

        public override void Draw2d(GameTime gameTime) {
            if(Barrios.Enabled){
                Barrios.Draw2d(gameTime);
            }
            
            if (Calles.Enabled)
            {
                Calles.Draw2d(gameTime);
            }
        }

        public override void Draw(GameTime gameTime, Effect effect, bool b){
            effect.Begin();
            effect.Techniques[0].Passes[0].Begin();
            if (b)
                effect.Parameters["Texture"].SetValue(Constants.TextureManager.Iconos[0][0].Image);

            Barrios.Draw(gameTime);

            effect.Techniques[0].Passes[0].End();
            effect.End();
        }

        public override void Update(GameTime gameTime){
            throw new NotImplementedException();
        }

        public override void Initialize(){
            throw new NotImplementedException();
        }

        public override void Dispose(){
            throw new NotImplementedException();
        }

        public override sealed void LoadFrom(string fileName){
            CityReader rdr = new CityReader(fileName);
            rdr.Process();
            Barrios = rdr.Barrios;
            Calles = rdr.Calles;
            Puntos = rdr.Points;

            Calles.Enabled = true;
            Puntos.Enabled = true;
            Barrios.Enabled = true;
            
            if (File.Exists(@"Data\Ciudades\Capital Federal\Calles\Intersecciones.7z"))
            {
                StreetManager = rdr.DeserializeIntersections(@"Data\Ciudades\Capital Federal\Calles\Intersecciones.7z");
            }

            Vector3 bb = Barrios[1].Mesh.BoundingSphere.Center;
            bb.Y = 71f;
            Constants.Camera.Position = new Vector3(bb.X, bb.Y, bb.Z);
        }
    }
}

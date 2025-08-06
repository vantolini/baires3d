using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace b3d{
    public class Manzana : RenderableComponent {
        public string Codigo;
        public MeshComponentPart Silueta;
        public List<Lote> Lotes;

        #region Overrides of RenderableComponent

        public override void Draw(GameTime gameTime){
            if (Constants.Camera.Position.Y > 100 || Lotes == null) {
                Silueta.Draw(gameTime);    
            }else{
                for(int i = 0; i < Lotes.Count;i++){
                    Lotes[i].Draw(gameTime);
                }
            }
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

        #endregion
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace b3d{
    public class Lote : MeshComponentPart {

        public string Codigo;


        #region Overrides of RenderableComponent

        public override void Draw(GameTime gameTime){
            
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
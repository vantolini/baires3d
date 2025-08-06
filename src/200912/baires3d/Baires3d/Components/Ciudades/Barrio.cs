using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace b3d{
    [System.Diagnostics.DebuggerDisplay("Barrio {Name}, vis: {Visible} {BoundingSphere.Center}")]
    public class Barrio:RenderableComponent {

        public MeshComponent Mesh;
        public ManzanasCollection Manzanas;
        public MeshComponent Lotes;
        public Barrio(string name):base(name){
            
        }

        #region Overrides of RenderableComponent

        public override void Draw(GameTime gameTime){
            switch (LOD){
                case LOD.Near:
                    if (Lotes != null){
                        Constants.GraphicsDevice.VertexDeclaration = Lotes.vertexDeclaration;
                        Lotes.Draw(gameTime);
                    }else{
                        Manzanas.Draw(gameTime);
                    }

                    break;
                case LOD.Middle:
                    Manzanas.Draw(gameTime);
                    break;
                case LOD.Far:
                    Mesh.Draw(gameTime);
                    break;
            }
        }
        public void CreateBounds() {
            BoundingSphere = Mesh.BoundingSphere;
            BoundingBox = BoundingBox.CreateFromSphere(BoundingSphere);
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

        internal void AddManzanas(List<Manzana> manzanas, MeshComponent mz) {

            if(Manzanas == null){
                Manzanas = new ManzanasCollection();
            }
            for(int i = 0; i < manzanas.Count;i++){
                Manzanas.Add(manzanas[i]);
            }
            Manzanas.Mesh = mz;
            Manzanas.Mesh.BuildBigBuffer();
        }

        public void Draw2d(GameTime time){
            switch (LOD){
                case LOD.Near:
                    DrawText(BoundingSphere.Center, Name + " | near, dist: " + this.Distance);
                    break;
                case LOD.Middle:
                    DrawText(BoundingSphere.Center, Name + " | mid, dist: " + this.Distance);
                    break;
                case LOD.Far:
                    DrawText(BoundingSphere.Center, Name + " | far, dist: " + this.Distance);
                    break;

            }
        }

        public void DrawText(Vector3 pos, string texto) {
            Vector3 center = Constants.ar3d.GraphicsDevice.Viewport.Project(
                pos,
                Constants.Camera.Projection,
                Constants.Camera.View,
                Matrix.Identity
                );
            if (Constants.Camera.Projection.Forward.Z < center.Z) {
                return;
            }

            Vector2 vc = Constants.Get2DCoords(pos);
            Vector2 textSize = Constants.Render2D.Fonts[2].MeasureString(texto);

            //int centerXPosition = (Constants.ar3d.graphics.PreferredBackBufferWidth / 2) - ((int)textSize.X / 2);
            vc.X -= textSize.X / 3;
            //Constants.Render2D.sb.DrawString(Constants.Render2D.Fonts[2], texto, new Vector2(centerXPosition, vc.Y), Color.Black);
            //float sumar = Constants.Camera.Position.Y / 1000;
            Constants.Render2D.DrawString(texto, vc, Color.Black, 0.8f, 0, 0.99999f, 2);
        }


    }
}
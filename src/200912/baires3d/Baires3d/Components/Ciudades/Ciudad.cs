using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace b3d{
    public abstract class Ciudad : RenderableComponent {
        public PuntosCollection Puntos;
        public BarriosCollection Barrios;
        public CallesCollection Calles;
        public List<MeshComponentPart> Extra;


        public StreetManager StreetManager;

        public abstract void LoadFrom(string fileName);
        public abstract void Draw2d(GameTime time);

        public abstract void Draw(GameTime gameTime, Effect effect, bool b);
    }
}
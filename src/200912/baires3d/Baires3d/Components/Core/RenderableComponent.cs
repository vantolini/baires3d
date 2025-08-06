using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace b3d{
    
    public enum LOD{
        Near,
        Middle,
        Far
    }

    [System.Diagnostics.DebuggerDisplay("{Type}: {Name}, vis: {Visible} {BoundingSphere.Center}")]
    public abstract class RenderableComponent : IRenderable
    {
        public Color Color;
        public float Alpha;
        public bool Pickable;

        protected internal string Name;

        protected internal LOD LOD;
        protected internal int VertexStart;
        protected internal int VertexEnd;
       
        protected internal Vector3 Position;
        public int ID;
        public BoundingBox BoundingBox;
        public BoundingSphere BoundingSphere;
        public float Distance;
        public bool Visible;
        public bool Enabled;

        public float Size;

        public string FilePath;
        public string FileName;

        public Vector2 Position2D;

        public ComponentType Type;

        public int TextureIndex;
        public abstract void Draw(GameTime gameTime);
        public abstract void Update(GameTime gameTime);
        public abstract void Initialize();
        public abstract void Dispose();

        protected RenderableComponent()
        {
        }

        protected RenderableComponent(string name)
        {
            Name = name;
        }

        protected RenderableComponent(string name, Vector3 position)
        {
            this.Name = name;
            this.Position = position;
        }
    }
}
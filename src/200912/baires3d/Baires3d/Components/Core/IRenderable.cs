using Microsoft.Xna.Framework;


namespace b3d
{
    internal interface IRenderable : IComponent
    {
        new void Initialize();
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime);
    }
}
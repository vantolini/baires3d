using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace b3d{
    public class LabelComponent : RenderableComponent
    {
        public string Label;
        public float Angle;

        public override void Initialize()
        {
        }

        public override void Dispose()
        {
        }

        public override void Update(GameTime gameTime)
        {
        }

        public LabelComponent()
        {
        }

        public void CreateBounds()
        {
            BoundingBox = BoundingBox.CreateFromPoints(new Vector3[] {Position});
            BoundingSphere = BoundingSphere.CreateFromBoundingBox(BoundingBox);
        }

        public override void Draw(GameTime gameTime)
        {
            float dist = Vector3.Distance(Position, Constants.Camera.Position);
            if (dist > 50) return;

            float siz = MathHelper.Min(MathHelper.Max(dist/16, 0.8f), 1.1f);
            Vector3 center = Constants.ar3d.GraphicsDevice.Viewport.Project(
                Position,
                Constants.Camera.Projection,
                Constants.Camera.View,
                Matrix.Identity
                );

            if (Constants.Camera.Projection.Forward.Z < center.Z)
            {
                return;
            }

            CreateBounds();

            Position2D = Constants.Get2DCoords(Position);
            Constants.Render2D.DrawAvenida(
                Label, // + " | " + dist,
                Position2D,
                Color.White,
                siz,
                (float) Angle + Constants.Camera.Yaw,
                0.8f);
        }
    }
}
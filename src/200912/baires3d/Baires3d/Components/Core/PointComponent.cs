using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace b3d{
    public class PointComponent : RenderableComponent
    {
        public string Url;
        public List<string> Data;
        public string ImageUrl;

        public override void Initialize()
        {
        }

        public override void Dispose()
        {
        }

        public override void Update(GameTime gameTime)
        {
        }

        public PointComponent()
        {
            this.Type = ComponentType.Point;
        }

        public void CreateBounds()
        {
            BoundingBox = BoundingBox.CreateFromPoints(new Vector3[] {Position});
            BoundingSphere = BoundingSphere.CreateFromBoundingBox(BoundingBox);
        }

        public override void Draw(GameTime gameTime)
        {
        }
    }
}
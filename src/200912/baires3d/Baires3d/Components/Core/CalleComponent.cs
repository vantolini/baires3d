using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace b3d{
    public class CalleComponent : RenderableComponent
    {
        public List<string> Data = new List<string>();
        public int Importance = 0;
        public string StreetType = "";

        public List<CalleTramoComponent> Tramos = new List<CalleTramoComponent>();
        public List<LabelComponent> Labels = new List<LabelComponent>();

        public List<RoundLine> Lines = new List<RoundLine>();

        public override void Initialize()
        {
        }

        public override void Dispose()
        {
        }

        public override void Update(GameTime gameTime)
        {
        }

        public CalleComponent()
        {
            this.Type = ComponentType.Streets;
        }

        internal void Init()
        {
            for(int i = 0; i < Tramos.Count;i++){
                Lines.AddRange(Tramos[i].Lines);
            }

        }

        public void CreateBounds()
        {
            BoundingSphere = Tramos[0].BoundingSphere;

            for (int i = 1; i < Tramos.Count; i++)
            {
                BoundingSphere = BoundingSphere.CreateMerged(BoundingSphere,
                                                             Tramos[i].BoundingSphere);
            }

            BoundingBox = BoundingBox.CreateFromSphere(BoundingSphere);
        }


        BasicEffect basicEffect;
        Texture2D checkerBlue;

        public override void Draw(GameTime gameTime)
        {
            float time = (float)gameTime.TotalRealTime.TotalSeconds;
            Constants.LineManager.Draw(ref Lines, 20f, this.Color, Constants.Camera.View * Constants.Camera.Projection,
                                       time, "NoBlur");
        }

        private void SetLabelPos()
        {
        }

        private float LastAngle;

        public bool Draw2d(GameTime gameTime)
        {
            List<string> Drawn = new List<string>();

            for (int i = 0; i < Tramos.Count; i++)
            {
                Tramos[i].Distance = Vector3.Distance(
                    Constants.Camera.Position,
                    Tramos[i].BoundingSphere.Center);

                if (Tramos[i].Avenida)
                {
                    if (Tramos[i].Distance > 6000f)
                    {
                        continue;
                    }
                }
                else
                {
                    if (Tramos[i].Distance >800f)
                    {
                        continue;
                    }
                }

                if (Drawn.Contains(Tramos[i].Name))
                    continue;

                if (Tramos[i].Draw2d(gameTime))
                    Drawn.Add(Tramos[i].Name);
            }

            return true;
        }
    }
}
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace b3d{
    public class CalleTramoComponent : RenderableComponent
    {
        public List<Vector3> Puntos = new List<Vector3>();
        public LabelComponent Label;
        private Vector3 Closest;

        private Vector3 Previous;

        public bool Avenida = false;

        public CalleTramoComponent()
        {
            this.Label = new LabelComponent();
        }

        private static bool isCloser(
            Vector3 lineA,
            Vector3 lineB)
        {
            if (Vector3.Distance(lineA, Constants.Camera.Position) < Vector3.Distance(lineB, Constants.Camera.Position))
                return true;


            return false;
        }

        private void ClosestPointOnLineSegment()
        {
            Closest = Vector3.Zero;
            for (int i = 0; i < Puntos.Count - 1; i++)
            {
                Vector3 v = Puntos[i + 1] - Puntos[i];
                v.Normalize();
                float t = Vector3.Dot(v, Constants.Camera.Position - Puntos[i]);
                if (t < 0)
                {
                    if (isCloser(Puntos[i], Closest))
                    {
                        Previous = Closest;
                        Closest = Puntos[i];
                    }
                    continue;
                }

                float d = (Puntos[i + 1] - Puntos[i]).Length();
                if (t > d)
                {
                    if (isCloser(Puntos[i + 1], Closest))
                    {
                        Previous = Closest;
                        Closest = Puntos[i + 1];
                    }
                    continue;
                }
                Vector3 a = Puntos[i] + v*t;
                if (isCloser(a, Closest))
                {
                    Previous = Closest;
                    Closest = a;
                }
            }
        }

        public List<RoundLine> Lines = new List<RoundLine>();

        public void Init(){
            for (int c = 0; c < Puntos.Count - 1; c++) {
                Lines.Add(
                    new RoundLine(
                        Puntos[c].X,
                        Puntos[c].Z,
                        Puntos[c + 1].X,
                        Puntos[c + 1].Z
                        ));
            }
        }

        public LineComponent Line;
        public override void Draw(GameTime gameTime){
            Line.Draw(gameTime);
        }

        public bool Draw2d(GameTime gameTime){
            ClosestPointOnLineSegment();
            if (Constants.Camera.Frustum.Contains(Closest)
                != ContainmentType.Contains){
                return false;
            }

            Label.Position = Closest;
            Vector3 center = Constants.ar3d.GraphicsDevice.Viewport.Project(
                Label.Position,
                Constants.Camera.Projection,
                Constants.Camera.View,
                Matrix.Identity
                );

            if (Constants.Camera.Projection.Forward.Z < center.Z){
                return false;
            }

            float Angle = (float) Math.Atan2(
                                      (Label.Position.Z - Previous.Z),
                                      (Label.Position.X - Previous.X));

            Angle = Angle - Constants.Camera.Yaw;

            Position2D = Constants.Get2DCoords(Label.Position);

            float FontSize = Constants.Camera.Position.Y/10*Distance;
            if (Avenida){
                FontSize = 0.87f;
            }else{
                FontSize = 0.83f;
            }
            string final_label = Name;

            if (Angle < 0){
                Angle += 3.1f;
            }

            Vector2 pos2d = Position2D;
            Position2D.Y -= 13;
            if (Avenida) {
                Constants.Render2D.DrawString(
                    final_label,
                    Position2D,
                    Color.Black,
                    FontSize,
                    Angle,
                    0.74f, 1);
            }
            else
            {
                Constants.Render2D.DrawString(
                    final_label,
                    Position2D,
                    Color.Black,
                    FontSize,
                    Angle,
                    0.67f, 0);
            }
            return true;
        }

        public void CreateBounds()
        {
            BoundingBox = BoundingBox.CreateFromPoints(Puntos.ToArray());
            BoundingSphere = BoundingSphere.CreateFromBoundingBox(BoundingBox);
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Initialize()
        {
        }

        public override void Dispose()
        {
        }
    }
}
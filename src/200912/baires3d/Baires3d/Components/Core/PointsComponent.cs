using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace b3d{
    public class PointsComponent : RenderableComponent
    {
        public List<PointComponent> Points = new List<PointComponent>();

        public override void Initialize()
        {
        }

        public override void Dispose()
        {
        }

        public override void Update(GameTime gameTime)
        {
        }

        public PointsComponent()
        {
            Type = ComponentType.Point;
        }

        private bool isVisible(ref BoundingSphere bs)
        {
            return Constants.Camera.Frustum.Contains(
                       bs) != ContainmentType.Disjoint;
        }

        public void CreateBounds()
        {
            BoundingSphere = Points[0].BoundingSphere;

            for (int i = 1; i < Points.Count; i++)
            {
                BoundingSphere = BoundingSphere.CreateMerged(BoundingSphere,
                                                             Points[i].BoundingSphere);
            }

            BoundingBox = BoundingBox.CreateFromSphere(BoundingSphere);
        }

        public override void Draw(GameTime gameTime)
        {

            if (isVisible(ref BoundingSphere))
            {
                Visible = true;
            }
            else
            {
                Visible = false;
                return;
            }

            bool foundPick = false;
            for (int i = 0; i < Points.Count; i++)
            {
                if (Points[i].TextureIndex != 0)
                {
                    //break;
                }
                float calledist = Vector3.Distance(Constants.Camera.Position, Points[i].Position);
               
               if (calledist > 10000f)
               {
                   continue;
               }

                calledist = calledist/3000;
                float call = Math.Min(calledist, 0.33f);
                calledist = Math.Max(call, 0.5f);

                calledist = 1;
                Vector3 center = Constants.ar3d.GraphicsDevice.Viewport.Project(
                    Points[i].Position,
                    Constants.Camera.Projection,
                    Constants.Camera.View,
                    Matrix.Identity
                    );
                if (Constants.Camera.Projection.Forward.Z < center.Z)
                {
                    return;
                }

                float scale = 0.5f + Math.Abs((Constants.Camera.Position.Y *10) - 0.5f);
                Points[i].Position2D = Constants.Get2DCoords(Points[i].Position);
                //Constants.Render2D.DrawIcon(Points[i].TextureIndex, Points[i].Position2D);

                if (foundPick == false &&
                    Points[i].Position2D.X > (Constants.Camera.MousePosition.X - 8) &&
                    Points[i].Position2D.X < (Constants.Camera.MousePosition.X + 14) &&
                    Points[i].Position2D.Y > (Constants.Camera.MousePosition.Y - 25) &&
                    Points[i].Position2D.Y < (Constants.Camera.MousePosition.Y + 6))
                {
                    /*
                    if(origdist < 700){
                    */
                    /*Points[i].Position2D.Y -= 18f;
                        Points[i].Position2D.X -= 30f;
                        */
                    foundPick = true;
                    Constants.Render2D.sb.Draw(
                        Constants.TextureManager.getIconByName(Name),
                        Points[i].Position2D,
                        null, Color.White, 0, Vector2.Zero, calledist*1.4f, SpriteEffects.None, 0f);


                    Points[i].Position2D.Y -= 18f;
                    Points[i].Position2D.X -= 30f;
                    Constants.Render2D.DrawAvenida(
                        Points[i].Name,
                        // + "\nDist: " + dist + "\nangle: " + angle.ToString() + "\nQuat: " + vr.ToString(),
                        Points[i].Position2D,
                        Color.Black,
                        1f,
                        0, 0.5f);

                    Points[i].Position2D.X -= 0.9f;
                    Points[i].Position2D.Y -= 0.9f;
                    Constants.Render2D.DrawAvenida(
                        Points[i].Name, // + " | " + dist,
                        Points[i].Position2D,
                        Color.White,
                        1f,
                        0, 0.8f);
                }
                else
                {
                    Constants.Render2D.sb.Draw(
                        Constants.TextureManager.getIconByName(Name),
                        Points[i].Position2D,
                        null, Color.White, 0, Vector2.Zero, scale , SpriteEffects.None, 0f);
                }
                // Constants.Logger.AddLog("vistoooo" + i);
            }
            //}
        }

        public void Draw2d(GameTime time){
            Draw(time);
        }
    }
}
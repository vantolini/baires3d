using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace b3d
{
    public class CameraManager
    {
        public CameraComponent camera;

        public Vector3 Position
        {
            get { return camera.Position; }
            set { camera.Position = value; }
        }

        private float GetPitch()
        {
            float temp = (-2.0f*(camera.Orientation.X*camera.Orientation.Z - camera.Orientation.W*camera.Orientation.Y));

            temp = MathHelper.Clamp(temp, -1.0f, 1.0f);

            return (float) Math.Asin(temp);
        }

        public float GetYaw()
        {
            return
                (float)
                Math.Atan2(2.0*(camera.Orientation.X*camera.Orientation.Y + camera.Orientation.W*camera.Orientation.Z),
                           camera.Orientation.W*camera.Orientation.W + camera.Orientation.X*camera.Orientation.X -
                           camera.Orientation.Y*camera.Orientation.Y - camera.Orientation.Z*camera.Orientation.Z);
        }

        public float Pitch
        {
            get { return GetPitch(); }
            set { Console.WriteLine(value); }
        }

        public float Yaw
        {
            get { return GetYaw(); }
            set { Console.WriteLine(value); }
        }

        public Vector3 Velocity
        {
            get { return camera.Velocity; }
            set { camera.Velocity = value; }
        }

        public Vector3 Acceleration
        {
            get { return camera.Velocity; }
            set { camera.Acceleration = value; }
        }

        public BoundingFrustum Frustum;

        public Matrix View
        {
            get { return camera.ViewMatrix; }
            set { camera.ViewMatrix = value; }
        }

        public Matrix Projection
        {
            get { return camera.ProjectionMatrix; }
            set { camera.ProjectionMatrix = value; }
        }

        private KeyboardState keyboardInput;
        private Keys lastKeyPressed;
        private double lastTime;
        private MouseState oldMouseInput;
        public MouseState MousePosition;
        public bool Picking = false;
        public bool Picked = false;
        public string PickedObject = "";
        public Ray CurrentRay;

        public Vector3 FlyTo;
        public bool Flying = false;
        private Vector3 Closest;
        private List<Vector3> Puntos = new List<Vector3>();

        private static bool isCloser(Vector3 lineA, Vector3 lineB)
        {
            return (Vector3.Distance(lineA, Constants.Camera.Position) <
                    Vector3.Distance(lineB, Constants.Camera.Position));
        }

        private Vector3 Previous;

        private void ClosestPointOnLineSegment()
        {
            Puntos.Add(Constants.Camera.Position);
            Puntos.Add(FlyTo);
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
            Puntos.Clear();
        }

        public void Fly(Vector3 to)
        {
            FlyTo = to;
            Flying = true;
        }

        public CameraManager()
        {
            Vector3 pos = Constants.Settings.Components.Camera.Position;
            Frustum = new BoundingFrustum(Matrix.Identity);

            Constants.ar3d.windowWidth = Constants.GraphicsDevice.DisplayMode.Width/2;
            Constants.ar3d.windowHeight = Constants.GraphicsDevice.DisplayMode.Height/2;

            Constants.ar3d.aRatio = Constants.ar3d.windowWidth/Constants.ar3d.windowHeight;
            camera = new CameraComponent(Constants.ar3d);

            camera.Perspective(90.0f, Constants.ar3d.aRatio, 0.04f, 4000.0f);
            camera.Position = pos;
            camera.OrbitMaxZoom = 0.00008f;
            camera.OrbitMinZoom = 0.00001f;
        }

        private bool setlook = false;
        public Vector3 PickedPos;
        public VertexPositionColor[] PickedTriangle;

        private Vector2 oldmouse = Vector2.Zero;
        private Vector2 newmouse = Vector2.Zero;
        public void Update(GameTime gameTime)
        {
            if(Picking){
                
                MouseState mst = Mouse.GetState();
                newmouse.X = mst.X;
                newmouse.Y = mst.Y;

                int centerX = Constants.ar3d.Window.ClientBounds.Width / 2;
                int centerY = Constants.ar3d.Window.ClientBounds.Height / 2;
                int deltaX = centerX - mst.X;
                int deltaY = centerY - mst.Y;

                if(Picked && setlook == false){

                }else{
                    setlook = false;
                }
            }else{
                setlook = false;
            }

            camera.Update(gameTime);
            if (Constants.ar3d.isHoldingLeft)
            {
                Flying = true;
            }
            else
            {
                Flying = false;
            }

            MouseState MousePosition = Mouse.GetState();
            if (MousePosition.LeftButton == ButtonState.Pressed
                && Constants.ar3d.IsActive
                //&&(oldMouseInput.LeftButton == ButtonState.Released)
                )
            {
                Vector3 rayNear =
                    Constants.ar3d.GraphicsDevice.Viewport.Unproject(
                        new Vector3(MousePosition.X, MousePosition.Y, 0.0f),
                        Projection, View, Matrix.Identity);
                Vector3 rayEnd =
    Constants.ar3d.GraphicsDevice.Viewport.Unproject(
        new Vector3(MousePosition.X, MousePosition.Y, 1.0f),
        Projection, View, Matrix.Identity);


                if (CurrentRay == null)
                {
                    CurrentRay = new Ray(rayNear, Vector3.Normalize(rayEnd - rayNear));
                }
                else
                {
                    Ray r = new Ray(rayNear, Vector3.Normalize(rayEnd - rayNear));
                    if (r != CurrentRay)
                    {
                        Picked = false;
                        CurrentRay = r;
                    }
                }
                Picking = true;
            }
            else
            {
                Picking = false;
                Picked = false;
            }
        }
    }
}
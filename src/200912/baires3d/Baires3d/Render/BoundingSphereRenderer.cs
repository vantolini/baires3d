using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace b3d{
    public static class BoundingSphereRenderer
    {
        private static VertexBuffer vertBuffer;
        private static VertexDeclaration vertDecl;
        private static BasicEffect effect;
        private static int sphereResolution;

        /// <summary>
        /// Initializes the graphics objects for rendering the spheres. If this method isn't
        /// run manually, it will be called the first time you render a sphere.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device to use when rendering.</param>
        /// <param name="sphereResolution">The number of line segments 
        ///     to use for each of the three circles.</param>
        public static void InitializeGraphics(GraphicsDevice graphicsDevice, int sphereResolution)
        {
            BoundingSphereRenderer.sphereResolution = sphereResolution;

            vertDecl = new VertexDeclaration(graphicsDevice, VertexPositionColor.VertexElements);
            effect = new BasicEffect(graphicsDevice, null);
            effect.LightingEnabled = false;
            effect.VertexColorEnabled = false;

            VertexPositionColor[] verts = new VertexPositionColor[(sphereResolution + 1)*3];

            int index = 0;

            float step = MathHelper.TwoPi/(float) sphereResolution;

            //create the loop on the XY plane first
            for (float a = 0f; a <= MathHelper.TwoPi; a += step)
            {
                verts[index++] = new VertexPositionColor(
                    new Vector3((float) Math.Cos(a), (float) Math.Sin(a), 0f),
                    Color.White);
            }

            //next on the XZ plane
            for (float a = 0f; a <= MathHelper.TwoPi; a += step)
            {
                verts[index++] = new VertexPositionColor(
                    new Vector3((float) Math.Cos(a), 0f, (float) Math.Sin(a)),
                    Color.White);
            }

            //finally on the YZ plane
            for (float a = 0f; a <= MathHelper.TwoPi; a += step)
            {
                verts[index++] = new VertexPositionColor(
                    new Vector3(0f, (float) Math.Cos(a), (float) Math.Sin(a)),
                    Color.White);
            }

            vertBuffer = new VertexBuffer(
                graphicsDevice,
                verts.Length*VertexPositionColor.SizeInBytes,
                BufferUsage.None);
            vertBuffer.SetData(verts);
        }

        /// <summary>
        /// Renders a bounding sphere using different colors for each axis.
        /// </summary>
        /// <param name="sphere">The sphere to render.</param>
        /// <param name="graphicsDevice">The graphics device to use when rendering.</param>
        /// <param name="view">The current view matrix.</param>
        /// <param name="projection">The current projection matrix.</param>
        /// <param name="xyColor">The color for the XY circle.</param>
        /// <param name="xzColor">The color for the XZ circle.</param>
        /// <param name="yzColor">The color for the YZ circle.</param>
        public static void Render(
            BoundingSphere sphere,
            GraphicsDevice graphicsDevice,
            Matrix view,
            Matrix projection,
            Color xyColor,
            Color xzColor,
            Color yzColor)
        {
            if (vertBuffer == null)
                InitializeGraphics(graphicsDevice, 96);

            graphicsDevice.VertexDeclaration = vertDecl;
            graphicsDevice.Vertices[0].SetSource(
                vertBuffer,
                0,
                VertexPositionColor.SizeInBytes);

            effect.World =
                Matrix.CreateScale(sphere.Radius)*
                Matrix.CreateTranslation(sphere.Center);
            effect.View = view;
            effect.Projection = projection;
            effect.DiffuseColor = xyColor.ToVector3();

            effect.Begin();
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Begin();

                //render each circle individually
                graphicsDevice.DrawPrimitives(
                    PrimitiveType.LineStrip,
                    0,
                    sphereResolution);

                effect.DiffuseColor = xzColor.ToVector3();
                effect.CommitChanges();
                graphicsDevice.DrawPrimitives(
                    PrimitiveType.LineStrip,
                    sphereResolution + 1,
                    sphereResolution);

                effect.DiffuseColor = yzColor.ToVector3();
                effect.CommitChanges();
                graphicsDevice.DrawPrimitives(
                    PrimitiveType.LineStrip,
                    (sphereResolution + 1)*2,
                    sphereResolution);

                pass.End();
            }
            effect.End();
        }

        /// <summary>
        /// Renders a bounding sphere using a single color for all three axis.
        /// </summary>
        /// <param name="sphere">The sphere to render.</param>
        /// <param name="graphicsDevice">The graphics device to use when rendering.</param>
        /// <param name="view">The current view matrix.</param>
        /// <param name="projection">The current projection matrix.</param>
        /// <param name="color">The color to use for rendering the circles.</param>
        public static void Render(
            ref BoundingSphere sphere,
            GraphicsDevice graphicsDevice,
            Matrix view,
            Matrix projection,
            Color color)
        {
            if (vertBuffer == null)
                InitializeGraphics(graphicsDevice, 96);

            graphicsDevice.VertexDeclaration = vertDecl;
            graphicsDevice.Vertices[0].SetSource(
                vertBuffer,
                0,
                VertexPositionColor.SizeInBytes);

            effect.World =
                Matrix.CreateScale(sphere.Radius)*
                Matrix.CreateTranslation(sphere.Center);
            effect.View = view;
            effect.Projection = projection;
            effect.DiffuseColor = color.ToVector3();

            effect.Begin();
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Begin();

                //render each circle individually
                graphicsDevice.DrawPrimitives(
                    PrimitiveType.LineStrip,
                    0,
                    sphereResolution);
                graphicsDevice.DrawPrimitives(
                    PrimitiveType.LineStrip,
                    sphereResolution + 1,
                    sphereResolution);
                graphicsDevice.DrawPrimitives(
                    PrimitiveType.LineStrip,
                    (sphereResolution + 1)*2,
                    sphereResolution);

                pass.End();
            }
            effect.End();
        }
    }
}
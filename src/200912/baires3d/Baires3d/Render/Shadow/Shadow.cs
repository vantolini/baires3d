/*
 * Created by Javier Cantón Ferrero.
 * MVP Windows-DirectX 2007/2008
 * DotnetClub Sevilla
 * Date 17/02/2007
 * Web www.codeplex.com/XNACommunity
 * Email javiuniversidad@gmail.com
 * blog: mirageproject.blogspot.com
 */


using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace b3d
{
    public class Shadow
    {
        public Effect shadowMap;
        private Effect depth;
        public Texture2D mask;
        public RenderTarget2D depthRenderTarget;
        public Texture2D depthBlurred;
        public DepthStencilBuffer stencilBuffer;
        private DepthStencilBuffer oldBuffer;
        private int shadowMapSize;
        private GaussianBlur gaussianBlur;
        private Light light;

        public Shadow()
        {
 
        }

        public void Initialize(GraphicsDevice device, ContentManager content, Light light)
        {
            this.light = light;

            shadowMapSize = 1024;

            //PresentationParameters pp = device.PresentationParameters;
            //depthRenderTarget = new RenderTarget2D(device,
            //    pp.BackBufferWidth, pp.BackBufferHeight, 1,
            //    pp.BackBufferFormat, pp.MultiSampleType, pp.MultiSampleQuality);

            depthRenderTarget = new RenderTarget2D(device, shadowMapSize, shadowMapSize, 1, SurfaceFormat.Rg32);
            stencilBuffer = new DepthStencilBuffer(device, shadowMapSize, shadowMapSize, DepthFormat.Depth24);

            depth = content.Load<Effect>("Depth");
            shadowMap = content.Load<Effect>("VarianceShadowMap");
            //mask = content.Load<Texture2D>("spotlight");

            gaussianBlur = new GaussianBlur();
            gaussianBlur.Initialize(device, content, shadowMapSize);
        }

        public void GetShadowMap(GameTime gameTime, GraphicsDevice device, CiudadesCollection ciudades, CameraComponent camera)
        {
            oldBuffer = device.DepthStencilBuffer;
            device.DepthStencilBuffer = stencilBuffer;
            device.SetRenderTarget(0, depthRenderTarget);
            device.Clear(Color.White);
            depth.Parameters["View"].SetValue(light.LightView);
            depth.Parameters["Projection"].SetValue(light.LightProject);
            depth.Parameters["FarClip"].SetValue(Constants.ar3d.cameraFar);

            ciudades.Draw(gameTime, depth, false);

            depth.Parameters["World"].SetValue(Matrix.Identity);

            //GaussianblurH
            device.SetRenderTarget(0, gaussianBlur.GaussianHRT);

            gaussianBlur.SetBlurEffectParameters(1.0f / device.Viewport.Width, 0);
            //depthRenderTarget.GetTexture().Save("pepepe.png", ImageFileFormat.Png);
            gaussianBlur.DrawQuad(device, depthRenderTarget.GetTexture(), gaussianBlur.gaussianBlur);

            //GaussianblurV
            device.SetRenderTarget(0, gaussianBlur.GaussianVRT);

            gaussianBlur.SetBlurEffectParameters(0, 1.0f / device.Viewport.Height);

            gaussianBlur.DrawQuad(device, gaussianBlur.GaussianHRT.GetTexture(), gaussianBlur.gaussianBlur);

            device.SetRenderTarget(0, null);
            device.DepthStencilBuffer = oldBuffer;
            device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1.0f, 0);

            depthBlurred = gaussianBlur.GaussianVRT.GetTexture();

            //device.Clear(Color.White);
        }

    }
}

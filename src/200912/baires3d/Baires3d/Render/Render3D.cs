using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace b3d
{
    public class Render3D
    {
        public RenderTarget2D renderTarget;
        private static VertexPositionColor[] verts = new VertexPositionColor[8];

        private static int[] indices = new int[]
                                           {
                                               0, 1,
                                               1, 2,
                                               2, 3,
                                               3, 0,
                                               0, 4,
                                               1, 5,
                                               2, 6,
                                               3, 7,
                                               4, 5,
                                               5, 6,
                                               6, 7,
                                               7, 4,
                                           };


        public Effect modelEffect;
        public Matrix modelWorldMatrix = Matrix.Identity;
        public Matrix prevModelWorldMatrix = Matrix.Identity;
        public MotionBlurType motionBlurType = MotionBlurType.None;
        public SpriteBatch spriteBatch;
        public Render3D(){
            BoundingSphereRenderer.InitializeGraphics(Constants.ar3d.GraphicsDevice, 96);
            BasicShader = new BasicShader();
            BasicShader.Init();

            //postProcessor = new PostProcessor(Constants.ar3d.GraphicsDevice, Constants.ar3d.Content);
            this._effect = Constants.ar3d.Content.Load<Effect>("Effects/ShadeAndBlur");

            PresentationParameters pp = Constants.GraphicsDevice.PresentationParameters;
            sceneTarget = new RenderTarget2D(Constants.ar3d.graphics.GraphicsDevice,
                pp.BackBufferWidth, pp.BackBufferHeight, 1,
                pp.BackBufferFormat, pp.MultiSampleType, pp.MultiSampleQuality);


/*            
            modelEffect = Constants.ar3d.Content.Load<Effect>("Effects\\Model");
            spriteBatch = new SpriteBatch(Constants.ar3d.GraphicsDevice);
            prevViewProj = Matrix.Identity;
            MakeRenderTargets();*/
        }

        public RenderTarget2D colorRT;
        public RenderTarget2D depthRT;
        public RenderTarget2D velocityRT;
        public RenderTarget2D velocityRTLastFrame;

        public Matrix prevView;
        public Matrix prevProj;
        public Matrix prevViewProj;
        TextureCube reflectionMap;
        private void MakeRenderTargets(){

            reflectionMap = Constants.ar3d.Content.Load<TextureCube>("Textures\\Ocean_Reflection");

            GraphicsDevice GraphicsDevice = Constants.ar3d.GraphicsDevice;
        

            if (colorRT != null)
                colorRT.Dispose();
            if (velocityRT != null)
                velocityRT.Dispose();
            if (depthRT != null)
                depthRT.Dispose();
            if (velocityRTLastFrame != null)
                velocityRTLastFrame.Dispose();
            postProcessor.FlushCache();

            // Store the color data in a Color texture              
            colorRT = new RenderTarget2D(GraphicsDevice,
                                                GraphicsDevice.PresentationParameters.BackBufferWidth,
                                                GraphicsDevice.PresentationParameters.BackBufferHeight,
                                                1,
                                                SurfaceFormat.Color,
                                                MultiSampleType.None,
                                                0,
                                                RenderTargetUsage.DiscardContents);

            // Store depth in a 32-bit floating point texture
            depthRT = new RenderTarget2D(GraphicsDevice,
                                                GraphicsDevice.PresentationParameters.BackBufferWidth,
                                                GraphicsDevice.PresentationParameters.BackBufferHeight,
                                                1,
                                                SurfaceFormat.Single,
                                                MultiSampleType.None,
                                                0,
                                                RenderTargetUsage.DiscardContents);

            // Store velocity in a texture with two 16-bit floating point channels
            velocityRT = new RenderTarget2D(GraphicsDevice,
                                                GraphicsDevice.PresentationParameters.BackBufferWidth,
                                                GraphicsDevice.PresentationParameters.BackBufferHeight,
                                                1,
                                                SurfaceFormat.Vector2,
                                                MultiSampleType.None,
                                                0,
                                                RenderTargetUsage.DiscardContents);

            velocityRTLastFrame = new RenderTarget2D(GraphicsDevice,
                                                GraphicsDevice.PresentationParameters.BackBufferWidth,
                                                GraphicsDevice.PresentationParameters.BackBufferHeight,
                                                1,
                                                SurfaceFormat.Vector2,
                                                MultiSampleType.None,
                                                0,
                                                RenderTargetUsage.DiscardContents);


        }
        public PostProcessor postProcessor;


        public void RenderBoundingBox(BoundingBox bb, Color color)
        {
            Vector3[] corners = bb.GetCorners();
            for (int i = 0; i < 8; i++)
            {
                verts[i].Position = corners[i];
                verts[i].Color = color;
            }


            Constants.ar3d.GraphicsDevice.DrawUserIndexedPrimitives(
                PrimitiveType.LineList,
                verts,
                0,
                8,
                indices,
                0,
                indices.Length/2);
        }
        public void RenderBoundingSphere(ref BoundingSphere bs, ref Color color)
        {
            BoundingSphereRenderer.Render(ref bs,
                                          Constants.ar3d.GraphicsDevice,
                                          Constants.Camera.View,
                                          Constants.Camera.Projection,
                                          color);
        }


        public BasicShader BasicShader;

        public void Begin()
        {
            BasicShader.Begin();
        }



        public void End()
        {
            BasicShader.End();
        }



        public void CommitChanges()
        {
            BasicShader.CommitChanges();
        }

        internal void EndPostprocess() {
            GraphicsDevice device = Constants.GraphicsDevice;

            device.RenderState.StencilEnable = false;

            // Do the post-processing
            device.SetRenderTarget(0, null);
            device.SetRenderTarget(1, null);
            device.SetRenderTarget(2, null);

           /*
            colorRT.GetTexture().Save("colorRT.png", ImageFileFormat.Png);
            velocityRT.GetTexture().Save("velocityRT.png", ImageFileFormat.Png);
            depthRT.GetTexture().Save("depthRT.png", ImageFileFormat.Png);
           
            */

           // velocityRT.GetTexture().Save("velocityRT.png", ImageFileFormat.Png);


            if (motionBlurType == MotionBlurType.None) {
                motionBlurType = MotionBlurType.DualVelocityBuffer12Samples;
                // If we're not post-processing, just copy the color target
                // to the back buffer.
                spriteBatch.Begin(SpriteBlendMode.None);
                device.SamplerStates[0].MagFilter = TextureFilter.Point;
                device.SamplerStates[0].MinFilter = TextureFilter.Point;
                spriteBatch.Draw(colorRT.GetTexture(), Vector2.Zero, Color.White);
                spriteBatch.End();
            }
            else {
                postProcessor.MotionBlur(colorRT, null, depthRT, velocityRT, velocityRTLastFrame, prevViewProj, motionBlurType);
            }



            // Hang onto the previous world and viewProj matrices for next frame
            prevModelWorldMatrix = modelWorldMatrix;
            prevView = Constants.Camera.View;
            prevProj = Constants.Camera.Projection;
            prevViewProj = Constants.Camera.View * Constants.Camera.Projection;

            // Swap the velocity buffers
            RenderTarget2D temp = velocityRTLastFrame;
            velocityRTLastFrame = velocityRT;
            velocityRT = temp;



        }

        internal void StartPostprocess() {
            Constants.ar3d.GraphicsDevice.SetRenderTarget(0, colorRT);
            Constants.ar3d.GraphicsDevice.SetRenderTarget(1, depthRT);
            Constants.ar3d.GraphicsDevice.SetRenderTarget(2, velocityRT);


        }

        internal void EndDraw() {
            modelEffect.CurrentTechnique.Passes[0].End();
            modelEffect.End();
        }

        internal void StartDraw() {

            modelEffect.CurrentTechnique = modelEffect.Techniques["Render"];

            // Set the parameters
            modelEffect.Parameters["g_matWorld"].SetValue(modelWorldMatrix);
            modelEffect.Parameters["g_matView"].SetValue(Constants.Camera.View);
            modelEffect.Parameters["g_matProj"].SetValue(Constants.Camera.Projection);
            modelEffect.Parameters["g_matPrevWorldViewProj"].SetValue(prevModelWorldMatrix * prevViewProj);

            modelEffect.Parameters["g_vCameraPositionWS"].SetValue(Constants.Camera.Position);
            modelEffect.Parameters["g_fCameraFarClip"].SetValue(Constants.ar3d.cameraFar *1.5f);

            modelEffect.Parameters["g_vSunlightDirectionWS"].SetValue(new Vector3(-0.7f, -1.0f, -1.0f));
            modelEffect.Parameters["g_vSunlightColor"].SetValue(new Vector3(1.0f, 0.7f, 0.4f));
            modelEffect.Parameters["g_fSunlightBrightness"].SetValue(5.0f);

            modelEffect.Parameters["g_vDiffuseAlbedo"].SetValue(new Vector3(1.0f));
            modelEffect.Parameters["g_vSpecularAlbedo"].SetValue(new Vector3(0.4f));
            modelEffect.Parameters["g_fSpecularPower"].SetValue(32.0f);
            modelEffect.Parameters["g_fReflectivity"].SetValue(0.7f);
            modelEffect.Parameters["g_fReflectionBrightness"].SetValue(1);

           // modelEffect.Parameters["ReflectionMap"].SetValue(reflectionMap);

            // Begin effect
            modelEffect.Begin(SaveStateMode.None);
            modelEffect.CurrentTechnique.Passes[0].Begin();

        }



        Effect _effect;

        Matrix previousWorld, World, View, Projection;

        RenderTarget2D sceneTarget;
        Texture2D sceneFrame;



    }

    public sealed class BasicDirectionalLight
    {
        // Fields
        private Vector3 cachedDiffuseColor;
        private Vector3 cachedSpecularColor;
        private EffectParameter diffuseColorParam;
        private EffectParameter directionParam;
        private bool enabled;
        private EffectParameter specularColorParam;

        // Methods
        internal BasicDirectionalLight(EffectParameter direction, EffectParameter diffuseColor, EffectParameter specularColor)
        {
            this.directionParam = direction;
            this.diffuseColorParam = diffuseColor;
            this.specularColorParam = specularColor;
        }

        internal void Copy(BasicDirectionalLight from)
        {
            this.enabled = from.enabled;
            this.cachedDiffuseColor = from.cachedDiffuseColor;
            this.cachedSpecularColor = from.cachedSpecularColor;
            this.diffuseColorParam.SetValue(this.cachedDiffuseColor);
            this.specularColorParam.SetValue(this.cachedSpecularColor);
        }

        // Properties
        public Vector3 DiffuseColor
        {
            get
            {
                return this.cachedDiffuseColor;
            }
            set
            {
                if (this.enabled)
                {
                    this.diffuseColorParam.SetValue(value);
                }
                this.cachedDiffuseColor = value;
            }
        }

        public Vector3 Direction
        {
            get
            {
                return this.directionParam.GetValueVector3();
            }
            set
            {
                this.directionParam.SetValue(value);
            }
        }

        public bool Enabled
        {
            get
            {
                return this.enabled;
            }
            set
            {
                if (this.enabled != value)
                {
                    this.enabled = value;
                    if (this.enabled)
                    {
                        this.diffuseColorParam.SetValue(this.cachedDiffuseColor);
                        this.specularColorParam.SetValue(this.cachedSpecularColor);
                    }
                    else
                    {
                        this.diffuseColorParam.SetValue(Vector3.Zero);
                        this.specularColorParam.SetValue(Vector3.Zero);
                    }
                }
            }
        }

        public Vector3 SpecularColor
        {
            get
            {
                return this.cachedSpecularColor;
            }
            set
            {
                if (this.enabled)
                {
                    this.specularColorParam.SetValue(value);
                }
                this.cachedSpecularColor = value;
            }
        }
    }


}
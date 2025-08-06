//======================================================================
//
//	MotionBlurSample
//
//		by MJP 
//      mpettineo@gmail.com
//      http://mynameismjp.wordpress.com/
//		12/1/09
//
//======================================================================
//
//	File:		PostProcessor.cs
//
//	Desc:		Defines the PostProcessor class.  This class is
//              responsible for applying the motion blur effect to the
//              scene.
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;


namespace b3d
{
    /// <summary>
	/// Handles rendering of various post-processing techniques,
    /// including bloom and tone mapping
	/// </summary>
	public class PostProcessor
	{
        public GraphicsDevice graphicsDevice;
		protected ContentManager contentManager;
        protected List<IntermediateTexture> intermediateTextures = new List<IntermediateTexture>();

        protected FullScreenQuad quad;

        protected Effect mbEffect;

        protected float blurSigma = 2.5f;

        protected Vector3[] frustumCornersWS = new Vector3[8];
        protected Vector3[] frustumCornersVS = new Vector3[8];
        protected Vector3[] farFrustumCornersVS = new Vector3[4];
        protected RenderTarget2D[] singleSourceArray = new RenderTarget2D[1];
        protected RenderTarget2D[] doubleSourceArray = new RenderTarget2D[2];
        protected RenderTarget2D[] tripleSourceArray = new RenderTarget2D[3];

        /// <summary>
        /// The class constructor
        /// </summary>
        /// <param name="graphicsDevice">The GraphicsDevice to use for rendering</param>
        /// <param name="contentManager">The ContentManager from which to load Effects</param>
		public PostProcessor(	GraphicsDevice graphicsDevice,
								ContentManager contentManager)
		{
			this.contentManager = contentManager;
            this.graphicsDevice = graphicsDevice;        

            // Load the effects
            mbEffect = contentManager.Load<Effect>("Effects\\pp_MotionBlur");

            // Initialize our buffers
            int width = graphicsDevice.PresentationParameters.BackBufferWidth;
            int height = graphicsDevice.PresentationParameters.BackBufferHeight;

            // Make our full-screen quad
            quad = new FullScreenQuad(graphicsDevice);
		}

        /// <summary>
        /// Performs tone mapping on the specified render target
        /// </summary>
        /// <param name="source">The source render target</param>
        /// <param name="result">The render target to which the result will be output</param>
        public void MotionBlur(RenderTarget2D source, 
                                RenderTarget2D result, 
                                RenderTarget2D depthTexture, 
                                RenderTarget2D velocityTexture,
                                RenderTarget2D prevVelocityTexture,
                                
                                Matrix prevViewProj,
                                MotionBlurType mbType)
		{
            // Get corners of the main camera's bounding frustum
            Matrix viewMatrix = Constants.Camera.camera.ViewMatrix;
            Constants.Camera.Frustum.GetCorners(frustumCornersWS);
            Vector3.Transform(frustumCornersWS, ref viewMatrix, frustumCornersVS);
            for (int i = 0; i < 4; i++)
                farFrustumCornersVS[i] = frustumCornersVS[i + 4];

            // Set the technique according to the motion blur type
            mbEffect.CurrentTechnique = mbEffect.Techniques[mbType.ToString()];

            mbEffect.Parameters["g_fBlurAmount"].SetValue(1.5f);
            mbEffect.Parameters["g_matInvView"].SetValue(Matrix.Invert(viewMatrix));
            mbEffect.Parameters["g_matLastViewProj"].SetValue(prevViewProj);

            RenderTarget2D[] sources;
            if (mbType == MotionBlurType.DepthBuffer4Samples
                || mbType == MotionBlurType.DepthBuffer8Samples
                || mbType == MotionBlurType.DepthBuffer12Samples)
            {
                sources = doubleSourceArray;
                sources[0] = source;
                sources[1] = depthTexture;
            }
            else
            {
                sources = tripleSourceArray;
                sources[0] = source;
                sources[1] = velocityTexture;
                sources[2] = prevVelocityTexture;
            }
            
            PostProcess(sources, result, mbEffect);
		}

        /// <summary>
        /// Disposes all intermediate textures in the cache
        /// </summary>
        public void FlushCache()
        {
            foreach (IntermediateTexture intermediateTexture in intermediateTextures)
                intermediateTexture.RenderTarget.Dispose();
            intermediateTextures.Clear();
        }

        /// <summary>
        /// Performs a post-processing step using a single source texture
        /// </summary>
        /// <param name="source">The source texture</param>
        /// <param name="result">The output render target</param>
        /// <param name="effect">The effect to use</param>
		protected void PostProcess(RenderTarget2D source, RenderTarget2D result, Effect effect)
		{
            RenderTarget2D[] sources = singleSourceArray;
			sources[0] = source;
			PostProcess(sources, result, effect);
		}

        /// <summary>
        /// Performs a post-processing step using multiple source textures
        /// </summary>
        /// <param name="sources">The source textures</param>
        /// <param name="result">The output render target</param>
        /// <param name="effect">The effect to use</param>
		protected void PostProcess(RenderTarget2D[] sources, RenderTarget2D result, Effect effect)
		{
			graphicsDevice.SetRenderTarget(0, result);
            graphicsDevice.Clear(Color.Black);

			for (int i = 0; i < sources.Length; i++)
				effect.Parameters["SourceTexture" + Convert.ToString(i)].SetValue(sources[i].GetTexture());
			effect.Parameters["g_vSourceDimensions"].SetValue(new Vector2(sources[0].Width, sources[0].Height));
            if (result == null)
                effect.Parameters["g_vDestinationDimensions"].SetValue(new Vector2(graphicsDevice.PresentationParameters.BackBufferWidth, graphicsDevice.PresentationParameters.BackBufferHeight));
            else
                effect.Parameters["g_vDestinationDimensions"].SetValue(new Vector2(result.Width, result.Height));
            effect.Parameters["g_vFrustumCornersVS"].SetValue(farFrustumCornersVS);

			// Begin effect
			effect.Begin(SaveStateMode.None);
			effect.CurrentTechnique.Passes[0].Begin();

			// Draw the quad
            quad.Draw(graphicsDevice);

			// We're done
			effect.CurrentTechnique.Passes[0].End();
			effect.End();
		}


		/// <summary>
		/// Checks the cache to see if a suitable rendertarget has already been created
		/// and isn't in use.  Otherwise, creates one according to the parameters
		/// </summary>
		/// <param name="width">Width of the RT</param>
		/// <param name="height">Height of the RT</param>
		/// <param name="format">Format of the RT</param>
		/// <returns>The suitable RT</returns>
		protected IntermediateTexture GetIntermediateTexture(int width, int height, SurfaceFormat format)
		{
            return GetIntermediateTexture(width, height, format, MultiSampleType.None, 0);
		}

        protected IntermediateTexture GetIntermediateTexture(int width,
                                                                int height,
                                                                SurfaceFormat format,
                                                                MultiSampleType msType,
                                                                int msQuality)
        {
            // Look for a matching rendertarget in the cache
            for(int i = 0; i < intermediateTextures.Count; i++)
            {
                if (intermediateTextures[i].InUse == false
                    && height == intermediateTextures[i].RenderTarget.Height
                    && format == intermediateTextures[i].RenderTarget.Format
                    && width == intermediateTextures[i].RenderTarget.Width
                    && msType == intermediateTextures[i].RenderTarget.MultiSampleType
                    && msQuality == intermediateTextures[i].RenderTarget.MultiSampleQuality)
                {
                    intermediateTextures[i].InUse = true;
                    return intermediateTextures[i];
                }
            }

            // We didn't find one, let's make one
            IntermediateTexture newTexture = new IntermediateTexture();
            newTexture.RenderTarget = new RenderTarget2D(graphicsDevice,
                                                            width,
                                                            height,
                                                            1,
                                                            format,
                                                            msType,
                                                            msQuality, 
                                                            RenderTargetUsage.DiscardContents);
            intermediateTextures.Add(newTexture);
            newTexture.InUse = true;
            return newTexture;
        }
      
	
        /// <summary>
        /// Swaps two RenderTarget's
        /// </summary>
        /// <param name="rt1">The first RT</param>
        /// <param name="rt2">The second RT</param>
        protected static void Swap (ref RenderTarget2D rt1, ref RenderTarget2D rt2)
        {
            RenderTarget2D temp = rt1;
            rt1 = rt2;
            rt2 = temp;
        }


        /// <summary>
        /// Used for textures that store intermediate results of
        /// passes during post-processing
        /// </summary>
        public class IntermediateTexture
        {
            public RenderTarget2D RenderTarget;
            public bool InUse;
        }

    }

}
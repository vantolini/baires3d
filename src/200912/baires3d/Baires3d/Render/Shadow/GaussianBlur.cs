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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace b3d
{
    class GaussianBlur
    {
        public RenderTarget2D GaussianHRT;
        public RenderTarget2D GaussianVRT;
        public Effect gaussianBlur;
        public SpriteBatch spriteBatch;

        public GaussianBlur()
        {
 
        }

        public void Initialize(GraphicsDevice device, ContentManager content, int shadowMapSize)
        {
            spriteBatch = new SpriteBatch(device);
            GaussianHRT = new RenderTarget2D(device, shadowMapSize, shadowMapSize, 1, SurfaceFormat.Rg32);
            GaussianVRT = new RenderTarget2D(device, shadowMapSize, shadowMapSize, 1, SurfaceFormat.Rg32);
            gaussianBlur = content.Load<Effect>("GaussianBlur");
        }

        public void DrawQuad(GraphicsDevice device, Texture2D texture, Effect effect)
        {
            //device.Clear(Color.White);

            effect.Begin();
            spriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.SaveState);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Begin();
                spriteBatch.Draw(texture, Vector2.Zero, Color.White);
                pass.End();
            }
            spriteBatch.End();
            effect.End();
        }

        #region Gaussian Helper
        public void SetBlurEffectParameters(float dx, float dy)
        {
            // Look up the sample weight and offset effect parameters.
            EffectParameter weightsParameter, offsetsParameter;

            weightsParameter = gaussianBlur.Parameters["SampleWeights"];
            offsetsParameter = gaussianBlur.Parameters["SampleOffsets"];

            // Look up how many samples our gaussian blur effect supports.
            int sampleCount = weightsParameter.Elements.Count;

            // Create temporary arrays for computing our filter settings.
            float[] sampleWeights = new float[sampleCount];
            Vector2[] sampleOffsets = new Vector2[sampleCount];

            // The first sample always has a zero offset.
            sampleWeights[0] = ComputeGaussian(0);
            sampleOffsets[0] = new Vector2(0);

            // Maintain a sum of all the weighting values.
            float totalWeights = sampleWeights[0];

            // Add pairs of additional sample taps, positioned
            // along a line in both directions from the center.
            for (int i = 0; i < sampleCount / 2; i++)
            {
                // Store weights for the positive and negative taps.
                float weight = ComputeGaussian(i + 1);

                sampleWeights[i * 2 + 1] = weight;
                sampleWeights[i * 2 + 2] = weight;

                totalWeights += weight * 2;

                // To get the maximum amount of blurring from a limited number of
                // pixel shader samples, we take advantage of the bilinear filtering
                // hardware inside the texture fetch unit. If we position our texture
                // coordinates exactly halfway between two texels, the filtering unit
                // will average them for us, giving two samples for the price of one.
                // This allows us to step in units of two texels per sample, rather
                // than just one at a time. The 1.5 offset kicks things off by
                // positioning us nicely in between two texels.
                float sampleOffset = i * 2 + 1.5f;

                Vector2 delta = new Vector2(dx, dy) * sampleOffset;

                // Store texture coordinate offsets for the positive and negative taps.
                sampleOffsets[i * 2 + 1] = delta;
                sampleOffsets[i * 2 + 2] = -delta;
            }

            // Normalize the list of sample weightings, so they will always sum to one.
            for (int i = 0; i < sampleWeights.Length; i++)
            {
                sampleWeights[i] /= totalWeights;
            }

            // Tell the effect about our new filter settings.
            weightsParameter.SetValue(sampleWeights);
            offsetsParameter.SetValue(sampleOffsets);
        }

        private float ComputeGaussian(float n)
        {
            float BlurAmount = 4;
            float theta = BlurAmount;

            return (float)((1.0 / Math.Sqrt(2 * Math.PI * theta)) *
                           Math.Exp(-(n * n) / (2 * theta * theta)));
        }
        #endregion
    }
}

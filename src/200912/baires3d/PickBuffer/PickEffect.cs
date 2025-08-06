//
//      coded by un
//            --------------
//                     mindshifter.com
//


using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mindshifter
{
    internal class PickEffect
    {
        private IGraphicsDeviceService  mGraphics;
        private Effect                  mEffect;

        internal PickEffect(IGraphicsDeviceService graphics)
        {
            mGraphics = graphics;

            mGraphics.DeviceCreated += new EventHandler(CreateEffect);
            if ((mGraphics.GraphicsDevice != null) && !mGraphics.GraphicsDevice.IsDisposed)
                CreateEffect(this, null);
        }

        private void CreateEffect(object sender, EventArgs e)
        {
            CompiledEffect ce = Effect.CompileEffectFromSource(EffectSource, new CompilerMacro[0],
                null, CompilerOptions.None, TargetPlatform.Windows);

            mEffect = new Effect(mGraphics.GraphicsDevice, ce.GetEffectCode(), CompilerOptions.None, null);
        }

        public void Render(Queue<PickRenderable> renderables)
        {
            mEffect.Begin(SaveStateMode.SaveState);
            mEffect.CurrentTechnique.Passes[0].Begin();

            foreach (PickRenderable rend in renderables)
            {
                mEffect.Parameters["WVP"].SetValue(rend.WVP);
                mEffect.Parameters["PickID"].SetValue(rend.PickColor.ToVector4());
                mEffect.CommitChanges();

                rend.Render();
            }

            mEffect.CurrentTechnique.Passes[0].End();
            mEffect.End();
        }

        private static string EffectSource =
    @"  float4x4 WVP;
        float4 PickID;

        float4 VS(float4 Position : POSITION0) : POSITION0
        {
            return mul(Position, WVP);
        }

        float4 PS(float4 Position : POSITION0) : COLOR0
        {
            return PickID;
        }

        Technique Pick
        {
            Pass Pick
            {
                AlphaBlendEnable = False;
                FillMode = Solid;
                ZEnable = True;
                ZWriteEnable = True;
                ZFunc = LessEqual;
                MultiSampleAntialias = False;

                VertexShader = compile vs_1_1 VS();
                PixelShader = compile ps_2_0 PS();
            }
        }";
    }
}
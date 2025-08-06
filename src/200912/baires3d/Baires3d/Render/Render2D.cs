using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace b3d
{
    public class Render2D
    {
        public SpriteBatch sb;

        public void Begin()
        {
            sb.Begin(
                SpriteBlendMode.AlphaBlend, 
                SpriteSortMode.Immediate,
                SaveStateMode.None
                );
        }

        public void End()
        {
            sb.End();
        }

        public void DrawLote(string Texto, Vector2 pos, Color color)
        {
            sb.DrawString(Fonts[3],
                          Texto,
                          pos,
                          Color.White,
                          0,
                          Vector2.One,
                          0.8f /* + (Distancia * 1.3f)*/,
                          SpriteEffects.None,
                          0);
        }

        public void DrawIcon(int IconIndex, Vector2 pos)
        {
            sb.Draw(Constants.TextureManager.imgIconos[IconIndex],
                    pos,
                    Color.White
                );
        }

        public Texture2D Box;
        public Vector2 msr = Vector2.Zero;

        public void DrawBox(string Texto, Vector2 pos, Color color, float fontSize, float rot)
        {
            msr = Fonts[0].MeasureString(Texto);
            pos.X -= 4;
            pos.Y += 1;
            sb.Draw(Box, pos,
                    new Rectangle((int) pos.X, (int) pos.Y, (int) msr.X + 2, (int) msr.Y - 4),
                    Color.Red,
                    rot, Vector2.Zero,
                    1,
                    SpriteEffects.None, 0.01f);
        }

        public void DrawBox(string Texto, Vector2 pos, Color color, float fontSize, float rot, int font)
        {
            msr = Fonts[font].MeasureString(Texto);
            pos.X -= 1;
            pos.Y += 1;
            sb.Draw(Box, pos,
                    new Rectangle((int) pos.X, (int) pos.Y, (int) msr.X + 2, (int) msr.Y),
                    Color.Red,
                    rot, Vector2.Zero,
                    1,
                    SpriteEffects.None, 0.01f);
        }


        public void DrawAvenida(string Texto, Vector2 pos, Color color, float fontSize, float rot)
        {
            sb.DrawString(Fonts[0],
                          Texto,
                          pos,
                          color,
                          rot,
                          Vector2.One,
                          fontSize
                          /*0.6f + (Distancia * 1.3f)*/,
                          SpriteEffects.None,
                          0.6f);
        }

        public void DrawAvenida(string Texto, Vector2 pos, Color color, float fontSize, float rot, float layerDepth)
        {
            sb.DrawString(Fonts[0],
                          Texto,
                          pos,
                          color,
                          rot,
                          Vector2.One,
                          fontSize
                          /*0.6f + (Distancia * 1.3f)*/,
                          SpriteEffects.None,
                          layerDepth);
        }

        public void DrawAvenida(string Texto, Vector2 pos, Color color, float fontSize)
        {
            sb.DrawString(Fonts[0],
                          Texto,
                          pos,
                          color,
                          0,
                          Vector2.One,
                          fontSize
                          /*0.6f + (Distancia * 1.3f)*/,
                          SpriteEffects.None,
                          0);
        }


        public void DrawString(string Texto, Vector2 Pos, Color color, float layerDepth)
        {
            sb.DrawString(Fonts[0], Texto, Pos, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None,
                          layerDepth);
        }

        public void DrawString(string Texto, Vector2 Pos, Color color, float fontSize, float rot, float layerDepth,
                               int font)
        {
            sb.DrawString(Fonts[font], Texto, Pos, Color.White, rot, Vector2.Zero, fontSize, SpriteEffects.None,
                          layerDepth);
        }

        public void DrawString(string Texto, Vector2 Pos, Color color)
        {
            sb.DrawString(Fonts[1], Texto, Pos, color, 0, Vector2.Zero, 0.8f, SpriteEffects.None,
                          1);
        }

        public SpriteFont[] Fonts;

        public Render2D()
        {
            sb = new SpriteBatch(Constants.GraphicsDevice);
            LoadFonts()
                ;
        }

        private void LoadFonts()
        {
            //0 Barrio
            //1 Calle
            //2 Avenida
            Box = Constants.ar3d.Content.Load<Texture2D>("pix");


            Fonts = new SpriteFont[3];
            for (int i = 0; i < Fonts.Length; i++)
            {
                Fonts[i] = Constants.ar3d.Content.Load<SpriteFont>("Fonts//" + i);
                Fonts[i].Spacing = -4f;
            }
        }
    }
}
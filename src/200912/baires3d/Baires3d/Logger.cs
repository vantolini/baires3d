using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace b3d
{
    public class Logger2d
    {
        private StringBuilder statlog = new StringBuilder(800);
        private Vector2 logPos = new Vector2(5, 5);

        public void AddLog(string logg)
        {
            statlog.Append(logg + "\r\n");
        }

        public void DrawLog()
        {
            Constants.Render2D.DrawString(statlog.ToString(), logPos, Color.Black, 0.98f);
            statlog.Remove(0, statlog.Length);
        }
    }
}
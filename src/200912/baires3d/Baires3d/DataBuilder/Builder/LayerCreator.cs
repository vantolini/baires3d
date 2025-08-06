using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace b3d
{
    internal class LayerCreator
    {
        public void BuildLayers(){

            CityBuilder Capital = new CityBuilder("Capital Federal");
            Capital.Process();
        }
    }
}
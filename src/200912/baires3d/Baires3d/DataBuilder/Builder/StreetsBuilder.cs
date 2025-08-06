using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace b3d
{
    internal class StreetCoord
    {
        public Vector3 Coord = Vector3.Zero;
        public List<int> Streets = new List<int>();

        public void addID(int id)
        {
            //if (!Streets.Contains(id))
            Streets.Add(id);
        }

        public StreetCoord(Vector3 coord)
        {
            Coord = coord;
        }
    }

    public class StreetsBuilder : LayerBuilder
    {
        public StreetsBuilder(string name) :base(name) {}

        public StreetsBuilder(string name, string fileName) : base(name, fileName) {}

        public override void Process()
        {
            //LayerHelper.SerializeIndex("Calles", @"Data\Ciudades\Capital Federal\", new string[] { "Calles" }, LayerFields.Streets);
            Console.WriteLine("wrote  Calles");
        }

        public override void Dispose()
        {
        }
    }
}
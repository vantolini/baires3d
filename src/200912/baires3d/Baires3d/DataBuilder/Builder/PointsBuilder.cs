using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace b3d
{
    public class PointsBuilder : LayerBuilder
    {

        public PointsBuilder(string name) :base(name) {}

        public PointsBuilder(string name, string fileName) : base(name, fileName) { }
        public List<Feature> Points;


        public override void Process()
        {
            
        }

        public override void Dispose()
        {
        }
    }
}
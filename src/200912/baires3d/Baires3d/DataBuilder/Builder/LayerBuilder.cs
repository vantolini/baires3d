using System.Collections.Generic;

namespace b3d
{
    public abstract class LayerBuilder : ILayerBuilder
    {
        public string Name;
        protected string FileName;
        //protected ShapefileLoader Shapefile;


        public LayerBuilder (){}
        public LayerBuilder(string name){
            Name = name;
        }

        public LayerBuilder(string name, string fileName){
            Name = name;
            FileName = fileName;
        }
        public List<Feature> Features;
        public abstract void Process();
        public abstract void Dispose();
    }
}
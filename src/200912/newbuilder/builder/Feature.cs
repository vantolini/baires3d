using System;
using System.Collections.Generic;
using System.Text;

namespace Builder
{
    [System.Diagnostics.DebuggerDisplay("Name: {Name} Pts: {Points}")]
    public class FeatureGroup
    {
        public int Id;
        public string Name;
        public List<Feature> Features = new List<Feature>();

        public void Add(Feature feature)
        {
            Features.Add(feature);
        }

        public void Build()
        {
        }
    }

    [System.Diagnostics.DebuggerDisplay("{Seccion} {Manzana} {Parcela}")]
    public class Feature
    {
        public float Altura;
        public int FloorCount;
        public List<Feature> Floors = new List<Feature>();
        public Color Color;
        public int Source;
        public int Target;
        public int Texture;

        public List<Tramo> Tramos = new List<Tramo>();

        public List<int> Intersections;

        public void AgregarInterseccion(List<int> ints)
        {
            for (int i = 0; i < ints.Count; i++)
            {
                if (!Intersections.Contains(ints[i]))
                {
                    Intersections.Add(ints[i]);
                }
            }
        }

        public List<Feature> Features = new List<Feature>();
        public int ID = 0;

        public int VertexStart;
        public int VertexEnd;
        public List<string> Data = new List<string>();
        public List<Vector3> Points;

        public List<Vector3> OrigLine = new List<Vector3>();
        public string Name;
        public List<Vector3> Techo;

        public int[] Indices;
        public int[] IndicesTecho;

        public float Height;
        public int numVertices, numPrimitives;
        public Vector3[] Paredes;
        public Vector2[] Coords;
        public PositionNormalTextureColor[] Vertices;
        public string Code = "";
        public bool EspacioVerde;
        public string Icon;
        public bool IsVerde;
        public int ColorIndex;
        internal float sr;
        internal BoundingBox boundingBox;
        internal string Manzana;
        internal bool Vereda;
        internal int FloorNumber;
        internal bool isRoof;
        internal bool isFirst;
        internal bool isSingle;
        internal bool isLast;
        internal string Seccion;
        internal string Parcela;

        public Feature()
        {
        }

        public Feature(string name) {
            this.Name = name;
        }
    }
}
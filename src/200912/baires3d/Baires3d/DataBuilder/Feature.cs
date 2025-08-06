using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace b3d
{
    [System.Diagnostics.DebuggerDisplay("{Position} C: {Color}")]
    public struct PositionNormalTextureColor2
    {
        /// <summary>
        /// The position of the vertex.
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// The texture coordinate0.
        /// </summary>
        public Vector3 Normal;

        /// <summary>
        /// The texture coordinate1.
        /// </summary>
        public Vector2 TextureCoordinate;

        public Color Color;


        /// <summary>
        /// Initializes a new instance of the <see cref="PositionNormalTextureColor"/> struct.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="normal">The normal.</param>
        /// <param name="texCoord1">The tex coord1.</param>
        public PositionNormalTextureColor2(Vector3 position, Vector3 normal, Vector2 texCoord1, Color color)
        {
            this.Position = position;
            this.Normal = normal;
            this.TextureCoordinate = texCoord1;
            this.Color = color;
        }

        public PositionNormalTextureColor2(Vector3 position, Vector3 normal, Vector2 texCoord1)
        {
            this.Position = position;
            this.Normal = normal;
            this.TextureCoordinate = texCoord1;
            this.Color = Color.White;
        }


        public PositionNormalTextureColor2(Vector3 position, Vector3 normal)
        {
            this.Position = position;
            this.Normal = normal;
            this.TextureCoordinate = Vector2.Zero;
            this.Color = Color.White;
        }

        public PositionNormalTextureColor2(Vector3 position)
        {
            this.Position = position;
            this.Normal = Vector3.UnitY;
            this.TextureCoordinate = Vector2.Zero;
            this.Color = Color.White;
        }

        public PositionNormalTextureColor2(Vector3 position, Color color)
        {
            this.Position = position;
            this.Normal = Vector3.UnitY;
            this.TextureCoordinate = Vector2.Zero;
            this.Color = color;
        }
    }


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


    public class Tramo
    {
        public List<Vector3> OrigLine = new List<Vector3>();

        public Tramo(List<Vector3> origLine)
        {
            OrigLine = origLine;
        }
    }

    [System.Diagnostics.DebuggerDisplay("Name: {Name} Pts: {Points}")]
    public class Feature
    {
        public List<Tramo> Tramos = new List<Tramo>();

        public List<int> Intersections = new List<int>();

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

        public int ID = 0;

        public int VertexStart;
        public int VertexEnd;
        public BoundingSphere BoundingSphere;
        public List<string> Data = new List<string>();
        public List<Vector3> Points;
        public List<Vector2> Points2D;
        public string Name;
        public List<Vector3> Techo;
        public List<Vector3> Exterior;
        public List<Vector3> OrigLine = new List<Vector3>();
        //public List<Vector3> CPoints;
        //public Node Nodo1;
        //public Node Nodo2;

        public Vector2 Centro
        {
            get
            {
                Vector2 Total = Vector2.Zero;
                foreach (Vector2 Punto in Points2D)
                    Total += Punto;

                return (Total/Points2D.Count);
            }
        }

        public List<Vector2> NonIntersecting = new List<Vector2>();
        public List<Vector2> Bordes = new List<Vector2>();

        public void ConstruirBordes()
        {
            Vector2 p1;
            Vector2 p2;

            Bordes.Clear();
            for (int i = 0; i < Points2D.Count; i++)
            {
                p1 = Points2D[i];
                if (i + 1 >= Points2D.Count)
                {
                    p2 = Points2D[0];
                }
                else
                {
                    p2 = Points2D[i + 1];
                }
                Bordes.Add(p2 - p1);
            }
        }


        public int[] Indices;
        public int[] IndicesTecho;

        public float Height;
        public int numVertices, numPrimitives;
        public Vector3[] Paredes;
        public PositionNormalTextureColor[] Vertices;
        public string Code = "";

        public Feature()
        {
        }
    }
}
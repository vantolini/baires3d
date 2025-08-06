using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace b3d{
    public class LineComponent : RenderableComponent
    {
        public VertexPositionNormalColor[] Vertices;
        public int[] Indices;



        public LineComponent(string name)
            : base(name)
        {
        }

        public override void Dispose()
        {
        }

        public override void Initialize()
        {
        }

        public override void Update(GameTime gameTime)
        {
        }


        public List<Vector3> GetTriangleStrip(Vector3[] points, float thickness)
        {
            if (points.Length == 0)
            {
                //return null;
            }
            Vector3 lastPoint = Vector3.Zero;
            List<Vector3> list = new List<Vector3>();
            for (int i = 0; i < points.Length; i++)
            {
                if (i == 0)
                {
                    lastPoint = points[i];
                    continue;
                }
                //the direction of the current line
                Vector3 direction = lastPoint - points[i];
                direction.Normalize();
                //the perpendiculat to the current line
                Vector3 normal = Vector3.Cross(direction, Vector3.Up);
                normal.Normalize();
                Vector3 p1 = lastPoint + normal *thickness;
                Vector3 p2 = lastPoint - normal*thickness;
                Vector3 p3 = points[i] + normal*thickness;
                Vector3 p4 = points[i] - normal*thickness;

                list.Add(new Vector3(p4.X, p4.Y, p4.Z));
                list.Add(new Vector3(p3.X, p3.Y, p3.Z));
                list.Add(new Vector3(p2.X, p2.Y, p2.Z));
                list.Add(new Vector3(p1.X, p1.Y, p1.Z));
                lastPoint = points[i];
            }
            return list;
        }



        public  Vector2 textureUpperLeft = new Vector2(0.0f, 0.0f);
        public  Vector2 textureUpperRight = new Vector2(1.0f, 0.0f);
        public  Vector2 textureLowerLeft = new Vector2(0.0f, 1.0f);
        public  Vector2 textureLowerRight = new Vector2(1.0f, 1.0f);


        public void CreateLine(Vector3[] points, Color color)
        {
            List<Vector3> vertitos = LayerHelper.GetTriangleStrip(points, 300f);

            int src_pos = 0, dst_pos = 0;
            Vector3 a, b, c;
            Vector3[] vertos = new Vector3[(vertitos.Count - 2)*3];

            a = vertitos[src_pos++];
            b = vertitos[src_pos++];
            for (int i = 0; i < vertitos.Count - 2; i++)
            {
                c = vertitos[src_pos++];
                if ((i & 1) == 0)
                {
                    vertos[dst_pos++] = a;
                    vertos[dst_pos++] = b;
                    vertos[dst_pos++] = c;
                }
                else
                {
                    vertos[dst_pos++] = b;
                    vertos[dst_pos++] = a;
                    vertos[dst_pos++] = c;
                }
                a = b;
                b = c;
            }
            Vertices = new VertexPositionNormalColor[vertos.Length];
            for (int xx = 0; xx < vertos.Length; xx++)
            {
                Vertices[xx] = new VertexPositionNormalColor(new Vector3(vertos[xx].X, 0, vertos[xx].Z), Vector3.Up, color);
            }

            Indices = new int[Vertices.Length];
            for (int x = 0; x < Indices.Length; x++) {
                Indices[x] = x;
            }

            for (int g = 0; g < Vertices.Length; g++)
                Vertices[g].Normal = new Vector3(0, 0, 0);

            for (int g = 0; g < Indices.Length / 3; g++) {
                Vector3 firstvec = Vertices[Indices[g * 3 + 1]].Position -
                                   Vertices[Indices[g * 3]].Position;
                Vector3 secondvec = Vertices[Indices[g * 3]].Position -
                                    Vertices[Indices[g * 3 + 2]].Position;
                Vector3 normal = Vector3.Cross(firstvec, secondvec);
                normal.Normalize();
                Vertices[Indices[g * 3]].Normal += normal;
                Vertices[Indices[g * 3 + 1]].Normal += normal;
                Vertices[Indices[g * 3 + 2]].Normal += normal;
            }
            vertexDeclaration = new VertexDeclaration(Constants.ar3d.GraphicsDevice, VertexPositionNormalColor.VertexElements);
        }

        private int mostrar = 0;


        
        private VertexDeclaration vertexDeclaration;
        
        public override void Draw(GameTime gameTime)
        {
            if (mostrar > 15)
            {
                //return;
            }
            else
            {
                Constants.GraphicsDevice.VertexDeclaration = vertexDeclaration;
               
                Visible = true;
                Constants.GraphicsDevice.DrawUserPrimitives(
                    PrimitiveType.TriangleList, Vertices, 0, Vertices.Length/3);
                if (mostrar > 30)
                {
                    mostrar = 0;
                }
            }
        }
    }
}
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace b3d{
    public class CallesComponent : RenderableComponent
    {
        public List<CalleComponent> Calles = new List<CalleComponent>();
        public List<CalleComponent> Avenidas = new List<CalleComponent>();
        public IndexBuffer indexBuffer;
        public VertexDeclaration vertexDeclaration;
        public int[] Indices;
        public VertexBuffer vertexBuffer;

        public override void Dispose()
        {
        }

        private int getAllVxCount()
        {
            int cnt = 0;

            for (int x = 0; x < Calles.Count; x++)
            {
                for (int i = 0; i < Calles[x].Tramos.Count; i++)
                {
                    cnt += Calles[x].Tramos[i].Line.Vertices.Length;
                }
            }
            return cnt;
        }

        public short sizeInBytes;
        public int VertexCount;
        public int TriangleCount;
        public void BuildBuffer(){

            int vxcount = getAllVxCount();
            VertexPositionNormalColor[] vx1 = new VertexPositionNormalColor[vxcount];
            int pp = 0;
            for (int x = 0; x < Calles.Count; x++)
            {
                for (int i = 0; i < Calles[x].Tramos.Count; i++)
                {
                    for (int v = 0; v < Calles[x].Tramos[i].Line.Vertices.Length; v++){
                        vx1[pp] = new VertexPositionNormalColor();
                        vx1[pp].Position = Calles[x].Tramos[i].Line.Vertices[v].Position;
                        vx1[pp].Normal = Calles[x].Tramos[i].Line.Vertices[v].Normal;
                        vx1[pp].Color = Calles[x].Tramos[i].Line.Vertices[v].Color;//LayerHelper.GetRandomColor();

                        pp++;
                    }
                }
            }


            List<int> allIndices = new List<int>();
            int add = 0;
            int cnttt = 0;
            for (int x = 0; x < Calles.Count; x++)
            {
                for (int e = 0; e < Calles[x].Tramos.Count; e++){
                    cnttt = 0;
                    for (int v = 0; v < Calles[x].Tramos[e].Line.Vertices.Length; v++){
                        allIndices.Add((cnttt + add));
                        cnttt++;
                    }
                    add += Calles[x].Tramos[e].Line.Vertices.Length;
                    
                }
                
            }
            Indices = allIndices.ToArray();

            
            for (int g = 0; g < vx1.Length; g++)
                vx1[g].Normal = new Vector3(0, 0, 0);

            for (int g = 0; g < Indices.Length / 3; g++) {
                Vector3 firstvec = vx1[Indices[g * 3 + 1]].Position -
                                   vx1[Indices[g * 3]].Position;
                Vector3 secondvec = vx1[Indices[g * 3]].Position -
                                    vx1[Indices[g * 3 + 2]].Position;
                Vector3 normal = Vector3.Cross(firstvec, secondvec);
                normal.Normalize();
                vx1[Indices[g * 3]].Normal += normal;
                vx1[Indices[g * 3 + 1]].Normal += normal;
                vx1[Indices[g * 3 + 2]].Normal += normal;
            }

            VertexBufferFactory.CreateVertexBuffer<VertexPositionNormalColor>(
    Constants.ar3d.GraphicsDevice,
    vx1, out vertexDeclaration, out vertexBuffer, out sizeInBytes);

            VertexCount = vx1.Length;
            TriangleCount = Indices.Length / 3;

            indexBuffer = new IndexBuffer(Constants.ar3d.GraphicsDevice, 4*Indices.Length, BufferUsage.None,
                                          IndexElementSize.ThirtyTwoBits);
            indexBuffer.SetData<int>(Indices);
        }

        public void CreateBounds()
        {
            BoundingSphere = Calles[0].BoundingSphere;

            for (int i = 1; i < Calles.Count; i++)
            {
                BoundingSphere = BoundingSphere.CreateMerged(BoundingSphere,
                                                             Calles[i].BoundingSphere);
            }

            BoundingBox = BoundingBox.CreateFromSphere(BoundingSphere);
        }

        public void Draw2d(GameTime gameTime)
        {
            if (!Visible)
                return;
            
            for (int i = 0; i < Calles.Count; i++)
            {
                if (Calles[i].Visible)
                {
                    Calles[i].Draw2d(gameTime);
                }
            }
        }

        private bool isVisible(ref BoundingSphere bs)
        {
            if (Constants.Camera.Frustum.Contains(
                    bs) != ContainmentType.Disjoint)
            {
                return true;
            }
            return false;
        }

        public override void Initialize()
        {
        }

        public override void Update(GameTime gameTime)
        {
        }

        public CallesComponent()
        {
            this.Type = ComponentType.Streets;
        }

        public override void Draw(GameTime gameTime)
        {

            if (isVisible(ref BoundingSphere))
            {
                Visible = true;
            }
            else
            {
                Visible = false;
                return;
            }

        }
    }
}
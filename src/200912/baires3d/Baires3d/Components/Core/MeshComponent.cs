using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace b3d{
    public class MeshComponent : MeshComponentPart
    {
        public List<MeshComponentPart> MeshParts = new List<MeshComponentPart>();
        public bool BigBufferBuilt = false;

        public MeshComponent(string name)
            : base(name) {
            
        }

        public MeshComponent(){
        }

        public override void Dispose()
        {
        }

        public override void Initialize()
        {
        }

        private int getAllVxCount()
        {
            int cnt = 0;
            for (int i = 0; i < MeshParts.Count; i++)
            {
                cnt += MeshParts[i].Positions.Length;
            }
            return cnt;
        }
        public void BuildBigBuffer() {
            List<int> allIndices = new List<int>();
            int add = 0;
            for (int x = 0; x < MeshParts.Count; x++) {
                for (int i = 0; i < MeshParts[x].Indices.Length; i++) {
                    allIndices.Add((MeshParts[x].Indices[i] + add));
                }
                add += MeshParts[x].Indices.Length;
            }
            Indices = allIndices.ToArray();


            bool hasNormals = (MeshParts[0].Normals != null);
            bool hasTx = (MeshParts[0].TextureCoordinates != null);

            int[] optIndices = null;


            if (!hasNormals) {
                if (!hasTx) {
                    int vxcount = getAllVxCount();
                    VertexPositionColor[] vx1 = new VertexPositionColor[vxcount];
                    int pp = 0;
                    for (int x = 0; x < MeshParts.Count; x++) {
                        for (int i = 0; i < MeshParts[x].Positions.Length; i++) {
                            vx1[pp] = new VertexPositionColor(
                                MeshParts[x].Positions[i],
                                MeshParts[x].Colors[i]
                                );
                            pp++;
                        }
                    }


                    VertexPositionColor[] optVertices = vx1;
                    optIndices = Indices;

                    VertexBufferFactory.CreateVertexBuffer<VertexPositionColor>(
                        Constants.ar3d.GraphicsDevice,
                        optVertices, out vertexDeclaration, out vertexBuffer, out sizeInBytes);

                    VertexCount = optVertices.Length;
                    TriangleCount = optIndices.Length / 3;
                } else {
                    int vxcount = getAllVxCount();
                    VertexPositionColorTexture[] vx1 = new VertexPositionColorTexture[vxcount];
                    int pp = 0;
                    for (int x = 0; x < MeshParts.Count; x++) {
                        for (int i = 0; i < MeshParts[x].Positions.Length; i++) {
                            vx1[pp] = new VertexPositionColorTexture(
                                MeshParts[x].Positions[i],
                                MeshParts[x].Colors[i],
                                MeshParts[x].TextureCoordinates[i]
                                );
                            pp++;
                        }
                    }


                    VertexPositionColorTexture[] optVertices = vx1;
                    optIndices = Indices;

                    VertexBufferFactory.CreateVertexBuffer<VertexPositionColorTexture>(
                        Constants.ar3d.GraphicsDevice,
                        optVertices, out vertexDeclaration, out vertexBuffer, out sizeInBytes);
                    VertexCount = optVertices.Length;
                    TriangleCount = optIndices.Length / 3;
                }
            }
            else {
                if (!hasTx) {
                    int vxcount = getAllVxCount();
                    VertexPositionNormalColor[] vx1 = new VertexPositionNormalColor[vxcount];
                    int pp = 0;
                    for (int x = 0; x < MeshParts.Count; x++) {
                        for (int i = 0; i < MeshParts[x].Positions.Length; i++) {
                            vx1[pp] = new VertexPositionNormalColor();
                            vx1[pp].Position = MeshParts[x].Positions[i];
                            vx1[pp].Normal = MeshParts[x].Normals[i];
                            vx1[pp].Color = MeshParts[x].Colors[i];

                            pp++;
                        }
                    }


                    VertexPositionNormalColor[] optVertices = vx1;
                    optIndices = Indices;


                    VertexBufferFactory.CreateVertexBuffer<VertexPositionNormalColor>(
                        Constants.ar3d.GraphicsDevice,
                        optVertices, out vertexDeclaration, out vertexBuffer, out sizeInBytes);
                    VertexCount = optVertices.Length;
                    TriangleCount = optIndices.Length / 3;
                }
                else {
                    int vxcount = getAllVxCount();
                    PositionNormalTextureColor[] vx1 = new PositionNormalTextureColor[vxcount];
                    int pp = 0;
                    for (int x = 0; x < MeshParts.Count; x++) {
                        for (int i = 0; i < MeshParts[x].Positions.Length; i++) {
                            vx1[pp] = new PositionNormalTextureColor(
                                MeshParts[x].Positions[i],
                                
                                MeshParts[x].Normals[i],
                                MeshParts[x].TextureCoordinates[i],
                                MeshParts[x].Colors[i]
                                );

                            pp++;
                        }
                    }

                    PositionNormalTextureColor[] optVertices = vx1;
                    optIndices = Indices;



                    VertexBufferFactory.CreateVertexBuffer<PositionNormalTextureColor>(
                        Constants.ar3d.GraphicsDevice,
                        optVertices, out vertexDeclaration, out vertexBuffer, out sizeInBytes);
                    VertexCount = optVertices.Length;
                    TriangleCount = optIndices.Length / 3;
                }
            }

            indexBuffer = new IndexBuffer(Constants.ar3d.GraphicsDevice, 4 * optIndices.Length, BufferUsage.None,
            IndexElementSize.ThirtyTwoBits);
            indexBuffer.SetData<int>(optIndices);

            BigBufferBuilt = true;
        }

        public void BuildBigBuffer2()
        {
            if (MeshParts[0].Normals == null)
            {
                if (MeshParts[0].TextureCoordinates == null)
                {
                    int vxcount = getAllVxCount();
                    VertexPositionColor[] vx1 = new VertexPositionColor[vxcount];
                    int pp = 0;
                    for (int x = 0; x < MeshParts.Count; x++)
                    {
                        for (int i = 0; i < MeshParts[x].Positions.Length; i++)
                        {
                            vx1[pp] = new VertexPositionColor(
                                MeshParts[x].Positions[i],
                                MeshParts[x].Colors[i]
                                );
                            pp++;
                        }
                    }
                    VertexBufferFactory.CreateVertexBuffer<VertexPositionColor>(
                        Constants.ar3d.GraphicsDevice,
                        vx1, out vertexDeclaration, out vertexBuffer, out sizeInBytes);

                    VertexCount = vx1.Length;
                    TriangleCount = VertexCount/3;
                }
                else
                {
                    int vxcount = getAllVxCount();
                    VertexPositionColorTexture[] vx1 = new VertexPositionColorTexture[vxcount];
                    int pp = 0;
                    for (int x = 0; x < MeshParts.Count; x++)
                    {
                        for (int i = 0; i < MeshParts[x].Positions.Length; i++)
                        {
                            vx1[pp] = new VertexPositionColorTexture(
                                MeshParts[x].Positions[i],
                                MeshParts[x].Colors[i],
                                MeshParts[x].TextureCoordinates[i]
                                );
                            pp++;
                        }
                    }
                    VertexBufferFactory.CreateVertexBuffer<VertexPositionColorTexture>(
                        Constants.ar3d.GraphicsDevice,
                        vx1, out vertexDeclaration, out vertexBuffer, out sizeInBytes);
                    VertexCount = vx1.Length;
                    TriangleCount = VertexCount/3;
                }
            }
            else
            {
                if (MeshParts[0].TextureCoordinates == null)
                {
                    int vxcount = getAllVxCount();
                    VertexPositionNormalColor[] vx1 = new VertexPositionNormalColor[vxcount];
                    int pp = 0;
                    for (int x = 0; x < MeshParts.Count; x++)
                    {
                        for (int i = 0; i < MeshParts[x].Positions.Length; i++)
                        {
                            vx1[pp] = new VertexPositionNormalColor();
                            vx1[pp].Position = MeshParts[x].Positions[i];
                            vx1[pp].Normal = MeshParts[x].Normals[i];
                            vx1[pp].Color = MeshParts[x].Colors[i];

                            pp++;
                        }
                    }
                    VertexBufferFactory.CreateVertexBuffer<VertexPositionNormalColor>(
                        Constants.ar3d.GraphicsDevice,
                        vx1, out vertexDeclaration, out vertexBuffer, out sizeInBytes);
                    VertexCount = vx1.Length;
                    TriangleCount = VertexCount/3;
                }
            }
            List<int> allIndices = new List<int>();
            int add = 0;
            for (int x = 0; x < MeshParts.Count; x++)
            {
                for (int i = 0; i < MeshParts[x].Indices.Length; i++)
                {
                    allIndices.Add((MeshParts[x].Indices[i] + add));
                }
                add += MeshParts[x].Indices.Length;
            }
            Indices = allIndices.ToArray();
            indexBuffer = new IndexBuffer(Constants.ar3d.GraphicsDevice, 4*Indices.Length, BufferUsage.None,
                                          IndexElementSize.ThirtyTwoBits);
            indexBuffer.SetData<int>(Indices);
        }

        public override void Update(GameTime gameTime)
        {
        }

        public void CreateBounds()
        {
            BoundingSphere = MeshParts[0].BoundingSphere;

            for (int i = 1; i < MeshParts.Count; i++)
            {
                BoundingSphere = BoundingSphere.CreateMerged(BoundingSphere,
                                                             MeshParts[i].BoundingSphere);
            }

            BoundingBox = BoundingBox.CreateFromSphere(BoundingSphere);
        }

        public void DrawText(Vector3 pos, string texto)
        {
            Vector3 center = Constants.ar3d.GraphicsDevice.Viewport.Project(
                pos,
                Constants.Camera.Projection,
                Constants.Camera.View,
                Matrix.Identity
                );
            if (Constants.Camera.Projection.Forward.Z < center.Z)
            {
                return;
            }

            Vector2 vc = Constants.Get2DCoords(pos);
            Vector2 textSize = Constants.Render2D.Fonts[2].MeasureString(texto);

            //int centerXPosition = (Constants.ar3d.graphics.PreferredBackBufferWidth / 2) - ((int)textSize.X / 2);
            vc.X -= textSize.X / 3;
            float sumar = Constants.Camera.Position.Y /1000;
            Constants.Render2D.DrawString(texto,vc,Color.Black, 1,0, 0.99999f, 2);
        }


        public void Draw2d(GameTime gameTime)
        {
            for (int i = 0; i < MeshParts.Count; i++)
            {
                DrawText(MeshParts[i].BoundingSphere.Center, MeshParts[i].Name);
            }
        }


        private bool isVisible(int i)
        {
            if (Constants.Camera.Frustum.Contains(
                    MeshParts[i].BoundingSphere) != ContainmentType.Disjoint)
            {
                return true;
            }

            return false;
        }


        public override void Draw(GameTime gameTime)
        {
            if(BigBufferBuilt){
                Constants.GraphicsDevice.Indices = indexBuffer;
                Constants.GraphicsDevice.Vertices[0].SetSource(vertexBuffer, 0, sizeInBytes);
                Constants.GraphicsDevice.DrawIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    0,
                    0, VertexCount,
                    0,
                   TriangleCount);
            }else{
                Visible = true;
                for (int i = 0; i < MeshParts.Count; i++)
                {
                    if (!CollisionHelper.isVisible(MeshParts[i].BoundingSphere))
                    {
                        MeshParts[i].Visible = false;
                        continue;
                    }
                    // isPicked(i);
                    MeshParts[i].Visible = true;
                    MeshParts[i].Draw(gameTime);
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace b3d{
    public class MeshComponentPart : RenderableComponent
    {
        
        public IndexBuffer indexBuffer;
        public VertexDeclaration vertexDeclaration;
       
        public VertexBuffer vertexBuffer;

        public short sizeInBytes;
        public int VertexCount;
        public int TriangleCount;
      
         public int[] Indices;
        public Vector3[] Positions;
        public Color[] Colors;
        public Vector3[] Normals;
        public Vector2[] TextureCoordinates;
        public string Codigo;

        public override void Dispose()
        {
        }

        public MeshComponentPart(string name, Vector3[] positions, Color[] colors, Vector3[] normals,
                                 Vector2[] texturecoordinates, int[] indices)
            : base(name)
        {
            Positions = positions;
            Colors = colors;
            Normals = normals;
            TextureCoordinates = texturecoordinates;
            Indices = indices;
            CreateBounds();
        }

        protected MeshComponentPart(){
            
        }

        protected MeshComponentPart(string name):base(name){
        }

        public void CreateBounds()
        {
            if (Positions.Length > 0){
                //PickingVertices = Positions;
                BoundingBox = BoundingBox.CreateFromPoints(Positions);
                BoundingSphere = BoundingSphere.CreateFromBoundingBox(BoundingBox);
            }
        }


        public override void Initialize()
        {
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(GameTime gameTime)
        {
            
        }
    }
}
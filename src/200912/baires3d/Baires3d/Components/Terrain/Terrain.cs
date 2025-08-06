#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace ar3d
{
    /// <summary>
    /// Terrain game component.
    /// Original ideas: Harald Vistnes (mailto:harald@vistnes.org)
    /// Author: Sergey Akopkokhyants (mailto:akserg@gmail.com)
    /// </summary>
    public class Terrain
    {
        protected const float MAX_ELEVATION = 128;

        GraphicsDevice device;
        VertexBuffer vb;
        IndexBuffer ib;
        VertexPositionTerrain[] vertices;
        int[] indices;

        int vertexCount;
        int indexCount;

        VertexDeclaration vertexDecl;

        public int BlockSize = 17;
        protected short maxLevel;
        public float LOD = 60.0f;

        Effect gridEffect;
        Texture2D displacementTexture, displacementNormal;
        Texture2D sandTexture, grassTexture, rockTexture, snowTexture;

        protected const float MIN_U = 0.0f;
        protected const float MIN_V = 0.0f;
        protected const float MAX_U = 1.0f;
        protected const float MAX_V = 1.0f;
        protected const float DEFAULT_SCALE = 1.0f;

        private Camera camera;

        public Vector4 vLightDir = new Vector4(1.0f, -1.0f, 0.5f, 0.0f);

        public int NumTriangles = 0;
        public int NumCulled = 0;

        public Terrain()
            : this(17)
        { }

        public Terrain(int BlockSize)
        {
            this.BlockSize = BlockSize;
        }
        

        public void LoadGraphicsContent(GraphicsDevice device, Effect gridEffect, Texture2D displacementTexture,
            Texture2D displacementNormal, Texture2D sandTexture, Texture2D grassTexture, Texture2D rockTexture, Texture2D snowTexture)
        {
            this.gridEffect = gridEffect;
            this.displacementTexture = displacementTexture;
            this.displacementNormal = displacementNormal;
            this.sandTexture = sandTexture;
            this.grassTexture = grassTexture;
            this.rockTexture = rockTexture;
            this.snowTexture = snowTexture;

            this.device = device;

            generateStructures();

            this.maxLevel = CalculateMaxLevels();
        }

        protected void generateStructures()
        {
            vertices = new VertexPositionTerrain[this.BlockSize * this.BlockSize
                + 2 * this.BlockSize + 2 * this.BlockSize - 4];

            float invScaleX = 1.0f / (this.BlockSize - 1.0f);
            float invScaleZ = 1.0f / (this.BlockSize - 1.0f);
            int indx = 0;
            float s, t;
            VertexPositionTerrain v;
            // Fill regular grid
            for (int i = 0; i < this.BlockSize; i++)
            {
                t = i * invScaleZ;
                for (int j = 0; j < this.BlockSize; j++)
                {
                    s = j * invScaleX;
                    v = new VertexPositionTerrain();
                    v.Position = new Vector3(s, 1, t);
                    vertices[indx++] = v;
                }
            }
            // top. left to right
            t = 1.0f;
            for (int j = 0; j < this.BlockSize; j++)
            {
                s = j * invScaleX;
                v = new VertexPositionTerrain();
                v.Position = new Vector3(s, -1, t);
                vertices[indx++] = v;
            }
            // right. up to down
            s = 1.0f;
            for (int i = this.BlockSize - 2; i > 0; i--)
            {
                t = i * invScaleZ;
                v = new VertexPositionTerrain();
                v.Position = new Vector3(s, -1, t);
                vertices[indx++] = v;
            }
            // bottom. right to left
            t = 0.0f;
            for (int j = this.BlockSize - 1; j >= 0; j--)
            {
                s = j * invScaleX;
                v = new VertexPositionTerrain();
                v.Position = new Vector3(s, -1, t);
                vertices[indx++] = v;
            }
            // left. down to up. without last
            s = 0.0f;
            for (int i = 1; i < this.BlockSize - 1; i++)
            {
                t = i * invScaleZ;
                v = new VertexPositionTerrain();
                v.Position = new Vector3(s, -1, t);
                vertices[indx++] = v;
            }
            //
            vertexCount = vertices.Length;
            vb = new Microsoft.Xna.Framework.Graphics.VertexBuffer(device,
                    VertexPositionTerrain.SizeInBytes * vertices.Length, BufferUsage.WriteOnly);
            vb.SetData<VertexPositionTerrain>(vertices);
            vertexDecl = new VertexDeclaration(device, VertexPositionTerrain.VertexElements);
            //
            short uiNumStrips = (short)(BlockSize - 1);
            indices = new int[2 * this.BlockSize * (this.BlockSize - 1) + 8 * (this.BlockSize - 1)];
            indx = 0;
            int z = 0;
            // fill regular grid
            while (z < this.BlockSize - 1)
            {
                for (short x = 0; x < this.BlockSize; x++)
                {
                    indices[indx++] = x + z * this.BlockSize;
                    indices[indx++] = x + (z + 1) * this.BlockSize;
                }
                z++;

                if (z < this.BlockSize - 1)
                {
                    for (int x = this.BlockSize - 1; x >= 0; x--)
                    {
                        indices[indx++] = x + (z + 1) * this.BlockSize;
                        indices[indx++] = x + z * this.BlockSize;
                    }
                }
                z++;
            }
            int num = this.BlockSize * this.BlockSize;
            // top, left to right
            for (int i = 0; i < this.BlockSize; i++)
            {
                indices[indx++] = num++;
                indices[indx++] = (this.BlockSize - 1) * this.BlockSize + i;
            }
            // right. up to down
            for (int j = 2; j < this.BlockSize; j++)
            {
                indices[indx++] = num++;
                indices[indx++] = (this.BlockSize - j) * this.BlockSize + this.BlockSize - 1;
            }
            // bottom. right to left
            for (int i = 0; i < this.BlockSize; i++)
            {
                indices[indx++] = num++;
                indices[indx++] = this.BlockSize - i - 1;
            }
            // left. down to up
            for (int j = 1; j < this.BlockSize - 1; j++)
            {
                indices[indx++] = num++;
                indices[indx++] = j * this.BlockSize;
            }
            //
            indexCount = indices.Length - 2;
            ib = new IndexBuffer(device, typeof(int), indices.Length, BufferUsage.WriteOnly);
            ib.SetData(indices);
        }

        private short CalculateMaxLevels()
        {
            int iPow2 = (this.displacementTexture.Width - 1) / (this.BlockSize - 1);
            short iLevels = 0;
            for (int i = 1; i < iPow2; i *= 2)
            {
                iLevels++;
            }
            return iLevels;
        }

        public void Draw(bool showBoundingBox)
        {
            this.camera = camera;

            device.RenderState.CullMode = CullMode.CullClockwiseFace;

            device.VertexDeclaration = vertexDecl;
            device.Vertices[0].SetSource(vb, 0, VertexPosition.SizeInBytes);
            device.Indices = ib;

            Matrix worldViewProj = 
                //Matrix.Identity 
                Matrix.CreateTranslation(new Vector3(Constants.Camera.Position.X, 0 ,Constants.Camera.Position.Y))
                * Constants.Camera.View * Constants.Camera.Projection;
            gridEffect.Parameters["worldViewProj"].SetValue(worldViewProj);
            gridEffect.Parameters["maxHeight"].SetValue(MAX_ELEVATION);
            gridEffect.Parameters["displacementMap"].SetValue(displacementTexture);
            gridEffect.Parameters["NormalMap"].SetValue(displacementNormal);
            gridEffect.Parameters["displacementWidth"].SetValue(displacementTexture.Width);
            gridEffect.Parameters["displacementHeight"].SetValue(displacementTexture.Height);
            gridEffect.Parameters["vecLightDir"].SetValue(vLightDir);
            

            gridEffect.Parameters["sandMap"].SetValue(sandTexture);
            gridEffect.Parameters["grassMap"].SetValue(grassTexture);
            gridEffect.Parameters["rockMap"].SetValue(rockTexture);
            gridEffect.Parameters["snowMap"].SetValue(snowTexture);

            NumTriangles = 0;
            NumCulled = 0;
            Render(MIN_U, MIN_V, MAX_U, MAX_V, maxLevel, DEFAULT_SCALE);
            if (showBoundingBox)
                Render_BB(MIN_U, MIN_V, MAX_U, MAX_V, maxLevel, DEFAULT_SCALE);
        }

        private void Render(float fMinU, float fMinV, 
                            float fMaxU, float fMaxV, 
                            int iLevel, float fScale)
        {
            Vector3 Min = new Vector3(fMinU * displacementTexture.Width, 
                                      0, 
                                      fMinV * displacementTexture.Height);

            Vector3 Max = new Vector3(fMaxU * displacementTexture.Width, 
                                      MAX_ELEVATION, 
                                      fMaxV * displacementTexture.Height);

            BoundingBox boundingBox = new BoundingBox(Min, Max);

            if (Constants.Camera.Frustum != null &&
                Constants.Camera.Frustum.Contains(boundingBox) == ContainmentType.Disjoint)
            {
                NumCulled++;
                return;
            }

            float fHalfU = (fMinU + fMaxU) / 2.0f;
            float fHalfV = (fMinV + fMaxV) / 2.0f;

            bool criterioResult = evaluateCriterion(
                    fHalfU, fHalfV, fMinU, fMaxU, iLevel);

            if (criterioResult)
                Draw(fMinU, fMinV, fScale, iLevel);
            else
            {
                // We need to continue dividing. Decrease scale for next level...
                fScale = fScale / 2.0f;
                // and reduse level as well.
                iLevel--;
                // Continue process with each quadrant - chunk.
                Render(fMinU, fMinV, fHalfU, fHalfV, iLevel, fScale);
                //Draw(fMinU, fMinV, fScale, iLevel);
                Render(fHalfU, fMinV, fMaxU, fHalfV, iLevel, fScale);
                //Draw(fHalfU, fMinV, fScale, iLevel);
                Render(fMinU, fHalfV, fHalfU, fMaxV, iLevel, fScale);
                //Draw(fMinU, fHalfV, fScale, iLevel);
                Render(fHalfU, fHalfV, fMaxU, fMaxV, iLevel, fScale);
                //Draw(fHalfU, fHalfV, fScale, iLevel);
            }
        }

        private void Render_BB(float fMinU, float fMinV, 
                               float fMaxU, float fMaxV, 
                               int iLevel, float fScale)
        {
            Vector3 Min = new Vector3(fMinU * displacementTexture.Width, 
                                      0, 
                                      fMinV * displacementTexture.Height);

            Vector3 Max = new Vector3(fMaxU * displacementTexture.Width, 
                                      MAX_ELEVATION, 
                                      fMaxV * displacementTexture.Height);

            BoundingBox boundingBox = new BoundingBox(Min, Max);
            if (Constants.Camera.Frustum != null &&
                Constants.Camera.Frustum.Contains(boundingBox) == ContainmentType.Disjoint)
            {
                return;
            }

            float fHalfU = (fMinU + fMaxU) / 2.0f;
            float fHalfV = (fMinV + fMaxV) / 2.0f;

            bool criterioResult = evaluateCriterion(
                fHalfU, fHalfV, fMinU, fMaxU, iLevel);

            if (criterioResult)
            {
                drawBoundingBox(fMinU, fMinV, fMaxU, fMaxV, iLevel, fScale);
            }
            else
            {
                // We need to continue dividing. Decrease scale for next level...
                fScale = fScale / 2.0f;
                // and reduse level as well.
                iLevel--;
                // Continue process with each quadrant - chunk.
                Render_BB(fMinU, fMinV, fHalfU, fHalfV, iLevel, fScale);
                Render_BB(fHalfU, fMinV, fMaxU, fHalfV, iLevel, fScale);
                Render_BB(fMinU, fHalfV, fHalfU, fMaxV, iLevel, fScale);
                Render_BB(fHalfU, fHalfV, fMaxU, fMaxV, iLevel, fScale);
            }
        }

        private void Draw(float fBiasU, float fBiasV, float fScale, int iLevel)
        {
            gridEffect.Parameters["fBiasU"].SetValue(fBiasU);
            gridEffect.Parameters["fBiasV"].SetValue(fBiasV);
            gridEffect.Parameters["fScale"].SetValue(fScale);

            gridEffect.Begin();
            foreach (EffectPass pass in gridEffect.CurrentTechnique.Passes)
            {
                pass.Begin();

                NumTriangles += indexCount;

                device.DrawIndexedPrimitives(
                        PrimitiveType.TriangleStrip, 
                        0, 
                        0,
                        vertexCount, 
                        0, 
                        indexCount);

                pass.End();
            }
            gridEffect.End();
        }

        private bool evaluateCriterion(float fHalfU, float fHalfV, 
                                       float fMinU, float fMaxU, 
                                       int iLevel)
        {
            // We use simple distance-based evaluation criterion
            // World-space extent of triangle
            float extentOfTriangle = 
                (fMaxU - fMinU) * this.displacementTexture.Width 
                    / (this.BlockSize - 1.0f);

            // Use it for calculate square
            float extentOfTriangle2 = extentOfTriangle * extentOfTriangle;
            
            Vector3 chunkPosition = new Vector3(
                    fHalfU * this.displacementTexture.Width, 
                    Constants.Camera.Position.Y, 
                    fHalfV * this.displacementTexture.Height);


            Vector3 cameraDistance = chunkPosition - Constants.Camera.Position;

            float length2 = cameraDistance.LengthSquared();
            // use distances squared
            float criterio2 = length2 / extentOfTriangle2;

            bool result = (criterio2 > this.LOD * this.LOD || iLevel < 1);
            
            return result;
        }

        private void drawBoundingBox(float fMinU, float fMinV, 
                                     float fMaxU, float fMaxV, 
                                     int iLevel, float fScale)
        {
            Vector3 Min = new Vector3(fMinU * displacementTexture.Width, 
                                      0, 
                                      fMinV * displacementTexture.Height);

            Vector3 Max = new Vector3(fMaxU * displacementTexture.Width, 
                                      MAX_ELEVATION, 
                                      fMaxV * displacementTexture.Height);
            BoundingBox boundingBox = new BoundingBox(Min, Max);

            drawBoundingBox(boundingBox, 
                            device, 
                            Matrix.Identity,
                            Constants.Camera.View, 
                            Constants.Camera.Projection, 
                            iLevel);

        }

        protected BasicEffect basicEffect;

        static Color[] colors = 
        { 
            Color.Red, 
            Color.Orange, 
            Color.Yellow, 
            Color.Green,
            Color.Blue, 
            Color.DarkBlue, 
            Color.Violet, 
            Color.LightPink, 
            Color.LimeGreen,
            Color.Maroon, 
            Color.MediumPurple, 
            Color.MistyRose, 
            Color.Olive
        };

        private void drawBoundingBox(
            BoundingBox bBox, 
            GraphicsDevice device, 
            Matrix worldMatrix, 
            Matrix viewMatrix, 
            Matrix projectionMatrix, 
            int level)
        {
            Color selColor = colors[level];
            // Prepare Data
            Vector3 v1 = bBox.Min;
            Vector3 v2 = bBox.Max;

            VertexPositionColor[] cubeLineVertices =
                new VertexPositionColor[8]
            {
                new VertexPositionColor(v1, Color.White),
                new VertexPositionColor(new Vector3(v2.X, v1.Y, v1.Z), selColor),
                new VertexPositionColor(new Vector3(v2.X, v1.Y, v2.Z), selColor),
                new VertexPositionColor(new Vector3(v1.X, v1.Y, v2.Z), selColor),

                new VertexPositionColor(new Vector3(v1.X, v2.Y, v1.Z), selColor),
                new VertexPositionColor(new Vector3(v2.X, v2.Y, v1.Z), selColor),
                new VertexPositionColor(v2, selColor),
                new VertexPositionColor(new Vector3(v1.X, v2.Y, v2.Z), selColor),
            };

            short[] cubeLineIndices = 
            { 
                0, 1, 1, 
                2, 2, 3, 
                3, 0, 4, 
                5, 5, 6, 
                6, 7, 7, 
                4, 0, 4, 
                1, 5, 2, 
                6, 3, 7 
            };

            // Prepare Effect
            if (basicEffect == null)
            {
                basicEffect = new BasicEffect(device, null);
            }
            basicEffect.World = worldMatrix;
            basicEffect.View = viewMatrix;
            basicEffect.Projection = projectionMatrix;
            basicEffect.VertexColorEnabled = true;
            // Draw
            basicEffect.Begin();
            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Begin();
                device.VertexDeclaration = 
                    new VertexDeclaration(device, 
                        VertexPositionColor.VertexElements);

                device.DrawUserIndexedPrimitives<VertexPositionColor>(
                            PrimitiveType.LineList, 
                            cubeLineVertices, 
                            0, 
                            8, 
                            cubeLineIndices, 
                            0, 
                            12);
                pass.End();
            }
            basicEffect.End();
        }
    }
}


using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace b3d
{
    [System.Diagnostics.DebuggerDisplay("{Position} C: {Color}")]
    [Serializable]
    public struct PositionNormalTextureColor
    {
        public static PositionNormalTextureColor FromInt32(int x)
        {
            using (BinaryReader reader = new BinaryReader(new MemoryStream(BitConverter.GetBytes(x))))
            {
                float pX = reader.ReadSingle();
                float pY = reader.ReadSingle();
                float pZ = reader.ReadSingle();

                Vector3 Position = new Vector3(
                    pX,
                    pY,
                    pZ
                    );

                float nX = reader.ReadSingle();
                float nY = reader.ReadSingle();
                float nZ = reader.ReadSingle();

                Vector3 Normal = new Vector3(
                    nX,
                    nY,
                    nZ
                    );

                float tX = reader.ReadSingle();
                float tY = reader.ReadSingle();
                Vector2 TextureCoordinate = new Vector2(
                    tX,
                    tY
                    );
                System.Drawing.Color clr = System.Drawing.Color.FromArgb(reader.ReadInt32());


                Color Color = new Color(clr.R, clr.G, clr.B);

                //Int16 b = reader.ReadInt16();
                //byte c = reader.ReadByte();
                return new PositionNormalTextureColor(Position, Normal, TextureCoordinate, Color);
            }
        }

        public int ToInt32()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryWriter w = new BinaryWriter(ms);
                w.Write(Position.X);
                w.Write(Position.Y);
                w.Write(Position.Z);

                w.Write(Normal.X);
                w.Write(Normal.Y);
                w.Write(Normal.Z);
                w.Write(TextureCoordinate.X);
                w.Write(TextureCoordinate.Y);

                w.Write(Color.PackedValue);


                w.Flush();
                return BitConverter.ToInt32(ms.ToArray(), 0);
            }
        }

        public static void PackToStream(PositionNormalTextureColor[] values, Stream stream)
        {
            int[] packed = new int[values.Length];
            for (int i = 0; i < values.Length; ++i)
            {
                packed[i] = values[i].ToInt32();
            }

            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, packed);
        }

        public static PositionNormalTextureColor[] UnpackFromStream(Stream stream)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            int[] packed = (int[]) formatter.Deserialize(stream);
            PositionNormalTextureColor[] values = new PositionNormalTextureColor[packed.Length];
            for (int i = 0; i < packed.Length; ++i)
            {
                values[i] = PositionNormalTextureColor.FromInt32(packed[i]);
            }
            return values;
        }


        /// <summary>
        /// The position of the vertex.
        /// </summary>
        //[FieldOffset(0)]
        public Vector3 Position;

        /// <summary>
        /// The texture coordinate0.
        /// </summary>
        //[FieldOffset(System.Runtime.InteropServices.Marshal.SizeOf(Vector3.))]
        //[FieldOffset(12)]
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
        public PositionNormalTextureColor(Vector3 position, Vector3 normal, Vector2 texCoord1, Color color)
        {
            this.Position = position;
            this.Normal = normal;
            this.TextureCoordinate = texCoord1;
            this.Color = color;
        }

        public PositionNormalTextureColor(Vector3 position, Vector3 normal, Vector2 texCoord1)
        {
            this.Position = position;
            this.Normal = normal;
            this.TextureCoordinate = texCoord1;
            this.Color = Color.White;
        }


        public PositionNormalTextureColor(Vector3 position, Vector3 normal)
        {
            this.Position = position;
            this.Normal = normal;
            this.TextureCoordinate = Vector2.Zero;
            this.Color = Color.White;
        }

        public PositionNormalTextureColor(Vector3 position)
        {
            this.Position = position;
            this.Normal = Vector3.UnitY;
            this.TextureCoordinate = Vector2.Zero;
            this.Color = Color.White;
        }

        public PositionNormalTextureColor(Vector3 position, Color color)
        {
            this.Position = position;
            this.Normal = Vector3.UnitY;
            this.TextureCoordinate = Vector2.Zero;
            this.Color = color;
        }
        public override int GetHashCode(){
            string str = (
                Position.GetHashCode().ToString()
                + Position.X.ToString()
                + Position.Y.ToString()
                + Position.Z.ToString()
                + Normal.X.ToString()
                + Normal.Y.ToString()
                + Normal.Z.ToString()
                );
            str = str.Replace("-", "").Replace(",", "").Replace(".", "");

            return this.GetHashCode();
        }
    }
    
    [System.Diagnostics.DebuggerDisplay("{Position} Color: {Color}")]
    public struct VertexPositionNormalColor
    {
        public Vector3 Position;
        public Color Color;
        public Vector3 Normal;

        public static int SizeInBytes = 7*4;

        public static VertexElement[] VertexElements = new VertexElement[]
                                                           {
                                                               new VertexElement(0, 0,
                                                                                 VertexElementFormat.
                                                                                     Vector3,
                                                                                 VertexElementMethod.
                                                                                     Default,
                                                                                 VertexElementUsage.
                                                                                     Position, 0),
                                                               new VertexElement(0, sizeof (float)*3,
                                                                                 VertexElementFormat.
                                                                                     Color,
                                                                                 VertexElementMethod.
                                                                                     Default,
                                                                                 VertexElementUsage.Color,
                                                                                 0),
                                                               new VertexElement(0, sizeof (float)*4,
                                                                                 VertexElementFormat.
                                                                                     Vector3,
                                                                                 VertexElementMethod.
                                                                                     Default,
                                                                                 VertexElementUsage.
                                                                                     Normal, 0),
                                                           };

        public VertexPositionNormalColor(Vector3 position, Vector3 normal, Color color){
            Position = position;
            Color = color;
            Normal = normal;
        }
    }
}
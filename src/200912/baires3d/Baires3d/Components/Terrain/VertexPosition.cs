using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ar3d
{
    /// <summary>
    /// VertexPosition structure for sending only vertex position to GPU.
    /// Author: Sergey Akopkokhyants (mailto:akserg@gmail.com)
    /// </summary>
    public struct VertexPositionTerrain
    {
        public Vector3 Position;

        public VertexPositionTerrain(Vector3 Position)
        {
            this.Position = Position;
        }

        public static readonly int SizeInBytes = sizeof(float) * 3;

        public static readonly VertexElement[] VertexElements =
        {
            new VertexElement(
                0, 
                0, 
                VertexElementFormat.Vector3, 
                VertexElementMethod.Default, 
                VertexElementUsage.Position, 0),
        };
    }
}

//-----------------------------------------------------------------------
// <copyright file="VertexBufferFactory.cs" company="Jendrik Illner">
// Jendrik Illner(jendrik.illner@gmail.com)
// Creative Commons Attribution 3.0 United States License. (http://creativecommons.org/licenses/by/3.0/us/)
// </copyright>
//-----------------------------------------------------------------------

namespace b3d
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// The vertex buffer factory creates 
    /// </summary>
    public static class VertexBufferFactory
    {
        /// <summary>
        /// Creates the vertex buffer.
        /// </summary>
        /// <typeparam name="T">The type of the vertex.</typeparam>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="vertices">The vertices.</param>
        /// <param name="vertexDeclaration">The vertex declaration.</param>
        /// <param name="vertexBuffer">The vertex buffer.</param>
        /// <param name="sizeInBytes">The size in bytes per vertex.</param>
        public static void CreateVertexBuffer<T>(GraphicsDevice graphicsDevice, T[] vertices,
                                                 out VertexDeclaration vertexDeclaration, out VertexBuffer vertexBuffer,
                                                 out short sizeInBytes) where T : struct
        {
            vertexDeclaration = VertexDeclarationFactory.GenerateVertexDeclaration<T>(graphicsDevice, out sizeInBytes);
            vertexBuffer = new VertexBuffer(graphicsDevice, sizeInBytes*vertices.Length, BufferUsage.None);
            vertexBuffer.SetData(vertices);
        }
    }
}
//-----------------------------------------------------------------------
// <copyright file="VertexElementAttribute.cs" company="Jendrik Illner">
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
    /// This attribute is used to describe elements of a vertex structure.
    /// When the vertex declaration factory is not able to get the meaning.
    /// </summary>
    public class VertexElementAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VertexElementAttribute"/> class.
        /// </summary>
        /// <param name="recognize">if set to <c>true</c> [recognize] the element for the creation.</param>
        public VertexElementAttribute(bool recognize)
        {
            // recognize is the opposite of ignore so invert it
            // we use recognize for the constructor because otherwise it looks incorrect when use [VertexElementAttribute(true)] to ignore an element
            this.IgnoreElement = !recognize;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexElementAttribute"/> class.
        /// </summary>
        /// <param name="elementUsage">The element usage.</param>
        public VertexElementAttribute(VertexElementUsage elementUsage)
        {
            this.ElementUsage = elementUsage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexElementAttribute"/> class.
        /// </summary>
        /// <param name="elementUsage">The element usage.</param>
        /// <param name="usageIndex">The usage index.</param>
        public VertexElementAttribute(VertexElementUsage elementUsage, byte usageIndex)
        {
            this.ElementUsage = elementUsage;
            this.UsageIndex = usageIndex;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [ignore element].
        /// </summary>
        /// <value><c>true</c> if [ignore element]; otherwise, <c>false</c>.</value>
        public bool IgnoreElement { get; set; }

        /// <summary>
        /// Gets or sets the element usage.
        /// </summary>
        /// <value>The element usage.</value>
        public VertexElementUsage ElementUsage { get; set; }

        /// <summary>
        /// Gets or sets the usage index.
        /// </summary>
        /// <value>The usage index.</value>
        public byte UsageIndex { get; set; }
    }
}
//
//      coded by un
//            --------------
//                     mindshifter.com
//

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Mindshifter
{
    /// <summary>
    /// Represents a transformation for an object that posesses a local
    /// space bounding box
    /// </summary>
    public interface IBoundedTransform : ITransform
    {
        /// <summary>
        /// Gets the bounds of the transform
        /// </summary>
        BoundingBox Bounds { get; }
    }
}
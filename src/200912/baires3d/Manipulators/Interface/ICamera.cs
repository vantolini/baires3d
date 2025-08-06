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
    /// Defines an interface for retrieving camera view and projection matrices
    /// </summary>
    public interface ICamera
    {
        /// <summary>
        /// Gets the camera's view matrix
        /// </summary>
        Matrix ViewMatrix { get; }

        /// <summary>
        /// Gets the camera's projection matrix
        /// </summary>
        Matrix ProjectionMatrix { get; }
    }
}
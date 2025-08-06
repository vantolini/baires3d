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
    /// Represents a 3D geometric transformation through separate
    /// translation, rotation and scale components
    /// </summary>
    public interface ITransform
    {
        /// <summary>
        /// Gets or sets the translation component of the transformation
        /// </summary>
        Vector3         Translation     { get; set; }

        /// <summary>
        /// Gets or sets the rotation component of the transformation
        /// </summary>
        Quaternion      Rotation        { get; set; }

        /// <summary>
        /// Gets or sets the scale component of the transformation
        /// </summary>
        Vector3         Scale           { get; set; }
    }
}
//
//      coded by un
//            --------------
//                     mindshifter.com
//


using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mindshifter
{
    // TransformState is used for undo and redo operations by providing
    // a stored version of an ITransform's transformation state
    internal sealed class TransformState
    {
        private ITransform  mTransform;

        public Vector3      mTranslation;
        public Quaternion   mRotation;
        public Vector3      mScale;

        /// <summary>
        /// Creates a new instance of TransformState and caches the
        /// transformation components of the specified ITransform
        /// </summary>
        /// <param name="tform">The ITransform object whose transformation components
        /// will be cached by the TransformState</param>
        public TransformState(ITransform tform)
        {
            mTransform = tform;

            mTranslation = tform.Translation;
            mRotation = tform.Rotation;
            mScale = tform.Scale;
        }

        /// <summary>
        /// Applies the cached transformation components of the TransformState
        /// to the corresponding ITransform object
        /// </summary>
        public void Apply()
        {
            mTransform.Translation = mTranslation;
            mTransform.Rotation = mRotation;
            mTransform.Scale = mScale;
        }
    }
}
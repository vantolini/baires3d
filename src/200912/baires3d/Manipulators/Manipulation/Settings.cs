using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mindshifter
{
    /// <summary>
    /// Helper class that automates setting and restoring default values
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Creates a new instance of Settings
        /// </summary>
        protected Settings()
        {

        }

        /// <summary>
        /// Restores the default values of the settings based on any DefaultValue attributes
        /// assigned to the object's fields
        /// </summary>
        public void RestoreDefaults()
        {
            // Get top level type of the object
            Type t = this.GetType();

            // Get all of the object's private fields
            FieldInfo[] fields = t.GetFields(BindingFlags.NonPublic 
                | BindingFlags.Public | BindingFlags.Instance);

            // Iterate over all fields and look for DefaultValue attributes
            foreach (FieldInfo fi in fields)
            {
                // find all default attributes
                object[] attrlist = fi.GetCustomAttributes(typeof(DefaultValueAttribute), true);

                // no attribute found, continue
                if (attrlist.Length == 0)
                    continue;

                // get the appropriate attribute type and set its value on the field
                DefaultValueAttribute def = (DefaultValueAttribute)attrlist[0];
                object value = def.Value;

                if (value != null)
                {
                    attrlist = fi.GetCustomAttributes(typeof(TypeConverterAttribute), true);
                    if (attrlist.Length > 0)
                    {
                        TypeConverterAttribute cattr = (TypeConverterAttribute)attrlist[0];
                        Type ctype = Type.GetType(cattr.ConverterTypeName);

                        if(ctype != null)
                        {
                            TypeConverter conv = (TypeConverter)Activator.CreateInstance(ctype);
                            if ((conv != null) && conv.CanConvertFrom(value.GetType()))
                                value = conv.ConvertFrom(value);
                        }
                    }
                }

                fi.SetValue(this, value);
            }
        }
    }

    public class ManipulatorSettings : Settings
    {
        /// <summary>
        /// Encapsulates translation-specific manipulator settings
        /// </summary>
        public class TranslationSettings : Settings
        {
            [DefaultValue(0.05f)]
            private float           mAxisRadius;

            [DefaultValue(1.5f)]
            private float           mAxisExtent;

            [DefaultValue(0.1f)]
            private float           mConeRadius;

            [DefaultValue(0.5f)]
            private float           mConeHeight;

            [DefaultValue(0.75f)]
            private float           mPlaneQuadrantSize;

            [DefaultValue(AxisDirections.Positive)]
            private AxisDirections   mAxisDrawMode;

            /// <summary>
            /// Gets or sets the radius of the cylinder used to represent
            /// each translation axis
            /// </summary>
            public float AxisRadius
            {
                get { return mAxisRadius; }
                set 
                { 
                    mAxisRadius = MathHelper.Clamp(value, 0, float.MaxValue);
                }
            }

            /// <summary>
            /// Gets or sets the extents of the cylinder used to represent
            /// each translation axis
            /// </summary>
            public float AxisExtent
            {
                get { return mAxisExtent; }
                set 
                {
                    mAxisExtent = MathHelper.Clamp(value, 0, float.MaxValue);
                }
            }

            /// <summary>
            /// Gets or sets the radius of the base of the cone used to
            /// represent the direction of each translation axis
            /// </summary>
            public float ConeRadius
            {
                get { return mConeRadius; }
                set
                {
                    mConeRadius = MathHelper.Clamp(value, 0, float.MaxValue);
                }
            }

            /// <summary>
            /// Gets or sets the height/length of the cone used to represent
            /// the direction of each translation axis
            /// </summary>
            public float ConeHeight
            {
                get { return mConeHeight; }
                set
                {
                    mConeHeight = MathHelper.Clamp(value, 0, float.MaxValue);
                }
            }

            public float PlaneQuadrantSize
            {
                get { return mPlaneQuadrantSize; }
                set
                {
                    mPlaneQuadrantSize = MathHelper.Clamp(value, 0, float.MaxValue);
                }
            }

            /// <summary>
            /// Gets or sets the directions of each axis that will be drawn by the manipulator
            /// </summary>
            public AxisDirections AxisDrawMode
            {
                get { return mAxisDrawMode; }
                set { mAxisDrawMode = value; }
            }

            /// <summary>
            /// Creates a new instance of TranslationSettings
            /// </summary>
            internal TranslationSettings()
            {

            }
        }

        /// <summary>
        /// Encapsulates rotation-specific manipulator settings
        /// </summary>
        public class RotationSettings : Settings
        {
            [DefaultValue(1.25f)]
            private float   mInnerRadius;

            [DefaultValue(1.50f)]
            private float   mOuterRadius;

            /// <summary>
            /// Gets or sets the inner radius of the circle strip used to represent each
            /// axis of rotation
            /// </summary>
            public float InnerRadius
            {
                get { return mInnerRadius; }
                set
                {
                    mInnerRadius = MathHelper.Clamp(value, 0, float.MaxValue);

                    if (mOuterRadius < mInnerRadius)
                        mOuterRadius = mInnerRadius;
                }
            }

            /// <summary>
            /// Gets or sets the outer radius of the circle strip used to represent each
            /// axis of rotation
            /// </summary>
            public float OuterRadius
            {
                get { return mOuterRadius; }
                set
                {
                    mOuterRadius = MathHelper.Clamp(value, 0, float.MaxValue);

                    if (mInnerRadius > mOuterRadius)
                        mInnerRadius = mOuterRadius;
                }
            }

            internal RotationSettings()
            {

            }
        }

        /// <summary>
        /// Encapsulates scale-specific manipulator settings
        /// </summary>
        public class ScaleSettings : Settings
        {
            [DefaultValue(0.05f)]
            private float           mAxisRadius;

            [DefaultValue(1.0f)]
            private float           mAxisExtent;

            [DefaultValue(0.1f)]
            private float           mAxisHandleSize;

            [DefaultValue(0.1f)]
            private float           mPlaneHandleSize;

            [DefaultValue(0.3f)]
            private float           mUniformHandleRadius;

            [DefaultValue(AxisDirections.Positive)]
            private AxisDirections  mAxisDrawMode;

            /// <summary>
            /// Gets or sets the radius of the cylinder used to represent each scale axis
            /// </summary>
            public float AxisRadius
            {
                get { return mAxisRadius; }
                set
                {
                    mAxisRadius = MathHelper.Clamp(value, 0, float.MaxValue);
                }
            }

            /// <summary>
            /// Gets or sets the extents of the cylinder used to represent each scale axis
            /// </summary>
            public float AxisExtent
            {
                get { return mAxisExtent; }
                set
                {
                    mAxisExtent = MathHelper.Clamp(value, 0, float.MaxValue);
                }
            }

            /// <summary>
            /// Gets or sets the size of the boxes used to represent the handles of each single-component scale axis
            /// </summary>
            public float AxisHandleSize
            {
                get { return mAxisHandleSize; }
                set
                {
                    mAxisHandleSize = MathHelper.Clamp(value, 0, float.MaxValue);
                }
            }

            /// <summary>
            /// Gets or sets the radius of the spheres used to represent the handles of each dual-component axis plane
            /// </summary>
            public float PlaneHandleRadius
            {
                get { return mPlaneHandleSize; }
                set
                {
                    mPlaneHandleSize = MathHelper.Clamp(value, 0, float.MaxValue);
                }
            }

            /// <summary>
            /// Gets or sets the radius of the sphere used to represent the handle for uniform scaling
            /// </summary>
            public float UniformHandleRadius
            {
                get { return mUniformHandleRadius; }
                set
                {
                    mUniformHandleRadius = MathHelper.Clamp(value, 0, float.MaxValue);
                }
            }

            /// <summary>
            /// Gets or sets the directions of each axis that will be drawn by the manipulator
            /// </summary>
            public AxisDirections AxisDrawMode
            {
                get { return mAxisDrawMode; }
                set { mAxisDrawMode = value; }
            }

            /// <summary>
            /// Creates a new instance of ScaleSettings
            /// </summary>
            internal ScaleSettings()
            {

            }
        }

        private TranslationSettings     mTranslationSettings;
        private RotationSettings        mRotationSettings;
        private ScaleSettings           mScaleSettings;

        [DefaultValue(0xFF0000)]
        [TypeConverter(typeof(ColorConverter))]
        private Color                   mXAxisColor;

        [DefaultValue(0x00FF00)]
        [TypeConverter(typeof(ColorConverter))]
        private Color                   mYAxisColor;

        [DefaultValue(0x0000FF)]
        [TypeConverter(typeof(ColorConverter))]
        private Color                   mZAxisColor;

        [DefaultValue(0x0000FF)]
        [TypeConverter(typeof(ColorConverter))]
        private Color                   mXYPlaneColor;

        [DefaultValue(0x00FF00)]
        [TypeConverter(typeof(ColorConverter))]
        private Color                   mXZPlaneColor;

        [DefaultValue(0xFF0000)]
        [TypeConverter(typeof(ColorConverter))]
        private Color                   mYZPlaneColor;

        [DefaultValue(0xFFEBCD)]
        [TypeConverter(typeof(ColorConverter))]
        private Color                   mUniformColor;

        [DefaultValue(0xFFFF00)]
        [TypeConverter(typeof(ColorConverter))]
        private Color                   mSelectionColor;

        [DefaultValue(50.0f)]
        private float                   mBaseScale;

        [DefaultValue(true)]
        private bool                    mDrawBounds;

        /// <summary>
        /// Gets the translation-specific settings for the manipulator
        /// </summary>
        public TranslationSettings Translation
        {
            get { return mTranslationSettings; }
        }

        /// <summary>
        /// Gets the rotation-specific settings for the manipulator
        /// </summary>
        public RotationSettings Rotation
        {
            get { return mRotationSettings; }
        }

        /// <summary>
        /// Gets the scale-specific settings for the manipulator
        /// </summary>
        public ScaleSettings Scale
        {
            get { return mScaleSettings; }
        }

        /// <summary>
        /// Gets or sets the display color of the x axis
        /// </summary>
        public Color XAxisColor
        {
            get { return mXAxisColor; }
            set { mXAxisColor = value; }
        }

        /// <summary>
        /// Gets or sets the display color of the y axis
        /// </summary>
        public Color YAxisColor
        {
            get { return mYAxisColor; }
            set { mYAxisColor = value; }
        }

        /// <summary>
        /// Gets or sets the display color of the z axis
        /// </summary>
        public Color ZAxisColor
        {
            get { return mZAxisColor; }
            set { mZAxisColor = value; }
        }

        /// <summary>
        /// Gets or sets the display color of the x/y plane
        /// </summary>
        public Color XYPlaneColor
        {
            get { return mXYPlaneColor; }
            set { mXYPlaneColor = value; }
        }

        /// <summary>
        /// Gets or sets the display color of the y/x plane
        /// </summary>
        public Color YXPlaneColor
        {
            get { return mXYPlaneColor; }
            set { mXYPlaneColor = value; }
        }

        /// <summary>
        /// Gets or sets the display color of the x/z plane
        /// </summary>
        public Color XZPlaneColor
        {
            get { return mXZPlaneColor; }
            set { mXZPlaneColor = value; }
        }

        /// <summary>
        /// Gets or sets the display color of the z/x plane
        /// </summary>
        public Color ZXPlaneColor
        {
            get { return mXZPlaneColor; }
            set { mXZPlaneColor = value; }
        }

        /// <summary>
        /// Gets or sets the display color of the y/z plane
        /// </summary>
        public Color YZPlaneColor
        {
            get { return mYZPlaneColor; }
            set { mYZPlaneColor = value; }
        }

        /// <summary>
        /// Gets or sets the display color of the z/y plane
        /// </summary>
        public Color ZYPlaneColor
        {
            get { return mYZPlaneColor; }
            set { mYZPlaneColor = value; }
        }

        /// <summary>
        /// Gets or sets the display color for uniform axis operations
        /// </summary>
        public Color UniformColor
        {
            get { return mUniformColor; }
            set { mUniformColor = value; }
        }

        /// <summary>
        /// Gets or sets the selection color with which selected axes of the
        /// manipulator are drawn
        /// </summary>
        public Color SelectionColor
        {
            get { return mSelectionColor; }
            set { mSelectionColor = value; }
        }

        /// <summary>
        /// Gets or sets the base scale of the manipulator, used in calculating the
        /// proper draw size of each axis depending on the camera distance
        /// </summary>
        public float BaseScale
        {
            get { return mBaseScale; }
            set { mBaseScale = MathHelper.Clamp(value, float.Epsilon, float.MaxValue); }
        }

        /// <summary>
        /// Gets or sets the flag that determines whether or not the bounding box is drawn when 
        /// an IBoundedTransform is being manipulated
        /// </summary>
        public bool DrawBoundsEnabled
        {
            get { return mDrawBounds; }
            set { mDrawBounds = value; }
        }

        /// <summary>
        /// Gets the display color of the specified axis
        /// </summary>
        /// <param name="axis">The axis for which to retrieve the display color</param>
        /// <returns>The display color of the specified axis</returns>
        public Color GetAxisColor(AxisFlags axis)
        {
            switch (axis)
            {
                case AxisFlags.X:
                    return mXAxisColor;
                case AxisFlags.Y:
                    return mYAxisColor;
                case AxisFlags.Z:
                    return mZAxisColor;
                case AxisFlags.XY:
                    return mXYPlaneColor;
                case AxisFlags.XZ:
                    return mXZPlaneColor;
                case AxisFlags.YZ:
                    return mYZPlaneColor;
                case AxisFlags.XYZ:
                    return mUniformColor;
            }

            return Color.Black;
        }

        /// <summary>
        /// Creates a new instance of ManipulatorSettings with default setting values
        /// </summary>
        internal ManipulatorSettings()
        {
            mTranslationSettings = new TranslationSettings();
            mRotationSettings = new RotationSettings();
            mScaleSettings = new ScaleSettings();

            mTranslationSettings.RestoreDefaults();
            mRotationSettings.RestoreDefaults();
            mScaleSettings.RestoreDefaults();
        }
    }
}
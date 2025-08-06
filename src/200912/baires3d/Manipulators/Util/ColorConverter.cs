using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mindshifter
{
    public class ColorConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(int))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(int))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is int)
            {
                int packed = (int)value;
                return new Color((byte)((packed >> 16) & 0xFF),
                    (byte)((packed >> 8) & 0xFF), (byte)(packed & 0xFF));
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, 
            object value, Type destinationType)
        {
            if (destinationType == typeof(int))
            {
                Color color = (Color)value;
                return (int)(((long)color.G << 16) | ((long)color.B << 8) | ((long)color.A));
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
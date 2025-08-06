﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FBX
{
    public class FbxValue
    {
        public object AsObject { get; }

		public FbxValue(object value)
		{
			AsObject = value;
		}

		public char AsChar
        {
            get
            {
                if (AsObject is char charValue)
                {
                    return charValue;
                }
                throw new InvalidCastException();
            }
        }

        public string AsString
        {
            get
            {
                if (AsObject is string stringValue)
                {
                    return stringValue;
                }
                throw new InvalidCastException();
            }
        }

        public int AsInt
        {
            get
            {
                if (AsObject is int intValue)
                {
                    return intValue;
                }
                throw new InvalidCastException();
            }
        }

        public int[] AsIntArray
        {
            get
            {
                if (AsObject is int[] intArrayValue)
                {
                    return intArrayValue;
                }
                throw new InvalidCastException();
            }
        }

        public long AsLong
        {
            get
            {
                if (AsObject is long longValue)
                {
                    return longValue;
                }
                throw new InvalidCastException();
            }
        }

        private double ToDouble(object value)
        {
            if (value is double doubleValue)
            {
                return doubleValue;
            }
            if (value is float floatValue)
            {
                return floatValue;
            }
            if (value is byte byteValue)
            {
                return byteValue;
            }
            if (value is long longValue)
            {
                return longValue;
            }
            if (value is int intValue)
            {
                return intValue;
            }
            throw new InvalidCastException();
        }

        private double[] ToDoubleArray(object value)
        {
            var result = new List<double>();
            if (value is double[] doubleArray)
            {
                return doubleArray;
            }
            if (value is float[] floatArray)
            {
                return (from item in floatArray select ToDouble(item)).ToArray();
            }
            if (value is byte[] byteArray)
            {
                return (from item in byteArray select ToDouble(item)).ToArray();
            }
            if (value is long[] longArray)
            {
                return (from item in longArray select ToDouble(item)).ToArray();
            }
            if (value is int[] intArray)
            {
                return (from item in intArray select ToDouble(item)).ToArray();
            }
            throw new InvalidCastException();
        }


        private float ToFloat(object value)
        {
			if (value is double doubleValue)
			{
				return (float)doubleValue;
			}
			if (value is float floatValue)
            {
                return floatValue;
            }
            if (value is byte byteValue)
            {
                return byteValue;
            }
            if (value is int longValue)
            {
                return longValue;
            }
            if (value is int intValue)
            {
                return intValue;
            }
            throw new InvalidCastException();
        }

        private float[] ToFloatArray(object value)
        {
            var result = new List<float>();
			if (value is double[] doubleArray)
			{
				return (from item in doubleArray select ToFloat(item)).ToArray();
			}
			if (value is float[] floatArray)
            {
                return floatArray;
            }
            if (value is byte[] byteArray)
            {
                return (from item in byteArray select ToFloat(item)).ToArray();
            }
            if (value is long[] longArray)
            {
                return (from item in longArray select ToFloat(item)).ToArray();
            }
            if (value is int[] intArray)
            {
                return (from item in intArray select ToFloat(item)).ToArray();
            }
            throw new InvalidCastException();
        }

        public double AsDouble => ToDouble(AsObject);

        public double[] AsDoubleArray => ToDoubleArray(AsObject);

        public float AsFloat => ToFloat(AsObject);

        public float[] AsFloatArray => ToFloatArray(AsObject);
    }
}

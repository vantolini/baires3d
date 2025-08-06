#region License
/*
MIT License
Copyright © 2006 The Mono.Xna Team

All rights reserved.

Authors:
 * Alan McGovern

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
#endregion License

using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Runtime.InteropServices;

namespace Builder {
    [Serializable]
    public struct Half : IEquatable<Half>, IComparable<Half> {
        /// <summary>
        /// Represents a value that is not-a-number (NaN).
        /// </summary>
        public static readonly Half NaN = new Half(0x7C01);

        /// <summary>
        /// Represents positive infinity.
        /// </summary>
        public static readonly Half PositiveInfinity = new Half(31744);

        /// <summary>
        /// Represents negative infinity.
        /// </summary>
        public static readonly Half NegativeInfinity = new Half(64512);

        /// <summary>
        /// Determines if this instance is zero.  It returns <see langword="true" />
        /// if this instance is either positive or negative zero.
        /// </summary>
        public bool IsZero { get { return (_bits == 0) || (_bits == 0x8000); } }

        /// <summary>
        /// Determines if this instance is Not-A-Number (NaN).
        /// </summary>
        public bool IsNaN { get { return (((_bits & 0x7C00) == 0x7C00) && (_bits & 0x03FF) != 0x0000); } }

        /// <summary>
        /// Determines if this instance represents positive infinity.
        /// </summary>
        public bool IsPositiveInfinity { get { return (_bits == 31744); } }

        /// <summary>
        /// Determines if this instance represents negative infinity.
        /// </summary>
        public bool IsNegativeInfinity { get { return (_bits == 64512); } }

        /// <summary>
        /// Determines if this instance represents either positive or negative infinity.
        /// </summary>
        public bool IsInfinity { get { return (_bits & 31744) == 31744; } }

        /// <summary>
        /// Initializes a new instance from a 32-bit single-precision floating-point number.
        /// </summary>
        /// <param name="value">A 32-bit, single-precision floating-point number.</param>
        public Half(float value)
            : this((double)value) {
        }

        /// <summary>
        /// Initializes a new instance from a 64-bit double-precision floating-point number.
        /// </summary>
        /// <param name="value">A 64-bit, double-precision floating-point number.</param>
        public Half(double value) {
            _bits = DoubleToHalf(BitConverter.DoubleToInt64Bits(value));
        }

        private Half(ushort value) {
            _bits = value;
        }

        /// <summary>
        /// Converts this instance to a 32-bit, single-precision floating-point number.</summary>
        /// <returns>The 32-bit, single-precision floating-point number.</returns>
        public float ToSingle() {
            // If it's problematic, this heap allocation can be eliminated by replacing
            // HalfToFloat with HalfToDouble and using BitConverter.Int64BitsToDouble().
            byte[] bytes = BitConverter.GetBytes(HalfToFloat(_bits));
            return BitConverter.ToSingle(bytes, 0);
        }

        /// <summary>
        /// Converts this instance to a 64-bit, double-precision floating-point number.</summary>
        /// <returns>The 64-bit, double-precision floating-point number.</returns>
        public double ToDouble() {
            return ToSingle();
        }

        /// <summary>
        /// Converts a 32-bit, single-precision, floating-point number to a 16-bit,
        /// half-precision, floating-point number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Half(float value) {
            return new Half(value);
        }

        /// <summary>
        /// Converts a 64-bit, double-precision, floating-point number to a 16-bit,
        /// half-precision, floating-point number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Half(double value) {
            return new Half(value);
        }

        /// <summary>
        /// Converts a 16-bit, half-precision, floating-point number to a 
        /// 32-bit, single-precision, floating-point number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator float(Half value) {
            return value.ToSingle();
        }

        /// <summary>
        /// Converts a 16-bit, half-precision, floating-point number to a
        /// 64-bit, double-precision, floating-point number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator double(Half value) {
            return value.ToDouble();
        }

        private static ushort DoubleToHalf(long bits) {
            // Our double-precision floating point number, F, is represented by the bit pattern in long i.
            // Disassemble that bit pattern into the sign, S, the exponent, E, and the significand, M.
            // Shift S into the position where it will go in in the resulting half number.
            // Adjust E, accounting for the different exponent bias of double and half (1023 versus 15).

            int sign = (int)((bits >> 48) & 0x00008000);
            int exponent = (int)(((bits >> 52) & 0x7FF) - (1023 - 15));
            long mantissa = bits & 0xFFFFFFFFFFFFF;

            // Now reassemble S, E and M into a half:

            if (exponent <= 0) {
                if (exponent < -10) {
                    // E is less than -10. The absolute value of F is less than Half.MinValue
                    // (F may be a small normalized float, a denormalized float or a zero).
                    //
                    // We convert F to a half zero with the same sign as F.

                    return (UInt16)sign;
                }

                // E is between -10 and 0. F is a normalized double whose magnitude is less than Half.MinNormalizedValue.
                //
                // We convert F to a denormalized half.

                // Add an explicit leading 1 to the significand.

                mantissa = mantissa | 0x10000000000000;

                // Round to M to the nearest (10+E)-bit value (with E between -10 and 0); in case of a tie, round to the nearest even value.
                //
                // Rounding may cause the significand to overflow and make our number normalized. Because of the way a half's bits
                // are laid out, we don't have to treat this case separately; the code below will handle it correctly.

                int t = 43 - exponent;
                long a = (1L << (t - 1)) - 1;
                long b = (mantissa >> t) & 1;

                mantissa = (mantissa + a + b) >> t;

                // Assemble the half from S, E (==zero) and M.

                return (ushort)(sign | (int)mantissa);
            }
            else if (exponent == 0x7ff - (1023 - 15)) {
                if (mantissa == 0) {
                    // F is an infinity; convert F to a half infinity with the same sign as F.

                    return (ushort)(sign | 0x7c00);
                }
                else {
                    // F is a NAN; we produce a half NAN that preserves the sign bit and the 10 leftmost bits of the
                    // significand of F, with one exception: If the 10 leftmost bits are all zero, the NAN would turn 
                    // into an infinity, so we have to set at least one bit in the significand.

                    int mantissa32 = (int)(mantissa >> 42);
                    return (ushort)(sign | 0x7c00 | mantissa32 | ((mantissa == 0) ? 1 : 0));
                }
            }
            else {
                // E is greater than zero.  F is a normalized double. We try to convert F to a normalized half.

                // Round to M to the nearest 10-bit value. In case of a tie, round to the nearest even value.

                mantissa = mantissa + 0x1FFFFFFFFFF + ((mantissa >> 42) & 1);

                if ((mantissa & 0x10000000000000) != 0) {
                    mantissa = 0;        // overflow in significand,
                    exponent += 1;        // adjust exponent
                }

                // exponent overflow
                if (exponent > 30) throw new ArithmeticException("Half: Hardware floating-point overflow.");

                // Assemble the half from S, E and M.

                return (ushort)(sign | (exponent << 10) | (int)(mantissa >> 42));
            }
        }

        /// <summary>Ported from OpenEXR's IlmBase 1.0.1</summary>
        private static int HalfToFloat(ushort ui16) {
            int sign = (ui16 >> 15) & 0x00000001;
            int exponent = (ui16 >> 10) & 0x0000001f;
            int mantissa = ui16 & 0x000003ff;

            if (exponent == 0) {
                if (mantissa == 0) {
                    // Plus or minus zero

                    return sign << 31;
                }
                else {
                    // Denormalized number -- renormalize it

                    while ((mantissa & 0x00000400) == 0) {
                        mantissa <<= 1;
                        exponent -= 1;
                    }

                    exponent += 1;
                    mantissa &= ~0x00000400;
                }
            }
            else if (exponent == 31) {
                if (mantissa == 0) {
                    // Positive or negative infinity

                    return (sign << 31) | 0x7f800000;
                }
                else {
                    // Nan -- preserve sign and significand bits

                    return (sign << 31) | 0x7f800000 | (mantissa << 13);
                }
            }

            // Normalized number

            exponent = exponent + (127 - 15);
            mantissa = mantissa << 13;

            // Assemble S, E and M.

            return (sign << 31) | (exponent << 23) | mantissa;
        }

        /// <summary>
        /// The smallest positive <see cref="Half"/>.
        /// </summary>
        public static readonly float MinValue = 5.96046448e-08f;

        /// <summary>
        /// The smallest positive normalized <see cref="Half"/>.
        /// </summary>
        public static readonly float MinNormalizedValue = 6.10351562e-05f;

        /// <summary>
        /// The largest positive <see cref="Half"/>.
        /// </summary>
        public static readonly float MaxValue = 65504.0f;

        /// <summary>
        /// Smallest positive value e for which <see cref="Half"/> (1.0 + e) != <see cref="Half"/> (1.0).
        /// </summary>
        public static readonly float Epsilon = 0.00097656f;

        /// <summary>
        /// Returns a value indicating whether this instance is equal to another
        /// instance.
        /// </summary>
        /// <param name="other">The other instance to which to compare this instance.</param>
        /// <returns>
        /// <see langword="true" /> if this instance exactly equals another; otherwise <see langword="false" />.</returns>
        public bool Equals(Half other) {
            return ToSingle().Equals(other.ToSingle());
        }

        public override bool Equals(object obj) {
            if (obj is Half) {
                return Equals((Half)obj);
            }
            else {
                return false;
            }
        }

        public override int GetHashCode() {
            return _bits.GetHashCode();
        }

        public static bool operator ==(Half left, Half right) {
            return left.Equals(right);
        }

        public static bool operator !=(Half left, Half right) {
            return !left.Equals(right);
        }

        public static bool operator <(Half left, Half right) {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(Half left, Half right) {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(Half left, Half right) {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(Half left, Half right) {
            return left.CompareTo(right) >= 0;
        }

        /// <summary>
        /// Compares this instance to a specified half-precision floating-point number
        /// and returns an integer that indicates whether the value of this instance
        /// is less than, equal to, or greater than the value of the specified half-precision
        /// floating-point number. 
        /// </summary>
        /// <param name="other">A half-precision floating-point number to compare.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and value. If the number is:
        /// <para>
        /// Less than zero, then this instance is less than other, or this instance is not a number
        /// (<see cref="NaN"/>) and other is a number.
        /// </para>
        /// <para>
        /// Zero: this instance is equal to value, or both this instance and other
        /// are not a number (<see cref="NaN"/>), <see cref="PositiveInfinity"/>, or
        /// <see cref="NegativeInfinity"/>.
        /// </para>
        /// <para>
        /// Greater than zero: this instance is greater than othrs, or this instance is a number
        /// and other is not a number (<see cref="NaN"/>).
        /// </para>
        /// </returns>
        public int CompareTo(Half other) {
            return ToSingle().CompareTo(other.ToSingle());
        }

        /// <summary>Converts this instance into a human-legible string representation.</summary>
        /// <returns>The string representation of this instance.</returns>
        public override string ToString() {
            return ToSingle().ToString();
        }

        /// <summary>Converts this instance into a human-legible string representation.</summary>
        /// <param name="format">Formatting for the output string.</param>
        /// <param name="formatProvider">Culture-specific formatting information.</param>
        /// <returns>The string representation of this instance.</returns>
        public string ToString(string format, IFormatProvider formatProvider) {
            return ToSingle().ToString(format, formatProvider);
        }

        /// <summary>Converts the string representation of a number to a half-precision floating-point equivalent.</summary>
        /// <param name="s">String representation of the number to convert.</param>
        /// <returns>A new <see cref="Half"/> instance.</returns>
        public static Half Parse(string s) {
            return new Half(Double.Parse(s));
        }

        /// <summary>Converts the string representation of a number to a half-precision floating-point equivalent.</summary>
        /// <param name="s">String representation of the number to convert.</param>
        /// <param name="style">Specifies the format of <paramref name="s"/>.</param>
        /// <param name="provider">Culture-specific formatting information.</param>
        /// <returns>A new <see cref="Half"/> instance.</returns>
        public static Half Parse(string s, NumberStyles style, IFormatProvider provider) {
            return new Half(Double.Parse(s, style, provider));
        }

        /// <summary>Converts the string representation of a number to a half-precision floating-point equivalent.</summary>
        /// <param name="s">String representation of the number to convert.</param>
        /// <param name="result">The <see cref="Half"/> instance to write to.</param>
        /// <returns><see langword="true" /> true if parsing succeeded; otherwise <see langword="false"/>.</returns>
        public static bool TryParse(string s, out Half result) {
            double f;
            bool b = Double.TryParse(s, out f);
            result = new Half(f);
            return b;
        }

        /// <summary>Converts the string representation of a number to a half-precision floating-point equivalent.</summary>
        /// <param name="s">String representation of the number to convert.</param>
        /// <param name="style">Specifies the format of <paramref name="s"/>.</param>
        /// <param name="provider">Culture-specific formatting information.</param>
        /// <param name="result">The Half instance to write to.</param>
        /// <returns><see langword="true" /> true if parsing succeeded; otherwise <see langword="false"/>.</returns>
        public static bool TryParse(string s, System.Globalization.NumberStyles style, IFormatProvider provider, out Half result) {
            double f;
            bool b = Double.TryParse(s, style, provider, out f);
            result = new Half(f);
            return b;
        }

        private ushort _bits;
    }
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector3B : IEquatable<Vector3B> {
        public static Vector3B False {
            get { return new Vector3B(false, false, false); }
        }

        public static Vector3B True {
            get { return new Vector3B(true, true, true); }
        }


        public Vector3B(bool x, bool y, bool z) {
            _x = x;
            _y = y;
            _z = z;
        }

        public Vector3B(Vector2B v, bool z) {
            _x = v.X;
            _y = v.Y;
            _z = z;
        }

        public bool X {
            get { return _x; }
        }

        public bool Y {
            get { return _y; }
        }

        public bool Z {
            get { return _z; }
        }

        public Vector2B XY {
            get { return new Vector2B(X, Y); }
        }

        public bool Equals(Vector3B other) {
            return _x == other._x && _y == other._y && _z == other._z;
        }

        public static bool operator ==(Vector3B left, Vector3B right) {
            return left.Equals(right);
        }

        public static bool operator !=(Vector3B left, Vector3B right) {
            return !left.Equals(right);
        }

        public override bool Equals(object obj) {
            if (obj is Vector3B) {
                return Equals((Vector3B)obj);
            }
            return false;
        }

        public override string ToString() {
            return string.Format(CultureInfo.CurrentCulture, "({0}, {1}, {2})", X, Y, Z);
        }

        public override int GetHashCode() {
            return (Convert.ToInt32(_x) * 4) + (Convert.ToInt32(_y) * 2) + Convert.ToInt32(_z);
        }

        public Vector3D ToVector3D() {
            return new Vector3D(Convert.ToDouble(_x), Convert.ToDouble(_y), Convert.ToDouble(_z));
        }

        public Vector3 ToVector3F() {
            return new Vector3(Convert.ToSingle(_x), Convert.ToSingle(_y), Convert.ToSingle(_z));
        }

        public Vector3I ToVector3I() {
            return new Vector3I(Convert.ToInt32(_x), Convert.ToInt32(_y), Convert.ToInt32(_z));
        }

        public Vector3H ToVector3H() {
            return new Vector3H(Convert.ToInt32(_x), Convert.ToInt32(_y), Convert.ToInt32(_z));
        }

        private readonly bool _x;
        private readonly bool _y;
        private readonly bool _z;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector3I : IEquatable<Vector3I> {
        public static Vector3I Zero {
            get { return new Vector3I(0, 0, 0); }
        }

        public static Vector3I UnitX {
            get { return new Vector3I(1, 0, 0); }
        }

        public static Vector3I UnitY {
            get { return new Vector3I(0, 1, 0); }
        }

        public static Vector3I UnitZ {
            get { return new Vector3I(0, 0, 1); }
        }

        public Vector3I(int x, int y, int z) {
            _x = x;
            _y = y;
            _z = z;
        }

        public Vector3I(Vector2I v, int z) {
            _x = v.X;
            _y = v.Y;
            _z = z;
        }

        public int X {
            get { return _x; }
        }

        public int Y {
            get { return _y; }
        }

        public int Z {
            get { return _z; }
        }

        public Vector2I XY {
            get { return new Vector2I(X, Y); }
        }

        public float MagnitudeSquared {
            get { return _x * _x + _y * _y + _z * _z; }
        }

        public double Magnitude {
            get { return Math.Sqrt(MagnitudeSquared); }
        }

        public Vector3I Cross(Vector3I other) {
            return new Vector3I(Y * other.Z - Z * other.Y,
                                Z * other.X - X * other.Z,
                                X * other.Y - Y * other.X);
        }

        public int Dot(Vector3I other) {
            return X * other.X + Y * other.Y + Z * other.Z;
        }

        public Vector3I Add(Vector3I addend) {
            return this + addend;
        }

        public Vector3I Subtract(Vector3I subtrahend) {
            return this - subtrahend;
        }

        public Vector3I Multiply(int scalar) {
            return this * scalar;
        }

        public Vector3I MultiplyComponents(Vector3I scale) {
            return new Vector3I(X * scale.X, Y * scale.Y, Z * scale.Z);
        }

        public Vector3I Divide(int scalar) {
            return this / scalar;
        }

        public Vector3I MostOrthogonalAxis {
            get {
                int x = Math.Abs(X);
                int y = Math.Abs(Y);
                int z = Math.Abs(Z);

                if ((x < y) && (x < z)) {
                    return UnitX;
                }
                else if ((y < x) && (y < z)) {
                    return UnitY;
                }
                else {
                    return UnitZ;
                }
            }
        }

        public Vector3I Negate() {
            return -this;
        }

        public bool Equals(Vector3I other) {
            return _x == other._x && _y == other._y && _z == other._z;
        }

        public static Vector3I operator -(Vector3I vector) {
            return new Vector3I(-vector.X, -vector.Y, -vector.Z);
        }

        public static Vector3I operator +(Vector3I left, Vector3I right) {
            return new Vector3I(left._x + right._x, left._y + right._y, left._z + right._z);
        }

        public static Vector3I operator -(Vector3I left, Vector3I right) {
            return new Vector3I(left._x - right._x, left._y - right._y, left._z - right._z);
        }

        public static Vector3I operator *(Vector3I left, int right) {
            return new Vector3I(left._x * right, left._y * right, left._z * right);
        }

        public static Vector3I operator *(int left, Vector3I right) {
            return right * left;
        }

        public static Vector3I operator /(Vector3I left, int right) {
            return new Vector3I(left._x / right, left._y / right, left._z / right);
        }

        public static bool operator >(Vector3I left, Vector3I right) {
            return (left.X > right.X) && (left.Y > right.Y) && (left.Z > right.Z);
        }

        public static bool operator >=(Vector3I left, Vector3I right) {
            return (left.X >= right.X) && (left.Y >= right.Y) && (left.Z >= right.Z);
        }

        public static bool operator <(Vector3I left, Vector3I right) {
            return (left.X < right.X) && (left.Y < right.Y) && (left.Z < right.Z);
        }

        public static bool operator <=(Vector3I left, Vector3I right) {
            return (left.X <= right.X) && (left.Y <= right.Y) && (left.Z <= right.Z);
        }

        public static bool operator ==(Vector3I left, Vector3I right) {
            return left.Equals(right);
        }

        public static bool operator !=(Vector3I left, Vector3I right) {
            return !left.Equals(right);
        }

        public override bool Equals(object obj) {
            if (obj is Vector3I) {
                return Equals((Vector3I)obj);
            }
            return false;
        }

        public override string ToString() {
            return string.Format(CultureInfo.CurrentCulture, "({0}, {1}, {2})", X, Y, Z);
        }

        public override int GetHashCode() {
            return _x.GetHashCode() ^ _y.GetHashCode() ^ _z.GetHashCode();
        }

        public Vector3D ToVector3D() {
            return new Vector3D((double)_x, (double)_y, (double)_z);
        }

        public Vector3 ToVector3F() {
            return new Vector3((float)_x, (float)_y, (float)_z);
        }

        public Vector3H ToVector3H() {
            return new Vector3H(_x, _y, _z);
        }

        public Vector3B ToVector3B() {
            return new Vector3B(Convert.ToBoolean(_x), Convert.ToBoolean(_y), Convert.ToBoolean(_z));
        }

        private readonly int _x;
        private readonly int _y;
        private readonly int _z;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector3H : IEquatable<Vector3H> {
        public static Vector3H Zero {
            get { return new Vector3H(0.0, 0.0, 0.0); }
        }

        public static Vector3H UnitX {
            get { return new Vector3H(1.0, 0.0, 0.0); }
        }

        public static Vector3H UnitY {
            get { return new Vector3H(0.0, 1.0, 0.0); }
        }

        public static Vector3H UnitZ {
            get { return new Vector3H(0.0, 0.0, 1.0); }
        }

        public static Vector3H Undefined {
            get { return new Vector3H(Half.NaN, Half.NaN, Half.NaN); }
        }

        public Vector3H(Half x, Half y, Half z) {
            _x = x;
            _y = y;
            _z = z;
        }

        public Vector3H(Vector2H v, Half z) {
            _x = v.X;
            _y = v.Y;
            _z = z;
        }

        public Vector3H(float x, float y, float z) {
            _x = new Half(x);
            _y = new Half(y);
            _z = new Half(z);
        }

        public Vector3H(double x, double y, double z) {
            _x = new Half(x);
            _y = new Half(y);
            _z = new Half(z);
        }

        public Half X {
            get { return _x; }
        }

        public Half Y {
            get { return _y; }
        }

        public Half Z {
            get { return _z; }
        }

        public Vector2H XY {
            get { return new Vector2H(X, Y); }
        }

        public bool IsUndefined {
            get { return Double.IsNaN(_x); }
        }

        public bool Equals(Vector3H other) {
            return _x == other._x && _y == other._y && _z == other._z;
        }

        public static bool operator ==(Vector3H left, Vector3H right) {
            return left.Equals(right);
        }

        public static bool operator !=(Vector3H left, Vector3H right) {
            return !left.Equals(right);
        }

        public override bool Equals(object obj) {
            if (obj is Vector3H) {
                return Equals((Vector3H)obj);
            }
            return false;
        }

        public override string ToString() {
            return string.Format(CultureInfo.CurrentCulture, "({0}, {1}, {2})", X, Y, Z);
        }

        public override int GetHashCode() {
            return _x.GetHashCode() ^ _y.GetHashCode() ^ _z.GetHashCode();
        }

        public Vector3D ToVector3D() {
            return new Vector3D(_x, _y, _z);
        }

        public Vector3I ToVector3I() {
            return new Vector3I((int)_x.ToDouble(), (int)_y.ToDouble(), (int)_z.ToDouble());
        }

        public Vector3 ToVector3F() {
            return new Vector3(_x, _y, _z);
        }

        public Vector3B ToVector3B() {
            return new Vector3B(Convert.ToBoolean(_x), Convert.ToBoolean(_y), Convert.ToBoolean(_z));
        }

        private readonly Half _x;
        private readonly Half _y;
        private readonly Half _z;
    }



    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector2B : IEquatable<Vector2B> {
        public static Vector2B False {
            get { return new Vector2B(false, false); }
        }

        public static Vector2B True {
            get { return new Vector2B(true, true); }
        }

        public Vector2B(bool x, bool y) {
            _x = x;
            _y = y;
        }

        public bool X {
            get { return _x; }
        }

        public bool Y {
            get { return _y; }
        }

        public bool Equals(Vector2B other) {
            return _x == other._x && _y == other._y;
        }

        public static bool operator ==(Vector2B left, Vector2B right) {
            return left.Equals(right);
        }

        public static bool operator !=(Vector2B left, Vector2B right) {
            return !left.Equals(right);
        }

        public override bool Equals(object obj) {
            if (obj is Vector2B) {
                return Equals((Vector2B)obj);
            }
            return false;
        }

        public override string ToString() {
            return string.Format(CultureInfo.CurrentCulture, "({0}, {1})", X, Y);
        }

        public override int GetHashCode() {
            return (Convert.ToInt32(_x) * 2) + Convert.ToInt32(_y);
        }

        public Vector2D ToVector2D() {
            return new Vector2D(Convert.ToDouble(_x), Convert.ToDouble(_y));
        }

        public Vector2 ToVector2F() {
            return new Vector2(Convert.ToSingle(_x), Convert.ToSingle(_y));
        }

        public Vector2I ToVector2I() {
            return new Vector2I(Convert.ToInt32(_x), Convert.ToInt32(_y));
        }

        public Vector2H ToVector2H() {
            return new Vector2H(Convert.ToInt32(_x), Convert.ToInt32(_y));
        }

        private readonly bool _x;
        private readonly bool _y;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector2I : IEquatable<Vector2I> {
        public static Vector2I Zero {
            get { return new Vector2I(0, 0); }
        }

        public static Vector2I UnitX {
            get { return new Vector2I(1, 0); }
        }

        public static Vector2I UnitY {
            get { return new Vector2I(0, 1); }
        }

        public Vector2I(int x, int y) {
            _x = x;
            _y = y;
        }

        public int X {
            get { return _x; }
        }

        public int Y {
            get { return _y; }
        }

        public Vector2I Add(Vector2I addend) {
            return this + addend;
        }

        public Vector2I Subtract(Vector2I subtrahend) {
            return this - subtrahend;
        }

        public Vector2I Multiply(int scalar) {
            return this * scalar;
        }

        public Vector2I Negate() {
            return -this;
        }

        public bool Equals(Vector2I other) {
            return _x == other._x && _y == other._y;
        }

        public static Vector2I operator -(Vector2I vector) {
            return new Vector2I(-vector.X, -vector.Y);
        }

        public static Vector2I operator +(Vector2I left, Vector2I right) {
            return new Vector2I(left._x + right._x, left._y + right._y);
        }

        public static Vector2I operator -(Vector2I left, Vector2I right) {
            return new Vector2I(left._x - right._x, left._y - right._y);
        }

        public static Vector2I operator *(Vector2I left, int right) {
            return new Vector2I(left._x * right, left._y * right);
        }

        public static Vector2I operator *(int left, Vector2I right) {
            return right * left;
        }

        public static bool operator ==(Vector2I left, Vector2I right) {
            return left.Equals(right);
        }

        public static bool operator !=(Vector2I left, Vector2I right) {
            return !left.Equals(right);
        }

        public override bool Equals(object obj) {
            if (obj is Vector2I) {
                return Equals((Vector2I)obj);
            }
            return false;
        }

        public override string ToString() {
            return string.Format(CultureInfo.CurrentCulture, "({0}, {1})", X, Y);
        }

        public override int GetHashCode() {
            return _x.GetHashCode() ^ _y.GetHashCode();
        }

        public Vector2D ToVector2D() {
            return new Vector2D(_x, _y);
        }

        public Vector2 ToVector2() {
            return new Vector2(_x, _y);
        }

        public Vector2H ToVector2H() {
            return new Vector2H(_x, _y);
        }

        public Vector2B ToVector2B() {
            return new Vector2B(Convert.ToBoolean(_x), Convert.ToBoolean(_y));
        }

        private readonly int _x;
        private readonly int _y;
    }



    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector2H : IEquatable<Vector2H> {
        public static Vector2H Zero {
            get { return new Vector2H(0.0f, 0.0f); }
        }

        public static Vector2H UnitX {
            get { return new Vector2H(1.0f, 0.0f); }
        }

        public static Vector2H UnitY {
            get { return new Vector2H(0.0f, 1.0f); }
        }

        public static Vector2H Undefined {
            get { return new Vector2H(Half.NaN, Half.NaN); }
        }

        public Vector2H(Half x, Half y) {
            _x = x;
            _y = y;
        }

        public Vector2H(float x, float y) {
            _x = new Half(x);
            _y = new Half(y);
        }

        public Vector2H(double x, double y) {
            _x = new Half(x);
            _y = new Half(y);
        }

        public Half X {
            get { return _x; }
        }

        public Half Y {
            get { return _y; }
        }

        public bool IsUndefined {
            get { return _x.IsNaN; }
        }

        public bool Equals(Vector2H other) {
            return _x == other._x && _y == other._y;
        }

        public static bool operator ==(Vector2H left, Vector2H right) {
            return left.Equals(right);
        }

        public static bool operator !=(Vector2H left, Vector2H right) {
            return !left.Equals(right);
        }

        public override bool Equals(object obj) {
            if (obj is Vector2H) {
                return Equals((Vector2H)obj);
            }
            return false;
        }

        public override string ToString() {
            return string.Format(CultureInfo.CurrentCulture, "({0}, {1})", X, Y);
        }

        public override int GetHashCode() {
            return _x.GetHashCode() ^ _y.GetHashCode();
        }

        public Vector2D ToVector2D() {
            return new Vector2D(_x, _y);
        }

        public Vector2 ToVector2() {
            return new Vector2(_x, _y);
        }

        public Vector2I ToVector2I() {
            return new Vector2I(Convert.ToInt32(_x), Convert.ToInt32(_y));
        }

        public Vector2B ToVector2B() {
            return new Vector2B(Convert.ToBoolean(_x), Convert.ToBoolean(_y));
        }

        private readonly Half _x;
        private readonly Half _y;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector2D : IEquatable<Vector2D> {
        public static Vector2D Zero {
            get { return new Vector2D(0.0, 0.0); }
        }

        public static Vector2D UnitX {
            get { return new Vector2D(1.0, 0.0); }
        }

        public static Vector2D UnitY {
            get { return new Vector2D(0.0, 1.0); }
        }

        public static Vector2D Undefined {
            get { return new Vector2D(Double.NaN, Double.NaN); }
        }

        public Vector2D(double x, double y) {
            _x = x;
            _y = y;
        }

        public double X {
            get { return _x; }
            set { _x = value; }

        }

        public double Y {
            get { return _y; }
            set { _y = value; }
        }

        public double MagnitudeSquared {
            get { return _x * _x + _y * _y; }
        }

        public double Magnitude {
            get { return Math.Sqrt(MagnitudeSquared); }
        }

        public bool IsUndefined {
            get { return Double.IsNaN(_x); }
        }

        public Vector2D Normalize(out double magnitude) {
            magnitude = Magnitude;
            return this / magnitude;
        }

        public Vector2D Normalize() {
            double magnitude;
            return Normalize(out magnitude);
        }

        public double Dot(Vector2D other) {
            return X * other.X + Y * other.Y;
        }

        public Vector2D Add(Vector2D addend) {
            return this + addend;
        }

        public Vector2D Subtract(Vector2D subtrahend) {
            return this - subtrahend;
        }

        public Vector2D Multiply(double scalar) {
            return this * scalar;
        }

        public Vector2D Divide(double scalar) {
            return this / scalar;
        }

        public Vector2D Negate() {
            return -this;
        }

        public bool EqualsEpsilon(Vector2D other, double epsilon) {
            return
                (Math.Abs(_x - other._x) <= epsilon) &&
                (Math.Abs(_y - other._y) <= epsilon);
        }

        public bool Equals(Vector2D other) {
            return _x == other._x && _y == other._y;
        }

        public static Vector2D operator -(Vector2D vector) {
            return new Vector2D(-vector.X, -vector.Y);
        }

        public static Vector2D operator +(Vector2D left, Vector2D right) {
            return new Vector2D(left._x + right._x, left._y + right._y);
        }

        public static Vector2D operator -(Vector2D left, Vector2D right) {
            return new Vector2D(left._x - right._x, left._y - right._y);
        }

        public static Vector2D operator *(Vector2D left, double right) {
            return new Vector2D(left._x * right, left._y * right);
        }

        public static Vector2D operator *(double left, Vector2D right) {
            return right * left;
        }

        public static Vector2D operator /(Vector2D left, double right) {
            return new Vector2D(left._x / right, left._y / right);
        }

        public static Vector2D operator /(Vector2D left, Vector2D right) {
            return new Vector2D(left._x / right._x, left._y / right._y);
        }

        public static bool operator ==(Vector2D left, Vector2D right) {
            return left.Equals(right);
        }

        public static bool operator !=(Vector2D left, Vector2D right) {
            return !left.Equals(right);
        }

        public override bool Equals(object obj) {
            if (obj is Vector2D) {
                return Equals((Vector2D)obj);
            }
            return false;
        }

        public override string ToString() {
            return string.Format(CultureInfo.CurrentCulture, "({0}, {1})", X, Y);
        }

        public override int GetHashCode() {
            return _x.GetHashCode() ^ _y.GetHashCode();
        }

        public Vector2 ToVector2() {
            return new Vector2((float)_x, (float)_y);
        }

        public Vector2H ToVector2H() {
            return new Vector2H(_x, _y);
        }

        private double _x;
        private double _y;
    }



    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector3D : IEquatable<Vector3D> {
        public static Vector3D Zero {
            get { return new Vector3D(0.0, 0.0, 0.0); }
        }

        public static Vector3D UnitX {
            get { return new Vector3D(1.0, 0.0, 0.0); }
        }

        public static Vector3D UnitY {
            get { return new Vector3D(0.0, 1.0, 0.0); }
        }

        public static Vector3D UnitZ {
            get { return new Vector3D(0.0, 0.0, 1.0); }
        }

        public static Vector3D Undefined {
            get { return new Vector3D(Double.NaN, Double.NaN, Double.NaN); }
        }

        public Vector3D(double x, double y, double z) {
            _x = x;
            _y = y;
            _z = z;
        }

        public Vector3D(Vector2D v, double z) {
            _x = v.X;
            _y = v.Y;
            _z = z;
        }

        public double X {
            get { return _x; }
            set { _x = value; }
        }

        public double Y {
            get { return _y; }
            set { _y = value; }
        }

        public double Z {
            get { return _z; }
            set { _z = value; }
        }

        public Vector2D XY {
            get { return new Vector2D(X, Y); }
        }

        public double MagnitudeSquared {
            get { return _x * _x + _y * _y + _z * _z; }
        }

        public double Magnitude {
            get { return Math.Sqrt(MagnitudeSquared); }
        }

        public bool IsUndefined {
            get { return Double.IsNaN(_x); }
        }

        public Vector3D Normalize(out double magnitude) {
            magnitude = Magnitude;
            return this / magnitude;
        }

        public Vector3D Normalize() {
            double magnitude;
            return Normalize(out magnitude);
        }
        public static void Cross(ref Vector3D source, ref Vector3D other, out Vector3D outito) {
            Vector3D outit = new Vector3D(source.Y * other.Z - source.Z * other.Y,
                                source.Z * other.X - source.X * other.Z,
                                source.X * other.Y - source.Y * other.X);
            outito = outit;
        }

        public static Vector3D Cross(ref Vector3D source, ref Vector3D other) {
            return new Vector3D(source.Y * other.Z - source.Z * other.Y,
                                source.Z * other.X - source.X * other.Z,
                                source.X * other.Y - source.Y * other.X);

        }

        public Vector3D Cross(Vector3D other) {
            return new Vector3D(Y * other.Z - Z * other.Y,
                                Z * other.X - X * other.Z,
                                X * other.Y - Y * other.X);
        }

        public double Dot(Vector3D other) {
            return X * other.X + Y * other.Y + Z * other.Z;
        }

        public Vector3D Add(Vector3D addend) {
            return this + addend;
        }

        public Vector3D Subtract(Vector3D subtrahend) {
            return this - subtrahend;
        }

        public Vector3D Multiply(double scalar) {
            return this * scalar;
        }

        public Vector3D MultiplyComponents(Vector3D scale) {
            return new Vector3D(X * scale.X, Y * scale.Y, Z * scale.Z);
        }

        public Vector3D Divide(double scalar) {
            return this / scalar;
        }

        public Vector3D MostOrthogonalAxis {
            get {
                double x = Math.Abs(X);
                double y = Math.Abs(Y);
                double z = Math.Abs(Z);

                if ((x < y) && (x < z)) {
                    return UnitX;
                }
                else if ((y < x) && (y < z)) {
                    return UnitY;
                }
                else {
                    return UnitZ;
                }
            }
        }

        public double AngleBetween(Vector3D other) {
            return Math.Acos(Normalize().Dot(other.Normalize()));
        }

        public Vector3D Negate() {
            return -this;
        }
        internal static Vector3D Negate(Vector3D zAxis) {
            return -zAxis;
        }

        public bool EqualsEpsilon(Vector3D other, double epsilon) {
            return
                (Math.Abs(_x - other._x) <= epsilon) &&
                (Math.Abs(_y - other._y) <= epsilon) &&
                (Math.Abs(_z - other._z) <= epsilon);
        }

        public bool Equals(Vector3D other) {
            return _x == other._x && _y == other._y && _z == other._z;
        }

        public static Vector3D operator -(Vector3D vector) {
            return new Vector3D(-vector.X, -vector.Y, -vector.Z);
        }

        public static Vector3D operator +(Vector3D left, Vector3D right) {
            return new Vector3D(left._x + right._x, left._y + right._y, left._z + right._z);
        }

        public static Vector3D operator -(Vector3D left, Vector3D right) {
            return new Vector3D(left._x - right._x, left._y - right._y, left._z - right._z);
        }

        public static Vector3D operator *(Vector3D left, double right) {
            return new Vector3D(left._x * right, left._y * right, left._z * right);
        }

        public static Vector3D operator *(double left, Vector3D right) {
            return right * left;
        }

        public static Vector3D operator /(Vector3D left, double right) {
            return new Vector3D(left._x / right, left._y / right, left._z / right);
        }

        public static bool operator >(Vector3D left, Vector3D right) {
            return (left.X > right.X) && (left.Y > right.Y) && (left.Z > right.Z);
        }

        public static bool operator >=(Vector3D left, Vector3D right) {
            return (left.X >= right.X) && (left.Y >= right.Y) && (left.Z >= right.Z);
        }

        public static bool operator <(Vector3D left, Vector3D right) {
            return (left.X < right.X) && (left.Y < right.Y) && (left.Z < right.Z);
        }

        public static bool operator <=(Vector3D left, Vector3D right) {
            return (left.X <= right.X) && (left.Y <= right.Y) && (left.Z <= right.Z);
        }

        public static bool operator ==(Vector3D left, Vector3D right) {
            return left.Equals(right);
        }

        public static bool operator !=(Vector3D left, Vector3D right) {
            return !left.Equals(right);
        }

        public override bool Equals(object obj) {
            if (obj is Vector3D) {
                return Equals((Vector3D)obj);
            }
            return false;
        }

        public override string ToString() {
            return string.Format(CultureInfo.CurrentCulture, "({0}, {1}, {2})", X, Y, Z);
        }

        public override int GetHashCode() {
            return _x.GetHashCode() ^ _y.GetHashCode() ^ _z.GetHashCode();
        }

        public Vector3 ToVector3F() {
            return new Vector3((float)_x, (float)_y, (float)_z);
        }

        public Vector3I ToVector3I() {
            return new Vector3I((int)_x, (int)_y, (int)_z);
        }

        public Vector3H ToVector3H() {
            return new Vector3H(_x, _y, _z);
        }

        public Vector3B ToVector3B() {
            return new Vector3B(Convert.ToBoolean(_x), Convert.ToBoolean(_y), Convert.ToBoolean(_z));
        }

        private double _x;
        private double _y;
        private double _z;

        public double LengthSquared() {
            return (((this.X * this.X) + (this.Y * this.Y)) + (this.Z * this.Z));
        }




        internal static void Dot(ref Vector3D source, ref Vector3D other, out double p) {
            p = source.X * other.X + source.Y * other.Y + source.Z * other.Z;
        }


        internal static double Dot(Vector3D source, Vector3D other) {
            return source.X * other.X + source.Y * other.Y + source.Z * other.Z;
        }

        internal static Vector3D Normalize(Vector3D vector3D) {
            return vector3D.Normalize();
        }

        internal static Vector3D Cross(Vector3D vector1, Vector3D vector2) {
            Vector3D vector = Vector3D.Zero;
            vector.X = (vector1.Y * vector2.Z) - (vector1.Z * vector2.Y);
            vector.Y = (vector1.Z * vector2.X) - (vector1.X * vector2.Z);
            vector.Z = (vector1.X * vector2.Y) - (vector1.Y * vector2.X);
            return vector;

        }

        internal double Length() {
            return Math.Sqrt(((X * X) + (Y * Y)) + (Z * Z));

        }


        public static double Distance(Vector3D value1, Vector3D value2) {
            double num3 = value1.X - value2.X;
            double num2 = value1.Y - value2.Y;
            double num = value1.Z - value2.Z;
            double num4 = ((num3 * num3) + (num2 * num2)) + (num * num);
            return Math.Sqrt(num4);

        }
    }
    
    
    



    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector3 : IEquatable<Vector3> {
        #region Private Fields

        private static Vector3 zero = new Vector3(0f, 0f, 0f);
        private static Vector3 one = new Vector3(1f, 1f, 1f);
        private static Vector3 unitX = new Vector3(1f, 0f, 0f);
        private static Vector3 unitY = new Vector3(0f, 1f, 0f);
        private static Vector3 unitZ = new Vector3(0f, 0f, 1f);
        private static Vector3 up = new Vector3(0f, 1f, 0f);
        private static Vector3 down = new Vector3(0f, -1f, 0f);
        private static Vector3 right = new Vector3(1f, 0f, 0f);
        private static Vector3 left = new Vector3(-1f, 0f, 0f);
        private static Vector3 forward = new Vector3(0f, 0f, -1f);
        private static Vector3 backward = new Vector3(0f, 0f, 1f);

        #endregion Private Fields


        #region Public Fields

        public float X;
        public float Y;
        public float Z;

        #endregion Public Fields


        #region Properties

        public float MagnitudeSquared
        {
            get { return X * X + Y * Y + Z * Z; }
        }
        public static Vector3 Zero {
            get { return zero; }
        }

        public static Vector3 One {
            get { return one; }
        }

        public static Vector3 UnitX {
            get { return unitX; }
        }

        public static Vector3 UnitY {
            get { return unitY; }
        }

        public static Vector3 UnitZ {
            get { return unitZ; }
        }

        public static Vector3 Up {
            get { return up; }
        }

        public static Vector3 Down {
            get { return down; }
        }

        public static Vector3 Right {
            get { return right; }
        }

        public static Vector3 Left {
            get { return left; }
        }

        public static Vector3 Forward {
            get { return forward; }
        }

        public static Vector3 Backward {
            get { return backward; }
        }

        #endregion Properties


        #region Constructors

        public Vector3(float x, float y, float z) {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }


        public Vector3(float value) {
            this.X = value;
            this.Y = value;
            this.Z = value;
        }


        public Vector3(Vector2 value, float z) {
            this.X = value.X;
            this.Y = value.Y;
            this.Z = z;
        }


        #endregion Constructors


        #region Public Methods

        public static Vector3 Add(Vector3 value1, Vector3 value2) {
            value1.X += value2.X;
            value1.Y += value2.Y;
            value1.Z += value2.Z;
            return value1;
        }

        public static void Add(ref Vector3 value1, ref Vector3 value2, out Vector3 result) {
            result.X = value1.X + value2.X;
            result.Y = value1.Y + value2.Y;
            result.Z = value1.Z + value2.Z;
        }

        public static Vector3 Barycentric(Vector3 value1, Vector3 value2, Vector3 value3, float amount1, float amount2) {
            return new Vector3(
                MathHelper.Barycentric(value1.X, value2.X, value3.X, amount1, amount2),
                MathHelper.Barycentric(value1.Y, value2.Y, value3.Y, amount1, amount2),
                MathHelper.Barycentric(value1.Z, value2.Z, value3.Z, amount1, amount2));
        }

        public static void Barycentric(ref Vector3 value1, ref Vector3 value2, ref Vector3 value3, float amount1, float amount2, out Vector3 result) {
            result = new Vector3(
                MathHelper.Barycentric(value1.X, value2.X, value3.X, amount1, amount2),
                MathHelper.Barycentric(value1.Y, value2.Y, value3.Y, amount1, amount2),
                MathHelper.Barycentric(value1.Z, value2.Z, value3.Z, amount1, amount2));
        }

        public static Vector3 CatmullRom(Vector3 value1, Vector3 value2, Vector3 value3, Vector3 value4, float amount) {
            return new Vector3(
                MathHelper.CatmullRom(value1.X, value2.X, value3.X, value4.X, amount),
                MathHelper.CatmullRom(value1.Y, value2.Y, value3.Y, value4.Y, amount),
                MathHelper.CatmullRom(value1.Z, value2.Z, value3.Z, value4.Z, amount));
        }

        public static void CatmullRom(ref Vector3 value1, ref Vector3 value2, ref Vector3 value3, ref Vector3 value4, float amount, out Vector3 result) {
            result = new Vector3(
                MathHelper.CatmullRom(value1.X, value2.X, value3.X, value4.X, amount),
                MathHelper.CatmullRom(value1.Y, value2.Y, value3.Y, value4.Y, amount),
                MathHelper.CatmullRom(value1.Z, value2.Z, value3.Z, value4.Z, amount));
        }

        public static Vector3 Clamp(Vector3 value1, Vector3 min, Vector3 max) {
            return new Vector3(
                MathHelper.Clamp(value1.X, min.X, max.X),
                MathHelper.Clamp(value1.Y, min.Y, max.Y),
                MathHelper.Clamp(value1.Z, min.Z, max.Z));
        }

        public static void Clamp(ref Vector3 value1, ref Vector3 min, ref Vector3 max, out Vector3 result) {
            result = new Vector3(
                MathHelper.Clamp(value1.X, min.X, max.X),
                MathHelper.Clamp(value1.Y, min.Y, max.Y),
                MathHelper.Clamp(value1.Z, min.Z, max.Z));
        }
        public static Vector3 Cross2(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3((float)((double)lhs.Y * (double)rhs.Z - (double)lhs.Z * (double)rhs.Y), (float)((double)lhs.Z * (double)rhs.X - (double)lhs.X * (double)rhs.Z), (float)((double)lhs.X * (double)rhs.Y - (double)lhs.Y * (double)rhs.X));
        }
        public static Vector3 Cross(Vector3 vector1, Vector3 vector2) {
            Cross(ref vector1, ref vector2, out vector1);
            return vector1;
        }

        public static void Cross(ref Vector3 vector1, ref Vector3 vector2, out Vector3 result) {
            result = new Vector3(vector1.Y * vector2.Z - vector2.Y * vector1.Z,
                                 -(vector1.X * vector2.Z - vector2.X * vector1.Z),
                                 vector1.X * vector2.Y - vector2.X * vector1.Y);
        }

        public static float Distance(Vector3 vector1, Vector3 vector2) {
            float result;
            DistanceSquared(ref vector1, ref vector2, out result);
            return (float)Math.Sqrt(result);
        }

        public static void Distance(ref Vector3 value1, ref Vector3 value2, out float result) {
            DistanceSquared(ref value1, ref value2, out result);
            result = (float)Math.Sqrt(result);
        }

        public static float DistanceSquared(Vector3 value1, Vector3 value2) {
            float result;
            DistanceSquared(ref value1, ref value2, out result);
            return result;
        }

        public static void DistanceSquared(ref Vector3 value1, ref Vector3 value2, out float result) {
            result = (value1.X - value2.X) * (value1.X - value2.X) +
                     (value1.Y - value2.Y) * (value1.Y - value2.Y) +
                     (value1.Z - value2.Z) * (value1.Z - value2.Z);
        }

        public static Vector3 Divide(Vector3 value1, Vector3 value2) {
            value1.X /= value2.X;
            value1.Y /= value2.Y;
            value1.Z /= value2.Z;
            return value1;
        }

        public static Vector3 Divide(Vector3 value1, float value2) {
            float factor = 1 / value2;
            value1.X *= factor;
            value1.Y *= factor;
            value1.Z *= factor;
            return value1;
        }

        public static void Divide(ref Vector3 value1, float divisor, out Vector3 result) {
            float factor = 1 / divisor;
            result.X = value1.X * factor;
            result.Y = value1.Y * factor;
            result.Z = value1.Z * factor;
        }

        public static void Divide(ref Vector3 value1, ref Vector3 value2, out Vector3 result) {
            result.X = value1.X / value2.X;
            result.Y = value1.Y / value2.Y;
            result.Z = value1.Z / value2.Z;
        }

        public static float Dot(Vector3 vector1, Vector3 vector2) {
            return vector1.X * vector2.X + vector1.Y * vector2.Y + vector1.Z * vector2.Z;
        }

        public static void Dot(ref Vector3 vector1, ref Vector3 vector2, out float result) {
            result = vector1.X * vector2.X + vector1.Y * vector2.Y + vector1.Z * vector2.Z;
        }

        public override bool Equals(object obj) {
            return (obj is Vector3) ? this == (Vector3)obj : false;
        }

        public bool Equals(Vector3 other) {
            return this == other;
        }

        public override int GetHashCode() {
            return (int)(this.X + this.Y + this.Z);
        }

        public static Vector3 Hermite(Vector3 value1, Vector3 tangent1, Vector3 value2, Vector3 tangent2, float amount) {
            Vector3 result = new Vector3();
            Hermite(ref value1, ref tangent1, ref value2, ref tangent2, amount, out result);
            return result;
        }

        public static void Hermite(ref Vector3 value1, ref Vector3 tangent1, ref Vector3 value2, ref Vector3 tangent2, float amount, out Vector3 result) {
            result.X = MathHelper.Hermite(value1.X, tangent1.X, value2.X, tangent2.X, amount);
            result.Y = MathHelper.Hermite(value1.Y, tangent1.Y, value2.Y, tangent2.Y, amount);
            result.Z = MathHelper.Hermite(value1.Z, tangent1.Z, value2.Z, tangent2.Z, amount);
        }

        public float Length() {
            float result;
            DistanceSquared(ref this, ref zero, out result);
            return (float)Math.Sqrt(result);
        }

        public float LengthSquared() {
            float result;
            DistanceSquared(ref this, ref zero, out result);
            return result;
        }

        public static Vector3 Lerp(Vector3 value1, Vector3 value2, float amount) {
            return new Vector3(
                MathHelper.Lerp(value1.X, value2.X, amount),
                MathHelper.Lerp(value1.Y, value2.Y, amount),
                MathHelper.Lerp(value1.Z, value2.Z, amount));
        }

        public static void Lerp(ref Vector3 value1, ref Vector3 value2, float amount, out Vector3 result) {
            result = new Vector3(
                MathHelper.Lerp(value1.X, value2.X, amount),
                MathHelper.Lerp(value1.Y, value2.Y, amount),
                MathHelper.Lerp(value1.Z, value2.Z, amount));
        }

        public static Vector3 Max(Vector3 value1, Vector3 value2) {
            return new Vector3(
                MathHelper.Max(value1.X, value2.X),
                MathHelper.Max(value1.Y, value2.Y),
                MathHelper.Max(value1.Z, value2.Z));
        }

        public static void Max(ref Vector3 value1, ref Vector3 value2, out Vector3 result) {
            result = new Vector3(
                MathHelper.Max(value1.X, value2.X),
                MathHelper.Max(value1.Y, value2.Y),
                MathHelper.Max(value1.Z, value2.Z));
        }

        public static Vector3 Min(Vector3 value1, Vector3 value2) {
            return new Vector3(
                MathHelper.Min(value1.X, value2.X),
                MathHelper.Min(value1.Y, value2.Y),
                MathHelper.Min(value1.Z, value2.Z));
        }

        public static void Min(ref Vector3 value1, ref Vector3 value2, out Vector3 result) {
            result = new Vector3(
                MathHelper.Min(value1.X, value2.X),
                MathHelper.Min(value1.Y, value2.Y),
                MathHelper.Min(value1.Z, value2.Z));
        }

        public static Vector3 Multiply(Vector3 value1, Vector3 value2) {
            value1.X *= value2.X;
            value1.Y *= value2.Y;
            value1.Z *= value2.Z;
            return value1;
        }

        public static Vector3 Multiply(Vector3 value1, float scaleFactor) {
            value1.X *= scaleFactor;
            value1.Y *= scaleFactor;
            value1.Z *= scaleFactor;
            return value1;
        }

        public static void Multiply(ref Vector3 value1, float scaleFactor, out Vector3 result) {
            result.X = value1.X * scaleFactor;
            result.Y = value1.Y * scaleFactor;
            result.Z = value1.Z * scaleFactor;
        }

        public static void Multiply(ref Vector3 value1, ref Vector3 value2, out Vector3 result) {
            result.X = value1.X * value2.X;
            result.Y = value1.Y * value2.Y;
            result.Z = value1.Z * value2.Z;
        }

        public static Vector3 Negate(Vector3 value) {
            value = new Vector3(-value.X, -value.Y, -value.Z);
            return value;
        }

        public static void Negate(ref Vector3 value, out Vector3 result) {
            result = new Vector3(-value.X, -value.Y, -value.Z);
        }

        public void Normalize() {
            Normalize(ref this, out this);
        }

        public static Vector3 Normalize(Vector3 vector) {
            Normalize(ref vector, out vector);
            return vector;
        }
        public static float Magnitude(Vector3 a)
        {
            return Mathf.Sqrt((float)((double)a.X * (double)a.X + (double)a.Y * (double)a.Y + (double)a.Z * (double)a.Z));
        }

        public static Vector3 Normalize2(Vector3 value)
        {
            float num = Vector3.Magnitude(value);
            if ((double)num > 9.99999974737875E-06)
                return value / num;
            return Vector3.zero;
        }

        /// <summary>
        ///   <para>Makes this vector have a magnitude of 1.</para>
        /// </summary>
        public void Normalize2()
        {
            float num = Vector3.Magnitude(this);
            if ((double)num > 9.99999974737875E-06)
                this = this / num;
            else
                this = Vector3.zero;
        }
        public static void Normalize(ref Vector3 value, out Vector3 result) {
            float factor;
            Distance(ref value, ref zero, out factor);
            factor = 1f / factor;
            result.X = value.X * factor;
            result.Y = value.Y * factor;
            result.Z = value.Z * factor;
        }

        public static Vector3 Reflect(Vector3 vector, Vector3 normal) {
            throw new NotImplementedException();
        }

        public static void Reflect(ref Vector3 vector, ref Vector3 normal, out Vector3 result) {
            throw new NotImplementedException();
        }

        public static Vector3 SmoothStep(Vector3 value1, Vector3 value2, float amount) {
            return new Vector3(
                MathHelper.SmoothStep(value1.X, value2.X, amount),
                MathHelper.SmoothStep(value1.Y, value2.Y, amount),
                MathHelper.SmoothStep(value1.Z, value2.Z, amount));
        }

        public static void SmoothStep(ref Vector3 value1, ref Vector3 value2, float amount, out Vector3 result) {
            result = new Vector3(
                MathHelper.SmoothStep(value1.X, value2.X, amount),
                MathHelper.SmoothStep(value1.Y, value2.Y, amount),
                MathHelper.SmoothStep(value1.Z, value2.Z, amount));
        }

        public static Vector3 Subtract(Vector3 value1, Vector3 value2) {
            value1.X -= value2.X;
            value1.Y -= value2.Y;
            value1.Z -= value2.Z;
            return value1;
        }

        public static void Subtract(ref Vector3 value1, ref Vector3 value2, out Vector3 result) {
            result.X = value1.X - value2.X;
            result.Y = value1.Y - value2.Y;
            result.Z = value1.Z - value2.Z;
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder(32);
            sb.Append("{X:");
            sb.Append(this.X);
            sb.Append(" Y:");
            sb.Append(this.Y);
            sb.Append(" Z:");
            sb.Append(this.Z);
            sb.Append("}");
            return sb.ToString();
        }
        /*
        public static Vector3 Transform(Vector3 position, Matrix matrix) {
            Transform(ref position, ref matrix, out position);
            return position;
        }

        public static void Transform(ref Vector3 position, ref Matrix matrix, out Vector3 result) {
            result = new Vector3((position.X * matrix.M11) + (position.Y * matrix.M21) + (position.Z * matrix.M31) + matrix.M41,
                                 (position.X * matrix.M12) + (position.Y * matrix.M22) + (position.Z * matrix.M32) + matrix.M42,
                                 (position.X * matrix.M13) + (position.Y * matrix.M23) + (position.Z * matrix.M33) + matrix.M43);
        }

        public static Vector3 TransformNormal(Vector3 normal, Matrix matrix) {
            TransformNormal(ref normal, ref matrix, out normal);
            return normal;
        }

        public static void TransformNormal(ref Vector3 normal, ref Matrix matrix, out Vector3 result) {
            result = new Vector3((normal.X * matrix.M11) + (normal.Y * matrix.M21) + (normal.Z * matrix.M31),
                                 (normal.X * matrix.M12) + (normal.Y * matrix.M22) + (normal.Z * matrix.M32),
                                 (normal.X * matrix.M13) + (normal.Y * matrix.M23) + (normal.Z * matrix.M33));
        }
        */
        #endregion Public methods


        #region Operators

        public static bool operator ==(Vector3 value1, Vector3 value2) {
            return value1.X == value2.X
                && value1.Y == value2.Y
                && value1.Z == value2.Z;
        }

        public static bool operator !=(Vector3 value1, Vector3 value2) {
            return !(value1 == value2);
        }

        public static Vector3 operator +(Vector3 value1, Vector3 value2) {
            value1.X += value2.X;
            value1.Y += value2.Y;
            value1.Z += value2.Z;
            return value1;
        }

        public static Vector3 operator -(Vector3 value) {
            value = new Vector3(-value.X, -value.Y, -value.Z);
            return value;
        }

        public static Vector3 operator -(Vector3 value1, Vector3 value2) {
            value1.X -= value2.X;
            value1.Y -= value2.Y;
            value1.Z -= value2.Z;
            return value1;
        }

        public static Vector3 operator *(Vector3 value1, Vector3 value2) {
            value1.X *= value2.X;
            value1.Y *= value2.Y;
            value1.Z *= value2.Z;
            return value1;
        }

        public static Vector3 operator *(Vector3 value, float scaleFactor) {
            value.X *= scaleFactor;
            value.Y *= scaleFactor;
            value.Z *= scaleFactor;
            return value;
        }

        public static Vector3 operator *(float scaleFactor, Vector3 value) {
            value.X *= scaleFactor;
            value.Y *= scaleFactor;
            value.Z *= scaleFactor;
            return value;
        }

        public static Vector3 operator /(Vector3 value1, Vector3 value2) {
            value1.X /= value2.X;
            value1.Y /= value2.Y;
            value1.Z /= value2.Z;
            return value1;
        }

        public static Vector3 operator /(Vector3 value, float divider) {
            float factor = 1 / divider;
            value.X *= factor;
            value.Y *= factor;
            value.Z *= factor;
            return value;
        }

        #endregion
    }
}

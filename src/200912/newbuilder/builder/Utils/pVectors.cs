using System;

namespace Builder {
    public struct DoubleFloat {
        public float Value1;
        public float Value2;
        public DoubleFloat(float value1, float value2){
            Value1 = value1;
            Value2 = value2;
        }

    }

    public static class DoubleToFloat{
        public static DoubleFloat Convert(double doubleValue ){
            float floatHigh;
            float floatLow;
            if (doubleValue >= 0.0)
            {
                double doubleHigh = Math.Floor(doubleValue / 65536.0) * 65536.0f;
                floatHigh = (float)doubleHigh;
                floatLow = (float)(doubleValue - doubleHigh);
            }
            else
            {
                double doubleHigh =  Math.Floor(-doubleValue / 65536.0) * 65536.0f;
                floatHigh = (float)-doubleHigh;
                floatLow = (float)(doubleValue + doubleHigh);
            }

            return new DoubleFloat(floatHigh, floatLow);
        }

    }



    public struct pVector2 {
        private double _x;
        private double _y;


        public static pVector2 operator *(pVector2 value1, pVector2 value2) {
            pVector2 vector = new pVector2();
            vector.X = value1.X * value2.X;
            vector.Y = value1.Y * value2.Y;
            return vector;
        }




        public static pVector2 Zero {
            get { return new pVector2(0, 0); }
        }

        public override string ToString() {
            return _x + "|" + _y;
        }

        public double X {
            get { return _x; }
            set { _x = value; }
        }
        public double Y {
            get { return _y; }
            set { _y = value; }
        }



        /// <summary>
        /// Creates a vector3 of doubles.
        /// </summary>
        public pVector2(double x, double y) {
            _x = x;
            _y = y;
        }

        public static pVector2 operator +(pVector2 value, float value2) {
            return new pVector2(value.X + value2, value.Y + value2);
        }
        public static pVector2 operator +(pVector2 value, double value2) {
            return new pVector2(value.X + value2, value.Y + value2);
        }
        public static pVector2 operator +(pVector2 value, pVector2 value2) {
            return new pVector2(value.X + value2.X, value.Y + value2.Y);
        }

        public static pVector2 operator *(pVector2 value, float value2) {
            return new pVector2(value.X * value2, value.Y * value2);
        }
        public static pVector2 operator *(pVector2 value, double value2) {
            return new pVector2(value.X * value2, value.Y * value2);
        }

        public static pVector2 operator -(pVector2 value, float value2) {
            return new pVector2(value.X - value2, value.Y - value2);
        }
        public static pVector2 operator -(pVector2 value, double value2) {
            return new pVector2(value.X - value2, value.Y - value2);
        }
        public static pVector2 operator -(pVector2 value, pVector2 value2) {
            return new pVector2(value.X - value2.X, value.Y - value2.Y);
        }
        public static pVector2 operator /(pVector2 value, float value2) {
            return new pVector2(value.X / value2, value.Y / value2);
        }
        public static pVector2 operator /(pVector2 value, double value2) {
            return new pVector2(value.X / value2, value.Y / value2);
        }


        public static Vector2 ToVector2(pVector2 value) {
            return new Vector2((float)value.X, (float)value.Y);
        }

        public Vector2 ToVector2() {
            return new Vector2((float)_x, (float)_y);
        }

        /// <summary>
        /// Returns the magnitude of this DVector.
        /// </summary>
        /// <returns></returns>
        double Magnitude() {
            return Math.Sqrt(Magnitude2());
        }

        /// <summary>
        /// Returns the magnitude ^2 of this DVector.
        /// </summary>
        /// <returns></returns>
        double Magnitude2() {
            return (_x * _x) + (_y * _y);
        }

        public static double Distance(pVector2 value1, pVector2 value2) {
            double num2 = value1.X - value2.X;
            double num = value1.Y - value2.Y;
            double num3 = (num2 * num2) + (num * num);
            return Math.Sqrt(num3);
        }

 

 


        public static double Dot(pVector2 value1, pVector2 value2) {
            return ((value1.X * value2.X) + (value1.Y * value2.Y));
        }


        /// <summary>
        /// Converts this vector into a unit vector.
        /// </summary>
        void Normalize() {
            _x /= Magnitude();
            _y /= Magnitude();
            //return new pVector2(_x, _y);
        }

 

        /// <summary>
        /// Returns the unit vector of the input DVector3.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static pVector2 Normalize(pVector2 vector) {
            return vector /= Math.Sqrt((vector.X * vector.X) + (vector.Y * vector.Y));
        }
    }
    [System.Diagnostics.DebuggerDisplay("{_x}, {_y}, {_z}")]
    public struct pVector3 {
        private double _x;
        private double _y;
        private double _z;
        public override string ToString(){

            return _x + "|" + _y + "|" + _z;
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
        public static pVector3 UnitX {
            get {
                return _unitX;
            }
        }
        public static pVector3 UnitY {
            get {
                return _unitY;
            }
        }

        public static pVector3 UnitZ {
            get {
                return _unitZ;
            }
        }

        private static pVector3 _unitX = new pVector3(1, 0, 0);

        private static pVector3 _unitY = new pVector3(0, 1, 0);

        private static pVector3 _unitZ = new pVector3(1, 0, 1);


        /// <summary>
        /// Creates a vector3 of doubles.
        /// </summary>
        public pVector3(double x, double y, double z) {
            _x = x;
            _y = y;
            _z = z;
        }

        /// <summary>
        /// Creates a vector3 of doubles.
        /// </summary>
        public pVector3(Vector3 value) {
            _x = (double)value.X;
            _y = (double)value.Y;
            _z = (double)value.Z;
        }

        public Vector3 Subtract(pVector3 value2) {
            Vector3 loReturn;
            loReturn.X = (float)(_x - value2.X);
            loReturn.Y = (float)(_y - value2.Y);
            loReturn.Z = (float)(_z - value2.Z);
            return loReturn;
        }

        public static pVector3 operator +(pVector3 value, float value2) {
            return new pVector3(value.X + value2, value.Y + value2, value.Z + value2);
        }
        public static pVector3 operator +(pVector3 value, double value2) {
            return new pVector3(value.X + value2, value.Y + value2, value.Z + value2);
        }
        public static pVector3 operator +(pVector3 value, Vector3 value2) {
            return new pVector3(value.X + value2.X, value.Y + value2.Y, value.Z + value2.Z);
        }
        public static pVector3 operator +(pVector3 value, pVector3 value2) {
            return new pVector3(value.X + value2.X, value.Y + value2.Y, value.Z + value2.Z);
        }

        public static pVector3 operator *(pVector3 value, float value2) {
            return new pVector3(value.X * value2, value.Y * value2, value.Z * value2);
        }
        public static pVector3 operator *(pVector3 value, double value2) {
            return new pVector3(value.X * value2, value.Y * value2, value.Z * value2);
        }
        public static pVector3 operator *(pVector3 value, Vector3 value2) {
            return new pVector3(value.X * value2.X, value.Y * value2.Y, value.Z * value2.Z);
        }
        public static pVector3 operator *(pVector3 value, pVector3 value2) {
            return new pVector3(value.X * value2.X, value.Y * value2.Y, value.Z * value2.Z);
        }

        public static pVector3 operator -(pVector3 value, float value2) {
            return new pVector3(value.X - value2, value.Y - value2, value.Z - value2);
        }
        public static pVector3 operator -(pVector3 value, double value2) {
            return new pVector3(value.X - value2, value.Y - value2, value.Z - value2);
        }
        public static pVector3 operator -(pVector3 value, Vector3 value2) {
            return new pVector3(value.X - value2.X, value.Y - value2.Y, value.Z - value2.Z);
        }
        public static pVector3 operator -(pVector3 value, pVector3 value2) {
            return new pVector3(value.X - value2.X, value.Y - value2.Y, value.Z - value2.Z);
        }

        public static bool operator ==(pVector3 value, pVector3 value2) {
            if (value.X == value2.X && value.Y == value2.Y && value.Z == value2.Z) {
                return true;
            }
            else {
                return false;
            }
        }
        public static bool operator !=(pVector3 value, pVector3 value2) {
            if (value.X != value2.X || value.Y != value2.Y || value.Z != value2.Z) {
                return true;
            }
            else {
                return false;
            }
        }

        public static pVector3 operator /(pVector3 value, float value2) {
            return new pVector3(value.X / value2, value.Y / value2, value.Z / value2);
        }
        public static pVector3 operator /(pVector3 value, double value2) {
            return new pVector3(value.X / value2, value.Y / value2, value.Z / value2);
        }
        public static pVector3 operator /(pVector3 value, Vector3 value2) {
            return new pVector3(value.X / value2.X, value.Y / value2.Y, value.Z / value2.Z);
        }
        public static pVector3 operator /(pVector3 value, pVector3 value2) {
            return new pVector3(value.X / value2.X, value.Y / value2.Y, value.Z / value2.Z);
        }

        // Presets
        public static pVector3 Backward { get { return new pVector3(0, 0, 1); } }
        public static pVector3 Down { get { return new pVector3(0, -1, 0); } }
        public static pVector3 Forward { get { return new pVector3(0, 0, -1); } }
        public static pVector3 Left { get { return new pVector3(-1, 0, 0); } }
        public static pVector3 One { get { return new pVector3(1, 1, 1); } }
        public static pVector3 Right { get { return new pVector3(1, 0, 0); } }
        public static pVector3 Up { get { return new pVector3(0, 1, 0); } }
        public static pVector3 Zero { get { return new pVector3(0, 0, 0); } }

        /// <summary>
        /// Returns the magnitude of this DVector.
        /// </summary>
        /// <returns></returns>
        public float Magnitude() {
            return (float)Math.Sqrt(Magnitude2());
        }

        /// <summary>
        /// Returns the magnitude ^2 of this DVector.
        /// </summary>
        /// <returns></returns>
        public double Magnitude2() {
            return (_x * _x) + (_y * _y) + (_z * _z);
        }

        /// <summary>
        /// Converts this vector into a unit vector.
        /// </summary>
        public void Normalize() {
            double ldMag = Magnitude();
            _x /= ldMag;
            _y /= ldMag;
            _z /= ldMag;
            return;
            double num2 = ((this.X * this.X) + (this.Y * this.Y)) + (this.Z * this.Z);
            double num = 1f / (Math.Sqrt(num2));
                _x *= num;
                _y *= num;
                _z *= num;

 




        }

        /// <summary>
        /// Returns the unit vector of the input DVector3.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static pVector3 Normalise(pVector3 vector) {
            return vector /= Math.Sqrt((vector.X * vector.X) + (vector.Y * vector.Y) + (vector.Z * vector.Z));
        }

        /// <summary>
        /// Calculates the normal vector of a surface.
        /// </summary>
        public static pVector3 Cross(pVector3 vectorA, pVector3 vectorB) {
            pVector3 lvReturnValue = pVector3.Zero;
            lvReturnValue.X = vectorA.Y * vectorB.Z - vectorA.Z * vectorB.Y;
            lvReturnValue.Y = vectorA.Z * vectorB.X - vectorA.X * vectorB.Z;
            lvReturnValue.Z = vectorA.X * vectorB.Y - vectorA.Y * vectorB.X;

            lvReturnValue.X = (vectorA.Y * vectorB.Z) - (vectorA.Z * vectorB.Y);
            lvReturnValue.Y = (vectorA.Z * vectorB.X) - (vectorA.X * vectorB.Z);
            lvReturnValue.Z = (vectorA.X * vectorB.Y) - (vectorA.Y * vectorB.X);



            return lvReturnValue;
        }

        public static float Dot(pVector3 vectorA, pVector3 vectorB) {
            double lvReturnValue;
            lvReturnValue = vectorA.X * vectorB.X +
                            vectorA.Y * vectorB.Y +
                            vectorA.Z * vectorB.Z;
            return (float)lvReturnValue;
        }

        /// <summary>
        /// Converts the DVector3 to Vector3.
        /// </summary>
        public Vector3 ConvertToVector3() {
            return new Vector3((float)_x, (float)_y, (float)_z);
        }

        /// <summary>
        /// Converts the DVector3 to Vector3.
        /// </summary>
        public static Vector3 ConvertToVector3(pVector3 value) {
            return new Vector3((float)value.X, (float)value.Y, (float)value.Z);
        }

        /// <summary>
        /// Returns the distance between the two input vectors.
        /// </summary>
        public static float Distance(pVector3 value1, pVector3 value2) {
            return (value1 - value2).Magnitude();
        }

        public bool Equals(pVector3 other){
            return other._x == _x && other._y == _y && other._z == _z;
        }

        public override bool Equals(object obj){
            if (ReferenceEquals(null, obj)){
                return false;
            }
            if (obj.GetType() != typeof (pVector3)){
                return false;
            }
            return Equals((pVector3) obj);
        }

        public override int GetHashCode(){
            unchecked{
                int result = _x.GetHashCode();
                result = (result*397) ^ _y.GetHashCode();
                result = (result*397) ^ _z.GetHashCode();
                return result;
            }
        }
    }

}

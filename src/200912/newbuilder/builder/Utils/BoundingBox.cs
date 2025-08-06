// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Builder {
    //[DataContract]
    [DebuggerDisplay("{DebugDisplayString,nq}")]
    public struct BoundingBox : IEquatable<BoundingBox>
    {

        #region Public Fields

        //[DataMember]
        public Vector3 Min;
      
        //[DataMember]
        public Vector3 Max;

        public const int CornerCount = 8;

        #endregion Public Fields


        #region Public Constructors

        public BoundingBox(Vector3 min, Vector3 max)
        {
            this.Min = min;
            this.Max = max;
        }

        #endregion Public Constructors


        #region Public Methods

        private static readonly Vector3 MaxVector3 = new Vector3(float.MaxValue);
        private static readonly Vector3 MinVector3 = new Vector3(float.MinValue);


        /// <summary>
        /// Create a bounding box from the given list of points.
        /// </summary>
        /// <param name="points">The array of Vector3 instances defining the point cloud to bound</param>
        /// <param name="index">The base index to start iterating from</param>
        /// <param name="count">The number of points to iterate</param>
        /// <returns>A bounding box that encapsulates the given point cloud.</returns>
        /// <exception cref="System.ArgumentException">Thrown if the given array is null or has no points.</exception>
        public static BoundingBox CreateFromPoints(Vector3[] points, int index = 0, int count = -1)
        {
            if (points == null || points.Length == 0)
                throw new ArgumentException();

            if (count == -1)
                count = points.Length;

            var minVec = MaxVector3;
            var maxVec = MinVector3;
            for (int i = index; i < count; i++)
            {                
                minVec.X = (minVec.X < points[i].X) ? minVec.X : points[i].X;
                minVec.Y = (minVec.Y < points[i].Y) ? minVec.Y : points[i].Y;
                minVec.Z = (minVec.Z < points[i].Z) ? minVec.Z : points[i].Z;

                maxVec.X = (maxVec.X > points[i].X) ? maxVec.X : points[i].X;
                maxVec.Y = (maxVec.Y > points[i].Y) ? maxVec.Y : points[i].Y;
                maxVec.Z = (maxVec.Z > points[i].Z) ? maxVec.Z : points[i].Z;
            }

            return new BoundingBox(minVec, maxVec);
        }


        /// <summary>
        /// Create a bounding box from the given list of points.
        /// </summary>
        /// <param name="points">The list of Vector3 instances defining the point cloud to bound</param>
        /// <param name="index">The base index to start iterating from</param>
        /// <param name="count">The number of points to iterate</param>
        /// <returns>A bounding box that encapsulates the given point cloud.</returns>
        /// <exception cref="System.ArgumentException">Thrown if the given list is null or has no points.</exception>
        public static BoundingBox CreateFromPoints(List<Vector3> points, int index = 0, int count = -1)
        {
            if (points == null || points.Count == 0)
                throw new ArgumentException();

            if (count == -1)
                count = points.Count;

            var minVec = MaxVector3;
            var maxVec = MinVector3;
            for (int i = index; i < count; i++)
            {
                minVec.X = (minVec.X < points[i].X) ? minVec.X : points[i].X;
                minVec.Y = (minVec.Y < points[i].Y) ? minVec.Y : points[i].Y;
                minVec.Z = (minVec.Z < points[i].Z) ? minVec.Z : points[i].Z;

                maxVec.X = (maxVec.X > points[i].X) ? maxVec.X : points[i].X;
                maxVec.Y = (maxVec.Y > points[i].Y) ? maxVec.Y : points[i].Y;
                maxVec.Z = (maxVec.Z > points[i].Z) ? maxVec.Z : points[i].Z;
            }

            return new BoundingBox(minVec, maxVec);
        }


        /// <summary>
        /// Create a bounding box from the given list of points.
        /// </summary>
        /// <param name="points">The list of Vector3 instances defining the point cloud to bound</param>
        /// <returns>A bounding box that encapsulates the given point cloud.</returns>
        /// <exception cref="System.ArgumentException">Thrown if the given list has no points.</exception>
        public static BoundingBox CreateFromPoints(IEnumerable<Vector3> points)
        {
            if (points == null)
                throw new ArgumentNullException();

            var empty = true;
            var minVec = MaxVector3;
            var maxVec = MinVector3;
            foreach (var ptVector in points)
            {
                minVec.X = (minVec.X < ptVector.X) ? minVec.X : ptVector.X;
                minVec.Y = (minVec.Y < ptVector.Y) ? minVec.Y : ptVector.Y;
                minVec.Z = (minVec.Z < ptVector.Z) ? minVec.Z : ptVector.Z;

                maxVec.X = (maxVec.X > ptVector.X) ? maxVec.X : ptVector.X;
                maxVec.Y = (maxVec.Y > ptVector.Y) ? maxVec.Y : ptVector.Y;
                maxVec.Z = (maxVec.Z > ptVector.Z) ? maxVec.Z : ptVector.Z;

                empty = false;
            }
            if (empty)
                throw new ArgumentException();

            return new BoundingBox(minVec, maxVec);
        }

        public static BoundingBox CreateMerged(BoundingBox original, BoundingBox additional)
        {
            BoundingBox result;
            CreateMerged(ref original, ref additional, out result);
            return result;
        }

        public static void CreateMerged(ref BoundingBox original, ref BoundingBox additional, out BoundingBox result)
        {
            result.Min.X = Math.Min(original.Min.X, additional.Min.X);
            result.Min.Y = Math.Min(original.Min.Y, additional.Min.Y);
            result.Min.Z = Math.Min(original.Min.Z, additional.Min.Z);
            result.Max.X = Math.Max(original.Max.X, additional.Max.X);
            result.Max.Y = Math.Max(original.Max.Y, additional.Max.Y);
            result.Max.Z = Math.Max(original.Max.Z, additional.Max.Z);
        }

        public bool Equals(BoundingBox other)
        {
            return (this.Min == other.Min) && (this.Max == other.Max);
        }

        public override bool Equals(object obj)
        {
            return (obj is BoundingBox) ? this.Equals((BoundingBox)obj) : false;
        }

        public Vector3[] GetCorners()
        {
            return new Vector3[] {
                new Vector3(this.Min.X, this.Max.Y, this.Max.Z), 
                new Vector3(this.Max.X, this.Max.Y, this.Max.Z),
                new Vector3(this.Max.X, this.Min.Y, this.Max.Z), 
                new Vector3(this.Min.X, this.Min.Y, this.Max.Z), 
                new Vector3(this.Min.X, this.Max.Y, this.Min.Z),
                new Vector3(this.Max.X, this.Max.Y, this.Min.Z),
                new Vector3(this.Max.X, this.Min.Y, this.Min.Z),
                new Vector3(this.Min.X, this.Min.Y, this.Min.Z)
            };
        }

        public void GetCorners(Vector3[] corners)
        {
            if (corners == null)
            {
                throw new ArgumentNullException("corners");
            }
            if (corners.Length < 8)
            {
                throw new ArgumentOutOfRangeException("corners", "Not Enought Corners");
            }
            corners[0].X = this.Min.X;
            corners[0].Y = this.Max.Y;
            corners[0].Z = this.Max.Z;
            corners[1].X = this.Max.X;
            corners[1].Y = this.Max.Y;
            corners[1].Z = this.Max.Z;
            corners[2].X = this.Max.X;
            corners[2].Y = this.Min.Y;
            corners[2].Z = this.Max.Z;
            corners[3].X = this.Min.X;
            corners[3].Y = this.Min.Y;
            corners[3].Z = this.Max.Z;
            corners[4].X = this.Min.X;
            corners[4].Y = this.Max.Y;
            corners[4].Z = this.Min.Z;
            corners[5].X = this.Max.X;
            corners[5].Y = this.Max.Y;
            corners[5].Z = this.Min.Z;
            corners[6].X = this.Max.X;
            corners[6].Y = this.Min.Y;
            corners[6].Z = this.Min.Z;
            corners[7].X = this.Min.X;
            corners[7].Y = this.Min.Y;
            corners[7].Z = this.Min.Z;
        }

        public override int GetHashCode()
        {
            return this.Min.GetHashCode() + this.Max.GetHashCode();
        }

        public bool Intersects(BoundingBox box)
        {
            bool result;
            Intersects(ref box, out result);
            return result;
        }

        public void Intersects(ref BoundingBox box, out bool result)
        {
            if ((this.Max.X >= box.Min.X) && (this.Min.X <= box.Max.X))
            {
                if ((this.Max.Y < box.Min.Y) || (this.Min.Y > box.Max.Y))
                {
                    result = false;
                    return;
                }

                result = (this.Max.Z >= box.Min.Z) && (this.Min.Z <= box.Max.Z);
                return;
            }

            result = false;
            return;
        }

        public static bool operator ==(BoundingBox a, BoundingBox b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(BoundingBox a, BoundingBox b)
        {
            return !a.Equals(b);
        }

        public override string ToString()
        {
            return "{{Min:" + this.Min.ToString() + " Max:" + this.Max.ToString() + "}}";
        }

        /// <summary>
        /// Deconstruction method for <see cref="BoundingBox"/>.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public void Deconstruct(out Vector3 min, out Vector3 max)
        {
            min = Min;
            max = Max;
        }

        #endregion Public Methods
    }
}

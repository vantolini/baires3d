﻿using System.Collections.Generic;
using StraightSkeletonNet.Primitives;

namespace StraightSkeletonNet
{
    /// <summary> Represents skeleton algorithm results. </summary>
    public class Skeleton
    {
        /// <summary> Result of skeleton algorithm for edge. </summary>
        public readonly List<EdgeResult> Edges;

        /// <summary> Distance points from edges. </summary>
        public readonly Dictionary<Vector2d, double> Distances;

        /// <summary> Creates instance of <see cref="Skeleton"/>. </summary>
        public Skeleton(List<EdgeResult> edges, Dictionary<Vector2d, double> distances)
        {
            Edges = edges;
            Distances = distances;
        }
    }
}
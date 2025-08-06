using System;
using System.Collections.Generic;

namespace Builder
{
    internal class StreetCoord
    {
        public Vector3 Coord = Vector3.Zero;
        public List<int> Streets = new List<int>();

        public void addID(int id)
        {
            //if (!Streets.Contains(id))
            Streets.Add(id);
        }

        public StreetCoord(Vector3 coord)
        {
            Coord = coord;
        }
    }
}
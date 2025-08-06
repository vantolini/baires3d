using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace b3d
{
    public class StreetSearchResults
    {
    }

    public class StreetManager 
    {
        public string Name;
        public string StreetToDraw = null;
        public List<string> CalleNames = new List<string>();
        public List<int> CalleIds = new List<int>();
        public List<CalleComponent> Calles;
        public Dictionary<int, List<int>> Intersections = new Dictionary<int, List<int>>();


        private bool drawSelectedStreets = false;
        private int id1 = -1;
        private int id2 = -1;

        public void Find(string calleOrigen, string calleDestino)
        {
            for (int i = 0; i < Calles.Count; i++)
            {
                if (Calles[i].Name == calleOrigen)
                {
                    id1 = i;
                }

                if (Calles[i].Name == calleDestino)
                {
                    id2 = i;
                }

                if (id1 != -1 && id2 != -1)
                {
                    drawSelectedStreets = false;
                    break;
                }
            }
            bool found = false;
            if (id1 != -1 && id2 != -1)
            {
                List<int> ints = new List<int>();
                ints = Intersections[id1];
                for (int x = 0; x < ints.Count; x++)
                {
                    if (ints[x] == id2)
                    {
                        found = true;
                        break;
                    }
                }
                if (found)
                {
                    Vector3 IntersectionPoint = Vector3.Zero;
                    bool foundIP = false;
                    for (int c1 = 0; c1 < Calles[id1].Tramos.Count; c1++)
                    {
                        for (int t1 = 0; t1 < Calles[id1].Tramos[c1].Puntos.Count; t1++)
                        {
                            Vector3 vx1 = Calles[id1].Tramos[c1].Puntos[t1];

                            for (int c2 = 0; c2 < Calles[id2].Tramos.Count; c2++)
                            {
                                for (int t2 = 0; t2 < Calles[id2].Tramos[c2].Puntos.Count; t2++)
                                {
                                    Vector3 vx2 = Calles[id2].Tramos[c2].Puntos[t2];
                                    if (vx1 == vx2)
                                    {
                                        IntersectionPoint = vx1;
                                        foundIP = true;
                                        break;
                                    }
                                }
                                if (foundIP)
                                    break;
                            }

                            if (foundIP)
                                break;
                        }

                        if (foundIP)
                            break;
                    }

                    if (foundIP)
                    {
                        drawSelectedStreets = true;
                        Console.WriteLine("Moving from:");
                        Console.WriteLine(Constants.Camera.Position);
                        Console.WriteLine("To:");


                        Console.WriteLine(IntersectionPoint);
                        Vector3 pos = IntersectionPoint;
                        pos.Y = 13;
                        Constants.Camera.Position = pos;
                        pos.Y = 0;
                        Constants.Camera.camera.LookAt(pos);
                    }
                }
                else
                {
                    drawSelectedStreets = false;
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoAPI;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;
using GisSharpBlog.NetTopologySuite.IO;
using Microsoft.Xna.Framework;
using Point=Triangulator.Geometry.Point;

namespace b3d
{
    public class ShapefileLoader : IDisposable
    {
        // public ShapefileReader shapeFile;


        public float ManzanaHeight = 0.94f;
        public float Altura = 0;

        public int HeightField = -1;

        public string AnchoCamino;
        public string AnchoAvenida;

        private ShapefileDataReader shapeFile;
        public ShapefileLoader(string filePath)
        {
            OpenShapefile(filePath);
        }


        private void OpenShapefile(string layerPath)
        {
            if (shapeFile != null && shapeFile.IsClosed == false){
                throw new Exception("WATAFAAAAAAAACK");
            }
            shapeFile = new ShapefileDataReader(layerPath, new GeometryFactory());
        }


        public List<Feature> ParsePoints()
        {
            List<Feature> Features = new List<Feature>();
            Feature feature;

            while (shapeFile.Read())
            {
                feature = new Feature();
                for (int q = 0; q < shapeFile.FieldCount; q++)
                {
                    feature.Data.Add(shapeFile[q].ToString());
                }
                IGeometry geometry = shapeFile.Geometry;

                feature.Points = new List<Vector3>();

                switch (shapeFile.ShapeHeader.ShapeType)
                {
                    case ShapeGeometryType.Point:

                        #region Point

                        IPoint pnt = geometry as IPoint;

                        if (pnt != null)
                        {
                            feature.Points.Add(
                                LayerHelper.getVector(
                                    pnt.X,
                                    0,
                                    pnt.Y
                                    )
                                );
                        }
                        else
                        {
                            continue;
                        }
                        break;

                        #endregion

                    default:
                        Console.Write(shapeFile.ShapeHeader.ShapeType.ToString());
                        break;
                }

                Features.Add(feature);
            }
            shapeFile.Close();
            return Features;
        }

        public List<Feature> ParsePolygons()
        {
            float current_height = 0;
            List<Feature> Features = new List<Feature>();
            Feature feature;

            while (shapeFile.Read())
            {
                feature = new Feature();
                for (int q = 0; q < shapeFile.FieldCount; q++)
                {
                    feature.Data.Add(shapeFile[q].ToString());
                }

                IGeometry geometry = shapeFile.Geometry;
                //HeightField = 11;
                if (HeightField == -1)
                {
                    current_height = Convert.ToSingle(Altura); //Convert.ToDouble(Altura);
                }
                else
                {
                    if (feature.Data[HeightField].ToString() == "" || feature.Data[HeightField].ToString() == "0" ||
                        feature.Data[HeightField].ToString() == "1")
                    {
                        feature.Data[HeightField] = "1";
                    }else{
                        feature.Data[HeightField] = feature.Data[HeightField];
                    }
                    current_height = Convert.ToSingle(feature.Data[HeightField].ToString().Replace(".", ","));
                    if (current_height == 0)
                    {
                        current_height = 1f;
                    }
                    current_height *= 3f;

                }
                feature.Height = current_height;
                feature.Points = new List<Vector3>();

                switch (shapeFile.ShapeHeader.ShapeType)
                {
                    case ShapeGeometryType.Polygon:

                        #region Polygon

                        IPolygon polygon = geometry as IPolygon;

                        int cnt = 0;

                        if (polygon != null)
                        {
                            Int32 pointCount = polygon.ExteriorRing.Coordinates.Length;
                            foreach (ICoordinate vertex in polygon.ExteriorRing.Coordinates)
                            {
                                if (cnt == (pointCount - 1))
                                {
                                    break;
                                }
                                cnt++;

                                feature.Points.Add(LayerHelper.getVector(vertex.X, current_height, vertex.Y));
                            }
                            //feature.Points.Reverse();
                        }
                        else
                        {
                            IMultiPolygon multipolygon = geometry as IMultiPolygon;
                            if (multipolygon != null)
                            {
                                foreach (IPolygon p in multipolygon.Geometries)
                                {
                                    Int32 pointCount = p.ExteriorRing.Coordinates.Length;

                                    foreach (ICoordinate vertex2 in p.ExteriorRing.Coordinates)
                                    {
                                        if (cnt == (pointCount - 1))
                                        {
                                            break;
                                        }
                                        cnt++;

                                        feature.Points.Add(LayerHelper.getVector(vertex2.X, current_height, vertex2.Y));
                                    }
                                    //feature.Points.Reverse();
                                }
                            }
                            else
                            {
                                throw new Exception();
                            }
                        }
                        break;

                        #endregion

                    default:
                        Console.Write(shapeFile.ShapeHeader.ShapeType.ToString());
                        break;
                }
                feature.Points.Reverse();
                if (feature.Points.Count > 3){

                    bool newtess = true;

                    if(newtess){
                        List<Triangulator.Geometry.Point> ptlist = new List<Point>();

                        //if (feature.Points.Count > 400) {
                            //Console.WriteLine("Triangulating " + feature.Points.Count + " vertices...");
                        //}
                        for (int i = 0; i < feature.Points.Count; i++) {
                            ptlist.Add(new Triangulator.Geometry.Point((float)feature.Points[i].X, (float)feature.Points[i].Z));
                        }

                        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

                        stopwatch.Start();

                        List<Triangulator.Geometry.Triangle> tris = Triangulator.Delauney.Triangulate(ptlist);
                        stopwatch.Stop();

                        //Console.WriteLine(feature.Points.Count + " vertices in : {0}", stopwatch.Elapsed);

                        feature.numVertices = tris.Count * 3;
                        feature.numPrimitives = tris.Count;
                        feature.Techo = new List<Vector3>();
                        foreach (Triangulator.Geometry.Triangle t in tris){
                            Vector3 vec1 = new Vector3((float)ptlist[t.p1].X, current_height, (float)ptlist[t.p1].Y);
                            Vector3 vec2 = new Vector3((float)ptlist[t.p2].X, current_height, (float)ptlist[t.p2].Y);
                            Vector3 vec3 = new Vector3((float)ptlist[t.p3].X, current_height, (float)ptlist[t.p3].Y);
                            feature.Techo.Add(vec1);
                            feature.Techo.Add(vec2);
                            feature.Techo.Add(vec3);
				        }

                        int[] sourceIndices;
                        int[] newIndx = new int[feature.Techo.Count];
                        for (int i = 0; i < feature.Techo.Count; i++) {
                            newIndx[i] = i;
                        }

                        feature.IndicesTecho = newIndx;
                        feature.Techo.Reverse();
                    }else{

                        Vector2[] ptlist = new Vector2[feature.Points.Count];
                        if(ptlist.Length > 100){
                            Console.WriteLine("Triangulating " + ptlist.Length + " vertices...");
                        }
                        for (int i = 0; i < feature.Points.Count; i++)
                        {
                            ptlist[i] = new Vector2((float) feature.Points[i].X, (float) feature.Points[i].Z);
                        }
                        int[] sourceIndices;
                        TriangulatorGood.Triangulate(
                            ptlist,
                            WindingOrder.CounterClockwise,
                            out ptlist,
                            out sourceIndices);

                        feature.numVertices = ptlist.Length;
                        feature.numPrimitives = ptlist.Length/3;
                        feature.Techo = new List<Vector3>();
                        for (int i = 0; i < sourceIndices.Length; i++)
                        {
                            feature.Techo.Add(
                                new Vector3(
                                    ptlist[sourceIndices[i]].X,
                                    current_height,
                                    ptlist[sourceIndices[i]].Y
                                    )
                                );
                        }
                        if (sourceIndices.Length != ptlist.Length)
                        {
                            //throw new Exception();
                        }

                        int[] newIndx = new int[feature.Techo.Count];
                        for (int i = 0; i < feature.Techo.Count; i++)
                        {
                            newIndx[i] = i;
                        }

                        feature.IndicesTecho = newIndx;
                    }
                }
                else
                {
                    if (feature.Points.Count < 3)
                    {
                        throw new Exception();
                    }
                    else
                    {
                        feature.IndicesTecho = new int[feature.Points.Count];
                        for (int i = 0; i < feature.IndicesTecho.Length; i++)
                        {
                            feature.IndicesTecho[i] = i;
                        }
                        feature.Techo = feature.Points;
                    }
                }

                Features.Add(feature);
            }
            shapeFile.Close();
            return Features;
        }

        public List<Feature> ParsePolyLine()
        {
            List<Feature> Features = new List<Feature>();
            Feature feature;
            List<Vector3> points = new List<Vector3>();
            string AnchoCamino = "2";
            while (shapeFile.Read())
            {
                feature = new Feature();
                for (int q = 0; q < shapeFile.FieldCount; q++)
                {
                    feature.Data.Add(shapeFile[q].ToString());
                }

                points.Clear();

                IGeometry geometry = shapeFile.Geometry;

                feature.Points = new List<Vector3>();

                switch (shapeFile.ShapeHeader.ShapeType)
                {
                    case ShapeGeometryType.LineString:

                        #region PolyLine

                        ILineString pntLine = geometry as ILineString;
                        if (pntLine != null)
                        {
                            List<Vector3> vecs = new List<Vector3>();
                            Int32 pointCount = pntLine.Coordinates.Length;

                            foreach (ICoordinate vertex in pntLine.Coordinates)
                            {
                                feature.OrigLine.Add(LayerHelper.getVector(vertex.X, 0,
                                                                           vertex.Y)
                                    );
                            }
                            feature.OrigLine.Reverse();
                            feature.Tramos.Add(new Tramo(feature.OrigLine));

                            Features.Add(feature);
                        }
                        else
                        {
                            IMultiLineString imul = geometry as IMultiLineString;

                            foreach (ILineString str in imul.Geometries)
                            {
                                List<Vector3> OrigLine = new List<Vector3>();

                                foreach (ICoordinate vertex in str.Coordinates)
                                {
                                    OrigLine.Add(LayerHelper.getVector(vertex.X, 0,
                                                                       vertex.Y)
                                        );
                                }
                                OrigLine.Reverse();
                                feature.Tramos.Add(new Tramo(OrigLine));
                            }

                            Features.Add(feature);
                        }
                        break;

                        #endregion

                    default:
                        Console.Write(shapeFile.ShapeHeader.ShapeType.ToString());
                        break;
                }
            }
            shapeFile.Close();
            return Features;
        }


        public void Close()
        {
            if (shapeFile != null)
            {
                if (!shapeFile.IsClosed)
                    shapeFile.Close();

                shapeFile = null;
            }
        }

        public void Dispose()
        {
            if (shapeFile != null)
            {
                if (!shapeFile.IsClosed)
                    shapeFile.Close();

                shapeFile = null;
            }
        }
    }
}
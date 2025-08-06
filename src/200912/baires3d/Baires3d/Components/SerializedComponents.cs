using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace b3d
{
    [Serializable]
    public class SerializedComponent
    {
        public string Name;
        public SerializedComponentInfo Info;
        public string SplitField;
        public SerializedComponentDataGroup Data;
    }


    [Serializable]
    public class SerializedComponentInfo
    {
        public List<string> Columns;

        public string DateCreation;
        public string Author;
        public ComponentType Type;

        public SerializedComponentInfo()
        {
        }

        public SerializedComponentInfo(string author, ComponentType type, string dateCreation)
        {
            Author = author;
            Type = type;
            DateCreation = dateCreation;
        }
    }

    [Serializable]
    public class SerializedComponentDataGroup
    {
        public List<SerializedComponentDataMesh> Meshes;
        public List<SerializedComponentDataPoint> Points;
        public List<SerializedComponentDataCalle> Calles;

        public SerializedComponentDataGroup()
        {
            this.Meshes = new List<SerializedComponentDataMesh>();
            this.Points = new List<SerializedComponentDataPoint>();
            this.Calles = new List<SerializedComponentDataCalle>();
        }

        public void InsertTramo(string calle, Vector3 start, Vector3 end, List<string> data)
        {
            int found = 0;
            for (int i = 0; i < Calles.Count; i++)
            {
                if (Calles[i].Name == calle)
                {
                    Calles[i].Tramos.Add(new SerializedComponentDataCalleTramo(start, end));
                    found = 1;
                    break;
                }
            }
            if (found == 0)
            {
                SerializedComponentDataCalle call = new SerializedComponentDataCalle(calle, data);
                call.Tramos.Add(new SerializedComponentDataCalleTramo(start, end));
                Calles.Add(call);
            }
        }
    }

    [Serializable]
    public class SerializedComponentDataCalleTramo
    {
        public Vector3 Start;
        public Vector3 End;

        public SerializedComponentDataCalleTramo()
        {
            Start = Vector3.Zero;
            End = Vector3.Zero;
        }

        public SerializedComponentDataCalleTramo(Vector3 start, Vector3 end)
        {
            Start = start;
            End = end;
        }
    }

    [Serializable]
    public class SerializedComponentDataCalle
    {
        public string Name;
        public BoundingSphere BoundingSphere;
        public List<SerializedComponentDataCalleTramo> Tramos;
        public List<string> Data;

        public SerializedComponentDataCalle(string name, List<string> data)
        {
            Tramos = new List<SerializedComponentDataCalleTramo>();
            Name = name;
            Data = data;
        }
    }

    [Serializable]
    public class SerializedComponentDataPoint
    {
        public string Name;
        public string Label;
        public float Size;

        public List<string> Data;
        public int TextureIndex;
        public Vector3 Position;
    }

    [Serializable]
    public class SerializedComponentDataMesh
    {
        public string Name;
        public int TextureIndex;
        public Vector3 Position;
        public int VertexCount;
        public string VertexStream;
        public List<int> Indices;
        public List<string> Data;

        public SerializedComponentDataMesh()
        {
            this.Indices = new List<int>();
            this.Data = new List<string>();
        }
    }


    [Serializable]
    public class SerializedMeshPartDescriptor
    {
        public string Name;
        public MeshType Type;

        public string StreamVertices;
        public string StreamIndices;
        public int VerticesCount;
        public int IndicesCount;

        public SerializedMeshPartDescriptor(
            string streamVertices,
            string streamIndices,
            int verticesCount,
            int indicesCount,
            MeshType type)
        {
            StreamVertices = streamVertices;
            StreamIndices = streamIndices;
            VerticesCount = verticesCount;
            IndicesCount = indicesCount;
            Type = type;
        }

        public SerializedMeshPartDescriptor(
            string streamVertices,
            string streamIndices,
            int verticesCount,
            int indicesCount,
            MeshType type,
            string name)
        {
            StreamVertices = streamVertices;
            StreamIndices = streamIndices;
            VerticesCount = verticesCount;
            IndicesCount = indicesCount;
            Type = type;
            Name = name;
        }
    }


    [Serializable]
    public class SerializedMesh
    {
        public string Name;
        public SerializedComponentInfo Info;
        public Vector3 Position;
        public List<SerializedMeshPartDescriptor> Meshes;

        public SerializedMesh()
        {
            Meshes = new List<SerializedMeshPartDescriptor>();
        }

        public SerializedMesh(SerializedComponentInfo nfo)
        {
            Info = nfo;
            Meshes = new List<SerializedMeshPartDescriptor>();
        }

        //public List<SerializedMeshPart> Meshes;
    }

    [Serializable]
    public class SerializedMeshPart
    {
        public string Name;
        public int TextureIndex;
        public Vector3 Position;
        public int VertexCount;
        public string VertexStream;
        public List<int> Indices;
        public List<string> Data;

        public SerializedMeshPart()
        {
            this.Indices = new List<int>();
            this.Data = new List<string>();
        }
    }


    [Serializable]
    public class SerializedPoints
    {
        public string Name;
        public SerializedComponentInfo Info;

        public List<SerializedPoint> Points;

        public SerializedPoints()
        {
            Points = new List<SerializedPoint>();
        }

        public SerializedPoints(SerializedComponentInfo nfo)
        {
            Info = nfo;
            Points = new List<SerializedPoint>();
        }
    }


    [Serializable]
    public class SerializedPoint
    {
        public string Name;
        public int TextureIndex;
        public Vector3 Position;

        public List<string> Data;

        public short DisplayLower;
        public short DisplayHigher;

        public SerializedPoint()
        {
            Data = new List<string>();
        }
    }
}
// Decompiled by DJ v3.9.9.91 Copyright 2005 Atanas Neshkov  Date: 26/07/2009 01:41:26 a.m.
// Home Page : http://members.fortunecity.com/neshkov/dj.html  - Check often for new version!
// Decompiler options: packimports(3) 
// Source File Name:   Geo.java


public class Geo
{

    public Geo()
    {
    }

    public static double area2(Vertex vertex, Vertex vertex1, Vertex vertex2)
    {
        return (vertex1.x - vertex.x) * (vertex2.y - vertex.y) - (vertex2.x - vertex.x) * (vertex1.y - vertex.y);
    }

    public static boolean left(Vertex vertex, Vertex vertex1, Vertex vertex2)
    {
        return area2(vertex, vertex1, vertex2) > 0.0D;
    }

    public static boolean leftOn(Vertex vertex, Vertex vertex1, Vertex vertex2)
    {
        return area2(vertex, vertex1, vertex2) >= 0.0D;
    }

    public static boolean collinear(Vertex vertex, Vertex vertex1, Vertex vertex2)
    {
        return Math.abs(area2(vertex, vertex1, vertex2)) < 1.0D;
    }

    public static boolean intersectProp(Vertex vertex, Vertex vertex1, Vertex vertex2, Vertex vertex3)
    {
        if(collinear(vertex, vertex1, vertex2) || collinear(vertex, vertex1, vertex3) || collinear(vertex2, vertex3, vertex) || collinear(vertex2, vertex3, vertex1))
            return false;
        else
            return left(vertex, vertex1, vertex2) ^ left(vertex, vertex1, vertex3) && left(vertex2, vertex3, vertex) ^ left(vertex2, vertex3, vertex1);
    }

    public static boolean between(Vertex vertex, Vertex vertex1, Vertex vertex2)
    {
        if(!collinear(vertex, vertex1, vertex2))
            return false;
        if(vertex.x != vertex1.x)
            return vertex.x <= vertex2.x && vertex2.x <= vertex1.x || vertex.x >= vertex2.x && vertex2.x >= vertex1.x;
        else
            return vertex.y <= vertex2.y && vertex2.y <= vertex1.y || vertex.y >= vertex2.y && vertex2.y >= vertex1.y;
    }

    public static boolean betweenD(Vertex vertex, Vertex vertex1, Vertex vertex2)
    {
        if(vertex.x != vertex1.x)
            return vertex.x <= vertex2.x && vertex2.x <= vertex1.x || vertex.x >= vertex2.x && vertex2.x >= vertex1.x;
        else
            return vertex.y <= vertex2.y && vertex2.y <= vertex1.y || vertex.y >= vertex2.y && vertex2.y >= vertex1.y;
    }

    public static boolean intersect(Vertex vertex, Vertex vertex1, Vertex vertex2, Vertex vertex3)
    {
        if(intersectProp(vertex, vertex1, vertex2, vertex3))
            return true;
        return between(vertex, vertex1, vertex2) || between(vertex, vertex1, vertex3) || between(vertex2, vertex3, vertex) || between(vertex2, vertex3, vertex1);
    }

    public static final double EPSILON = 1D;
}
// Decompiled by DJ v3.9.9.91 Copyright 2005 Atanas Neshkov  Date: 26/07/2009 01:41:26 a.m.
// Home Page : http://members.fortunecity.com/neshkov/dj.html  - Check often for new version!
// Decompiler options: packimports(3) 
// Source File Name:   Edge.java

import java.awt.Graphics;

public class Edge
{

    public Edge(Vertex vertex, Vertex vertex1)
    {
        a = vertex;
        b = vertex1;
    }

    public boolean isEndPoint(Vertex vertex)
    {
        return vertex == a || vertex == b;
    }

    public boolean definedBy(Vertex vertex, Vertex vertex1)
    {
        return isEndPoint(vertex) && isEndPoint(vertex1) && vertex != vertex1;
    }

    public void draw(Graphics g)
    {
        g.drawLine((int)a.x, (int)a.y, (int)b.x, (int)b.y);
    }

    public Vertex a;
    public Vertex b;
}
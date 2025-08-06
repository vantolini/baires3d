// Decompiled by DJ v3.9.9.91 Copyright 2005 Atanas Neshkov  Date: 26/07/2009 01:41:27 a.m.
// Home Page : http://members.fortunecity.com/neshkov/dj.html  - Check often for new version!
// Decompiler options: packimports(3) 
// Source File Name:   Skeleton.java

import java.awt.Graphics;
import java.util.Vector;

public class Skeleton
{

    public Skeleton()
    {
        edges = new Vector();
    }

    public void addEdge(Edge edge)
    {
        edges.addElement(edge);
    }

    public Edge getEdge(int i)
    {
        return (Edge)edges.elementAt(i);
    }

    public int size()
    {
        return edges.size();
    }

    public void draw(Graphics g)
    {
        for(int i = 0; i < edges.size(); i++)
            ((Edge)edges.elementAt(i)).draw(g);

    }

    Vector edges;
}
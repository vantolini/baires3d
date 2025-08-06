// Decompiled by DJ v3.9.9.91 Copyright 2005 Atanas Neshkov  Date: 26/07/2009 01:41:27 a.m.
// Home Page : http://members.fortunecity.com/neshkov/dj.html  - Check often for new version!
// Decompiler options: packimports(3) 
// Source File Name:   PolygonSet.java

import java.awt.Color;
import java.awt.Graphics;
import java.util.Vector;

public class PolygonSet
{

    public PolygonSet()
    {
        polygons = new Vector();
    }

    public void addPoly(RealPolygon realpolygon)
    {
        polygons.addElement(realpolygon);
    }

    public RealPolygon getPoly(int i)
    {
        return (RealPolygon)polygons.elementAt(i);
    }

    public int size()
    {
        return polygons.size();
    }

    public void removePoly(RealPolygon realpolygon)
    {
        polygons.removeElement(realpolygon);
    }

    public TriangulationSet triangulate()
    {
        triangulations = new Vector();
        for(int i = 0; i < size(); i++)
            triangulations.addElement(((RealPolygon)polygons.elementAt(i)).triangulate());

        return new TriangulationSet(this, triangulations);
    }

    public RealPolygon findPolyWithTriangle(Triangle triangle)
    {
        for(int i = 0; i < triangulations.size(); i++)
            if(((Triangulation)triangulations.elementAt(i)).containsTriangle(triangle))
                return (RealPolygon)polygons.elementAt(i);

        return null;
    }

    public void drawPolygons(Graphics g)
    {
        for(int i = 0; i < polygons.size(); i++)
        {
            ((RealPolygon)polygons.elementAt(i)).drawPolygon(g);
            g.setColor(Color.cyan);
        }

    }

    Vector polygons;
    Vector triangulations;
}
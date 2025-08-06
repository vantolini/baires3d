// Decompiled by DJ v3.9.9.91 Copyright 2005 Atanas Neshkov  Date: 26/07/2009 01:41:26 a.m.
// Home Page : http://members.fortunecity.com/neshkov/dj.html  - Check often for new version!
// Decompiler options: packimports(3) 
// Source File Name:   AllTriangles.java

import java.io.PrintStream;
import java.util.Vector;

public class AllTriangles
{

    public AllTriangles(RealPolygon realpolygon)
    {
        poly = realpolygon;
        triangles = new Vector[realpolygon.getSize()];
        for(int i = 0; i < triangles.length; i++)
            triangles[i] = new Vector();

    }

    public void addTriangle(Vertex vertex, Triangle triangle)
    {
        System.out.println("Adding triangle " + triangle + " to vertex " + poly.getIndex(vertex));
        triangles[poly.getIndex(vertex)].addElement(triangle);
    }

    public String toString()
    {
        String s = "AllTriangles\n";
        for(int i = 0; i < triangles.length; i++)
        {
            s = s + "Vertex " + i + "\n";
            for(int j = 0; j < triangles[i].size(); j++)
                s = s + triangles[i].elementAt(j).toString() + "\n";

        }

        return s;
    }

    Vector triangles[];
    RealPolygon poly;
}
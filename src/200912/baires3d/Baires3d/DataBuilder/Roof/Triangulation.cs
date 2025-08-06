// Decompiled by DJ v3.9.9.91 Copyright 2005 Atanas Neshkov  Date: 26/07/2009 01:41:27 a.m.
// Home Page : http://members.fortunecity.com/neshkov/dj.html  - Check often for new version!
// Decompiler options: packimports(3) 
// Source File Name:   Triangulation.java

import java.awt.Color;
import java.awt.Graphics;
import java.io.PrintStream;
import java.util.Vector;

public class Triangulation
{

    public Triangulation(RealPolygon realpolygon, Vector vector, AllTriangles alltriangles, Vector vector1)
    {
        poly = realpolygon;
        diagonals = vector;
        triangles = alltriangles;
        uniqueTriangles = vector1;
        System.out.println(triangles);
        System.out.println("unique triangles");
        for(int i = 0; i < vector1.size(); i++)
            System.out.println(vector1.elementAt(i));

    }

    public Vector getTriangles()
    {
        return uniqueTriangles;
    }

    public boolean containsTriangle(Triangle triangle)
    {
        return uniqueTriangles.contains(triangle);
    }

    public Vector getDiagonals()
    {
        return diagonals;
    }

    public void removeTriangle(Triangle triangle)
    {
        uniqueTriangles.removeElement(triangle);
    }

    public void removeDiagonalsAt(Vertex vertex)
    {
        for(int i = 0; i < diagonals.size(); i++)
        {
            Edge edge = (Edge)diagonals.elementAt(i);
            if(edge.isEndPoint(vertex))
            {
                diagonals.removeElementAt(i);
                i--;
            }
        }

    }

    public void computeCollapsingTimes()
    {
        for(int i = 0; i < uniqueTriangles.size(); i++)
            ((Triangle)uniqueTriangles.elementAt(i)).computeCollapseTime();

    }

    public String toString()
    {
        String s = "unique triangles\n";
        for(int i = 0; i < uniqueTriangles.size(); i++)
            s = s + uniqueTriangles.elementAt(i).toString() + "\n";

        return s;
    }

    public void draw(Graphics g)
    {
        g.setColor(Color.green);
        for(int i = 0; i < diagonals.size(); i++)
            ((Edge)diagonals.elementAt(i)).draw(g);

    }

    RealPolygon poly;
    Vector diagonals;
    AllTriangles triangles;
    Vector uniqueTriangles;
}
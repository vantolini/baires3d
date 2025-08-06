// Decompiled by DJ v3.9.9.91 Copyright 2005 Atanas Neshkov  Date: 26/07/2009 01:41:27 a.m.
// Home Page : http://members.fortunecity.com/neshkov/dj.html  - Check often for new version!
// Decompiler options: packimports(3) 
// Source File Name:   TriangulationSet.java

import java.awt.Color;
import java.awt.Graphics;
import java.io.PrintStream;
import java.util.Vector;

public class TriangulationSet
{

    public TriangulationSet(PolygonSet polygonset, Vector vector)
    {
        polySet = polygonset;
        diagonals = new Vector();
        triangles = new Vector();
        for(int i = 0; i < vector.size(); i++)
        {
            System.out.println("\n\n======================================");
            System.out.println("Triangulation " + i);
            Triangulation triangulation = (Triangulation)vector.elementAt(i);
            Vector vector1 = triangulation.getTriangles();
            System.out.println("Number of triangles " + vector1.size());
            for(int j = 0; j < vector1.size(); j++)
                triangles.addElement(vector1.elementAt(j));

            vector1 = triangulation.getDiagonals();
            System.out.println("Number of diagonals " + vector1.size());
            for(int k = 0; k < vector1.size(); k++)
                diagonals.addElement(vector1.elementAt(k));

            System.out.println("==========================================\n\n");
        }

    }

    public Vector getDiagonals()
    {
        return diagonals;
    }

    public Vector getTriangles()
    {
        return triangles;
    }

    public void computeCollapsingTimes()
    {
        for(int i = 0; i < triangles.size(); i++)
            ((Triangle)triangles.elementAt(i)).computeCollapseTime();

    }

    public void draw(Graphics g)
    {
        g.setColor(Color.green);
        for(int i = 0; i < diagonals.size(); i++)
            ((Edge)diagonals.elementAt(i)).draw(g);

    }

    private PolygonSet polySet;
    Vector diagonals;
    Vector triangles;
}
// Decompiled by DJ v3.9.9.91 Copyright 2005 Atanas Neshkov  Date: 26/07/2009 01:41:27 a.m.
// Home Page : http://members.fortunecity.com/neshkov/dj.html  - Check often for new version!
// Decompiler options: packimports(3) 
// Source File Name:   Triangle.java

import java.awt.Component;
import java.io.PrintStream;
import java.util.Vector;

public class Triangle
    implements Comparable
{

    public Triangle(Vertex vertex, Vertex vertex1, Vertex vertex2)
    {
        a = vertex;
        b = vertex1;
        c = vertex2;
        collapseTime = 0.0D;
    }

    public boolean is2Merge()
    {
        return a.equals(b) || b.equals(c) || c.equals(a);
    }

    public Vertex getMerge1()
    {
        if(!is2Merge())
            return null;
        if(a.equals(b))
            return a;
        if(b.equals(c))
            return b;
        else
            return c;
    }

    public Vertex getMerge2()
    {
        if(!is2Merge())
            return null;
        if(a.equals(b))
            return b;
        if(b.equals(c))
            return c;
        else
            return a;
    }

    public Vertex getNotMerge()
    {
        if(a.equals(b))
            return c;
        if(a.equals(c))
            return b;
        else
            return a;
    }

    public boolean isFlipEvent(Vector vector)
    {
        System.out.println(">>>>>>>> in isFlipEvent");
        if(isEdgeEvent(vector))
            return false;
        System.out.println(">>>>>>> Testing between and diagonal");
        return Geo.betweenD(a, b, c) && isDiagonal(vector, a, b) || Geo.betweenD(a, c, b) && isDiagonal(vector, a, c) || Geo.betweenD(b, c, a) && isDiagonal(vector, b, c);
    }

    public boolean isSplitEvent(Vector vector)
    {
        System.out.println(">>>>>>>> in isSplitEvent");
        if(isEdgeEvent(vector))
            return false;
        System.out.println(">>>>>>> Testing between and not diagonal");
        return Geo.betweenD(a, b, c) && !isDiagonal(vector, a, b) || Geo.betweenD(a, c, b) && !isDiagonal(vector, a, c) || Geo.betweenD(b, c, a) && !isDiagonal(vector, b, c);
    }

    private boolean isDiagonal(Vector vector, Vertex vertex, Vertex vertex1)
    {
        System.out.println(">>>>>>>>>> In isDiagonal");
        for(int i = 0; i < vector.size(); i++)
        {
            Edge edge = (Edge)vector.elementAt(i);
            if(edge.definedBy(vertex, vertex1))
                return true;
        }

        return false;
    }

    public boolean isEdgeEvent(Vector vector)
    {
        System.out.println(">>>>>>>>>>>> in isEdgeEvent");
        return is2Merge();
    }

    public boolean lessThan(Comparable comparable)
    {
        return collapseTime < ((Triangle)comparable).collapseTime;
    }

    public double getTime()
    {
        return collapseTime;
    }

    public void setTime(double d)
    {
        collapseTime = d;
    }

    public void setEvent(PolygonEvent polygonevent)
    {
        e = polygonevent;
    }

    public PolygonEvent getEvent()
    {
        return e;
    }

    public boolean triangleVertex(Vertex vertex)
    {
        return vertex == a || vertex == b || vertex == c;
    }

    public Vertex getVertex(int i)
    {
        if(i == 0)
            return a;
        if(i == 1)
            return b;
        if(i == 2)
            return c;
        else
            return null;
    }

    public void computeCollapseTime()
    {
        Vertex vertex1;
        Vertex vertex2;
        Vertex vertex = vertex1 = vertex2 = null;
        try
        {
            vertex = (Vertex)a.clone();
            vertex1 = (Vertex)b.clone();
            vertex2 = (Vertex)c.clone();
            collapseTriangle = new RealPolygon();
            collapseTriangle.addVertex(vertex);
            collapseTriangle.addVertex(vertex1);
            collapseTriangle.addVertex(vertex2);
        }
        catch(CloneNotSupportedException clonenotsupportedexception)
        {
            System.out.println(clonenotsupportedexception);
            System.exit(1);
        }
        double d = 0.001D;
        double d1 = 0.0D;
        double d2 = d * vertex.speed * Math.cos(vertex.angle);
        double d3 = d * vertex.speed * Math.sin(vertex.angle);
        double d4 = d * vertex1.speed * Math.cos(vertex1.angle);
        double d5 = d * vertex1.speed * Math.sin(vertex1.angle);
        double d6 = d * vertex2.speed * Math.cos(vertex2.angle);
        double d7 = d * vertex2.speed * Math.sin(vertex2.angle);
        long l = System.currentTimeMillis();
        while(!Geo.collinear(vertex, vertex1, vertex2) && d1 < 1000D) 
        {
            vertex.x = vertex.x + d2;
            vertex.y = vertex.y - d3;
            vertex1.x = vertex1.x + d4;
            vertex1.y = vertex1.y - d5;
            vertex2.x = vertex2.x + d6;
            vertex2.y = vertex2.y - d7;
            d1 += d;
            if(sCanvas.showTriangle)
            {
                long l1 = l + 100L;
                long l2 = System.currentTimeMillis();
                if(l2 > l1)
                {
                    l = l2;
                    sCanvas.update(sCanvas.getGraphics());
                }
            }
        }
        collapseTime = d1;
        collapseTriangle = null;
    }

    public String toString()
    {
        String s = "[" + a + b + c + ", collapseTime = " + collapseTime + "]";
        return s;
    }

    Vertex a;
    Vertex b;
    Vertex c;
    PolygonEvent e;
    double collapseTime;
    public static RealPolygon collapseTriangle = null;
    public static SkeletonCanvas sCanvas = null;

}
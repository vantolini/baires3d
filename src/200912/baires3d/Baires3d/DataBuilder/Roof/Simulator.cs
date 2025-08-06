// Decompiled by DJ v3.9.9.91 Copyright 2005 Atanas Neshkov  Date: 26/07/2009 01:41:27 a.m.
// Home Page : http://members.fortunecity.com/neshkov/dj.html  - Check often for new version!
// Decompiler options: packimports(3) 
// Source File Name:   Simulator.java

import java.awt.Component;
import java.io.PrintStream;
import java.text.DecimalFormat;
import java.text.NumberFormat;
import java.util.Vector;

public class Simulator
{

    public Simulator(Roofs roofs, SkeletonCanvas skeletoncanvas, PolygonSet polygonset)
    {
        init = false;
        firstTime = true;
        roofApplet = roofs;
        sCanvas = skeletoncanvas;
        polySet = polygonset;
        RealPolygon realpolygon = polygonset.getPoly(0);
        RealPolygon realpolygon1 = (RealPolygon)realpolygon.clone();
        skeleton = new Skeleton();
        skeletoncanvas.setSkeleton(skeleton);
        for(int i = 0; i < realpolygon.getSize(); i++)
            skeleton.addEdge(new Edge(realpolygon1.getVertex(i), realpolygon.getVertex(i)));

        time = 0.0D;
        offsetTime = 0.0D;
        if(!realpolygon.isCounterClockwise())
        {
            realpolygon.switchDirection();
            System.out.println("Direction of poly switched!");
        }
        decFormat = new DecimalFormat("#######.#");
    }

    public void run()
    {
        run(false);
    }

    public void run(boolean flag)
    {
        int i = 0;
        if(!init)
        {
            init = true;
            initialization();
        }
        boolean flag1 = true;
        while(flag1) 
        {
            if(flag)
                flag1 = false;
            if(heap.size() == 0)
            {
                roofApplet.printMessage("Stopping at time t = " + decFormat.format(time));
                sCanvas.hideBisectors();
                sCanvas.hideDiagonals();
                sCanvas.removeHighLight();
                roofApplet.running = false;
                break;
            }
            Triangle triangle = (Triangle)heap.removeFirst();
            double d;
            for(d = triangle.getTime(); time < d; time += 0.25D)
            {
                updatePolySet();
                if(++i > 5)
                {
                    i = 0;
                    sCanvas.update(sCanvas.getGraphics());
                }
                long l = System.currentTimeMillis();
                long l1 = l + 10L;
                for(long l2 = l; l2 < l1; l2 = System.currentTimeMillis());
            }

            time = d;
            roofApplet.printMessage("");
            roofApplet.printMessage("Triangle collapse at time t = " + decFormat.format(d));
            System.out.println("************ Now testing events...");
            if(triangle.isFlipEvent(triangulationSet.diagonals))
                processFlipEvent(triangle);
            else
            if(triangle.isEdgeEvent(triangulationSet.diagonals))
                processEdgeEvent(triangle);
            else
            if(triangle.isSplitEvent(triangulationSet.diagonals))
                processSplitEvent(triangle);
            else
                roofApplet.printMessage("Unknown event.  Bug in applet.\nContact dbelan2@po-box.mcgill.ca\nSend list of points used.\nThanks!\n\n");
        }
    }

    private void processEdgeEvent(Triangle triangle)
    {
        RealPolygon realpolygon = polySet.findPolyWithTriangle(triangle);
        roofApplet.printMessage("Edge Event at time t = " + decFormat.format(time));
        Vertex vertex = triangle.getMerge1();
        Vertex vertex1 = triangle.getMerge2();
        realpolygon.removeVertex(vertex);
        Vertex vertex2 = null;
        try
        {
            vertex2 = (Vertex)vertex1.clone();
        }
        catch(CloneNotSupportedException clonenotsupportedexception)
        {
            System.out.println(clonenotsupportedexception);
        }
        if(realpolygon.getSize() == 0)
        {
            polySet.removePoly(realpolygon);
            System.out.println("============================== Poly removed");
        } else
        {
            realpolygon.replaceVertex(vertex1, vertex2);
            double ad[] = computeBisector(realpolygon.getPrevVertex(vertex2), vertex2, realpolygon.getNextVertex(vertex2));
            vertex2.angle = ad[0];
            vertex2.speed = ad[1];
            skeleton.addEdge(new Edge(vertex1, vertex2));
            if(heap.size() == 0)
                skeleton.addEdge(new Edge(vertex2, triangle.getNotMerge()));
            sCanvas.highLight(vertex1);
        }
        recomputeTimes();
    }

    public void recomputeTimes()
    {
        roofApplet.printMessage("Updating triangles, speeds and collapsing times...\nPlease wait...");
        initialization();
        roofApplet.printMessage("Done.\nReady!");
        System.out.println("Nb. items on heap " + heap.size());
    }

    private void processFlipEvent(Triangle triangle)
    {
        roofApplet.printMessage("Flip Event at time t = " + decFormat.format(time));
        recomputeTimes();
    }

    private void processSplitEvent(Triangle triangle)
    {
        roofApplet.printMessage("Split Event at time t = " + decFormat.format(time));
        System.out.println("In split event -----------------------------");
        RealPolygon realpolygon = polySet.findPolyWithTriangle(triangle);
        RealPolygon realpolygon1 = new RealPolygon();
        RealPolygon realpolygon2 = new RealPolygon();
        Vertex vertex = triangle.a;
        Vertex vertex1 = triangle.b;
        Vertex vertex2 = triangle.c;
        Vertex vertex3;
        Vertex vertex4;
        Vertex vertex5;
        if(Geo.betweenD(vertex, vertex1, vertex2))
        {
            vertex3 = vertex2;
            vertex4 = vertex;
            vertex5 = vertex1;
        } else
        if(Geo.betweenD(vertex, vertex2, vertex1))
        {
            vertex3 = vertex1;
            vertex4 = vertex;
            vertex5 = vertex2;
        } else
        {
            vertex3 = vertex;
            vertex4 = vertex1;
            vertex5 = vertex2;
        }
        Vertex vertex8 = vertex3;
        do
        {
            vertex8 = realpolygon.getPrevVertex(vertex8);
            realpolygon1.addVertex(vertex8);
        } while(vertex8 != vertex4 && vertex8 != vertex5);
        Vertex vertex7 = null;
        try
        {
            vertex7 = (Vertex)vertex3.clone();
        }
        catch(CloneNotSupportedException clonenotsupportedexception)
        {
            System.out.println(clonenotsupportedexception);
        }
        realpolygon1.addVertex(vertex7);
        vertex8 = vertex3;
        do
        {
            vertex8 = realpolygon.getNextVertex(vertex8);
            realpolygon2.addVertex(vertex8);
        } while(vertex8 != vertex4 && vertex8 != vertex5);
        Vertex vertex6 = null;
        try
        {
            vertex6 = (Vertex)vertex3.clone();
        }
        catch(CloneNotSupportedException clonenotsupportedexception1)
        {
            System.out.println(clonenotsupportedexception1);
        }
        realpolygon2.addVertex(vertex6);
        skeleton.addEdge(new Edge(vertex3, vertex6));
        skeleton.addEdge(new Edge(vertex3, vertex7));
        sCanvas.hideDiagonals();
        if(!realpolygon1.isCounterClockwise())
        {
            realpolygon1.switchDirection();
            System.out.println("Direction of poly switched!");
        }
        if(!realpolygon2.isCounterClockwise())
        {
            realpolygon2.switchDirection();
            System.out.println("Direction of poly switched!");
        }
        polySet.removePoly(realpolygon);
        polySet.addPoly(realpolygon1);
        polySet.addPoly(realpolygon2);
        System.out.println("\n\n----- Poly divided");
        System.out.println("poly  : " + realpolygon);
        System.out.println("poly1 : " + realpolygon1);
        System.out.println("poly2 : " + realpolygon2);
        System.out.println("\nv is : " + vertex3 + "\n\n");
        sCanvas.highLight(vertex3);
        recomputeTimes();
    }

    private void initialization()
    {
        heap = new Heap();
        if(firstTime)
            roofApplet.printMessage("Triangulating...");
        if(firstTime)
            sCanvas.showTriangle();
        else
            sCanvas.hideTriangle();
        triangulationSet = polySet.triangulate();
        sCanvas.setTriangulationSet(triangulationSet);
        sCanvas.showDiagonals();
        if(firstTime)
            roofApplet.printMessage("Computing bisectors...");
        computeAngles();
        sCanvas.showBisectors();
        if(firstTime)
            roofApplet.printMessage("Computing collapsing times...");
        Triangle.sCanvas = sCanvas;
        triangulationSet.computeCollapsingTimes();
        sCanvas.hideTriangle();
        insertTrianglesInHeap(triangulationSet);
        firstTime = false;
    }

    private void insertTrianglesInHeap(TriangulationSet triangulationset)
    {
        heap = new Heap();
        Vector vector = triangulationset.getTriangles();
        for(int i = 0; i < vector.size(); i++)
        {
            Triangle triangle = (Triangle)vector.elementAt(i);
            triangle.collapseTime = triangle.collapseTime + time;
            heap.insert(triangle);
        }

    }

    public void reset()
    {
    }

    private void computeAngles()
    {
        for(int j = 0; j < polySet.size(); j++)
        {
            RealPolygon realpolygon = polySet.getPoly(j);
            int i = realpolygon.getSize();
            for(int k = 0; k < i; k++)
            {
                Vertex vertex = realpolygon.getVertex(((k - 1) + i) % i);
                Vertex vertex1 = realpolygon.getVertex(k);
                Vertex vertex2 = realpolygon.getVertex((k + 1) % i);
                double ad[] = computeBisector(vertex, vertex1, vertex2);
                vertex1.angle = ad[0];
                vertex1.speed = ad[1];
            }

        }

    }

    public double[] computeBisector(Vertex vertex, Vertex vertex1, Vertex vertex2)
    {
        double d = vertex2.distance(vertex1);
        double d2 = vertex.distance(vertex1);
        double d1 = vertex.distance(vertex2);
        double d3 = Math.acos(((d * d + d2 * d2) - d1 * d1) / (2D * d * d2));
        if(!Geo.left(vertex, vertex1, vertex2))
        {
            d3 = 6.2831853071795862D - d3;
            System.out.println("left");
        }
        double d5 = vertex2.y - vertex1.y;
        double d4 = vertex2.x - vertex1.x;
        double d8 = Math.atan(d5 / d4);
        if(d4 >= 0.0D && d5 >= 0.0D)
            d8 = 6.2831853071795862D - d8;
        else
        if(d4 >= 0.0D && d5 <= 0.0D)
            d8 = -d8;
        else
        if(d4 <= 0.0D && d5 >= 0.0D)
            d8 = -d8 + 3.1415926535897931D;
        else
        if(d4 <= 0.0D && d5 <= 0.0D)
            d8 = 3.1415926535897931D - d8;
        else
            System.out.println("Simulator computeAngle dx or dy == 0");
        double d7 = vertex.y - vertex1.y;
        double d6 = vertex.x - vertex1.x;
        double d9 = Math.atan(d7 / d6);
        if(d6 >= 0.0D && d7 >= 0.0D)
            d9 = 6.2831853071795862D - d9;
        else
        if(d6 >= 0.0D && d7 <= 0.0D)
            d9 = -d9;
        else
        if(d6 <= 0.0D && d7 >= 0.0D)
            d9 = -d9 + 3.1415926535897931D;
        else
        if(d6 <= 0.0D && d7 <= 0.0D)
            d9 = 3.1415926535897931D - d9;
        else
            System.out.println("Simulator computeAngle dx or dy == 0");
        double d10 = d8 - d3 / 2D;
        double d11 = 1.0D / Math.cos(1.5707963267948966D - d3 / 2D);
        double ad[] = new double[2];
        ad[0] = d10;
        ad[1] = d11;
        System.out.println(">>>>>>>>>>>>>>>>>>>>>>>>> speed " + d11);
        return ad;
    }

    private double radToDeg(double d)
    {
        return (d * 180D) / 3.1415926535897931D;
    }

    private void updatePolySet()
    {
        for(int i = 0; i < polySet.size(); i++)
            updatePoly(polySet.getPoly(i));

    }

    private void updatePoly(RealPolygon realpolygon)
    {
        for(int i = 0; i < realpolygon.getSize(); i++)
        {
            Vertex vertex = realpolygon.getVertex(i);
            vertex.x = vertex.x + 0.25D * vertex.speed * Math.cos(vertex.angle);
            vertex.y = vertex.y - 0.25D * vertex.speed * Math.sin(vertex.angle);
            sCanvas.repaint();
        }

    }

    private int sign(double d)
    {
        if(d > 0.0D)
            return 1;
        return d >= 0.0D ? 0 : -1;
    }

    boolean init;
    double time;
    double offsetTime;
    public final double DELTA_T = 0.25D;
    public final double SPEED = 1.0D;
    public final int SCREEN_UPDATE = 5;
    public final int FRAME_TIME = 10;
    Roofs roofApplet;
    SkeletonCanvas sCanvas;
    PolygonSet polySet;
    TriangulationSet triangulationSet;
    Skeleton skeleton;
    Heap heap;
    DecimalFormat decFormat;
    boolean firstTime;
}
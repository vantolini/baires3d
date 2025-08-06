// Decompiled by DJ v3.9.9.91 Copyright 2005 Atanas Neshkov  Date: 26/07/2009 01:41:27 a.m.
// Home Page : http://members.fortunecity.com/neshkov/dj.html  - Check often for new version!
// Decompiler options: packimports(3) 
// Source File Name:   SkeletonCanvas.java


using System;

namespace baires3d {

public class SkeletonCanvas {

    public SkeletonCanvas(Roofs roofs, int i, int j)
    {
        done = false;
        nextPoint = null;
        realPoly = null;
        shrinkPoly = null;
        triangulationSet = null;
        skeleton = null;
        highLight = null;
        repaintCounter = 0;
        showBisectors = false;
        showTriangle = false;
        showDiagonals = false;
        about = false;
        roofApplet = roofs;
        setSize(i, j);
        setBackground(new Color(Roofs.bgColor));
        realPoly = new RealPolygon();
        addMouseListener(this);
        addMouseMotionListener(this);
    }

    public bool isDone()
    {
        return done;
    }

    public RealPolygon getInputPoly()
    {
        return realPoly;
    }

    public PolygonSet createShrinkPoly()
    {
        shrinkPoly = new PolygonSet();
        shrinkPoly.addPoly((RealPolygon)realPoly.clone());
        return shrinkPoly;
    }

    public void setTriangulationSet(TriangulationSet triangulationset)
    {
        triangulationSet = triangulationset;
    }

    public void setSkeleton(Skeleton skeleton1)
    {
        skeleton = skeleton1;
    }

    public void hideBisectors()
    {
        showBisectors = false;
        repaint();
    }

    public void showBisectors()
    {
        showBisectors = true;
        repaint();
    }

    public void hideTriangle()
    {
        showTriangle = false;
        repaint();
    }

    public void showTriangle()
    {
        showTriangle = true;
        repaint();
    }

    public void hideDiagonals()
    {
        showDiagonals = false;
        repaint();
    }

    public void showDiagonals()
    {
        showDiagonals = true;
        repaint();
    }

    public void highLight(Vertex vertex)
    {
        highLight = vertex;
        repaint();
    }

    public void removeHighLight()
    {
        highLight = null;
        repaint();
    }

    public void paint(Graphics g)
    {
        Dimension dimension = getSize();
        g.setColor(Color.black);
        g.draw3DRect(1, 1, dimension.width - 5, dimension.height - 5, true);
        g.drawString("Drawing area", 10, 20);
        if(Triangle.collapseTriangle != null && showTriangle)
        {
            g.setColor(Color.orange);
            Triangle.collapseTriangle.drawPolygon(g);
        }
        g.setColor(Color.black);
        if(done)
            realPoly.drawPolygon(g);
        else
        if(realPoly.getSize() > 0)
        {
            realPoly.drawPolyline(g);
            g.setColor(Color.red);
            Point point = realPoly.getPoint(0);
            int j = (int)Math.round(Math.sqrt(64D));
            g.fillOval(point.x - j / 2, point.y - j / 2, j, j);
            Point point1 = realPoly.getPoint(realPoly.getSize() - 1);
            g.setColor(Color.green);
            g.drawLine(point1.x, point1.y, nextPoint.x, nextPoint.y);
        }
        if(shrinkPoly != null && showBisectors)
        {
            g.setColor(Color.blue);
            shrinkPoly.drawPolygons(g);
            for(int i = 0; i < shrinkPoly.size(); i++)
            {
                RealPolygon realpolygon = shrinkPoly.getPoly(i);
                for(int k = 0; k < realpolygon.getSize(); k++)
                {
                    Vertex vertex1 = realpolygon.getVertex(k);
                    g.setColor(Color.red);
                    int l = (int)(30D * Math.Cos(vertex1.angle));
                    int i1 = (int)(30D * Math.Sin(vertex1.angle));
                    if(l > 40 || i1 > 40)
                        break;
                    int j1 = (int)(vertex1.x + (double)l);
                    int k1 = (int)(vertex1.y - (double)i1);
                    g.drawLine((int)vertex1.x, (int)vertex1.y, j1, k1);
                }

            }

        }
        if(triangulationSet != null && showDiagonals)
        {
            g.setColor(Color.green);
            triangulationSet.draw(g);
        }
        if(skeleton != null)
        {
            g.setColor(Color.magenta);
            skeleton.draw(g);
        }
        if(highLight != null)
        {
            byte byte0 = 8;
            Vertex vertex = highLight;
            g.setColor(Color.pink);
            g.fillOval((int)vertex.x - byte0 / 2, (int)vertex.y - byte0 / 2, byte0, byte0);
        }
    }

    public void reset()
    {
        realPoly = new RealPolygon();
        shrinkPoly = null;
        triangulationSet = null;
        skeleton = null;
        highLight = null;
        done = false;
        repaint();
    }

    public void mouseClicked(MouseEvent mouseevent)
    {
        if(done)
            return;
        Point point = nextPoint;
        if(realPoly.getSize() > 0)
            roofApplet.printStatus2("Click on the red vertex to close the polygon.");
        if(realPoly.getSize() > 1 && distanceSquared(point, realPoly.getPoint(0)) < 64)
        {
            done = true;
            roofApplet.printStatus1("Polygon is closed.  Now click on RUN or on STEP.");
            roofApplet.printStatus2("");
            System.out.println("Done : " + realPoly);
        } else
        {
            realPoly.addPoint(point);
            roofApplet.printMessage("Point (" + point.x + ", " + point.y + ") added.");
        }
        repaint();
    }

    private int distanceSquared(Point point, Point point1)
    {
        int i = point1.x - point.x;
        int j = point1.y - point.y;
        int k = i * i + j * j;
        if(k < 0)
            k = -k;
        return k;
    }

    public void mouseEntered(MouseEvent mouseevent)
    {
    }

    public void mouseExited(MouseEvent mouseevent)
    {
    }

    public void mousePressed(MouseEvent mouseevent)
    {
    }

    public void mouseReleased(MouseEvent mouseevent)
    {
    }

    public void mouseDragged(MouseEvent mouseevent)
    {
    }

    public void mouseMoved(MouseEvent mouseevent)
    {
        nextPoint = mouseevent.getPoint();
        repaintCounter = (repaintCounter + 1) % 5;
        if(repaintCounter == 0)
            repaint();
    }

    public final int DELTA = 64;
    public final int HIGHLIGHT_DIAMETER = 8;
    bool done;
    Roofs roofApplet;
    Point nextPoint;
    RealPolygon realPoly;
    PolygonSet shrinkPoly;
    TriangulationSet triangulationSet;
    Skeleton skeleton;
    Vertex highLight;
    int repaintCounter;
    bool showBisectors;
    bool showTriangle;
    bool showDiagonals;
    public bool about;
}}
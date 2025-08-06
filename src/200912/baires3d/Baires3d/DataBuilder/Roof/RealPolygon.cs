
using System;
using Microsoft.Xna.Framework;

namespace baires3d {
public class RealPolygon
{
    Vector3[] vertices;
    public RealPolygon()
    {
    }

    public RealPolygon(Vector3[] vertices)
    {
        this.vertices = vertices;
        /*
        for(int i = 0; i < polygon.npoints; i++)
        {
            System.out.println("i = " + i + " " + polygon.xpoints[i] + "," + polygon.ypoints[i]);
            vertices.addElement(new Vertex(polygon.xpoints[i], polygon.ypoints[i], 0.0D));
        }*/

    }

    public void addVertex(RoofVertex vertex)
    {
        vertices.addElement(vertex);
    }

    public void removeVertex(int i)
    {
        vertices.removeElementAt(i);
    }

    public void removeVertex(RoofVertex vertex)
    {
        vertices.removeElement(vertex);
    }

    public void addPoint(Point point)
    {
        addVertex(new Vertex(point.x, point.y));
    }

    public Vertex getVertex(int i)
    {
        int j = getSize();
        return (Vertex)vertices.elementAt((i + j) % j);
    }

    public int getIndex(Vertex vertex)
    {
        for(int i = 0; i < getSize(); i++)
            if(vertex == (Vertex)vertices.elementAt(i))
                return i;

        return -1;
    }

    public Vertex getPrevVertex(Vertex vertex)
    {
        return getVertex(getIndex(vertex) - 1);
    }

    public Vertex getNextVertex(Vertex vertex)
    {
        return getVertex(getIndex(vertex) + 1);
    }

    public void setVertex(Vertex vertex, int i)
    {
        int j = getSize();
        if(j == 0)
        {
            return;
        } else
        {
            vertices.setElementAt(vertex, (i + j) % j);
            return;
        }
    }

    public void replaceVertex(Vertex vertex, Vertex vertex1)
    {
        setVertex(vertex1, getIndex(vertex));
    }

    public Point getPoint(int i)
    {
        Vertex vertex = getVertex(i);
        int j = (int)Math.round(vertex.x);
        int k = (int)Math.round(vertex.y);
        return new Point(j, k);
    }

    public int getSize()
    {
        return vertices.size();
    }

    public boolean isCounterClockwise()
    {
        int i = findLowestRightmost();
        return Geo.left(getVertex(i - 1), getVertex(i), getVertex(i + 1));
    }

    private int findLowestRightmost()
    {
        int i = 0;
        for(int j = 1; j < getSize(); j++)
        {
            Vertex vertex = getVertex(i);
            Vertex vertex1 = getVertex(j);
            if(vertex1.y < vertex.y)
                i = j;
            else
            if(vertex1.y == vertex.y && vertex1.x > vertex.x)
                i = j;
        }

        return i;
    }

    public void switchDirection()
    {
        int i = getSize();
        for(int j = 1; j <= (i - 1) / 2; j++)
        {
            Object obj = vertices.elementAt(j);
            vertices.setElementAt(vertices.elementAt(i - j), j);
            vertices.setElementAt(obj, i - j);
        }

    }

    public Object clone()
    {
        RealPolygon realpolygon = new RealPolygon();
        for(int i = 0; i < getSize(); i++)
            try
            {
                realpolygon.vertices.addElement(getVertex(i).clone());
            }
            catch(CloneNotSupportedException clonenotsupportedexception)
            {
                System.out.println(clonenotsupportedexception);
                System.exit(1);
            }

        return realpolygon;
    }

    public String toString()
    {
        String s = "RealPolygon [ ";
        for(int i = 0; i < vertices.size(); i++)
            s = s + ((Vertex)vertices.elementAt(i)).toString();

        s = s + " ]";
        return s;
    }

    private boolean diagonalie(Vertex vertex, Vertex vertex1)
    {
        int j = 0;
        int i = getSize();
        Vertex vertex4 = getVertex(0);
        Vertex vertex2 = vertex4;
        do
        {
            System.out.println("d");
            System.out.println("i = " + j);
            j = (j + 1) % i;
            Vertex vertex3 = getVertex(j);
            if(vertex2 != vertex && vertex3 != vertex && vertex2 != vertex1 && vertex3 != vertex1 && Geo.intersect(vertex, vertex1, vertex2, vertex3))
                return false;
            vertex2 = vertex3;
            System.out.println("c = " + vertex2 + ", first = " + vertex4);
        } while(vertex2 != vertex4);
        return true;
    }

    private boolean inCone(int i, int j)
    {
        int k = getSize();
        Vertex vertex = getVertex(i);
        Vertex vertex1 = getVertex(j);
        Vertex vertex2 = getVertex((i + 1) % k);
        Vertex vertex3 = getVertex(((i - 1) + k) % k);
        if(Geo.leftOn(vertex, vertex2, vertex3))
            return Geo.left(vertex, vertex1, vertex3) && Geo.left(vertex1, vertex, vertex2);
        else
            return !Geo.leftOn(vertex, vertex1, vertex2) || !Geo.leftOn(vertex1, vertex, vertex3);
    }

    private boolean diagonal(int i, int j)
    {
        return inCone(i, j) && inCone(j, i) && diagonalie(getVertex(i), getVertex(j));
    }

    private void earInit()
    {
        System.out.println("in earInit");
        int i = getSize();
        for(int j = 0; j < i; j++)
            getVertex(j).ear = diagonal(((j - 1) + i) % i, (j + 1) % i);

    }

    public Triangulation triangulate()
    {
        RealPolygon realpolygon = new RealPolygon();
        realpolygon.vertices = (Vector)vertices.clone();
        AllTriangles alltriangles = new AllTriangles(realpolygon);
        return realpolygon.triangulateHelper(alltriangles);
    }

    private Triangulation triangulateHelper(AllTriangles alltriangles)
    {
        Vector vector = new Vector();
        Vector vector1 = new Vector();
        earInit();
        System.out.println("Triangulating");
        double d = System.currentTimeMillis();
        double d1 = d + 5000D;
        int j1;
        for(double d2 = d; (j1 = getSize()) > 3 && d2 < d1;)
        {
            d2 = System.currentTimeMillis();
            System.out.println(".");
            int k = 0;
            do
            {
                if(getVertex(k).ear)
                {
                    int l = (k + 1) % j1;
                    int i1 = (l + 1) % j1;
                    int j = ((k - 1) + j1) % j1;
                    int i = ((j - 1) + j1) % j1;
                    System.out.println("Diagonal " + j + " " + l);
                    Vertex vertex = getVertex(j);
                    Vertex vertex2 = getVertex(k);
                    Vertex vertex4 = getVertex(l);
                    vector.addElement(new Edge(vertex, vertex4));
                    Triangle triangle = new Triangle(vertex, vertex2, vertex4);
                    alltriangles.addTriangle(vertex, triangle);
                    alltriangles.addTriangle(vertex2, triangle);
                    alltriangles.addTriangle(vertex4, triangle);
                    vector1.addElement(triangle);
                    getVertex(j).ear = diagonal(i, l);
                    getVertex(l).ear = diagonal(j, i1);
                    removeVertex(k);
                    System.out.println("new poly = " + this);
                    break;
                }
                k = (k + 1) % j1;
            } while(k != 0);
        }

        Vertex vertex1 = getVertex(0);
        Vertex vertex3 = getVertex(1);
        Vertex vertex5 = getVertex(2);
        System.out.println("poly left " + this);
        System.out.println("left " + vertex1 + vertex3 + vertex5);
        Triangle triangle1 = new Triangle(vertex1, vertex3, vertex5);
        alltriangles.addTriangle(vertex1, triangle1);
        alltriangles.addTriangle(vertex3, triangle1);
        alltriangles.addTriangle(vertex5, triangle1);
        vector1.addElement(triangle1);
        return new Triangulation(this, vector, alltriangles, vector1);
    }

    private void draw(Graphics g, boolean flag)
    {
        int i = getSize();
        byte byte0;
        if(!flag)
            byte0 = -1;
        else
            byte0 = 0;
        if(i == 0)
            return;
        for(int j = 0; j < i + byte0; j++)
        {
            Vertex vertex = getVertex(j);
            Vertex vertex1 = getVertex((j + 1) % i);
            g.drawLine((int)vertex.x, (int)vertex.y, (int)vertex1.x, (int)vertex1.y);
        }

    }

    public void drawPolygon(Graphics g)
    {
        draw(g, true);
    }

    public void drawPolyline(Graphics g)
    {
        draw(g, false);
    }

    
}
}
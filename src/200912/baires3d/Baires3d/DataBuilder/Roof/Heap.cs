// Decompiled by DJ v3.9.9.91 Copyright 2005 Atanas Neshkov  Date: 26/07/2009 01:41:27 a.m.
// Home Page : http://members.fortunecity.com/neshkov/dj.html  - Check often for new version!
// Decompiler options: packimports(3) 
// Source File Name:   Heap.java

import java.util.Vector;

class Heap
    implements OrderedSet
{

    Heap()
    {
        elements = new Vector();
        heapSize = 0;
    }

    int parent(int i)
    {
        return (i - 1) / 2;
    }

    int left(int i)
    {
        return 2 * i + 1;
    }

    int right(int i)
    {
        return 2 * i + 2;
    }

    void heapify(int i)
    {
        int k = left(i);
        int l = right(i);
        int j;
        if(k < heapSize && ((Comparable)elements.elementAt(k)).lessThan((Comparable)elements.elementAt(i)))
            j = k;
        else
            j = i;
        if(l < heapSize && ((Comparable)elements.elementAt(l)).lessThan((Comparable)elements.elementAt(j)))
            j = l;
        if(j != i)
        {
            Object obj = elements.elementAt(i);
            elements.setElementAt(elements.elementAt(j), i);
            elements.setElementAt(obj, j);
            heapify(j);
        }
    }

    public void insert(Comparable comparable)
    {
        int i = heapSize;
        heapSize++;
        for(; i > 0 && comparable.lessThan((Comparable)elements.elementAt(parent(i))); i = parent(i))
            if(i == heapSize - 1)
                elements.addElement(elements.elementAt(parent(i)));
            else
                elements.setElementAt(elements.elementAt(parent(i)), i);

        if(i == heapSize - 1)
            elements.addElement(comparable);
        else
            elements.setElementAt(comparable, i);
    }

    public Comparable removeFirst()
    {
        if(heapSize == 0)
        {
            return null;
        } else
        {
            Comparable comparable = (Comparable)elements.elementAt(0);
            elements.setElementAt(elements.elementAt(heapSize - 1), 0);
            elements.removeElementAt(heapSize - 1);
            heapSize--;
            heapify(0);
            return comparable;
        }
    }

    public int size()
    {
        return heapSize;
    }

    Vector elements;
    int heapSize;
}
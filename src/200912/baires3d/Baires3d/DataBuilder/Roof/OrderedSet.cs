// Decompiled by DJ v3.9.9.91 Copyright 2005 Atanas Neshkov  Date: 26/07/2009 01:41:27 a.m.
// Home Page : http://members.fortunecity.com/neshkov/dj.html  - Check often for new version!
// Decompiler options: packimports(3) 
// Source File Name:   OrderedSet.java


interface OrderedSet
{

    public abstract void insert(Comparable comparable);

    public abstract Comparable removeFirst();

    public abstract int size();
}
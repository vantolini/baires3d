using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Collections;
using Tao.OpenGl;

namespace DataSuite {
// Referenced classes of package com.sun.j3d.utils.geometry:
// Bridge, Clean, Desperate, EarClip,
// GeometryInfo, ListNode, NoHash, Orientation,
// PntNode, Project, Simple, TriangleGLU,
// Distance, Left, HeapNode

    public class TriangulatorGLUbest {
/**
* @deprecated Method TriangulatorGLUbest is deprecated
*/

        public TriangulatorGLUbest()
        {
            faces = null;
            loops = null;
            chains = null;
            points = null;
            triangles = null;
            list = null;
            randomGen = null;
            numPoints = 0;
            maxNumPoints = 0;
            numList = 0;
            maxNumList = 0;
            numLoops = 0;
            maxNumLoops = 0;
            numTriangles = 0;
            maxNumTriangles = 0;
            numFaces = 0;
            numTexSets = 0;
            firstNode = 0;
            numChains = 0;
            maxNumChains = 0;
            pUnsorted = null;
            maxNumPUnsorted = 0;
            noHashingEdges = false;
            noHashingPnts = false;
            vtxList = null;
            numVtxList = 0;
            numReflex = 0;
            distances = null;
            maxNumDist = 0;
            leftMost = null;
            maxNumLeftMost = 0;
            heap = null;
            numHeap = 0;
            maxNumHeap = 0;
            numZero = 0;
            maxNumPolyArea = 0;
            polyArea = null;
            stripCounts = null;
            vertexIndices = null;
            vertices = null;
            colors = null;
            normals = null;
            ccwLoop = true;
            earsRandom = true;
            earsSorted = true;
            epsilon = 9.9999999999999998E-013D;
            earsRandom = false;
            earsSorted = false;
        }

/**
* @deprecated Method TriangulatorGLUbest is deprecated
*/

        public TriangulatorGLUbest(int i)
        {
            faces = null;
            loops = null;
            chains = null;
            points = null;
            triangles = null;
            list = null;
            randomGen = null;
            numPoints = 0;
            maxNumPoints = 0;
            numList = 0;
            maxNumList = 0;
            numLoops = 0;
            maxNumLoops = 0;
            numTriangles = 0;
            maxNumTriangles = 0;
            numFaces = 0;
            numTexSets = 0;
            firstNode = 0;
            numChains = 0;
            maxNumChains = 0;
            pUnsorted = null;
            maxNumPUnsorted = 0;
            noHashingEdges = false;
            noHashingPnts = false;
            vtxList = null;
            numVtxList = 0;
            numReflex = 0;
            distances = null;
            maxNumDist = 0;
            leftMost = null;
            maxNumLeftMost = 0;
            heap = null;
            numHeap = 0;
            maxNumHeap = 0;
            numZero = 0;
            maxNumPolyArea = 0;
            polyArea = null;
            stripCounts = null;
            vertexIndices = null;
            vertices = null;
            colors = null;
            normals = null;
            ccwLoop = true;
            earsRandom = true;
            earsSorted = true;
            epsilon = 9.9999999999999998E-013D;
            switch (i){
                case 0: // '\0'
                    earsRandom = false;
                    earsSorted = false;
                    break;

                case 1: // '\001'
                    randomGen = new Random();
                    earsRandom = true;
                    earsSorted = false;
                    break;

                case 2: // '\002'
                    earsRandom = false;
                    earsSorted = true;
                    break;

                default:
                    earsRandom = false;
                    earsSorted = false;
                    break;
            }
        }

        public void triangulate(Point3f[] vertices, int[] vertexIndices, Vector3f[] normals, int[] stripCounts,
                                int[] faces)
        {
            int i2 = 0;
            bool flag1 = false;
            bool flag2 = false;
            bool[] aflag = new bool[1];
            bool[] aflag1 = new bool[1];
//geometryinfo.indexify();
            this.vertices = vertices;
            this.vertexIndices = vertexIndices;
            this.normals = normals;
            this.stripCounts = stripCounts;
            this.faces = faces;
            if (faces == null){
                if (stripCounts == null)
                    Console.WriteLine("StripCounts is null! Don't know what to do.");
                faces = new int[stripCounts.Length];
                for (int i = 0; i < stripCounts.Length; i++)
                    faces[i] = 1;
            }
            numFaces = faces.Length;
            numTexSets = 0;
            maxNumLoops = 0;
            maxNumList = 0;
            maxNumPoints = 0;
            maxNumDist = 0;
            maxNumLeftMost = 0;
            maxNumPUnsorted = 0;
            for (int j = 0; j < faces.Length; j++){
                maxNumLoops += faces[j];
                for (int i1 = 0; i1 < faces[j];){
                    maxNumList += stripCounts[i2] + 1;
                    i1++;
                    i2++;
                }
            }

            maxNumList += 20;
            loops = new int[maxNumLoops];
            list = new ListNode[maxNumList];
            numVtxList = 0;
            numReflex = 0;
            numTriangles = 0;
            numChains = 0;
            numPoints = 0;
            numLoops = 0;
            numList = 0;
            i2 = 0;
            int j2 = 0;
            for (int k = 0; k < faces.Length; k++){
                for (int j1 = 0; j1 < faces[k];){
                    int k2 = makeLoopHeader();
                    int l2 = loops[k2];
                    for (int l1 = 0; l1 < stripCounts[i2]; l1++){
                        list[numList] = new ListNode(vertexIndices[j2]);
                        int i3 = numList++;
                        insertAfter(l2, i3);
                        list[i3].setCommonIndex(j2);
                        l2 = i3;
                        j2++;
                    }

                    deleteHook(k2);
                    j1++;
                    i2++;
                }
            }

            maxNumTriangles = maxNumList/2;
            triangles = new TriangleGLU[maxNumTriangles];
            setEpsilon(1E-008D);
//printListData();

            int i4 = 0;
            bool flag4 = false;
            for (int k1 = 0; k1 < numFaces; k1++){
                ccwLoop = true;
                aflag[0] = false;
                int j4 = i4 + faces[k1];
                bool flag;
                if (faces[k1] > 1)
                    flag = true;
                else if (Simple.simpleFace(this, loops[i4]))
                    flag = false;
                else
                    flag = true;
                if (flag){
//Console.WriteLine("faces["+k1+"] "+faces[k1]);
                    for (int k4 = 0; k4 < faces[k1]; k4++)
                        preProcessList(i4 + k4);

                    Project.projectFace(this, i4, j4);

//Console.WriteLine("Before cleaning ...");
//printListData();

                    int l4 = Clean.cleanPolyhedralFace(this, i4, j4);

                    if (faces[k1] == 1)
                        Orientation.determineOrientation(this, loops[i4]);
                    else
                        Orientation.adjustOrientation(this, i4, j4);
                    if (faces[k1] > 1){
                        NoHash.prepareNoHashEdges(this, i4, j4);
                    }
                    else{
                        noHashingEdges = false;
                        noHashingPnts = false;
                    }
                    for (int l = i4; l < j4; l++)
                        EarClip.classifyAngles(this, loops[l]);

//Console.WriteLine("After classifyAngles ...");
//printListData();

                    if (faces[k1] > 1)
                        Bridge.constructBridges(this, i4, j4);
                    resetPolyList(loops[i4]);
                    NoHash.prepareNoHashPnts(this, i4);
                    EarClip.classifyEars(this, loops[i4]);
                    aflag[0] = false;
                    do{
                        if (aflag[0])
                            break;
                        if (!EarClip.clipEar(this, aflag)){
                            if (flag1){
                                int j3 = getNode();
                                resetPolyList(j3);
                                loops[i4] = j3;
                                if (Desperate.desperate(this, j3, i4, aflag)){
                                    if (!Desperate.letsHope(this, j3))
                                        return;
                                }
                                else{
                                    flag1 = false;
                                }
                            }
                            else{
                                bool flag3 = true;
                                int k3 = getNode();
                                resetPolyList(k3);
                                EarClip.classifyEars(this, k3);
                                flag1 = true;
                            }
                        }
                        else{
                            flag1 = false;
                        }
                        if (aflag[0]){
                            int l3 = getNextChain(aflag1);
                            if (aflag1[0]){
                                resetPolyList(l3);
                                loops[i4] = l3;
                                noHashingPnts = false;
                                NoHash.prepareNoHashPnts(this, i4);
                                EarClip.classifyEars(this, l3);
                                flag1 = false;
                                aflag[0] = false;
                            }
                        }
                    } while (true);
                }
                i4 = j4;
            }

//writeTriangleToGeomInfo();
        }

        public void printVtxList()
        {
            Console.WriteLine("numReflex " + numReflex + " reflexVertices " + reflexVertices);
            for (int i = 0; i < numVtxList; i++)
                Console.WriteLine("" + i + " pnt " + vtxList[i].pnt + ", next " + vtxList[i].next);
        }

        public void printListData()
        {
            for (int i = 0; i < numList; i++)
                Console.WriteLine("list[" + i + "].index " + list[i].index + ", prev " + list[i].prev + ", next " +
                                  list[i].next + ", convex " + list[i].convex + ", vertexIndex " + list[i].vcntIndex);
        }

        public void preProcessList(int i)
        {
            resetPolyList(loops[i]);
            int j = loops[i];
            int k = j;
            for (int l = list[k].next; l != j; l = list[k].next){
                if (list[k].index == list[l].index){
                    if (l == loops[i])
                        loops[i] = list[l].next;
                    deleteLinks(l);
                }
                k = list[k].next;
            }
        }

        public void writeTriangleToGeomInfo()
        {
            int i1 = 0;
            int[] ai = new int[numTriangles*3];
            for (int i = 0; i < numTriangles; i++){
                int i2 = list[triangles[i].v1].getCommonIndex();
                ai[i1++] = vertexIndices[i2];
                i2 = list[triangles[i].v2].getCommonIndex();
                ai[i1++] = vertexIndices[i2];
                i2 = list[triangles[i].v3].getCommonIndex();
                ai[i1++] = vertexIndices[i2];
            }

            if (normals != null){
//int[] ai1 = normals;
//int[] ai3 = new int[numTriangles * 3];
//int j1 = 0;
//for (int j = 0; j < numTriangles; j++)
//{
// int j2 = list[triangles[j].v1].getCommonIndex();
// ai3[j1++] = ai1[j2];
// j2 = list[triangles[j].v2].getCommonIndex();
// ai3[j1++] = ai1[j2];
// j2 = list[triangles[j].v3].getCommonIndex();
// ai3[j1++] = ai1[j2];
//}

//gInfo.setNormalIndices(ai3);
            }
            if (colors != null){
//int k1 = 0;
////int[] ai2 = gInfo.getColorIndices();
//int[] ai4 = new int[numTriangles * 3];
//for(int k = 0; k < numTriangles; k++)
//{
// int k2 = list[triangles[k].v1].getCommonIndex();
// ai4[k1++] = ai2[k2];
// k2 = list[triangles[k].v2].getCommonIndex();
// ai4[k1++] = ai2[k2];
// k2 = list[triangles[k].v3].getCommonIndex();
// ai4[k1++] = ai2[k2];
//}

//gInfo.setColorIndices(ai4);
            }
        }

        public void setEpsilon(double d)
        {
            epsilon = d;
        }

        public bool inPolyList(int i)
        {
            return i >= 0 && i < numList && numList <= maxNumList;
        }

        public void updateIndex(int i, int j)
        {
            list[i].index = j;
        }

        public int getAngle(int i)
        {
            return list[i].convex;
        }

        public void setAngle(int i, int j)
        {
            list[i].convex = j;
        }

        public void resetPolyList(int i)
        {
            firstNode = i;
        }

        public int getNode()
        {
            return firstNode;
        }

        public bool inLoopList(int i)
        {
            return i >= 0 && i < numLoops && numLoops <= maxNumLoops;
        }

        public void deleteHook(int i)
        {
            if (!inLoopList(i))
                Console.WriteLine("TriangulatorGLUbest:deleteHook : Loop access out of range.");
            int j = loops[i];
            int k = list[j].next;
            if (inPolyList(j) && inPolyList(k)){
                deleteLinks(j);
                loops[i] = k;
            }
            else{
                Console.WriteLine("TriangulatorGLUbest:deleteHook : List access out of range.");
            }
        }

        public void deleteLinks(int i)
        {
            if (inPolyList(i) && inPolyList(list[i].prev) && inPolyList(list[i].next)){
                if (firstNode == i)
                    firstNode = list[i].next;
                list[list[i].next].prev = list[i].prev;
                list[list[i].prev].next = list[i].next;
                list[i].prev = list[i].next = i;
            }
            else{
                Console.WriteLine("TriangulatorGLUbest:deleteLinks : Access out of range.");
            }
        }

        public void rotateLinks(int i, int j)
        {
            int l = list[i].next;
            int i1 = list[j].next;
            int k = list[i].next;
            list[i].next = list[j].next;
            list[j].next = k;
            list[l].prev = j;
            list[i1].prev = i;
        }

        public void storeChain(int i)
        {
            if (numChains >= maxNumChains){
                maxNumChains += 20;
                int[] ai = chains;
                chains = new int[maxNumChains];
                if (ai != null)
                    System.Array.Copy(ai, chains, ai.Length);
            }
            chains[numChains] = i;
            numChains++;
        }

        public int getNextChain(bool[] aflag)
        {
            if (numChains > 0){
                aflag[0] = true;
                numChains--;
                return chains[numChains];
            }
            else{
                aflag[0] = false;
                numChains = 0;
                return 0;
            }
        }

        public void splitSplice(int i, int j, int k, int l)
        {
            list[i].next = l;
            list[l].prev = i;
            list[j].prev = k;
            list[k].next = j;
        }

        public int makeHook()
        {
            int i = numList;
            if (numList >= maxNumList){
                maxNumList += 100;
                ListNode[] alistnode = list;
                list = new ListNode[maxNumList];
                System.Array.Copy(alistnode, 0, list, 0, alistnode.Length);
            }
            list[numList] = new ListNode(-1);
            list[numList].prev = i;
            list[numList].next = i;
            list[numList].index = -1;
            numList++;
            return i;
        }

        public int makeLoopHeader()
        {
            int j = makeHook();
            if (numLoops >= maxNumLoops){
                maxNumLoops += 20;
                int[] ai = loops;
                loops = new int[maxNumLoops];
                System.Array.Copy(ai, 0, loops, 0, ai.Length);
            }
            loops[numLoops] = j;
            int i = numLoops;
            numLoops++;
            return i;
        }

        public int makeNode(int i)
        {
            if (numList >= maxNumList){
                maxNumList += 100;
                ListNode[] alistnode = list;
                list = new ListNode[maxNumList];
                System.Array.Copy(alistnode, 0, list, 0, alistnode.Length);
            }
            list[numList] = new ListNode(i);
            int j = numList;
            list[numList].index = i;
            list[numList].prev = -1;
            list[numList].next = -1;
            numList++;
            return j;
        }

        public void insertAfter(int i, int j)
        {
            if (inPolyList(i) && inPolyList(j)){
                list[j].next = list[i].next;
                list[j].prev = i;
                list[i].next = j;
                int k = list[j].next;
                if (inPolyList(k))
                    list[k].prev = j;
                else
                    Console.WriteLine("TriangulatorGLUbest:deleteHook : List access out of range.");
                return;
            }
            else{
                Console.WriteLine("TriangulatorGLUbest:deleteHook : List access out of range.");
                return;
            }
        }

        public int fetchNextData(int i)
        {
            return list[i].next;
        }

        public int fetchData(int i)
        {
            return list[i].index;
        }

        public int fetchPrevData(int i)
        {
            return list[i].prev;
        }

        public void swapLinks(int i)
        {
            int j = list[i].next;
            list[i].next = list[i].prev;
            list[i].prev = j;
            int k = j;
            int l;
            for (; j != i; j = l){
                l = list[j].next;
                list[j].next = list[j].prev;
                list[j].prev = l;
            }
        }

        public void storeTriangle(int i, int j, int k)
        {
            if (numTriangles >= maxNumTriangles){
                maxNumTriangles += 50;
                TriangleGLU[] atriangle = triangles;
                triangles = new TriangleGLU[maxNumTriangles];
                if (atriangle != null)
                    System.Array.Copy(atriangle, 0, triangles, 0, atriangle.Length);
            }
            if (ccwLoop)
                triangles[numTriangles] = new TriangleGLU(i, j, k);
            else
                triangles[numTriangles] = new TriangleGLU(j, i, k);
            numTriangles++;
        }

        public void initPnts(int i)
        {
            if (maxNumPoints < i){
                maxNumPoints = i;
                points = new Point2f[maxNumPoints];
            }
            for (int j = 0; j < i; j++)
                points[j] = new Point2f(0.0F, 0.0F);

            numPoints = 0;
        }

        public bool inPointsList(int i)
        {
            return i >= 0 && i < numPoints && numPoints <= maxNumPoints;
        }

        public int storePoint(double d, double d1)
        {
            if (numPoints >= maxNumPoints){
                maxNumPoints += 100;
                Point2f[] apoint2f = points;
                points = new Point2f[maxNumPoints];
                if (apoint2f != null)
                    System.Array.Copy(apoint2f, 0, points, 0, apoint2f.Length);
            }
            points[numPoints] = new Point2f((float) d, (float) d1);
            int i = numPoints;
            numPoints++;
            return i;
        }

        public int[] faces;
        public int[] loops;
        public int[] chains;
        public Point2f[] points;
        public TriangleGLU[] triangles;
        public ListNode[] list;
        public Random randomGen;
        public int numPoints;
        public int maxNumPoints;
        public int numList;
        public int maxNumList;
        public int numLoops;
        public int maxNumLoops;
        public int numTriangles;
        public int maxNumTriangles;
        public int numFaces;
        public int numTexSets;
        public int firstNode;
        public int numChains;
        public int maxNumChains;
        public Point2f[] pUnsorted;
        public int maxNumPUnsorted;
        public bool noHashingEdges;
        public bool noHashingPnts;
        public int loopMin;
        public int loopMax;
        public PntNode[] vtxList;
        public int numVtxList;
        public int numReflex;
        public int reflexVertices;
        public Distance[] distances;
        public int maxNumDist;
        public Left[] leftMost;
        public int maxNumLeftMost;
        public HeapNode[] heap;
        public int numHeap;
        public int maxNumHeap;
        public int numZero;
        public int maxNumPolyArea;
        public double[] polyArea;
        public int[] stripCounts;
        public int[] vertexIndices;
        public Point3f[] vertices;
        public Object[] colors;
        public Vector3f[] normals;
        public bool ccwLoop;
        public bool earsRandom;
        public bool earsSorted;
        public int identCntr;
        public double epsilon;
        public static double ZERO = 1E-008D;
        public static int EARS_SEQUENCE = 0;
        public static int EARS_RANDOM = 1;
        public static int EARS_SORTED = 2;
        public static int INC_LIST_BK = 100;
        public static int INC_LOOP_BK = 20;
        public static int INC_TRI_BK = 50;
        public static int INC_POINT_BK = 100;
        public static int INC_DIST_BK = 50;
        public static int DEBUG = 0;
    }

    public class TriangleGLU {
        public TriangleGLU(int i, int j, int k)
        {
            v1 = i;
            v2 = j;
            v3 = k;
        }

        public int v1;
        public int v2;
        public int v3;
    }

    public class Distance {
        public Distance() {}

        public void copy(Distance distance)
        {
            ind = distance.ind;
            dist = distance.dist;
        }

        public int ind;
        public double dist;
    }

    public class HeapNode {
        public HeapNode() {}

        public void copy(HeapNode heapnode)
        {
            index = heapnode.index;
            prev = heapnode.prev;
            next = heapnode.next;
            ratio = heapnode.ratio;
        }

        public int index;
        public int prev;
        public int next;
        public double ratio;
    }

    public class Left {
        public Left() {}

        public void copy(Left left)
        {
            ind = left.ind;
            index = left.index;
        }

        public int ind;
        public int index;
    }

    public class ListNode {
        public ListNode(int i)
        {
            index = i;
            prev = -1;
            next = -1;
            convex = 0;
            vcntIndex = -1;
        }

        public void setCommonIndex(int i)
        {
            vcntIndex = i;
        }

        public int getCommonIndex()
        {
            return vcntIndex;
        }

        public int index;
        public int prev;
        public int next;
        public int convex;
        public int vcntIndex;
    }

    public class PntNode {
        public PntNode() {}

        public int pnt;
        public int next;
    }

    public class Point2f : Tuple2f {
        public Point2f()
        {
            this.x = 0f;
            this.y = 0f;
        }

        public Point2f(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class Point3f : Tuple3f {
        public Point3f() {}

        public Point3f(double[] d)
        {
            x = (float) d[0];
            y = (float) d[1];
            z = (float) d[2];
        }

        public void set(double[] d)
        {
            x = (float) d[0];
            y = (float) d[1];
            z = (float) d[2];
        }
    }

    public class Vector3f : Tuple3f {}

    public class Bridge {
        public Bridge() {}

        public static void constructBridges(TriangulatorGLUbest TriangulatorGLUbest, int i, int j)
        {
            int[] ai = new int[1];
            int[] ai1 = new int[1];
            int[] ai2 = new int[1];
            int[] ai3 = new int[1];
            int[] ai4 = new int[1];
            int[] ai5 = new int[1];
            if (!TriangulatorGLUbest.noHashingEdges)
                Console.WriteLine("Bridge:constructBridges noHashingEdges is false");
            if (j <= i)
                Console.WriteLine("Bridge:constructBridges loopMax<=loopMin");
            if (i < 0)
                Console.WriteLine("Bridge:constructBridges loopMin<0");
            if (j > TriangulatorGLUbest.numLoops)
                Console.WriteLine("Bridge:constructBridges loopMax>triRef.numLoops");
            int k1 = j - i - 1;
            if (k1 > TriangulatorGLUbest.maxNumLeftMost){
                TriangulatorGLUbest.maxNumLeftMost = k1;
                TriangulatorGLUbest.leftMost = new Left[k1];
            }
            findLeftMostVertex(TriangulatorGLUbest, TriangulatorGLUbest.loops[i], ai1, ai);
            int l = 0;
            for (int k = i + 1; k < j; k++){
                findLeftMostVertex(TriangulatorGLUbest, TriangulatorGLUbest.loops[k], ai5, ai4);
                TriangulatorGLUbest.leftMost[l] = new Left();
                TriangulatorGLUbest.leftMost[l].ind = ai5[0];
                TriangulatorGLUbest.leftMost[l].index = ai4[0];
                l++;
            }

            sortLeft(TriangulatorGLUbest.leftMost, k1);
            int j1 = TriangulatorGLUbest.numPoints + 2*TriangulatorGLUbest.numLoops;
            TriangulatorGLUbest.maxNumDist = j1;
            TriangulatorGLUbest.distances = new Distance[j1];
            for (int l1 = 0; l1 < TriangulatorGLUbest.maxNumDist; l1++)
                TriangulatorGLUbest.distances[l1] = new Distance();

            for (int i1 = 0; i1 < k1; i1++){
                if (findBridge(TriangulatorGLUbest, ai1[0], ai[0], TriangulatorGLUbest.leftMost[i1].index, ai3, ai2)) ;
                if (ai2[0] == TriangulatorGLUbest.leftMost[i1].index)
                    simpleBridge(TriangulatorGLUbest, ai3[0], TriangulatorGLUbest.leftMost[i1].ind);
                else
                    insertBridge(TriangulatorGLUbest, ai3[0], ai2[0], TriangulatorGLUbest.leftMost[i1].ind,
                                 TriangulatorGLUbest.leftMost[i1].index);
            }
        }

        public static bool findBridge(TriangulatorGLUbest TriangulatorGLUbest, int i, int j, int k, int[] ai, int[] ai1)
        {
            int j2 = 0;
            Object obj = null;
            ai[0] = i;
            ai1[0] = j;
            if (ai1[0] == k)
                return true;
            if (j2 >= TriangulatorGLUbest.maxNumDist){
                TriangulatorGLUbest _tmp = TriangulatorGLUbest;
                TriangulatorGLUbest.maxNumDist += 50;
                Distance[] adistance = TriangulatorGLUbest.distances;
                TriangulatorGLUbest.distances = new Distance[TriangulatorGLUbest.maxNumDist];
                System.Array.Copy(adistance, 0, TriangulatorGLUbest.distances, 0, adistance.Length);
                for (int k3 = adistance.Length; k3 < TriangulatorGLUbest.maxNumDist; k3++)
                    TriangulatorGLUbest.distances[k3] = new Distance();
            }
            TriangulatorGLUbest.distances[j2].dist = Numerics.baseLength(TriangulatorGLUbest.points[k],
                                                                         TriangulatorGLUbest.points[ai1[0]]);
            TriangulatorGLUbest.distances[j2].ind = ai[0];
            j2++;
            ai[0] = TriangulatorGLUbest.fetchNextData(ai[0]);
            for (ai1[0] = TriangulatorGLUbest.fetchData(ai[0]);
                 ai[0] != i;
                 ai1[0] = TriangulatorGLUbest.fetchData(ai[0])){
                if (ai1[0] == k)
                    return true;
                if (j2 >= TriangulatorGLUbest.maxNumDist){
                    TriangulatorGLUbest _tmp1 = TriangulatorGLUbest;
                    TriangulatorGLUbest.maxNumDist += 50;
                    Distance[] adistance1 = TriangulatorGLUbest.distances;
                    TriangulatorGLUbest.distances = new Distance[TriangulatorGLUbest.maxNumDist];
                    System.Array.Copy(adistance1, 0, TriangulatorGLUbest.distances, 0, adistance1.Length);
                    for (int l3 = adistance1.Length; l3 < TriangulatorGLUbest.maxNumDist; l3++)
                        TriangulatorGLUbest.distances[l3] = new Distance();
                }
                TriangulatorGLUbest.distances[j2].dist = Numerics.baseLength(TriangulatorGLUbest.points[k],
                                                                             TriangulatorGLUbest.points[ai1[0]]);
                TriangulatorGLUbest.distances[j2].ind = ai[0];
                j2++;
                ai[0] = TriangulatorGLUbest.fetchNextData(ai[0]);
            }

            sortDistance(TriangulatorGLUbest.distances, j2);
            for (int l1 = 0; l1 < j2; l1++){
                ai[0] = TriangulatorGLUbest.distances[l1].ind;
                ai1[0] = TriangulatorGLUbest.fetchData(ai[0]);
                if (ai1[0] > k)
                    continue;
                int k2 = TriangulatorGLUbest.fetchPrevData(ai[0]);
                int l = TriangulatorGLUbest.fetchData(k2);
                int i3 = TriangulatorGLUbest.fetchNextData(ai[0]);
                int j1 = TriangulatorGLUbest.fetchData(i3);
                bool flag = TriangulatorGLUbest.getAngle(ai[0]) > 0;
                bool flag1 = Numerics.isInCone(TriangulatorGLUbest, l, ai1[0], j1, k, flag);
                if (!flag1)
                    continue;
                BBox bbox = new BBox(TriangulatorGLUbest, ai1[0], k);
                if (!NoHash.noHashEdgeIntersectionExists(TriangulatorGLUbest, bbox, -1, -1, ai[0], -1))
                    return true;
            }

            for (int i2 = 0; i2 < j2; i2++){
                ai[0] = TriangulatorGLUbest.distances[i2].ind;
                ai1[0] = TriangulatorGLUbest.fetchData(ai[0]);
                int l2 = TriangulatorGLUbest.fetchPrevData(ai[0]);
                int i1 = TriangulatorGLUbest.fetchData(l2);
                int j3 = TriangulatorGLUbest.fetchNextData(ai[0]);
                int k1 = TriangulatorGLUbest.fetchData(j3);
                BBox bbox1 = new BBox(TriangulatorGLUbest, ai1[0], k);
                if (!NoHash.noHashEdgeIntersectionExists(TriangulatorGLUbest, bbox1, -1, -1, ai[0], -1))
                    return true;
            }

            ai[0] = i;
            ai1[0] = j;
            return false;
        }

        public static void findLeftMostVertex(TriangulatorGLUbest TriangulatorGLUbest, int i, int[] ai, int[] ai1)
        {
            int j = i;
            int k = TriangulatorGLUbest.fetchData(j);
            ai[0] = j;
            ai1[0] = k;
            j = TriangulatorGLUbest.fetchNextData(j);
            for (int l = TriangulatorGLUbest.fetchData(j); j != i; l = TriangulatorGLUbest.fetchData(j)){
                if (l < ai1[0]){
                    ai[0] = j;
                    ai1[0] = l;
                }
                else if (l == ai1[0] && TriangulatorGLUbest.getAngle(j) < 0){
                    ai[0] = j;
                    ai1[0] = l;
                }
                j = TriangulatorGLUbest.fetchNextData(j);
            }
        }

        public static void simpleBridge(TriangulatorGLUbest TriangulatorGLUbest, int i, int j)
        {
            TriangulatorGLUbest.rotateLinks(i, j);
            int i1 = TriangulatorGLUbest.fetchData(i);
            int l = TriangulatorGLUbest.fetchNextData(i);
            int l1 = TriangulatorGLUbest.fetchData(l);
            int k = TriangulatorGLUbest.fetchPrevData(i);
            int k1 = TriangulatorGLUbest.fetchData(k);
            int i2 = Numerics.isConvexAngle(TriangulatorGLUbest, k1, i1, l1, i);
            TriangulatorGLUbest.setAngle(i, i2);
            int j1 = TriangulatorGLUbest.fetchData(j);
            l = TriangulatorGLUbest.fetchNextData(j);
            l1 = TriangulatorGLUbest.fetchData(l);
            k = TriangulatorGLUbest.fetchPrevData(j);
            k1 = TriangulatorGLUbest.fetchData(k);
            i2 = Numerics.isConvexAngle(TriangulatorGLUbest, k1, j1, l1, j);
            TriangulatorGLUbest.setAngle(j, i2);
        }

        private static void insertBridge(TriangulatorGLUbest TriangulatorGLUbest, int i, int j, int k, int l)
        {
            int i1 = TriangulatorGLUbest.makeNode(j);
            TriangulatorGLUbest.insertAfter(i, i1);
            int l2 = TriangulatorGLUbest.list[i].getCommonIndex();
            TriangulatorGLUbest.list[i1].setCommonIndex(l2);
            int j1 = TriangulatorGLUbest.makeNode(l);
            TriangulatorGLUbest.insertAfter(k, j1);
            l2 = TriangulatorGLUbest.list[k].getCommonIndex();
            TriangulatorGLUbest.list[j1].setCommonIndex(l2);
            TriangulatorGLUbest.splitSplice(i, i1, k, j1);
            int l1 = TriangulatorGLUbest.fetchNextData(i);
            int j2 = TriangulatorGLUbest.fetchData(l1);
            int k1 = TriangulatorGLUbest.fetchPrevData(i);
            int i2 = TriangulatorGLUbest.fetchData(k1);
            int k2 = Numerics.isConvexAngle(TriangulatorGLUbest, i2, j, j2, i);
            TriangulatorGLUbest.setAngle(i, k2);
            l1 = TriangulatorGLUbest.fetchNextData(i1);
            j2 = TriangulatorGLUbest.fetchData(l1);
            k1 = TriangulatorGLUbest.fetchPrevData(i1);
            i2 = TriangulatorGLUbest.fetchData(k1);
            k2 = Numerics.isConvexAngle(TriangulatorGLUbest, i2, j, j2, i1);
            TriangulatorGLUbest.setAngle(i1, k2);
            l1 = TriangulatorGLUbest.fetchNextData(k);
            j2 = TriangulatorGLUbest.fetchData(l1);
            k1 = TriangulatorGLUbest.fetchPrevData(k);
            i2 = TriangulatorGLUbest.fetchData(k1);
            k2 = Numerics.isConvexAngle(TriangulatorGLUbest, i2, l, j2, k);
            TriangulatorGLUbest.setAngle(k, k2);
            l1 = TriangulatorGLUbest.fetchNextData(j1);
            j2 = TriangulatorGLUbest.fetchData(l1);
            k1 = TriangulatorGLUbest.fetchPrevData(j1);
            i2 = TriangulatorGLUbest.fetchData(k1);
            k2 = Numerics.isConvexAngle(TriangulatorGLUbest, i2, l, j2, j1);
            TriangulatorGLUbest.setAngle(j1, k2);
        }

        private static int l_comp(Left left, Left left1)
        {
            if (left.index < left1.index)
                return -1;
            return left.index <= left1.index ? 0 : 1;
        }

        private static int d_comp(Distance distance, Distance distance1)
        {
            if (distance.dist < distance1.dist)
                return -1;
            return distance.dist <= distance1.dist ? 0 : 1;
        }

        private static void sortLeft(Left[] aleft, int i)
        {
            Left left = new Left();
            for (int j = 0; j < i; j++){
                for (int k = j + 1; k < i; k++)
                    if (l_comp(aleft[j], aleft[k]) > 0){
                        left.copy(aleft[j]);
                        aleft[j].copy(aleft[k]);
                        aleft[k].copy(left);
                    }
            }
        }

        public static void sortDistance(Distance[] adistance, int i)
        {
            Distance distance = new Distance();
            for (int j = 0; j < i; j++){
                for (int k = j + 1; k < i; k++)
                    if (d_comp(adistance[j], adistance[k]) > 0){
                        distance.copy(adistance[j]);
                        adistance[j].copy(adistance[k]);
                        adistance[k].copy(distance);
                    }
            }
        }
    }

    public class Simple {
        public Simple() {}

        public static bool simpleFace(TriangulatorGLUbest TriangulatorGLUbest, int i)
        {
            int j = TriangulatorGLUbest.fetchPrevData(i);
            int j2 = TriangulatorGLUbest.fetchData(j);
            if (j == i){
                Console.WriteLine("***** polygon with only one vertex?! *****\n");
                return true;
            }
            int k = TriangulatorGLUbest.fetchNextData(i);
            int l1 = TriangulatorGLUbest.fetchData(k);
            if (j == k){
                Console.WriteLine("***** polygon with only two vertices?! *****\n");
                return true;
            }
            int l = TriangulatorGLUbest.fetchNextData(k);
            int i2 = TriangulatorGLUbest.fetchData(l);
            if (j == l){
                int j1 = TriangulatorGLUbest.fetchData(i);
                TriangulatorGLUbest.storeTriangle(i, k, l);
                return true;
            }
            int i1 = TriangulatorGLUbest.fetchNextData(l);
            int k2 = TriangulatorGLUbest.fetchData(i1);
            if (j == i1){
                TriangulatorGLUbest.initPnts(5);
                int k1 = TriangulatorGLUbest.fetchData(i);
                Point3f point3f = new Point3f();
                Point3f point3f1 = new Point3f();
                Point3f point3f2 = new Point3f();
                Basic.vectorSub(TriangulatorGLUbest.vertices[k1], TriangulatorGLUbest.vertices[l1], point3f);
                Basic.vectorSub(TriangulatorGLUbest.vertices[i2], TriangulatorGLUbest.vertices[l1], point3f1);
                Basic.vectorProduct(point3f, point3f1, point3f2);
                double d = Math.Abs(point3f2.x);
                double d1 = Math.Abs(point3f2.y);
                double d2 = Math.Abs(point3f2.z);
                if (d2 >= d && d2 >= d1){
                    TriangulatorGLUbest.points[1].x = TriangulatorGLUbest.vertices[k1].x;
                    TriangulatorGLUbest.points[1].y = TriangulatorGLUbest.vertices[k1].y;
                    TriangulatorGLUbest.points[2].x = TriangulatorGLUbest.vertices[l1].x;
                    TriangulatorGLUbest.points[2].y = TriangulatorGLUbest.vertices[l1].y;
                    TriangulatorGLUbest.points[3].x = TriangulatorGLUbest.vertices[i2].x;
                    TriangulatorGLUbest.points[3].y = TriangulatorGLUbest.vertices[i2].y;
                    TriangulatorGLUbest.points[4].x = TriangulatorGLUbest.vertices[k2].x;
                    TriangulatorGLUbest.points[4].y = TriangulatorGLUbest.vertices[k2].y;
                }
                else if (d >= d1 && d >= d2){
                    TriangulatorGLUbest.points[1].x = TriangulatorGLUbest.vertices[k1].z;
                    TriangulatorGLUbest.points[1].y = TriangulatorGLUbest.vertices[k1].y;
                    TriangulatorGLUbest.points[2].x = TriangulatorGLUbest.vertices[l1].z;
                    TriangulatorGLUbest.points[2].y = TriangulatorGLUbest.vertices[l1].y;
                    TriangulatorGLUbest.points[3].x = TriangulatorGLUbest.vertices[i2].z;
                    TriangulatorGLUbest.points[3].y = TriangulatorGLUbest.vertices[i2].y;
                    TriangulatorGLUbest.points[4].x = TriangulatorGLUbest.vertices[k2].z;
                    TriangulatorGLUbest.points[4].y = TriangulatorGLUbest.vertices[k2].y;
                }
                else{
                    TriangulatorGLUbest.points[1].x = TriangulatorGLUbest.vertices[k1].x;
                    TriangulatorGLUbest.points[1].y = TriangulatorGLUbest.vertices[k1].z;
                    TriangulatorGLUbest.points[2].x = TriangulatorGLUbest.vertices[l1].x;
                    TriangulatorGLUbest.points[2].y = TriangulatorGLUbest.vertices[l1].z;
                    TriangulatorGLUbest.points[3].x = TriangulatorGLUbest.vertices[i2].x;
                    TriangulatorGLUbest.points[3].y = TriangulatorGLUbest.vertices[i2].z;
                    TriangulatorGLUbest.points[4].x = TriangulatorGLUbest.vertices[k2].x;
                    TriangulatorGLUbest.points[4].y = TriangulatorGLUbest.vertices[k2].z;
                }
                TriangulatorGLUbest.numPoints = 5;
                int l2 = Numerics.orientation(TriangulatorGLUbest, 1, 2, 3);
                int i3 = Numerics.orientation(TriangulatorGLUbest, 1, 3, 4);
                if (l2 > 0 && i3 > 0 || l2 < 0 && i3 < 0){
                    TriangulatorGLUbest.storeTriangle(i, k, l);
                    TriangulatorGLUbest.storeTriangle(i, l, i1);
                }
                else{
                    TriangulatorGLUbest.storeTriangle(k, l, i1);
                    TriangulatorGLUbest.storeTriangle(k, i1, i);
                }
                return true;
            }
            else{
                return false;
            }
        }
    }

    public class EarClip {
        public EarClip() {}

        public static void classifyAngles(TriangulatorGLUbest TriangulatorGLUbest, int i)
        {
            int k = i;
            int j1 = TriangulatorGLUbest.fetchData(k);
            int j = TriangulatorGLUbest.fetchPrevData(k);
            int i1 = TriangulatorGLUbest.fetchData(j);
            do{
                int l = TriangulatorGLUbest.fetchNextData(k);
                int k1 = TriangulatorGLUbest.fetchData(l);
                int l1 = Numerics.isConvexAngle(TriangulatorGLUbest, i1, j1, k1, k);
                TriangulatorGLUbest.setAngle(k, l1);
                i1 = j1;
                j1 = k1;
                k = l;
            } while (k != i);
        }

        public static void classifyEars(TriangulatorGLUbest TriangulatorGLUbest, int i)
        {
            int[] ai = new int[1];
            int[] ai1 = new int[1];
            double[] ad = new double[1];
            Heap.initHeap(TriangulatorGLUbest);
            int j = i;
            int k = TriangulatorGLUbest.fetchData(j);
            do{
                if (TriangulatorGLUbest.getAngle(j) > 0 && isEar(TriangulatorGLUbest, j, ai, ai1, ad))
                    Heap.dumpOnHeap(TriangulatorGLUbest, ad[0], j, ai[0], ai1[0]);
                j = TriangulatorGLUbest.fetchNextData(j);
                int l = TriangulatorGLUbest.fetchData(j);
            } while (j != i);
        }

        public static bool isEar(TriangulatorGLUbest TriangulatorGLUbest, int i, int[] ai, int[] ai1, double[] ad)
        {
            int l = TriangulatorGLUbest.fetchData(i);
            ai1[0] = TriangulatorGLUbest.fetchNextData(i);
            int i1 = TriangulatorGLUbest.fetchData(ai1[0]);
            int l1 = TriangulatorGLUbest.fetchNextData(ai1[0]);
            int j1 = TriangulatorGLUbest.fetchData(l1);
            ai[0] = TriangulatorGLUbest.fetchPrevData(i);
            int k = TriangulatorGLUbest.fetchData(ai[0]);
            int k1 = TriangulatorGLUbest.fetchPrevData(ai[0]);
            int j = TriangulatorGLUbest.fetchData(k1);
            if (k == i1 || k == l || l == i1 || TriangulatorGLUbest.getAngle(i) == 2){
                ad[0] = 0.0D;
                return true;
            }
            if (j == i1)
                if (TriangulatorGLUbest.getAngle(k1) < 0 || TriangulatorGLUbest.getAngle(ai1[0]) < 0){
                    ad[0] = 0.0D;
                    return true;
                }
                else{
                    return false;
                }
            if (k == j1)
                if (TriangulatorGLUbest.getAngle(ai[0]) < 0 || TriangulatorGLUbest.getAngle(l1) < 0){
                    ad[0] = 0.0D;
                    return true;
                }
                else{
                    return false;
                }
            bool flag = TriangulatorGLUbest.getAngle(ai[0]) > 0;
            bool flag1 = Numerics.isInCone(TriangulatorGLUbest, j, k, l, i1, flag);
            if (!flag1)
                return false;
            flag = TriangulatorGLUbest.getAngle(ai1[0]) > 0;
            flag1 = Numerics.isInCone(TriangulatorGLUbest, l, i1, j1, k, flag);
            if (flag1){
                BBox bbox = new BBox(TriangulatorGLUbest, k, i1);
                if (!NoHash.noHashIntersectionExists(TriangulatorGLUbest, l, i, i1, k, bbox)){
                    if (TriangulatorGLUbest.earsSorted)
                        ad[0] = Numerics.getRatio(TriangulatorGLUbest, k, i1, l);
                    else
                        ad[0] = 1.0D;
                    return true;
                }
            }
            return false;
        }

        public static bool clipEar(TriangulatorGLUbest TriangulatorGLUbest, bool[] aflag)
        {
            double[] ad = new double[1];
            int[] ai = new int[1];
            int[] ai1 = new int[1];
            int[] ai2 = new int[1];
            int[] ai3 = new int[1];
            int[] ai4 = new int[1];
            int[] ai5 = new int[1];
            bool flag = false;
            int j;
            int k;
            int j1;
            int l1;
            do{
                if (!Heap.deleteFromHeap(TriangulatorGLUbest, ai5, ai1, ai3))
                    return false;
                j = TriangulatorGLUbest.fetchPrevData(ai5[0]);
                j1 = TriangulatorGLUbest.fetchData(j);
                k = TriangulatorGLUbest.fetchNextData(ai5[0]);
                l1 = TriangulatorGLUbest.fetchData(k);
            } while (ai1[0] != j || ai3[0] != k);
            int k1 = TriangulatorGLUbest.fetchData(ai5[0]);
            TriangulatorGLUbest.deleteLinks(ai5[0]);
            TriangulatorGLUbest.storeTriangle(j, ai5[0], k);
            int i = TriangulatorGLUbest.fetchPrevData(j);
            int i1 = TriangulatorGLUbest.fetchData(i);
            if (i == k){
                aflag[0] = true;
                return true;
            }
            int j2 = Numerics.isConvexAngle(TriangulatorGLUbest, i1, j1, l1, j);
            int l = TriangulatorGLUbest.fetchNextData(k);
            int i2 = TriangulatorGLUbest.fetchData(l);
            int k2 = Numerics.isConvexAngle(TriangulatorGLUbest, j1, l1, i2, k);
            if (j1 != l1){
                if (j2 >= 0 && TriangulatorGLUbest.getAngle(j) < 0)
                    NoHash.deleteReflexVertex(TriangulatorGLUbest, j);
                if (k2 >= 0 && TriangulatorGLUbest.getAngle(k) < 0)
                    NoHash.deleteReflexVertex(TriangulatorGLUbest, k);
            }
            else if (j2 >= 0 && TriangulatorGLUbest.getAngle(j) < 0)
                NoHash.deleteReflexVertex(TriangulatorGLUbest, j);
            else if (k2 >= 0 && TriangulatorGLUbest.getAngle(k) < 0)
                NoHash.deleteReflexVertex(TriangulatorGLUbest, k);
            TriangulatorGLUbest.setAngle(j, j2);
            TriangulatorGLUbest.setAngle(k, k2);
            if (j2 > 0 && isEar(TriangulatorGLUbest, j, ai, ai2, ad))
                Heap.insertIntoHeap(TriangulatorGLUbest, ad[0], j, ai[0], ai2[0]);
            if (k2 > 0 && isEar(TriangulatorGLUbest, k, ai2, ai4, ad))
                Heap.insertIntoHeap(TriangulatorGLUbest, ad[0], k, ai2[0], ai4[0]);
            i = TriangulatorGLUbest.fetchPrevData(j);
            i1 = TriangulatorGLUbest.fetchData(i);
            l = TriangulatorGLUbest.fetchNextData(k);
            i2 = TriangulatorGLUbest.fetchData(l);
            if (i == l){
                TriangulatorGLUbest.storeTriangle(j, k, l);
                aflag[0] = true;
            }
            else{
                aflag[0] = false;
            }
            return true;
        }
    }

    public class Desperate {
        public Desperate() {}

        public static bool desperate(TriangulatorGLUbest TriangulatorGLUbest, int i, int j, bool[] aflag)
        {
            int[] ai = new int[1];
            int[] ai1 = new int[1];
            int[] ai2 = new int[1];
            int[] ai3 = new int[1];
            int[] ai4 = new int[1];
            int[] ai5 = new int[1];
            int[] ai6 = new int[1];
            int[] ai7 = new int[1];
            aflag[0] = false;
            if (existsCrossOver(TriangulatorGLUbest, i, ai4, ai, ai5, ai1, ai6, ai2, ai7, ai3)){
                handleCrossOver(TriangulatorGLUbest, ai4[0], ai[0], ai5[0], ai1[0], ai6[0], ai2[0], ai7[0], ai3[0]);
                return false;
            }
            NoHash.prepareNoHashEdges(TriangulatorGLUbest, j, j + 1);
            if (existsSplit(TriangulatorGLUbest, i, ai4, ai, ai5, ai1)){
                handleSplit(TriangulatorGLUbest, ai4[0], ai[0], ai5[0], ai1[0]);
                aflag[0] = true;
                return false;
            }
            else{
                return true;
            }
        }

        public static bool existsCrossOver(TriangulatorGLUbest TriangulatorGLUbest, int i, int[] ai, int[] ai1,
                                           int[] ai2, int[] ai3, int[] ai4, int[] ai5,
                                           int[] ai6, int[] ai7)
        {
            ai[0] = i;
            ai1[0] = TriangulatorGLUbest.fetchData(ai[0]);
            ai2[0] = TriangulatorGLUbest.fetchNextData(ai[0]);
            ai3[0] = TriangulatorGLUbest.fetchData(ai2[0]);
            ai4[0] = TriangulatorGLUbest.fetchNextData(ai2[0]);
            ai5[0] = TriangulatorGLUbest.fetchData(ai4[0]);
            ai6[0] = TriangulatorGLUbest.fetchNextData(ai4[0]);
            ai7[0] = TriangulatorGLUbest.fetchData(ai6[0]);
            do{
                BBox bbox = new BBox(TriangulatorGLUbest, ai1[0], ai3[0]);
                BBox bbox1 = new BBox(TriangulatorGLUbest, ai5[0], ai7[0]);
                if (bbox.BBoxOverlap(bbox1) &&
                    Numerics.segIntersect(TriangulatorGLUbest, bbox.imin, bbox.imax, bbox1.imin, bbox1.imax, -1))
                    return true;
                ai[0] = ai2[0];
                ai1[0] = ai3[0];
                ai2[0] = ai4[0];
                ai3[0] = ai5[0];
                ai4[0] = ai6[0];
                ai5[0] = ai7[0];
                ai6[0] = TriangulatorGLUbest.fetchNextData(ai4[0]);
                ai7[0] = TriangulatorGLUbest.fetchData(ai6[0]);
            } while (ai[0] != i);
            return false;
        }

        public static void handleCrossOver(TriangulatorGLUbest TriangulatorGLUbest, int i, int j, int k, int l, int i1,
                                           int j1, int k1,
                                           int l1)
        {
            int i2 = TriangulatorGLUbest.getAngle(i);
            int j2 = TriangulatorGLUbest.getAngle(k1);
            bool flag;
            if (i2 < j2)
                flag = true;
            else if (i2 > j2)
                flag = false;
            else if (TriangulatorGLUbest.earsSorted){
                double d = Numerics.getRatio(TriangulatorGLUbest, j1, l1, j);
                double d1 = Numerics.getRatio(TriangulatorGLUbest, j, l, l1);
                if (d1 < d)
                    flag = false;
                else
                    flag = true;
            }
            else{
                flag = true;
            }
            if (flag){
                TriangulatorGLUbest.deleteLinks(k);
                TriangulatorGLUbest.storeTriangle(i, k, i1);
                TriangulatorGLUbest.setAngle(i1, 1);
                Heap.insertIntoHeap(TriangulatorGLUbest, 0.0D, i1, i, k1);
            }
            else{
                TriangulatorGLUbest.deleteLinks(i1);
                TriangulatorGLUbest.storeTriangle(k, i1, k1);
                TriangulatorGLUbest.setAngle(k, 1);
                Heap.insertIntoHeap(TriangulatorGLUbest, 0.0D, k, i, k1);
            }
        }

        public static bool letsHope(TriangulatorGLUbest TriangulatorGLUbest, int i)
        {
            int l = i;
            int i2 = TriangulatorGLUbest.fetchData(l);
            do{
                if (TriangulatorGLUbest.getAngle(l) > 0){
                    int j = TriangulatorGLUbest.fetchPrevData(l);
                    int k1 = TriangulatorGLUbest.fetchData(j);
                    int i1 = TriangulatorGLUbest.fetchNextData(l);
                    int j2 = TriangulatorGLUbest.fetchData(i1);
                    Heap.insertIntoHeap(TriangulatorGLUbest, 0.0D, l, j, i1);
                    return true;
                }
                l = TriangulatorGLUbest.fetchNextData(l);
                i2 = TriangulatorGLUbest.fetchData(l);
            } while (l != i);
            TriangulatorGLUbest.setAngle(i, 1);
            int k = TriangulatorGLUbest.fetchPrevData(i);
            int l1 = TriangulatorGLUbest.fetchData(k);
            int j1 = TriangulatorGLUbest.fetchNextData(i);
            int k2 = TriangulatorGLUbest.fetchData(j1);
            Heap.insertIntoHeap(TriangulatorGLUbest, 0.0D, i, k, j1);
            i2 = TriangulatorGLUbest.fetchData(i);
            return true;
        }

        public static bool existsSplit(TriangulatorGLUbest TriangulatorGLUbest, int i, int[] ai, int[] ai1, int[] ai2,
                                       int[] ai3)
        {
            if (TriangulatorGLUbest.numPoints > TriangulatorGLUbest.maxNumDist){
                TriangulatorGLUbest.maxNumDist = TriangulatorGLUbest.numPoints;
                TriangulatorGLUbest.distances = new Distance[TriangulatorGLUbest.maxNumDist];
                for (int i2 = 0; i2 < TriangulatorGLUbest.maxNumDist; i2++)
                    TriangulatorGLUbest.distances[i2] = new Distance();
            }
            ai[0] = i;
            ai1[0] = TriangulatorGLUbest.fetchData(ai[0]);
            int k = TriangulatorGLUbest.fetchNextData(ai[0]);
            int j1 = TriangulatorGLUbest.fetchData(k);
            int l = TriangulatorGLUbest.fetchNextData(k);
            int k1 = TriangulatorGLUbest.fetchData(l);
            int j = TriangulatorGLUbest.fetchPrevData(ai[0]);
            int i1 = TriangulatorGLUbest.fetchData(j);
            if (foundSplit(TriangulatorGLUbest, l, k1, j, ai[0], ai1[0], i1, j1, ai2, ai3))
                return true;
            i1 = ai1[0];
            ai[0] = k;
            ai1[0] = j1;
            k = l;
            j1 = k1;
            l = TriangulatorGLUbest.fetchNextData(k);
            for (int l1 = TriangulatorGLUbest.fetchData(l); l != i; l1 = TriangulatorGLUbest.fetchData(l)){
                if (foundSplit(TriangulatorGLUbest, l, l1, i, ai[0], ai1[0], i1, j1, ai2, ai3))
                    return true;
                i1 = ai1[0];
                ai[0] = k;
                ai1[0] = j1;
                k = l;
                j1 = l1;
                l = TriangulatorGLUbest.fetchNextData(k);
            }

            return false;
        }

        public static int windingNumber(TriangulatorGLUbest TriangulatorGLUbest, int i, Point2f point2f)
        {
            int k = TriangulatorGLUbest.fetchData(i);
            int j = TriangulatorGLUbest.fetchNextData(i);
            int l = TriangulatorGLUbest.fetchData(j);
            double d;
            for (
                d =
                Numerics.angle(TriangulatorGLUbest, point2f, TriangulatorGLUbest.points[k],
                               TriangulatorGLUbest.points[l]);
                j != i;
                d +=
                Numerics.angle(TriangulatorGLUbest, point2f, TriangulatorGLUbest.points[k],
                               TriangulatorGLUbest.points[l])){
                k = l;
                j = TriangulatorGLUbest.fetchNextData(j);
                l = TriangulatorGLUbest.fetchData(j);
            }

            d += 3.1415926535897931D;
            int i1 = (int) (d/6.2831853071795862D);
            return i1;
        }

        public static bool foundSplit(TriangulatorGLUbest TriangulatorGLUbest, int i, int j, int k, int l, int i1,
                                      int j1, int k1,
                                      int[] ai, int[] ai1)
        {
            int l1 = 0;
            do{
                TriangulatorGLUbest.distances[l1].dist = Numerics.baseLength(TriangulatorGLUbest.points[i1],
                                                                             TriangulatorGLUbest.points[j]);
                TriangulatorGLUbest.distances[l1].ind = i;
                l1++;
                i = TriangulatorGLUbest.fetchNextData(i);
                j = TriangulatorGLUbest.fetchData(i);
            } while (i != k);
            Bridge.sortDistance(TriangulatorGLUbest.distances, l1);
            for (int i2 = 0; i2 < l1; i2++){
                ai[0] = TriangulatorGLUbest.distances[i2].ind;
                ai1[0] = TriangulatorGLUbest.fetchData(ai[0]);
                if (i1 == ai1[0])
                    continue;
                int l2 = TriangulatorGLUbest.fetchPrevData(ai[0]);
                int j2 = TriangulatorGLUbest.fetchData(l2);
                int i3 = TriangulatorGLUbest.fetchNextData(ai[0]);
                int k2 = TriangulatorGLUbest.fetchData(i3);
                bool flag = TriangulatorGLUbest.getAngle(ai[0]) > 0;
                bool flag1 = Numerics.isInCone(TriangulatorGLUbest, j2, ai1[0], k2, i1, flag);
                if (!flag1)
                    continue;
                flag = TriangulatorGLUbest.getAngle(l) > 0;
                flag1 = Numerics.isInCone(TriangulatorGLUbest, j1, i1, k1, ai1[0], flag);
                if (!flag1)
                    continue;
                BBox bbox = new BBox(TriangulatorGLUbest, i1, ai1[0]);
                if (NoHash.noHashEdgeIntersectionExists(TriangulatorGLUbest, bbox, -1, -1, l, -1))
                    continue;
                Point2f point2f = new Point2f();
                Basic.vectorAdd2D(TriangulatorGLUbest.points[i1], TriangulatorGLUbest.points[ai1[0]], point2f);
                Basic.multScalar2D(0.5D, point2f);
                if (windingNumber(TriangulatorGLUbest, k, point2f) == 1)
                    return true;
            }

            return false;
        }

        public static void handleSplit(TriangulatorGLUbest TriangulatorGLUbest, int i, int j, int k, int l)
        {
            int l2 = -1;
            int i1 = TriangulatorGLUbest.makeNode(j);
            TriangulatorGLUbest.insertAfter(i, i1);
            l2 = TriangulatorGLUbest.list[i].getCommonIndex();
            TriangulatorGLUbest.list[i1].setCommonIndex(l2);
            int j1 = TriangulatorGLUbest.makeNode(l);
            TriangulatorGLUbest.insertAfter(k, j1);
            l2 = TriangulatorGLUbest.list[k].getCommonIndex();
            TriangulatorGLUbest.list[j1].setCommonIndex(l2);
            TriangulatorGLUbest.splitSplice(i, i1, k, j1);
            TriangulatorGLUbest.storeChain(i);
            TriangulatorGLUbest.storeChain(k);
            int l1 = TriangulatorGLUbest.fetchNextData(i);
            int j2 = TriangulatorGLUbest.fetchData(l1);
            int k1 = TriangulatorGLUbest.fetchPrevData(i);
            int i2 = TriangulatorGLUbest.fetchData(k1);
            int k2 = Numerics.isConvexAngle(TriangulatorGLUbest, i2, j, j2, i);
            TriangulatorGLUbest.setAngle(i, k2);
            l1 = TriangulatorGLUbest.fetchNextData(i1);
            j2 = TriangulatorGLUbest.fetchData(l1);
            k1 = TriangulatorGLUbest.fetchPrevData(i1);
            i2 = TriangulatorGLUbest.fetchData(k1);
            k2 = Numerics.isConvexAngle(TriangulatorGLUbest, i2, j, j2, i1);
            TriangulatorGLUbest.setAngle(i1, k2);
            l1 = TriangulatorGLUbest.fetchNextData(k);
            j2 = TriangulatorGLUbest.fetchData(l1);
            k1 = TriangulatorGLUbest.fetchPrevData(k);
            i2 = TriangulatorGLUbest.fetchData(k1);
            k2 = Numerics.isConvexAngle(TriangulatorGLUbest, i2, l, j2, k);
            TriangulatorGLUbest.setAngle(k, k2);
            l1 = TriangulatorGLUbest.fetchNextData(j1);
            j2 = TriangulatorGLUbest.fetchData(l1);
            k1 = TriangulatorGLUbest.fetchPrevData(j1);
            i2 = TriangulatorGLUbest.fetchData(k1);
            k2 = Numerics.isConvexAngle(TriangulatorGLUbest, i2, l, j2, j1);
            TriangulatorGLUbest.setAngle(j1, k2);
        }
    }

    public class NoHash {
        public NoHash() {}

        public static void insertAfterVtx(TriangulatorGLUbest TriangulatorGLUbest, int i)
        {
            if (TriangulatorGLUbest.vtxList == null){
                int j = Math.Max(TriangulatorGLUbest.numVtxList + 1, 100);
                TriangulatorGLUbest.vtxList = new PntNode[j];
            }
            else if (TriangulatorGLUbest.numVtxList >= TriangulatorGLUbest.vtxList.Length){
                int k = Math.Max(TriangulatorGLUbest.numVtxList + 1, TriangulatorGLUbest.vtxList.Length + 100);
                PntNode[] apntnode = TriangulatorGLUbest.vtxList;
                TriangulatorGLUbest.vtxList = new PntNode[k];
                System.Array.Copy(apntnode, 0, TriangulatorGLUbest.vtxList, 0, apntnode.Length);
            }
            TriangulatorGLUbest.vtxList[TriangulatorGLUbest.numVtxList] = new PntNode();
            TriangulatorGLUbest.vtxList[TriangulatorGLUbest.numVtxList].pnt = i;
            TriangulatorGLUbest.vtxList[TriangulatorGLUbest.numVtxList].next = TriangulatorGLUbest.reflexVertices;
            TriangulatorGLUbest.reflexVertices = TriangulatorGLUbest.numVtxList;
            TriangulatorGLUbest.numVtxList++;
            TriangulatorGLUbest.numReflex++;
        }

        public static void deleteFromList(TriangulatorGLUbest TriangulatorGLUbest, int i)
        {
            if (TriangulatorGLUbest.numReflex == 0)
                return;
            int j = TriangulatorGLUbest.reflexVertices;
            if (!inVtxList(TriangulatorGLUbest, j))
                Console.WriteLine("NoHash:deleteFromList. Problem :Not is InVtxList ..." + j);
            int l = TriangulatorGLUbest.vtxList[j].pnt;
            if (l == i){
                TriangulatorGLUbest.reflexVertices = TriangulatorGLUbest.vtxList[j].next;
                TriangulatorGLUbest.numReflex--;
            }
            else{
                for (int k = TriangulatorGLUbest.vtxList[j].next; k != -1;){
                    if (!inVtxList(TriangulatorGLUbest, k))
                        Console.WriteLine("NoHash:deleteFromList. Problem :Not is InVtxList ..." + k);
                    int i1 = TriangulatorGLUbest.vtxList[k].pnt;
                    if (i1 == i){
                        TriangulatorGLUbest.vtxList[j].next = TriangulatorGLUbest.vtxList[k].next;
                        k = -1;
                        TriangulatorGLUbest.numReflex--;
                    }
                    else{
                        j = k;
                        k = TriangulatorGLUbest.vtxList[j].next;
                    }
                }
            }
        }

        public static bool inVtxList(TriangulatorGLUbest TriangulatorGLUbest, int i)
        {
            return 0 <= i && i < TriangulatorGLUbest.numVtxList;
        }

        public static void freeNoHash(TriangulatorGLUbest TriangulatorGLUbest)
        {
            TriangulatorGLUbest.noHashingEdges = false;
            TriangulatorGLUbest.noHashingPnts = false;
            TriangulatorGLUbest.numVtxList = 0;
        }

        public static void prepareNoHashEdges(TriangulatorGLUbest TriangulatorGLUbest, int i, int j)
        {
            TriangulatorGLUbest.loopMin = i;
            TriangulatorGLUbest.loopMax = j;
            TriangulatorGLUbest.noHashingEdges = true;
        }

        public static void prepareNoHashPnts(TriangulatorGLUbest TriangulatorGLUbest, int i)
        {
            TriangulatorGLUbest.numVtxList = 0;
            TriangulatorGLUbest.reflexVertices = -1;
            int j = TriangulatorGLUbest.loops[i];
            int k = j;
            TriangulatorGLUbest.numReflex = 0;
            int l = TriangulatorGLUbest.fetchData(k);
            do{
                if (TriangulatorGLUbest.getAngle(k) < 0)
                    insertAfterVtx(TriangulatorGLUbest, k);
                k = TriangulatorGLUbest.fetchNextData(k);
                int i1 = TriangulatorGLUbest.fetchData(k);
            } while (k != j);
            TriangulatorGLUbest.noHashingPnts = true;
        }

        public static bool noHashIntersectionExists(TriangulatorGLUbest TriangulatorGLUbest, int i, int j, int k, int l,
                                                    BBox bbox)
        {
            int[] ai = new int[1];
            if (!TriangulatorGLUbest.noHashingPnts)
                Console.WriteLine("NoHash:noHashIntersectionExists noHashingPnts is false");
            if (TriangulatorGLUbest.numReflex <= 0)
                return false;
            if (i < bbox.imin)
                bbox.imin = i;
            else if (i > bbox.imax)
                bbox.imax = i;
            double d = TriangulatorGLUbest.points[i].y;
            if (d < bbox.ymin)
                bbox.ymin = d;
            else if (d > bbox.ymax)
                bbox.ymax = d;
            int k1 = TriangulatorGLUbest.reflexVertices;
            bool flag = false;
            do{
                int i1 = TriangulatorGLUbest.vtxList[k1].pnt;
                int l1 = TriangulatorGLUbest.fetchData(i1);
                if (bbox.pntInBBox(TriangulatorGLUbest, l1)){
                    int j1 = TriangulatorGLUbest.fetchNextData(i1);
                    int i2 = TriangulatorGLUbest.fetchData(j1);
                    if (i1 != j && i1 != j1)
                        if (l1 == i){
                            if (Degenerate.handleDegeneracies(TriangulatorGLUbest, i, j, k, l, l1, i1))
                                return true;
                        }
                        else if (l1 != k && l1 != l){
                            bool flag1 = Numerics.vtxInTriangle(TriangulatorGLUbest, i, k, l, l1, ai);
                            if (flag1)
                                return true;
                        }
                }
                k1 = TriangulatorGLUbest.vtxList[k1].next;
            } while (k1 != -1);
            return false;
        }

        public static void deleteReflexVertex(TriangulatorGLUbest TriangulatorGLUbest, int i)
        {
            deleteFromList(TriangulatorGLUbest, i);
        }

        public static bool noHashEdgeIntersectionExists(TriangulatorGLUbest TriangulatorGLUbest, BBox bbox, int i, int j,
                                                        int k, int l)
        {
            if (!TriangulatorGLUbest.noHashingEdges)
                Console.WriteLine("NoHash:noHashEdgeIntersectionExists noHashingEdges is false");
            TriangulatorGLUbest.identCntr = 0;
            for (int k1 = TriangulatorGLUbest.loopMin; k1 < TriangulatorGLUbest.loopMax; k1++){
                int i1 = TriangulatorGLUbest.loops[k1];
                int j1 = i1;
                int l1 = TriangulatorGLUbest.fetchData(j1);
                do{
                    j1 = TriangulatorGLUbest.fetchNextData(j1);
                    int i2 = TriangulatorGLUbest.fetchData(j1);
                    BBox bbox1 = new BBox(TriangulatorGLUbest, l1, i2);
                    if (bbox.BBoxOverlap(bbox1) &&
                        Numerics.segIntersect(TriangulatorGLUbest, bbox.imin, bbox.imax, bbox1.imin, bbox1.imax, l))
                        return true;
                    l1 = i2;
                } while (j1 != i1);
            }

            if (TriangulatorGLUbest.identCntr >= 4)
                return BottleNeck.checkBottleNeck(TriangulatorGLUbest, l, i, j, k);
            else
                return false;
        }

        public static int NIL = -1;
    }

    public class Orientation {
        public Orientation() {}

        public static void adjustOrientation(TriangulatorGLUbest TriangulatorGLUbest, int i, int j)
        {
            if (i >= j)
                Console.WriteLine("Orientation:adjustOrientation Problem i1>=i2 !!!");
            if (TriangulatorGLUbest.numLoops >= TriangulatorGLUbest.maxNumPolyArea){
                TriangulatorGLUbest.maxNumPolyArea = TriangulatorGLUbest.numLoops;
                double[] ad = TriangulatorGLUbest.polyArea;
                TriangulatorGLUbest.polyArea = new double[TriangulatorGLUbest.maxNumPolyArea];
                if (ad != null)
                    System.Array.Copy(ad, 0, TriangulatorGLUbest.polyArea, 0, ad.Length);
            }
            for (int k = i; k < j; k++){
                int k1 = TriangulatorGLUbest.loops[k];
                TriangulatorGLUbest.polyArea[k] = polygonArea(TriangulatorGLUbest, k1);
            }

            double d = Math.Abs(TriangulatorGLUbest.polyArea[i]);
            int j1 = i;
            for (int l = i + 1; l < j; l++)
                if (d < Math.Abs(TriangulatorGLUbest.polyArea[l])){
                    d = Math.Abs(TriangulatorGLUbest.polyArea[l]);
                    j1 = l;
                }

            if (j1 != i){
                int l1 = TriangulatorGLUbest.loops[i];
                TriangulatorGLUbest.loops[i] = TriangulatorGLUbest.loops[j1];
                TriangulatorGLUbest.loops[j1] = l1;
                double d1 = TriangulatorGLUbest.polyArea[i];
                TriangulatorGLUbest.polyArea[i] = TriangulatorGLUbest.polyArea[j1];
                TriangulatorGLUbest.polyArea[j1] = d1;
            }
            if (TriangulatorGLUbest.polyArea[i] < 0.0D)
                TriangulatorGLUbest.swapLinks(TriangulatorGLUbest.loops[i]);
            for (int i1 = i + 1; i1 < j; i1++)
                if (TriangulatorGLUbest.polyArea[i1] > 0.0D)
                    TriangulatorGLUbest.swapLinks(TriangulatorGLUbest.loops[i1]);
        }

        public static double polygonArea(TriangulatorGLUbest TriangulatorGLUbest, int i)
        {
            int j = 0;
            double d = 0.0D;
            double d1 = 0.0D;
            int k = i;
            int j1 = TriangulatorGLUbest.fetchData(k);
            int l = TriangulatorGLUbest.fetchNextData(k);
            int l1 = TriangulatorGLUbest.fetchData(l);
            d = Numerics.stableDet2D(TriangulatorGLUbest, j, j1, l1);
            k = l;
            for (int k1 = l1; k != i; k1 = l1){
                int i1 = TriangulatorGLUbest.fetchNextData(k);
                l1 = TriangulatorGLUbest.fetchData(i1);
                double d2 = Numerics.stableDet2D(TriangulatorGLUbest, j, k1, l1);
                d += d2;
                k = i1;
            }

            return d;
        }

        public static void determineOrientation(TriangulatorGLUbest TriangulatorGLUbest, int i)
        {
            double d = polygonArea(TriangulatorGLUbest, i);
            if (d < 0.0D){
                TriangulatorGLUbest.swapLinks(i);
                TriangulatorGLUbest.ccwLoop = false;
            }
        }
    }

    public class BBox {
        public BBox(TriangulatorGLUbest TriangulatorGLUbest, int i, int j)
        {
            imin = Math.Min(i, j);
            imax = Math.Max(i, j);
            ymin = Math.Min(TriangulatorGLUbest.points[imin].y, TriangulatorGLUbest.points[imax].y);
            ymax = Math.Max(TriangulatorGLUbest.points[imin].y, TriangulatorGLUbest.points[imax].y);
        }

        public bool pntInBBox(TriangulatorGLUbest TriangulatorGLUbest, int i)
        {
            return imax >= i
                       ? imin <= i
                             ? ymax >= (double) TriangulatorGLUbest.points[i].y
                                   ? ymin <= (double) TriangulatorGLUbest.points[i].y
                                   : false
                             : false
                       : false;
        }

        public bool BBoxOverlap(BBox bbox)
        {
            return imax >= bbox.imin ? imin <= bbox.imax ? ymax >= bbox.ymin ? ymin <= bbox.ymax : false : false : false;
        }

        public bool BBoxContained(BBox bbox)
        {
            return imin <= bbox.imin && imax >= bbox.imax && ymin <= bbox.ymin && ymax >= bbox.ymax;
        }

        public bool BBoxIdenticalLeaf(BBox bbox)
        {
            return imin == bbox.imin && imax == bbox.imax;
        }

        public void BBoxUnion(BBox bbox, BBox bbox1)
        {
            bbox1.imin = Math.Min(imin, bbox.imin);
            bbox1.imax = Math.Max(imax, bbox.imax);
            bbox1.ymin = Math.Min(ymin, bbox.ymin);
            bbox1.ymax = Math.Max(ymax, bbox.ymax);
        }

        public double BBoxArea(TriangulatorGLUbest TriangulatorGLUbest)
        {
            return (double) (TriangulatorGLUbest.points[imax].x - TriangulatorGLUbest.points[imin].x)*(ymax - ymin);
        }

        public int imin;
        public int imax;
        public double ymin;
        public double ymax;
    }

    public class Project {
        public Project() {}

        public static void projectFace(TriangulatorGLUbest TriangulatorGLUbest, int i, int j)
        {
            Vector3f vector3f = new Vector3f();
            Vector3f vector3f1 = new Vector3f();
            determineNormal(TriangulatorGLUbest, TriangulatorGLUbest.loops[i], vector3f);
            int l = i + 1;
            if (l < j){
                for (int k = l; k < j; k++){
                    determineNormal(TriangulatorGLUbest, TriangulatorGLUbest.loops[k], vector3f1);
                    if (Basic.dotProduct(vector3f, vector3f1) < 0.0D)
                        Basic.invertVector(vector3f1);
                    Basic.vectorAdd(vector3f, vector3f1, vector3f);
                }

                double d = Basic.lengthL2(vector3f);
                if (Numerics.gt(d, 1E-008D)){
                    Basic.divScalar(d, vector3f);
                }
                else{
                    vector3f.x = vector3f.y = 0.0F;
                    vector3f.z = 1.0F;
                }
            }
            projectPoints(TriangulatorGLUbest, i, j, vector3f);
        }

        public static void determineNormal(TriangulatorGLUbest TriangulatorGLUbest, int i, Vector3f vector3f)
        {
            int k = i;
            int j1 = TriangulatorGLUbest.fetchData(k);
            int j = TriangulatorGLUbest.fetchPrevData(k);
            int i1 = TriangulatorGLUbest.fetchData(j);
            int l = TriangulatorGLUbest.fetchNextData(k);
            int k1 = TriangulatorGLUbest.fetchData(l);
            Vector3f vector3f2 = new Vector3f();
            Basic.vectorSub(TriangulatorGLUbest.vertices[i1], TriangulatorGLUbest.vertices[j1], vector3f2);
            Vector3f vector3f3 = new Vector3f();
            Basic.vectorSub(TriangulatorGLUbest.vertices[k1], TriangulatorGLUbest.vertices[j1], vector3f3);
            Vector3f vector3f1 = new Vector3f();
            Basic.vectorProduct(vector3f2, vector3f3, vector3f1);
            double d = Basic.lengthL2(vector3f1);
            if (Numerics.gt(d, 1E-008D)){
                Basic.divScalar(d, vector3f1);
                vector3f.set(vector3f1);
            }
            else{
                vector3f.x = vector3f.y = vector3f.z = 0.0F;
            }
            vector3f2.set(vector3f3);
            k = l;
            l = TriangulatorGLUbest.fetchNextData(k);
            for (int l1 = TriangulatorGLUbest.fetchData(l); k != i; l1 = TriangulatorGLUbest.fetchData(l)){
                Basic.vectorSub(TriangulatorGLUbest.vertices[l1], TriangulatorGLUbest.vertices[j1], vector3f3);
                Basic.vectorProduct(vector3f2, vector3f3, vector3f1);
                d = Basic.lengthL2(vector3f1);
                if (Numerics.gt(d, 1E-008D)){
                    Basic.divScalar(d, vector3f1);
                    if (Basic.dotProduct(vector3f, vector3f1) < 0.0D)
                        Basic.invertVector(vector3f1);
                    Basic.vectorAdd(vector3f, vector3f1, vector3f);
                }
                vector3f2.set(vector3f3);
                k = l;
                l = TriangulatorGLUbest.fetchNextData(k);
            }

            d = Basic.lengthL2(vector3f);
            if (Numerics.gt(d, 1E-008D)){
                Basic.divScalar(d, vector3f);
            }
            else{
                vector3f.x = vector3f.y = 0.0F;
                vector3f.z = 1.0F;
            }
        }

        public static void projectPoints(TriangulatorGLUbest TriangulatorGLUbest, int i, int j, Vector3f vector3f)
        {
            Matrix4f matrix4f = new Matrix4f();
            Point3f point3f = new Point3f();
            Vector3f vector3f1 = new Vector3f();
            Vector3f vector3f2 = new Vector3f();
            if ((double) Math.Abs(vector3f.x) > 0.10000000000000001D ||
                (double) Math.Abs(vector3f.y) > 0.10000000000000001D){
                vector3f1.x = -vector3f.y;
                vector3f1.y = vector3f.x;
                vector3f1.z = 0.0F;
            }
            else{
                vector3f1.x = vector3f.z;
                vector3f1.z = -vector3f.x;
                vector3f1.y = 0.0F;
            }
            double d = Basic.lengthL2(vector3f1);
            Basic.divScalar(d, vector3f1);
            Basic.vectorProduct(vector3f1, vector3f, vector3f2);
            d = Basic.lengthL2(vector3f2);
            Basic.divScalar(d, vector3f2);
            matrix4f.m00 = vector3f1.x;
            matrix4f.m01 = vector3f1.y;
            matrix4f.m02 = vector3f1.z;
            matrix4f.m03 = 0.0F;
            matrix4f.m10 = vector3f2.x;
            matrix4f.m11 = vector3f2.y;
            matrix4f.m12 = vector3f2.z;
            matrix4f.m13 = 0.0F;
            matrix4f.m20 = vector3f.x;
            matrix4f.m21 = vector3f.y;
            matrix4f.m22 = vector3f.z;
            matrix4f.m23 = 0.0F;
            matrix4f.m30 = 0.0F;
            matrix4f.m31 = 0.0F;
            matrix4f.m32 = 0.0F;
            matrix4f.m33 = 1.0F;
            TriangulatorGLUbest.initPnts(20);
            for (int i1 = i; i1 < j; i1++){
                int k = TriangulatorGLUbest.loops[i1];
                int l = k;
                int j1 = TriangulatorGLUbest.fetchData(l);
                matrix4f.transform(TriangulatorGLUbest.vertices[j1], point3f);
                j1 = TriangulatorGLUbest.storePoint(point3f.x, point3f.y);
                TriangulatorGLUbest.updateIndex(l, j1);
                l = TriangulatorGLUbest.fetchNextData(l);
                for (int k1 = TriangulatorGLUbest.fetchData(l); l != k; k1 = TriangulatorGLUbest.fetchData(l)){
                    matrix4f.transform(TriangulatorGLUbest.vertices[k1], point3f);
                    k1 = TriangulatorGLUbest.storePoint(point3f.x, point3f.y);
                    TriangulatorGLUbest.updateIndex(l, k1);
                    l = TriangulatorGLUbest.fetchNextData(l);
                }
            }
        }
    }

    public class Clean {
        public Clean() {}

        public static void initPUnsorted(TriangulatorGLUbest TriangulatorGLUbest, int i)
        {
            if (i > TriangulatorGLUbest.maxNumPUnsorted){
                TriangulatorGLUbest.maxNumPUnsorted = i;
                TriangulatorGLUbest.pUnsorted = new Point2f[TriangulatorGLUbest.maxNumPUnsorted];
                for (int j = 0; j < TriangulatorGLUbest.maxNumPUnsorted; j++)
                    TriangulatorGLUbest.pUnsorted[j] = new Point2f();
            }
        }

        public static int cleanPolyhedralFace(TriangulatorGLUbest TriangulatorGLUbest, int i, int j)
        {
            initPUnsorted(TriangulatorGLUbest, TriangulatorGLUbest.numPoints);
            for (int l = 0; l < TriangulatorGLUbest.numPoints; l++)
                TriangulatorGLUbest.pUnsorted[l].set(TriangulatorGLUbest.points[l]);

            sort(TriangulatorGLUbest.points, TriangulatorGLUbest.numPoints);
            int i1 = 0;
            for (int k1 = 1; k1 < TriangulatorGLUbest.numPoints; k1++)
                if (pComp(TriangulatorGLUbest.points[i1], TriangulatorGLUbest.points[k1]) != 0){
                    i1++;
                    TriangulatorGLUbest.points[i1] = TriangulatorGLUbest.points[k1];
                }

            int j2 = i1 + 1;
            int k = TriangulatorGLUbest.numPoints - j2;
            for (int j1 = i; j1 < j; j1++){
                int l2 = TriangulatorGLUbest.loops[j1];
                int i3 = TriangulatorGLUbest.fetchNextData(l2);
                int k2;
                for (k2 = TriangulatorGLUbest.fetchData(i3); i3 != l2; k2 = TriangulatorGLUbest.fetchData(i3)){
                    int l1 = findPInd(TriangulatorGLUbest.points, j2, TriangulatorGLUbest.pUnsorted[k2]);
                    TriangulatorGLUbest.updateIndex(i3, l1);
                    i3 = TriangulatorGLUbest.fetchNextData(i3);
                }

                int i2 = findPInd(TriangulatorGLUbest.points, j2, TriangulatorGLUbest.pUnsorted[k2]);
                TriangulatorGLUbest.updateIndex(i3, i2);
            }

            TriangulatorGLUbest.numPoints = j2;
            return k;
        }

        public static void sort(Point2f[] apoint2f, int i)
        {
            Point2f point2f = new Point2f();
            for (int j = 0; j < i; j++){
                for (int k = j + 1; k < i; k++)
                    if (pComp(apoint2f[j], apoint2f[k]) > 0){
                        point2f.set(apoint2f[j]);
                        apoint2f[j].set(apoint2f[k]);
                        apoint2f[k].set(point2f);
                    }
            }
        }

        public static int findPInd(Point2f[] apoint2f, int i, Point2f point2f)
        {
            for (int j = 0; j < i; j++)
                if (point2f.x == apoint2f[j].x && point2f.y == apoint2f[j].y)
                    return j;

            return -1;
        }

        public static int pComp(Point2f point2f, Point2f point2f1)
        {
            if (point2f.x < point2f1.x)
                return -1;
            if (point2f.x > point2f1.x)
                return 1;
            if (point2f.y < point2f1.y)
                return -1;
            return point2f.y <= point2f1.y ? 0 : 1;
        }
    }

    public class Numerics {
        public Numerics() {}

        public static double max3(double d, double d1, double d2)
        {
            return d <= d1 ? d1 <= d2 ? d2 : d1 : d <= d2 ? d2 : d;
        }

        public static double min3(double d, double d1, double d2)
        {
            return d >= d1 ? d1 >= d2 ? d2 : d1 : d >= d2 ? d2 : d;
        }

        public static bool lt(double d, double d1)
        {
            return d < -d1;
        }

        public static bool le(double d, double d1)
        {
            return d <= d1;
        }

        public static bool ge(double d, double d1)
        {
            return d > -d1;
        }

        public static bool eq(double d, double d1)
        {
            return d <= d1 && d >= -d1;
        }

        public static bool gt(double d, double d1)
        {
            return d > d1;
        }

        public static double baseLength(Tuple2f tuple2f, Tuple2f tuple2f1)
        {
            double d = tuple2f1.x - tuple2f.x;
            double d1 = tuple2f1.y - tuple2f.y;
            return Math.Abs(d) + Math.Abs(d1);
        }

        public static double sideLength(Tuple2f tuple2f, Tuple2f tuple2f1)
        {
            double d = tuple2f1.x - tuple2f.x;
            double d1 = tuple2f1.y - tuple2f.y;
            return d*d + d1*d1;
        }

        public static bool inBetween(int i, int j, int k)
        {
            return i <= k && k <= j;
        }

        public static bool strictlyInBetween(int i, int j, int k)
        {
            return i < k && k < j;
        }

        public static double stableDet2D(TriangulatorGLUbest TriangulatorGLUbest, int i, int j, int k)
        {
            double d;
            if (i == j || i == k || j == k){
                d = 0.0D;
            }
            else{
                Point2f point2f = TriangulatorGLUbest.points[i];
                Point2f point2f1 = TriangulatorGLUbest.points[j];
                Point2f point2f2 = TriangulatorGLUbest.points[k];
                if (i < j){
                    if (j < k)
                        d = Basic.det2D(point2f, point2f1, point2f2);
                    else if (i < k)
                        d = -Basic.det2D(point2f, point2f2, point2f1);
                    else
                        d = Basic.det2D(point2f2, point2f, point2f1);
                }
                else if (i < k)
                    d = -Basic.det2D(point2f1, point2f, point2f2);
                else if (j < k)
                    d = Basic.det2D(point2f1, point2f2, point2f);
                else
                    d = -Basic.det2D(point2f2, point2f1, point2f);
            }
            return d;
        }

        public static int orientation(TriangulatorGLUbest TriangulatorGLUbest, int i, int j, int k)
        {
            double d = stableDet2D(TriangulatorGLUbest, i, j, k);
            int byte0;
            if (lt(d, TriangulatorGLUbest.epsilon))
                byte0 = -1;
            else if (gt(d, TriangulatorGLUbest.epsilon))
                byte0 = 1;
            else
                byte0 = 0;
            return byte0;
        }

        public static bool isInCone(TriangulatorGLUbest TriangulatorGLUbest, int i, int j, int k, int l, bool flag)
        {
            bool flag1 = true;
            if (flag){
                if (i != j){
                    int i1 = orientation(TriangulatorGLUbest, i, j, l);
                    if (i1 < 0)
                        flag1 = false;
                    else if (i1 == 0)
                        if (i < j){
                            if (!inBetween(i, j, l))
                                flag1 = false;
                        }
                        else if (!inBetween(j, i, l))
                            flag1 = false;
                }
                if (j != k && flag1){
                    int k1 = orientation(TriangulatorGLUbest, j, k, l);
                    if (k1 < 0)
                        flag1 = false;
                    else if (k1 == 0)
                        if (j < k){
                            if (!inBetween(j, k, l))
                                flag1 = false;
                        }
                        else if (!inBetween(k, j, l))
                            flag1 = false;
                }
            }
            else{
                int j1 = orientation(TriangulatorGLUbest, i, j, l);
                if (j1 <= 0){
                    int l1 = orientation(TriangulatorGLUbest, j, k, l);
                    if (l1 < 0)
                        flag1 = false;
                }
            }
            return flag1;
        }

        public static int isConvexAngle(TriangulatorGLUbest TriangulatorGLUbest, int i, int j, int k, int l)
        {
            if (i == j)
                return j != k ? 1 : 1;
            if (j == k)
                return -1;
            int j1 = orientation(TriangulatorGLUbest, i, j, k);
            int i1;
            if (j1 > 0)
                i1 = 1;
            else if (j1 < 0){
                i1 = -1;
            }
            else{
                Point2f point2f = new Point2f();
                Point2f point2f1 = new Point2f();
                Basic.vectorSub2D(TriangulatorGLUbest.points[i], TriangulatorGLUbest.points[j], point2f);
                Basic.vectorSub2D(TriangulatorGLUbest.points[k], TriangulatorGLUbest.points[j], point2f1);
                double d = Basic.dotProduct2D(point2f, point2f1);
                if (d < 0.0D)
                    i1 = 0;
                else
                    i1 = spikeAngle(TriangulatorGLUbest, i, j, k, l);
            }
            return i1;
        }

        public static bool pntInTriangle(TriangulatorGLUbest TriangulatorGLUbest, int i, int j, int k, int l)
        {
            bool flag = false;
            int i1 = orientation(TriangulatorGLUbest, j, k, l);
            if (i1 >= 0){
                int j1 = orientation(TriangulatorGLUbest, i, j, l);
                if (j1 >= 0){
                    int k1 = orientation(TriangulatorGLUbest, k, i, l);
                    if (k1 >= 0)
                        flag = true;
                }
            }
            return flag;
        }

        public static bool vtxInTriangle(TriangulatorGLUbest TriangulatorGLUbest, int i, int j, int k, int l, int[] ai)
        {
            bool flag = false;
            int i1 = orientation(TriangulatorGLUbest, j, k, l);
            if (i1 >= 0){
                int j1 = orientation(TriangulatorGLUbest, i, j, l);
                if (j1 > 0){
                    j1 = orientation(TriangulatorGLUbest, k, i, l);
                    if (j1 > 0){
                        flag = true;
                        ai[0] = 0;
                    }
                    else if (j1 == 0){
                        flag = true;
                        ai[0] = 1;
                    }
                }
                else if (j1 == 0){
                    int k1 = orientation(TriangulatorGLUbest, k, i, l);
                    if (k1 > 0){
                        flag = true;
                        ai[0] = 2;
                    }
                    else if (k1 == 0){
                        flag = true;
                        ai[0] = 3;
                    }
                }
            }
            return flag;
        }

        public static bool segIntersect(TriangulatorGLUbest TriangulatorGLUbest, int i, int j, int k, int l, int i1)
        {
            if (i == j || k == l)
                return false;
            if (i == k && j == l)
                return true;
            if (k == i1 || l == i1)
                TriangulatorGLUbest.identCntr++;
            int l1 = orientation(TriangulatorGLUbest, i, j, k);
            int i2 = orientation(TriangulatorGLUbest, i, j, l);
            if (l1 == 1 && i2 == 1 || l1 == -1 && i2 == -1)
                return false;
            if (l1 == 0){
                if (strictlyInBetween(i, j, k))
                    return true;
                if (i2 == 0){
                    if (strictlyInBetween(i, j, l))
                        return true;
                }
                else{
                    return false;
                }
            }
            else if (i2 == 0)
                return strictlyInBetween(i, j, l);
            int j1 = orientation(TriangulatorGLUbest, k, l, i);
            int k1 = orientation(TriangulatorGLUbest, k, l, j);
            return (j1 > 0 || k1 > 0) && (j1 < 0 || k1 < 0);
        }

        public static double getRatio(TriangulatorGLUbest TriangulatorGLUbest, int i, int j, int k)
        {
            Point2f point2f = TriangulatorGLUbest.points[i];
            Point2f point2f1 = TriangulatorGLUbest.points[j];
            Point2f point2f2 = TriangulatorGLUbest.points[k];
            double d1 = baseLength(point2f, point2f1);
            double d2 = baseLength(point2f, point2f2);
            double d3 = baseLength(point2f2, point2f1);
            double d4 = max3(d1, d2, d3);
            if (10D*d1 < Math.Min(d2, d3))
                return 0.10000000000000001D;
            double d = stableDet2D(TriangulatorGLUbest, i, j, k);
            if (lt(d, TriangulatorGLUbest.epsilon))
                d = -d;
            else if (!gt(d, TriangulatorGLUbest.epsilon))
                return d4 <= d1 ? 1.7976931348623157E+308D : 0.10000000000000001D;
            double d5 = (d4*d4)/d;
            if (d5 < 10D)
                return d5;
            if (d1 < d4)
                return 0.10000000000000001D;
            else
                return d5;
        }

        public static int spikeAngle(TriangulatorGLUbest TriangulatorGLUbest, int i, int j, int k, int l)
        {
            int j1 = l;
            int i2 = TriangulatorGLUbest.fetchData(j1);
            int i1 = TriangulatorGLUbest.fetchPrevData(j1);
            int l1 = TriangulatorGLUbest.fetchData(i1);
            int k1 = TriangulatorGLUbest.fetchNextData(j1);
            int j2 = TriangulatorGLUbest.fetchData(k1);
            return recSpikeAngle(TriangulatorGLUbest, i, j, k, i1, k1);
        }

        public static int recSpikeAngle(TriangulatorGLUbest TriangulatorGLUbest, int i, int j, int k, int l, int i1)
        {
            if (l == i1)
                return -2;
            if (i != k){
                int k3;
                int l3;
                if (i < j){
                    k3 = i;
                    l3 = j;
                }
                else{
                    k3 = j;
                    l3 = i;
                }
                if (inBetween(k3, l3, k)){
                    j = k;
                    i1 = TriangulatorGLUbest.fetchNextData(i1);
                    k = TriangulatorGLUbest.fetchData(i1);
                    if (l == i1)
                        return 2;
                    int j1 = orientation(TriangulatorGLUbest, i, j, k);
                    if (j1 > 0)
                        return 2;
                    if (j1 < 0)
                        return -2;
                    else
                        return recSpikeAngle(TriangulatorGLUbest, i, j, k, l, i1);
                }
                j = i;
                l = TriangulatorGLUbest.fetchPrevData(l);
                i = TriangulatorGLUbest.fetchData(l);
                if (l == i1)
                    return 2;
                int k1 = orientation(TriangulatorGLUbest, i, j, k);
                if (k1 > 0)
                    return 2;
                if (k1 < 0)
                    return -2;
                else
                    return recSpikeAngle(TriangulatorGLUbest, i, j, k, l, i1);
            }
            int j3 = j;
            j = i;
            l = TriangulatorGLUbest.fetchPrevData(l);
            i = TriangulatorGLUbest.fetchData(l);
            if (l == i1)
                return 2;
            i1 = TriangulatorGLUbest.fetchNextData(i1);
            k = TriangulatorGLUbest.fetchData(i1);
            if (l == i1)
                return 2;
            int l1 = orientation(TriangulatorGLUbest, i, j, k);
            if (l1 > 0){
                int j2 = orientation(TriangulatorGLUbest, i, j, j3);
                if (j2 > 0){
                    int l2 = orientation(TriangulatorGLUbest, j, k, j3);
                    if (l2 > 0)
                        return -2;
                }
                return 2;
            }
            if (l1 < 0){
                int k2 = orientation(TriangulatorGLUbest, j, i, j3);
                if (k2 > 0){
                    int i3 = orientation(TriangulatorGLUbest, k, j, j3);
                    if (i3 > 0)
                        return 2;
                }
                return -2;
            }
            Point2f point2f = new Point2f();
            Basic.vectorSub2D(TriangulatorGLUbest.points[i], TriangulatorGLUbest.points[j], point2f);
            Point2f point2f1 = new Point2f();
            Basic.vectorSub2D(TriangulatorGLUbest.points[k], TriangulatorGLUbest.points[j], point2f1);
            double d = Basic.dotProduct2D(point2f, point2f1);
            if (d < 0.0D){
                int i2 = orientation(TriangulatorGLUbest, j, i, j3);
                return i2 <= 0 ? -2 : 2;
            }
            else{
                return recSpikeAngle(TriangulatorGLUbest, i, j, k, l, i1);
            }
        }

        public static double angle(TriangulatorGLUbest TriangulatorGLUbest, Point2f point2f, Point2f point2f1,
                                   Point2f point2f2)
        {
            int i = Basic.signEps(Basic.det2D(point2f2, point2f, point2f1), TriangulatorGLUbest.epsilon);
            if (i == 0)
                return 0.0D;
            Point2f point2f3 = new Point2f();
            Point2f point2f4 = new Point2f();
            Basic.vectorSub2D(point2f1, point2f, point2f3);
            Basic.vectorSub2D(point2f2, point2f, point2f4);
            double d = Math.Atan2(point2f3.y, point2f3.x);
            double d1 = Math.Atan2(point2f4.y, point2f4.x);
            if (d < 0.0D)
                d += 6.2831853071795862D;
            if (d1 < 0.0D)
                d1 += 6.2831853071795862D;
            double d2 = d - d1;
            if (d2 > 3.1415926535897931D)
                d2 = 6.2831853071795862D - d2;
            else if (d2 < -3.1415926535897931D)
                d2 = 6.2831853071795862D + d2;
            if (i == 1)
                if (d2 < 0.0D)
                    return -d2;
                else
                    return d2;
            if (d2 > 0.0D)
                return -d2;
            else
                return d2;
        }
    }

    public class Basic {
        public Basic() {}

        public static double detExp(double d, double d1, double d2, double d3,
                                    double d4, double d5, double d6, double d7, double d8)
        {
            return (d*(d4*d8 - d5*d7) - d1*(d3*d8 - d5*d6)) + d2*(d3*d7 - d4*d6);
        }

        public static double det3D(Tuple3f tuple3f, Tuple3f tuple3f1, Tuple3f tuple3f2)
        {
            return
                (double)
                ((tuple3f.x*(tuple3f1.y*tuple3f2.z - tuple3f1.z*tuple3f2.y) -
                  tuple3f.y*(tuple3f1.x*tuple3f2.z - tuple3f1.z*tuple3f2.x)) +
                 tuple3f.z*(tuple3f1.x*tuple3f2.y - tuple3f1.y*tuple3f2.x));
        }

        public static double det2D(Tuple2f tuple2f, Tuple2f tuple2f1, Tuple2f tuple2f2)
        {
            return
                (double)
                ((tuple2f.x - tuple2f1.x)*(tuple2f1.y - tuple2f2.y) + (tuple2f1.y - tuple2f.y)*(tuple2f1.x - tuple2f2.x));
        }

        public static double length2(Tuple3f tuple3f)
        {
            return (double) (tuple3f.x*tuple3f.x + tuple3f.y*tuple3f.y + tuple3f.z*tuple3f.z);
        }

        public static double lengthL1(Tuple3f tuple3f)
        {
            return (double) (Math.Abs(tuple3f.x) + Math.Abs(tuple3f.y) + Math.Abs(tuple3f.z));
        }

        public static double lengthL2(Tuple3f tuple3f)
        {
            return Math.Sqrt(tuple3f.x*tuple3f.x + tuple3f.y*tuple3f.y + tuple3f.z*tuple3f.z);
        }

        public static double dotProduct(Tuple3f tuple3f, Tuple3f tuple3f1)
        {
            return (double) (tuple3f.x*tuple3f1.x + tuple3f.y*tuple3f1.y + tuple3f.z*tuple3f1.z);
        }

        public static double dotProduct2D(Tuple2f tuple2f, Tuple2f tuple2f1)
        {
            return (double) (tuple2f.x*tuple2f1.x + tuple2f.y*tuple2f1.y);
        }

        public static void vectorProduct(Tuple3f tuple3f, Tuple3f tuple3f1, Tuple3f tuple3f2)
        {
            tuple3f2.x = tuple3f.y*tuple3f1.z - tuple3f1.y*tuple3f.z;
            tuple3f2.y = tuple3f1.x*tuple3f.z - tuple3f.x*tuple3f1.z;
            tuple3f2.z = tuple3f.x*tuple3f1.y - tuple3f1.x*tuple3f.y;
        }

        public static void vectorAdd(Tuple3f tuple3f, Tuple3f tuple3f1, Tuple3f tuple3f2)
        {
            tuple3f2.x = tuple3f.x + tuple3f1.x;
            tuple3f2.y = tuple3f.y + tuple3f1.y;
            tuple3f2.z = tuple3f.z + tuple3f1.z;
        }

        public static void vectorSub(Tuple3f tuple3f, Tuple3f tuple3f1, Tuple3f tuple3f2)
        {
            tuple3f2.x = tuple3f.x - tuple3f1.x;
            tuple3f2.y = tuple3f.y - tuple3f1.y;
            tuple3f2.z = tuple3f.z - tuple3f1.z;
        }

        public static void vectorAdd2D(Tuple2f tuple2f, Tuple2f tuple2f1, Tuple2f tuple2f2)
        {
            tuple2f2.x = tuple2f.x + tuple2f1.x;
            tuple2f2.y = tuple2f.y + tuple2f1.y;
        }

        public static void vectorSub2D(Tuple2f tuple2f, Tuple2f tuple2f1, Tuple2f tuple2f2)
        {
            tuple2f2.x = tuple2f.x - tuple2f1.x;
            tuple2f2.y = tuple2f.y - tuple2f1.y;
        }

        public static void invertVector(Tuple3f tuple3f)
        {
            tuple3f.x = -tuple3f.x;
            tuple3f.y = -tuple3f.y;
            tuple3f.z = -tuple3f.z;
        }

        public static void divScalar(double d, Tuple3f tuple3f)
        {
            tuple3f.x /= (float) d;
            tuple3f.y /= (float) d;
            tuple3f.z /= (float) d;
        }

        public static void multScalar2D(double d, Tuple2f tuple2f)
        {
            tuple2f.x *= (float) d;
            tuple2f.y *= (float) d;
        }

        public static int signEps(double d, double d1)
        {
            return d > d1 ? 1 : d >= -d1 ? 0 : -1;
        }

        public static double D_RND_MAX = 2147483647D;
    }

    public class Heap {
        public Heap() {}

        public static void printHeapData(TriangulatorGLUbest TriangulatorGLUbest)
        {
            Console.WriteLine("\nHeap Data : numZero " + TriangulatorGLUbest.numZero + " numHeap " +
                              TriangulatorGLUbest.numHeap);
            for (int i = 0; i < TriangulatorGLUbest.numHeap; i++)
                Console.WriteLine(" ratio " + TriangulatorGLUbest.heap[i].ratio + ", index " +
                                  TriangulatorGLUbest.heap[i].index + ", prev " + TriangulatorGLUbest.heap[i].prev +
                                  ", next " + TriangulatorGLUbest.heap[i].next);

            Console.WriteLine(" ");
        }

        public static void initHeap(TriangulatorGLUbest TriangulatorGLUbest)
        {
            TriangulatorGLUbest.maxNumHeap = TriangulatorGLUbest.numPoints;
            TriangulatorGLUbest.heap = new HeapNode[TriangulatorGLUbest.maxNumHeap];
            TriangulatorGLUbest.numHeap = 0;
            TriangulatorGLUbest.numZero = 0;
        }

        public static void storeHeapData(TriangulatorGLUbest TriangulatorGLUbest, int i, double d, int j, int k, int l)
        {
            TriangulatorGLUbest.heap[i] = new HeapNode();
            TriangulatorGLUbest.heap[i].ratio = d;
            TriangulatorGLUbest.heap[i].index = j;
            TriangulatorGLUbest.heap[i].prev = k;
            TriangulatorGLUbest.heap[i].next = l;
        }

        public static void dumpOnHeap(TriangulatorGLUbest TriangulatorGLUbest, double d, int i, int j, int k)
        {
            if (TriangulatorGLUbest.numHeap >= TriangulatorGLUbest.maxNumHeap){
                HeapNode[] aheapnode = TriangulatorGLUbest.heap;
                TriangulatorGLUbest.maxNumHeap = TriangulatorGLUbest.maxNumHeap + TriangulatorGLUbest.numPoints;
                TriangulatorGLUbest.heap = new HeapNode[TriangulatorGLUbest.maxNumHeap];
                System.Array.Copy(aheapnode, 0, TriangulatorGLUbest.heap, 0, aheapnode.Length);
            }
            int l;
            if (d == 0.0D){
                if (TriangulatorGLUbest.numZero < TriangulatorGLUbest.numHeap)
                    if (TriangulatorGLUbest.heap[TriangulatorGLUbest.numHeap] == null)
                        storeHeapData(TriangulatorGLUbest, TriangulatorGLUbest.numHeap,
                                      TriangulatorGLUbest.heap[TriangulatorGLUbest.numZero].ratio,
                                      TriangulatorGLUbest.heap[TriangulatorGLUbest.numZero].index,
                                      TriangulatorGLUbest.heap[TriangulatorGLUbest.numZero].prev,
                                      TriangulatorGLUbest.heap[TriangulatorGLUbest.numZero].next);
                    else
                        TriangulatorGLUbest.heap[TriangulatorGLUbest.numHeap].copy(
                            TriangulatorGLUbest.heap[TriangulatorGLUbest.numZero]);
                l = TriangulatorGLUbest.numZero;
                TriangulatorGLUbest.numZero++;
            }
            else{
                l = TriangulatorGLUbest.numHeap;
            }
            storeHeapData(TriangulatorGLUbest, l, d, i, j, k);
            TriangulatorGLUbest.numHeap++;
        }

        public static void insertIntoHeap(TriangulatorGLUbest TriangulatorGLUbest, double d, int i, int j, int k)
        {
            dumpOnHeap(TriangulatorGLUbest, d, i, j, k);
        }

        public static bool deleteFromHeap(TriangulatorGLUbest TriangulatorGLUbest, int[] ai, int[] ai1, int[] ai2)
        {
            if (TriangulatorGLUbest.numZero > 0){
                TriangulatorGLUbest.numZero--;
                TriangulatorGLUbest.numHeap--;
                ai[0] = TriangulatorGLUbest.heap[TriangulatorGLUbest.numZero].index;
                ai1[0] = TriangulatorGLUbest.heap[TriangulatorGLUbest.numZero].prev;
                ai2[0] = TriangulatorGLUbest.heap[TriangulatorGLUbest.numZero].next;
                if (TriangulatorGLUbest.numZero < TriangulatorGLUbest.numHeap)
                    TriangulatorGLUbest.heap[TriangulatorGLUbest.numZero].copy(
                        TriangulatorGLUbest.heap[TriangulatorGLUbest.numHeap]);
                return true;
            }
            if (TriangulatorGLUbest.earsRandom){
                if (TriangulatorGLUbest.numHeap <= 0){
                    TriangulatorGLUbest.numHeap = 0;
                    return false;
                }
                double d = TriangulatorGLUbest.randomGen.NextDouble();
                int i = (int) (d*(double) TriangulatorGLUbest.numHeap);
                TriangulatorGLUbest.numHeap--;
                if (i > TriangulatorGLUbest.numHeap)
                    i = TriangulatorGLUbest.numHeap;
                ai[0] = TriangulatorGLUbest.heap[i].index;
                ai1[0] = TriangulatorGLUbest.heap[i].prev;
                ai2[0] = TriangulatorGLUbest.heap[i].next;
                if (i < TriangulatorGLUbest.numHeap)
                    TriangulatorGLUbest.heap[i].copy(TriangulatorGLUbest.heap[TriangulatorGLUbest.numHeap]);
                return true;
            }
            if (TriangulatorGLUbest.numHeap <= 0){
                TriangulatorGLUbest.numHeap = 0;
                return false;
            }
            else{
                TriangulatorGLUbest.numHeap--;
                ai[0] = TriangulatorGLUbest.heap[TriangulatorGLUbest.numHeap].index;
                ai1[0] = TriangulatorGLUbest.heap[TriangulatorGLUbest.numHeap].prev;
                ai2[0] = TriangulatorGLUbest.heap[TriangulatorGLUbest.numHeap].next;
                return true;
            }
        }
    }

    public class Tuple2f {
        public Tuple2f()
        {
            x = 0;
            y = 0;
        }

        public Tuple2f(float f, float f1)
        {
            x = f;
            y = f1;
        }

        public void set(float f, float f1)
        {
            x = f;
            y = f1;
        }

        public void set(float[] af)
        {
            x = af[0];
            y = af[1];
        }

        public void set(Tuple2f tuple2f)
        {
            x = tuple2f.x;
            y = tuple2f.y;
        }

        public float x;
        public float y;
    }

    public class Tuple3f {
        public Tuple3f()
        {
            x = 0;
            y = 0;
            z = 0;
        }

        public Tuple3f(float f, float f1, float f2)
        {
            x = f;
            y = f1;
            z = f2;
        }

        public float x;
        public float y;
        public float z;

        public void set(Tuple3f tuple3f)
        {
            x = tuple3f.x;
            y = tuple3f.y;
            z = tuple3f.z;
        }
    }

    public class Matrix4f {
        public Matrix4f(float f, float f1, float f2, float f3, float f4, float f5, float f6,
                        float f7, float f8, float f9, float f10, float f11, float f12, float f13,
                        float f14, float f15)
        {
            m00 = f;
            m01 = f1;
            m02 = f2;
            m03 = f3;
            m10 = f4;
            m11 = f5;
            m12 = f6;
            m13 = f7;
            m20 = f8;
            m21 = f9;
            m22 = f10;
            m23 = f11;
            m30 = f12;
            m31 = f13;
            m32 = f14;
            m33 = f15;
        }

        public Matrix4f()
        {
            m00 = 0.0F;
            m01 = 0.0F;
            m02 = 0.0F;
            m03 = 0.0F;
            m10 = 0.0F;
            m11 = 0.0F;
            m12 = 0.0F;
            m13 = 0.0F;
            m20 = 0.0F;
            m21 = 0.0F;
            m22 = 0.0F;
            m23 = 0.0F;
            m30 = 0.0F;
            m31 = 0.0F;
            m32 = 0.0F;
            m33 = 0.0F;
        }

        public float m00;
        public float m01;
        public float m02;
        public float m03;
        public float m10;
        public float m11;
        public float m12;
        public float m13;
        public float m20;
        public float m21;
        public float m22;
        public float m23;
        public float m30;
        public float m31;
        public float m32;
        public float m33;

        public void transform(Point3f point3f, Point3f point3f1)
        {
            float f = m00*point3f.x + m01*point3f.y + m02*point3f.z + m03;
            float f1 = m10*point3f.x + m11*point3f.y + m12*point3f.z + m13;
            point3f1.z = m20*point3f.x + m21*point3f.y + m22*point3f.z + m23;
            point3f1.x = f;
            point3f1.y = f1;
        }
    }

    internal class Degenerate {
        private Degenerate() {}

        public static bool handleDegeneracies(TriangulatorGLUbest TriangulatorGLUbest, int i, int j, int k, int l,
                                              int i1, int j1)
        {
            int[] ai = new int[1];
            double d = 0.0D;
            double d3 = 0.0D;
            double d4 = 0.0D;
            int l2 = TriangulatorGLUbest.fetchPrevData(j1);
            int l1 = TriangulatorGLUbest.fetchData(l2);
            if (l1 != k && l1 != l){
                bool flag = Numerics.vtxInTriangle(TriangulatorGLUbest, i, k, l, l1, ai);
                if (flag && ai[0] == 0)
                    return true;
                if (k <= l){
                    if (i1 <= l1)
                        flag = Numerics.segIntersect(TriangulatorGLUbest, k, l, i1, l1, -1);
                    else
                        flag = Numerics.segIntersect(TriangulatorGLUbest, k, l, l1, i1, -1);
                }
                else if (i1 <= l1)
                    flag = Numerics.segIntersect(TriangulatorGLUbest, l, k, i1, l1, -1);
                else
                    flag = Numerics.segIntersect(TriangulatorGLUbest, l, k, l1, i1, -1);
                if (flag)
                    return true;
            }
            l2 = TriangulatorGLUbest.fetchNextData(j1);
            l1 = TriangulatorGLUbest.fetchData(l2);
            if (l1 != k && l1 != l){
                bool flag1 = Numerics.vtxInTriangle(TriangulatorGLUbest, i, k, l, l1, ai);
                if (flag1 && ai[0] == 0)
                    return true;
                if (k <= l){
                    if (i1 <= l1)
                        flag1 = Numerics.segIntersect(TriangulatorGLUbest, k, l, i1, l1, -1);
                    else
                        flag1 = Numerics.segIntersect(TriangulatorGLUbest, k, l, l1, i1, -1);
                }
                else if (i1 <= l1)
                    flag1 = Numerics.segIntersect(TriangulatorGLUbest, l, k, i1, l1, -1);
                else
                    flag1 = Numerics.segIntersect(TriangulatorGLUbest, l, k, l1, i1, -1);
                if (flag1)
                    return true;
            }
            int k1 = i;
            int i2 = j;
            j = TriangulatorGLUbest.fetchNextData(j);
            for (i = TriangulatorGLUbest.fetchData(j); j != j1; i = k){
                int j2 = TriangulatorGLUbest.fetchNextData(j);
                k = TriangulatorGLUbest.fetchData(j2);
                double d1 = Numerics.stableDet2D(TriangulatorGLUbest, k1, i, k);
                d3 += d1;
                j = j2;
            }

            j = TriangulatorGLUbest.fetchPrevData(i2);
            for (i = TriangulatorGLUbest.fetchData(j); j != j1; i = k){
                int k2 = TriangulatorGLUbest.fetchPrevData(j);
                k = TriangulatorGLUbest.fetchData(k2);
                double d2 = Numerics.stableDet2D(TriangulatorGLUbest, k1, i, k);
                d4 += d2;
                j = k2;
            }

            TriangulatorGLUbest _tmp = TriangulatorGLUbest;
            if (Numerics.le(d3, 1E-008D)){
                TriangulatorGLUbest _tmp1 = TriangulatorGLUbest;
                if (Numerics.le(d4, 1E-008D))
                    return false;
            }
            TriangulatorGLUbest _tmp2 = TriangulatorGLUbest;
            if (Numerics.ge(d3, 1E-008D)){
                TriangulatorGLUbest _tmp3 = TriangulatorGLUbest;
                if (Numerics.ge(d4, 1E-008D))
                    return false;
            }
            return true;
        }
    }

    internal class BottleNeck {
        private BottleNeck() {}

        private static bool checkArea(TriangulatorGLUbest TriangulatorGLUbest, int i, int j)
        {
            double d = 0.0D;
            double d3 = 0.0D;
            double d4 = 0.0D;
            int j1 = TriangulatorGLUbest.fetchData(i);
            int k = TriangulatorGLUbest.fetchNextData(i);
            int i2;
            for (int k1 = TriangulatorGLUbest.fetchData(k); k != j; k1 = i2){
                int l = TriangulatorGLUbest.fetchNextData(k);
                i2 = TriangulatorGLUbest.fetchData(l);
                double d1 = Numerics.stableDet2D(TriangulatorGLUbest, j1, k1, i2);
                d3 += d1;
                k = l;
            }

            TriangulatorGLUbest _tmp = TriangulatorGLUbest;
            if (Numerics.le(d3, 1E-008D))
                return false;
            k = TriangulatorGLUbest.fetchNextData(j);
            int j2;
            for (int l1 = TriangulatorGLUbest.fetchData(k); k != i; l1 = j2){
                int i1 = TriangulatorGLUbest.fetchNextData(k);
                j2 = TriangulatorGLUbest.fetchData(i1);
                double d2 = Numerics.stableDet2D(TriangulatorGLUbest, j1, l1, j2);
                d4 += d2;
                k = i1;
            }

            TriangulatorGLUbest _tmp1 = TriangulatorGLUbest;
            return !Numerics.le(d4, 1E-008D);
        }

        public static bool checkBottleNeck(TriangulatorGLUbest TriangulatorGLUbest, int i, int j, int k, int l)
        {
            int j1 = i;
            int i1 = TriangulatorGLUbest.fetchPrevData(l);
            int k1 = TriangulatorGLUbest.fetchData(i1);
            if (k1 != j && k1 != k){
                bool flag = Numerics.pntInTriangle(TriangulatorGLUbest, i, j, k, k1);
                if (flag)
                    return true;
            }
            bool flag1;
            if (j <= k){
                if (j1 <= k1)
                    flag1 = Numerics.segIntersect(TriangulatorGLUbest, j, k, j1, k1, -1);
                else
                    flag1 = Numerics.segIntersect(TriangulatorGLUbest, j, k, k1, j1, -1);
            }
            else if (j1 <= k1)
                flag1 = Numerics.segIntersect(TriangulatorGLUbest, k, j, j1, k1, -1);
            else
                flag1 = Numerics.segIntersect(TriangulatorGLUbest, k, j, k1, j1, -1);
            if (flag1)
                return true;
            i1 = TriangulatorGLUbest.fetchNextData(l);
            k1 = TriangulatorGLUbest.fetchData(i1);
            if (k1 != j && k1 != k){
                flag1 = Numerics.pntInTriangle(TriangulatorGLUbest, i, j, k, k1);
                if (flag1)
                    return true;
            }
            if (j <= k){
                if (j1 <= k1)
                    flag1 = Numerics.segIntersect(TriangulatorGLUbest, j, k, j1, k1, -1);
                else
                    flag1 = Numerics.segIntersect(TriangulatorGLUbest, j, k, k1, j1, -1);
            }
            else if (j1 <= k1)
                flag1 = Numerics.segIntersect(TriangulatorGLUbest, k, j, j1, k1, -1);
            else
                flag1 = Numerics.segIntersect(TriangulatorGLUbest, k, j, k1, j1, -1);
            if (flag1)
                return true;
            i1 = TriangulatorGLUbest.fetchNextData(l);
            for (int l1 = TriangulatorGLUbest.fetchData(i1); i1 != l; l1 = TriangulatorGLUbest.fetchData(i1)){
                if (j1 == l1 && checkArea(TriangulatorGLUbest, l, i1))
                    return true;
                i1 = TriangulatorGLUbest.fetchNextData(i1);
            }

            return false;
        }
    }
}

/*
and how to use:

Give a list of vertices, vertexIndices, normals (maby null)
and faces:

TriangulatorGLUbest.triangulate(vertices, vertexIndices, normals, faces, null);

then you will get a list of triangles

for (int i = 0; i < TriangulatorGLUbest.numTriangles; i++)
{
if (TriangulatorGLUbest == null)
{
}

Gl.glBegin(Gl.GL_TRIANGLES); // Drawing Using Triangles
if( texture) Gl.glEnable(Gl.GL_TEXTURE_2D);

int i1 = TriangulatorGLUbest.list[TriangulatorGLUbest.triangles[i].v1].getCommonIndex();
int i2 = TriangulatorGLUbest.list[TriangulatorGLUbest.triangles[i].v2].getCommonIndex();
int i3 = TriangulatorGLUbest.list[TriangulatorGLUbest.triangles[i].v3].getCommonIndex();

if (faceset.normals != null && faceset.normalsIndex != null
&& faceset.normalsIndex.Count > i1)
{
int norm_ind = (int)faceset.normalsIndex[i1];
if (norm_ind != -1)
{
GeoPoint norm = (GeoPoint)faceset.normals[norm_ind];
Gl.glNormal3d(norm.x[0], norm.x[1], norm.x[2]); // Top
}
}

if (texture && faceset.texIndex != null && faceset.texIndex.Count > i1)
{
int texind = (int)faceset.texIndex[i1];
if (texind != -1)
{
GeoPoint texPoint = (GeoPoint)faceset.texCoords[texind];
Gl.glTexCoord2d(texPoint.x[0], texPoint.x[1]);
}
}

Gl.glVertex3f(
TriangulatorGLUbest.vertices[TriangulatorGLUbest.vertexIndices[i1]].x,
TriangulatorGLUbest.vertices[TriangulatorGLUbest.vertexIndices[i1]].y,
TriangulatorGLUbest.vertices[TriangulatorGLUbest.vertexIndices[i1]].z);

if (faceset.normals != null && faceset.normalsIndex != null
&& faceset.normalsIndex.Count > i2)
{
int norm_ind = (int)faceset.normalsIndex[i2];
if (norm_ind != -1)
{
GeoPoint norm = (GeoPoint)faceset.normals[norm_ind];
Gl.glNormal3d(norm.x[0], norm.x[1], norm.x[2]); // Top
}
}
if (texture && faceset.texIndex != null && faceset.texIndex.Count > i2)
{
int texind = (int)faceset.texIndex[i2];
if (texind != -1)
{
GeoPoint texPoint = (GeoPoint)faceset.texCoords[texind];
Gl.glTexCoord2d(texPoint.x[0], texPoint.x[1]);
}
}

Gl.glVertex3f(
TriangulatorGLUbest.vertices[TriangulatorGLUbest.vertexIndices[i2]].x,
TriangulatorGLUbest.vertices[TriangulatorGLUbest.vertexIndices[i2]].y,
TriangulatorGLUbest.vertices[TriangulatorGLUbest.vertexIndices[i2]].z);

if (faceset.normals != null && faceset.normalsIndex != null
&& faceset.normalsIndex.Count > i3)
{
int norm_ind = (int)faceset.normalsIndex[i3];
if (norm_ind != -1)
{
GeoPoint norm = (GeoPoint)faceset.normals[norm_ind];
Gl.glNormal3d(norm.x[0], norm.x[1], norm.x[2]); // Top
}
//OPLog.log(GetType(), System.Diagnostics.EventLogEntryType.Warning, "Ungültiger Normalenindex " + norm_ind + " in " + this.Bezeichnung);
}
if (texture && faceset.texIndex != null && faceset.texIndex.Count > i3)
{
int texind = (int)faceset.texIndex[i3];
if (texind != -1)
{
GeoPoint texPoint = (GeoPoint)faceset.texCoords[texind];
Gl.glTexCoord2d(texPoint.x[0], texPoint.x[1]);
}
}

Gl.glVertex3f(
TriangulatorGLUbest.vertices[TriangulatorGLUbest.vertexIndices[i3]].x,
TriangulatorGLUbest.vertices[TriangulatorGLUbest.vertexIndices[i3]].y,
TriangulatorGLUbest.vertices[TriangulatorGLUbest.vertexIndices[i3]].z);

Gl.glEnd(); // Drawing Using Triangles
}
}
}*/
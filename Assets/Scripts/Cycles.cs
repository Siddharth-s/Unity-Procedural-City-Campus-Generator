using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;

public class Cycles
{

    //  Graph modelled as list of edges

    public static List<int[]> cycles = new List<int[]>();

    public static List<int[]> FindCycles(int[,] graph)
    {
        cycles = new List<int[]>();
        List<int[]> allCycles = new List<int[]>();

        for (int i = 0; i < graph.GetLength(0); i++)
            for (int j = 0; j < graph.GetLength(1); j++)
            {
                FindNewCycles(new int[] { graph[i, j] },graph);
            }

        foreach (int[] cy in cycles)
        {
            string s = "" + cy[0];
            allCycles.Add(cy);
            
            for (int i = 1; i < cy.Length; i++)
                s += "," + cy[i];

            Debug.Log("Cycle Vertices: " + s);
        }
        return allCycles;
    }
    static void FindNewCycles(int[] path , int[,] graph)
    {
        int n = path[0];
        int x;
        int[] sub = new int[path.Length + 1];

        for (int i = 0; i < graph.GetLength(0); i++)
            for (int y = 0; y <= 1; y++)
                if (graph[i, y] == n)
                //  edge referes to our current node
                {
                    x = graph[i, (y + 1) % 2];
                    if (!Visited(x, path))
                    //  neighbor node not on path yet
                    {
                        sub[0] = x;
                        Array.Copy(path, 0, sub, 1, path.Length);
                        //  explore extended path
                        FindNewCycles(sub,graph);
                    }
                    else if ((path.Length > 2) && (x == path[path.Length - 1]))
                    //  cycle found
                    {
                        int[] p = Normalize(path);
                        int[] inv = Invert(p);
                        if (IsNew(p) && IsNew(inv))
                            cycles.Add(p);
                    }
                }
    }

    static bool Equals(int[] a, int[] b)
    {
        bool ret = (a[0] == b[0]) && (a.Length == b.Length);

        for (int i = 1; ret && (i < a.Length); i++)
            if (a[i] != b[i])
            {
                ret = false;
            }

        return ret;
    }

    static int[] Invert(int[] path)
    {
        int[] p = new int[path.Length];

        for (int i = 0; i < path.Length; i++)
            p[i] = path[path.Length - 1 - i];

        return Normalize(p);
    }

    //  rotate cycle path such that it begins with the Smallest node
    static int[] Normalize(int[] path)
    {
        int[] p = new int[path.Length];
        int x = Smallest(path);
        int n;

        Array.Copy(path, 0, p, 0, path.Length);

        while (p[0] != x)
        {
            n = p[0];
            Array.Copy(p, 1, p, 0, p.Length - 1);
            p[p.Length - 1] = n;
        }

        return p;
    }

    static bool IsNew(int[] path)
    {
        bool ret = true;

        foreach (int[] p in cycles)
            if (Equals(p, path))
            {
                ret = false;
                break;
            }

        return ret;
    }

    static int Smallest(int[] path)
    {
        int min = path[0];

        foreach (int p in path)
            if (p < min)
                min = p;

        return min;
    }

    static bool Visited(int n, int[] path)
    {
        bool ret = false;

        foreach (int p in path)
            if (p == n)
            {
                ret = true;
                break;
            }

        return ret;
    }

    public static List<int[]> ReturnAllCircles(int[,] graph)
    {
        return  FindCycles(graph);
    }
}


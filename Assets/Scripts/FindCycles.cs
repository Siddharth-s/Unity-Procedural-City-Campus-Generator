using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//***** Not Complete
public static class FindCycles 
{
    public static void FindMCB(List<int[]> planarGraph,List<Vertex> allVertices)
    {
        List<int[]> planarGraphCopy = new List<int[]>(planarGraph);// so if we make changes to this original wont be affected
        List<Vertex> verticesCopy = new List<Vertex>(allVertices);
        
        ChangeFromXZtoXY(verticesCopy);//x,z to x,y

        Vertex leftMostBottomMostVertex = verticesCopy[0];

        leftMostBottomMostVertex = FindLeftMostBottomMostPoint(verticesCopy, leftMostBottomMostVertex);

        //TODO: Select first clockwisemost edgesw
        //TODO: Extract individual cycles by choosing counterClockwiseMost edges from new vertices * stop when the firts vertex is reached
        //TODO: Remove appropriate vertex/edge 
        //TODO: Remove Filaments
        //TODO: Repeat until no vertex/edges are left

        Debug.Log(leftMostBottomMostVertex.coord.x + " " + leftMostBottomMostVertex.coord.y + " ID:" + leftMostBottomMostVertex.id);// Checking if the 
    }

    private static Vertex GetClockwiseMost(Vertex vPrev, Vertex vCurr)//TODO: Work on this
    {
        if (true)
        {
            return null;
        }
    }


    private static void ChangeFromXZtoXY(List<Vertex> verticesCopy)
    {
        foreach (Vertex p in verticesCopy)
        {
            Vector3 temp = new Vector3(p.coord.x, p.coord.z, 0);
            p.coord = temp;
        }
    }

    private static Vertex FindLeftMostBottomMostPoint(List<Vertex> verticesCopy, Vertex leftMostBottomMostVertex)
    {
        foreach (Vertex p in verticesCopy)
        {
            if (p.coord.x < leftMostBottomMostVertex.coord.x)
            {
                leftMostBottomMostVertex = p;
            }
            else if (p.coord.x == leftMostBottomMostVertex.coord.x && p.coord.y < leftMostBottomMostVertex.coord.y)
            {
                leftMostBottomMostVertex = p;
            }
        }
        return leftMostBottomMostVertex;
    }
}

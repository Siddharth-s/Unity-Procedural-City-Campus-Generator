//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class FindCycles : MonoBehaviour
//{

//    struct Tree
//    {
//        List<int> cycle;
//        List<Tree> children;
//    };
//    public class Vertex
//    {
//        //C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//        //		Vertex(int inName, STLVector<Point> inPosition);
//        //C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//        //ORIGINAL LINE: bool operator < (Vertex const& vertex) const;
//        //C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//        //		bool operator < (Vertex vertex);

//        // The index into the 'positions' input provided to the call to
//        // operator().  The index is used when reporting cycles to the
//        // caller of the constructor for MinimalCycleBasis.
//        public int name;

//        // Multiple vertices can share a position during processing of
//        // graph components.
//        public List<Point> position = new List<Point>(2);

//        // The mVertexStorage member owns the Vertex objects and maintains
//        // the reference counts on those objects.  The adjacent pointers
//        // are considered to be weak pointers; neither object ownership nor
//        // reference counting is required by 'adjacent'.
//        public SortedSet<Vertex> adjacent = new SortedSet<Vertex>();

//        // Support for depth-first traversal of a graph.
//        public int visited;
//    }

//    Vertex GetClockwiseMost(Vertex vPrev, Vertex vCurr)
//    {
//        Vertex vNext = null;
//        bool vCurrConvex = false;
//        List<Point> dCurr = new List<Point>();
//        List<Point> dNext = new List<Point>(2);
//        if (vPrev != null)
//        {
//            dCurr[0] = (*vCurr.position)[0] - (*vPrev.position)[0];
//            dCurr[1] = (*vCurr.position)[1] - (*vPrev.position)[1];
//        }
//        else
//        {
//            dCurr[0] = (Point)0;
//            dCurr[1] = (Point) - 1;
//        }

//        foreach (var vAdj in vCurr.adjacent)
//        {
//            // vAdj is a vertex adjacent to vCurr.  No backtracking is allowed.
//            if (vAdj == vPrev)
//            {
//                continue;
//            }

//            // Compute the potential direction to move in.
//            List<Point> dAdj = new List<Point>(2);
//            dAdj[0] = (vAdj.position)[0] - (vCurr.position)[0];
//            dAdj[1] = (vAdj.position)[1] - (vCurr.position)[1];

//            // Select the first candidate.
//            if (vNext == null)
//            {
//                vNext = vAdj;
//                dNext = new List<Point>(dAdj);
//                vCurrConvex = (dNext[0] * dCurr[1] <= dNext[1] * dCurr[0]) != null;
//                continue;
//            }

//            // Update if the next candidate is clockwise of the current
//            // clockwise-most vertex.
//            if (vCurrConvex)
//            {
//                if (dCurr[0] * dAdj[1] < dCurr[1] * dAdj[0] != null || dNext[0] * dAdj[1] < dNext[1] * dAdj[0] != null)
//                {
//                    vNext = vAdj;
//                    dNext = new List<Point>(dAdj);
//                    vCurrConvex = (dNext[0] * dCurr[1] <= dNext[1] * dCurr[0]) != null;
//                }
//            }
//            else
//            {
//                if (dCurr[0] * dAdj[1] < dCurr[1] * dAdj[0] != null && dNext[0] * dAdj[1] < dNext[1] * dAdj[0] != null)
//                {
//                    vNext = vAdj;
//                    dNext = new List<Point>(dAdj);
//                    vCurrConvex = (dNext[0] * dCurr[1] < dNext[1] * dCurr[0]) != null;
//                }
//            }
//        }

//        return vNext;
//    }
//}




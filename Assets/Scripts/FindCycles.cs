using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FindCycles 
{
    public static void FindMCB(List<int[]> planarGraph,List<Point> points)
    {
        List<int[]> planarGraphCopy = new List<int[]>(planarGraph);// so if we make changes to this original wont be affected
        List<Point> pointsCopy = new List<Point>(points);
        
        ChangeFromXZtoXY(pointsCopy);//x,z to x,y

        Point leftMostBottomMostPoint = pointsCopy[0];

        leftMostBottomMostPoint = FindLeftMostBottomMostPoint(pointsCopy, leftMostBottomMostPoint);// TODO: check if this works

        //TODO: Select first clockwisemost edgesw

        Debug.Log(leftMostBottomMostPoint.coord.x + " " + leftMostBottomMostPoint.coord.y + " ID:" + leftMostBottomMostPoint.id);
    }

    private static Point GetClockwiseMost(Point pPrev, Point pCurr)
    {
        if (true)
        {
            return null;
        }
    }


    private static void ChangeFromXZtoXY(List<Point> pointsCopy)
    {
        foreach (Point p in pointsCopy)
        {
            Vector3 temp = new Vector3(p.coord.x, p.coord.z, 0);
            p.coord = temp;
        }
    }

    private static Point FindLeftMostBottomMostPoint(List<Point> pointsCopy, Point leftMostBottomMostPoint)
    {
        foreach (Point p in pointsCopy)
        {
            if (p.coord.x < leftMostBottomMostPoint.coord.x)
            {
                leftMostBottomMostPoint = p;
            }
            else if (p.coord.x == leftMostBottomMostPoint.coord.x && p.coord.y < leftMostBottomMostPoint.coord.y)
            {
                leftMostBottomMostPoint = p;
            }
        }

        return leftMostBottomMostPoint;
    }
}

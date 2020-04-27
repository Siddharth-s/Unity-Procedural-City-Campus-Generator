using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public static class Utility
{
    public static Vector3 LineLineIntersection(Vector3 A, Vector3 B, Vector3 C, Vector3 D)
    {
        // Line AB represented as a1x + b1y = c1  
        float a1 = B.z - A.z;
        float b1 = A.x - B.x;
        float c1 = a1 * (A.x) + b1 * (A.z);

        // Line CD represented as a2x + b2y = c2  
        float a2 = D.z - C.z;
        float b2 = C.x - D.x;
        float c2 = a2 * (C.x) + b2 * (C.z);

        float determinant = a1 * b2 - a2 * b1;

        if (determinant == 0)
        {
            // The lines are parallel. This is simplified  
            // by returning a pair of FLT_MAX  
            return new Vector3(float.MaxValue, float.MaxValue);
        }
        else
        {
            float x = (b2 * c1 - b1 * c2) / determinant;
            float z = (a1 * c2 - a2 * c1) / determinant;
            return new Vector3(x,0f, z);
        }
    }

    public static int NOofIntersectionsByRay(Line line, List<Line> targetLines)
    {
        int totalIntersections = 0;
        Vector3 temp;
        foreach (var targetLine in targetLines)
        {
            temp = LineLineIntersection(line.a, line.b, targetLine.a, targetLine.b);
            if (temp != new Vector3(float.MaxValue, float.MaxValue, float.MaxValue))//lines are not parallel
            {
                if(PointInBetween(targetLine.a,temp,targetLine.b))
                {
                    if((temp - line.a).magnitude >= 0)
                    {
                        totalIntersections++;
                    }
                }
            }
        }
        
        return totalIntersections;
    }

    public static Vector3 CalculateCentroid(List<Vector3> points)
    {
        float sumX = 0, sumZ = 0;
        for (int j = 0; j < points.Count; j++)
        {
            sumX += points[j].x;
            sumZ += points[j].z;
        }
        return new Vector3(sumX / points.Count, 0, sumZ / points.Count);
    }

    public static float DistanceOfLine(Line a)
    {
        float dis = Mathf.Abs(Vector3.Distance(a.a, a.b));
        return 0f;
    }

    public static bool PointInBetween(Vector3 a,Vector3 b, Vector3 c)
    {
        float error = 0.01f;
        float xCheck = Mathf.Abs(c.x - a.x) - (Mathf.Abs(c.x - b.x) + Mathf.Abs(b.x - a.x));
        float zCheck = Mathf.Abs(c.z - a.z) - (Mathf.Abs(c.z - b.z) + Mathf.Abs(b.z - a.z));
        //Debug.Log("XCheck: " + xCheck + "  ZCheck: " + zCheck);
        if (-error  <= xCheck && xCheck <=  error )
        {
            if(-error <= zCheck && zCheck <= error)
            {
                return true;
            }
        }
        return false;
    }

    public static Vector3 PerpendicularVector(Vector3 vector,bool unitVector = false)
    {
        Vector3 perpendicularVector;
        perpendicularVector.x = vector.z;
        perpendicularVector.z = -vector.x;
        perpendicularVector.y = 0;
        if(unitVector)
        {
            perpendicularVector = perpendicularVector.normalized;
        }
        return perpendicularVector;
    }

    public static Vector3 VectorFromLine(Line line,bool unitVector = false)
    {
        Vector3 vect = line.b - line.a;
        if(unitVector)
        {
            vect = vect.normalized;
        }
        return vect;
    }

    public static Vector3 MidPoint(Vector3 a,Vector3 b)
    {
        Vector3 midPt = (a + b)*0.5f;
        return midPt;
    }
    public static Vector3 MidPoint(Line a)
    {
        Vector3 midPt = (a.a + a.b) * 0.5f;
        return midPt;
    }
    public static void DestroyChildren(GameObject obj)
    {
        List<GameObject> x = new List<GameObject>(); 
        foreach(Transform transform in obj.transform)
        {
            x.Add(transform.gameObject);
            //GameObject.DestroyImmediate(transform.gameObject);
        }
        foreach(GameObject g in x)
        {
            GameObject.DestroyImmediate(g);
        }
    }

    public static bool AreLinesSame(Line p,Line q)
    {

        if(p.a == q.b && p.b == q.a)
        {
            return true;
        }else if(p.a == q.a && p.b == q.b)
        {
            return true;
        }
        return false;
    }

    public static int[] IDPairofLine(Line a,ref List<Point> allPoints)//Returns a pair integer id for the given Line end points 
    {
        int[] edge = new int[2];
        int pointA = -1, pointB = -1;
        foreach(Point point in allPoints)
        {
            if(a.a == point.coord)
            {
                pointA = point.id;
            }
            if(a.b == point.coord)
            {
                pointB = point.id;
            }
        }
        edge[0] = pointA;
        edge[1] = pointB;
        return edge;
    }
    public static void  AddVerticesAtIntersection(Line line ,ref List<Line> allLines, ref List<Point> points,Vector3 gatePosition)
    {
        Vector3 tempIntersection = Vector3.zero;
        List<Line> linesToEdit = new List<Line>();
        List<Vector3> intersectionPoint = new List<Vector3>();
        foreach (Line targetLine in allLines)
        {
            tempIntersection = Utility.LineLineIntersection(line.a, line.b, targetLine.a, targetLine.b);
            if (tempIntersection.x != float.MaxValue)//if lines are not parallel
            {
                if (!AreLinesSame(line , targetLine))
                {
                    if (Utility.PointInBetween(targetLine.a, tempIntersection, targetLine.b))
                    {
                        linesToEdit.Add(targetLine);
                        intersectionPoint.Add(tempIntersection);
                        points.Add(new Point(tempIntersection, points.Count + 1)); 
                    }
                    if(!linesToEdit.Contains(line))
                    {
                        if (!(line.a == tempIntersection || line.b == tempIntersection)) // if the intersection point is not the edge of the give line
                        {
                            if (PointInBetween(line.a, tempIntersection, line.b))
                            {
                                linesToEdit.Add(line);
                                intersectionPoint.Add(tempIntersection);
                            }
                        }
                    }
                    
                }
            }
        }
        for (int i = 0; i < linesToEdit.Count; i++)
        {
            //if(Mathf.Abs(Vector3.Distance(gatePosition,linesToEdit[i].a)) <= 1f)
            //{
            //    Debug.Log("Hello");
            //    allLines.Remove(linesToEdit[i]);
            //    allLines.Add(new Line(gatePosition, intersectionPoint[i]));
            //    allLines.Add(new Line(intersectionPoint[i], linesToEdit[i].b));
            //}else if(Mathf.Abs(Vector3.Distance(gatePosition, linesToEdit[i].b)) <= 1f)
            //{
            //    Debug.Log("Hello");
            //    allLines.Remove(linesToEdit[i]);
            //    allLines.Add(new Line(linesToEdit[i].a, intersectionPoint[i]));
            //    allLines.Add(new Line(intersectionPoint[i], gatePosition));
            //}
            //else
            {
                allLines.Remove(linesToEdit[i]);
                allLines.Add(new Line(linesToEdit[i].a, intersectionPoint[i]));
                allLines.Add(new Line(intersectionPoint[i], linesToEdit[i].b));
            }
            
        }
    }


    public static Vector3 PointFromID(List<Point> points,int id)
    {
        foreach (var item in points)
        {
            if(id == item.id)
            {
                return item.coord;
            }
        }
        return new Vector3(float.MaxValue,float.MaxValue,float.MaxValue);
    }

    public static bool IsInPolygon(int[] loop, Vector3 p,List<Point> point)
    {

        Vector3[] poly = new Vector3[loop.Length];
        for (int i = 0; i < loop.Length; i++)
        {
            poly[i] = PointFromID(point,loop[i]);
        }//poly contains all coordinates of the points
        Vector3 p1, p2;
        bool inside = false;

        if (poly.Length < 3)
        {
            return inside;
        }

        var oldPoint = new Vector3(
            poly[poly.Length - 1].x,0, poly[poly.Length - 1].z);

        for (int i = 0; i < poly.Length; i++)
        {
            var newPoint = new Vector3(poly[i].x,0, poly[i].z);

            if (newPoint.x > oldPoint.x)
            {
                p1 = oldPoint;
                p2 = newPoint;
            }
            else
            {
                p1 = newPoint;
                p2 = oldPoint;
            }

            if ((newPoint.x < p.x) == (p.x <= oldPoint.x)
                && (p.z - (long)p1.z) * (p2.x - p1.x)
                < (p2.z - (long)p1.z) * (p.x - p1.x))
            {
                inside = !inside;
            }

            oldPoint = newPoint;
        }

        return inside;
    }

    public static bool IsInPolygonUsingRay(int[] loop, Vector3 p, List<Point> points)
    {
        bool inside = false;
        //Vector3[] poly = new Vector3[loop.Length];
        
        //for (int i = 0; i < loop.Length - 1; i++)
        //{
        //    poly[i] = PointFromID(point, loop[i]);
        //}//poly contains all coordinates of the points

        List<Line> linesOfPoly = ListOfLineOfAPolygon(loop,points);
        int count = NOofIntersectionsByRay(new Line(p,p + Vector3.right), linesOfPoly);
        if(count%2 != 0)
        {
            inside = true;
        }
        return inside;
    }

    public static List<Line> ListOfLineOfAPolygon(int[] arr,List<Point> points)//takes array of points ids of polygon
    {
        List<Line> polygon = new List<Line>();
        for (int i = 0; i < arr.Length - 1; i++)
        {
            polygon.Add(new Line(PointFromID(points,arr[i]),PointFromID(points,arr[i])));
        }
        polygon.Add(new Line(PointFromID(points, arr[arr.Length - 1]), PointFromID(points, arr[0])));
        return polygon;
    }
}

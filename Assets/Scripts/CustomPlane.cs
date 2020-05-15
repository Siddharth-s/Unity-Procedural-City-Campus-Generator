using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public struct Line
{
    public Vector3 a, b;

    public Line(Vector3 x,Vector3 z)
    {
        a = x;
        b = z;
    }
}
public class Vertex
{
    public Vector3 coord;
    public int id;

    public Vertex(Vector3 x,int a)
    {
        coord = x;
        id = a;
    }
    public Vertex()
    {
        coord = Vector3.zero;
        id = 0;
    }
}

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CustomPlane : MonoBehaviour
{
    public GameObject roadBlock;
    Mesh mesh;
    public List<Vector3> planeVertices;

    //for mesh generation of plane-----

    List<int> trianglesList;
    Vector3[] vertices;
    int[] triangles;
    //----------------------------------
    //Storing information for comp
    [HideInInspector] public float minX, minZ, maxX, maxZ;
    [HideInInspector] public Vector3 centroid;
    List<Line> outerLines;//contains pairs of vertices to define lines
    List<Line> allprimaryRoads;
    List<Vertex> allVertices;//store vertex and its id
    List<int[,]> edges;//in format {x.id,y.id}
    List<int[]> allCyclesInGraph;
    List<int> intList;
    bool[,] grid;
    
    //refrences
    public Material roadMaterial;
    public GameObject gate,roads;
    Vector3 gateCoords;

    public void GeneratePlane()
    {
        CalculateMinMax();  
        AssignInitialPoints();  // Points lying on circumference
        outerLines = new List<Line>();
        allprimaryRoads = new List<Line>();
        outerLines.Clear();
        centroid = Utility.CalculateCentroid(planeVertices);


        for (int j = 0; j < planeVertices.Count - 1; j++)
            {
                GenerateRoadBetweenPoints(planeVertices[j], planeVertices[j + 1],0.15f);
                outerLines.Add(new Line(new Vector3(planeVertices[j].x, 0f, planeVertices[j].z), new Vector3(planeVertices[j+1].x, 0f, planeVertices[j+1].z))); //Add Encircling lines to lines List
            }
        GenerateRoadBetweenPoints(planeVertices[0], planeVertices[planeVertices.Count-1],0.15f);
        outerLines.Add(new Line(new Vector3(planeVertices[0].x, 0f, planeVertices[0].z), new Vector3(planeVertices[planeVertices.Count - 1].x, 0f, planeVertices[planeVertices.Count - 1].z)));//Add last Encircling line to lines List


        //mesh generation------------------------------------------------
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        PopulateVertices();
        PopulateTriangles();

        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        //--------------------------------------------------------------
    }

    private void PopulateVertices()
    {
        vertices = new Vector3[planeVertices.Count];
        int i = 0;
        foreach (Vector3 point in planeVertices)
        {
            vertices[i] = point;
            i++;
        }
    }

    private void PopulateTriangles()
    {
        trianglesList = new List<int>();
        for(int j = 0; j < planeVertices.Count - 2; j++)
        {

                trianglesList.Add(0);
                trianglesList.Add(j + 1);
                trianglesList.Add(j + 2);

        }

        triangles = new int[trianglesList.Count];
        int z = 0;
        foreach (int x in trianglesList)
        {
            triangles[z] = x;
            z++;
        }
    }

    private void CalculateMinMax()
    {
        minX = minZ = maxX = maxZ = 0;
        foreach(Vector3 point in planeVertices)
        {
            if(point.x < minX)
            {
                minX = point.x;
            }
            if (point.z < minZ)
            {
                minZ = point.z;
            }
            if (point.x > maxX)
            {
                maxX = point.x;
            }
            if (point.z > minZ)
            {
                maxZ = point.z;
            }
        }
        //Debug.Log(minX + " " +  maxX + " " + minZ + " " + maxZ);
    }

    private void AssignInitialPoints()
    {
        allVertices = new List<Vertex>();
        foreach(Vector3 pt in planeVertices)
        {
            allVertices.Add(new Vertex(pt, allVertices.Count + 1));
        }
    }

    void GenerateRoadBetweenPoints(Vector3 a, Vector3 b,float width = 0.2f)//#*# remove from here later and put in road script
    {
        GameObject road = Instantiate(roadBlock,roads.transform) as GameObject;
        float x = 0;
        x = Vector3.Distance(a, b);
        road.transform.localScale = new Vector3(x, 0.1f, width);

        if(b.magnitude >= a.magnitude)
        {
            road.transform.position = a + (b - a) / 2;
        }else
        {
            road.transform.position = b + (a - b) / 2;
        }
       

        Vector3 v = b - a;
        road.transform.rotation = Quaternion.FromToRotation(Vector3.right, v);
    }


    //public void GenerateGrid()//Grid of map, true -> road
    //{
    //    grid = new bool[(int)(maxX - minX) * 100, (int)(maxZ - minZ) * 100];
    //    for (int i = 0; i < grid.GetLength(0); i++)
    //    {
    //        for (int j = 0; j < grid.GetLength(1); j++)
    //        {
    //            grid[i, j] = false;
    //        }
    //    }
    //}

    public Line GenerateTestPrimaryRoad()//from gate to outerroads
    {
        gateCoords = gate.transform.position;//****

        Vector3 pointA = Vector3.zero, pointB = Vector3.zero, temp = Vector3.zero,temp2 = Vector3.zero,tempIntersection = Vector3.zero;
        bool first = false, second = false;
        temp = Random.insideUnitSphere.normalized * ((maxX - minX) > (maxZ - minZ) ? (maxZ - minZ) : (maxX - minX)) * 0.75f;
        temp = new Vector3(centroid.x + temp.x, 0,centroid.z + temp.z);
        foreach (Line targetLine in outerLines)
        {
            tempIntersection = Utility.LineLineIntersection(gateCoords, temp, targetLine.a, targetLine.b);
            if (tempIntersection.x != float.MaxValue)//if lines are not parallel
            {
                if(Utility.PointInBetween(targetLine.a,tempIntersection,targetLine.b))
                {
                    if (!first)
                    {
                        pointA = tempIntersection;
                        first = true;
                    }else if(!second)
                    {
                        pointB = tempIntersection;
                        second = true;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        if (Mathf.Abs(Vector3.Distance(pointA, gateCoords)) > Mathf.Abs(Vector3.Distance(pointB, gateCoords)))//assign closest point as gate point
        {
            Vector3 tempSwap = pointA;
            pointA = pointB;
            pointB = tempSwap;
        }
        return new Line(pointA, pointB);

        //GenerateRoadBetweenPoints(pointA, pointB);
    }

    public Line GenerateLongestPrimaryRoad(int samples = 10)  //  generates 10 roads and returns the longest
    {
        float max = 0f;
        Line bestPrimaryRoad = new Line();
        List<Line> primaryRoadList = new List<Line>();
        for (int i = 0; i < samples; i++)
        {
        primaryRoadList.Add(GenerateTestPrimaryRoad());
        }
        foreach(Line road in primaryRoadList)
        {
            if(Utility.DistanceOfLine(road) >= max)
            {
                max = Utility.DistanceOfLine(road);
                bestPrimaryRoad = road;
            }
        }
        //GenerateRoadBetweenPoints(bestPrimaryRoad.a,bestPrimaryRoad.b);
        return bestPrimaryRoad;
        
    }

    public void GeneratePrimaryRoad()//generate a road and store it in allprimaryroad
    {
        allprimaryRoads = new List<Line>();
        allprimaryRoads.Clear();
        Line primaryRoad = GenerateLongestPrimaryRoad();
        allprimaryRoads.Add(primaryRoad);//adding primary road to the allprimaryroad list.. (TODO: Draw roads from here)
        GenerateRoadBetweenPoints(primaryRoad.a, primaryRoad.b);

        Utility.AddVerticesAtIntersection(primaryRoad, ref outerLines, ref allVertices,gate.transform.position);
    }

    public void GenerateSecondPrimaryRoad()
    {
        //Line firstP = allprimaryRoads[0];
        //allprimaryRoads.Clear();
        //allprimaryRoads.Add(firstP);
        //ReDrawPrimaryRoads()  //****TODO**** 
        if(Random.Range(0,5) == 0)//draw a second primary road from gate point
        {
            float maxDistanceEndPoints = 0f;
            Line bestSecondRoad = new Line();
            List<Line> primaryRoadList = new List<Line>();
            for (int i = 0; i < 25; i++)
            {
                primaryRoadList.Add(GenerateLongestPrimaryRoad());
            }

            foreach (Line road in primaryRoadList)
            {
                if (Vector3.Distance(road.b, allprimaryRoads[0].b) >= maxDistanceEndPoints)
                {
                    maxDistanceEndPoints = Utility.DistanceOfLine(road);
                    bestSecondRoad = road;
                }
            }
            GenerateRoadBetweenPoints(bestSecondRoad.a, bestSecondRoad.b);
            allprimaryRoads.Add(bestSecondRoad);
            Utility.AddVerticesAtIntersection(bestSecondRoad, ref outerLines, ref allVertices, gate.transform.position);
            Utility.AddVerticesAtIntersection(bestSecondRoad, ref allprimaryRoads, ref allVertices, gate.transform.position);
        }
        else
        {
            if( Random.Range(0, 2) == 0 )//draw  second primary road/roads perpendicular to first primary road
            {
                Vector3 perpendicularUnitVectToPrimaryRoad = Utility.PerpendicularVector(Utility.VectorFromLine(allprimaryRoads[0]), true);

                if(Random.Range(0, 2) == 0)//from mid point
                {
                    #region midPointRoad
                    Vector3 midpt = Utility.MidPoint(allprimaryRoads[0]);
                    Vector3 pointA = Vector3.zero, pointB = Vector3.zero;
                    Line perpendicularFromMidPoint = new Line(midpt, midpt + perpendicularUnitVectToPrimaryRoad);
                    bool first = false, second = false;
                    foreach(Line targetLine in outerLines)
                    {
                        Vector3 tempIntersection = Utility.LineLineIntersection(midpt, midpt + perpendicularUnitVectToPrimaryRoad, targetLine.a, targetLine.b);
                        if (tempIntersection.x != float.MaxValue)//if lines are not parallel
                        {
                            if (Utility.PointInBetween(targetLine.a, tempIntersection, targetLine.b))//intersection point inside the line segment
                            {
                                if (!first)
                                {
                                    pointA = tempIntersection;
                                    first = true;
                                }
                                else if (!second)
                                {
                                    pointB = tempIntersection;
                                    second = true;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                    #endregion
                    GenerateRoadBetweenPoints(pointA,pointB);
                    allprimaryRoads.Add(new Line(pointA,pointB));
                    Utility.AddVerticesAtIntersection(new Line(pointA, pointB), ref outerLines, ref allVertices, gate.transform.position);
                    Utility.AddVerticesAtIntersection(new Line(pointA, pointB), ref allprimaryRoads, ref allVertices, gate.transform.position);
                }
                else// from one or both of two 1/3rds of the road 
                {  
                    Vector3 oneThird = Vector3.Lerp(allprimaryRoads[0].a ,allprimaryRoads[0].b,1/3f);
                    Vector3 twoThird = Vector3.Lerp(allprimaryRoads[0].a, allprimaryRoads[0].b,2/3f);
                    Vector3 pointAOneThird = Vector3.zero, pointBOneThird = Vector3.zero;
                    Vector3 pointATwoThird = Vector3.zero, pointBTwoThird = Vector3.zero;
                    Line perpendicularFromOneThird = new Line(oneThird, oneThird + perpendicularUnitVectToPrimaryRoad);
                    Line perpendicularFromTwoThird = new Line(twoThird, twoThird + perpendicularUnitVectToPrimaryRoad);
                    bool first = false, second = false;
                    #region oneThirdRoad 
                    foreach (Line targetLine in outerLines)
                    {
                        Vector3 tempIntersectionOneThird = Utility.LineLineIntersection(oneThird, oneThird + perpendicularUnitVectToPrimaryRoad, targetLine.a, targetLine.b);
                        if (tempIntersectionOneThird.x != float.MaxValue)//if lines are not parallel
                        {
                            if (Utility.PointInBetween(targetLine.a, tempIntersectionOneThird, targetLine.b))//intersection point inside the line segment
                            {
                                if (!first)
                                {
                                    pointAOneThird = tempIntersectionOneThird;
                                    first = true;
                                }
                                else if (!second)
                                {
                                    pointBOneThird = tempIntersectionOneThird;
                                    second = true;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                    #endregion 
                    first = second = false;
                    #region twoThirdRoad
                    foreach (Line targetLine in outerLines)
                    {
                        Vector3 tempIntersectionTwoThird = Utility.LineLineIntersection(twoThird, twoThird + perpendicularUnitVectToPrimaryRoad, targetLine.a, targetLine.b);
                        if (tempIntersectionTwoThird.x != float.MaxValue)//if lines are not parallel
                        {
                            if (Utility.PointInBetween(targetLine.a, tempIntersectionTwoThird, targetLine.b))//intersection point inside the line segment
                            {
                                if (!first)
                                {
                                    pointATwoThird = tempIntersectionTwoThird;
                                    first = true;
                                }
                                else if (!second)
                                {
                                    pointBTwoThird = tempIntersectionTwoThird;
                                    second = true;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                    #endregion
                    int ran = Random.Range(0, 3);
                    if(ran == 0)
                    {
                        GenerateRoadBetweenPoints(pointAOneThird, pointBOneThird);
                        allprimaryRoads.Add(new Line(pointAOneThird, pointBOneThird));
                        Utility.AddVerticesAtIntersection(new Line(pointAOneThird, pointBOneThird), ref outerLines, ref allVertices, gate.transform.position);
                        Utility.AddVerticesAtIntersection(new Line(pointAOneThird, pointBOneThird), ref allprimaryRoads, ref allVertices, gate.transform.position);
                    }
                    else if(ran == 1)
                    {
                        GenerateRoadBetweenPoints(pointATwoThird, pointBTwoThird);
                        allprimaryRoads.Add(new Line(pointATwoThird, pointBTwoThird));
                        Utility.AddVerticesAtIntersection(new Line(pointATwoThird, pointBTwoThird), ref outerLines, ref allVertices, gate.transform.position);
                        Utility.AddVerticesAtIntersection(new Line(pointATwoThird, pointBTwoThird), ref allprimaryRoads, ref allVertices, gate.transform.position);
                    }
                    else
                    {
                        GenerateRoadBetweenPoints(pointAOneThird, pointBOneThird);
                        allprimaryRoads.Add(new Line(pointAOneThird, pointBOneThird));
                        Utility.AddVerticesAtIntersection(new Line(pointAOneThird, pointBOneThird), ref outerLines, ref allVertices, gate.transform.position);
                        Utility.AddVerticesAtIntersection(new Line(pointAOneThird, pointBOneThird), ref allprimaryRoads, ref allVertices, gate.transform.position);
                        GenerateRoadBetweenPoints(pointATwoThird, pointBTwoThird);
                        allprimaryRoads.Add(new Line(pointATwoThird, pointBTwoThird));
                        Utility.AddVerticesAtIntersection(new Line(pointATwoThird, pointBTwoThird), ref outerLines, ref allVertices, gate.transform.position);
                        Utility.AddVerticesAtIntersection(new Line(pointATwoThird, pointBTwoThird), ref allprimaryRoads, ref allVertices, gate.transform.position);
                    }
                
                }
            }
        }
    }

    public void GenerateFullNetwork()
    {
        DestroyRoads();
        GeneratePlane();
        GeneratePrimaryRoad();
        GenerateSecondPrimaryRoad();
    }

    public void DestroyRoads()
    {
        Utility.DestroyChildren(roads);
    }

    public void TestCode()
    {

        GetAllCycles();
    }

    public void GetAllCycles()
    {
        ClearConsole();

        allCyclesInGraph = new List<int[]>();

        List<int[]> graph = new List<int[]>(); 
        foreach (Line l in outerLines)
        {
            graph.Add(Utility.IDPairofLine(l, ref allVertices));
        }
        foreach (Line l in allprimaryRoads)
        {
            graph.Add(Utility.IDPairofLine(l, ref allVertices));
        }
        int[,] graphArray = new int[graph.Count, 2];
        for (int i = 0; i < graphArray.GetLength(0); i++)
        {
            graphArray[i, 0] = graph[i][0];
            graphArray[i, 1] = graph[i][1];
        }
        string s = "";
        for (int i = 0; i < graphArray.GetLength(0); i++)
        {
            s += ("(" + graphArray[i, 0] + " " + graphArray[i, 1] + ")");
        }
        Debug.Log(s);
        allCyclesInGraph = Cycles.ReturnAllCircles(graphArray);//all cycles in the graph

        //NOW DELETE THE CYCLES CONTAINING EXTRA EDGES AND POINTS

        //Debug.Log(allCyclesInGraph.Count);

        //List<int> cyclesIDToRemove = new List<int>();

        //for (int i = 0; i < allCyclesInGraph.Count; i++)
        //{
        //    foreach (var pt in points)//all points of current cycle
        //    {
        //        if (!allCyclesInGraph[i].Contains(pt.id))//all the points not in this cycle, check if point is inside the cycle
        //        {
        //            string j = "";
        //            for (int k = 0; k < allCyclesInGraph[i].Length; k++)
        //            {
        //                j += "" + allCyclesInGraph[i][k] + ",";
        //            }

        //            if (Utility.IsInPolygon(allCyclesInGraph[i], pt.coord, points)) // If the current point(point other than the one which make up the cycle) is inside the current cycle
        //            {
        //                //Debug.Log(i.ToString() + ": " + Utility.IsInPolygon(allCyclesInGraph[i], pt.coord, points) + " " + pt.id.ToString());
        //                //allCyclesInGraph.Remove(allCyclesInGraph[i]);
        //                cyclesIDToRemove.Add(i);
        //            }

        //            if (Utility.IsInPolygonUsingRay(allCyclesInGraph[i], pt.coord, points))
        //            {
        //                //Debug.Log(pt.id);
        //                //cyclesIDToRemove.Add(i);
        //            }
        //        }
        //    }//remove if point inside 
        //    //TODO: Remove if two pairs are connected
        //}// Remove cycles

        //foreach(var cycleToBeRemovedID in cyclesIDToRemove)
        //{

        //    string cy = "*_* " + cycleToBeRemovedID.ToString() + " : ";
        //    foreach (var pt in allCyclesInGraph[cycleToBeRemovedID])
        //    {
        //        cy += " " + pt + ",";
        //    }
        //    Debug.Log(cy);
        //}

        Debug.Log(allCyclesInGraph.Count);

        // New Approach

        FindCycles.FindMCB(graph,allVertices);

    }

    private static void ClearConsole()
    {
        var logEntries = System.Type.GetType("UnityEditor.LogEntries, UnityEditor.dll");
        var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        clearMethod.Invoke(null, null);
    }

    void OnDrawGizmosSelected()
    {
        if (allVertices != null)
        {
            DrawNumbersForPoints(allVertices);
            DrawGizmoOfLines(outerLines);
            DrawGizmoOfLines(allprimaryRoads);
        }
    }

    private void DrawNumbersForPoints(List<Vertex> listOfPoints)
    {
        foreach (Vertex pt in listOfPoints)
        {
            Gizmos.color = Color.red;
            UnityEditor.Handles.color = Color.magenta;
            //Gizmos.DrawSphere(pt.coord+ Vector3.up, 0.2f);
            UnityEditor.Handles.Label(pt.coord, pt.id.ToString());
        }
    }

    private void DrawGizmoOfLines(List<Line> listOfLines)
    {
        foreach (Line pt in listOfLines)
        {
            Gizmos.color = Random.ColorHSV();
            Gizmos.DrawLine(pt.a + Vector3.up * 3, pt.b + Vector3.up * 3);
        }
    }
}

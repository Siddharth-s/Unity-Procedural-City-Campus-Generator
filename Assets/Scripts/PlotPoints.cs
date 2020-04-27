using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotPoints : MonoBehaviour
{
    public float groundWidth = 200;
    public float groundLength = 200;
    public float gridCellSize = 5;
    public int numberOfRoadPoints;
    public List<Vector2> roadPoints;
    
    public void GeneratePoints()
    {
        numberOfRoadPoints = (int)Random.Range(10, groundLength * 2 /gridCellSize);
        for(int i = 0; i < numberOfRoadPoints; i++)
        {
            int x = (int)(gridCellSize * (int)Random.Range(1,groundLength/gridCellSize));
            int z = (int)(gridCellSize * (int)Random.Range(1, groundWidth / gridCellSize));
            roadPoints.Add(new Vector2(x, z));
            Instantiate(GameObject.CreatePrimitive(PrimitiveType.Sphere),new Vector3(x,0,z),Quaternion.identity);
        }
    }

    private void Start()
    {
        GeneratePoints();
    }

    void GenerateRoadBetweenPoints(Vector3 a,Vector3 b)
    {
        GameObject road = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube));
        float x= 0;
        x = Vector3.Distance(a,b);
        road.transform.localScale = new Vector3(x,0.5f,2);
        road.transform.position = (b - a)/2;
        //TODO:  align 
    }
}

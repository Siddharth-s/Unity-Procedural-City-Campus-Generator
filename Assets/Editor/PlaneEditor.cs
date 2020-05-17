using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CustomPlane))]
public class PlaneEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        //base.OnInspectorGUI();

        CustomPlane plane = target as CustomPlane;
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        if (GUILayout.Button("Build Object"))
        {
            plane.GeneratePlane();
        }
        if (GUILayout.Button("Generate Primary Road"))
        {
            plane.GeneratePrimaryRoad();
        }
        if (GUILayout.Button("Generate Second Primary Road"))
        {
            plane.GenerateSecondPrimaryRoad();
        }
        if (GUILayout.Button("Destroy Roads"))
        {
            plane.DestroyRoads();
        }
        if (GUILayout.Button("Generate Full Network"))
        {
            plane.GenerateFullNetwork();
        }
        if (GUILayout.Button("Test Code"))
        {
            plane.TestCode();
        }
    }
}

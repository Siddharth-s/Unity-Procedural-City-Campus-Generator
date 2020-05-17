using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BuildingData))]
public class BuildingInputEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        //base.OnInspectorGUI();

        BuildingData buildingData = target as BuildingData;
        EditorGUILayout.Space();

    }
}

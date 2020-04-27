using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlaceField))]
public class MapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        //base.OnInspectorGUI();

        PlaceField map = target as PlaceField;

        if (GUILayout.Button("Build Object"))
        {
            map.GenerateMap();
        }
    }
}

using UnityEngine;
using System.Collections;
using Features.Tiles;
using Features.Tiles.Scripts;
using UnityEditor;

[CustomEditor(typeof(TileManager))]
public class ObjectBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TileManager myScript = (TileManager)target;
        if(GUILayout.Button("Update Placed Units Position"))
        {
            myScript.UpdateUnitsPlacedInScenePosition();
        }
    }
}

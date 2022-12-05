using UnityEngine;
using Features.Tiles;
using UnityEditor;

[CustomEditor(typeof(TileBaking))]
public class TileBakingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TileBaking myScript = (TileBaking)target;
        if(GUILayout.Button("Build Object"))
        {
            myScript.Bake();
        }
    }
}
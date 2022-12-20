using System;
using UnityEditor;
using UnityEngine;

namespace Features.Battle.Scripts.Editor
{
    [CustomEditor(typeof(UnitTeamData_SO))]
    public class UnitTeamDataEditor : UnityEditor.Editor
    {
        private SerializedProperty _ownerNetworkedPlayerUnits;
        private SerializedProperty _ownTeamRuntimeSet;
        private SerializedProperty _battleData;
        
        private void OnEnable()
        {
            _battleData = serializedObject.FindProperty("battleData");
            _ownerNetworkedPlayerUnits = serializedObject.FindProperty("ownerNetworkedPlayerUnits");
            _ownTeamRuntimeSet = serializedObject.FindProperty("ownTeamRuntimeSet");
        }

        public override void OnInspectorGUI()
        {
            UnitTeamData_SO teamData = target as UnitTeamData_SO;

            if (teamData == null) return;
            
            EditorGUILayout.PropertyField(_battleData,
                new GUIContent("OBattle Data"));

            teamData.isAI = EditorGUILayout.Toggle("is AI", teamData.isAI);

            if (!teamData.isAI)
            {
                EditorGUILayout.PropertyField(_ownerNetworkedPlayerUnits,
                    new GUIContent("Owner Networked Player Units"));
            }
            
            EditorGUILayout.PropertyField(_ownTeamRuntimeSet,
                new GUIContent("Owner Team Runtime Set"));
        }
    }
}

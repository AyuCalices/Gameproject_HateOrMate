using System;
using Features.Unit.Scripts.Behaviours.Services.UnitStats;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BaseStatsData_SO))]
[CanEditMultipleObjects]
public class BaseStatsDataEditor : Editor
{
    private BaseStatsData_SO _baseStatsData;

    private float OrientationHealth => _baseStatsData.orientationHealth * _baseStatsData.expectedEnemyClientSize;
    private float OrientationKillTime => _baseStatsData.orientationKillTime * _baseStatsData.expectedEnemyClientSize;
    
    private void OnEnable()
    {
        _baseStatsData = (BaseStatsData_SO) target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();
        
        GUI.enabled = false;

        float dps = _baseStatsData.attackBaseValue / _baseStatsData.speedValue;
        EditorGUILayout.FloatField("Current DPS", dps);
        EditorGUILayout.FloatField("Suiciding Time in Seconds", _baseStatsData.healthBaseValue / dps);

        float healthScaledOrientationTime = _baseStatsData.healthBaseValue / OrientationHealth *
                                            OrientationKillTime;
        EditorGUILayout.FloatField("Balanced Attack", OrientationHealth / healthScaledOrientationTime * _baseStatsData.speedValue);
        
        GUI.enabled = true;
    }
}

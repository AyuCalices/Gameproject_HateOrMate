using System;
using System.Collections.Generic;
using Features.Connection.Scripts.Utils;
using UnityEngine;

namespace Features.Unit.Scripts
{
    [CreateAssetMenu]
    public class UnitType_SO : NetworkedScriptableObject
    {
        public string unitName;
        
        [Header("How much damage will this unit get when being attacked by a certain unit type.")]
        [SerializeField] private List<UnitDamageRelation> unitRelation;

        public void GetDamageByUnitRelations(UnitType_SO attackerUnitType, ref float unrelatedDamage)
        {
            foreach (UnitDamageRelation relation in unitRelation)
            {
                if (relation.unitType == attackerUnitType)
                {
                    Debug.Log($"Gained Damage Scale by '{attackerUnitType.name}' is '{relation.scale}' resolving into '{(unrelatedDamage * relation.scale)}' damage");
                    unrelatedDamage *= relation.scale;
                    return;
                }
            }
            
            Debug.Log($"Gained Damage Scale by '{attackerUnitType.name}' is '{1}' resolving into '{unrelatedDamage}' damage");
        }
    }

    [Serializable]
    public class UnitDamageRelation
    {
        public UnitType_SO unitType;
        public float scale;
    }
}
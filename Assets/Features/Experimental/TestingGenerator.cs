using System;
using Features.GlobalReferences;
using Features.GlobalReferences.Scripts;
using Features.Loot;
using Features.Loot.Scripts;
using Features.Mod;
using Features.ModView;
using Features.Unit.Classes;
using UnityEngine;

namespace Features.Experimental
{
    public class TestingGenerator : MonoBehaviour
    {
        public static Action<string, UnitClassData_SO> onSpawnUnit;

        [SerializeField] private UnitClassData_SO unitClassDataSo;
        [SerializeField] private LootableGenerator_SO modGenerator;

        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                modGenerator.OnAddInstanceToPlayer();
            }
        
            if (Input.GetKeyDown(KeyCode.U))
            {
                onSpawnUnit.Invoke("Player", unitClassDataSo);
            }
        }
    }
}

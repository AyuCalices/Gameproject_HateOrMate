using System;
using Features.GlobalReferences;
using Features.Loot;
using Features.Mod;
using Features.ModView;
using UnityEngine;

namespace Features.Experimental
{
    public class TestingGenerator : MonoBehaviour
    {
        public static Action onSpawnUnit;

        [SerializeField] private LootableGenerator_SO modGenerator;
        [SerializeField] private NetworkedUnitRuntimeSet_SO localUnitRuntimeSet;
        [SerializeField] private ModDragBehaviour modDragBehaviourPrefab;

        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                modGenerator.OnAddInstanceToPlayer();
            }
        
            if (Input.GetKeyDown(KeyCode.U))
            {
                onSpawnUnit.Invoke();
            }
        }
    }
}

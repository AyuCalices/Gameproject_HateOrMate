using System;
using Features.GlobalReferences;
using Features.Mod;
using Features.ModView;
using Features.Unit;
using Features.Unit.GridMovement;
using Features.Unit.Modding;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Features.Experimental
{
    public class TestingGenerator : MonoBehaviour
    {
        public static Action onSpawnUnit;

        [SerializeField] private BaseModGenerator_SO modGenerator;
        [SerializeField] private NetworkedUnitRuntimeSet_SO localUnitRuntimeSet;
        [SerializeField] private ModDragBehaviour modDragBehaviourPrefab;

        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                localUnitRuntimeSet.TryInstantiateModToAny(modDragBehaviourPrefab, modGenerator.Generate());
            }
        
            if (Input.GetKeyDown(KeyCode.U))
            {
                onSpawnUnit.Invoke();
            }
        }
    }
}

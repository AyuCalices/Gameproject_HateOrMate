using System;
using Features.GlobalReferences;
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
        
        [SerializeField] private NetworkedUnitRuntimeSet_SO localUnitRuntimeSet;
        [SerializeField] private ModDragBehaviour modDragBehaviourPrefab;

        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                ModDragBehaviour modDragBehaviour = Instantiate(modDragBehaviourPrefab);
                modDragBehaviour.GetComponent<Image>().color = Random.ColorHSV();
                localUnitRuntimeSet.TryAddModToAny(modDragBehaviour);
            }
        
            if (Input.GetKeyDown(KeyCode.U))
            {
                onSpawnUnit.Invoke();
            }
        }
    }
}

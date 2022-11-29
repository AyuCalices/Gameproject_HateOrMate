using System;
using Features.Grid;
using Features.ModView;
using Features.Unit;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Features.Experimental
{
    public class TestingGenerator : MonoBehaviour
    {
        public static Func<LocalUnitBehaviour> onSpawnUnit;
        
        [SerializeField] private LocalUnitRuntimeSet_SO localUnitRuntimeSet;
        [SerializeField] private ModDragBehaviour modDragBehaviourPrefab;
        [SerializeField] private GridManager gridManager;

        
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
                LocalUnitBehaviour localUnitBehaviour = onSpawnUnit.Invoke();
                gridManager.AddUnitToRandom(localUnitBehaviour);
            }
        }
    }
}

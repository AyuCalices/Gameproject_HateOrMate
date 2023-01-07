using DataStructures.ReactiveVariable;
using Features.Loot.Scripts;
using UnityEngine;

namespace Features.DevCheats
{
    public class InputCheats : MonoBehaviour
    {
        [SerializeField] private LootTable_SO lootTable;
        [SerializeField] private IntReactiveVariable stageVariable;
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                lootTable.RandomizeLootableGenerator().OnAddInstanceToPlayer(stageVariable.Get());
            }
        }
    }
}

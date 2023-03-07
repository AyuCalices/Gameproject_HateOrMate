using System;
using System.Collections.Generic;
using DataStructures.ReactiveVariable;
using Features.Loot.Scripts;
using UnityEngine;

namespace Features.General.DevCheats.Scripts
{
    public class InputCheats : MonoBehaviour
    {
        [SerializeField] private List<LootableCheatKeyBinding> lootableCheats;
        [SerializeField] private IntReactiveVariable stageVariable;
        
        private void Update()
        {
            foreach (LootableCheatKeyBinding lootableCheatKeyBinding in lootableCheats)
            {
                if (Input.GetKeyDown(lootableCheatKeyBinding.keyCode))
                {
                    lootableCheatKeyBinding.lootTable.RandomizeLootableGenerator().OnAddInstanceToPlayer(stageVariable.Get());
                }
            }
        }
    }

    [Serializable]
    public class LootableCheatKeyBinding
    {
        public LootTable_SO lootTable;
        public KeyCode keyCode;
    }
}

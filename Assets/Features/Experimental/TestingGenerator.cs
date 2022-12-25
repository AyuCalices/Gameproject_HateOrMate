using System;
using Features.Battle.Scripts;
using Features.Loot.Scripts;
using Features.Unit.Classes;
using UnityEngine;

namespace Features.Experimental
{
    public class TestingGenerator : MonoBehaviour
    {
        public static Action<string, UnitClassData_SO, SynchronizedBaseStats> onNetworkedSpawnUnit;

        [SerializeField] private LootTable_SO lootTable;
        [SerializeField] private UnitClassData_SO unitClassDataSo;

        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                lootTable.RandomizeLootableGenerator().OnAddInstanceToPlayer();
            }
        
            if (Input.GetKeyDown(KeyCode.U))
            {
                onNetworkedSpawnUnit.Invoke("Player", unitClassDataSo, new SynchronizedBaseStats(10, 50, 3));
            }
        }
    }
}

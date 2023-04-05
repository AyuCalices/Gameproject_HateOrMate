using System.Collections.Generic;
using Features.BattleScene.Scripts;
using Features.Mods.Scripts.ModTypes;
using Features.Unit.Scripts;
using Features.Unit.Scripts.Behaviours.Services.BattleBehaviour;
using Photon.Pun;
using UnityEngine;

namespace Features.MovementAndSpawning.Scripts
{
    public class PlayerUnitSpawner : MonoBehaviourPunCallbacks
    {
        [SerializeField] private List<SpawnerInstance> spawnerInstances;
    
        public override void OnEnable()
        {
            base.OnEnable();
        
            //ordered by logical order
            BattleManager.onLocalSpawnUnit += PlayerSynchronizedSpawn;
            UnitMod.onAddUnit += PlayerSynchronizedSpawn;
        }

        public override void OnDisable()
        {
            base.OnDisable();
        
            //ordered by logical order
            BattleManager.onLocalSpawnUnit -= PlayerSynchronizedSpawn;
            UnitMod.onAddUnit -= PlayerSynchronizedSpawn;
        }

        private UnitBattleBehaviour PlayerSynchronizedSpawn(string spawnerReference, UnitClassData_SO unitClassData, int level)
        {
            return SpawnHelper.PhotonSpawnUnit(spawnerInstances, spawnerReference, unitClassData, level, true);
        }
    }
}
    
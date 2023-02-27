using System.Collections.Generic;
using Features.Battle.Scripts;
using Features.Loot.Scripts.GeneratedLoot;
using Features.MovementAndSpawning;
using Features.Unit.Scripts;
using Features.Unit.Scripts.Behaviours.Battle;
using Photon.Pun;
using UnityEngine;

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

    private NetworkedBattleBehaviour PlayerSynchronizedSpawn(string spawnerReference, UnitClassData_SO unitClassData, int level)
    {
        return SpawnHelper.PhotonSpawnUnit(spawnerInstances, spawnerReference, unitClassData, level, true);
    }
}
    
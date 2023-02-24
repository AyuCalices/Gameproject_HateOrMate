using System;
using System.Collections.Generic;
using Features.Battle.Scripts;
using Features.Tiles.Scripts;
using Features.Unit.Scripts;
using Features.Unit.Scripts.Behaviours.Battle;
using Features.Unit.Scripts.Behaviours.Mod;
using Photon.Pun;
using UnityEngine;

namespace Features.MovementAndSpawning
{
    public class SpawnerInstance : MonoBehaviour
    {
        [SerializeField] private BattleData_SO battleData;

        public TeamTagType[] opponentTagType;
        public string reference;
        public NetworkedBattleBehaviour localPlayerPrefab;
        public TeamTagType[] ownSpawnsTeamTagType;
        public NetworkedBattleBehaviour networkedPlayerPrefab;
        public TeamTagType[] mateSpawnsTeamTagType;
        public bool isTargetable;

        public List<SpawnPosition> spawnPositions;


        public bool TryGetSpawnPosition(out KeyValuePair<Vector3Int, RuntimeTile> tileKeyValuePair)
        {
            foreach (SpawnPosition spawnPosition in spawnPositions)
            {
                if (spawnPosition.HasValidTile && !spawnPosition.SpawnerInstanceTile.ContainsUnit)
                {
                    tileKeyValuePair = new KeyValuePair<Vector3Int, RuntimeTile>(spawnPosition.GridPosition, spawnPosition.SpawnerInstanceTile);
                    return true;
                }
            }

            tileKeyValuePair = default;
            return false;
        }

        public NetworkedBattleBehaviour PhotonInstantiate(int photonActorNumber, UnitClassData_SO unitClassData, Vector3Int gridPosition, int index, int level)
        {
            object[] data =
            {
                photonActorNumber,
                unitClassData,
                gridPosition,
                index,
                level,
                Array.ConvertAll(ownSpawnsTeamTagType, value => (int) value),
                Array.ConvertAll(mateSpawnsTeamTagType, value => (int) value),
                Array.ConvertAll(opponentTagType, value => (int) value),
                isTargetable
            };
            NetworkedBattleBehaviour instantiatedDamageAnimatorBehaviour = PhotonNetwork
                .Instantiate("Unit", battleData.TileRuntimeDictionary.GetCellToWorldPosition(gridPosition), Quaternion.identity, 0, data)
                .GetComponent<NetworkedBattleBehaviour>();
            
            return instantiatedDamageAnimatorBehaviour;
        }
        
        public NetworkedBattleBehaviour InstantiateAndInitialize(int photonActorNumber, UnitClassData_SO unitClassData, Vector3Int gridPosition, int index, int level)
        {
            bool isOwner = PhotonNetwork.LocalPlayer.ActorNumber == photonActorNumber;
            NetworkedBattleBehaviour selectedPrefab = isOwner ? localPlayerPrefab : networkedPlayerPrefab;
            TeamTagType[] teamTagType = isOwner ? ownSpawnsTeamTagType : mateSpawnsTeamTagType;
            
            NetworkedBattleBehaviour instantiatedUnit = Instantiate(selectedPrefab, transform);
            
            //initialize values
            if (unitClassData.sprite != null)
            {
                instantiatedUnit.SetSprite(unitClassData.sprite);
            }
            instantiatedUnit.SetTeamTagType(teamTagType, opponentTagType);
            instantiatedUnit.IsTargetable = isTargetable;
            instantiatedUnit.SpawnerInstanceIndex = index;


            instantiatedUnit.UnitClassData = unitClassData;
            
            instantiatedUnit.NetworkedStatsBehaviour.SetBaseStats(unitClassData.baseStatsData, level);
            
            
            if (battleData.TileRuntimeDictionary.TryGetByGridPosition(gridPosition, out RuntimeTile tileBehaviour))
            {
                tileBehaviour.AddUnit(instantiatedUnit.gameObject);
            }
            
            instantiatedUnit.transform.position = battleData.TileRuntimeDictionary.GetCellToWorldPosition(gridPosition);
            return instantiatedUnit;
        }
    }
}

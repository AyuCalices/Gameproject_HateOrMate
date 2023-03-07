using System;
using System.Collections.Generic;
using Features.BattleScene.Scripts;
using Features.Tiles.Scripts;
using Features.Unit.Scripts;
using Features.Unit.Scripts.Behaviours.Services.BattleBehaviour;
using Photon.Pun;
using UnityEngine;

namespace Features.MovementAndSpawning.Scripts
{
    public class SpawnerInstance : MonoBehaviour
    {
        [SerializeField] private BattleData_SO battleData;

        public TeamTagType[] opponentTagType;
        public string reference;
        public TeamTagType[] ownSpawnsTeamTagType;
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

        public UnitBattleBehaviour PhotonInstantiate(UnitClassData_SO unitClassData, Vector3Int gridPosition, int level, bool isBenched)
        {
            object[] data =
            {
                unitClassData,
                level,
                Array.ConvertAll(ownSpawnsTeamTagType, value => (int) value),
                Array.ConvertAll(mateSpawnsTeamTagType, value => (int) value),
                Array.ConvertAll(opponentTagType, value => (int) value),
                isTargetable,
                isBenched
            };
            UnitBattleBehaviour instantiatedDamageAnimatorBehaviour = PhotonNetwork
                .Instantiate("Unit", battleData.TileRuntimeDictionary.GetCellToWorldPosition(gridPosition), Quaternion.identity, 0, data)
                .GetComponent<UnitBattleBehaviour>();
            
            return instantiatedDamageAnimatorBehaviour;
        }
    }
}

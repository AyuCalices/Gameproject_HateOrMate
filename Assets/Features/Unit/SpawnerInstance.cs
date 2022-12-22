using System.Collections.Generic;
using Features.Battle.Scripts;
using Features.Tiles;
using Features.Unit.Battle.Scripts;
using Features.Unit.Classes;
using Photon.Pun;
using UnityEngine;

namespace Features.Unit
{
    public class SpawnerInstance : MonoBehaviour
    {
        [SerializeField] private BattleData_SO battleData;
        
        public string reference;
        public NetworkedBattleBehaviour localPlayerPrefab;
        public UnitTeamData_SO localPlayerTeamData;
        public NetworkedBattleBehaviour networkedPlayerPrefab;
        public UnitTeamData_SO networkedPlayerTeamData;
        public bool isTargetable;

        public List<SpawnPosition> spawnPositions;
        

        private readonly List<PhotonView> _spawnedUnits = new List<PhotonView>();


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

        private SpawnPosition GetSpawnPositionByGrid(Vector3Int gridPosition)
        {
            return spawnPositions.Find(x => x.GridPosition == gridPosition);
        }
        
        public NetworkedBattleBehaviour InstantiateAndInitialize(NetworkedBattleBehaviour selectedPrefab, UnitTeamData_SO unitTeamData, UnitClassData_SO unitClassData, Vector3Int gridPosition)
        {
            NetworkedBattleBehaviour player = Instantiate(selectedPrefab, transform);
            _spawnedUnits.Add(player.PhotonView);
            
            //initialize values
            player.UnitTeamData = unitTeamData;
            player.IsTargetable = isTargetable;
            if (player.TryGetComponent(out BattleBehaviour battleBehaviour))
            {
                battleBehaviour.UnitClassData = unitClassData;
            }
            
            if (battleData.TileRuntimeDictionary.TryGetByGridPosition(gridPosition, out RuntimeTile tileBehaviour))
            {
                tileBehaviour.AddUnit(player.gameObject);
            }
            
            player.transform.position = battleData.TileRuntimeDictionary.GetCellToWorldPosition(gridPosition);
            return player;
        }

        public void DestroyByReference(PhotonView playerPhotonView)
        {
            if (!_spawnedUnits.Contains(playerPhotonView)) return;

            Destroy(playerPhotonView);
        }

        private void Destroy(PhotonView playerPhotonView)
        {
            Vector3Int gridPosition = battleData.TileRuntimeDictionary.GetWorldToCellPosition(playerPhotonView.transform.position);
            if (battleData.TileRuntimeDictionary.TryGetByGridPosition(gridPosition, out RuntimeTile tileBehaviour))
            {
                tileBehaviour.RemoveUnit();
            }
            
            _spawnedUnits.Remove(playerPhotonView);
            Destroy(playerPhotonView.gameObject);
        }

        public bool TryDestroy(int viewID)
        {
            PhotonView photonView = _spawnedUnits.Find(x => x.ViewID == viewID);
            if (photonView == null)
            {
                return false;
            }

            Destroy(photonView);
            return true;
        }

        public void DestroyAll()
        {
            for (int index = _spawnedUnits.Count - 1; index >= 0; index--)
            {
                PhotonView spawnedUnit = _spawnedUnits[index];
                Destroy(spawnedUnit);
            }
        }
    }
}

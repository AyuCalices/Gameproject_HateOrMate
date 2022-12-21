using System;
using System.Collections.Generic;
using Features.Battle.Scripts;
using Features.Tiles;
using Features.Unit.Battle.Scripts;
using Features.Unit.Classes;
using Features.Unit.Modding;
using Photon.Pun;
using ToolBox.Pools;
using UnityEngine;

namespace Features.Unit
{
    public class SpawnerInstance : MonoBehaviour
    {
        [SerializeField] private BattleData_SO battleData;
        [SerializeField] private SpawnPosition spawnPosition;
        
        public string reference;
        public NetworkedBattleBehaviour localPlayerPrefab;
        public UnitTeamData_SO localPlayerTeamData;
        public NetworkedBattleBehaviour networkedPlayerPrefab;
        public UnitTeamData_SO networkedPlayerTeamData;
        public bool isTargetable;
        

        private List<PhotonView> _spawnedUnits = new List<PhotonView>();

        private RuntimeTile _spawnerInstanceTile;
        private Vector3Int _gridPosition;
        private bool _hasValidTile;

        private void Awake()
        {
            _gridPosition = battleData.TileRuntimeDictionary.GetWorldToCellPosition(transform.position);
            _hasValidTile = battleData.TileRuntimeDictionary.TryGetByGridPosition(_gridPosition,
                out _spawnerInstanceTile);
        }

        public bool TryGetSpawnPosition(out KeyValuePair<Vector3Int, RuntimeTile> tileKeyValuePair)
        {
            switch (spawnPosition)
            {
                case SpawnPosition.RandomPlaceablePosition:
                    if (battleData.TileRuntimeDictionary.TryGetRandomPlaceableTileBehaviour(
                        out tileKeyValuePair)) return true;
                    break;
                
                case SpawnPosition.ThisTransform:
                    if (_hasValidTile)
                    {
                        tileKeyValuePair = new KeyValuePair<Vector3Int, RuntimeTile>(_gridPosition, _spawnerInstanceTile);
                        return true;
                    }
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }

            tileKeyValuePair = default;
            return false;
        }
        
        public NetworkedBattleBehaviour InstantiateAndInitialize(NetworkedBattleBehaviour selectedPrefab, UnitTeamData_SO unitTeamData, UnitClassData_SO unitClassData, Vector3Int gridPosition)
        {
            NetworkedBattleBehaviour player = selectedPrefab.gameObject.Reuse<NetworkedBattleBehaviour>(transform);
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
            if (_spawnedUnits.Contains(playerPhotonView))
            {
                _spawnedUnits.Remove(playerPhotonView);
                playerPhotonView.gameObject.Release();
            }
        }

        public bool TryDestroy(int viewID)
        {
            PhotonView photonView = _spawnedUnits.Find(x => x.ViewID == viewID);
            if (photonView == null)
            {
                return false;
            }
            
            photonView.gameObject.Release();
            return true;
        }

        public void DestroyAll()
        {
            foreach (PhotonView spawnedUnit in _spawnedUnits)
            {
                spawnedUnit.gameObject.Release();
            }
        }
    }
}

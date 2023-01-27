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
        
        public NetworkedBattleBehaviour InstantiateAndInitialize(int photonActorNumber, UnitClassData_SO unitClassData, Vector3Int gridPosition, int index)
        {
            bool isOwner = PhotonNetwork.LocalPlayer.ActorNumber == photonActorNumber;
            NetworkedBattleBehaviour selectedPrefab = isOwner ? localPlayerPrefab : networkedPlayerPrefab;
            TeamTagType[] teamTagType = isOwner ? ownSpawnsTeamTagType : mateSpawnsTeamTagType;
            
            NetworkedBattleBehaviour instantiatedUnit = Instantiate(selectedPrefab, transform);
            _spawnedUnits.Add(instantiatedUnit.PhotonView);
            
            //initialize values
            if (unitClassData.sprite != null)
            {
                instantiatedUnit.SetSprite(unitClassData.sprite);
            }
            instantiatedUnit.SetTeamTagType(teamTagType, opponentTagType);
            instantiatedUnit.IsTargetable = isTargetable;
            instantiatedUnit.SpawnerInstanceIndex = index;
            instantiatedUnit.NetworkedStatsBehaviour.SetBaseStats(unitClassData.baseStatsGenerator, unitClassData.baseStatsData);
            if (instantiatedUnit.TryGetComponent(out BattleBehaviour battleBehaviour))
            {
                battleBehaviour.UnitClassData = unitClassData;
            }

            if (instantiatedUnit.TryGetComponent(out ModUnitBehaviour modUnitBehaviour))
            {
                modUnitBehaviour.InstantiateModView();
            }
            
            if (battleData.TileRuntimeDictionary.TryGetByGridPosition(gridPosition, out RuntimeTile tileBehaviour))
            {
                tileBehaviour.AddUnit(instantiatedUnit.gameObject);
            }
            
            instantiatedUnit.transform.position = battleData.TileRuntimeDictionary.GetCellToWorldPosition(gridPosition);
            return instantiatedUnit;
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

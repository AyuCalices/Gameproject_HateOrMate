using DataStructures.StateLogic;
using Features.Tiles.Scripts;
using Features.Unit.Scripts.Behaviours.Services.BattleBehaviour;
using Photon.Pun;
using UnityEngine;

namespace Features.Unit.Scripts.Behaviours.States
{
    public class BenchedState : IState
    {
        private readonly UnitBattleBehaviour _battleBehaviour;

        public BenchedState(UnitBattleBehaviour battleBehaviour)
        {
            _battleBehaviour = battleBehaviour;
        }

        public void Enter()
        {
            if (!_battleBehaviour.UnitServiceProvider.GetService<PhotonView>().IsMine)
            {
                _battleBehaviour.gameObject.SetActive(false);
            }
            else
            {
                Vector3Int gridPosition = _battleBehaviour.battleData.TileRuntimeDictionary.GetWorldToCellPosition(_battleBehaviour.transform.position);
                if (_battleBehaviour.battleData.TileRuntimeDictionary.TryGetByGridPosition(gridPosition, out RuntimeTile tileBehaviour))
                {
                    tileBehaviour.AddUnit(_battleBehaviour.gameObject);
                }
            }
        }

        public void Execute() { }

        public void Exit()
        {
            if (!_battleBehaviour.gameObject.activeSelf)
            {
                _battleBehaviour.gameObject.SetActive(true);
            }
        }
    }
}

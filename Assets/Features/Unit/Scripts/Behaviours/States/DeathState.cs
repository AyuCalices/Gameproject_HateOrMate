using System;
using DataStructures.StateLogic;
using Features.BattleScene.Scripts;
using Features.Tiles.Scripts;
using Features.Unit.Scripts.Behaviours.Services.BattleBehaviour;
using UnityEngine;

namespace Features.Unit.Scripts.Behaviours.States
{
    public class DeathState : IState
    {
        public static Action onUnitEnterDeathState;
        
        private readonly UnitBattleBehaviour _battleBehaviour;
        private readonly BattleData_SO _battleData;
        
        public DeathState(UnitBattleBehaviour battleBehaviour, BattleData_SO battleData)
        {
            _battleBehaviour = battleBehaviour;
            _battleData = battleData;
        }

        public void Enter()
        {
            onUnitEnterDeathState?.Invoke();
            _battleBehaviour.gameObject.SetActive(false);
            
            Vector3Int currentCellPosition = GridPositionHelper.GetCurrentCellPosition(_battleData, _battleBehaviour.UnitServiceProvider.transform);
            GridPositionHelper.RemoveUnitFromRuntimeTiles(_battleData, currentCellPosition);
        }

        public void Execute()
        {
        }

        public void Exit()
        {
            _battleBehaviour.gameObject.SetActive(true);
        }
    }
}

using System;
using DataStructures.StateLogic;
using Features.Tiles.Scripts;
using Features.Unit.Scripts.Behaviours.Services.BattleBehaviour;
using UnityEngine;

namespace Features.Unit.Scripts.Behaviours.States
{
    public class MovementState : IState
    {
        public static Action<UnitServiceProvider, Vector3Int, int> onPerformMovement;
        
        private readonly UnitServiceProvider _unitServiceProvider;
        private readonly Vector3Int _targetPosition;
        private readonly int _skipLastMovementsCount;
        private readonly TileRuntimeDictionary_SO _tileRuntimeDictionary;

        public MovementState(UnitServiceProvider unitServiceProvider, Vector3Int targetPosition, int skipLastMovementsCount, TileRuntimeDictionary_SO tileRuntimeDictionary)
        {
            _unitServiceProvider = unitServiceProvider;
            _targetPosition = targetPosition;
            _skipLastMovementsCount = skipLastMovementsCount;
            _tileRuntimeDictionary = tileRuntimeDictionary;
        }

        public void Enter()
        {
            Vector3Int originPosition = _tileRuntimeDictionary.GetWorldToCellPosition(_unitServiceProvider.transform.position);
            if (_tileRuntimeDictionary.GenerateAStarPath(originPosition, _targetPosition, out _))
            {
                onPerformMovement.Invoke(_unitServiceProvider, _targetPosition, _skipLastMovementsCount);
            }
            else
            {
                _unitServiceProvider.GetService<UnitBattleBehaviour>().ForceIdleState();
            }
        }

        public void Execute()
        {
        }

        public void Exit()
        {  
        }
    }
}

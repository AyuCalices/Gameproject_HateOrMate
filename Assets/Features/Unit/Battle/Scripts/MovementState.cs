using System;
using System.Collections.Generic;
using DataStructures.StateLogic;
using ExitGames.Client.Photon;
using Features.Tiles;
using Features.Unit.Battle.Scripts.CanMoveAction;
using Features.Unit.GridMovement;
using Features.Unit.Modding;
using Photon.Pun;
using Photon.Realtime;
using ThirdParty.LeanTween.Framework;
using UnityEngine;

namespace Features.Unit.Battle.Scripts
{
    public class MovementState : IState
    {
        private readonly IsMovable_SO _movableAction;
        private readonly BattleBehaviour _battleBehaviour;
        private readonly Vector3Int _targetPosition;
        private readonly int _skipLastMovementsCount;

        

        public MovementState(IsMovable_SO movableAction, BattleBehaviour battleBehaviour, Vector3Int targetPosition, int skipLastMovementsCount)
        {
            _movableAction = movableAction;
            _battleBehaviour = battleBehaviour;
            _targetPosition = targetPosition;
            _skipLastMovementsCount = skipLastMovementsCount;
        }

        public void Enter()
        {
            _movableAction.RequestMove(_battleBehaviour, _targetPosition, _skipLastMovementsCount);
        }

        public void Execute()
        {
        }

        public void Exit()
        {  
        }
    }
}

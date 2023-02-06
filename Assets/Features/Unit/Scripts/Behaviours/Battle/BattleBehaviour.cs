using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Features.Battle.Scripts;
using Features.Battle.StateMachine;
using Features.Unit.Scripts.Behaviours.Stat;
using Features.Unit.Scripts.Class;
using Features.Unit.Scripts.View;
using UnityEngine;

namespace Features.Unit.Scripts.Behaviours.Battle
{
    [RequireComponent(typeof(NetworkedStatsBehaviour), typeof(UnitBattleView))]
    public class BattleBehaviour : NetworkedBattleBehaviour
    {
        private BattleClass _battleClass;
        public BattleClass BattleClass => _battleClass;

        private UnitClassData_SO _unitClassData;
        public UnitClassData_SO UnitClassData
        {
            get => _unitClassData;
            set
            {
                _unitClassData = value;
                _battleClass = UnitClassData.battleClasses.Generate(value.baseDamageAnimationBehaviour, NetworkedStatsBehaviour, this, unitBattleView);
            }
        }

        private KeyValuePair<NetworkedBattleBehaviour, float> _closestUnit;
        
        public KeyValuePair<NetworkedBattleBehaviour, float> GetTarget => _closestUnit;
        private bool HasTarget { get; set; }
        private bool TargetInRange => _closestUnit.Value < NetworkedStatsBehaviour.NetworkedStatServiceLocator.GetTotalValue_CheckMin(StatType.Range);
        public float MovementSpeed => NetworkedStatsBehaviour.NetworkedStatServiceLocator.GetTotalValue_CheckMin(StatType.MovementSpeed);

        public override void OnStageEnd()
        {
            base.OnStageEnd();
            
            if (CurrentState is AttackState)
            {
                ForceIdleState();
            }

            _battleClass.OnStageEnd();
        }

        private void Update()
        {
            if (!battleData.StateIsValid(typeof(BattleState), StateProgressType.Execute)) return;

            List<NetworkedBattleBehaviour> enemyUnits = battleData.AllUnitsRuntimeSet.GetUnitsByTag(OpponentTagType);
            HasTarget = TryGetClosestTargetableByWorldPosition(enemyUnits, transform.position, out _closestUnit);

            stateMachine.Update();
        }
        
        private bool ContainsTargetable(ref List<NetworkedBattleBehaviour> networkedUnitBehaviours)
        {
            networkedUnitBehaviours.RemoveAll(e => !e.IsTargetable || e.CurrentState is DeathState || e.IsSpawnedLocally);

            return networkedUnitBehaviours.Count > 0;
        }
        
        private bool TryGetClosestTargetableByWorldPosition(List<NetworkedBattleBehaviour> networkedUnitBehaviours, Vector3 worldPosition, 
            out KeyValuePair<NetworkedBattleBehaviour, float> closestUnit)
        {
            if (!ContainsTargetable(ref networkedUnitBehaviours))
            {
                closestUnit = default;
                return false;
            }

            //get closest
            int closestUnitIndex = 0;
            float closestDistance = Vector3.Distance(worldPosition, networkedUnitBehaviours[0].transform.position);
            
            for (int index = 1; index < networkedUnitBehaviours.Count; index++)
            {
                float distanceNext = Vector3.Distance(worldPosition, networkedUnitBehaviours[index].transform.position);
                if (distanceNext < closestDistance)
                {
                    closestUnitIndex = index;
                    closestDistance = distanceNext;
                }
            }

            closestUnit = new KeyValuePair<NetworkedBattleBehaviour, float>(networkedUnitBehaviours[closestUnitIndex], closestDistance);
            return true;
        }

        #region Request States

        internal bool TryRequestIdleState()
        {
            bool result = !HasTarget && CurrentState is not DeathState;
            
            if (result)
            {
                stateMachine.ChangeState(new IdleState(this));
            }

            return result;
        }

        internal override bool TryRequestAttackState()
        {
            bool result = HasTarget && TargetInRange && !IsSpawnedLocally && CurrentState is IdleState && 
                          battleData.StateIsValid(typeof(BattleState), StateProgressType.Execute);
         
            if (result)
            {
                stateMachine.ChangeState(new AttackState(this, _battleClass));
            }

            return result;
        }

        internal override bool TryRequestMovementStateByClosestUnit()
        {
            bool result = HasTarget && !TargetInRange && CurrentState is IdleState && MovementSpeed > 0;

            if (result)
            {
                NetworkedBattleBehaviour closestStats = GetTarget.Key;
                Vector3Int enemyPosition = battleData.TileRuntimeDictionary.GetWorldToCellPosition(closestStats.transform.position);
                //TODO: magic number
                stateMachine.ChangeState(new MovementState( this, enemyPosition, 1, battleData.TileRuntimeDictionary));
            }

            return result;
        }
        
        internal override bool TryRequestDeathState()
        {
            bool result = battleData.StateIsValid(typeof(BattleState), StateProgressType.Execute);
            
            if (result)
            {
                stateMachine.ChangeState(new DeathState(this));
            }
            else
            {
                Debug.LogWarning("Requesting Death is only possible during Battle!");
            }

            return false;
        }

        #endregion
    }
}

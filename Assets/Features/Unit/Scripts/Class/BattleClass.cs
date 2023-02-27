using System;
using System.Globalization;
using System.Linq;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.StructWrapping;
using Features.Battle.Scripts;
using Features.Connection;
using Features.Connection.Scripts.Utils;
using Features.Loot.Scripts.ModView;
using Features.Unit.Scripts.Behaviours;
using Features.Unit.Scripts.Behaviours.Battle;
using Features.Unit.Scripts.Stats;
using Features.Unit.Scripts.View;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Features.Unit.Scripts.Class
{
    [Serializable]
    public abstract class BattleClass
    {
        protected readonly NetworkedStatsBehaviour ownerNetworkingStatsBehaviour;
        protected readonly NetworkedBattleBehaviour ownerBattleBehaviour;
        protected readonly UnitBattleView ownerUnitBattleView;

        protected BattleClass(NetworkedStatsBehaviour ownerNetworkingStatsBehaviour, NetworkedBattleBehaviour ownerBattleBehaviour, 
            UnitBattleView ownerUnitBattleView)
        {
            this.ownerNetworkingStatsBehaviour = ownerNetworkingStatsBehaviour;
            this.ownerUnitBattleView = ownerUnitBattleView;
            this.ownerBattleBehaviour = ownerBattleBehaviour;
        }
        
        protected void Attack_RaiseEvent(int targetID, float attackValue, float targetHealth, UnitClassData_SO attackerUnitClassData)
        {
            object[] data = new object[]
            {
                targetID,
                attackValue,
                targetHealth,
                attackerUnitClassData
            };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.All,
                CachingOption = EventCaching.AddToRoomCache
            };

            SendOptions sendOptions = new SendOptions
            {
                Reliability = true
            };

            PhotonNetwork.RaiseEvent((int)RaiseEventCode.OnAttack, data, raiseEventOptions, sendOptions);
        }
        
        public void AttackCallback(float attackValue, float targetHealth, UnitClassData_SO unitClassData)
        {
            ownerBattleBehaviour.UnitClassData.unitType.GetDamageByUnitRelations(unitClassData.unitType, ref attackValue);
            ownerNetworkingStatsBehaviour.RemovedHealth += attackValue;

            if (ownerNetworkingStatsBehaviour.RemovedHealth >= targetHealth && ownerBattleBehaviour.CurrentState is not DeathState)
            {
                ownerBattleBehaviour.TryRequestDeathState();
            }
        }

        public void InitializeBattleActions()
        {
            InternalInitializeBattleActions();
        }
        protected abstract void InternalInitializeBattleActions();
        
        
        public void UpdateBattleActions()
        {
            InternalUpdateBattleActions();
        }
        protected abstract void InternalUpdateBattleActions();

        
        public abstract void OnStageEnd();


        public void OnPerformAction()
        {
            InternalOnPerformAction();
        }
        protected abstract void InternalOnPerformAction();

        protected void SendAttack(NetworkedBattleBehaviour attackedNetworkedBattleBehaviour, float attackValue)
        {
            NetworkedStatsBehaviour attackedUnitStats = attackedNetworkedBattleBehaviour.NetworkedStatsBehaviour;

            Attack_RaiseEvent(attackedUnitStats.photonView.ViewID, attackValue,
                attackedUnitStats.GetFinalStat(StatType.Health), attackedNetworkedBattleBehaviour.UnitClassData);
        }
    }
}

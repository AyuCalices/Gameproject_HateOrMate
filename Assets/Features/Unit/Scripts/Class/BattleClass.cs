using System;
using System.Globalization;
using System.Linq;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.StructWrapping;
using Features.Battle.Scripts;
using Features.Battle.Scripts.Unit.ServiceLocatorSystem;
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
        protected readonly UnitServiceProvider ownerUnitServiceProvider;

        protected BattleClass(UnitServiceProvider ownerUnitServiceProvider)
        {
            this.ownerUnitServiceProvider = ownerUnitServiceProvider;
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
            ownerUnitServiceProvider.GetService<NetworkedBattleBehaviour>().UnitClassData.unitType.GetDamageByUnitRelations(unitClassData.unitType, ref attackValue);
            ownerUnitServiceProvider.GetService<NetworkedStatsBehaviour>().RemovedHealth += attackValue;

            if (ownerUnitServiceProvider.GetService<NetworkedStatsBehaviour>().RemovedHealth >= targetHealth && ownerUnitServiceProvider.GetService<NetworkedBattleBehaviour>().CurrentState is not DeathState)
            {
                ownerUnitServiceProvider.GetService<NetworkedBattleBehaviour>().TryRequestDeathState();
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

        protected void SendAttack(UnitServiceProvider targetUnitServiceProvider, float attackValue)
        {
            NetworkedStatsBehaviour targetUnitStats = targetUnitServiceProvider.GetService<NetworkedStatsBehaviour>();

            Attack_RaiseEvent(targetUnitStats.photonView.ViewID, attackValue,
                targetUnitStats.GetFinalStat(StatType.Health), targetUnitServiceProvider.GetService<NetworkedBattleBehaviour>().UnitClassData);
        }
    }
}

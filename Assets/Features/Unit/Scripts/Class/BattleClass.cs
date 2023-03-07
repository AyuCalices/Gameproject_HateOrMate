using System;
using ExitGames.Client.Photon;
using Features.General.Photon.Scripts;
using Features.Unit.Scripts.Behaviours;
using Features.Unit.Scripts.Behaviours.Services;
using Features.Unit.Scripts.Behaviours.Services.BattleBehaviour;
using Features.Unit.Scripts.Behaviours.Services.UnitStats;
using Features.Unit.Scripts.Behaviours.States;
using Photon.Pun;
using Photon.Realtime;

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
            UnitStatsBehaviour ownerUnitStatsBehaviour = ownerUnitServiceProvider.GetService<UnitStatsBehaviour>();
            
            ownerUnitServiceProvider.UnitClassData.unitType.GetDamageByUnitRelations(unitClassData.unitType, ref attackValue);
            ownerUnitStatsBehaviour.RemovedHealth += attackValue;
            ownerUnitServiceProvider.GetService<UnitBattleView>().SetHealthSlider(ownerUnitStatsBehaviour.RemovedHealth, targetHealth);
            ownerUnitServiceProvider.GetService<UnitBattleView>().InstantiateDamagePopup(attackValue);
            
            ownerUnitServiceProvider.onHitEvent.Raise();

            if (ownerUnitStatsBehaviour.RemovedHealth >= targetHealth && 
                ownerUnitServiceProvider.GetService<UnitBattleBehaviour>().CurrentState is not DeathState)
            {
                ownerUnitServiceProvider.GetService<UnitBattleBehaviour>().TryRequestDeathState();
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
            UnitStatsBehaviour targetUnitStats = targetUnitServiceProvider.GetService<UnitStatsBehaviour>();

            Attack_RaiseEvent(targetUnitStats.photonView.ViewID, attackValue,
                targetUnitStats.GetFinalStat(StatType.Health), targetUnitServiceProvider.UnitClassData);
        }
    }
}

using System;
using System.Globalization;
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
        protected readonly BattleBehaviour ownerBattleBehaviour;
        protected readonly UnitBattleView ownerUnitBattleView;

        protected BattleClass(NetworkedStatsBehaviour ownerNetworkingStatsBehaviour, BattleBehaviour ownerBattleBehaviour, 
            UnitBattleView ownerUnitBattleView, DamagePopup damagePopupPrefab, CanvasFocus_SO canvasFocus)
        {
            this.ownerNetworkingStatsBehaviour = ownerNetworkingStatsBehaviour;
            this.ownerUnitBattleView = ownerUnitBattleView;
            this.ownerBattleBehaviour = ownerBattleBehaviour;
        }
        
        /// <summary>
        /// Caster unit sends value to target. May be an attack or a heal.
        /// </summary>
        /// <param name="targetID"></param>
        /// <param name="value"></param>
        protected void SendFloatToTargetRaiseEvent(int targetID, float value, UnitClassData_SO attackerUnitClassData)
        {
            object[] data = new object[]
            {
                targetID,
                value,
                attackerUnitClassData
            };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.Others,
                CachingOption = EventCaching.AddToRoomCache
            };

            SendOptions sendOptions = new SendOptions
            {
                Reliability = true
            };

            PhotonNetwork.RaiseEvent((int)RaiseEventCode.OnSendFloatToTarget, data, raiseEventOptions, sendOptions);
        }
        
        /// <summary>
        /// Attacked Unit Receives an attack from the caster. May be an attack or heal.
        /// </summary>
        /// <param name="value"></param>
        public void OnReceiveFloatActionCallback(float value, UnitClassData_SO unitClassData)
        {
            ownerBattleBehaviour.UnitClassData.unitType.GetDamageByUnitRelations(unitClassData.unitType, ref value);
            ownerNetworkingStatsBehaviour.RemovedHealth += value;

            float totalHealth = ownerNetworkingStatsBehaviour.GetFinalStat(StatType.Health);
            ownerNetworkingStatsBehaviour.RaiseDamageGained(ownerBattleBehaviour, ownerNetworkingStatsBehaviour.RemovedHealth, totalHealth);
                
            UpdateAllClientsHealthRaiseEvent(
                ownerBattleBehaviour.PhotonView.ViewID,
                ownerBattleBehaviour.NetworkedStatsBehaviour.RemovedHealth,
                totalHealth
            );
        }
        
        /// <summary>
        /// Attacked Unit sends new Health value to all players.
        /// </summary>
        /// <param name="updateUnitID"></param>
        /// <param name="newCurrentHealth"></param>
        /// <param name="totalHealth"></param>
        private void UpdateAllClientsHealthRaiseEvent(int updateUnitID, float newCurrentHealth, float totalHealth)
        {
            object[] data = new object[]
            {
                updateUnitID,
                newCurrentHealth,
                totalHealth
            };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.Others,
                CachingOption = EventCaching.AddToRoomCache
            };

            SendOptions sendOptions = new SendOptions
            {
                Reliability = true
            };

            PhotonNetwork.RaiseEvent((int)RaiseEventCode.OnUpdateAllClientsHealth, data, raiseEventOptions, sendOptions);
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
            if (attackedNetworkedBattleBehaviour is BattleBehaviour attackedBattleBehaviour)
            {
                attackedBattleBehaviour.UnitClassData.unitType.GetDamageByUnitRelations(ownerBattleBehaviour.UnitClassData.unitType, ref attackValue);
                attackedUnitStats.RemovedHealth += attackValue;
                
                float totalHealth = attackedUnitStats.GetFinalStat(StatType.Health);
                ownerNetworkingStatsBehaviour.RaiseDamageGained(attackedNetworkedBattleBehaviour, attackedUnitStats.RemovedHealth, totalHealth);
                
                UpdateAllClientsHealthRaiseEvent(
                    attackedBattleBehaviour.PhotonView.ViewID,
                    attackedUnitStats.RemovedHealth,
                    totalHealth
                );
            }
            else
            {
                SendFloatToTargetRaiseEvent(
                    attackedNetworkedBattleBehaviour.PhotonView.ViewID,
                    attackValue,
                    ownerBattleBehaviour.UnitClassData
                );
            }
        }
    }
}

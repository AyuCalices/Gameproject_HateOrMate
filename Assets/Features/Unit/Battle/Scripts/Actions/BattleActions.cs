using System;
using ExitGames.Client.Photon;
using Features.GlobalReferences.Scripts;
using Features.Unit.Modding;
using Features.Unit.Modding.Stat;
using Features.Unit.View;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Features.Unit.Battle.Scripts.Actions
{
    [Serializable]
    public abstract class BattleActions
    {
        protected readonly NetworkedStatsBehaviour ownerNetworkingStatsBehaviour;
        protected readonly BattleBehaviour ownerBattleBehaviour;
        protected readonly UnitView ownerUnitView;


        protected BattleActions(NetworkedStatsBehaviour ownerNetworkingStatsBehaviour,
            BattleBehaviour ownerBattleBehaviour, UnitView ownerUnitView)
        {
            this.ownerNetworkingStatsBehaviour = ownerNetworkingStatsBehaviour;
            this.ownerUnitView = ownerUnitView;
            this.ownerBattleBehaviour = ownerBattleBehaviour;
        }
        
        /// <summary>
        /// Caster unit sends value to target. May be an attack or a heal.
        /// </summary>
        /// <param name="targetID"></param>
        /// <param name="value"></param>
        protected void SendFloatToTargetRaiseEvent(int targetID, float value)
        {
            object[] data = new object[]
            {
                targetID,
                value
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
        public void OnReceiveFloatActionCallback(float value)
        {
            //TODO: getComponent
            ownerBattleBehaviour.NetworkedStatsBehaviour.RemovedHealth += value;
            UpdateAllClientsHealthRaiseEvent(
                ownerBattleBehaviour.GetComponent<PhotonView>().ViewID,
                ownerBattleBehaviour.NetworkedStatsBehaviour.RemovedHealth,
                ownerBattleBehaviour.GetComponent<NetworkedStatsBehaviour>().NetworkedStatServiceLocator.GetTotalValue(StatType.Health)
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
                Receivers = ReceiverGroup.All,
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

        protected void SendAttack(NetworkedBattleBehaviour closestStats)
        {
            NetworkedStatsBehaviour networkedStats = closestStats.NetworkedStatsBehaviour;
            if (closestStats.GetComponent<BattleBehaviour>() != null)
            {
                //TODO: getComponent
                networkedStats.RemovedHealth += ownerNetworkingStatsBehaviour.NetworkedStatServiceLocator.GetTotalValue(StatType.Damage);
                UpdateAllClientsHealthRaiseEvent(
                    closestStats.GetComponent<PhotonView>().ViewID,
                    networkedStats.RemovedHealth,
                    networkedStats.NetworkedStatServiceLocator.GetTotalValue(StatType.Health)
                );
            }
            else
            {
                SendFloatToTargetRaiseEvent(
                    closestStats.PhotonView.ViewID,
                    ownerNetworkingStatsBehaviour.NetworkedStatServiceLocator.GetTotalValue(StatType.Damage)
                );
            }
        }
    }
}

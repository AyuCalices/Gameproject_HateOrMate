using System;
using ExitGames.Client.Photon;
using Features.Connection;
using Features.Connection.Scripts.Utils;
using Features.Unit.Scripts.Behaviours.Battle;
using Features.Unit.Scripts.Behaviours.Stat;
using Features.Unit.Scripts.View;
using Photon.Pun;
using Photon.Realtime;

namespace Features.Unit.Scripts.Class
{
    [Serializable]
    public abstract class BattleClass
    {
        protected readonly NetworkedStatsBehaviour ownerNetworkingStatsBehaviour;
        protected readonly BattleBehaviour ownerBattleBehaviour;
        protected readonly UnitBattleView ownerUnitBattleView;


        protected BattleClass(NetworkedStatsBehaviour ownerNetworkingStatsBehaviour,
            BattleBehaviour ownerBattleBehaviour, UnitBattleView ownerUnitBattleView)
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
            ownerBattleBehaviour.NetworkedStatsBehaviour.RemovedHealth += value;
            UpdateAllClientsHealthRaiseEvent(
                ownerBattleBehaviour.PhotonView.ViewID,
                ownerBattleBehaviour.NetworkedStatsBehaviour.RemovedHealth,
                ownerBattleBehaviour.NetworkedStatsBehaviour.NetworkedStatServiceLocator.GetTotalValue(StatType.Health)
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
                networkedStats.RemovedHealth += ownerNetworkingStatsBehaviour.NetworkedStatServiceLocator.GetTotalValue(StatType.Damage);
                UpdateAllClientsHealthRaiseEvent(
                    closestStats.PhotonView.ViewID,
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

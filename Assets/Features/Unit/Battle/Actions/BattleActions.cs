using System.Collections.Generic;
using ExitGames.Client.Photon;
using Features.GlobalReferences;
using Features.Unit.Modding;
using Features.Unit.Modding.Stat;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Features.Unit.Battle
{
    public abstract class BattleActions
    {
        protected readonly NetworkedUnitBehaviour ownerNetworkingUnitBehaviour;
        protected readonly BattleBehaviour ownerBattleBehaviour;
        protected readonly UnitView ownerUnitView;
        protected readonly NetworkedUnitRuntimeSet_SO opponentNetworkedUnitRuntimeSet;


        protected BattleActions(NetworkedUnitBehaviour ownerNetworkingUnitBehaviour,
            BattleBehaviour ownerBattleBehaviour, UnitView ownerUnitView,
            NetworkedUnitRuntimeSet_SO opponentNetworkedUnitRuntimeSet)
        {
            this.ownerNetworkingUnitBehaviour = ownerNetworkingUnitBehaviour;
            this.ownerUnitView = ownerUnitView;
            this.ownerBattleBehaviour = ownerBattleBehaviour;
            this.opponentNetworkedUnitRuntimeSet = opponentNetworkedUnitRuntimeSet;
        }
        
        /// <summary>
        /// Photon RaiseEvent for Networking sending a damage/heal value
        /// </summary>
        /// <param name="targetID">The Target PhotonView ID</param>
        /// <param name="value">The Damage/Heal value to be send</param>
        protected void SendAttackRaiseEvent(int targetID, float value)
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

            PhotonNetwork.RaiseEvent(RaiseEventCode.OnPerformUnitAttack, data, raiseEventOptions, sendOptions);
        }
        
        /// <summary>
        /// Photon RaiseEvent Callback for Networking acquiring a damage/heal value
        /// </summary>
        /// <param name="ownerBattleBehaviour"></param>
        /// <param name="value"></param>
        public void OnSendAttackActionCallback(float value)
        {
            ownerBattleBehaviour.RemovedHealth += value;
            RaiseSendHealthEvent(
                ownerBattleBehaviour.GetComponent<PhotonView>().ViewID,
                ownerBattleBehaviour.RemovedHealth,
                ownerBattleBehaviour.GetComponent<NetworkedUnitBehaviour>().NetworkedStatServiceLocator.GetTotalValue(StatType.Health)
            );
        }

        /// <summary>
        /// Photon RaiseEvent for Networking sending new health values
        /// </summary>
        /// <param name="updateUnitID"></param>
        /// <param name="newCurrentHealth"></param>
        /// <param name="totalHealth"></param>
        private void RaiseSendHealthEvent(int updateUnitID, float newCurrentHealth, float totalHealth)
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

            PhotonNetwork.RaiseEvent(RaiseEventCode.OnPerformUpdateUnitHealth, data, raiseEventOptions, sendOptions);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="updateUnit"></param>
        /// <param name="newRemovedHealth"></param>
        /// <param name="totalHealth"></param>
        public void OnSendHealthActionCallback(float newRemovedHealth,
            float totalHealth)
        {
            ownerBattleBehaviour.RemovedHealth = newRemovedHealth;
        
            if (ownerBattleBehaviour.RemovedHealth >= totalHealth)
            {
                Debug.LogWarning("DEAD!");
            }
        }
        
        
        public void UpdateBattleActions()
        {
            InternalUpdateBattleActions();
        }
        protected abstract void InternalUpdateBattleActions();
        
        

        //TODO: implement moving to enemy
        public void Move()
        {
            InternalMove();
        }
        protected virtual void InternalMove() {}
        
        
        
        public void OnPerformAction()
        {
            InternalOnPerformAction();
        }
        protected abstract void InternalOnPerformAction();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="casterControlType"></param>
        protected void PerformAttack(UnitControlType casterControlType)
        {
            KeyValuePair<NetworkedUnitBehaviour, float> closestUnit = ownerNetworkingUnitBehaviour.EnemyRuntimeSet
                .GetClosestByWorldPosition(ownerNetworkingUnitBehaviour.transform.position);
            
            BattleBehaviour targetBattleBehaviour = closestUnit.Key.GetComponent<BattleBehaviour>();
            if (!targetBattleBehaviour.IsTargetable) return;
            
            if ((casterControlType == UnitControlType.Master && closestUnit.Key.ControlType is UnitControlType.AI) 
                || (casterControlType == UnitControlType.AI && closestUnit.Key.ControlType is UnitControlType.Master))
            {
                
                targetBattleBehaviour.RemovedHealth += ownerNetworkingUnitBehaviour.NetworkedStatServiceLocator.GetTotalValue(StatType.Damage);
                RaiseSendHealthEvent(
                    closestUnit.Key.GetComponent<PhotonView>().ViewID,
                    targetBattleBehaviour.RemovedHealth,
                    closestUnit.Key.NetworkedStatServiceLocator.GetTotalValue(StatType.Health)
                );
            }
            else
            {
                SendAttackRaiseEvent(
                    closestUnit.Key.PhotonView.ViewID,
                    ownerNetworkingUnitBehaviour.NetworkedStatServiceLocator.GetTotalValue(StatType.Damage)
                );
            }
        }
    }
}

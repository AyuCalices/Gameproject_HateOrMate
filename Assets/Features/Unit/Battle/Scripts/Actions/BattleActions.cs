using ExitGames.Client.Photon;
using Features.GlobalReferences.Scripts;
using Features.Unit.Modding;
using Features.Unit.Modding.Stat;
using Features.Unit.View;
using Photon.Pun;
using Photon.Realtime;

namespace Features.Unit.Battle.Scripts.Actions
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

            PhotonNetwork.RaiseEvent((int)RaiseEventCode.OnPerformUnitAttack, data, raiseEventOptions, sendOptions);
        }
        
        /// <summary>
        /// Photon RaiseEvent Callback for Networking acquiring a damage/heal value
        /// </summary>
        /// <param name="ownerBattleBehaviour"></param>
        /// <param name="value"></param>
        public void OnSendAttackActionCallback(float value)
        {
            ownerBattleBehaviour.NetworkedUnitBehaviour.RemovedHealth += value;
            RaiseSendHealthEvent(
                ownerBattleBehaviour.GetComponent<PhotonView>().ViewID,
                ownerBattleBehaviour.NetworkedUnitBehaviour.RemovedHealth,
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

            PhotonNetwork.RaiseEvent((int)RaiseEventCode.OnPerformUpdateUnitHealth, data, raiseEventOptions, sendOptions);
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
            ownerBattleBehaviour.NetworkedUnitBehaviour.RemovedHealth = newRemovedHealth;
        
            if (ownerBattleBehaviour.NetworkedUnitBehaviour.RemovedHealth >= totalHealth)
            {
                ownerBattleBehaviour.TryRequestDeathState();
                ownerBattleBehaviour.NetworkedUnitBehaviour.StageCheck();
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
        
        public void Move()
        {
            InternalMove();
        }
        protected virtual void InternalMove() {}

        public abstract void OnStageEnd();
        
        public void OnPerformAction()
        {
            InternalOnPerformAction();
        }
        protected abstract void InternalOnPerformAction();

        protected void SendAttack(UnitControlType casterControlType, NetworkedUnitBehaviour closestUnit)
        {
            BattleBehaviour targetBattleBehaviour = closestUnit.GetComponent<BattleBehaviour>();
            if ((casterControlType == UnitControlType.Master && closestUnit.ControlType is UnitControlType.AI) 
                || (casterControlType == UnitControlType.AI && closestUnit.ControlType is UnitControlType.Master))
            {
                targetBattleBehaviour.NetworkedUnitBehaviour.RemovedHealth += ownerNetworkingUnitBehaviour.NetworkedStatServiceLocator.GetTotalValue(StatType.Damage);
                RaiseSendHealthEvent(
                    closestUnit.GetComponent<PhotonView>().ViewID,
                    targetBattleBehaviour.NetworkedUnitBehaviour.RemovedHealth,
                    closestUnit.NetworkedStatServiceLocator.GetTotalValue(StatType.Health)
                );
            }
            else
            {
                SendAttackRaiseEvent(
                    closestUnit.PhotonView.ViewID,
                    ownerNetworkingUnitBehaviour.NetworkedStatServiceLocator.GetTotalValue(StatType.Damage)
                );
            }
        }
    }
}

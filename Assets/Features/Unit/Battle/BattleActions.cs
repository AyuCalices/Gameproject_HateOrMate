using System.Collections.Generic;
using ExitGames.Client.Photon;
using Features.GlobalReferences;
using Features.Unit.Modding;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Features.Unit.Battle
{
    public abstract class BattleActions
    {
        protected readonly NetworkedUnitBehaviour ownerNetworkingUnitBehaviour;
        protected readonly NetworkedUnitRuntimeSet_SO opponentNetworkedUnitRuntimeSet;
        protected readonly UnitView unitView;
        
        public BattleActions(NetworkedUnitBehaviour ownerNetworkingUnitBehaviour, NetworkedUnitRuntimeSet_SO opponentNetworkedUnitRuntimeSet, UnitView unitView)
        {
            this.ownerNetworkingUnitBehaviour = ownerNetworkingUnitBehaviour;
            this.opponentNetworkedUnitRuntimeSet = opponentNetworkedUnitRuntimeSet;
            this.unitView = unitView;
        }
        
        
        //TODO: Tower: keep track of stamina + UI
        public void UpdateBattleActions()
        {
            InternalUpdateBattleActions(ownerNetworkingUnitBehaviour, unitView);
        }
        protected abstract void InternalUpdateBattleActions(NetworkedUnitBehaviour ownerNetworkingUnitBehaviour, UnitView unitView);
        
        

        //TODO: implement moving to enemy
        public void Move()
        {
            InternalMove(ownerNetworkingUnitBehaviour, opponentNetworkedUnitRuntimeSet);
        }
        protected virtual void InternalMove(NetworkedUnitBehaviour ownerNetworkingUnitBehaviour, NetworkedUnitRuntimeSet_SO opponentNetworkedUnitRuntimeSet) {}
        
        
        
        //TODO: implement Types: Clicker Damage, Idle Damage (Melee/Range), Heal
        //TODO: send event for stamina UI updated & Health value changed
        //TODO: send event for all client, what they should do (by RaiseEvent & maybe with eventCode for attack/Heal?)
        public void OnPerformAction()
        {
            InternalOnPerformAction(ownerNetworkingUnitBehaviour, opponentNetworkedUnitRuntimeSet);
        }
        protected abstract void InternalOnPerformAction(NetworkedUnitBehaviour ownerNetworkingUnitBehaviour, NetworkedUnitRuntimeSet_SO opponentNetworkedUnitRuntimeSet);



        
        public abstract void OnSendAttackActionCallback(BattleBehaviour targetBattleBehaviour, float value);

        protected void RaiseSendAttackEvent(int targetID, float value)
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
        
        public abstract void OnSendHealthActionCallback(BattleBehaviour updateUnit, float newRemovedHealth, float totalHealth);

        protected void RaiseSendHealthEvent(int updateUnitID, float newCurrentHealth, float totalHealth)
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
    }
}

using DataStructures.StateLogic;
using ExitGames.Client.Photon;
using Features.Connection;
using Features.Connection.Scripts.Utils;
using Features.Unit.Scripts.Behaviours;
using Features.Unit.Scripts.Behaviours.Battle;
using Features.Unit.Scripts.Behaviours.Stat;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Battle.Scripts
{
    public class BattleState : IState
    {
        private readonly BattleManager _battleManager;
        private readonly Button _requestLootPhaseButton;
        private readonly NetworkedUnitRuntimeSet_SO _allUnitsRuntimeSet;

        public BattleState(BattleManager battleManager, Button requestLootPhaseButton, NetworkedUnitRuntimeSet_SO allUnitsRuntimeSet)
        {
            _battleManager = battleManager;
            _requestLootPhaseButton = requestLootPhaseButton;
            _allUnitsRuntimeSet = allUnitsRuntimeSet;
        }
    
        public void Enter()
        {
            _requestLootPhaseButton.interactable = true;
            NetworkedStatsBehaviour.onDamageGained += CheckStage;
        }

        public void Execute()
        {
        }

        public void Exit()
        {
            NetworkedStatsBehaviour.onDamageGained -= CheckStage;
        }

        private void CheckStage(NetworkedBattleBehaviour networkedBattleBehaviour, float newRemovedHealth, float totalHealth)
        {
            if (newRemovedHealth >= totalHealth)
            {
                networkedBattleBehaviour.TryRequestDeathState();
                _battleManager.SetStage();
            }
        }

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == (int)RaiseEventCode.OnSendFloatToTarget)
            {
                object[] data = (object[]) photonEvent.CustomData;
                if (_allUnitsRuntimeSet.TryGetUnitByViewID((int) data[0], out NetworkedBattleBehaviour networkedUnitBehaviour)
                    && networkedUnitBehaviour is BattleBehaviour battleBehaviour)
                {
                    battleBehaviour.BattleClass.OnReceiveFloatActionCallback((float) data[1]);
                }
            }
            
            if (photonEvent.Code == (int)RaiseEventCode.OnUpdateAllClientsHealth)
            {
                object[] data = (object[]) photonEvent.CustomData;
                if (_allUnitsRuntimeSet.TryGetUnitByViewID((int) data[0], out NetworkedBattleBehaviour networkedUnitBehaviour))
                {
                    OnUpdateAllClientsHealthCallback(networkedUnitBehaviour, (float) data[1], (float) data[2]);
                }
            }
            
            if (photonEvent.Code == (int)RaiseEventCode.OnRestartStage)
            {
                object[] data = (object[]) photonEvent.CustomData;
                bool enterLootingState = (bool) data[0];
                _battleManager.EndStage(true, enterLootingState);
            }
            
            if (photonEvent.Code == (int)RaiseEventCode.OnNextStage)
            {
                object[] data = (object[]) photonEvent.CustomData;
                bool enterLootingState = (bool) data[0];
                _battleManager.EndStage(false, enterLootingState);
            }
        }
        
        /// <summary>
        /// All players update this units health
        /// </summary>
        /// <param name="newRemovedHealth"></param>
        /// <param name="totalHealth"></param>
        private void OnUpdateAllClientsHealthCallback(NetworkedBattleBehaviour networkedBattleBehaviour, float newRemovedHealth,
            float totalHealth)
        {
            NetworkedStatsBehaviour networkedStatsBehaviour = networkedBattleBehaviour.NetworkedStatsBehaviour;
            networkedStatsBehaviour.RemovedHealth = newRemovedHealth;
        
            CheckStage(networkedBattleBehaviour, newRemovedHealth, totalHealth);
        }
    }
}

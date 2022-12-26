using DataStructures.StateLogic;
using ExitGames.Client.Photon;
using Features.Connection;
using Features.Unit.Scripts.Behaviours;
using Features.Unit.Scripts.Behaviours.Battle;
using Features.Unit.Scripts.Behaviours.Stat;

namespace Features.Battle.Scripts
{
    public class BattleState : IState
    {
        private readonly BattleManager _battleManager;
        private readonly BattleData_SO _battleData;
        private readonly NetworkedUnitRuntimeSet_SO _allUnitsRuntimeSet;

        public BattleState(BattleManager battleManager, BattleData_SO battleData, NetworkedUnitRuntimeSet_SO allUnitsRuntimeSet)
        {
            _battleManager = battleManager;
            _battleData = battleData;
            _allUnitsRuntimeSet = allUnitsRuntimeSet;
        }
    
        public void Enter()
        {
        }

        public void Execute()
        {
        }

        public void Exit()
        {
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
            
            else if (photonEvent.Code == (int)RaiseEventCode.OnUpdateAllClientsHealth)
            {
                object[] data = (object[]) photonEvent.CustomData;
                if (_allUnitsRuntimeSet.TryGetUnitByViewID((int) data[0], out NetworkedBattleBehaviour networkedUnitBehaviour))
                {
                    OnUpdateAllClientsHealthCallback(networkedUnitBehaviour, (float) data[1], (float) data[2]);
                }
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
        
            if (networkedStatsBehaviour.RemovedHealth >= totalHealth)
            {
                networkedBattleBehaviour.TryRequestDeathState();
                SetStage();
            }
        }

        private void SetStage()
        {
            if (!_battleData.PlayerUnitsRuntimeSet.HasUnitAlive())
            {
                _battleManager.EndStage(true);
                return;
            }

            if (!_battleData.EnemyUnitsRuntimeSet.HasUnitAlive())
            {
                _battleManager.EndStage(false);
            }
        }
    }
}

using DataStructures.StateLogic;
using ExitGames.Client.Photon;
using Features.GlobalReferences.Scripts;
using Features.Unit.Battle.Scripts;
using Features.Unit.Modding;

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
                //TODO: getComponent
                object[] data = (object[]) photonEvent.CustomData;
                if (_allUnitsRuntimeSet.TryGetUnitByViewID((int) data[0], out NetworkedUnitBehaviour networkedUnitBehaviour)
                    && networkedUnitBehaviour.TryGetComponent(out BattleBehaviour battleBehaviour))
                {
                    battleBehaviour.BattleActions.OnReceiveFloatActionCallback((float) data[1]);
                }
            }
            
            else if (photonEvent.Code == (int)RaiseEventCode.OnUpdateAllClientsHealth)
            {
                //TODO: getComponent
                object[] data = (object[]) photonEvent.CustomData;
                if (_allUnitsRuntimeSet.TryGetUnitByViewID((int) data[0], out NetworkedUnitBehaviour networkedUnitBehaviour))
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
        private void OnUpdateAllClientsHealthCallback(NetworkedUnitBehaviour networkedUnitBehaviour, float newRemovedHealth,
            float totalHealth)
        {
            networkedUnitBehaviour.RemovedHealth = newRemovedHealth;
        
            if (networkedUnitBehaviour.RemovedHealth >= totalHealth)
            {
                if (networkedUnitBehaviour.TryGetComponent(out BattleBehaviour battleBehaviour))
                {
                    battleBehaviour.TryRequestDeathState();
                }

                SetStage();
            }
        }

        private void SetStage()
        {
            if (!_battleData.PlayerUnitsRuntimeSet.HasUnitAlive())
            {
                _battleManager.RequestStageSetupState(true);
                return;
            }

            if (!_battleData.EnemyUnitsRuntimeSet.HasUnitAlive())
            {
                _battleManager.RequestStageSetupState(false);
            }
        }
    }
}

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
        private readonly NetworkedUnitRuntimeSet_SO _allUnitsRuntimeSet;

        public BattleState(BattleManager battleManager, NetworkedUnitRuntimeSet_SO allUnitsRuntimeSet)
        {
            _battleManager = battleManager;
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
            //if (battleData.CurrentState is not BattleState) return;
            //1st step: send damage + animation behaviour from attacker to calculating instance - Client & Master: Send to others
            if (photonEvent.Code == (int)RaiseEventCode.OnPerformUnitAttack)
            {
                object[] data = (object[]) photonEvent.CustomData;
                if (_allUnitsRuntimeSet.TryGetUnitByViewID((int) data[0], out NetworkedUnitBehaviour networkedUnitBehaviour)
                    && networkedUnitBehaviour.TryGetComponent(out BattleBehaviour battleBehaviour))
                {
                    battleBehaviour.BattleActions.OnSendAttackActionCallback((float) data[1]);
                }
            }
            //2nd step: raise event to update health on all clients on attacked instance
            else if (photonEvent.Code == (int)RaiseEventCode.OnPerformUpdateUnitHealth)
            {
                object[] data = (object[]) photonEvent.CustomData;
                if (_allUnitsRuntimeSet.TryGetUnitByViewID((int) data[0], out NetworkedUnitBehaviour networkedUnitBehaviour) 
                    && networkedUnitBehaviour.TryGetComponent(out BattleBehaviour battleBehaviour))
                {
                    battleBehaviour.BattleActions.OnSendHealthActionCallback((float) data[1], (float) data[2]);
                }
            }
        }
    }
}

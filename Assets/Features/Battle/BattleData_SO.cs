using DataStructures.StateLogic;
using Features.GlobalReferences;
using UnityEngine;

namespace Features.Battle
{
    [CreateAssetMenu]
    public class BattleData_SO : ScriptableObject
    {
        [SerializeField] private NetworkedUnitRuntimeSet_SO enemyUnitRuntimeSet;

        [SerializeField] private NetworkedUnitRuntimeSet_SO playerTeamUnitRuntimeSet;

        public NetworkedUnitRuntimeSet_SO EnemyUnitRuntimeSet => enemyUnitRuntimeSet;
        
        public NetworkedUnitRuntimeSet_SO PlayerTeamUnitRuntimeSet => playerTeamUnitRuntimeSet;

        
        //TODO: correct battleManager initialisation - initializing of stages inside stageStateMachine
        //TODO: add rewards for stage completing in RuntimeList & add event for UI
        //TODO: update stage UI by event
    }
}

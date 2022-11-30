using Features.GlobalReferences;
using UnityEngine;

namespace Features.Battle
{
    public class BattleManager : MonoBehaviour
    {
        [SerializeField] private NetworkedUnitRuntimeSet_SO enemyUnitsRuntimeSet;

        [SerializeField] private LocalUnitRuntimeSet_SO teamUnitsRuntimeSet;
        
        //TODO: keep track of stage information (initialize Stage, Run Stage, EndStage) almost certain with State Machine ?
        //TODO: add rewards for stage completing in RuntimeList & add event for UI
        //TODO: update stage UI by event

        public void InitializeStage()
        {
            
        }

        public void NextStage()
        {
            
        }

        public void RestartStage()
        {
            
        }
    }
}

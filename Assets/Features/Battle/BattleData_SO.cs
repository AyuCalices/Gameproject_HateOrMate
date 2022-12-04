using DataStructures.StateLogic;
using Features.GlobalReferences;
using Features.Unit.Modding;
using Features.Unit.Modding.Stat;
using Photon.Pun;
using UnityEngine;

namespace Features.Battle
{
    [CreateAssetMenu]
    public class BattleData_SO : ScriptableObject
    {
        //TODO: stage information
        //TODO: rewards
        
        [SerializeField] private NetworkedUnitRuntimeSet_SO enemyUnitRuntimeSet;

        [SerializeField] private NetworkedUnitRuntimeSet_SO playerTeamUnitRuntimeSet;

        public NetworkedUnitRuntimeSet_SO EnemyUnitRuntimeSet => enemyUnitRuntimeSet;
        
        public NetworkedUnitRuntimeSet_SO PlayerTeamUnitRuntimeSet => playerTeamUnitRuntimeSet;
        
        public int Stage { get; set; }
        
        public BattleManager BattleManager { get; private set; }
        public IState CurrentState => BattleManager.CurrentState;

        public void RegisterBattleManager(BattleManager battleManager)
        {
            BattleManager = battleManager;
        }

        public void SetAiStats(AIUnitBehaviour aiUnitBehaviour)
        {
            if (aiUnitBehaviour.NetworkingInitialized)
            {
                aiUnitBehaviour.NetworkedStatServiceLocator.SetBaseValue(StatType.Damage, 10 * (Stage + 1));
                aiUnitBehaviour.NetworkedStatServiceLocator.SetBaseValue(StatType.Health, 50 * (Stage + 1));
                aiUnitBehaviour.NetworkedStatServiceLocator.SetBaseValue(StatType.Speed, 3);
            }
        }
    }
}

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
            if (PhotonNetwork.IsMasterClient && aiUnitBehaviour.NetworkingInitialized)
            {
                aiUnitBehaviour.NetworkedStatServiceLocator.RemoveAllValues();
                
                aiUnitBehaviour.NetworkedStatServiceLocator.TryAddLocalValue(StatType.Damage, StatValueType.Stat,10 * (Stage + 1));
                aiUnitBehaviour.NetworkedStatServiceLocator.TryAddLocalValue(StatType.Damage, StatValueType.ScalingStat, 1);
            
                aiUnitBehaviour.NetworkedStatServiceLocator.TryAddLocalValue(StatType.Health, StatValueType.Stat, 50 * (Stage + 1));
                aiUnitBehaviour.NetworkedStatServiceLocator.TryAddLocalValue(StatType.Health, StatValueType.ScalingStat, 1);
            
                aiUnitBehaviour.NetworkedStatServiceLocator.TryAddLocalValue(StatType.Speed, StatValueType.Stat, 3);
                aiUnitBehaviour.NetworkedStatServiceLocator.TryAddLocalValue(StatType.Speed, StatValueType.ScalingStat, 1);
            }
        }
    }
}

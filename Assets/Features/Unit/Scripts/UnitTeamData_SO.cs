using Features.Battle.Scripts;
using Features.Unit.Scripts.Behaviours;
using UnityEngine;

namespace Features.Unit.Scripts
{
    [CreateAssetMenu]
    public class UnitTeamData_SO : ScriptableObject
    {
        public BattleData_SO battleData;
        public bool isAI;
        public NetworkedUnitRuntimeSet_SO ownerNetworkedPlayerUnits;
        public NetworkedUnitRuntimeSet_SO ownTeamRuntimeSet;
        
        public NetworkedUnitRuntimeSet_SO EnemyRuntimeSet => battleData.GetEnemyTeam(ownTeamRuntimeSet);
    }
}

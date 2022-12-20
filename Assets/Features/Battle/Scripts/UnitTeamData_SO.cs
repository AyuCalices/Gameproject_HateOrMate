using Features.GlobalReferences.Scripts;
using UnityEngine;

namespace Features.Battle.Scripts
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

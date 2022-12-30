using System;
using Features.Loot.Scripts.ModView;
using Features.Unit.Scripts;
using Features.Unit.Scripts.Behaviours.Battle;
using Features.Unit.Scripts.Behaviours.Stat;

namespace Features.Loot.Scripts.GeneratedLoot
{
    public class UnitMod : BaseMod
    {
        public static Func<string, UnitClassData_SO, BaseStats, NetworkedBattleBehaviour> onAddUnit;
        public static Action<string, int> onRemoveUnit;
        
        private readonly UnitClassData_SO _classData;
        private NetworkedBattleBehaviour _instantiatedUnit;

        public UnitMod(UnitClassData_SO classData, string modName, string description, ModDragBehaviour modDragBehaviourPrefab) 
            : base(modName, description, modDragBehaviourPrefab)
        {
            _classData = classData;
        }

        protected override void InternalAddMod(NetworkedStatsBehaviour moddedLocalStats)
        {
            _instantiatedUnit = onAddUnit.Invoke("Player", _classData, new BaseStats(10, 50, 3));
        }

        protected override void InternalRemoveMod(NetworkedStatsBehaviour moddedLocalStats)
        {
            if (_instantiatedUnit != null)
            {
                onRemoveUnit.Invoke("Player", _instantiatedUnit.PhotonView.ViewID);
            }
        }
    }
}

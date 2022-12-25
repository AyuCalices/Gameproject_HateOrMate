using Features.Unit.Modding;
using System;
using Features.Battle.Scripts;
using Features.Unit.Battle.Scripts;
using Features.Unit.Classes;

namespace Features.Mod
{
    public class UnitMod : BaseMod
    {
        public static Func<string, UnitClassData_SO, SynchronizedBaseStats, NetworkedBattleBehaviour> onAddUnit;
        public static Action<string, int> onRemoveUnit;
        
        private readonly UnitClassData_SO _classData;
        private NetworkedBattleBehaviour _instantiatedUnit;

        public UnitMod(UnitClassData_SO classData, string modName, string description) : base(modName, description)
        {
            _classData = classData;
        }

        protected override void InternalAddMod(NetworkedStatsBehaviour moddedLocalStats)
        {
            _instantiatedUnit = onAddUnit.Invoke("Player", _classData, new SynchronizedBaseStats(10, 50, 3));
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

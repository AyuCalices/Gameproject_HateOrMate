using Features.Unit.Modding;
using System;
using Features.Unit.Battle.Scripts;
using Features.Unit.Classes;
using Photon.Pun;

namespace Features.Mod
{
    public class UnitMod : BaseMod
    {
        public static Func<string, UnitClassData_SO, NetworkedBattleBehaviour> onAddUnit;
        public static Action<string, PhotonView> onRemoveUnit;
        
        private readonly UnitClassData_SO _classData;
        private NetworkedBattleBehaviour _instantiatedUnit;

        public UnitMod(UnitClassData_SO classData, string modName, string description) : base(modName, description)
        {
            _classData = classData;
        }

        protected override void InternalAddMod(NetworkedStatsBehaviour moddedLocalStats)
        {
            _instantiatedUnit = onAddUnit.Invoke("Player", _classData);
        }

        protected override void InternalRemoveMod(NetworkedStatsBehaviour moddedLocalStats)
        {
            if (_instantiatedUnit != null)
            {
                onRemoveUnit.Invoke("Player", _instantiatedUnit.PhotonView);
            }
        }
    }
}

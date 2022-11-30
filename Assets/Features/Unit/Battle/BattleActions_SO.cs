using Features.GlobalReferences;
using Features.Unit.Modding;
using UnityEngine;

namespace Features.Unit.Battle
{
    public abstract class BattleActions_SO : ScriptableObject
    {
        //TODO: Tower: keep track of stamina + UI
        public virtual void UpdateBattleActions(NetworkedUnitBehaviour ownerNetworkingUnitBehaviour) { }

        //TODO: implement moving to enemy
        public virtual void Move(NetworkedUnitBehaviour ownerNetworkingUnitBehaviour, NetworkedUnitRuntimeSet_SO opponentNetworkedUnitRuntimeSet) {}
        
        //TODO: implement Types: Clicker Damage, Idle Damage (Melee/Range), Heal
        //TODO: send event for stamina UI updated & Health value changed
        //TODO: send event for all client, what they should do (by RaiseEvent & maybe with eventCode for attack/Heal?)
        public abstract void OnCastComplete(NetworkedUnitBehaviour ownerNetworkingUnitBehaviour, NetworkedUnitRuntimeSet_SO opponentNetworkedUnitRuntimeSet);
    }
    
    
}

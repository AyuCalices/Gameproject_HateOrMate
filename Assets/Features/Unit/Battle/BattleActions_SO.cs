using UnityEngine;

namespace Features.Unit.Battle
{
    public abstract class BattleActions_SO : ScriptableObject
    {
        //TODO: Tower: keep track of stamina
        //TODO: send event for stamina UI updated every time it changes
        public virtual void UpdateBattleActions() { }

        //TODO: decide if Move here ?
        public virtual void Move() {}
        
        //TODO: implement Types: Clicker Damage, Idle Damage (Melee/Range), Heal
        //TODO: send event for stamina UI updated & Health value changed
        //TODO: send event for all client, what they should do (by RaiseEvent & maybe with eventCode for attack/Heal?)
        public abstract void OnCastComplete();
    }
    
    
}

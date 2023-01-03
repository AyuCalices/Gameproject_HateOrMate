using Features.Loot.Scripts.GeneratedLoot;
using Features.Unit.Scripts.Behaviours.Mod;
using UnityEngine;

namespace Features.Loot.Scripts.ModView
{
    public class DragController
    {
        public bool IsSuccessful { get; set; }

        public DragController()
        {
            IsSuccessful = false;
        }
        
        public void AddOrExchangeMod(ModBehaviour originMod, ModBehaviour targetMod, IModContainer origin, IModContainer target)
        {
            if (target == origin) return;
            
            IsSuccessful = true;
            
            origin.RemoveMod(originMod, !target.DisableModOnSwap());
            
            //can be null due to hand
            if (target.ContainsMod() && targetMod != null)
            {
                target.RemoveMod(targetMod, !origin.DisableModOnSwap());
                origin.SwapAddMod(targetMod);
                targetMod.SetNewOrigin(origin);
            }

            target.SwapAddMod(originMod);
            originMod.SetNewOrigin(target);
        }
    }
}
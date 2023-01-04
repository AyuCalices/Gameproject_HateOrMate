
namespace Features.Loot.Scripts.ModView
{
    public static class ModHelper
    {
        public static void AddOrExchangeMod(ModBehaviour movingMod, ModBehaviour blockedTargetMod, IModContainer movingModOrigin, IModContainer movingModTarget)
        {
            if (movingModTarget == movingModOrigin) return;

            movingModOrigin.RemoveMod(movingMod, !movingModTarget.DisableModOnSwap);
            
            //can be null due to hand
            if (movingModTarget.ContainsMod && blockedTargetMod != null)
            {
                movingModTarget.RemoveMod(blockedTargetMod, !movingModOrigin.DisableModOnSwap);
                movingModOrigin.AddMod(blockedTargetMod);
                blockedTargetMod.SetNewOrigin(movingModOrigin);
            }

            movingModTarget.AddMod(movingMod);
            movingMod.SetNewOrigin(movingModTarget);
        }
    }
}

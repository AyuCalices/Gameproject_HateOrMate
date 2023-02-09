
namespace Features.Loot.Scripts.ModView
{
    public static class ModHelper
    {
        public static void AddOrExchangeMod(ModBehaviour movingMod, ModBehaviour targetMod, IModContainer movingModOrigin, IModContainer movingModTarget)
        {
            if (movingModTarget == movingModOrigin) return;

            movingModOrigin.RemoveMod(movingMod);
            
            //can be null due to hand
            if (movingModTarget.ContainsMod && targetMod != null)
            {
                movingModTarget.RemoveMod(targetMod);
                movingModOrigin.AddMod(targetMod);
                targetMod.InitializeNewOrigin(movingModOrigin);
            }

            movingModTarget.AddMod(movingMod);
            movingMod.InitializeNewOrigin(movingModTarget);
        }
    }
}

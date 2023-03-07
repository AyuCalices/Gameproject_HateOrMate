
using Features.Mods.Scripts.View.ModContainer;

namespace Features.Mods.Scripts.View
{
    public static class ModSwapHelper
    {
        public static void Perform(ModViewBehaviour movingModView, ModViewBehaviour targetModView, IModContainer movingModOrigin, IModContainer movingModTarget)
        {
            if (movingModTarget == movingModOrigin) return;

            movingModOrigin.RemoveMod(movingModView);
            
            //can be null due to hand
            if (movingModTarget.ContainsMod && targetModView != null)
            {
                movingModTarget.RemoveMod(targetModView);
                movingModOrigin.AddMod(targetModView);
                targetModView.InitializeNewOrigin(movingModOrigin);
            }

            movingModTarget.AddMod(movingModView);
            movingModView.InitializeNewOrigin(movingModTarget);
        }
    }
}

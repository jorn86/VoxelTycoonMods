using HarmonyLib;
using VoxelTycoon.Buildings;

namespace Sandbox
{
    [HarmonyPatch(typeof(Building), nameof(Building.Price), MethodType.Getter)]
    internal class DecorationPricePatch
    {
        internal static bool Prefix(Building __instance, ref double __result)
        {
            if (__instance is Decoration || __instance is Plant || __instance is House)
            {
                return SandboxSettings.FreeIf(SandboxSettings.FreeTerraform, ref __result);
            }
            return true;
        }
    }
}

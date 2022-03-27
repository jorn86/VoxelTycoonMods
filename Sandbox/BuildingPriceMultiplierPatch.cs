using HarmonyLib;
using VoxelTycoon.Buildings;
using VoxelTycoon.Researches;

namespace Sandbox
{
    [HarmonyPatch(typeof(Building), nameof(Building.PriceMultiplier), MethodType.Getter)]
    internal class BuildingPriceMultiplierPatch
    {
        internal static bool Prefix(Building __instance, ref double __result)
        {
            if (__instance is Headquarters ||
                __instance is Mine ||
                __instance is Warehouse ||
                __instance is Device ||
                __instance is Lab)
            {
                return SandboxSettings.FreeIf(SandboxSettings.FreeBuilding, ref __result);
            }
            return true;
        }
    }
}

using HarmonyLib;
using VoxelTycoon;

namespace Sandbox
{
    [HarmonyPatch(typeof(Region), nameof(Region.Price), MethodType.Getter)]
    internal class RegionPricePatch
    {
        internal static bool Prefix(ref double __result)
        {
            return SandboxSettings.FreeIf(SandboxSettings.FreeExplore, ref __result);
        }
    }
}

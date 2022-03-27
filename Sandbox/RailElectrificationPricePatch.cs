using HarmonyLib;
using VoxelTycoon.Tracks.Rails;

namespace Sandbox
{
    [HarmonyPatch(typeof(Rail), nameof(Rail.ElectrificationPrice), MethodType.Getter)]
    internal class RailElectrificationPricePatch
    {
        internal static bool Prefix(ref double __result)
        {
            return SandboxSettings.FreeIf(SandboxSettings.FreeInfra, ref __result);
        }
    }
}

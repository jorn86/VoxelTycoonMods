using HarmonyLib;
using VoxelTycoon.Tracks.Rails;

namespace Sandbox
{
    [HarmonyPatch(typeof(RailSignalSharedData), nameof(RailSignalSharedData.BasePrice), MethodType.Getter)]
    internal class SignalPricePatch
    {
        internal static bool Prefix(ref double __result)
        {
            return SandboxSettings.FreeIf(SandboxSettings.FreeInfra, ref __result);
        }
    }
}

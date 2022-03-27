using HarmonyLib;
using VoxelTycoon;
using VoxelTycoon.Tools;

namespace Sandbox
{
    [HarmonyPatch(typeof(ToolHelper), nameof(ToolHelper.GetFlattenPrice), typeof(Xyz), typeof(int))]
    internal class TerraformPricePatch
    {
        internal static bool Prefix(ref double __result)
        {
            return SandboxSettings.FreeIf(SandboxSettings.FreeTerraform, ref __result);
        }
    }
}

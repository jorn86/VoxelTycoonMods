using HarmonyLib;
using VoxelTycoon;
using VoxelTycoon.Tools;

namespace Sandbox
{
    [HarmonyPatch(typeof(ToolHelper), nameof(ToolHelper.GetFlattenPrice), argumentTypes: new Type[] { typeof(Xyz), typeof(int) })]
    class TerraformPricePatch
    {
        static bool Prefix(ref double __result)
        {
            return SandboxSettings.FreeIf(SandboxSettings.FreeTerraform, ref __result);
        }
    }
}

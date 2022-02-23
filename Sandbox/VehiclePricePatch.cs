using HarmonyLib;
using VoxelTycoon.Tracks;

namespace Sandbox
{
    [HarmonyPatch(typeof(VehicleUnitSharedData), nameof(VehicleUnitSharedData.BasePrice), MethodType.Getter)]
    class VehiclePricePatch
    {
        internal static bool Prefix(ref double __result)
        {
            return SandboxSettings.FreeIf(SandboxSettings.FreeVehicle, ref __result);
        }
    }
}

using HarmonyLib;
using VoxelTycoon.Buildings;
using VoxelTycoon.Tracks;
using VoxelTycoon.Tracks.Conveyors;
using VoxelTycoon.Tracks.Rails;
using VoxelTycoon.Tracks.Roads;

namespace Sandbox
{
    [HarmonyPatch(typeof(Building), nameof(Building.Price), MethodType.Getter)]
    class BuildingPricePatch
    {
        internal static bool Prefix(Building __instance, ref double __result)
        {
            if (__instance is VehicleDepot ||
                __instance is VehicleStation ||
                __instance is Rail ||
                __instance is Road ||
                __instance is RailBridge ||
                __instance is RoadBridge ||
                __instance is RailTunnel ||
                __instance is RoadTunnel)
            {
                return SandboxSettings.FreeIf(SandboxSettings.FreeInfra, ref __result);
            }

            if (__instance is Conveyor ||
                __instance is ConveyorBridge ||
                __instance is ConveyorConnector ||
                __instance is ConveyorFilter ||
                __instance is ConveyorTunnel)
            {
                return SandboxSettings.FreeIf(SandboxSettings.FreeBuilding, ref __result);
            }
            return true;
        }
    }
}

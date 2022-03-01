using HarmonyLib;
using VoxelTycoon;
using VoxelTycoon.Tracks.Rails;

namespace MassElectrification
{
    [HarmonyPatch(typeof(RailElectrificationTrackNodeSelector), nameof(RailElectrificationTrackNodeSelector.Current), MethodType.Getter)]
    class NodeSelectorPatch
    {
        private static readonly RailMassElectrificationTrackNodeSelector _instance = new RailMassElectrificationTrackNodeSelector();

        internal static bool Prefix(ref RailElectrificationTrackNodeSelector __result)
        {
            __result = _instance;
            return false;
        }
    }
}

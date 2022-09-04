using HarmonyLib;
using VoxelTycoon.Tracks.Rails;

namespace PathfinderPatch
{
    [HarmonyPatch(typeof(TrainPathfinder), nameof(TrainPathfinder.Current), MethodType.Getter)]
    internal class PatchedPathfinderPatch
    {
        private static readonly TrainPathfinder instance = new PatchedTrainPathfinder();

        internal static bool Prefix(ref TrainPathfinder __result)
        {
            __result = instance;
            return false;
        }
    }
}

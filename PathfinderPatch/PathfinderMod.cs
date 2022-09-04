using HarmonyLib;
using VoxelTycoon;
using VoxelTycoon.Modding;

namespace PathfinderPatch
{
    public class PathfinderMod : Mod
    {
        private static readonly Logger _logger = new Logger<PathfinderMod>();
        private const string PatchId = "org.hertsig.voxeltycoon.Pathfinder";

        protected override void Initialize()
        {
            new Harmony(PatchId).PatchAll();
            _logger.Log("Pathfinder patch complete");
        }

        protected override void Deinitialize()
        {
            new Harmony(PatchId).UnpatchAll();
            _logger.Log("Pathfinder un-patch complete");
        }
    }
}

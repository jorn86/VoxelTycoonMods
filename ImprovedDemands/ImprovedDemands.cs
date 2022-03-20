using HarmonyLib;
using VoxelTycoon;
using VoxelTycoon.Cities;
using VoxelTycoon.Modding;

namespace ImprovedDemands
{
    class ImprovedDemands : Mod
    {
        private static readonly Logger _logger = new Logger<ImprovedDemands>();
        private const string PatchId = "org.hertsig.voxeltycoon.ImprovedDemands";

        protected override void Initialize()
        {
            new Harmony(PatchId).PatchAll();
            _logger.Log("ImprovedDemands patch complete");
        }

        protected override void Deinitialize()
        {
            new Harmony(PatchId).UnpatchAll();
            _logger.Log("ImprovedDemands un-patch complete");
        }
    }
}

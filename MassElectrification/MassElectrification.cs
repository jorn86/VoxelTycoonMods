using HarmonyLib;
using VoxelTycoon;
using VoxelTycoon.Modding;

namespace MassElectrification
{
    public class MassElectrification : Mod
    {
        private static readonly Logger _logger = new Logger<MassElectrification>();
        private const string PatchId = "org.hertsig.voxeltycoon.MassElectrification";

        protected override void Initialize()
        {
            new Harmony(PatchId).PatchAll();
            _logger.Log("Mass electrification patch complete");
        }

        protected override void Deinitialize()
        {
            new Harmony(PatchId).UnpatchAll();
            _logger.Log("Mass electrification un-patch complete");
        }
    }
}

using HarmonyLib;
using VoxelTycoon;
using VoxelTycoon.Modding;

namespace SignalDistance
{
    public class SignalDistance : Mod
    {
        private static readonly Logger _logger = new Logger<SignalDistance>();
        private const string PatchId = "org.hertsig.voxeltycoon.SignalDistance";

        protected override void Initialize()
        {
            new Harmony(PatchId).PatchAll();
            _logger.Log("SignalDistance patch complete");
        }

        protected override void Deinitialize()
        {
            new Harmony(PatchId).UnpatchAll();
            _logger.Log("SignalDistance un-patch complete");
        }
    }
}

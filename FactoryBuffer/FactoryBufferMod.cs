using HarmonyLib;
using VoxelTycoon;
using VoxelTycoon.Modding;

namespace FactoryBuffer
{
    public class FactoryBufferMod : Mod
    {
        private static readonly Logger _logger = new Logger<FactoryBufferMod>();
        private const string PatchId = "org.hertsig.voxeltycoon.FactoryBuffer";

        protected override void Initialize()
        {
            new Harmony(PatchId).PatchAll();
            _logger.Log("FactoryBuffer patch complete");
        }

        protected override void Deinitialize()
        {
            new Harmony(PatchId).UnpatchAll();
            _logger.Log("FactoryBuffer un-patch complete");
        }

    }
}

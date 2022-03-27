using HarmonyLib;
using VoxelTycoon;
using VoxelTycoon.Modding;
using VoxelTycoon.Tools.Builder;

namespace BigMines
{
    public class BigMines : Mod
    {
        private static readonly Logger _logger = new Logger<BigMines>();
        private const string PatchId = "org.hertsig.voxeltycoon.BigMines";

        protected override void Initialize()
        {
            new Harmony(PatchId).PatchAll();
            _logger.Log("BigMines patch complete");
        }

        protected override void Deinitialize()
        {
            new Harmony(PatchId).UnpatchAll();
            _logger.Log("BigMines un-patch complete");
        }
    }
}

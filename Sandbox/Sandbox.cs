using HarmonyLib;
using VoxelTycoon;
using VoxelTycoon.Game;
using VoxelTycoon.Modding;

namespace Sandbox
{
    class Sandbox : Mod
    {
        private static readonly Logger _logger = new Logger<Sandbox>();
        private const string PatchId = "org.hertsig.voxeltycoon.Sandbox";

        protected override void OnGameStarted()
        {
            // Moved from Initialize to here because game apparently calls
            // vehicle price code before settings are initialized
            new Harmony(PatchId).PatchAll();
            _logger.Log("Sandbox patch complete");
        }

        protected override void Deinitialize()
        {
            new Harmony(PatchId).UnpatchAll();
            _logger.Log("Sandbox un-patch complete");
        }

        protected override void OnUpdate()
        {
            //LogBuildingUnderCursor();
        }

        internal static void LogBuildingUnderCursor()
        {
            var building = GameUI.Current.BuildingUnderCursor;
            if (building != null)
            {
                _logger.Log("Got " + building.GetType() + " / " + building.DisplayName);
            }
        }
    }
}

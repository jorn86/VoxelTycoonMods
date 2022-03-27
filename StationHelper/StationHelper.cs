using System;
using System.Linq;
using HarmonyLib;
using VoxelTycoon;
using VoxelTycoon.Buildings;
using VoxelTycoon.Modding;
using VoxelTycoon.Tools;
using VoxelTycoon.Tools.Builder;
using VoxelTycoon.Tools.TrackBuilder;
using VoxelTycoon.Tracks.Rails;
using VoxelTycoon.UI;

namespace StationHelper
{
    internal class StationHelper : Mod
    {
        private static readonly Logger _logger = new Logger<StationHelper>();
        private const string PatchId = "org.hertsig.voxeltycoon.StationHelper";

        protected override void Initialize()
        {
            new Harmony(PatchId).PatchAll();
            _logger.Log("StationHelper patch complete");
        }

        protected override void OnGameStarted()
        {
            _logger.Log("StationHelper starting");
            new RailBuilderTool(null).Use(it =>
            {
                if (it.ElectrificationAvailable) it.ElectrificationMode = RailElectrificationMode.Right;
            });

            AssetLibrary.Current.GetAll<RailSignal>()
                         .Select(railSignal => new RailSignalBuilderTool(railSignal))
                         .ForEach(tool => tool.Use(it => it.Spacing = 30));

            BuildingRecipeManager.Current.GetAll().Enumerate()
                .OfType<RailStationRecipe>()
                .Select(recipe => new RailStationBuilderTool(recipe.Building as RailStation))
                .ForEach(tool => tool.Use(it => { it.Width = 2; it.Length = 30; }));
        }

        protected override void Deinitialize()
        {
            new Harmony(PatchId).UnpatchAll();
            _logger.Log("StationHelper un-patch complete");
        }
    }

    internal static class ToolExt
    {
        internal static void Use<T>(this T tool, Action<T> action) where T : ITool
        {
            tool.Activate();
            action(tool);
            if (!tool.Deactivate(true))
            {
                tool.Deactivate(false);
            }
        }
    }
}

using System;
using HarmonyLib;
using UnityEngine.UI;
using VoxelTycoon;
using VoxelTycoon.Game.UI;
using VoxelTycoon.Tools;

namespace StationHelper
{
    [HarmonyPatch(typeof(SettingsWindowUpDownItem))]
    class SettingPatch
    {
        private static readonly Logger _logger = new Logger<RailElectrificationTool>();
        private static bool _inPatch;

        [HarmonyPatch(nameof(SettingsWindowUpDownItem.Less))]
        internal static void Postfix(SettingsWindowUpDownItem __instance, Text ___NameText)
        {
            Run(___NameText.text, __instance.Less);
        }

        [HarmonyPatch(nameof(SettingsWindowUpDownItem.More))]
        internal static void Postfix(SettingsWindowUpDownItem __instance, Text ___NameText, object ___DescriptionText)
        {
            Run(___NameText.text, __instance.More);
        }

        private static void Run(string name, Action action)
        {
            if (!Active(name)) return;

            _inPatch = true;
            for (var i = 0; i < 9; i++)
            {
                action();
            }
            _inPatch = false;
        }

        private static bool Active(string name)
        {
            return !_inPatch && InputHelper.Control &&
                   (name == S.RailStationBuilderWindowLength ||
                    name == S.RailSignalBuilderToolSpacing);
        }
    }
}

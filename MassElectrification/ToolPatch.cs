using HarmonyLib;
using VoxelTycoon;
using VoxelTycoon.Tools;
using VoxelTycoon.Tracks.Rails;

namespace MassElectrification
{
    [HarmonyPatch(typeof(RailElectrificationTool), nameof(RailElectrificationTool.OnUpdate))]
    class ToolPatch
    {
        private static readonly Logger _logger = new Logger<RailElectrificationTool>();
        private static bool _multiMode = false;

        internal static bool Prefix(ref Rail ____rail, UniqueList<Rail> ____targets)
        {
            // make tool update trigger on pressing or releasing the control key
            // (instead of just on hovering over a different rail piece)
            if (_multiMode != InputHelper.Control)
            {
                _multiMode = InputHelper.Control;
                ____rail = null;
                ClearSelection(____targets);
            }
            return true;
        }

        private static void ClearSelection(UniqueList<Rail> _targets)
        {
            for (int i = 0; i < _targets.Count; i++)
            {
                _targets[i].SetTint(null);
            }

            _targets.Clear();
        }
    }
}

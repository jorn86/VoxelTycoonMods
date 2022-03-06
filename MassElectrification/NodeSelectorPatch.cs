using HarmonyLib;
using System.Collections.Generic;
using VoxelTycoon;
using VoxelTycoon.Tracks;
using VoxelTycoon.Tracks.Rails;

namespace MassElectrification
{
    [HarmonyPatch(typeof(RailElectrificationTrackNodeSelector), nameof(RailElectrificationTrackNodeSelector.Select))]
    class NodeSelectorPatch
    {
        private static readonly Logger _logger = new Logger<NodeSelectorPatch>();

        internal static bool Prefix(Rail track, List<TrackConnection> result)
        {
            if (!InputHelper.Control)
            {
                return true;
            }

            for (int i = 0; i < track.ConnectionCount; i++)
            {
                MultiSelect(track.GetConnection(i), result);
            }
            return false;
        }

        private static void MultiSelect(RailConnection connection, List<TrackConnection> result)
        {
            if (connection.Track.ElectrificationMode != RailElectrificationMode.None || result.Contains(connection))
            {
                return;
            }
            result.Add(connection);
            connection.InnerConnection.OuterConnections.ForEach(c =>
            {
                MultiSelect(c as RailConnection, result);
            });
        }
    }
}

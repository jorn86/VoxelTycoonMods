using HarmonyLib;
using VoxelTycoon;
using VoxelTycoon.Tools;
using VoxelTycoon.Tracks.Rails;
using System;
using VoxelTycoon.Tracks;
using System.Collections.Generic;

namespace SignalDistance
{
    [HarmonyPatch(typeof(RailSignalBuilderTool), methodName: "GetMessage")]
    class SignalBuilderToolPatch
    {
        private static readonly Logger _logger = new Logger<SignalBuilderToolPatch>();

        internal static void Postfix(RailConnection connection, ref string __result)
        {
            var forward = DistanceToNextSignal(connection, c => c.Signal);
            var back = DistanceToNextSignal(connection.InnerConnection, c => c.InnerConnection.Signal);
            
            __result += $" - {forward} / {back}";
        }

        private static string DistanceToNextSignal(RailConnection connection, Func<RailConnection, RailSignal> signalSelector)
        {
            var next = connection;
            var length = 0f;
            var visited = new List<Track>();
            while (true)
            {
                if (visited.Contains(next.Track))
                {
                    break;
                }

                if (next != connection && signalSelector.Invoke(next) != null)
                {
                    break;
                }

                visited.Add(next.Track);
                length += next.Length;

                next = next.InnerConnection;
                if (next.OuterConnectionCount == 0) break;
                if (next.OuterConnectionCount > 1)
                {
                    return $"intersection {ToolHelper.FormatDistance(length)}";
                }
                else
                {
                    next = next.GetOuterConnection(0);
                }
            }
            return ToolHelper.FormatDistance(length);
        }
    }
}

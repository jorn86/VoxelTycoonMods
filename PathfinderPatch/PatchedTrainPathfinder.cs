using System.Collections.Generic;
using VoxelTycoon;
using VoxelTycoon.Tracks;
using VoxelTycoon.Tracks.Rails;

namespace PathfinderPatch
{
    internal class PatchedTrainPathfinder : TrainPathfinder
    {
        private static readonly Logger _logger = new Logger<PatchedTrainPathfinder>();

        protected override float GetLength(TrackConnection value)
        {
            var connection = (RailConnection) value;
            var length = connection.PathNode.Length;
            if (value.Track.Parent is RailStation)
            {
                //_logger.Log("Station penalty: " + length);
                length *= 3f;
            }
            if (!connection.BlockIsOpen())
            {
                length *= 5f;
            }

            connection.PathNode.Connections.ForEach(c =>
            {
                var path = c.Path;
                for (var i = 0; i < path.UnitCount; i++)
                {
                    var unit = path.GetUnit(i);
                    if (Vehicle == unit)
                    {
                        //_logger.Log("Self penalty for " + unit + ": " + length);
                        length += 50_000f;
                    }
                    else if (((Train) unit).IsWaitingForOpenSignal)
                    {
                        //_logger.Log($"Train {Vehicle.Id}: Signal penalty for train {unit.Id}: {length}");
                        length += 2000f;
                    }
                }
            });
            return length;
        }
    }
}

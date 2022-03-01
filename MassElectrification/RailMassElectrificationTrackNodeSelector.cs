using System.Collections.Generic;
using System.Linq;
using VoxelTycoon;
using VoxelTycoon.Tracks;
using VoxelTycoon.Tracks.Rails;

namespace MassElectrification
{
    class RailMassElectrificationTrackNodeSelector : RailElectrificationTrackNodeSelector
    {
        private RailElectrificationMode _electrificationMode;

        public override void Select(Rail track, List<TrackConnection> result)
        {
            _electrificationMode = track.ElectrificationMode;
            if (InputHelper.Control)
                MultiSelect(track, result);
            else
                base.Select(track, result);
        }

        private void MultiSelect(Rail track, List<TrackConnection> result)
        {
            var treshold = WorldSettings.Current.GetFloat<MassElectrificationSettings>(MassElectrificationSettings.TrackCount).RoundToInt();
            base.Select(track, result);
            if (result.Count < treshold)
            {
                var connections = result.SelectMany(connection => connection.OuterConnections)
                    .Where(connection => IsValid(connection.Track as Rail) && !result.Any(it => it.Track == connection.Track))
                    .ToList();
                foreach (var connection in connections)
                {
                    if (result.Count < treshold)
                    {
                        MultiSelect(connection.Track as Rail, result);
                    }
                }
            }
        }

        protected override bool IsValid(Rail track)
        {
            return track.ElectrificationMode == _electrificationMode;
        }
    }
}

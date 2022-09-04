using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using VoxelTycoon;
using VoxelTycoon.Notifications;
using VoxelTycoon.Tracks;
using VoxelTycoon.Tracks.Rails;
using VoxelTycoon.UI;

namespace PathfinderPatch
{
    [HarmonyPatch]
    internal class ChainSignalPatch
    {
        private static readonly VoxelTycoon.Logger _logger = new Logger<ChainSignalPatch>();
        private static readonly ISet<Train> notifyBlocked = new HashSet<Train>();
        private static readonly Dictionary<Train, float> recentPassThroughNotifications = new Dictionary<Train, float>();

        [HarmonyPatch(typeof(ChainBlockRailSignal), nameof(ChainBlockRailSignal.IsOpenInternal))]
        internal static bool Prefix(ChainBlockRailSignal __instance, HashSet<RailSignal> visitedSignals, Train train, UniqueList<RailSignal> ____nextSignals, ref bool __result)
        {
            __result = IsOpenInternal(__instance, visitedSignals, train, ____nextSignals);
            return false;
        }

        private static bool IsOpenInternal(ChainBlockRailSignal signal, HashSet<RailSignal> visitedSignals, Train train, UniqueList<RailSignal> _nextSignals)
        {
            if (!signal.IsBuilt)
            {
                return true;
            }

            if (visitedSignals.Contains(signal))
            {
                _logger.Log($"Train {train.Id} encountered signal {signal} twice");
                return signal.Connection.InnerConnection.BlockIsOpen();
            }

            visitedSignals.Add(signal);
            if (!signal.Connection.InnerConnection.BlockIsOpen())
            {                
                if (!TrainWaitingForSignals(_nextSignals, train))
                {
                    return false;
                }
                if (train.Length > 40)
                {
                    notifyBlocked.Add(train);
                    return false;
                }
            }

            var nextSignal = train.GetNextSignal(signal);
            if (nextSignal != null && _nextSignals.Contains(nextSignal))
            {
                return nextSignal.IsOpenInternal(visitedSignals, train);
            }

            for (var i = 0; i < _nextSignals.Count; i++)
            {
                if (!_nextSignals[i].IsOpenInternal(visitedSignals, train))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool TrainWaitingForSignals(UniqueList<RailSignal> signals, Train train)
        {
            var any = false;
            for (var si = 0; si < signals.Count; si++)
            {
                var connection = signals[si].Connection;
                for (var ui = 0; ui < connection.Path.UnitCount; ui++)
                {
                    if (connection.Path.GetUnit(ui) != train)
                    {
                        return false;
                    }
                    any = true;
                }
            }
            return any;
        }

        [HarmonyPatch(typeof(Vehicle), methodName: "PushNotifications")]
        internal static void Postfix(Vehicle __instance)
        {
            if (!(__instance is Train)) return;

            var train = __instance as Train;
            recentPassThroughNotifications.TryGetValue(train, out var lastNotification);
            if (notifyBlocked.Remove(train)
                && Time.fixedTime - lastNotification > 15
                && train.IsWaitingForOpenSignal)
            {
                NotificationManager.Current.PushCritical($"{train.Name} blocked by itself",
                    $"{train.Name} is blocked by itself at a signal",
                    new GoToVehicleNotificationAction(train),
                    FontIcon.FaSolid("\uf637"));
                recentPassThroughNotifications[train] = Time.fixedTime;
            }
        }
    }
}

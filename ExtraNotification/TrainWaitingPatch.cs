using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using VoxelTycoon;
using VoxelTycoon.Notifications;
using VoxelTycoon.Tracks;
using VoxelTycoon.Tracks.Rails;
using VoxelTycoon.UI;

namespace ExtraNotification
{
    [HarmonyPatch]
    class TrainWaitingPatch
    {
        private static readonly VoxelTycoon.Logger _logger = new Logger<TrainWaitingPatch>();
        private static readonly Dictionary<Train, float> waiting = new Dictionary<Train, float>();

        [HarmonyPatch(typeof(Train), nameof(Train.DetectObstacle))]
        internal static void Postfix(Train __instance, ref object obstacle)
        {
            if (obstacle == Train.ClosedSignalObstacle && !waiting.ContainsKey(__instance))
            {
                waiting.Add(__instance, Time.fixedTime);                
            }
            else if (!__instance.IsWaitingForOpenSignal)
            {
                waiting.Remove(__instance);
            }
        }

        [HarmonyPatch(typeof(Vehicle), methodName: "PushNotifications")]
        internal static void Postfix(Vehicle __instance)
        {
            var warningTime = ExtraNotificationSettings.GetSignalWarningDays();
            if (__instance is Train
                && warningTime > 0
                && waiting.TryGetValue(__instance as Train, out float value)
                && Time.fixedTime - value > TimeManager.DaysToGameSeconds(warningTime))
            {
                NotificationManager.Current.PushWarning($"{__instance.Name} waiting at a signal",
                    $"{__instance.Name} has been waiting at a signal for {warningTime} days",
                    new GoToVehicleNotificationAction(__instance),
                    FontIcon.FaSolid("\uf637"));
                waiting.Remove(__instance as Train);
            }
        }
    }
}

using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using VoxelTycoon;
using VoxelTycoon.Notifications;
using VoxelTycoon.Tracks;
using VoxelTycoon.Tracks.Rails;
using VoxelTycoon.Tracks.Roads;
using VoxelTycoon.Tracks.Tasks;

namespace ExtraNotification
{
    [HarmonyPatch]
    class EmptyUnloadPatch
    {
        private static readonly Logger _logger = new Logger<EmptyUnloadPatch>();
        private static readonly ISet<Vehicle> _emptyUnload = new HashSet<Vehicle>();

        internal static bool GameStarted = false;

        [HarmonyPatch(typeof(TransferTask), methodName: "Run")]
        internal static bool Prefix(TransferTask __instance)
        {
            if (GameStarted
                && Enabled(__instance.Vehicle)
                && __instance.UnloadMode != TransferMode.None 
                && __instance.LoadMode == TransferMode.None
                && __instance.GetTargetUnits().ToList().All(it => it.Storage.Count == 0))
            {
                _logger.Log("Added " + __instance.Vehicle.Name);
                _emptyUnload.Add(__instance.Vehicle);
            }
            return true;
        }

        private static bool Enabled(Vehicle vehicle)
        {
            if (vehicle is Car) return ExtraNotificationSettings.Get(ExtraNotificationSettings.EmptyTruckWarning);
            if (vehicle is Train) return ExtraNotificationSettings.Get(ExtraNotificationSettings.EmptyTrainWarning);
            return false;
        }

        [HarmonyPatch(typeof(Vehicle), methodName: "PushNotifications")]
        internal static void Postfix(Vehicle __instance)
        {
            if (_emptyUnload.Remove(__instance))
            {
                _logger.Log("Removed " + __instance.Name);
                NotificationManager.Current.PushWarning($"{__instance.Name} has nothing to unload",
                    $"{__instance.Name} arrived at {__instance.VehicleStation.Location.Name} with Unload order, but doesn't have cargo to unload",
                    new GoToVehicleNotificationAction(__instance));
            }
        }
    }
}

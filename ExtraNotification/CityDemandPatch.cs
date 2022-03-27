using HarmonyLib;
using VoxelTycoon;
using VoxelTycoon.Cities;
using VoxelTycoon.Notifications;

namespace ExtraNotification
{
    [HarmonyPatch(typeof(CityDemand), nameof(CityDemand.PushNotification))]
    internal class CityDemandPatch
    {
        private static readonly Logger _logger = new Logger<CityDemandPatch>();

        internal static bool Prefix(string title)
        {
            if (title == S.DemandIncreased)
            {
                return ExtraNotificationSettings.Get(ExtraNotificationSettings.DemandIncreased);
            }
            if (title == S.DemandDecreased)
            {
                return ExtraNotificationSettings.Get(ExtraNotificationSettings.DemandDecreased);
            }
            if (title == S.DemandOversupplied)
            {
                return ExtraNotificationSettings.Get(ExtraNotificationSettings.DemandOversupplied);
            }
            if (title == S.DemandClosing)
            {
                return ExtraNotificationSettings.Get(ExtraNotificationSettings.StoreClosing);
            }
            if (title == S.DemandClosed)
            {
                return ExtraNotificationSettings.Get(ExtraNotificationSettings.StoreClosed);
            }
            return true;
        }
    }
}

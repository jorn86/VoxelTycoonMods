using HarmonyLib;
using VoxelTycoon;
using VoxelTycoon.Cities;
using VoxelTycoon.Notifications;

namespace ExtraNotification
{
    [HarmonyPatch(typeof(CityDemand), nameof(CityDemand.PushNotification))]
    class CityDemandPatch
    {
        private static readonly Logger _logger = new Logger<CityDemandPatch>();

        internal static bool Prefix(string title)
        {
            if (title == S.DemandIncreased)
            {
                _logger.Log("Demand increased warning");
                return ExtraNotificationSettings.Get(ExtraNotificationSettings.DemandIncreased);
            }
            if (title == S.DemandDecreased)
            {
                _logger.Log("Demand decreased warning");
                return ExtraNotificationSettings.Get(ExtraNotificationSettings.DemandDecreased);
            }
            if (title == S.DemandOversupplied)
            {
                _logger.Log("Demand oversupplied warning");
                return ExtraNotificationSettings.Get(ExtraNotificationSettings.DemandOversupplied);
            }
            if (title == S.DemandClosing)
            {
                _logger.Log("Demand closing warning");
                return ExtraNotificationSettings.Get(ExtraNotificationSettings.StoreClosing);
            }
            if (title == S.DemandClosed)
            {
                _logger.Log("Demand closed warning");
                return ExtraNotificationSettings.Get(ExtraNotificationSettings.StoreClosed);
            }
            return true;
        }
    }
}

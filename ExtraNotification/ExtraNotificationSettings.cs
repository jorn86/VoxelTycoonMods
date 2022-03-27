using VoxelTycoon;
using VoxelTycoon.Game.UI;
using VoxelTycoon.Modding;

namespace ExtraNotification
{
    class ExtraNotificationSettings : SettingsMod
    {
        public const string DemandOversupplied = "DemandOversupplied";
        public const string DemandIncreased = "DemandIncreased";
        public const string DemandDecreased = "DemandDecreased";
        public const string StoreClosing = "StoreClosing";
        public const string StoreClosed = "StoreClosed";
        public const string EmptyTrainWarning = "EmptyTrainWarning";
        public const string EmptyTruckWarning = "EmptyTruckWarning";
        public const string SignalWarning = "SignalWarning";

        protected override void SetDefaults(WorldSettings worldSettings)
        {
            worldSettings.SetBool<ExtraNotificationSettings>(DemandOversupplied, true);
            worldSettings.SetBool<ExtraNotificationSettings>(DemandIncreased, false);
            worldSettings.SetBool<ExtraNotificationSettings>(DemandDecreased, true);
            worldSettings.SetBool<ExtraNotificationSettings>(StoreClosing, true);
            worldSettings.SetBool<ExtraNotificationSettings>(StoreClosed, true);
            worldSettings.SetBool<ExtraNotificationSettings>(EmptyTrainWarning, true);
            worldSettings.SetBool<ExtraNotificationSettings>(EmptyTruckWarning, true);
            worldSettings.SetInt<ExtraNotificationSettings>(SignalWarning, 3);
        }

        protected override void SetupSettingsControl(SettingsControl settingsControl, WorldSettings worldSettings)
        {
            SetupToggle(settingsControl, worldSettings, DemandOversupplied, "Show demand oversupplied notifications",
                   "Allows you to disable the base game notification when a city store demand is oversupplied");
            SetupToggle(settingsControl, worldSettings, DemandIncreased, "Show demand increased notifications",
                   "Allows you to disable the base game notification when a city store increases its demand");
            SetupToggle(settingsControl, worldSettings, DemandDecreased, "Show demand decreased notifications",
                   "Allows you to disable the base game notification when a city store decreases its demand");
            SetupToggle(settingsControl, worldSettings, StoreClosing, "Show store is closing notifications",
                   "Allows you to disable the base game notification when a city store is closing because it's undersupplied");
            SetupToggle(settingsControl, worldSettings, StoreClosed, "Show store closed notifications",
                   "Allows you to disable the base game notification when a city store has closed");
            SetupToggle(settingsControl, worldSettings, EmptyTrainWarning, "Empty train unload",
                "Shows a warning when a train arrives at a station with only an Unload order, but there is no cargo");
            SetupToggle(settingsControl, worldSettings, EmptyTruckWarning, "Empty truck unload",
                "Shows a warning when a truck arrives at a station with only an Unload order, but there is no cargo");
            SetupSlider(settingsControl, worldSettings, SignalWarning, "Waiting at signal warning time",
                "Shows a warning when a train has been waiting at the same signal every this many in-game days. 0 is disabled.",
                0, 30);
        }

        private void SetupSlider(SettingsControl settingsControl, WorldSettings worldSettings, string id, string name, string description, int min, int max)
        {
            settingsControl.AddSlider(name, description,
                   () => worldSettings.GetInt<ExtraNotificationSettings>(id),
                   it => worldSettings.SetInt<ExtraNotificationSettings>(id, it.RoundToInt()),
                   min, max,
                   it => it.RoundToInt().ToString());
        }

        private void SetupToggle(SettingsControl settingsControl, WorldSettings worldSettings, string id, string name, string description)
        {
            settingsControl.AddToggle(name, description,
                () => worldSettings.GetBool<ExtraNotificationSettings>(id),
                it => worldSettings.SetBool<ExtraNotificationSettings>(id, it));
        }

        internal static bool Get(string setting)
        {
            return WorldSettings.Current.GetBool<ExtraNotificationSettings>(setting);
        }

        internal static int GetSignalWarningDays()
        {
            return WorldSettings.Current.GetInt<ExtraNotificationSettings>(SignalWarning);
        }
    }
}

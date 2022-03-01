using VoxelTycoon;
using VoxelTycoon.Game.UI;
using VoxelTycoon.Modding;

namespace MassElectrification
{
    class MassElectrificationSettings : SettingsMod
    {
        public const string TrackCount = "TrackCount";

        protected override void SetDefaults(WorldSettings worldSettings)
        {
            worldSettings.SetFloat<MassElectrificationSettings>(TrackCount, 1000);
        }

        protected override void SetupSettingsControl(SettingsControl settingsControl, WorldSettings worldSettings)
        {
            settingsControl.AddSlider("Track count", "The maximum number of tracks to electrify at once. " +
                "Raising this above 1000 or so may lead to performance issues, so be careful. " +
                "If the game freezes after pressing Control and hovering over the track, just wait until it un-freezes. " +
                "It'll happen eventually, but it may take a long time if you set this value too high.", 
                () => worldSettings.GetFloat<MassElectrificationSettings>(TrackCount),
                v => worldSettings.SetFloat<MassElectrificationSettings>(TrackCount, v),
                100, 10000,
                v => v.RoundToInt().ToString());
        }
    }
}

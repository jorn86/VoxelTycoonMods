using VoxelTycoon;
using VoxelTycoon.Game.UI;
using VoxelTycoon.Modding;

namespace Sandbox
{
    class SandboxSettings : SettingsMod
    {
        public const string FreeBuilding = "FreeBuilding";
        public const string FreeInfra = "FreeInfra";
        public const string FreeVehicle = "FreeVehicle";
        public const string FreeTerraform = "FreeTerraform";
        public const string FreeExplore = "FreeExplore";

        protected override void SetDefaults(WorldSettings worldSettings)
        {
            worldSettings.SetBool<SandboxSettings>(FreeBuilding, true);
            worldSettings.SetBool<SandboxSettings>(FreeInfra, true);
            worldSettings.SetBool<SandboxSettings>(FreeVehicle, true);
            worldSettings.SetBool<SandboxSettings>(FreeTerraform, true);
            worldSettings.SetBool<SandboxSettings>(FreeExplore, false);
        }

        protected override void SetupSettingsControl(SettingsControl settingsControl, WorldSettings worldSettings)
        {
            SetupSetting(settingsControl, worldSettings, FreeBuilding, "Free factories", "Mines, factories, labs and belts");
            SetupSetting(settingsControl, worldSettings, FreeInfra, "Free infrastructure", "Stations, depots, roads, rails and signals");
            SetupSetting(settingsControl, worldSettings, FreeVehicle, "Free vehicles", "Trains, wagons, buses and trucks");
            SetupSetting(settingsControl, worldSettings, FreeTerraform, "Free terraforming", "Raising and lowering land, removing houses, trees and building decorations");
            SetupSetting(settingsControl, worldSettings, FreeExplore, "Free exploration", "Unlocking new regions");
        }

        private void SetupSetting(SettingsControl settingsControl, WorldSettings worldSettings, string id, string name, string description)
        {
            settingsControl.AddToggle(name, description, worldSettings.GetBool<SandboxSettings>(id),
                // Using this [Obsolete] method turns the toggle into an on/off dropdown,
                // which is not as nice but at least it works. A normal Toggle doesn't save the setting.
                () => worldSettings.SetBool<SandboxSettings>(id, true),
                () => worldSettings.SetBool<SandboxSettings>(id, false));
        }

        internal static bool FreeIf(string setting, ref double __result)
        {
            if (WorldSettings.Current.GetBool<SandboxSettings>(setting))
            {
                __result = 0;
                return false;
            }
            return true;
        }
    }
}

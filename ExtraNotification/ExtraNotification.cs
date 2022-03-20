using HarmonyLib;
using UnityEngine;
using VoxelTycoon;
using VoxelTycoon.Modding;

namespace ExtraNotification
{
    class ExtraNotification : Mod
    {
        private static readonly VoxelTycoon.Logger _logger = new Logger<ExtraNotification>();
        private const string PatchId = "org.hertsig.voxeltycoon.ExtraNotification";

        protected override void Initialize()
        {
            new Harmony(PatchId).PatchAll();
            _logger.Log("ExtraNotification patch complete");
        }

        protected override void OnUpdate()
        {
            if (!EmptyUnloadPatch.GameStarted && Time.timeSinceLevelLoad > 10)
            {
                EmptyUnloadPatch.GameStarted = true;
            }
        }

        protected override void Deinitialize()
        {
            EmptyUnloadPatch.GameStarted = false;
            new Harmony(PatchId).UnpatchAll();
            _logger.Log("ExtraNotification un-patch complete");
        }
    }
}

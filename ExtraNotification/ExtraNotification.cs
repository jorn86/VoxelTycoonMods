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
            //_logger.Log($"{Time.time}; {Time.timeSinceLevelLoad}; {Time.frameCount}");
            if (!EmptyUnloadPatch.GameStarted && Time.timeSinceLevelLoad > 10)
            {
                _logger.Log($"{EmptyUnloadPatch.GameStarted} to true");
                EmptyUnloadPatch.GameStarted = true;
            }
        }

        protected override void Deinitialize()
        {
            _logger.Log($"{EmptyUnloadPatch.GameStarted} to false");
            EmptyUnloadPatch.GameStarted = false;
            new Harmony(PatchId).UnpatchAll();
            _logger.Log("ExtraNotification un-patch complete");
        }
    }
}

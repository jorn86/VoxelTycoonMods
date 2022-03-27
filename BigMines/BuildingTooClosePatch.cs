using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using VoxelTycoon;
using VoxelTycoon.Buildings;
using VoxelTycoon.Tools.Builder;
using Logger = VoxelTycoon.Logger;

namespace BigMines
{
    [HarmonyPatch(typeof(BuildingHelper), nameof(BuildingHelper.TooClose))]
    internal class BuildingTooClosePatch
    {
        private static readonly Logger _logger = new Logger<BuildingTooClosePatch>();
        private static readonly Dictionary<Item, List<int>> MineAssetIdsPerItem = new Dictionary<Item, List<int>>();

        internal static bool Prefix(int assetId, Xyz size, Xyz position, int spacing, ref Collider[] ____colliders, ref bool __result)
        {
            var asset = AssetLibrary.Current.TryGet<Building>(assetId);
            if (!(asset is Mine mine)) return true;

            __result = TooClose(assetId, size, position, spacing, ref ____colliders);
            return false;
        }

        private static bool TooClose(int assetId, Xyz size, Xyz position, int spacing, ref Collider[] colliders)
        {
            if (spacing < 1) return false;

            var distance = Mathf.Max(size.X, size.Z) * 30f; // was: 1.5 * spacing
            var count = Physics.OverlapBoxNonAlloc(
                new Vector3(position.X + size.X / 2f, 0.0f, position.Z + size.Z / 2f),
                new Vector3(distance, 31f, distance),
                colliders, Quaternion.identity, 1024);

            if (!MineAssetIdsPerItem.Any())
            {
                InitAssetIds();
            }

            for (var index = 0; index < count; ++index)
            {
                var component = colliders[index].GetComponent<Building>();
                colliders[index] = null;
                if (component is Mine mine
                    && mine.IsBuilt
                    && SameItemType(assetId, mine.AssetId)
                    && BuildingHelper.GetDistance(position, size, mine.Position, mine.Size) < Math.Max(spacing, mine.SharedData.Spacing))
                {
                    return true;
                }
            }
            return false;
        }

        private static void InitAssetIds()
        {
            foreach (var recipe in BuildingRecipeManager.Current.GetAll().Enumerate()
                         .OfType<MineRecipe>())
            {
                var mineAssetId = recipe.Building.AssetId;
                StorageManager.Current.GetStorages(mineAssetId).Enumerate().ForEach(storage =>
                    {
                        if (!MineAssetIdsPerItem.TryGetValue(storage.Item, out var list))
                        {
                            list = new List<int>();
                            MineAssetIdsPerItem[storage.Item] = list;
                        }
                        list.Add(mineAssetId);
                    });
            }
            MineAssetIdsPerItem.ForEach(e => _logger.Log($"{e.Key.DisplayName}: {e.Value.Join()}"));
        }

        private static bool SameItemType(int firstAsset, int secondAsset)
        {
            return firstAsset == secondAsset ||
                   MineAssetIdsPerItem.Values.Where(it => it.Contains(firstAsset))
                       .Any(it => it.Contains(secondAsset));
        }
    }
}

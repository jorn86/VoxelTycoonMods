using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VoxelTycoon;
using VoxelTycoon.Buildings;
using VoxelTycoon.Game.UI;
using VoxelTycoon.Recipes;
using VoxelTycoon.UI;

namespace FactoryBuffer
{
    [HarmonyPatch(typeof(DeviceWindowOverviewTab), methodName: "InvalidateRequiredResources")]
    class DeviceUiPatch
    {
        internal static bool Prefix(bool immidiate, Device ____device, List<Item> ____allItems, Dictionary<Item, ProgressResourceView> ____itemToProgressResourceView)
        {
            InvalidateRequiredResources(immidiate, ____device, ____allItems, ____itemToProgressResourceView);
            return false;
        }

        private static void InvalidateRequiredResources(bool immidiate, Device _device, List<Item> _allItems, Dictionary<Item, ProgressResourceView> _itemToProgressResourceView)
        {
            if (_device.Recipe == null)
            {
                return;
            }

            foreach (var item in _allItems)
            {
                var inputItems = _device.Recipe.InputItems;
                var progressResourceView = _itemToProgressResourceView[item];
                var recipeItem = inputItems.FirstOrDefault((RecipeItem x) => x.Item == item);
                if (recipeItem != null)
                {
                    var waitingItemCount = _device.GetWaitingItemCount(recipeItem.Item);
                    progressResourceView.MaxValue = ConsumeItemsPatch.MaxBuffer(recipeItem);
                    progressResourceView.SetValue(Mathf.RoundToInt(waitingItemCount), immidiate);
                    progressResourceView.SetActive(active: true);
                    if (waitingItemCount >= recipeItem.Count) {
                        progressResourceView.FillColor = UIColors.Colors.SolidGreen;
                    }
                    else
                    {
                        progressResourceView.FillColor = UIColors.Colors.SolidOrange;
                    }
                }
                else
                {
                    progressResourceView.SetActive(active: false);
                }
            }
        }
    }
}

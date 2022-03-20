using HarmonyLib;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelTycoon;
using VoxelTycoon.Buildings;
using VoxelTycoon.Recipes;
using VoxelTycoon.Tracks;
using VoxelTycoon.Tracks.Conveyors;
using System.Threading.Tasks;

namespace FactoryBuffer
{
    [HarmonyPatch(typeof(DeviceHelper), nameof(DeviceHelper.WaitForConsumeItems))]
    class ConsumeItemsPatch
    {
        private static readonly VoxelTycoon.Logger _logger = new Logger<ConsumeItemsPatch>();

        internal static bool Prefix(Recipe recipe, IList<TrackConnection> connections, IDictionary<Item, int> waitingItems, ref IEnumerator __result)
        {
            __result = WaitForConsumeItems(recipe, connections, waitingItems);
            return false;
        }

        public static int MaxBuffer(RecipeItem item)
        {
            return Mathf.RoundToInt(item.Count * 3f);
        }

        private static IEnumerator WaitForConsumeItems(Recipe recipe, IList<TrackConnection> connections, IDictionary<Item, int> waitingItems)
        {
            while (!ConsumeAll(waitingItems, recipe))
            {
                if (Load(recipe, connections, waitingItems))
                {
                    yield return null;
                }
                else
                {
                    yield return new WaitForSeconds(0.1f);
                }
            }

            BackgroundLoad(recipe, connections, waitingItems);
        }

        private static bool ConsumeAll(IDictionary<Item, int> waitingItems, Recipe recipe)
        {
            if (!recipe.InputItems.All(it => waitingItems.TryGetValue(it.Item, out int num) && num > it.Count))
            {
                return false;
            }
            foreach (var item in recipe.InputItems)
            {
                waitingItems[item.Item] = waitingItems[item.Item] - Mathf.RoundToInt(item.Count);
            }
            return true;
        }

        private static string PrintItems(IDictionary<Item, int> waitingItems)
        {
            return waitingItems.Join(e => $"{e.Value} {e.Key.DisplayName}");
        }

        private static bool BufferFull(Recipe recipe, IDictionary<Item, int> waitingItems)
        {
            if (recipe.InputItems.All(it => waitingItems.TryGetValue(it.Item, out int num) && num >= MaxBuffer(it)))
            {
                return true;
            }
            return false;
        }

        private static bool Load(Recipe recipe, IList<TrackConnection> connections, IDictionary<Item, int> waitingItems)
        {
            var any = false;
            foreach (var item in recipe.InputItems)
            {
                if (!waitingItems.TryGetValue(item.Item, out int num) || num < MaxBuffer(item))
                {
                    foreach (TrackConnection connection in connections)
                    {
                        if (CargoHelper.InputItem(connection, item.Item))
                        {
                            waitingItems[item.Item] = num + 1;
                            any = true;
                        }
                    }
                }
            }
            return any;
        }

        private static async void BackgroundLoad(Recipe recipe, IList<TrackConnection> connections, IDictionary<Item, int> waitingItems)
        {
            var timeSinceLastInput = Time.time;
            while (!BufferFull(recipe, waitingItems))
            {
                if (Load(recipe, connections, waitingItems))
                {
                    timeSinceLastInput = Time.time;
                }
                if (timeSinceLastInput + 10f < Time.time)
                {
                    return;
                }
                await Task.Delay(300);
            }
        }
    }
}

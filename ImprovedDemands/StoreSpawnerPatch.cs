﻿using HarmonyLib;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using VoxelTycoon;
using VoxelTycoon.Cities;

namespace ImprovedDemands
{
    [HarmonyPatch(typeof(CityStoreSpawner), nameof(CityStoreSpawner.GetSpawnInfo))]
    class StoreSpawnerPatch
    {
        private static readonly VoxelTycoon.Logger _logger = new Logger<StoreSpawnerPatch>();

        private static readonly List<Region> _unlockedRegions = new List<Region>();
        private static readonly UniqueList<Item> _unlockedRegionItems = new UniqueList<Item>();

        internal static bool Prefix(City city, QuickRandom random, ref CityStoreSpawnInfo __result)
        {
            try
            {
                __result = GetSpawnInfo(city, random);
            }
            finally
            {
                _unlockedRegions.Clear();
                _unlockedRegionItems.Clear();
            }
            return false;
        }

        private static CityStoreSpawnInfo GetSpawnInfo(City city, QuickRandom random)
        {
            var currentDemands = Enumerate(city.Demands.Count, i => city.Demands[i])
                .OrderBy(it => it.Tier)
                .ToList();

            var allDemands = CityStoreSpawnInfoManager.Current.GetAll();
            var possibleDemands = Enumerate(allDemands.Count, i => allDemands[i])
                // region supports it
                .Where(it => city.Region.Biome.Tags.Contains(it.Tag))
                // it's not already demanded
                .Where(it => !currentDemands.Any(d => it.Item == d.Item))
                .ToList();

            var possibleTiers = CalculateNewTierProbabilities(currentDemands, possibleDemands.Select(it => it.Tier).Distinct());

            var tier = RandomHelper.FromProbability(random, possibleTiers);
            _logger.Log($"Randomly selected tier {tier} from {possibleTiers.Join(it => $"{it.Value} (p{it.Probability})")} for {city.Name}");
            
            DetermineAvailableResources();
            var finalSelection = possibleDemands.Where(it => it.Tier == tier)
                .Select(it => new ValueProbability<CityStoreSpawnInfo>(it, GetSpawnableRecipeProbability(it)))
                .ToList();

            var result = RandomHelper.FromProbability(random, finalSelection);
            _logger.Log($"Spawning {result.Item.DisplayName} (T{result.Tier}) store in {city.Name}");
            return result;
        }

        private static void DetermineAvailableResources()
        {
            RegionHelper.GetUnlockedRegions(_unlockedRegions);
            _unlockedRegions
                .SelectMany(it => Enumerate(it.Deposits.Count, i => it.Deposits[i].Item))
                .ToList()
                .ForEach(it => _unlockedRegionItems.Add(it));
        }

        private static List<ValueProbability<int>> CalculateNewTierProbabilities(IEnumerable<CityDemand> currentDemands, IEnumerable<int> possibleTiers)
        {
            var probabilities = new List<ValueProbability<int>>();
            if (currentDemands.Count() == 0)
            {
                probabilities.Add(new ValueProbability<int>(0, 1));
                return probabilities;
            }

            _logger.Log("Currently satisfied by tier:");
            var satisfactionByTier = currentDemands
                .Where(it => it.DeliveredCounter.Lifetime > 0 && it.SatisfactionGrade >= Grade.Normal)
                .GroupBy(it => it.Tier)
                .ToDictionary(it => it.Key, it =>
                {
                    int sum = it.Sum(demand => (int) demand.SatisfactionGrade);
                    _logger.Log($"T{it.Key}: {it.Join(demand => $"{demand.Item.DisplayName}: {demand.SatisfactionGrade.GetDisplayName()} ({(int) demand.SatisfactionGrade})")}");
                    return sum;
                });

            var highestSatisfiedTier = satisfactionByTier.Count == 0 ? 0 : satisfactionByTier.Max(it => it.Key);
            satisfactionByTier.TryGetValue(highestSatisfiedTier - 1, out int secondHighestTierSatisfaction);
            satisfactionByTier.TryGetValue(highestSatisfiedTier, out int highestTierSatisfaction);
            var totalSatisfaction = satisfactionByTier.Sum(e => e.Value);
            
            if (possibleTiers.Contains(highestSatisfiedTier + 1) && highestTierSatisfaction >= 5)
            {
                probabilities.Add(new ValueProbability<int>(highestSatisfiedTier + 1, highestTierSatisfaction));
                probabilities.Add(new ValueProbability<int>(highestSatisfiedTier, secondHighestTierSatisfaction));
            }
            else if (possibleTiers.Contains(highestSatisfiedTier))
            {
                probabilities.Add(new ValueProbability<int>(highestSatisfiedTier, highestTierSatisfaction + secondHighestTierSatisfaction));
            }
            if (possibleTiers.Contains(highestSatisfiedTier - 1) && totalSatisfaction > 2 * highestTierSatisfaction)
            {
                probabilities.Add(new ValueProbability<int>(highestSatisfiedTier - 1, totalSatisfaction - highestTierSatisfaction - secondHighestTierSatisfaction));
            }
            if (probabilities.Count == 0)
            {
                var fallbackTier = possibleTiers.Where(it => it <= highestSatisfiedTier).Max();
                _logger.Log($"No valid results for tiers {highestSatisfiedTier - 1} - {highestSatisfiedTier + 1}, adding {fallbackTier}");
                probabilities.Add(new ValueProbability<int>(fallbackTier, 1));
            }
            return probabilities;
        }

        private static IEnumerable<V> Enumerate<V>(int size, Func<int, V> indexer) {
            for (int i = 0; i < size; i++)
            {
                yield return indexer.Invoke(i);
            }
        }

        private static float GetSpawnableRecipeProbability(CityStoreSpawnInfo recipe)
        {
            float probability = 1f / Mathf.Pow(GetDemandsCount(recipe.Item) * 2 + 1, 2f);
            if (!IsIngredientsUnlocked(recipe.Item))
            {
                probability /= 10f;
            }

            return probability;
        }

        private static int GetDemandsCount(Item item)
        {
            int num = 0;
            for (int i = 0; i < _unlockedRegions.Count; i++)
            {
                ImmutableList<City> cities = _unlockedRegions[i].Cities;
                for (int j = 0; j < cities.Count; j++)
                {
                    CityDemandCollection demands = cities[j].Demands;
                    for (int k = 0; k < demands.Count; k++)
                    {
                        if (demands[k].Item == item)
                        {
                            num++;
                        }
                    }
                }
            }

            return num;
        }

        private static bool IsIngredientsUnlocked(Item item)
        {
            ImmutableList<Item> ingredients = Manager<RecipeManager>.Current.GetIngredients(item);
            for (int i = 0; i < ingredients.Count; i++)
            {
                if (!_unlockedRegionItems.Contains(ingredients[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
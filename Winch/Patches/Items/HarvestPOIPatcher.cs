using HarmonyLib;
using System;
using System.Collections.Generic;
using Winch.Core;
using Winch.Util;
using System.Threading.Tasks;

namespace Winch.Patches.Items;

[HarmonyPatch(typeof(HarvestValidator))]
[HarmonyPatch("Awake")]
public class HarvestValidatorPatcher
{
    public static void Prefix(HarvestValidator __instance)
    {
        __instance.allHarvestPOIs.ForEach(POI =>
        {
            // Could do as nested loop to handle both, but isn't really necessary for two things.
            for (int i = 0; i < POI.harvestPOIData.items.Count; i++)
            {
                var currentItem = POI.harvestPOIData.items[i];
                if (ItemUtil.itemsToReplace.ContainsKey(currentItem.id))
                {
                    WinchCore.Log.Info($"Replaced {currentItem.id}");
                    POI.harvestPOIData.items[i] = (HarvestableItemData) ItemUtil.itemsToReplace[currentItem.id];
                }
            }
            for (int i = 0; i < POI.harvestPOIData.nightItems.Count; i++)
            {
                var currentItem = POI.harvestPOIData.nightItems[i];
                if (ItemUtil.itemsToReplace.ContainsKey(currentItem.id))
                {
                    WinchCore.Log.Info($"Replaced {currentItem.id}");
                    POI.harvestPOIData.nightItems[i] = (HarvestableItemData)ItemUtil.itemsToReplace[currentItem.id];
                }
            }
        });
    }
}

[HarmonyPatch(typeof(HarvestPOIDataModel))]
[HarmonyPatch("GetParticlePrefab")]
public class HarvestPOIDataModelPatcher
{
    public static void Postfix(ref UnityEngine.GameObject __result)
    {
        if (__result == null)
        {
            __result = GetDefaultParticle();
        }
    }

    public static UnityEngine.GameObject GetDefaultParticle()
    {
        if (PoiUtil.DefaultHarvestParticlePrefab != null) return PoiUtil.DefaultHarvestParticlePrefab;
        int i = 0;
        while (GameManager.Instance.ItemManager.allItems[i].id != "cod" && i < GameManager.Instance.ItemManager.allItems.Count) i++;
        return GameManager.Instance.ItemManager.allItems[i].harvestParticlePrefab;
    }
}
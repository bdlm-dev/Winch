using DG.Tweening.Core.Easing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Winch.Core;
using Winch.Core.API.Events.Addressables;
using Winch.Serialization;
using Winch.Serialization.Item;

namespace Winch.Util;

internal static class ItemUtil
{
    private static readonly Dictionary<Type, IDredgeTypeConverter> Converters = new()
    {
        { typeof(NonSpatialItemData), new NonSpatialItemDataConverter() },
        { typeof(MessageItemData), new MessageItemDataConverter() },
        { typeof(ResearchableItemData), new ResearchableItemDataConverter() },
        { typeof(SpatialItemData), new SpatialItemDataConverter() },
        { typeof(EngineItemData), new EngineItemDataConverter() },
        { typeof(FishItemData), new FishItemDataConverter() },
        { typeof(RelicItemData), new RelicItemDataConverter() },
        { typeof(DeployableItemData), new DeployableItemDataConverter() },
        { typeof(DredgeItemData), new DredgeItemDataConverter() },
        { typeof(RodItemData), new RodItemDataConverter() },
        { typeof(LightItemData), new LightItemDataConverter() },
        { typeof(DamageItemData), new DamageItemDataConverter() },
    };

    public static UnityEngine.GameObject? defaultHarvestParticlePrefab;

    public static Dictionary<string, ItemData> harvestableItemDataDict = new();
    public static Dictionary<string, ItemData> allItemDataDict = new();
    public static Dictionary<string, ItemData> itemsToReplace = new();

    public static void PopulateItemData()
    {
        if (allItemDataDict.Count > 0) return;
        foreach (var item in GameManager.Instance.ItemManager.allItems)
        {
            if (item is FishItemData or RelicItemData or HarvestableItemData)
            {
                harvestableItemDataDict.Add(item.id, item);
                //WinchCore.Log.Debug($"Added item {item.id} to harvestableItemDataDict");
            }
            allItemDataDict.Add(item.id, item);
            //WinchCore.Log.Debug($"Added item {item.id} to allItemDataDict");
        }

        ReplaceItems();
    }

    internal static void AddItemFromMeta<T>(string metaPath) where T : ItemData
    {
        var meta = UtilHelpers.ParseMeta(metaPath);
        if (meta == null)
        {
            WinchCore.Log.Error($"Meta file {metaPath} is empty");
            return;
        }
        var itemName = (string)meta["id"];
        if (allItemDataDict.ContainsKey(itemName))
        {
            if (meta.ContainsKey("doReplaceOriginal") && (bool)meta["doReplaceOriginal"]) { }
            else
            {
                WinchCore.Log.Error($"Duplicate item {itemName} at {metaPath} failed to load");
                return;
            }
        }
        var item = UtilHelpers.GetScriptableObjectFromMeta<T>(meta, metaPath);
        if (UtilHelpers.PopulateObjectFromMeta<T>(item, meta, Converters))
        {
            if (meta.ContainsKey("doReplaceOriginal") && (bool)meta["doReplaceOriginal"])
            {
                itemsToReplace.Add(item.id, item);
                WinchCore.Log.Info($"Added {item.id} to itemsToReplace");
            }
            else
            {
                GameManager.Instance.ItemManager.allItems.Add(item);
            }
        }
    }

    internal static void ReplaceItems()
    {
        WinchCore.Log.Info($"Attempting to replace {itemsToReplace.Count} item{(itemsToReplace.Count > 1 ? "s" : "")} in allItems.");
        try
        {
            for(int i = 0; i < GameManager.Instance.ItemManager.allItems?.Count; i++) 
            {
                if (itemsToReplace.ContainsKey(GameManager.Instance.ItemManager.allItems[i].id))
                {
                    GameManager.Instance.ItemManager.allItems[i] = itemsToReplace[GameManager.Instance.ItemManager.allItems[i].id];
                }
            }
        }
        catch (Exception ex)
        {
            WinchCore.Log.Debug($"Error replacing items in allItems: {ex}");
        }
    }
}

using AeLa.EasyFeedback.APIs;
using HarmonyLib;
using System;
using Winch.Core;
using Winch.Util;
using System.Collections.Generic;

namespace Winch.Patches.Items;

[HarmonyPatch(typeof(Encyclopedia))]
[HarmonyPatch("Awake")]
public class EncyclopediaPatcher
{
    public static void Prefix(ref List<FishItemData> ___allFish)
    {
        WinchCore.Log.Info($"Attempting to replace fish in Encyclopedia.");
        try
        {
            for (int i = 0; i < ___allFish.Count; i++)
            {
                if (ItemUtil.itemsToReplace.ContainsKey(___allFish[i].id))
                {
                    ___allFish[i] = (FishItemData) ItemUtil.itemsToReplace[___allFish[i].id];
                }
            }
        }
        catch (Exception ex)
        {
            WinchCore.Log.Debug($"Error replacing fish in Encyclopedia: {ex}");
        }
    }
}
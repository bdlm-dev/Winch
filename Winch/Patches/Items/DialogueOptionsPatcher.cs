using Antlr4.Runtime.Misc;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winch.Core;
using Winch.Util;
using UnityEngine.Localization.Settings;

namespace Winch.Patches.Items;

[HarmonyPatch(typeof(DialogManager))]
[HarmonyPatch("ShowDialog")]
internal class DialogManagerPatcher
{
    public static void Prefix(DialogOptions dialogOptions)
    {
        var args = dialogOptions.textArguments;
        for (int i = 0; i < args.Length; i++) {
            if (ItemUtil.itemsToReplace.ContainsKey((string)args[i]))
            {
                dialogOptions.textArguments[i] = LocalizationSettings.StringDatabase.GetLocalizedStringAsync("ITEMS", LocalizationSettings.LocaleItemUtil.itemsToReplace)
            }
        }
    }
}

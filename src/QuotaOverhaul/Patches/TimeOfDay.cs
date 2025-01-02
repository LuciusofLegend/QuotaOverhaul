using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using QuotaOverhaul;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;

namespace QuotaOverhaul
{

    [HarmonyPatch(typeof(TimeOfDay))]
    public class QuotaVariablesPatch
    {
        [HarmonyPatch("Awake")]
        [HarmonyPrefix]
        public static void SetQuotaVariables()
        {
            var quotaVariables = TimeOfDay.Instance.quotaVariables;

            quotaVariables.startingQuota = Config.startingQuota.Value;
            quotaVariables.baseIncrease = Config.quotaMinIncrease.Value;
            quotaVariables.increaseSteepness = Config.quotaIncreaseSteepness.Value;
            quotaVariables.randomizerMultiplier = Config.quotaRandomizerMultiplier.Value;
            QuotaManager.baseProfitQuota = quotaVariables.startingQuota;
        }
    }

    [HarmonyPatch(typeof(TimeOfDay))]
    public class QuotaUpdatePatch
    {
        [HarmonyPatch("SetNewProfitQuota")]
        [HarmonyPrefix]
        public static void RevertQuota()
        {
            TimeOfDay.Instance.profitQuota = QuotaManager.baseProfitQuota;
        }

        [HarmonyPostfix]
        public static void SaveQuota()
        {
            QuotaManager.baseProfitQuota = TimeOfDay.Instance.profitQuota;
        }
    }
}
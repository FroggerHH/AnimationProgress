// using System.Collections.Generic;
// using Extensions;
// using HarmonyLib;
// using UnityEngine;
// using static AnimationProgress.Plugin;
// using static ZoneSystem;
//
// namespace AnimationProgress.Patch;
//
// [HarmonyPatch]
// public static class SaveIDs
// {
//     private static bool spawningHaldor;
//
//     [HarmonyPatch(typeof(ZoneSystem), nameof(ZoneSystem.SpawnLocation))]
//     [HarmonyPrefix]
//     private static void SpawningHaldorPrefix(ZoneSystem __instance, ZoneLocation location,
//         Vector3 pos, List<GameObject> spawnedGhostObjects)
//     {
//         if (location.m_prefabName != HALDOR_LOCATION_NAME) return;
//
//         spawningHaldor = true;
//     }
//
//     [HarmonyPatch(typeof(ZoneSystem), nameof(ZoneSystem.SpawnLocation))]
//     [HarmonyPostfix]
//     private static void SpawningHaldorPostfix(ZoneSystem __instance, ZoneLocation location,
//         Vector3 pos, List<GameObject> spawnedGhostObjects)
//     {
//         if (location.m_prefabName != HALDOR_LOCATION_NAME) return;
//
//         spawningHaldor = false;
//     }
//
//     [HarmonyPatch(typeof(ZNetView), nameof(ZNetView.Awake))]
//     [HarmonyWrapSafe]
//     [HarmonyPostfix]
//     private static void ZNViewAwake(ZNetView __instance)
//     {
//         if (!spawningHaldor) return;
//
//         var id = __instance.GetZDO().Get_ID();
//         if (savedIDs.Contains(id)) return;
//
//         savedIDs.Add(id);
//
//         savedIDsConfig.Value = string.Join(",", savedIDs);
//         _self.Config.Reload();
//         _self.UpdateConfiguration();
//     }
// }
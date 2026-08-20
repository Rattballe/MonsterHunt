using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterHunt;

[HarmonyPatch(typeof(EnemyDirector), "AmountSetup")]
internal static class EnemyDensityPatch
{
    [HarmonyPostfix]
    private static void Postfix(
        EnemyDirector __instance,
        ref int ___totalAmount,
        ref List<EnemySetup> ___enemyList,
        ref List<EnemySetup> ___enemyListCurrent)
    {
        if (!Plugin.IsHost() || __instance == null || ___enemyList == null)
            return;

        int multiplier = Mathf.Clamp(Plugin.EnemyMultiplier.Value, 1, 10);
        if (multiplier <= 1) return;

        List<EnemySetup> original = new(___enemyList);
        for (int i = 1; i < multiplier; i++)
            ___enemyList.AddRange(original);

        if (___enemyListCurrent != null && ___enemyListCurrent.Count > 0)
        {
            List<EnemySetup> current = new(___enemyListCurrent);
            for (int i = 1; i < multiplier; i++)
                ___enemyListCurrent.AddRange(current);
        }

        ___totalAmount = ___enemyList.Count;

        if (Plugin.DebugLogging.Value)
            Plugin.Log.LogInfo($"Enemy density changed: {original.Count} -> {___enemyList.Count}");
    }
}

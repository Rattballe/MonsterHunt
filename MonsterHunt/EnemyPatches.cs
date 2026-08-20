using HarmonyLib;
using System.Collections.Generic;

namespace MonsterHunt;

internal static class RewardTracker
{
    private static readonly HashSet<int> Rewarded = new();

    internal static void TryReward(EnemyHealth health)
    {
        if (health == null || !Plugin.IsHost()) return;
        int id = health.GetInstanceID();
        lock (Rewarded)
        {
            if (!Rewarded.Add(id)) return;
        }

        try
        {
            EnemyParent parent = health.enemy != null ? health.enemy.EnemyParent : health.GetComponentInParent<EnemyParent>();
            int reward = Plugin.RewardFor(parent);
            Plugin.AddMoney(reward);
            if (Plugin.DebugLogging.Value)
                Plugin.Log.LogInfo($"Monster killed: {(parent != null ? parent.enemyName : health.name)} +${reward}");
        }
        catch (System.Exception ex)
        {
            Plugin.Log.LogWarning($"Monster reward failed: {ex.Message}");
        }
    }
}

[HarmonyPatch(typeof(EnemyHealth), "Death")]
internal static class EnemyDeathPatch
{
    [HarmonyPostfix]
    private static void Postfix(EnemyHealth __instance) => RewardTracker.TryReward(__instance);
}

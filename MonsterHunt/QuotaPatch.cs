using HarmonyLib;

namespace MonsterHunt;

[HarmonyPatch(typeof(RoundDirector), "StartRoundLogic")]
internal static class NoQuotaPatch
{
    [HarmonyPrefix]
    private static void Prefix(ref int value)
    {
        if (!Plugin.IsHost() || !Plugin.DisableQuota.Value) return;
        value = int.MaxValue;
    }
}

using HarmonyLib;
using System.Reflection;

namespace MonsterHunt;

[HarmonyPatch(typeof(ValuableObject), "DollarValueSetLogic")]
internal static class ValuableMoneyPatch
{
    [HarmonyPostfix]
    private static void Postfix(ValuableObject __instance)
    {
        if (!Plugin.IsHost() || !Plugin.DisableValuableMoney.Value || __instance == null) return;

        try
        {
            FieldInfo current = AccessTools.Field(typeof(ValuableObject), "dollarValueCurrent");
            FieldInfo original = AccessTools.Field(typeof(ValuableObject), "dollarValueOriginal");
            current?.SetValue(__instance, 0f);
            original?.SetValue(__instance, 0f);
        }
        catch (System.Exception ex)
        {
            Plugin.Log.LogWarning($"Could not zero valuable value: {ex.Message}");
        }
    }
}

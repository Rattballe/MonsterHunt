using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace MonsterHunt;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class Plugin : BaseUnityPlugin
{
    public const string PluginGuid = "rattballe.repo.monsterhunt";
    public const string PluginName = "Monster Hunt";
    public const string PluginVersion = "0.2.0";

    internal static ManualLogSource Log;
    internal static ConfigEntry<bool> Enabled;
    internal static ConfigEntry<int> EnemyMultiplier;
    internal static ConfigEntry<int> Tier1Reward;
    internal static ConfigEntry<int> Tier2Reward;
    internal static ConfigEntry<int> Tier3Reward;
    internal static ConfigEntry<bool> DisableValuableMoney;
    internal static ConfigEntry<bool> DisableQuota;
    internal static ConfigEntry<bool> HostOnly;
    internal static ConfigEntry<bool> DebugLogging;
    internal static ConfigEntry<bool> FreeShotgun;
    internal static ConfigEntry<bool> InfiniteCrystals;
    internal static ConfigEntry<int> InfiniteCrystalAmount;

    private Harmony harmony;
    private float currencyTimer;

    private void Awake()
    {
        Log = Logger;
        Enabled = Config.Bind("General", "Enabled", true, "Enable Monster Hunt.");
        EnemyMultiplier = Config.Bind("Enemies", "EnemyMultiplier", 3, "Repeat the vanilla enemy selection this many times. 1 = vanilla.");
        Tier1Reward = Config.Bind("Rewards", "Tier1Reward", 100, "Reward for low-tier monster kills.");
        Tier2Reward = Config.Bind("Rewards", "Tier2Reward", 250, "Reward for medium-tier monster kills.");
        Tier3Reward = Config.Bind("Rewards", "Tier3Reward", 750, "Reward for high-tier monster kills.");
        DisableValuableMoney = Config.Bind("Economy", "DisableValuableMoney", true, "Normal valuables give no run currency.");
        DisableQuota = Config.Bind("GameMode", "DisableQuota", true, "Make the normal round quota effectively unreachable.");
        HostOnly = Config.Bind("Multiplayer", "HostOnly", true, "Only the host modifies spawning and currency.");
        DebugLogging = Config.Bind("Debug", "DebugLogging", false, "Enable diagnostic logging.");
        FreeShotgun = Config.Bind("Loadout", "FreeShotgun", true, "Spawn a free vanilla shotgun when a level starts.");
        InfiniteCrystals = Config.Bind("Loadout", "InfiniteCrystals", true, "Keep run currency at the configured amount.");
        InfiniteCrystalAmount = Config.Bind("Loadout", "InfiniteCrystalAmount", int.MaxValue, "Currency maintained by InfiniteCrystals. int.MaxValue is effectively unlimited.");

        if (!Enabled.Value)
            return;

        harmony = new Harmony(PluginGuid);
        harmony.PatchAll(Assembly.GetExecutingAssembly());
        Log.LogInfo("Monster Hunt 0.2.0 loaded: free shotgun + unlimited currency enabled.");
    }

    private void Update()
    {
        if (!Enabled.Value || !InfiniteCrystals.Value || !IsHost())
            return;

        currencyTimer -= Time.unscaledDeltaTime;
        if (currencyTimer > 0f)
            return;

        currencyTimer = 0.5f;
        try
        {
            SemiFunc.StatSetRunCurrency(InfiniteCrystalAmount.Value);
        }
        catch (System.Exception ex)
        {
            if (DebugLogging.Value)
                Log.LogWarning($"Could not maintain infinite currency: {ex.Message}");
        }
    }

    internal static bool IsHost()
    {
        try { return SemiFunc.IsMasterClientOrSingleplayer(); }
        catch { return true; }
    }

    internal static int RewardFor(EnemyParent enemy)
    {
        try
        {
            int difficulty = (int)enemy.difficulty;
            return difficulty >= 2 ? Tier3Reward.Value : difficulty == 1 ? Tier2Reward.Value : Tier1Reward.Value;
        }
        catch { return Tier1Reward.Value; }
    }

    internal static void AddMoney(int amount)
    {
        if (amount <= 0 || !IsHost() || InfiniteCrystals.Value)
            return;

        int current = SemiFunc.StatGetRunCurrency();
        SemiFunc.StatSetRunCurrency(current + amount);
    }
}

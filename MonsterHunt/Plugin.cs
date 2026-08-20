using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MonsterHunt;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class Plugin : BaseUnityPlugin
{
    public const string PluginGuid = "rattballe.repo.monsterhunt";
    public const string PluginName = "Monster Hunt";
    public const string PluginVersion = "0.1.0";

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

    private Harmony harmony;

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

        if (!Enabled.Value)
            return;

        harmony = new Harmony(PluginGuid);
        harmony.PatchAll(Assembly.GetExecutingAssembly());
        Log.LogInfo("Monster Hunt loaded: vanilla enemies, kill rewards, higher density, no quota.");
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
        if (amount <= 0 || !IsHost()) return;
        int current = SemiFunc.StatGetRunCurrency();
        SemiFunc.StatSetRunCurrency(current + amount);
    }
}

using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;

namespace MonsterHunt;

[HarmonyPatch(typeof(PlayerAvatar), "Start")]
internal static class FreeLoadoutPatch
{
    [HarmonyPostfix]
    private static void Postfix(PlayerAvatar __instance)
    {
        if (!Plugin.Enabled.Value || !Plugin.FreeShotgun.Value || !Plugin.IsHost() || __instance == null)
            return;

        Plugin.Log.LogInfo("Monster Hunt: scheduling free shotgun spawn.");
        __instance.StartCoroutine(SpawnAfterLoad(__instance));
    }

    private static System.Collections.IEnumerator SpawnAfterLoad(PlayerAvatar player)
    {
        yield return new WaitForSeconds(2f);

        if (player == null)
            yield break;

        try
        {
            GameObject prefab = Resources.Load<GameObject>("Items/Item Gun Shotgun");
            if (prefab == null)
            {
                Plugin.Log.LogError("Monster Hunt: could not find vanilla shotgun prefab at Items/Item Gun Shotgun.");
                yield break;
            }

            Transform spawnTransform = player.transform;
            if (Camera.main != null)
                spawnTransform = Camera.main.transform;

            Vector3 position = spawnTransform.position + spawnTransform.forward * 1.5f + Vector3.down * 0.25f;
            Quaternion rotation = Quaternion.identity;

            if (!TryPhotonRoomInstantiate(prefab.name, position, rotation))
                UnityEngine.Object.Instantiate(prefab, position, rotation);

            Plugin.Log.LogInfo("Monster Hunt: free shotgun spawned.");
        }
        catch (Exception ex)
        {
            Plugin.Log.LogError($"Monster Hunt: free shotgun spawn failed: {ex}");
        }
    }

    private static bool TryPhotonRoomInstantiate(string prefabName, Vector3 position, Quaternion rotation)
    {
        try
        {
            Type photonNetwork = Type.GetType("Photon.Pun.PhotonNetwork, PhotonUnityNetworking");
            if (photonNetwork == null)
                return false;

            MethodInfo method = photonNetwork.GetMethod(
                "InstantiateRoomObject",
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { typeof(string), typeof(Vector3), typeof(Quaternion), typeof(byte), typeof(object[]) },
                null);

            if (method == null)
                return false;

            method.Invoke(null, new object[] { prefabName, position, rotation, (byte)0, null });
            return true;
        }
        catch (Exception ex)
        {
            if (Plugin.DebugLogging.Value)
                Plugin.Log.LogWarning($"Photon shotgun spawn unavailable, using local instantiate: {ex.Message}");
            return false;
        }
    }
}

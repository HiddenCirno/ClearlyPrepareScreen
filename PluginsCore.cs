using BepInEx;
using EFT;
using EFT.UI;
using EFT.UI.Matchmaker;
using EFT.UI.SessionEnd;
using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;

namespace ClearPreRaidScreen
{
    [BepInPlugin(PluginsInfo.GUID, PluginsInfo.NAME, PluginsInfo.VERSION)]
    public class PluginsCore : BaseUnityPlugin
    {
        public static EnvironmentUI CachedEnvUI = null;
        public void Awake()
        {
            var harmony = new Harmony(PluginsInfo.GUID);
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(MatchmakerFinalCountdown), nameof(MatchmakerFinalCountdown.Show), new Type[] { typeof(Profile), typeof(DateTime) })]
        internal class Patch_MatchmakerFinalCountdown_Show
        {
            [HarmonyPostfix]
            static void Postfix()
            {
                try
                {
                    var envUI = UnityEngine.Object.FindObjectOfType<EnvironmentUI>();

                    if (envUI != null && envUI.gameObject.activeSelf)
                    {
                        CachedEnvUI = envUI;
                        CachedEnvUI.gameObject.SetActive(false);
                    }

                }
                catch (Exception ex)
                {
                    Debug.LogError($"[LogoReplace] 隐藏 EnvironmentUI 时出错: {ex.Message}");
                }
            }
        }
        [HarmonyPatch(typeof(GameWorld), "OnGameStarted")]
        public class GameStartPatch
        {
            [HarmonyPostfix]
            public static void Postfix(GameWorld __instance)
            {
                try
                {
                    if (CachedEnvUI != null && CachedEnvUI.gameObject.activeSelf == false)
                    {
                        CachedEnvUI.gameObject.SetActive(true);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[LogoReplace] 恢复 EnvironmentUI 时出错: {ex.Message}");
                }
            }
        }
    }
    
}

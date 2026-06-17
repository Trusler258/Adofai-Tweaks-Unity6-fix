using System.Linq;
using System.Reflection;
using AdofaiTweaks.Compat.Async;
using AdofaiTweaks.Core.Attributes;
using HarmonyLib;
using UnityEngine;
using static RDInputType;

namespace AdofaiTweaks.Tweaks.KeyLimiter;

internal static class KeyLimiterPatches
{
    [SyncTweakSettings] private static KeyLimiterSettings Settings { get; set; }

    private static readonly PropertyInfo _scrControllerCLSModeProperty =
        AccessTools.Property(typeof(scrController), "CLSMode");

    // LEGACY: OldCountValidKeys variants for versions <=119 (no longer active on r143)
    // r120+ uses KeyboardMainWithLimit / AsyncKeyboardMainWithLimit below

    [TweakPatch("KeyLimiter.CountValidKeysPressedBeforeMultipressPatch", "scrController", "CountValidKeysPressed", MaxVersion = 71)]
    private static class CountValidKeysPressedBeforeMultipressPatch
    {
        public static bool Prefix(ref int __result, scrController __instance) {
            if (!Settings.LimitKeyOnCLS && (bool)_scrControllerCLSModeProperty.GetValue(__instance)) return true;
            if (!Settings.LimitKeyOnMainScreen && !__instance.gameworld && !(bool)_scrControllerCLSModeProperty.GetValue(__instance)) return true;
            if (Settings.IsListening) { __result = 0; return false; }
            int keysPressed = Settings.ActiveKeys.Count(Input.GetKeyDown) + KeyLimiterTweak.ALWAYS_BOUND_KEYS.Count(Input.GetKeyDown);
            __result = Mathf.Min(1, keysPressed);
            return false;
        }
    }

    [TweakPatch("KeyLimiter.CountValidKeysPressedAfterMultipressPatch", "scrController", "CountValidKeysPressed", MinVersion = 72, MaxVersion = 96)]
    private static class CountValidKeysPressedAfterMultipressPatch
    {
        public static bool Prefix(ref int __result, scrController __instance) {
            if (!Settings.LimitKeyOnCLS && (bool)_scrControllerCLSModeProperty.GetValue(__instance)) return true;
            if (!Settings.LimitKeyOnMainScreen && !__instance.gameworld && !(bool)_scrControllerCLSModeProperty.GetValue(__instance)) return true;
            if (Settings.IsListening) { __result = 0; return false; }
            int keysPressed = Settings.ActiveKeys.Count(Input.GetKeyDown) + KeyLimiterTweak.ALWAYS_BOUND_KEYS.Count(Input.GetKeyDown);
            __result = Mathf.Min(4, keysPressed);
            return false;
        }
    }

    [TweakPatch("KeyLimiter.CountValidKeysAfterAsyncInputRefactorPatch", "scrPlayer", "CountValidKeysPressed", MinVersion = 97)]
    // FIXED: Changed from scrController to scrPlayer (method moved in r120+), removed MaxVersion=119
    private static class CountValidKeysAfterAsyncInputRefactorPatch
    {
        public static bool Prefix(ref int __result, scrPlayer __instance) {
            if (!Settings.LimitKeyOnCLS && ADOBase.isCLS) return true;
            if (!Settings.LimitKeyOnMainScreen && !__instance.currFloor && !ADOBase.isCLS) return true;
            if (Settings.IsListening) { __result = 0; return false; }
            int keysPressed = 0;
            if (AsyncInputManagerCompat.IsAsyncInputEnabled) {
                keysPressed += Settings.ActiveAsyncKeys.Count(AsyncInputCompat.GetKeyDown) + AsyncInputManagerCompat.GetKeyDownCountForAlwaysBoundKeys();
            } else {
                keysPressed += Settings.ActiveKeys.Count(Input.GetKeyDown) + KeyLimiterTweak.ALWAYS_BOUND_KEYS.Count(Input.GetKeyDown);
            }
            __result = Mathf.Min(4, keysPressed);
            return false;
        }
    }

    [TweakPatch("KeyLimiter.CheckForSpecialInputKeysOrPausePatch", "scrController", "CheckForSpecialInputKeysOrPause", MaxVersion = 93)]
    private static class ControllerCheckForSpecialInputKeysOrPausePatch
    {
        public static void Postfix(ref bool __result, scrController __instance) {
            if (Settings.IsListening) return;
            if (__instance?.paused ?? true) return;
            if ((bool)_scrControllerCLSModeProperty.GetValue(__instance)) return;
            foreach (KeyCode code in Settings.ActiveKeys) {
                if (Input.GetKeyDown(code)) { __result = false; return; }
            }
        }
    }

    [TweakPatch("KeyLimiter.KeyboardMainWithLimit", "RDInputType_Keyboard", "Main", MinVersion = 120)]
    private static class KeyboardMainWithLimit
    {
        private static readonly MethodInfo GetStateCountMethod = AccessTools.Method(typeof(RDInputType_Keyboard), "GetStateCount");
        public static void Postfix(RDInputType_Keyboard __instance, ref int __result, ButtonState state) {
            if (Settings.IsListening) { __result = 0; return; }
            if (!Settings.LimitKeyOnCLS && ADOBase.isCLS) return;
            if (!Settings.LimitKeyOnMainScreen && !ADOBase.controller.gameworld && !ADOBase.isCLS) return;
            if (scrController.instance.pauseMenu.isActiveAndEnabled) return;
            MainStateCount stateCount = (MainStateCount)GetStateCountMethod.Invoke(__instance, new object[] { state });
            __result = stateCount.keys.Where(k => Settings.ActiveKeys.Contains((KeyCode)k.value)).Count();
        }
    }

    [TweakPatch("KeyLimiter.AsyncKeyboardMainWithLimit", "RDInputType_AsyncKeyboard", "Main", MinVersion = 120)]
    private static class AsyncKeyboardMainWithLimit
    {
        private static readonly MethodInfo GetStateCountMethod = AccessTools.Method(typeof(RDInputType_AsyncKeyboard), "GetStateCount");
        public static void Postfix(RDInputType_AsyncKeyboard __instance, ref int __result, ButtonState state) {
            if (Settings.IsListening) { __result = 0; return; }
            if (!Settings.LimitKeyOnCLS && ADOBase.isCLS) return;
            if (!Settings.LimitKeyOnMainScreen && !ADOBase.controller.gameworld && !ADOBase.isCLS) return;
            if (scrController.instance.pauseMenu.isActiveAndEnabled) return;
            MainStateCount stateCount = (MainStateCount)GetStateCountMethod.Invoke(__instance, new object[] { state });
            __result = stateCount.keys.Select(AsyncInputManagerCompat.ConvertAnyKeyCodeToRaw).Count(kRaw => Settings.ActiveAsyncKeys.Contains(kRaw));
        }
    }
}

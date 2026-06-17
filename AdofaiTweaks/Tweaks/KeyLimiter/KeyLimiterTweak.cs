using System;
using System.Collections.Generic;
using System.Text;
using AdofaiTweaks.Compat.Async;
using AdofaiTweaks.Core;
using AdofaiTweaks.Core.Attributes;
using AdofaiTweaks.Strings;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.KeyLimiter;

[RegisterTweak(id: "key_limiter", settingsType: typeof(KeyLimiterSettings), patchesType: typeof(KeyLimiterPatches))]
public class KeyLimiterTweak : Tweak
{
    public override string Name => TweakStrings.Get(TranslationKeys.KeyLimiter.NAME);
    public override string Description => TweakStrings.Get(TranslationKeys.KeyLimiter.DESCRIPTION);

    public static readonly ISet<KeyCode> ALWAYS_BOUND_KEYS = new HashSet<KeyCode> {
        KeyCode.Mouse0, KeyCode.Mouse1, KeyCode.Mouse2,
        KeyCode.Mouse3, KeyCode.Mouse4, KeyCode.Mouse5, KeyCode.Mouse6,
    };

    [SyncTweakSettings] private KeyLimiterSettings Settings { get; set; }

    public override void OnUpdate(float deltaTime) { UpdateRegisteredKeys(); }

    private void UpdateRegisteredKeys() {
        if (!Settings.IsListening) return;
        if (AsyncInputManagerCompat.IsAsyncInputEnabled) {
            foreach (var key in AsyncInputManagerCompat.GetKeysDownThisFrame()) {
                if (Settings.ActiveAsyncKeys.Contains(key)) Settings.ActiveAsyncKeys.Remove(key);
                else Settings.ActiveAsyncKeys.Add(key);
            }
        } else {
            foreach (KeyCode code in Enum.GetValues(typeof(KeyCode))) {
                if (!Input.GetKeyDown(code) || ALWAYS_BOUND_KEYS.Contains(code)) continue;
                if (Settings.ActiveKeys.Contains(code)) Settings.ActiveKeys.Remove(code);
                else Settings.ActiveKeys.Add(code);
            }
        }
    }

    public override void OnHideGUI() { Settings.IsListening = false; }

    public override void OnSettingsGUI() {
        AsyncInputManagerCompat.UpdateAsyncKeyCache();
        DrawKeyRegisterSettingsGUI();
    }

    private void DrawKeyRegisterSettingsGUI() {
        if (AsyncInputManagerCompat.IsAsyncAvailable) {
            GUILayout.Label(TweakStrings.Get(TranslationKeys.KeyLimiter.SELECTED_INPUT_SYSTEM,
                AsyncInputManagerCompat.IsAsyncInputEnabled ?
                    TweakStrings.Get(TranslationKeys.KeyLimiter.ASYNCHRONOUS_INPUT_SYSTEM) :
                    TweakStrings.Get(TranslationKeys.KeyLimiter.SYNCHRONOUS_INPUT_SYSTEM)));
            GUILayout.Space(12f);
        }
        GUILayout.Label(TweakStrings.Get(TranslationKeys.KeyLimiter.REGISTERED_KEYS));
        GUILayout.BeginHorizontal();
        GUILayout.Space(20f);
        GUILayout.BeginVertical();
        GUILayout.Space(8f);
        if (AsyncInputManagerCompat.IsAsyncInputEnabled) {
            foreach (var code in Settings.ActiveAsyncKeys) {
                GUILayout.Label(code + "(" + AsyncInputManagerCompat.GetLabel(code) + ")");
                GUILayout.Space(8f);
            }
        } else {
            foreach (KeyCode code in Settings.ActiveKeys) {
                GUILayout.Label(code.ToString());
                GUILayout.Space(8f);
            }
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        GUILayout.Space(12f);

        GUILayout.BeginHorizontal();
        if (Settings.IsListening) {
            if (GUILayout.Button(TweakStrings.Get(TranslationKeys.KeyLimiter.DONE))) Settings.IsListening = false;
            GUILayout.Label(TweakStrings.Get(TranslationKeys.KeyLimiter.PRESS_KEY_REGISTER));
        } else {
            if (GUILayout.Button(TweakStrings.Get(TranslationKeys.KeyLimiter.CHANGE_KEYS))) Settings.IsListening = true;
            if (GUILayout.Button(TweakStrings.Get(TranslationKeys.KeyLimiter.CLEAR_ALL_KEYS))) {
                if (AsyncInputManagerCompat.IsAsyncInputEnabled) Settings.ActiveAsyncKeys.Clear();
                else Settings.ActiveKeys.Clear();
            }
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        Settings.LimitKeyOnCLS = GUILayout.Toggle(Settings.LimitKeyOnCLS, TweakStrings.Get(TranslationKeys.KeyLimiter.LIMIT_CLS));
        Settings.LimitKeyOnMainScreen = GUILayout.Toggle(Settings.LimitKeyOnMainScreen, TweakStrings.Get(TranslationKeys.KeyLimiter.LIMIT_MAIN_MENU));
    }
}

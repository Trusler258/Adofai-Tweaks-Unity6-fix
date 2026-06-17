using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AdofaiTweaks.Core;
using AdofaiTweaks.Core.Attributes;
using AdofaiTweaks.Strings;
using AdofaiTweaks.Translation;
using HarmonyLib;
using UnityEngine;
using UnityModManagerNet;

namespace AdofaiTweaks;

public static class AdofaiTweaks
{
    public static UnityModManager.ModEntry.ModLogger Logger { get; private set; }
    public static bool IsEnabled { get; private set; }
    public static readonly int ReleaseNumber = (int)AccessTools.Field(typeof(GCNS), "releaseNumber").GetValue(null);

    [SyncTweakSettings]
    public static GlobalSettings GlobalSettings { get; set; }

    private static List<Type> allTweakTypes;
    private static readonly List<TweakRunner> tweakRunners = new List<TweakRunner>();
    private static SettingsSynchronizer synchronizer;

    internal static void Setup(UnityModManager.ModEntry modEntry) {
        allTweakTypes =
            AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.GetCustomAttribute<RegisterTweakAttribute>() != null)
                .OrderBy(t => t.Name)
                .ThenBy(t => t.GetCustomAttribute<RegisterTweakAttribute>().Priority)
                .ToList();

        Logger = modEntry.Logger;
        synchronizer = new SettingsSynchronizer();
        synchronizer.Load(modEntry);

        synchronizer.Register(typeof(TweakStrings));
        synchronizer.Register(typeof(AdofaiTweaks));
        synchronizer.Register(typeof(TweakRunner));

        modEntry.OnToggle = OnToggle;
        modEntry.OnGUI = OnGUI;
        modEntry.OnHideGUI = OnHideGUI;
        modEntry.OnSaveGUI = OnSaveGUI;
        modEntry.OnUpdate = OnUpdate;

#if DEBUG
        modEntry.HasUpdate = false;
        modEntry.Info.DisplayName += " <color=#a7a7a7><i>[Debug Build]</i></color>";
#endif
    }

    private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value) {
        IsEnabled = value;
        if (value) {
            StartTweaks();
        } else {
            StopTweaks();
            synchronizer.Save(modEntry);
        }
        return true;
    }

    private static void StartTweaks() {
        HashSet<string> tweakIds = new HashSet<string>();
        foreach (Type tweakType in allTweakTypes) {
            RegisterTweakAttribute registerAttribute = tweakType.GetCustomAttribute<RegisterTweakAttribute>();
            if (tweakIds.Contains(registerAttribute.Id)) {
                throw new InvalidOperationException("Found conflicting tweaks with the ID '{0}', ");
            }
            ConstructorInfo constructor = tweakType.GetConstructor(new Type[] { });
            Tweak tweak = (Tweak)constructor.Invoke(null);
            TweakSettings settings = synchronizer.GetSettingsForType(registerAttribute.SettingsType);
            TweakRunner runner = new TweakRunner(tweak, settings);
            tweakRunners.Add(runner);
            synchronizer.Register(runner.Tweak);
            synchronizer.Register(runner.TweakMetadata.PatchesType);
        }
        synchronizer.Sync();
        foreach (TweakRunner runner in tweakRunners) {
            runner.Start();
        }
    }

    private static void StopTweaks() {
        foreach (TweakRunner runner in tweakRunners) {
            runner.Stop();
            synchronizer.Unregister(runner.Tweak);
            synchronizer.Unregister(runner.TweakMetadata.PatchesType);
        }
        tweakRunners.Clear();
    }

    private static void OnGUI(UnityModManager.ModEntry modEntry) {
        if (GlobalSettings.Language.IsSymbolLanguage() && TweakAssets.SymbolLangNormalFont != null) {
            GUI.skin.button.font = TweakAssets.SymbolLangNormalFont;
            GUI.skin.label.font = TweakAssets.SymbolLangNormalFont;
            GUI.skin.textArea.font = TweakAssets.SymbolLangNormalFont;
            GUI.skin.textField.font = TweakAssets.SymbolLangNormalFont;
            GUI.skin.toggle.font = TweakAssets.SymbolLangNormalFont;
            GUI.skin.button.fontSize = 15;
            GUI.skin.label.fontSize = 15;
            GUI.skin.textArea.fontSize = 15;
            GUI.skin.textField.fontSize = 15;
            GUI.skin.toggle.fontSize = 15;
        }
        GUI.skin.toggle = new GUIStyle(GUI.skin.toggle) {
            margin = new RectOffset(0, 4, 6, 6),
        };
        GUI.skin.label.wordWrap = false;

        GUILayout.Space(4);

        GUILayout.BeginHorizontal();
        GUILayout.Space(4);
        GUILayout.Label(
            TweakStrings.Get(TranslationKeys.Global.GLOBAL_LANGUAGE),
            new GUIStyle(GUI.skin.label) {
                fontStyle = GlobalSettings.Language.IsSymbolLanguage() ? FontStyle.Normal : FontStyle.Bold,
                font = GlobalSettings.Language.IsSymbolLanguage() ? TweakAssets.KoreanBoldFont : null,
            });
        foreach (LanguageEnum language in Enum.GetValues(typeof(LanguageEnum))) {
            string langString = TweakStrings.GetForLanguage(TranslationKeys.Global.LANGUAGE_NAME, language);
            GUIStyle style = new GUIStyle(GUI.skin.button);
            if (language == GlobalSettings.Language) {
                if (language.IsSymbolLanguage() && TweakAssets.KoreanBoldFont != null) {
                    style.font = TweakAssets.KoreanBoldFont;
                    style.fontSize = 15;
                } else if (!language.IsSymbolLanguage()) {
                    style.fontStyle = FontStyle.Bold;
                }
            } else if (language.IsSymbolLanguage() && TweakAssets.SymbolLangNormalFont != null) {
                style.font = TweakAssets.SymbolLangNormalFont;
                style.fontSize = 15;
            }
            bool click = GUILayout.Button(langString, style);
            if (click) {
                GlobalSettings.Language = language;
                foreach (TweakRunner runner in tweakRunners) {
                    runner.OnLanguageChange();
                }
            }
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(4);
        foreach (TweakRunner runner in tweakRunners) {
            runner.OnGUI();
        }

        GUI.skin.button.font = null;
        GUI.skin.label.font = null;
        GUI.skin.textArea.font = null;
        GUI.skin.textField.font = null;
        GUI.skin.toggle.font = null;
        GUI.skin.button.fontSize = 0;
        GUI.skin.label.fontSize = 0;
        GUI.skin.textArea.fontSize = 0;
        GUI.skin.textField.fontSize = 0;
        GUI.skin.toggle.fontSize = 0;

        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        var footerStyle = new GUIStyle(GUI.skin.label) { fontSize = 10, fontStyle = FontStyle.Italic };
        footerStyle.normal.textColor = new Color(0.45f, 0.45f, 0.45f, 1f);
        GUILayout.Label(GetFooterText(), footerStyle);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

#if DEBUG
        GUILayout.Label($"<color=#a7a7a7><i>DEBUG: r{ReleaseNumber}</i></color>");
#endif
    }

    private static void OnHideGUI(UnityModManager.ModEntry modEntry) {
        foreach (TweakRunner runner in tweakRunners) {
            runner.OnHideGUI();
        }
        synchronizer.Save(modEntry);
    }

    private static void OnSaveGUI(UnityModManager.ModEntry modEntry) {
        synchronizer.Save(modEntry);
    }

    private static void OnUpdate(UnityModManager.ModEntry modEntry, float deltaTime) {
        foreach (TweakRunner runner in tweakRunners) {
            runner.OnUpdate(deltaTime);
        }
    }

    private static string GetFooterText() {
        switch (GlobalSettings.Language.ToString()) {
            case "CHINESE_SIMPLIFIED":
                return "原版模组作者 PizzaLovers007 | Unity 6 移植 @Trusler | 可能存在未知 Bug";
            case "KOREAN":
                return "원본 모드 제작자 PizzaLovers007 | Unity 6 포팅 @Trusler | 알 수 없는 버그가 있을 수 있습니다";
            case "JAPANESE":
                return "原作 PizzaLovers007 | Unity 6 移植 @Trusler | 未知のバグが存在する可能性があります";
            default:
                return "Original mod by PizzaLovers007 | Unity 6 port by @Trusler | Unknown bugs may exist";
        }
    }
}

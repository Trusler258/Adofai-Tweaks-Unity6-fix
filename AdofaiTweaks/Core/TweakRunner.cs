using System;
using System.Collections.Generic;
using System.Linq;
using AdofaiTweaks.Core.Attributes;
using AdofaiTweaks.Translation;
using HarmonyLib;
using UnityEngine;

namespace AdofaiTweaks.Core;

internal class TweakRunner
{
    internal Tweak Tweak { get; private set; }
    internal RegisterTweakAttribute TweakMetadata { get; private set; }

    private TweakSettings Settings { get; set; }
    private IList<TweakPatch> TweakPatches { get; set; } = new List<TweakPatch>();
    private IList<TweakPatch> ValidTweakPatches { get; set; } = new List<TweakPatch>();
    private readonly Harmony harmony;

    [SyncTweakSettings]
    private static GlobalSettings GlobalSettings { get; set; }

    public TweakRunner(Tweak tweak, TweakSettings settings) {
        Tweak = tweak;
        Settings = settings;
        TweakMetadata = Attribute.GetCustomAttribute(tweak.GetType(), typeof(RegisterTweakAttribute)) as RegisterTweakAttribute;
        harmony = new Harmony("adofai_tweaks." + TweakMetadata.Id);

        foreach (Type type in TweakMetadata.PatchesType.GetNestedTypes(AccessTools.all)) {
            TweakPatchAttribute attr = type.GetCustomAttributes(false).OfType<TweakPatchAttribute>()?.FirstOrDefault();
            if (attr != null) {
                TweakPatch tweakPatch = new TweakPatch(type, attr, harmony);
                TweakPatch duplicatePatch = TweakPatches.FirstOrDefault(p => p.Metadata.PatchId.Equals(attr.PatchId));
                if (duplicatePatch != null) {
                    AdofaiTweaks.Logger.Log($"Patch with the ID of '{duplicatePatch.Metadata.PatchId}' is already registered.");
                } else {
                    if (tweakPatch?.IsValidPatch(true) ?? false) {
                        ValidTweakPatches.Add(tweakPatch);
                    }
                    TweakPatches.Add(tweakPatch);
                }
            }
        }
    }

    private Type[] GetAllNestedTypes(Type type) {
        return GetAllNestedTypes(type.GetNestedTypes(AccessTools.all));
    }

    private Type[] GetAllNestedTypes(Type[] types) {
        List<Type> typeList = new List<Type>(types.ToArray());
        foreach (Type t in types) {
            typeList.Add(GetAllNestedTypes(t));
        }
        return typeList.ToArray();
    }

    private void EnableTweak() {
        Tweak.OnEnable();
        foreach (Type type in GetAllNestedTypes(TweakMetadata.PatchesType)) {
            try { harmony.CreateClassProcessor(type).Patch(); }
            catch (Exception e) { AdofaiTweaks.Logger.Log($"  [WARN] Failed to patch {type.Name}: {e.GetType().Name} - {e.Message}"); }
        }
        foreach (TweakPatch patch in ValidTweakPatches) {
            try { patch.Patch(); } catch (Exception e) { AdofaiTweaks.Logger.Log($"  [WARN] TweakPatch {patch.Metadata.PatchId} failed: {e.Message}"); }
        }
        Tweak.OnPatch();
    }

    private void DisableTweak() {
        Tweak.OnDisable();
        harmony.UnpatchAll(harmony.Id);
        foreach (TweakPatch patch in ValidTweakPatches) {
            patch.Unpatch();
        }
        Tweak.OnUnpatch();
    }

    internal void Start() {
        if (Settings.IsEnabled) EnableTweak();
    }

    internal void Stop() {
        if (Settings.IsEnabled) DisableTweak();
    }

    internal void OnGUI() {
        GUILayout.BeginHorizontal();
        bool newIsExpanded = GUILayout.Toggle(Settings.IsExpanded,
            Settings.IsEnabled ? (Settings.IsExpanded ? "\u25e2" : "\u25b6") : "",
            new GUIStyle() { fixedWidth = 10, normal = new GUIStyleState() { textColor = Color.white }, fontSize = 15, margin = new RectOffset(4, 2, 6, 6) });
        bool newIsEnabled = GUILayout.Toggle(Settings.IsEnabled, Tweak.Name,
            new GUIStyle(GUI.skin.toggle) { fontStyle = GlobalSettings.Language.IsSymbolLanguage() ? FontStyle.Normal : FontStyle.Bold,
                font = (GlobalSettings.Language.IsSymbolLanguage() && TweakAssets.KoreanBoldFont != null) ? TweakAssets.KoreanBoldFont : null, margin = new RectOffset(0, 4, 4, 4) });
        GUILayout.Label("-");
        GUILayout.Label(Tweak.Description, new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Italic });
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        if (newIsEnabled != Settings.IsEnabled) {
            Settings.IsEnabled = newIsEnabled;
            if (newIsEnabled) { EnableTweak(); newIsExpanded = true; } else { DisableTweak(); }
        }

        if (newIsExpanded != Settings.IsExpanded) {
            Settings.IsExpanded = newIsExpanded;
            if (!newIsExpanded) Tweak.OnHideGUI();
        }

        if (Settings.IsExpanded && Settings.IsEnabled) {
            GUILayout.BeginHorizontal();
            GUILayout.Space(24f);
            GUILayout.BeginVertical();
            try { Tweak.OnSettingsGUI(); } catch (Exception e) { AdofaiTweaks.Logger.Log($"  [WARN] OnSettingsGUI for {TweakMetadata.Id} failed: {e.Message}"); }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.Space(12f);
        }
    }

    internal void OnHideGUI() { if (Settings.IsEnabled) Tweak.OnHideGUI(); }
    internal void OnUpdate(float deltaTime) { if (Settings.IsEnabled) Tweak.OnUpdate(deltaTime); }
    internal void OnLanguageChange() { if (Settings.IsEnabled) Tweak.OnLanguageChange(); }
}

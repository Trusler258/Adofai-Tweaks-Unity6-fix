using System.Reflection;
using AdofaiTweaks.Core.Attributes;
using AdofaiTweaks.Utils;
using HarmonyLib;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.PlanetOpacity;

internal static class PlanetOpacityPatches
{
    [SyncTweakSettings] private static PlanetOpacitySettings Settings { get; set; }

    static PlanetOpacityPatches() {
        CanLoadPlanetRenderer = true; // r143 uses PlanetRenderer API
    }

    public static bool CanLoadPlanetRenderer { get; }

    private static float GetBodyOpacityNew(PlanetRenderer planet) {
        if (PlanetComparison.IsRedPlanet(planet)) return Settings.PlanetOpacity1.Body;
        if (PlanetComparison.IsBluePlanet(planet)) return Settings.PlanetOpacity2.Body;
        return Settings.PlanetOpacity3.Body;
    }

    private static float GetTailOpacityNew(PlanetRenderer planet) {
        if (PlanetComparison.IsRedPlanet(planet)) return Settings.PlanetOpacity1.Tail;
        if (PlanetComparison.IsBluePlanet(planet)) return Settings.PlanetOpacity2.Tail;
        return Settings.PlanetOpacity3.Tail;
    }

    private static float GetRingOpacityNew(PlanetRenderer planet) {
        if (PlanetComparison.IsRedPlanet(planet)) return Settings.PlanetOpacity1.Ring;
        if (PlanetComparison.IsBluePlanet(planet)) return Settings.PlanetOpacity2.Ring;
        return Settings.PlanetOpacity3.Ring;
    }

    private static Color ApplyOpacity(Color color, float opacity) {
        return new Color(color.r, color.g, color.b, color.a * opacity / 100f);
    }

    // Patch EnableDefaultFireAndIceColor because for default red/blue planets,
    // SetPlanetColor is never called — the body color is set directly as sprite.color = Color.white
    [TweakPatch("PlanetOpacity.DefaultFireAndIceColorPost128", "PlanetRenderer", "EnableDefaultFireAndIceColor", minVersion: 128)]
    private static class PlanetRendererEnableDefaultFireAndIceColorPatch {
        public static void Postfix(PlanetRenderer __instance) {
            if (PlanetComparison.IsFake(__instance)) return;
            float opacity = GetBodyOpacityNew(__instance);
            if (opacity >= 100f) return;
            __instance.sprite.color = ApplyOpacity(__instance.sprite.color, opacity);
        }
    }

    // Prefix modifies color BEFORE the original method runs
    [TweakPatch("PlanetOpacity.SetPlanetColorPost128", "PlanetRenderer", "SetPlanetColor", minVersion: 128)]
    private static class PlanetRendererSetPlanetColorPatch {
        public static void Prefix(PlanetRenderer __instance, ref Color color) {
            if (PlanetComparison.IsFake(__instance)) return;
            color = ApplyOpacity(color, GetBodyOpacityNew(__instance));
        }
    }

    [TweakPatch("PlanetOpacity.SetTailColorPost128", "PlanetRenderer", "SetTailColor", minVersion: 128)]
    private static class PlanetRendererSetTailColorPatch {
        public static void Prefix(PlanetRenderer __instance, ref Color color) {
            if (PlanetComparison.IsFake(__instance)) return;
            color = ApplyOpacity(color, GetTailOpacityNew(__instance));
        }
    }

    [TweakPatch("PlanetOpacity.SetRingColorPost128", "PlanetRenderer", "SetRingColor", minVersion: 128)]
    private static class PlanetRendererSetRingColorPatch {
        public static void Prefix(PlanetRenderer __instance, ref Color color) {
            if (PlanetComparison.IsFake(__instance)) return;
            color = ApplyOpacity(color, GetRingOpacityNew(__instance));
        }
    }

    [TweakPatch("PlanetOpacity.SetFaceColorPost128", "PlanetRenderer", "SetFaceColor", minVersion: 128)]
    private static class PlanetRendererSetFaceColorPatch {
        public static void Prefix(PlanetRenderer __instance, ref Color color) {
            if (PlanetComparison.IsFake(__instance)) return;
            color = ApplyOpacity(color, GetBodyOpacityNew(__instance));
        }
    }
}

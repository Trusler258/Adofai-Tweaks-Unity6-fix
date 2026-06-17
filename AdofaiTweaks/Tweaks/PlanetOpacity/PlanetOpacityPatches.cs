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

    private static float GetBodyOpacity(scrPlanet planet) {
        if (PlanetComparison.IsRedPlanetLegacy(planet))         return Settings.PlanetOpacity1.Body;
        if (PlanetComparison.IsBluePlanetLegacy(planet)) return Settings.PlanetOpacity2.Body;
        return Settings.PlanetOpacity3.Body;
    }

    private static float GetBodyOpacityNew(PlanetRenderer planet) {
        if (PlanetComparison.IsRedPlanet(planet)) return Settings.PlanetOpacity1.Body;
        if (PlanetComparison.IsBluePlanet(planet)) return Settings.PlanetOpacity2.Body;
        return Settings.PlanetOpacity3.Body;
    }

    private static Color ApplyOpacity(Color color, float opacity) {
        return new Color(color.r, color.g, color.b, color.a * opacity / 100f);
    }

    [TweakPatch("PlanetOpacity.SetPlanetColorPost128", "PlanetRenderer", "SetPlanetColor", minVersion: 128)]
    private static class PlanetRendererSetPlanetColorPatch {
        public static void Postfix(PlanetRenderer __instance, ref Color color) {
            if (PlanetComparison.IsFake(__instance)) return;
            color = ApplyOpacity(color, GetBodyOpacityNew(__instance));
        }
    }

    [TweakPatch("PlanetOpacity.SetTailColorPost128", "PlanetRenderer", "SetTailColor", minVersion: 128)]
    private static class PlanetRendererSetTailColorPatch {
        public static void Postfix(PlanetRenderer __instance, ref Color color) {
            if (PlanetComparison.IsFake(__instance)) return;
            color = ApplyOpacity(color, GetBodyOpacityNew(__instance));
        }
    }

    [TweakPatch("PlanetOpacity.SetRingColorPost128", "PlanetRenderer", "SetRingColor", minVersion: 128)]
    private static class PlanetRendererSetRingColorPatch {
        public static void Postfix(PlanetRenderer __instance, ref Color color) {
            if (PlanetComparison.IsFake(__instance)) return;
            color = ApplyOpacity(color, GetBodyOpacityNew(__instance));
        }
    }

    [TweakPatch("PlanetOpacity.SetFaceColorPost128", "PlanetRenderer", "SetFaceColor", minVersion: 128)]
    private static class PlanetRendererSetFaceColorPatch {
        public static void Postfix(PlanetRenderer __instance, ref Color color) {
            if (PlanetComparison.IsFake(__instance)) return;
            color = ApplyOpacity(color, GetBodyOpacityNew(__instance));
        }
    }
}

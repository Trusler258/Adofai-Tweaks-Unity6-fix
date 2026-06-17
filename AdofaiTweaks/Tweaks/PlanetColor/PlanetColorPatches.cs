using System.Reflection;
using AdofaiTweaks.Core.Attributes;
using AdofaiTweaks.Utils;
using HarmonyLib;
using UnityEngine;
using AdofaiPlanetColor = PlanetColor;

namespace AdofaiTweaks.Tweaks.PlanetColor;

internal static class PlanetColorPatches
{
    [SyncTweakSettings] private static PlanetColorSettings Settings { get; set; }
    private static PlanetColor Red => Settings.ColorProfiles[0];
    private static PlanetColor Blue => Settings.ColorProfiles[1];
    private static PlanetColor Green => Settings.ColorProfiles[2];

    // r128+ patches (PlanetRenderer)
    [TweakPatch("PlanetColor.SetPlanetColorPost128", "PlanetRenderer", "SetPlanetColor", minVersion: 128)]
    private static class PlanetRendererSetPlanetColorPatch {
        public static void Prefix(PlanetRenderer __instance, ref Color color) {
            if (PlanetComparison.IsFake(__instance)) return;
            if (__instance.IsRedPlanet() && Red.Enabled) color = Red.Body.SolidColor;
            else if (__instance.IsBluePlanet() && Blue.Enabled) color = Blue.Body.SolidColor;
            else if (__instance.IsGreenPlanet() && Green.Enabled) color = Green.Body.SolidColor;
        }
    }

    [TweakPatch("PlanetColor.SetTailColorPost128", "PlanetRenderer", "SetTailColor", minVersion: 128)]
    private static class PlanetRendererSetTailColorPatch {
        public static void Prefix(PlanetRenderer __instance, ref Color color) {
            if (PlanetComparison.IsFake(__instance)) return;
            if (__instance.IsRedPlanet() && Red.Enabled) color = Red.Tail.SolidColor;
            else if (__instance.IsBluePlanet() && Blue.Enabled) color = Blue.Tail.SolidColor;
            else if (__instance.IsGreenPlanet() && Green.Enabled) color = Green.Tail.SolidColor;
        }
    }

    [TweakPatch("PlanetColor.SetCoreColorPost128", "PlanetRenderer", "SetCoreColor", minVersion: 128)]
    private static class PlanetRendererSetCoreColorPatch {
        public static void Prefix(PlanetRenderer __instance, ref Color color) {
            if (PlanetComparison.IsFake(__instance)) return;
            if (__instance.IsRedPlanet() && Red.Enabled) color = Red.Body.SolidColor;
            else if (__instance.IsBluePlanet() && Blue.Enabled) color = Blue.Body.SolidColor;
            else if (__instance.IsGreenPlanet() && Green.Enabled) color = Green.Body.SolidColor;
        }
    }

    [TweakPatch("PlanetColor.SetRingColorPost128", "PlanetRenderer", "SetRingColor", minVersion: 128)]
    private static class PlanetRendererSetRingColorPatch {
        public static void Prefix(PlanetRenderer __instance, ref Color color) {
            if (PlanetComparison.IsFake(__instance)) return;
            if (__instance.IsRedPlanet() && Red.Enabled) color = Red.Body.SolidColor;
            else if (__instance.IsBluePlanet() && Blue.Enabled) color = Blue.Body.SolidColor;
            else if (__instance.IsGreenPlanet() && Green.Enabled) color = Green.Body.SolidColor;
        }
    }

    [TweakPatch("PlanetColor.SetFaceColorPost128", "PlanetRenderer", "SetFaceColor", minVersion: 128)]
    private static class PlanetRendererSetFaceColorPatch {
        public static void Prefix(PlanetRenderer __instance, ref Color color) {
            if (PlanetComparison.IsFake(__instance)) return;
            if (__instance.IsRedPlanet() && Red.Enabled) color = Red.Body.SolidColor;
            else if (__instance.IsBluePlanet() && Blue.Enabled) color = Blue.Body.SolidColor;
            else if (__instance.IsGreenPlanet() && Green.Enabled) color = Green.Body.SolidColor;
        }
    }

    [TweakPatch("PlanetColor.GetPlayerColor", "Persistence", "GetPlayerColor")]
    private static class GetTweakedPlayerColorPatch {
        public static void Postfix(bool red, ref Color __result) {
            if (red && Red.Enabled) __result = Red.Body.SolidColor;
            else if (!red && Blue.Enabled) __result = Blue.Body.SolidColor;
        }
    }

    [TweakPatch("PlanetColor.NoMultiplanet", "PlanetarySystem", "ApplyMultiplanetColors")]
    private static class DoNotColorMultiplanetPatch { public static bool Prefix() => !Green.Enabled; }
}

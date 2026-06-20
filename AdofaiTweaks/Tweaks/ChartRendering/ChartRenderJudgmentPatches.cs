using HarmonyLib;

namespace AdofaiTweaks.Tweaks.ChartRendering
{
    [HarmonyPatch(typeof(scrHitTextManager), nameof(scrHitTextManager.ShowHitText), typeof(HitMargin), typeof(scrPlanet), typeof(float))]
    internal static class ChartRenderJudgmentPatches
    {
        private static bool Prefix()
        {
            return !ChartRenderSession.IsRendering || ChartRenderMain.Settings.ChartRenderShowHitJudgments;
        }
    }
}

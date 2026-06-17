using JetBrains.Annotations;

namespace AdofaiTweaks.Utils;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class PlanetComparison {
    public static bool IsRedPlanet(this PlanetRenderer planet) => planet && planet == PlanetGetter.RedPlanet?.planetRenderer;
    public static bool IsBluePlanet(this PlanetRenderer planet) => planet && planet == PlanetGetter.BluePlanet?.planetRenderer;
    public static bool IsGreenPlanet(this PlanetRenderer planet) => planet && planet == PlanetGetter.GreenPlanet?.planetRenderer;
    public static bool IsRedPlanetLegacy(this scrPlanet planet) => planet && planet == PlanetGetter.RedPlanet;
    public static bool IsBluePlanetLegacy(this scrPlanet planet) => planet && planet == PlanetGetter.BluePlanet;
    public static bool IsGreenPlanetLegacy(this scrPlanet planet) => planet && planet == PlanetGetter.GreenPlanet;
    public static bool IsFake(PlanetRenderer planet) => !planet || (!planet.IsRedPlanet() && !planet.IsBluePlanet() && !planet.IsGreenPlanet());
    public static bool IsFakeLegacy(scrPlanet planet) => !planet || (!planet.IsRedPlanetLegacy() && !planet.IsBluePlanetLegacy() && !planet.IsGreenPlanetLegacy());
}

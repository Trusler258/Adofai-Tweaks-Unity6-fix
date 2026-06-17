using System.Reflection;
using HarmonyLib;

namespace AdofaiTweaks.Utils;

public static class PlanetGetter {
    private enum PlanetType { Red, Blue, Green }

    private static readonly FieldInfo LegacyRedPlanetField = AccessTools.Field(typeof(scrController), "redPlanet");
    private static readonly FieldInfo LegacyBluePlanetField = AccessTools.Field(typeof(scrController), "bluePlanet");
    private static readonly FieldInfo LegacyGreenPlanetField = AccessTools.Field(typeof(scrController), "greenPlanet");

    public static scrPlanet RedPlanet {
        get {
            return AdofaiTweaks.ReleaseNumber switch {
                <= 127 => scrController.instance == null ? null : (scrPlanet)LegacyRedPlanetField.GetValue(scrController.instance),
                _ => GetNewPlanet(PlanetType.Red)
            };
        }
    }

    public static scrPlanet BluePlanet {
        get {
            return AdofaiTweaks.ReleaseNumber switch {
                <= 127 => scrController.instance == null ? null : (scrPlanet)LegacyBluePlanetField.GetValue(scrController.instance),
                _ => GetNewPlanet(PlanetType.Blue)
            };
        }
    }

    public static scrPlanet GreenPlanet {
        get {
            return AdofaiTweaks.ReleaseNumber switch {
                <= 127 => scrController.instance == null ? null : (scrPlanet)LegacyGreenPlanetField?.GetValue(scrController.instance),
                _ => GetNewPlanet(PlanetType.Green)
            };
        }
    }

    private static scrPlanet GetNewPlanet(PlanetType planet) {
        return planet switch {
            PlanetType.Red => scrController.instance?.planetRed,
            PlanetType.Blue => scrController.instance?.planetBlue,
            PlanetType.Green => scrController.instance?.planetGreen,
            _ => null
        };
    }
}

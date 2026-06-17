using System.IO;
using UnityEngine;

namespace AdofaiTweaks.Core;

/// <summary>
/// Holds references to all Unity assets for AdofaiTweaks. These assets are
/// all loaded on first use of any of them.
/// </summary>
public static class TweakAssets
{
    /// <summary>
    /// The normal font to use for text for symbol languages.
    /// </summary>
    public static Font SymbolLangNormalFont { get; private set; }

    /// <summary>
    /// The bold font to use for Korean text.
    /// </summary>
    public static Font KoreanBoldFont { get; private set; }

    /// <summary>
    /// The sprite for the bottom arrow on the hit error meter.
    /// </summary>
    public static Sprite HandSprite { get; private set; }

    /// <summary>
    /// The sprite for the colored part of the hit error meter.
    /// </summary>
    public static Sprite MeterSprite { get; private set; }

    /// <summary>
    /// The sprite for the colored ticks on the hit error meter.
    /// </summary>
    public static Sprite TickSprite { get; private set; }

    /// <summary>
    /// The sprite for the key's outline in the key viewer.
    /// </summary>
    public static Sprite KeyOutlineSprite { get; private set; }

    /// <summary>
    /// The sprite for the key's background fill in the key viewer.
    /// </summary>
    public static Sprite KeyBackgroundSprite { get; private set; }

    static TweakAssets() {
        try {
            AssetBundle assets = AssetBundle.LoadFromFile(
                Path.Combine("Mods", "AdofaiTweaks", "adofai_tweaks.assets"));
            if (assets == null) {
                AdofaiTweaks.Logger?.Log("Asset bundle failed to load - creating fallback assets");
                CreateFallbackAssets();
                return;
            }
            SymbolLangNormalFont = SafeLoad<Font>(assets, "Assets/NanumGothic-Regular.ttf");
            KoreanBoldFont = SafeLoad<Font>(assets, "Assets/NanumGothic-Bold.ttf");
            HandSprite = SafeLoad<Sprite>(assets, "Assets/Hand.png");
            MeterSprite = SafeLoad<Sprite>(assets, "Assets/Meter.png");
            TickSprite = SafeLoad<Sprite>(assets, "Assets/Tick.png");
            KeyOutlineSprite = SafeLoad<Sprite>(assets, "Assets/KeyOutline.png");
            KeyBackgroundSprite = SafeLoad<Sprite>(assets, "Assets/KeyBackground.png");
            assets.Unload(false);
        } catch (System.Exception e) {
            AdofaiTweaks.Logger?.Log($"Failed to load asset bundle: {e.Message}");
            CreateFallbackAssets();
        }
    }

    private static void CreateFallbackAssets() {
        AdofaiTweaks.Logger?.Log("Creating embedded sprite assets (original asset bundle incompatible with Unity 6)");
        KeyBackgroundSprite = SpriteFromBase64(EmbeddedAssets.KeyBackgroundPng, "KeyBackground");
        KeyOutlineSprite = SpriteFromBase64(EmbeddedAssets.KeyOutlinePng, "KeyOutline");
        HandSprite = SpriteFromBase64(EmbeddedAssets.HandPng, "Hand");
        MeterSprite = SpriteFromBase64(EmbeddedAssets.MeterPng, "Meter");
        TickSprite = SpriteFromBase64(EmbeddedAssets.TickPng, "Tick");
    }

    private static Sprite SpriteFromBase64(string b64, string name, float pixelsPerUnit = 100f) {
        var bytes = System.Convert.FromBase64String(b64);
        var tex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
        // Use reflection to call ImageConversion.LoadImage since we can't reference the module at compile time
        bool loaded = false;
        try {
            var imageConversionType = System.Type.GetType("UnityEngine.ImageConversion, UnityEngine.ImageConversionModule");
            if (imageConversionType != null) {
                var loadImageMethod = imageConversionType.GetMethod("LoadImage", new[] { typeof(Texture2D), typeof(byte[]), typeof(bool) });
                if (loadImageMethod != null) {
                    loaded = (bool)loadImageMethod.Invoke(null, new object[] { tex, bytes, false });
                } else {
                    loadImageMethod = imageConversionType.GetMethod("LoadImage", new[] { typeof(Texture2D), typeof(byte[]) });
                    if (loadImageMethod != null)
                        loaded = (bool)loadImageMethod.Invoke(null, new object[] { tex, bytes });
                }
            }
        } catch { }
        if (!loaded) {
            AdofaiTweaks.Logger?.Log($"Failed to load {name} sprite");
            return null;
        }
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
    }

    /// <summary>
    /// Embedded original PNG sprites from the UnityProject/Assets folder.
    /// </summary>
    private static class EmbeddedAssets
    {
        public const string KeyBackgroundPng =
            "iVBORw0KGgoAAAANSUhEUgAAAGQAAABkCAYAAABw4pVUAAAABHNCSVQICAgIfAhkiAAAAAlwSFlzAAAO" +
            "xAAADsQBlSsOGwAAABl0RVh0U29mdHdhcmUAd3d3Lmlua3NjYXBlLm9yZ5vuPBoAAAKcSURBVHic7d3N" +
            "ahNRGMbx/5vEayh+3IPaLi1ZKYiFVr0Ol0L1AkQFQcFdr8IWRSi6qHVZUS/Cj16DSY+LyYTTaRpBmjmP" +
            "zvPb5GQygZf5M8lMNgEzO138aYeUUg8YAuuTx/PAEtBf7Gj/vDFwCPwA9oAdYD8ijua9aW6QlNJN4DFw" +
            "5YyG7LrPwIOI2D1th5lBUkp94BGwuaDBum4LuBcRv5ovnAiSUhoA28CtFgbrsjfA7YgY5Rt7M3Z8gWO0" +
            "YQ141tx47AyZfGe8bWsiA+BGRLyrn0yDTK6mDoCrJabqsK/Acn31lX9kDXGMEi4D1+oneZCN9mexiemx" +
            "z4OsFhjEKsN6kQe5VGAQq1yoF/mX+gj/HFLKKCLOwfEzxDHKGdSLWTeGVpCDiHEQMQ4ixkHEOIgYBxHj" +
            "IGIcRIyDiHEQMQ4ixkHEOIgYBxHjIGIcRIyDiHEQMQ4ixkHEOIgYBxHjIGIcRIyDiHEQMQ4ixkHEOIgY" +
            "BxHjIGIcRIyDiHEQMQ4ixkHEOIgYBxHjIGIcRIyDiHEQMQ4ixkHEOIgYBxHjIGIcRIyDiHEQMQ4ixkHE" +
            "OIgYBxHjIGIcRIyDiHEQMQ4ixkHEOIgYBxHjIGIcRIyDiHEQMQ4ixkHEOIgYBxGTBxkXm8Kmf3KfBzks" +
            "MIhVftaLPMj3AoNYZXrs8yD7BQaxyod6kQfZLjCIVabHPv+36AAOgOUSE3XYF2AlIo4gO0MiIgEPS03V" +
            "YffrGNC4D4mIXeBl6yN11/OIeJ9viOYeKaUB8ApYa2uqjnoN3ImIUb7xxJ36ZIcN4GlLg3XRFnC3GQNm" +
            "nCG5lNJ14AmwsqDBuuYTsNn8mMrNDQKQUuoBq8A6MAQuAktA/4yG/F+NqX79+AbsATvAx8nFk5n9ld8o" +
            "OF5jSorg1wAAAABJRU5ErkJggg==";

        public const string KeyOutlinePng =
            "iVBORw0KGgoAAAANSUhEUgAAAGQAAABkCAYAAABw4pVUAAAABHNCSVQICAgIfAhkiAAAAAlwSFlzAAAO" +
            "xAAADsQBlSsOGwAAABl0RVh0U29mdHdhcmUAd3d3Lmlua3NjYXBlLm9yZ5vuPBoAAAOOSURBVHic7d1N" +
            "a1RXHMfx3/9GMKJCzEaNO+tGt4VqoXWtFLvwAd9Bu3Fj7dNKowWjbrupb0ChCILQUl+IumlBY8EEQtw0" +
            "iaVJvl3MBK6HexMX8/Bn5vfZ3XvOHU7Ol5nJvZsJ7QDYL+mMpHOSTkg6LOmgpImdrh1zG5IWJb2R9FzS" +
            "E0l/RMQ/210UbQPAtKQfJV2RtKd36xxra5J+lnQnIt42TWgMAlyW9Iukqf6tbawtS/o6Ih6VA1X9AAjg" +
            "lqSHcox+mpb0KzALtH5KCbhJs5fAbeAzYAbw98cOgInuXn0OzAGvWvZ2tu0FLgObxeQ14Btg92D/nNED" +
            "7O7u5Vqxx5vAhXLyAWC5mLgAfDKk9Y8s4GR3b+uWgKn6pHsN74yTQ1z3SAM+BlaKPZ/bGtwPrBaDV4e8" +
            "5pEHfFvs+QqwT8ClYuClvzP6D5gE5ou9v1Cpcwde9yAi/h3GIsdJRLyT9KA4fa5S53FI3e+DWZJJ+q04" +
            "PlFJOlKc/HNAizHpr+J4JoB1vf+gcFdEbAxwUWML2CXpv9qp9QCoT4qI9lt567ly/6u2iTYcDpKMgyTj" +
            "IMk4SDIOkoyDJOMgyThIMg6SjIMk4yDJOEgyDpKMgyTjIMk4SDIOkoyDJOMgyThIMg6SjIMk4yDJOEgy" +
            "DpKMgyTjIMk4SDIOkoyDJOMgyThIMg6SjIMk4yDJOEgyDpKMgyTjIMk4SDIOkoyDJOMgyThIMg6SjIMk" +
            "4yDJOEgyDpKMgyTjIMk4SDIOkoyDJOMgyThIMg6SjIMk4yDJOEgyDpKMgyTjIMk4SDIOkoyDJOMgyThI" +
            "Mg6SjIMkU0l670ckgYmWudZj3R+WrFuvJC0WJw8OaD0mHSqOFypJb4qTHw1oMSYdLY4XKknPi5NnB7QY" +
            "k74ojp8JuFj8yPor/8B9/wF7gNfF3p8XsA9YLQauDXvBow74odjzFWDv1uDdYnANODXkNY8s4FPgXbHn" +
            "t+sTDgDLxYQFR+k94BSwWOz1EjBVTrwEbDa8U74DJoe0/pEBTALfN7wzNoHzbRfN0mweuAOcBmbwzeOO" +
            "gInuXp3u7t18y97eqF8XxYuEpBuSrpdj1nNImpX0U0SwdbJx04GLku5Lmh7I0sbPkqSvIuJxOdD4cDEi" +
            "Hqlzx35X0mp/1zZWViXNSTrWFEP6gI8lOv8bn5H0paTjko6o87zL3yPb21DnOeHfkl5IeiLpaUSsbHfR" +
            "/7LD9XFRXeWTAAAAAElFTkSuQmCC";

        public const string HandPng =
            "iVBORw0KGgoAAAANSUhEUgAAAB4AAACMCAYAAABrnhnqAAAABHNCSVQICAgIfAhkiAAAAAlwSFlzAAAO" +
            "xAAADsQBlSsOGwAAABl0RVh0U29mdHdhcmUAd3d3Lmlua3NjYXBlLm9yZ5vuPBoAAAEtSURBVHic7dIx" +
            "TgJRFIXh91QqQyiMLMCCUjdBQkFB4hooaNgDW7AzNsYNmBhjoYWdjTsgUTtjY2csEMlvMyjg8AADhuL/" +
            "kikmc+85LzMTgrROgAPgKrv2/6u0Bgz4MQBqqy6tAz1+6wH1VZU2gX5O6VAfaC67tJMonNRZRuEmcLxA" +
            "6dApUPhr6TZwmQi/Ba4Tz2+A4qKlO8BdIvQMKABbwEli7h4oz1u6B3QTYUdAHJmPpP+BJ6Ayq7QMvEwJ" +
            "+ARaid1WNpPnGdhNFbenLL4DjTneViObzdMend2Y2P3IyXsNIVRjjBezirOZarYzKS/7+8Qlxr/vw8zv" +
            "k59TAR5HcrpAaeyQOUvFEMJhdnseY3xbtHiZOZIkSZIkSZIkSZIkSZIkSZIkSZIkSWvrC8RbbHSVihWz" +
            "AAAAAElFTkSuQmCC";

        public const string MeterPng =
            "iVBORw0KGgoAAAANSUhEUgAAAZAAAADICAYAAADGFbfiAAAABHNCSVQICAgIfAhkiAAAAAlwSFlzAAAO" +
            "xAAADsQBlSsOGwAAABl0RVh0U29mdHdhcmUAd3d3Lmlua3NjYXBlLm9yZ5vuPBoAACAASURBVHic7d13" +
            "fBzVtQfw3+xs31XvkmXJki1L7t3GhWrAQGJwqIEUqgkmJCF5PEoeKfCSECAxL4Ek2JCQkGCH8IhNsbEB" +
            "A65yl5tcVKxm9a5t2t3ZeX9c7cMY2dLuzk5Zne/nMx/Zsnb2elc7Z+69554LEEIIIYQQQgghhBBCCCGE" +
            "EEIIIYQQQgghhBBCCCGEEEIIIYQQQgghhBBCCCGEEEIIIYQQQgghhBBCCCGEEEIIIYQQRXFKN4AQBfEA" +
            "DANfMfDnsz8ThoGvvrO+J571d//AIUSxjYSoFgUQEkssA4cZgGngz6aBv5sB6MGCQvCQ6vc/GFSChx+A" +
            "55yjf+Cre+AgRPMogBAt0QGIGzhs5xxWfN6TUDsBgAuA85yjb+AIKNc0QoaPAghRIw5A/MCRcNaf7Yj9" +
            "31kRgANAD1gw6QHQO3CICraLkC+J9Q8j0QYLgCQAKQBSASSCDTeRzwXAgkk7gK6Bow8UVIiCKIAQufEA" +
            "kgGkgQWLFFCwCJcfLKAEj07QhD6REQUQEm08WKBIH/iaDDaXQaQXAAsibWcdNJ9CooYCCIkGG4CMsw7D" +
            "hX+cRIkA1jNpBdACNuxFiGQogBAp6MCGpHIAZIIFEKI+TgDNAM6AeidEAhRASLh4sN7FKADZoF6G1njB" +
            "eiaNYAHFr2xziBZRACGh4MGCxWiw4KGVdRfkwgSwnkkdgCbQRDwZJgogZCgcWKZUHoBcUE8j1vnAeiUN" +
            "YMGE0oTJeVEAIecTD2AMWNCwKNwWogw3WK+kBmwhIyFfQAGEnE0HNkRVADZERUhQF4BqALWgIS4ygAII" +
            "AVhtqXywHodJ2aYQlfMBqAdQBaBb4bYQhVEAGbk4sMV94wBkKdwWok3tACrAsrhormQEogAy8ujBsqjG" +
            "gc1zEBIpB4DTYL0S3xA/S2IIBZCRwwIWNApAmVQkOnxg8yQVoD1PRgQKILHPCqAILHDQug0ihwDYPEk5" +
            "WO+ExCgKILHLBqAYbGKc3meihGAgOQ5Wep7EGLqwxJ44ABPBSozQ+0vUQMTnPRIKJDGEymrHDguAmQCu" +
            "Blv8R8EjAitXrpzpdDpfdjqdL69cuXKm0u3ROA4sceNqABeBim3GDLrIaJ8RwHiwCXKa45CI0+n8k9Vq" +
            "TQYAl8vVabPZvqN0m2JIAGx1+zEAHmWbQiJBO8FpFw8WOMaD3kfJBYPHuX8mktCBJXXkAjgJ4BRodbsm" +
            "0YVHm7IBTAMNBRBtMwCYBKAQwBGwMilEQyiAaEsSWOBIVbohWpA8KzEueVJcoj3fFm8dbU40p5riTYn6" +
            "eN6mt+otOqvOxFt5k87Cm3RW6MDpjLwVADgdjOee65v9X/lLwCd6RBF+0RfwiIIoCB7BGRBEv+ARnH5n" +
            "oM/f53d4e/2O/g6vw9Pk6ek56ehs393d2X2izyX//15TLADmABgLoAxAh7LNIcNFAUQbzACmgJVUJwMs" +
            "qUZDzlcyMlNnJ2Xbx1ozLemmdFOyMdUQr08zxOnTOZ77UiAIl86os+mMwR5faFNNoiD2+5z+Dn+f0Onp" +
            "8La4Gz3NjhpXc9fhvpaGD1qbnaedNA/AJAO4DKwncgQ0P6J6FEDULw+s1yHZxVBreAuvy1uWlZlxWWp+" +
            "4gR7vm2UJc+YbMw22Pg0cOrPJOR4zmSMN2Qb4w3Z1hzzJEz5YgUZn8Pf5u3w1TnPuOt7TznrO0o76yr+" +
            "0dAgOISROC/AgRX2zAGbZK8E1dlSLcrCUq8EsLTcFKUbIrfcr2am5S7LLEqeHD/ONto61pRsyOP0nKxV" +
            "gl/D+i/8/U5cL+fTQxREb3+Hr8bZ4K7qOdpbdeajjqrqN+oaIYy4i2kXgP0DX4nKUABRHx5sBXkxRsI6" +
            "HR7cuLvzRud+NXNi4sS4idYs8zjeoktUullKB5DBBLyBPtcZz4nuckf5mQ9bT1Ssqj0tuIWA0u2SgQjW" +
            "EzkK2rtdVSiAqEsagNmI8eyqvBsyM8Z8c9S05CmJk2y55gk6ky5O6TadS40B5FwBv+h2n/GUdxzsOXT6" +
            "n42HatY2NCndpihzAtgLoE3phhCGAog68AAmgK3piLn3xJJhNJT8oHBC1uK0afFFtunGeEO20m0aihYC" +
            "yLl8Dn+bo9p1qPnTjgOHn6k47GnyeJVuU5RUAzgE6o0oLuYuVhqUAtbrUN1deCTMWWbjlEfHTc66Ku2i" +
            "+LG22ToDp6l91bUYQM4mCqLX1eA50rana/+x31bvbS/t7FG6TRJzAtgHoFXphoxkFECUwwOYDJb7HhPv" +
            "g22MzTztZ0WzMxelXGQbbZ3K8drdd0TrAeQLRAiuBvfRpq0d2w8/dXJP7ylnrOzVIYLtPXIUtJJdETFx" +
            "4dKgeADzwDKtNM0Yb+Sn/mzc1Jxr0xfGF9pny50tFS0xFUDOIgrwOU67DjRubt1+6KkTB9wt3ljYQbAX" +
            "QCmAWOtlqR4FEPnlgaXnarrw4dhv544a/90xVyRNjl/Em3QxtzVurAaQswnegKP7cO+2yldqt5x4uVbr" +
            "ZUQEsMWHFUo3ZCShACIfA1jgyFW6IeGyjTKbpj8zYV72FWmLLZmm8Uq3J5pGQgA5W3+7t6rls44t+58o" +
            "367xIa4zYHMjsZpAoCoUQOSRBmAuWM0fzcm7ITNj8uPjrk6annCZzqDTToqxiIDgFrp8Tn+X4Ar0+By+" +
            "Xm+Xv9vb6+vz9QluX4/P5e3wOt0dPrfoDQiuepcTALzOgL9xc8sfzz5V0pT4u+IKbDYdB86aa7XxZh1v" +
            "SDSYjQm82ZxishsS9XZDnMGuj9PHG+P0CXq7Pklv45N5iy6R4zjNfM4CftHdfbj3s2O/qfqg+o2GRqXb" +
            "EyYXgN0A2pVuSKzTzC+2hhUAmA4NLgqc/vT4yQV35C6x5VtmqfUiKAqi19vjb+rv8DZ6mj2NfTXu5t5K" +
            "R1vXwd62ls86O7293rAmV0VRfPPsv3Mcd0s45+HtPJ8+NykhdW5SetKEuEzbaEuGOd2UaUw2ZhqTDNk6" +
            "vWqz00RXvftwzVuNGw88duKg4BW0tgJeBJtcP6F0Q2KZKi8KMUIPYBY0NmTF23n+ot9NuSj3hszrjUkG" +
            "NRVvFH0OodXT2l/jrHXXdh3prWn6qK22fkNzezTKe8iyoRQPLvcrmWmZl6bkJk2Iz7XlWfIsWaYCQ7wh" +
            "Eyr6bHq7fWfOvN+yvvSho9u9XV6trb2oBxvS0lq7NUE1v6QxJg5s607NZFlZMoyGeS9NvTTr6vSlBjuf" +
            "oXR7At6A093cX9FX7aro2NtVUfl6fUX3kT6nXM+/cuXKmcuXL18OAKtWrVr18MMP75fruROL46xj7sgu" +
            "TJmTVBA/1l5kyzEXq2G1vuASOpo+6Xiv9KEjH2usgnAfgJ1g2VpEQhRApJcDtreBJiodm7PMxgWvTL0q" +
            "67K0pUrWoAp4A05ng6e8+1jf0YZ3mo+d+ktt/QgsHDg4HtzYb+aOGvXVzJKkSXEl1tGWSXozr9jNieAN" +
            "OFq3db5Xev+hjb1Vmplw9wHYA0Cr8zqqRAFEWuMATIUGXldLqtEwb/W0K7KvSlumt/JJsjdARMDd1l/R" +
            "faSvrH5988FTL9ee1uA4uzJ4cOPvz8/PvT5zStKk+GmWTNN4TsfJfsMS6A/0NX3S/s6u+8s+cNZ5+uV+" +
            "/jCVg5WJJxJQ/YVOI3RgKbr5CrdjSLyd5xf+efplo67NuFFv42UtFR/wi25HtfNAy7bOfSd+X3Oo81C3" +
            "Q87nj1W2MTbz5EfHTsu8NGV2XIF1htyZcn6P0NP8Udu6nXeVbXa3a2JhYh3YvAitXo8QBZDIGQHMB0vV" +
            "VbU5L0yeWfjtUd8wJhpy5HpOwRtw9FU49zV91L77yK9OHo6Rlc+qZYw38hMfLZww6tr0eYklcfPknDvx" +
            "O/yt1Wsb39j1nbJdGhh+7ACwA4BWek6qRAEkMvEAFkLl5ddLvps/ZtLjRd+yZpsnyvF8ogCfq8F9+Mym" +
            "1q37Hzu+V4OZOzGBt/C6CQ8XFI25NefihJK4BXIVtPS0eauOv1j9+uGnTpXL8XwRcADYDjbJTsJAASR8" +
            "yWDBQ7W1nxYsSE54+sVJN782KeESOWpUeZr7TzRuaf/k0M9PlGp8NXPMMWeZjTOfKZmbvTjtcmu2eQKi" +
            "/dkXIdx6tPfT3z9a/r8bN7aqeUGfF6wnouY2qhYFkPBkgxVDVGU9K7ud51/58/TLll6f8XWjURf360ZP" +
            "+alsy4RoPFfAG+jrLOvZevzF2o+qX687E43nINLKuzE7Y8IPCi5PnpFwabQSKDKbPcd/kWkqEQTRu317" +
            "1/rbb927rkW9w5cCgF0AYn1DLslRAAldPtgCQVW+dj9+sqjk4YfH3JOYZBwd/J5HEPsf6BddOgkvFp42" +
            "b1XDxpYNB350rFQjE6fkHLyd5+f8dtKc0V/NvMacaSqW6ryiL+B8VhTFVKPOHvxeb5+/afXLta89+kj5" +
            "QameR2IigANgm1WRYVLlRVDFisH28FCd4uI462uvT7915sz4JYOVHVnb6j35YXpkBRDFgCj0VThLK16t" +
            "23D0uUqqehpDxq/IH1O8In9JQkn8Qk4X2T4uM5o8xx/MMpUM9m+1te79y+8pW/3xx+2dkTxHFFGabwgo" +
            "gAzfNLB1Hqqz8oWJM++5L/9eq1V33rRcERDv7/I3CEmGkEuriP6Ap/NAz0f7nzj+XpN6P/hEAhmLkhNn" +
            "PjPxupTZCVfqDDprqI8X+/ytL9v4FIPu/MO7Pl/AuX5dyz/u+Pq+jwV1ZmudAtsylwyBAsjwTAfbOVBV" +
            "Fi1KTnz1ten3FRRYZw/n5/d2+Rv+lGTIwTDf94A30Ne6s+uDvT889kHnwW7KVBlB4otsljm/n3xV5qLU" +
            "60KpUHBze3/VklRj4XB+tqHBffj73z26av36ZjVuS1sNNqSlxgCnGhRALowDm+/IV7gdX/KHP02e9607" +
            "R99nDjHP/0fN/Se7M80XHMoKeAN9LZ92vLvjO4c/0FjNIyIxc5bZuGDVlCszF6fdMFT5FHO7t+qlVMOw" +
            "gkeQIIj9Gze0rblx2e6NKuyNnAawHxREzosCyPlxAGaD7SCoGsXFcdY3/jnjG1OmxC8O5/Gt/YHex3id" +
            "gRukjHjAL7o79nRv3rm8bF33MfkKFxL1s40ym+b+YeoV2YvTbhi0RyJC+A+Hr60kTp8Zzvnr69xld99Z" +
            "9sdPPmnvirix0qoH21uEgsggKIAMTge2AdQopRtytp8/PX7ywz8sfNBq5ZMjOc9zjd7yE9mm/0/rFQX4" +
            "2vd0bdh5f9k6OSveEu2xjbGZF7467SsZi5KWcnqdOfj97Kb+8qezjBGlinv6hZ5XVtWt+sH3ju6NvKWS" +
            "oiByHhRAvowDq6Y7eqgflIvdzvPr35v3tYsvTr6R4yLfmMojwLeiP9DLWfnk3kpn6YFHjr1Ru665RYq2" +
            "kpEheVZi3Pw/TrkpZWbiVQGf4PktODHJyNmHfuTQjh93bL3qil2rmpo8atqWtgFAKSiIfIEqF8IpTFVF" +
            "Ea+5Jj31w4/nPzZxUtzFHCdNwNfrwHdXOvf9768qXt56+4ENPScc1OsgIXE3erynVteWOetde5YW2MyX" +
            "5pglW0eSlmbMu/Ou3Om1Ne5jx471qaXgZjwAK2ix4RdQD+SLZgAIaRIwmla+MHHm/Q+MedAo0Z0dALjd" +
            "Qtdb/2r6+713H9yuwklLolErX5g48657Rt9lt+vTpTqn3y+6//H3+lfuuevQNqnOKYFKAGpdDCk7CiCf" +
            "mwIgooV2UuF5cO9tmLd08ZWpt0u1F7koQti/v2fzt+7Yv/YU1akiUZCVZTb+Y+2MZQsWpCzl+cgWI56t" +
            "7GDPpiuvKP1rl3qKclYAKFO6EWpAAYSZAECWSrVDmTo1zr7unbnfyx1tmSbVOTs7vad/9tOTf/rDizWn" +
            "pTonIedz003Zmb99YeLy7BzzJKnO2dLcf/K2W/b9Ztu2zm6pzhmhowCOK90IpdEcCDAGbJW54pY/kJf/" +
            "xpqZP0lLM0kyjOb3Bzwb3m/7+8L521/etbNLbemRJEaVl/c5fvc/1VvHjLG2FpfElfB85JWg7XZ96i23" +
            "5iz0+QOndu7o6pCinRFKB+AGoJaApoiR3gPJArAAKngdfvfipNnL78//nl6isusNDe7DD604+vK77za3" +
            "SXE+QsJxySWpSav/POXuggLbXCnOFwjAt3bNmVXf+saBz6Q4X4REsCq+I7YKteIXTgUlA7gEgOx7SZ9r" +
            "w8Z51195tTTzHX6/2P/eO81/v/WWfZtpkpyoxat/mbro9jtG3W2QaLvdHTs6/335JTvWquB3XADwGdgO" +
            "hyPOSA0gdgCXQ+HNoOx2nv9024K7pk1LuEqK87W29lf88PtHX1y7tpFSDYnqXH11esrqV6euyM4xS1LR" +
            "uqrSVXrFZTteamjwKL0tbT+ATzACdzYciQHEBOAKKLwNbVGRzfLRlvmPSDHRKIoQPvu0462vXrf73263" +
            "EJCifYREA8+De3vdnGuvuTbjdl2EZeMBoL3NW3XD0r3PlJZ29kjRvgg4AHwMtsPhiDHSJtF1ABYBGHZ1" +
            "0WhYtCg5cdNHFz2ZnhHZ/hwA4HIFOp5+6sSzy+89tNXvF5XuzmuVD+wu0g3ABcAzcPQBcIJdFILf84EN" +
            "WwAj7/MTMVEE1q45U9HW7j248OKUSaYQi4Gey2rjk2+5LXtedZXroMKLDo0AUgDUYQStVh9pPZBZYFlX" +
            "irnttuysP62e+mMpFlzVnHbtu/nG/X88SKXWz8cDFgCChxufBwIPWGCIdDdFA1iv1gTAPHBYwHq4wcN8" +
            "3kePYIWFNss778+5b/x4+8JIz+X2CD2PPlL+SxWkqleDVfAdEUZSACkCMFXJBnz/4YLCX/yq5HGzSRcf" +
            "yXlEEYGPPmxb85VrS99RwSSiGngAdAHoPedQy8IzPVgpjOCRANYLpsACYO2bsxZ/7casuyId0vL5RPev" +
            "f1X57M9+ekLpHQXLwBYbxryR0gXPAivNrljAfPTxceP/+xcl/2U6a5/ocHi9gb7nn6t6/p67yraO0AEr" +
            "ASzjpQ7sQ3oIbAvSOgAtYHn5bgBqmgsK4PM1Ay1gbT0FoAZAO9j4OcACSsTFMrXmrX81Vre1ew9efEnK" +
            "NKMx/CwtnucMCxelLEhJNdZ/sLG1Uco2higDQCc+f19j1kjogcSBTZpLVlohVD9+sqjkyZ8UPaYfZA+O" +
            "UHS0e0/fe/eh50fY2o4A2EW2ZeBrJ9QVHKSkA0svTwW7CKViBAWUWbMS495eN/sHkWZpBQKif/Wquv95" +
            "8IHDu6VqWxh8AD5CjAeRWA8gerB03QvupBZNzzw7YdrDPyz4D57njJGcp6LCuevyS3a+pLIS19HiAtAI" +
            "oBlAG9QzFCU3PYA0AJkAssGqwcY0u53nt3w2/9szZiQuieQ8ogjhr6/Vv3jv3WU7pGpbGHrAMrOEoX5Q" +
            "q2J9CGs22J2cIp6/dOKMu58a/Q1rfEQbQImffNz25vx52/7S2+uP2V9EsDmLarAhqcNgwcOB2O1tDEcA" +
            "7DVoBhuuawLLFgtO2sccr1cUV6+qOzh1Wnzv+PFxU8Pd/4bjoCscYysoOWbrXXeqpU7qdg6TGSzox+xK" +
            "9VgOIGMBSLZHQah+saBk8n9ML/xPw37O1DdFaDXbQp84FwT4Xv9b3R9uunH/BzE63+EGmwc4CFacrhVs" +
            "QpwMzgP2GlWBbXDkBbtARdS7VaM3/9lYZTbzlXPnJs7ieS7k4WeXQ+jU/4bTzbImXZwXb21aX91cH412" +
            "DkMiPk/yiDmxGkCSAcyDQkN0T8weV/xfs8c9rtdxRt7PGfh94BzThU6Tdfg5757+QN+PHz/x348/ejzW" +
            "9h4QwCaRywaOZlDQCEc/2PBeJVhQ4cDm+2JmzmTLx+0tZ8549l+xOHWW0agb9vCds09o1z8rwtarT+EA" +
            "blpawpwcm+XMu6dbGqLZ3gvIRIz+nsdiADGC1bhS5K7sP2cVFj19UfETeh33/ymavJ8z8fsA53R/p8nC" +
            "DxlEHA5/23fuO/zz1atqaqPbWln1gJW/3gO2x7RL2ebEFBfYMEkVWK/OihhJES472Nu7e1f3zqVLM6eY" +
            "LfyQC4DdTqFT9xwLHsHvcQA3PT1+dpLJULOptk2JMj8c2FB6DWJsSDYWA8hssOwV2a2YnD/m2UUTf2LQ" +
            "fTnbivdxJt1eDs5p/k6T9fxBpLvLV3/rzfueWr++uTW6rZWFCHZ3fAistxHLGVRqIIC9xlVgr7sBrFei" +
            "6WSZmhqXZ+OGlp3LlmUV2+368362XQ6hi38esPV8HjyCOI7Tzc5MmqvjcPLThg4lshiNYDX4lOoFRUWs" +
            "BZAxAEqUeOIbCjMzXl489Scm/vzDVLyPM+n26eCaJnQZrV9eD9Lc5Dl+6cW7nt6zp7s3uq2NOgFsQnwP" +
            "2OQvrZSXnwvsYlUPFkASoOHhrdZWr2/NG2d23HhTdl5ioiH73H93OfwdumchDhY8gnQc+AXZKXM73N5D" +
            "e1u7ldjHIx7sfYmZPURiKYDYwfb2kP1DMicjIf7t6+b81G7g04b6WRZEOPHcINLU5Cm/eMHOZyorHVre" +
            "btYPdvdbCnbhGgkpx2rnBcveqgYL7InQ6Ofe4fALr/25ofSmm7NHJScbRgW/73YKnfzznGjrOX/vJIjn" +
            "OMPluamzDnf07T7V5VBiGDUDbLgxJj4bmu7ankUHtt4jSe4nLky0WUpvWfizVIsxpBpbXqvY5/6R4LSn" +
            "6DPr69wH58/b/hsNr/Hwg62srkCMfDBimBGsrM84qGAvnHBYLLxu975FD0yYEHfJwLCVYO3mQxq27vP6" +
            "m69Zv/vJHY2KVPHtBCv/rvnh3FgJIFMARFzZNlR2I88f/+blj42ym8OqsdVvC/SeXOrYd+k1u17p6vJq" +
            "cbFcAKzHcRwsK4hohxlsuLcAGhzaMhp5buuWBXdO2mybc6Fhqwtpc3krp77x2c+anIrcuJ0AcESB55WU" +
            "5n5xBpECdkclu603Lbgz3OABAC3N/VVXXLbrVY0GjzoAG8Emxyl4aI8HbP3NB2DvpaZ4vYK4YOHW16qq" +
            "nGXhniPNahy79eb5Dxl5Xokb6fFgyw00TZNjoWcJ7u8he8riu0vnXHdJTsqN4T6+weE5NHvN1ufa3N5I" +
            "y4nLrQtsjqMCkZdCJ8rzgY3JN4NNtGumXIoI4K/lDQdvGpeVmWoxjg7nHMlm46hLRiXrXiuvl7uCLwdW" +
            "puY0NLx/iNYDyGQAOXI/6XOLJkz/9oTcFVyYQ4BnnJ4jc9Zue7bZ1a+lC7AHwAGwu1ZawxF7glUBnGBp" +
            "8JqYH/GLovjq0fq9N4/Lzk6xGHPDOUdenLWkIMHWsq6qWe6emAnsGqLZlH0tB5BksA2iZO1+3jkxN/eX" +
            "F5U8wXPhFUdsdvafnLtm268alRl3DVctgB1gk38ktnWDZWzxYJ8x1c+T+kVR/MepM/u+XpRTkGAyZIVx" +
            "Cm5SavwMjyAc3dHY1SF5Ay8sBaz3p8nsS60GEEWGrgoTbZa3r5v9X1ZDeMURu/t99Yv/vfO/K7pdWvll" +
            "6QULHJWI4Yqi5EsCYBe1VrALnOoLN3r8QuCfpxr33DF+1ES7ceh03nPpOPALc1Jm7Gzs2l7T65Kz5AgH" +
            "9hprcihLqwFkIoBRQ/6UhHiAK71t0feybeawFiq6/IGOWzfuf2p7Y5cWFhGJAE6CzXXQcNXI5QIb1tKB" +
            "XeRU3Rtx+PzClvqOfbcW5cwy60MvXmrQceZrxqQXvVpev83jF+RMsQ3eCGtunx8tBhA7gLmQ+Zf5w6/N" +
            "u3FWeuLV4TzWGwj0Lf+47Of/W9ncInW7oiDY66iFBu+IiOREsM28msHmRlTdG2l2ebwHWrr3fW1s9jwj" +
            "P/wCjEE2gz71spxU0yvH6g5Fo30XkILPKyxrhhYDyEVgQUQ2v5pfMvWbJbn3hzNpHhDh+0npyV++dKjm" +
            "dDTaJrFKALtAvQ7yZcFJdiNUnn5a3etyt7q9R67JT1/Ec6GXgs+xm4tybJYGmav3Bqspa6qAqtYCyGjI" +
            "vGBwQXZywkuXT/nxYAUSh0F8/XjDH3+07dgByRsmrX4Au8FSc6nXQc5HBCuL0g1Woly1148DrT09iSZ9" +
            "7bys5AXh3PhNTYubfrzLuau8s0/OLWntYHXjNFMLT7W/AIMwAFgIGdMLjTzPbbtp/iMpZmNeOI/f2tjx" +
            "9vXv7t0gdbsk1g5gK2J0wxsSFX1giw+ToeJ1I5vr2ppnZSS6i5Ls00J9rI7j9JePSi16+Uj9p/2CIOdN" +
            "VXBCXRNlTrQUQKYCSJfzCTctm3vjtLSEy8N5bEW3c9ecN7a+qvLb+VNgPQ8trUch6uAHCyIGsIueKq05" +
            "eabihsLMhEybuTDUx1oNfPKCnCRO5kWGBrDrcrOMzxk2rQSQeLB9PmSbOH9i9rjieybmrQhnT+bOfl/t" +
            "RWu3/brH51driRI/WKn1U0o3hGiaCHah6wMb0lJlaaQ1J5sO3TkhtyTOqA/5BnR0nLXYGxCPbG/slHN9" +
            "SDJYdQDVlwhS5Rs+iKmQMXiMjjebHps1dgXHhR5gvYGAc/mHh59vcHrU+uY7AWwBK7dOiBTqwarLqjL5" +
            "otfrFb62Yd8LLn8g5CDAAbonZo99qCjRFs4caLg4sCobqqeFHkgm2LoP2Wy7ecF9uXGWkN9AURTF35Wd" +
            "/s0LZdWV0WiXBDrA5jucSjeExBwP2JBWClQ4L9LQ5+7vF8RTi0enXRzqjaGR19kWj05Lfulwzd5otW8Q" +
            "cWCVH+ScxA+Z2gMIB7ZJlGy55ysvnjjz+oLMb4bz2E8bOt+8Y9OBLVK3SSJ1AHaC5jtI9AhgvRE7WGFG" +
            "VdnV1Nk5PT3eWZwUNyPUx6ZZjPl58dbG9dXNcvbcE8HKyqiW2oewCsDmP2QxPS0x7v4p+feH89gGh/vw" +
            "kvW735a6TRKpAJvz0ERmB9E0AayCQbnSDRnMsnf3bTrZ5dgRzmPvKM659+r8dDkTBhLAtulWLTUHEANk" +
            "Hrpas2TGnRZelxjq4/r9Qs83NpW95JU33W84wMnynQAACqhJREFURLD9OspA6zuIvI4BkHs197Dc8P6+" +
            "1Q6fP+SyIUadzrbqiilh3WBGYCJUXBlZzUNYxQDCqawZlucWTZi+tCDjjlAfJ4qi+NyB6uf/fKxObStI" +
            "A2C9Di2sgCexqQNsvi0bKqqj1e72+jhw1Zfnpl4S6iLDBKMhM8tuqn/vdKtcq9T1YL26dpmeLyRq7YEY" +
            "wPZslkVhos3ywOT8+8J57I6mrnVP7Dx+WOo2RSgANoxAmVZEacGtAFQ1fPrLvRUndjZ2rg/nsd8qGX33" +
            "xJQ4m9RtuoDxYCVkVEetAaQYMr5gb1478zabgQ+5BHS723v6uvV734xGmyIQAKtndUbphhAyoAksiKhq" +
            "S4Al60vfbHP3V4T6OAuvS/zb1dNviUabzsMAhbbtHooaA4gJwFi5nuy70/LHTE9LCLnKbkCE7/GdJ17q" +
            "9XrV9KEQAGwD0Kh0Qwg5RzNYFqBqPi8OryCs+OTo7/2BQMj7f0xPjV/yyIyxso2SgI3IqK4SshoDSDFk" +
            "mjTiAe7J2ePv4cJ4HT6obVnzytFaubfAvJBgz0Oz22OSmBcMIqoZznqrorF5XXXz66E+juM47vE5Y5fb" +
            "jbxc88h6yFxIdjjUFkAsAEKuWROuv18z89J0qzHkrmGzs//ksvf2vx+NNoUpGDyalG4IIUNoBqu/ppqs" +
            "wJvf3/9hbZ97f6iPSzIZ8tYsmbkkGm06j7GQeRfWoagtgBRBpsywySlxtmWFmSFnXQVE+B7bcfxlFaXs" +
            "imDZVjRsRbSiAYCcq7qHdP+Ww6+EM5S1JC/9litzU+VaG8JDZXMhagogRrCFg7JY3e26qXN/9Rl/QAxp" +
            "f/It9e1v/fV4vZwbzQzlECjbimhPLQDVZC9uqmnt2FzXFlJCTEAU/W3VzTW/beuRc0K9ECrKyFLTOpDx" +
            "YHWvou5GIOMHovhgfEN7hre83tWeYj9tS7Cmchx3wZzwrn5f7fw3d74k8/4AF3ISwHGlG0FImDqgonLw" +
            "/65srnxgSt5Mi55PGupn29p6q3Rv7UTykdq8dH8g3wsc2c7+P9GmA6umrYp1IWoJIDyAeZBp8vx9YEUy" +
            "kAsAeq/fbD9xJq23ob3Rk5vWYzYZBq3hI4qi+OSuU8992tCmijcOrNeh9p0OCRlKK1jhQMVrZ/lFUfQF" +
            "xNNX5aVffr4Fhn0eb2vvpoMtaZ8dLTR4fMGikdxkIHcl8IlMd5YJYNtPK34jq5YAUghglBxP9BNgwjLg" +
            "S3Mf5j53vPlgdXyTs/8EPyqFN/C6L5RvPtrh2PKtzQc2y9HGYeiCChdnERKmJgAZYEk0iipt7uq6vjAz" +
            "Ictm/sJSAq8QcLXtq6xKfmdPrq2j70t7wtuAlAyg/j02vxNterDqx50yPNcFqWUORLZ1Hw8Ct1/gn7ms" +
            "o7UlptWbbU1H68oFkVWu9QYCzns/OrhGpiYOxQVgO1SUT09IhASw9N6Q5iOj5bYN+99w+YVOABBFCM21" +
            "rcfx6odC5q4TxVxAPO8183bg6/Hy3ZTLds28ELX0QBrA7qbjEcVhrBeB2ZcBS4f6OV1A1MedbknznGzs" +
            "68lKavywtefdlQer1VBd1A+2n4eq9wggJAx+AG0A8qDwjW2Hx+fPj7N25Bn5zMC/d/WnHKgew/uFIRfx" +
            "mYC4sUDnv6Jbgr0frLr2AbDXTFGqKXA2QAc2N1EMicu4WwBdI/Bc4sDcx3B1ArWjgEfd6hgu2gOWvUJI" +
            "rBoNYK7SjQCAM8BPs0OsCO4COscC32sCvBI3xwE271ENFY0+qGUIKygAdoHcDHanLdnCuLGApQ1oEEOc" +
            "ePoD8DeVBI9KUPAgsa8OQJXSjQCAXwB/C/F6IbYA1UWAlIUW28EWCX8A1vNQTfAA1NcDGUwi2OKZXEgQ" +
            "8O4Ecn8M3DSWZX1d8P9fA+wfA/w60ueUQCfYntNqCGSERJsOwKVQQXrvUWDFRNaWC2oAjqwE1vyW3ehF" +
            "SgRbsX8c8qQGh00LASTIAlZQrAAsdzwiDwEFPwJuzQOmD/bvIhD4NvCj15WvausF8CHY5DkhI4UNwJWQ" +
            "4LMeiSuA5E3A//DnKWTYBJS/BPzzF9Ksx/KBDVFVQCUJBUPRUgAJ0oONkxaB5Y9H5BFg3IPA1/KAmWd/" +
            "/wiwZQrwp0jPLwHa14OMVKMAXKR0I3YAt88Hbjj7e21AxRvA2z8AQq6hNQgnWOCoAgsimqHFABLEge1Y" +
            "OBYshzwijwLjHwJuzQEmCYDvq8D3Nyq/2vM0gH0Kt4EQJc0By8xSzETAdgD4vRGwdwH1/wLeWgGUCpEv" +
            "5OsC623UQQWLAsOh5QBythSwHkkOIvw//RKYUgJkLQM2SdKy8DnAhq4UT9UjREF6sKEsu5KN+AdwWS/g" +
            "+i6wJ8LAIYINi5+Cyuc3hiNWAkiQDWyOpBAKj51GSATwKZTvARGiBikALoO2r1d+sJ7GKQB9CrdFMmpZ" +
            "SCgVH1htnUqwBTfx0GYgCeZ7E0LYhLIJwJdKiGiAByxolIItmJZ6fYiitBzRhyM4T1IMFaQEDpMTbB0M" +
            "DV0R8jkewFVQeCgrBN1ggaMeMZx+H2s9kMH0gU1Gt4HV0bdD3YFzF2Koi0uIRESwz4WiE+pDEMEWP+8H" +
            "cARADzQ6OT5cIyGABLnA7gaC+5gnQH0r8YNjpISQL3OCDUsrXvr9HAGwz+5usOHnEbNmS8134tFmAJAP" +
            "tpGV4mWkweZvNkEjC4gIUYgZwBKoY27TAzZXGZxzHXFGUg/kXAGwEiGVYHc2dii7Yf1hsAQAQsj5+cHq" +
            "Qcmye+l59IANUe0D+8yqqj6VnEZyD2QwqWAT7lkyP28v2MR5TI+XEiIRDmxCXdKK3cPQDraNdKPMz6ta" +
            "I7kHMhgX2FhmcJ4kEfLMk+wB7fFBSChcYCWNoi04v7EHwAlQgssXUAAZnBesGmb1wJ+juZ6kGYAaNqsi" +
            "REscYCMG0Urr7Qcb3t4Nto3CiJzjGAoNYQ0PD5Y+KEkBx7OIYOVKeiQ8JyEjRQJYmRMpr2N9YJmQtRjB" +
            "cxvDRT2Q4RHBCp9VgtWvMUKaQFIHWnFOSLj6wT6HUqT1tgM4BOAg2Ged5iOHIWr7j8ewloEjEazmVh7C" +
            "C8QipNlDgJCR7BjYZnPh9EICYBPiJ8EyMkmIqAcSPg/YqtPTYF3dBIT2etYMHISQ8HkBWAEkhfCY4MZN" +
            "u8E+v7T2Kkw0ByIdPdjCxHEYemIvAGAjRtCKVUKiyAa2uHCojEkH2P4bNaBac5KgHoh0ggsTq8DGUE04" +
            "fyCpHTgIIZHzgc2FJJ7n37vAFv7tB/uMxmxxQ7nRHIj0RLBx1UawbvU4sHz1s3t7JxVoFyGx7AS++DkT" +
            "wVLkjyMGNm5SK+qBRJcHbPexanz+T9ICls1FCJFOP9gNmwVsFKB04CvNb5CYYQCb8COESM8KdRRZJIQQ" +
            "QgghhBBCCCGEEEIIIYQQQgghhBBCCCGEEEIIIYQQQgghhBBCCCGEEEIIIYQQQgghhBBCCCGEEEIIIYQQ" +
            "QgghhBBCCCGEEEIIIYQQQgghhBBCCCGEEEIIIYQQQgghhBBCCCGEEEIIIYQQQgghhBBCCCGEEEIIIYQQ" +
            "QgghhBBCCCGEEEIIIYQQQgghhBBCCCGEEEIIIYQQQgghhBBCCCGEEEIIIYQQQgghhBBCCCGEEEIIIYQQ" +
            "tfk/nYSCD+CCEUgAAAAASUVORK5CYII=";

        public const string TickPng =
            "iVBORw0KGgoAAAANSUhEUgAAAAgAAAC2CAYAAADkz8yOAAAABHNCSVQICAgIfAhkiAAAAAlwSFlzAAAO" +
            "xAAADsQBlSsOGwAAABl0RVh0U29mdHdhcmUAd3d3Lmlua3NjYXBlLm9yZ5vuPBoAAABiSURBVFiF7dGx" +
            "EYAgEETRxRIMaM0e6MyuiA1sYU1QcETHxMT5L+K4vbngpM+F/WF7lDSVcg4hrGqbtrOrXAaOQPJVkqSh" +
            "ZGJnfWwDtwicA0unV/+ejvXu3AAAAAAAAADwTxv7nU53+BjHRAAAAABJRU5ErkJggg==";
    }

    private static T SafeLoad<T>(AssetBundle assets, string path) where T : Object {
        try { return assets.LoadAsset<T>(path); }
        catch { return null; }
    }
}
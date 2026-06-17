using System.Collections.Generic;
using System.Xml.Serialization;
using AdofaiTweaks.Core;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.KeyLimiter;

public class KeyLimiterSettings : TweakSettings
{
    public List<KeyCode> ActiveKeys { get; set; } = new();
    public List<ushort> ActiveAsyncKeys { get; set; } = new();
    public bool ShowKeyViewer { get; set; }
    public bool ViewerOnlyGameplay { get; set; }
    public bool AnimateKeys { get; set; } = true;
    public float KeyViewerSize { get; set; } = 100f;
    public float KeyViewerXPos { get; set; } = 0.89f;
    public float KeyViewerYPos { get; set; } = 0.03f;
    public bool LimitKeyOnCLS { get; set; } = true;
    public bool LimitKeyOnMainScreen { get; set; } = true;

    [XmlIgnore] public bool IsListening { get; set; }
    [XmlIgnore] public Color PressedOutlineColor { get; set; } = Color.black;
    [XmlIgnore] public Color ReleasedOutlineColor { get; set; } = Color.black;
    [XmlIgnore] public Color PressedBackgroundColor { get; set; } = Color.black;
    [XmlIgnore] public Color ReleasedBackgroundColor { get; set; } = Color.black;
    [XmlIgnore] public Color PressedTextColor { get; set; } = Color.black;
    [XmlIgnore] public Color ReleasedTextColor { get; set; } = Color.black;
}

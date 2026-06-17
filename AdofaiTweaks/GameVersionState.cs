namespace AdofaiTweaks;

public static class GameVersionState {
    static GameVersionState() {
        OldAsyncInputAvailable = AdofaiTweaks.ReleaseNumber == 97;
        AsyncInputAvailable = AdofaiTweaks.ReleaseNumber >= 98;
    }

    public static readonly bool OldAsyncInputAvailable;
    public static readonly bool AsyncInputAvailable;
}

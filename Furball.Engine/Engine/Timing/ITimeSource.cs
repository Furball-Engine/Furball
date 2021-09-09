namespace Furball.Engine.Engine.Timing {
    /// <summary>
    /// ITimeSource is currently used only by Drawables for Tween Timing,
    /// This is Seperated out because There may be many Time Sources available,
    /// For Example we right now have Game Time, and when Audio becomes a Thing Audio Time
    /// Which makes it good to seperate it out instead of just having 2 Time Channels
    /// Like in pEngine where we only have AudioController.Time and pEngineGame.Time
    /// </summary>
    public interface ITimeSource {
        int GetCurrentTime();
    }
}

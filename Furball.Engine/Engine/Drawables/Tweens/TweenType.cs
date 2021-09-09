namespace Furball.Engine.Engine.Drawables.Tweens {
    /// <summary>
    /// Tween Type, used to distingluish when for example using a VectorTween,
    /// it can be used for both Movement and Scale and this is how its differenciated
    /// </summary>
    public enum TweenType {
        Movement,
        Fade,
        Color,
        Scale,
        Rotation,
    }
}

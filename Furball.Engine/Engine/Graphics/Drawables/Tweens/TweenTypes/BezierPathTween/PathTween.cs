using Microsoft.Xna.Framework;

namespace Furball.Engine.Engine.Graphics.Drawables.Tweens.TweenTypes.BezierPathTween {
    public class PathTween : Tween {
        private readonly Path _path;

        public PathTween(Path path, int startTime, int endTime, Easing easing = Easing.None) {
            this.TweenType = TweenType.Path;

            this._path     = path;
            this.StartTime = startTime;
            this.EndTime   = endTime;
            this.Easing    = easing;
        }

        public Vector2 GetCurrent() {
            if (!this.Initiated)
                return this._path.SegmentFromProgress(0).GetPosition(0.0, this._path.PathSegments.Length);
            if (this.Terminated)
                return this._path.SegmentFromProgress(1.0).GetPosition(1.0, this._path.PathSegments.Length);

            return this._path.GetCurrent(this.CalculateCurrent(0.0, 1.0));
        }
    }
}

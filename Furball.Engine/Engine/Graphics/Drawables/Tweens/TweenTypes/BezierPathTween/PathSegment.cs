using Furball.Engine.Engine.Helpers;
using Microsoft.Xna.Framework;

namespace Furball.Engine.Engine.Graphics.Drawables.Tweens.TweenTypes.BezierPathTween {
    public class PathSegment {
        public BezierCurveType CurveType;

        internal int       Index;
        internal PathRange PathProgressRange;

        public PathSegment(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4) {
            this.CurveType = BezierCurveType.Cubic;
        }

        internal double GetLocalProgress(double progress, int totalSegments) {
            double segmentLength = 1.0 / totalSegments;
            double passed = segmentLength * this.Index;
            return (progress - passed) * totalSegments;
        }

        internal bool IsBetween(double progress) => this.PathProgressRange.begin >= progress && this.PathProgressRange.end <= progress;
    }
}

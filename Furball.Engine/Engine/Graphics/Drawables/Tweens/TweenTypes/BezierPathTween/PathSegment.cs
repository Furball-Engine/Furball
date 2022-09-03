using System.Numerics;
using Furball.Engine.Engine.Helpers;

namespace Furball.Engine.Engine.Graphics.Drawables.Tweens.TweenTypes.BezierPathTween; 

public class PathSegment {
    public BezierCurveType CurveType;

    internal int       Index;
    internal PathRange PathProgressRange;

    private Vector2 _point1;
    private Vector2 _point2;
    private Vector2 _point3;
    private Vector2 _point4;

    public PathSegment(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4) {
        this.CurveType = BezierCurveType.Cubic;

        this._point1 = p1;
        this._point2 = p2;
        this._point3 = p3;
        this._point4 = p4;
    }

    public PathSegment(Vector2 p1, Vector2 p2, Vector2 p3) {
        this.CurveType = BezierCurveType.Quadratic;

        this._point1 = p1;
        this._point2 = p2;
        this._point3 = p3;
    }

    internal double GetLocalProgress(double progress, int totalSegments) {
        double segmentLength = 1.0 / totalSegments;
        double passed        = segmentLength * this.Index;
        return (progress - passed) * totalSegments;
    }

    public Vector2 GetPosition(double progress, int totalSegments) {
        float localProgress = (float)this.GetLocalProgress(progress, totalSegments);

        switch (this.CurveType) {
            case BezierCurveType.Cubic: {
                Vector2 bezier = BezierHelper.CubicBezier(this._point1, this._point2, this._point3, this._point4, localProgress);

                return new Vector2(bezier.X, bezier.Y);
            }
            case BezierCurveType.Quadratic: {
                Vector2 bezier = BezierHelper.QuadraticBezier(this._point1, this._point2, this._point3, localProgress);

                return new Vector2(bezier.X, bezier.Y);
            }
        }

        return Vector2.Zero;
    }

    internal bool IsBetween(double progress) => this.PathProgressRange.Begin <= progress && this.PathProgressRange.End >= progress;
}
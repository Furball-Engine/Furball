using System;
using System.Collections.Generic;
using System.Numerics;
using Vector2=Microsoft.Xna.Framework.Vector2;

namespace Furball.Engine.Engine.Graphics.Drawables.Tweens.TweenTypes.BezierPathTween {
    internal record PathRange(double begin, double end);

    public class Path {
        private PathSegment[]   _path;
        private List<PathRange> _pathRanges;
        private double          _segmentLength;

        public Path(params PathSegment[] segments) {
            this._path = segments;

            int segmentCount = this._path.Length;
            double segmentLength = 1.0 / segmentCount;

            List<PathRange> ranges = new();

            double current = 0.0;

            for (int i = 0; i != segmentCount; i++) {
                PathSegment currentSegment = this._path[i];
                currentSegment.Index = i;

                double begin = current;
                double end = current + segmentLength;

                PathRange assignedRange = new PathRange(begin, end);

                ranges.Add(assignedRange);
                currentSegment.PathProgressRange = assignedRange;

                current = end;
            }

            this._pathRanges    = ranges;
            this._segmentLength = segmentLength;
        }

        public PathSegment SegmentFromProgress(double progress) {
            if (progress > 1.0 || progress < 0.0)
                throw new InvalidOperationException("SegmentFromProgress requires `progress` to be between 0 and 1..");

            for (int i = 0; i != this._path.Length; i++) {
                PathSegment current = this._path[i];

                if (current.IsBetween(progress))
                    return current;
            }

            return null;
        }

        public Vector2 GetCurrent(double progress) {
            PathSegment segment = this.SegmentFromProgress(progress);
            return segment.GetPosition(progress, this._path.Length);
        }
    }
}

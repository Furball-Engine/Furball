using System;
using System.Collections.Generic;
using System.Numerics;
using Furball.Vixie.Helpers;
using JetBrains.Annotations;

namespace Furball.Engine.Engine.Graphics.Drawables.Tweens.TweenTypes.BezierPathTween; 

internal record PathRange(double Begin, double End);

public class Path {
    public readonly PathSegment[] PathSegments;

    // ReSharper disable twice NotAccessedField.Local
    private List<PathRange> _pathRanges;
    private double          _segmentLength;

    public Path(params PathSegment[] segments) {
        this.PathSegments = segments;

        int    segmentCount  = this.PathSegments.Length;
        double segmentLength = 1.0 / segmentCount;

        List<PathRange> ranges = new();

        double current = 0.0;

        for (int i = 0; i != segmentCount; i++) {
            PathSegment currentSegment = this.PathSegments[i];
            currentSegment.Index = i;

            double begin = current;
            double end   = current + segmentLength;

            PathRange assignedRange = new(begin, end);

            ranges.Add(assignedRange);
            currentSegment.PathProgressRange = assignedRange;

            current = end;
        }

        this._pathRanges    = ranges;
        this._segmentLength = segmentLength;
    }

    [CanBeNull]
    public PathSegment SegmentFromProgress(double progress) {
        if (progress > 1.0 || progress < 0.0)
            throw new InvalidOperationException("SegmentFromProgress requires `progress` to be between 0 and 1..");

        for (int i = 0; i != this.PathSegments.Length; i++) {
            PathSegment current = this.PathSegments[i];

            if (current.IsBetween(progress))
                return current;
        }

        return null;
    }

    public Vector2 GetCurrent(double progress) {
        PathSegment segment = this.SegmentFromProgress(progress);
        
        Guard.EnsureNonNull(segment, "segment");
        
        return segment.GetPosition(progress, this.PathSegments.Length);
    }
}
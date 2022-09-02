using System;

namespace Furball.Engine.Engine.Input; 

public struct ScrollWheel {
    public bool Equals(ScrollWheel other) => this.X.Equals(other.X) && this.Y.Equals(other.Y);
    public override bool Equals(object obj) => obj is ScrollWheel other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(this.X, this.Y);

    /// <summary>
    /// The X value of the scroll wheel
    /// </summary>
    public float X;
    /// <summary>
    /// The Y value of the scroll wheel
    /// </summary>
    public float Y;
}

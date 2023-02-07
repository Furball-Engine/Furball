using System.Numerics;
using System.Runtime.CompilerServices;
using Furball.Engine.Engine.Graphics.Drawables;
using Silk.NET.Input;

namespace Furball.Engine.Engine.Input;

public class InputObject {
    public Vector2 Position = Vector2.Zero;
    public Vector2 Size     = Vector2.Zero;

    public Drawable Drawable;
    
    //agressive opt + inlining
    [MethodImpl((MethodImplOptions)768)]
    public bool Contains(Vector2 point) {
        return point.X >= this.Position.X && point.X <= this.Position.X + this.Size.X && point.Y >= this.Position.Y && point.Y <= this.Position.Y + this.Size.Y;
    }
    
    public readonly int Index = 0;
    public InputObject(int index) => this.Index = index;

    internal bool[] LastClicked = new bool[(int)(MouseButton.Button12 + 1)];
    internal bool   LastHovered     = false;
}
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Silk.NET.Input;

namespace Furball.Engine.Engine.Input; 

public class FurballMouse {
    internal struct DragState {
        public bool        Active;
        public MouseButton Button;
        public Vector2     StartPosition;
    }

    public   Vector2     Position;
    public   string      Name;
    public   ScrollWheel ScrollWheel;
    internal Vector2     TempPosition;
    internal Vector2     PositionCache;
    internal ScrollWheel TempScrollWheel;
    internal ScrollWheel ScrollWheelCache;

    public List<MouseButton> PressedButtons = new();

    internal List<MouseButton> QueuedButtonPresses = new();
    internal List<MouseButton> QueuedButtonReleases = new();

    internal DragState[] DragStates = new DragState[12];

    internal Vector2 LastKnownSoftwareCursorPosition;
    public   bool    SoftwareCursor;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsButtonPressed(MouseButton button) => this.PressedButtons.Contains(button);
}
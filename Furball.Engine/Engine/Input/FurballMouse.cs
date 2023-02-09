using System.Collections.Generic;
using System.Numerics;
using Silk.NET.Input;

namespace Furball.Engine.Engine.Input;

public class FurballMouse {
    internal class DragState {
        public bool         Active;
        public Vector2      StartPosition;
        public Vector2      LastPosition;
        public FurballMouse Mouse;
    }

    public FurballMouse() {
        for (int i = 0; i < this.DragStates.Length; i++) {
            this.DragStates[i] = new DragState();
        }
    }
    
    public   Vector2     Position;
    public   string      Name;
    public   ScrollWheel ScrollWheel;
    internal Vector2     TempPosition;
    internal ScrollWheel TempScrollWheel;

    public bool[] PressedButtons = new bool[(int)(MouseButton.Button12 + 1)];

    internal List<MouseButton> QueuedButtonPresses  = new();
    internal List<MouseButton> QueuedButtonReleases = new();

    internal DragState[] DragStates = new DragState[(int)(MouseButton.Button12 + 1)];

    internal Vector2 LastKnownSoftwareCursorPosition;
    public   bool    SoftwareCursor;
}

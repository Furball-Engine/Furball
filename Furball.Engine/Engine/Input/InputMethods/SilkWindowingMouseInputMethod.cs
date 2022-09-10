using System;
using System.Collections.Generic;
using System.Numerics;
using Furball.Vixie.Helpers.Helpers;
using Furball.Vixie.Input;
using Silk.NET.Input;

namespace Furball.Engine.Engine.Input.InputMethods;

public class SilkWindowingMouseInputMethod : InputMethod {
    private IReadOnlyList<IMouse> _silkMice;
    private List<Vector2>         _silkRawLastState = new();
    public override void Update() {
        foreach (FurballMouse mouse in this.Mice) {
            mouse.Position = mouse.TempPosition;
            mouse.ScrollWheel = mouse.TempScrollWheel;
        }
    }

    public override void Dispose() {
        this.Mice.Clear();
    }

    public override void Initialize() {
        this._silkMice = Mouse.GetMice();

        for (int i = 0; i < this._silkMice.Count; i++) {
            int j = i;

            IMouse mouse = this._silkMice[i];

            mouse.Scroll    += (_, wheel) => this.MouseOnScroll(wheel, j);
            mouse.MouseDown += (_, button) => this.MouseOnDown(button, j);
            mouse.MouseUp   += (_, button) => this.MouseOnUp(button, j);
            mouse.MouseMove += (_, vector2) => this.MouseOnMove(vector2, j);
            
            this.Mice.Add(new FurballMouse {
                Name        = mouse.Name,
                ScrollWheel = new ScrollWheel()
            });
            
            this._silkRawLastState.Add(Vector2.Zero);
        }
    }

    public bool SetRawInputStatus(bool value) {
        CursorMode newMode = value ? CursorMode.Raw : CursorMode.Normal;
        for (int i = 0; i < this._silkMice.Count; i++) {
            IMouse mouse = this._silkMice[i];
            if (mouse.Cursor.IsSupported(newMode)) {
                mouse.Cursor.CursorMode     = newMode;
                this.Mice[i].SoftwareCursor = value;
            }
            else
                return false;
        }

        return true;
    }

    private void MouseOnMove(Vector2 vector2, int i) {
        FurballMouse mouse     = this.Mice[i];
        IMouse       silkMouse = this._silkMice[i];

        if (silkMouse.Cursor.CursorMode == CursorMode.Raw) {
            Vector2 diff = (vector2 - this._silkRawLastState[i]) / FurballGame.VerticalRatio;

            mouse.TempPosition += diff;

            this._silkRawLastState[i] = vector2;
        } else {
            //Make sure its always scaled down to the right coordinate system
            mouse.TempPosition = vector2 / FurballGame.VerticalRatio;
        }
        
        mouse.TempPosition.X = mouse.TempPosition.X.Clamp(0, FurballGame.WindowWidth);
        mouse.TempPosition.Y = mouse.TempPosition.Y.Clamp(0, FurballGame.WindowHeight);
        
        this.Mice[i] = mouse;
    }
    private void MouseOnDown(MouseButton button, int i) {
        FurballMouse mouse = this.Mice[i];

        mouse.PressedButtons.Add(button);
        mouse.QueuedButtonPresses.Add(button);

        this.Mice[i] = mouse;
    }

    private void MouseOnUp(MouseButton button, int i) {
        FurballMouse mouse = this.Mice[i];

        mouse.PressedButtons.Remove(button);
        mouse.QueuedButtonReleases.Add(button);

        this.Mice[i] = mouse;
    }

    private void MouseOnScroll(Silk.NET.Input.ScrollWheel scrollWheel, int i) {
        FurballMouse mouse = this.Mice[i];

        mouse.TempScrollWheel.X += scrollWheel.X * 4;
        mouse.TempScrollWheel.Y += scrollWheel.Y * 4;

        this.Mice[i] = mouse;
    }
}

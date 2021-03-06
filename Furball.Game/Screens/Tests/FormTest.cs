using System.Drawing;
using System.Numerics;
using Furball.Engine;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.UiElements;
using Silk.NET.Input;
using Color=Furball.Vixie.Backends.Shared.Color;

namespace Furball.Game.Screens.Tests;

public class FormTest : TestScreen {
    public override void Initialize() {
        base.Initialize();

        this.Manager.Add(
        new DrawableButton(new(10), FurballGame.DEFAULT_FONT, 26, "Create Form", Color.White, Color.Black, Color.Black, new Vector2(300, 50), this.OnButtonClick)
        );
    }

    private int _formCount;
    private void OnButtonClick(object sender, (MouseButton, Point) e) {
        this.Manager.Add(
        new FurballForm(
        $"Test Form {++this._formCount}",
        new BlankDrawable {
            OverrideSize = new(200, 200)
        }
        )
        );
    }
}

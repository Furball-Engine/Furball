using System.Numerics;
using Furball.Engine;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.UiElements;
using Furball.Engine.Engine.Input.Events;
using Furball.Vixie.Backends.Shared;

namespace Furball.Game.Screens.Tests;

public class FormTest : TestScreen {
    public override void Initialize() {
        base.Initialize();

        this.Manager.Add(
        new DrawableButton(new(10), FurballGame.DefaultFont, 26, "Create Form", Color.White, Color.Black, Color.Black, new Vector2(300, 50), this.OnButtonClick)
        );
    }

    private int _formCount;
    private void OnButtonClick(object sender, MouseButtonEventArgs mouseButtonEventArgs) {
        this.Manager.Add(
        new DrawableForm(
        $"Test Form {++this._formCount}",
        new BlankDrawable {
            OverrideSize = new(200, 200)
        }
        )
        );
    }
}

using System.Drawing;
using System.Numerics;
using Furball.Engine;
using Furball.Engine.Engine;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.UiElements;
using Furball.Game.Screens;
using Silk.NET.Input;
using Color=Furball.Vixie.Backends.Shared.Color;

namespace Furball.Game; 

public class TestScreen : Screen {
    private TexturedDrawable _background;
        
    public override void Initialize() {
        base.Initialize();

        this.Manager.Add(this._background = new TexturedDrawable(FurballGame.WhitePixel, Vector2.Zero) {
            ColorOverride = Color.BlueViolet,
            Depth         = 2
        });

        this.Manager.Add(
        new DrawableButton(
        new(10, FurballGame.DEFAULT_WINDOW_HEIGHT - 10),
        FurballGame.DEFAULT_FONT,
        24,
        "Back",
        Color.Red,
        Color.White,
        Color.Black,
        Vector2.Zero,
        delegate {
            ScreenManager.ChangeScreen(new ScreenSelector());
        }
        ) {
            OriginType = OriginType.BottomLeft
        }
        );
    }

    public override void Relayout(float newWidth, float newHeight) {
        base.Relayout(newWidth, newHeight);

        this._background.Scale = new Vector2(newWidth, newHeight);
    }
}
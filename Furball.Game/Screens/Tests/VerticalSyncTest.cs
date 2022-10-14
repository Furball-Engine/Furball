using System.Numerics;
using Furball.Engine;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.UiElements;
using Furball.Vixie.Backends.Shared;

namespace Furball.Game;

public class VerticalSyncTest : TestScreen {
    private TextDrawable focusStatusText;
    private TextDrawable vsyncStatusText;

    public override void Initialize() {
        base.Initialize();
        this.focusStatusText = new TextDrawable(Vector2.Zero, FurballGame.DefaultFont, "", 24);
        this.vsyncStatusText = new TextDrawable(new Vector2(0, 32), FurballGame.DefaultFont, "", 24);

        this.Manager.Add(new DrawableButton(
            new Vector2(200, 0),
            FurballGame.DefaultFont,
            24,
            "Enable Vertical Sync",
            Color.Lime,
            Color.Black,
            Color.Black,
            new Vector2(250, 28),
            delegate {
                FurballGame.Instance.WindowManager.VSync = true;
            }));

        this.Manager.Add(new DrawableButton(
            new Vector2(200, 32),
            FurballGame.DefaultFont,
            24,
            "Disable Vertical Sync",
            Color.Red,
            Color.Black,
            Color.Black,
            new Vector2(250, 28),
            delegate {
                FurballGame.Instance.WindowManager.VSync = false;
            }));

        this.Manager.Add(focusStatusText);
        this.Manager.Add(vsyncStatusText);
    }

    public override void Update(double gameTime) {
        base.Update(gameTime);

        this.focusStatusText.Text = $"Focused: {FurballGame.Instance.WindowManager.Focused}";
        this.vsyncStatusText.Text = $"Vertical Sync: {FurballGame.Instance.WindowManager.VSync}";
    }
}
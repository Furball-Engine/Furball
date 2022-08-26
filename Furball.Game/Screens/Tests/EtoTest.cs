using System.Drawing;
using System.Numerics;
using Eto.Forms;
using Furball.Engine;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.UiElements;
using Furball.Engine.Engine.Helpers;
using Silk.NET.Input;
using Color=Furball.Vixie.Backends.Shared.Color;

namespace Furball.Game.Screens.Tests; 

public class EtoTest : TestScreen {
    private DrawableButton _messageDialogButton;
    private DrawableButton _colorPickerButton;
    private TextDrawable   _dialogResult;
    public override void Initialize() {
        base.Initialize();

        this.Manager.Add(
        this._dialogResult = new TextDrawable(new Vector2(10), FurballGame.DEFAULT_FONT, "Result: ", 50) {
            ScreenOriginType = OriginType.TopRight,
            OriginType       = OriginType.TopRight
        }
        );
        this.Manager.Add(this._messageDialogButton = new DrawableButton(new Vector2(10), FurballGame.DEFAULT_FONT, 24, "Message Dialog", Color.Blue, Color.Black, Color.Black, Vector2.Zero));
        this._messageDialogButton.OnClick += delegate {
            EtoHelper.MessageDialog(
            delegate(object _, DialogResult result) {
                FurballGame.GameTimeScheduler.ScheduleMethod(
                _ => {
                    this._dialogResult.Text = $"Result: {result.ToString()}";
                }, 0);
            }, "Test!", MessageBoxButtons.YesNoCancel);
        };

        this.Manager.Add(this._colorPickerButton = new DrawableButton(new Vector2(10, 62), FurballGame.DEFAULT_FONT, 24, "Message Dialog", Color.Blue, Color.Black, Color.Black, Vector2.Zero));
        this._colorPickerButton.OnClick += delegate {
            EtoHelper.OpenColorPicker(
            (_, color) => {
                FurballGame.GameTimeScheduler.ScheduleMethod(
                _ => {
                    this._dialogResult.FadeColor(color, 100);
                }, 0);
            }, Color.White);
        };

    }
}

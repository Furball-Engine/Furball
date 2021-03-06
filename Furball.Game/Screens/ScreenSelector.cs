using System.Collections.Generic;
using System.Numerics;
using Furball.Engine;
using Furball.Engine.Engine;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.UiElements;
using Furball.Engine.Engine.Localization;
using Furball.Game.Screens.Tests;
using Furball.Vixie.Backends.Shared;

namespace Furball.Game.Screens; 

public class ScreenSelector : TestScreen {
    private          List<(LocalizationStrings, Screen)> _screens;
    private readonly List<DrawableButton>                _buttons = new();

    private TextDrawable _topText;

    public override void Relayout(float newWidth, float newHeight) {
        base.Relayout(newWidth, newHeight);

        this._topText.Position = new Vector2(newWidth / 2f, 40);
    }

    public override void Initialize() {
        base.Initialize();

        this._screens = new List<(LocalizationStrings, Screen)> {
            (LocalizationStrings.CatmullTest, new CatmullTest()),
            (LocalizationStrings.TextBoxTest, new TextBoxTest()),
            (LocalizationStrings.CircleTest, new CircleDrawableTest()),
            (LocalizationStrings.ScrollingStutterTest, new ScrollingTest()),
            (LocalizationStrings.AudioEffectsTest, new AudioEffectTest()),
            (LocalizationStrings.LoadingScreenTest, new LoadingScreenTest()),
            (LocalizationStrings.FixedTimeStepTest, new FixedTimeStepTest()),
            (LocalizationStrings.LayoutingTest, new LayoutingTest()),
            (LocalizationStrings.MultiScreenTest, new MultiScreenTest()),
            (LocalizationStrings.FormTest, new FormTest())
        };

        this.Manager.Add(this._topText = new TextDrawable(new Vector2(1280f / 2f, 40), FurballGame.DEFAULT_FONT, LocalizationManager.GetLocalizedString(LocalizationStrings.ChooseScreen), 48) {
            OriginType = OriginType.Center
        });

        int currentY = 90;
        int currentX = 55;
        int i        = 0;

        foreach ((LocalizationStrings localizationString, Screen screen) in this._screens) {
            DrawableButton screenButton = new(
            new Vector2(currentX, currentY),
            FurballGame.DEFAULT_FONT,
            26,
            "",
            Color.White,
            Color.Black,
            Color.Black,
            new Vector2(300, 50)
            );

            screenButton.OnClick += delegate {
                ScreenManager.ChangeScreen(screen);
                    
                // FurballGame.Instance.ChangeScreenSize(1600, 900);
            };

            this.Manager.Add(screenButton);
            this._buttons.Add(screenButton);

            currentY += 70;
            i++;

            if (i % 8 == 0 && i != 0) {
                currentX += 320;
                currentY =  90;
            }
        }
        
        this.UpdateTextStrings();
    }

    public override void UpdateTextStrings() {
        base.UpdateTextStrings();

        for (int i = 0; i < this._buttons.Count; i++) {
            DrawableButton drawableButton = this._buttons[i];

            drawableButton.Text = LocalizationManager.GetLocalizedString(this._screens[i].Item1);
        }

        this._topText.Text = LocalizationManager.GetLocalizedString(LocalizationStrings.ChooseScreen);
    }
}
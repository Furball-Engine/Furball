using System.Collections.Generic;
using System.Numerics;
using Furball.Engine;
using Furball.Engine.Engine;
using Furball.Engine.Engine.Graphics;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.UiElements;
using Furball.Engine.Engine.Localization;
using Furball.Engine.Engine.Input.Events;
using Furball.Game.Screens;
using Furball.Vixie.Backends.Shared;

namespace Furball.Game; 

public class TestScreen : Screen {
    private TexturedDrawable _background;
    private DrawableButton   _languageButton;
    private DrawableButton   _backButton;

    public override void Initialize() {
        base.Initialize();

        this.Manager.Add(this._background = new TexturedDrawable(FurballGame.WhitePixel, Vector2.Zero) {
            ColorOverride = Color.BlueViolet,
            Depth         = 2
        });

        this.Manager.Add(
        this._backButton = new DrawableButton(
        new(10, FurballGame.DEFAULT_WINDOW_HEIGHT - 10),
        FurballGame.DefaultFont,
        24,
        LocalizationManager.GetLocalizedString(LocalizationStrings.Back),
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

        this.Manager.Add(
        this._languageButton = new DrawableButton(new(10, 10), FurballGame.DefaultFont, 24, "Language: ", Color.Blue, Color.White, Color.Black, Vector2.Zero) {
            ScreenOriginType = OriginType.BottomRight,
            OriginType       = OriginType.BottomRight
        }
        );
        
        this._languageButton.OnClick += OnLanguageButtonClick;
        
        this.UpdateLanguageButton();
    }
    
    private void OnLanguageButtonClick(object sender, MouseButtonEventArgs mouseButtonEventArgs) {
        List<ISO639_2Code> supported = LocalizationManager.GetSupportedLanguages();

        for (int i = 0; i < supported.Count; i++) {
            ISO639_2Code code = supported[i];
            if (code == LocalizationManager.CurrentLanguage.Iso6392Code()) {
                if (i != supported.Count - 1)
                    LocalizationManager.CurrentLanguage = LocalizationManager.GetLanguageFromCode(supported[i + 1])!;
                else
                    LocalizationManager.CurrentLanguage = LocalizationManager.GetLanguageFromCode(supported[0])!;

                break;
            }
        }

        this.UpdateLanguageButton();
    }

    private void UpdateLanguageButton() {
        this._languageButton.Text = $"Language: {LocalizationManager.CurrentLanguage}";
    }

    public override void UpdateTextStrings() {
        this.UpdateLanguageButton();
        this._backButton.Text = LocalizationManager.GetLocalizedString(LocalizationStrings.Back);
    }

    public override void Relayout(float newWidth, float newHeight) {
        base.Relayout(newWidth, newHeight);

        if(this._background != null)
            this._background.Scale = new Vector2(newWidth, newHeight);
    }
}
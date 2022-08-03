using Furball.Engine;
using Furball.Engine.Engine.Localization;
using Furball.Game.Screens;
using Furball.Vixie;
using Silk.NET.Input;
using SixLabors.ImageSharp;

namespace Furball.Game;

public class FurballTestGame : FurballGame {
    public FurballTestGame() : base(new ScreenSelector()) {}

    protected override void Initialize() {
        base.Initialize();
        
        GraphicsBackend.Current.ScreenshotTaken += OnScreenshot;
        
        InputManager.OnKeyDown += delegate(object sender, Key key) {
            if (key == Key.F1)
                GraphicsBackend.Current.TakeScreenshot();
        };
    }
    
    private void OnScreenshot(object sender, Image e) {
        e.Save("test.png");
    }

    protected override void InitializeLocalizations() {
        LocalizationManager.AddDefaultTranslation(LocalizationStrings.Back, "Back");

        LocalizationManager.AddDefaultTranslation(LocalizationStrings.CatmullTest,          "Catmull Test");
        LocalizationManager.AddDefaultTranslation(LocalizationStrings.TextBoxTest,          "TextBox Test");
        LocalizationManager.AddDefaultTranslation(LocalizationStrings.CircleTest,           "Circle Test");
        LocalizationManager.AddDefaultTranslation(LocalizationStrings.ScrollingStutterTest, "Scrolling Stutter Test");
        LocalizationManager.AddDefaultTranslation(LocalizationStrings.AudioEffectsTest,     "Audio Effects Test");
        LocalizationManager.AddDefaultTranslation(LocalizationStrings.LoadingScreenTest,    "Loading Screen Test");
        LocalizationManager.AddDefaultTranslation(LocalizationStrings.FixedTimeStepTest,    "Fixed Time Step Test");
        LocalizationManager.AddDefaultTranslation(LocalizationStrings.LayoutingTest,        "Layouting Test");
        LocalizationManager.AddDefaultTranslation(LocalizationStrings.MultiScreenTest,      "Multi Screen Test");
        LocalizationManager.AddDefaultTranslation(LocalizationStrings.FormTest,             "Form Test");

        LocalizationManager.AddDefaultTranslation(LocalizationStrings.ChooseScreen, "Choose Screen");
    }
}

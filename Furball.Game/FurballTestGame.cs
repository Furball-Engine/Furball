using Furball.Engine;
using Furball.Engine.Engine.Input;
using Furball.Engine.Engine.Localization;
using Furball.Game.Screens;
using Furball.Vixie;
using Silk.NET.Input;
using SixLabors.ImageSharp;

namespace Furball.Game;

public class FurballTestGame : FurballGame {
    public FurballTestGame() : base(new ScreenSelector()) {}

    private enum TestKeybinds {
        TakeScreenshot
    }
    
    protected override void Initialize() {
        base.Initialize();
        
        GraphicsBackend.Current.ScreenshotTaken += OnScreenshot;
    }

    private Keybind _screenshotKeybind;
    public override void RegisterKeybinds() {
        base.RegisterKeybinds();
        
        InputManager.RegisterKeybind(this._screenshotKeybind = new Keybind(TestKeybinds.TakeScreenshot, "Take Screenshot", Key.F1,
                                                 () => {
                                                     GraphicsBackend.Current.TakeScreenshot();
                                                 }));
    }

    public override void UnregisterKeybinds() {
        base.UnregisterKeybinds();
        
        InputManager.UnregisterKeybind(this._screenshotKeybind);
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
        LocalizationManager.AddDefaultTranslation(LocalizationStrings.SmartTextTest,        "Smart Text Test");
        LocalizationManager.AddDefaultTranslation(LocalizationStrings.VideoDrawableTest,    "VideoDrawable Test");

        LocalizationManager.AddDefaultTranslation(LocalizationStrings.ChooseScreen, "Choose Screen");
    }
}

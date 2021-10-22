using Furball.Engine;
using Furball.Engine.Engine;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.UiElements;
using Furball.Engine.Engine.Helpers.Logger;
using Furball.Engine.Engine.Localization;
using Furball.Engine.Engine.Localization.Languages;
using Kettu;
using Microsoft.Xna.Framework;

namespace Furball.Game.Screens {
    public class BasicTestScreen : Screen {
        public override void Initialize() {
            base.Initialize();
            
            TexturedDrawable background = new TexturedDrawable(FurballGame.WhitePixel, Vector2.Zero) {
                ColorOverride = Color.BlueViolet,
                Scale = new Vector2(1280, 720),
                Depth = 1f
            };
            
            this.Manager.Add(background);
            UiButtonDrawable screenSwitchButton = new (new Vector2(FurballGame.Random.Next(0, 1280), FurballGame.Random.Next(0, 720)), "Change Language", FurballGame.DEFAULT_FONT, 30, Color.Cyan, Color.Red, Color.Black, new Vector2(200, 40));

            screenSwitchButton.OnClick += delegate {
                Logger.Log($"button click event {FurballGame.Time}");
                Logger.Log($"Current Language: {LocalizationManager.GetLanguageFromCode(LocalizationManager.CurrentLanguage.Iso6392Code())}");
                LocalizationManager.CurrentLanguage = new LojbanLanguage();

                FurballGame.GameTimeScheduler.ScheduleMethod(
                delegate {
                    ScreenManager.ChangeScreen(new BasicTestScreen());
                },
                FurballGame.Time + 1000
                );

                FurballGame.GameTimeScheduler.ScheduleMethod(
                delegate {
                    ScreenManager.ChangeScreen(new BasicTestScreen());
                },
                FurballGame.Time + 2000
                );

                FurballGame.GameTimeScheduler.ScheduleMethod(
                delegate {
                    ScreenManager.ChangeScreen(new BasicTestScreen());
                },
                FurballGame.Time + 3000
                );
            };

            screenSwitchButton.OnHover += delegate {
                Logger.Log($"button hover event {FurballGame.Time}");
            };
            
            this.Manager.Add(screenSwitchButton);

            TextDrawable localizationTest = new(new(10f), FurballGame.DEFAULT_FONT, LocalizationManager.GetLocalizedString("cat"), 30);
            this.Manager.Add(localizationTest);
        }
    }
}

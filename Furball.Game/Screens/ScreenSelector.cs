using System.Collections.Generic;
using System.Numerics;
using Furball.Engine;
using Furball.Engine.Engine;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.UiElements;
using Furball.Vixie.Graphics;


namespace Furball.Game.Screens {
    public class ScreenSelector : Screen {
        public List<(string, Screen)> Screens;

        public override void Initialize() {
            base.Initialize();

            this.Screens = new List<(string, Screen)> {
                ("Catmull Testing", new CatmullTestScreen()),
            };

            TexturedDrawable background = new TexturedDrawable(FurballGame.WhitePixel, Vector2.Zero) {
                ColorOverride = Color.BlueViolet,
                Scale         = new Vector2(1280, 720),
                Depth         = 1f
            };

            this.Manager.Add(background);
            TextDrawable topText = new TextDrawable(new Vector2(1280f / 2f, 40), FurballGame.DEFAULT_FONT, "Choose Screen", 48) {
                OriginType = OriginType.Center
            };

            this.Manager.Add(topText);

            int currentY = 90;
            int currentX = 55;
            int i = 0;

            foreach ((string screenName, Screen screen) in this.Screens) {
                UiButtonDrawable screenButton = new UiButtonDrawable(
                    new Vector2(currentX, currentY),
                    screenName,
                    FurballGame.DEFAULT_FONT,
                    26,
                    Color.White,
                    Color.Black,
                    Color.Black,
                    new Vector2(250, 50)
                );

                screenButton.OnClick += delegate {
                    ScreenManager.ChangeScreen(screen);
                    
                    // FurballGame.Instance.ChangeScreenSize(1600, 900);
                };

                this.Manager.Add(screenButton);

                currentY += 70;
                i++;

                if (i % 9 == 0 && i != 0) {
                    currentX += 300;
                    currentY =  90;
                }
            }
        }
    }
}

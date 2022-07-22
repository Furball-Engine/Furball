using System.Collections.Generic;
using System.Numerics;
using Furball.Engine;
using Furball.Engine.Engine;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.UiElements;
using Furball.Vixie.Backends.Shared;


namespace Furball.Game.Screens; 

public class ScreenSelector : TestScreen {
    public  List<(string, Screen)> Screens;
    private TextDrawable           _topText;

    public override void Relayout(float newWidth, float newHeight) {
        base.Relayout(newWidth, newHeight);

        this._topText.Position = new Vector2(newWidth / 2f, 40);
    }

    public override void Initialize() {
        base.Initialize();
            
        this.Screens = new List<(string, Screen)> {
            ("Catmull Testing", new CatmullTest()),
            ("UI TextBox Testing", new TextBoxTest()),
            ("Circle Test", new CircleDrawableTest()),
            ("Scrolling Stutter Test", new ScrollingTest()),
            ("Audio Effects Testing", new AudioEffectTest()),
            ("Loading Screen Test", new LoadingScreenTest()),
            ("Fixed Time Step test", new FixedTimeStepTest()),
            ("Layouting Test", new LayoutingTest())
        };

        this.Manager.Add(this._topText = new TextDrawable(new Vector2(1280f / 2f, 40), FurballGame.DEFAULT_FONT, "Choose Screen", 48) {
            OriginType = OriginType.Center
        });

        int currentY = 90;
        int currentX = 55;
        int i        = 0;

        foreach ((string screenName, Screen screen) in this.Screens) {
            DrawableButton screenButton = new(
            new Vector2(currentX, currentY),
            FurballGame.DEFAULT_FONT,
            26,
            screenName,
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
using System.Numerics;
using Furball.Engine;
using Furball.Engine.Engine.Graphics.Drawables.UiElements;
using Furball.Vixie.Backends.Shared;

namespace Furball.Game.Screens.Tests; 

public class InputOrderTest : TestScreen {
    public override void Initialize() {
        base.Initialize();

        this.Manager.Add(new DrawableButton(new Vector2(10, 10), FurballGame.DefaultFont, 24, "Button 1", Color.Blue, Color.Black, Color.Black, Vector2.Zero) {
            Depth = 1
        });
        
        this.Manager.Add(new DrawableButton(new Vector2(30, 30), FurballGame.DefaultFont, 24, "Button 2", Color.Blue, Color.Black, Color.Black, Vector2.Zero) {
            Depth = 0.5
        });
    }
}

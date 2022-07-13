using System.Numerics;
using Furball.Engine;
using Furball.Engine.Engine;
using Furball.Engine.Engine.Graphics.Drawables.UiElements;

namespace Furball.Game.Screens {
    public class TextBoxTest : TestScreen {
        public override void Initialize() {
            base.Initialize();
            
            this.Manager.Add(new DrawableTextBox(new Vector2(10, 10), FurballGame.DEFAULT_FONT, 24, 240, "hi"));
        }
    }
}

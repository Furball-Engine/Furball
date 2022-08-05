using System.Numerics;
using Furball.Engine;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.UiElements;

namespace Furball.Game.Screens.Tests; 

public class TextBoxTest : TestScreen {
    public override void Initialize() {
        base.Initialize();
            
        this.Manager.Add(new DrawableTextBox(new Vector2(10, 10), FurballGame.DEFAULT_FONT, 24, 300, "Text Box 1"));
        this.Manager.Add(new DrawableTextBox(new Vector2(10, 40), FurballGame.DEFAULT_FONT, 24, 300, "Text Box 2"));
        this.Manager.Add(new DrawableTextBox(new Vector2(10, 70), FurballGame.DEFAULT_FONT, 24, 300, "Text Box 3"));
        this.Manager.Add(new DrawableForm("Test Textbox *IN* FORM!", new DrawableTextBox(Vector2.Zero, FurballGame.DEFAULT_FONT, 24, 300, "I am in a form!")));
    }
}
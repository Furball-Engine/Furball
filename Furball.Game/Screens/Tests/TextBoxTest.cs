using System.Numerics;
using FontStashSharp;
using Furball.Engine;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.UiElements;
using Furball.Engine.Engine.Input;
using Furball.Vixie.Backends.Shared;

namespace Furball.Game.Screens.Tests; 

public class TextBoxTest : TestScreen {
    public class TextInputStackDrawableTest : TextDrawable, ICharInputHandler {
        public bool SaveInStack {
            get;
            set;
        } = true;
        public void HandleChar(char c) {
            this.Text += c;
        }
        public void HandleFocus() {
            //Set us to blue when focused
            this.ColorOverride = Color.CornflowerBlue;
        }
        public void HandleDefocus() {
            //Set us to red when unfocused
            this.ColorOverride = Color.LightPink;
        }

        public TextInputStackDrawableTest(Vector2 position, FontSystem font, string text, int fontSize) : base(position, font, text, fontSize) {
            FurballGame.InputManager.TakeTextFocus(this);
        }

        public override void Dispose() {
            base.Dispose();
            
            FurballGame.InputManager.ReleaseTextFocus(this);
        }
    }
    
    public override void Initialize() {
        base.Initialize();
            
        this.Manager.Add(new TextInputStackDrawableTest(new Vector2(10), FurballGame.DEFAULT_FONT, "Global input grabber: ", 40) {
            ScreenOriginType = OriginType.TopRight,
            OriginType = OriginType.TopRight
        });
        
        this.Manager.Add(new DrawableTextBox(new Vector2(10, 10), FurballGame.DEFAULT_FONT, 24, 300, "Text Box 1"));
        this.Manager.Add(new DrawableTextBox(new Vector2(10, 40), FurballGame.DEFAULT_FONT, 24, 300, "Text Box 2"));
        this.Manager.Add(new DrawableTextBox(new Vector2(10, 70), FurballGame.DEFAULT_FONT, 24, 300, "Text Box 3"));
        this.Manager.Add(new DrawableForm("Test Textbox *IN* FORM!", new DrawableTextBox(Vector2.Zero, FurballGame.DEFAULT_FONT, 24, 300, "I am in a form!")));
    }
}
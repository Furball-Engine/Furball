using System.Numerics;
using FontStashSharp;
using FontStashSharp.RichText;
using Furball.Engine.Engine.Graphics.Drawables.Managers;

namespace Furball.Engine.Engine.Graphics.Drawables; 

public class RichTextDrawable : Drawable {
    private readonly RichTextLayout _layout;
    
    public RichTextDrawable(Vector2 position, string text, FontSystem system, int fontSize) {
        this._layout = new RichTextLayout {
            Font = system.GetFont(fontSize),// fontSystem is default Arial
            Text = "First line./nSecond line.",
        };
    }

    public override void Draw(double time, DrawableBatch batch, DrawableManagerArgs args) {
        base.Draw(time, batch, args);
        
        //TODO
        // batch.DrawRichString();
    }
}

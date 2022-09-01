using System.Numerics;
using FontStashSharp;
using FontStashSharp.RichText;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Helpers;

namespace Furball.Engine.Engine.Graphics.Drawables; 

public class RichTextDrawable : Drawable {
    public readonly RichTextLayout Layout;

    static RichTextDrawable() {
        RichTextDefaults.FontResolver = s => {
            var args     = s.Split(',');
            var fontName = args[0].Trim();
            var fontSize = int.Parse(args[1].Trim());

            return ContentManager.LoadSystemFont(fontName).GetFont(fontSize);
        };
        // TODO
        // RichTextDefaults.ImageResolver = s => ContentManager.LoadTextureFromFileCached(s, ContentSource.User);
    }

    public override Vector2 Size {
        get => this.Layout.Measure(null).ToVector2() * this.Scale;
    }

    public RichTextDrawable(Vector2 position, string text, FontSystem system, int fontSize) {
        this.Layout = new RichTextLayout {
            Font = system.GetFont(fontSize),// fontSystem is default Arial
            Text = text
        };
        this.Position = position;
    }

    public override void Draw(double time, DrawableBatch batch, DrawableManagerArgs args) {
        batch.DrawRichString(this.Layout, args.Position, args.Color, args.Scale, args.Rotation, this.RotationOrigin);
    }
}

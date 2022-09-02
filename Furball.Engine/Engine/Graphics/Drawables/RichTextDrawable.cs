using System.Drawing;
using System.Numerics;
using FontStashSharp;
using FontStashSharp.RichText;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Helpers;
using Furball.Vixie;
using Furball.Vixie.Backends.Shared;

namespace Furball.Engine.Engine.Graphics.Drawables; 

public class RichTextDrawable : Drawable {
    public readonly RichTextLayout Layout;

    static RichTextDrawable() {
        RichTextDefaults.FontResolver = s => {
            string[] args     = s.Split(',');
            string fontName = args[0].Trim();
            int fontSize = int.Parse(args[1].Trim());

            return ContentManager.LoadSystemFont(fontName).GetFont(fontSize);
        };
        RichTextDefaults.ImageResolver = s => {
            Texture tex = ContentManager.LoadTextureFromFileCached(s, ContentSource.User);
            return new TextureFragment((VixieTexture)tex, new Rectangle(0, 0, tex.Width, tex.Height));
        };
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

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
            string[] args        = s.Split(',');
            string   textureName = args[0];

            Vector2 scale = args.Length switch {
                2 => new Vector2(float.Parse(args[1])),
                3 => new Vector2(float.Parse(args[1]), float.Parse(args[2])),
                _ => Vector2.One
            };

            Texture tex = ContentManager.LoadTextureFromFileCached(textureName, ContentSource.User);
            TextureFragment fragment = new((VixieTexture)tex, new Rectangle(0, 0, tex.Width, tex.Height)) {
                Scale = scale
            };
            return fragment;
        };
    }

    public override Vector2 Size => this.Layout.Size.ToVector2() * this.Scale;

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

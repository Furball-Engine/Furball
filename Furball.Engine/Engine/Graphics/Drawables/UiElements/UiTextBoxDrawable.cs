using SpriteFontPlus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Furball.Engine.Engine.Graphics.Drawables.Managers;

namespace Furball.Engine.Engine.Graphics.Drawables.UiElements {
    public class UiTextBoxDrawable : TextDrawable {
        public float TextBoxWidth;

        public override Vector2 Size => new(this.TextBoxWidth, base.Size.Y);

        public UiTextBoxDrawable(byte[] font, string text, float size, float width, CharacterRange[] range = null) : base(font, text, size, range) {
            this.TextBoxWidth = width;
        }
        public UiTextBoxDrawable(SpriteFont font, string text, float width) : base(font, text) {
            this.TextBoxWidth = width;
        }

        public override void Draw(GameTime time, DrawableBatch batch, DrawableManagerArgs args) {
            batch.ShapeBatch.DrawRectangle(args.Position - args.Origin, this.Size, Color.Transparent, Color.White);
            
            base.Draw(time, batch, args);
        }
    }
}

using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteFontPlus;

namespace Furball.Engine.Engine.Graphics.Drawables {
    /// <summary>
    /// Simple way to Draw Text
    /// </summary>
    public class TextDrawable : ManagedDrawable {
        /// <summary>
        /// SpriteFont that gets used during Drawing
        /// </summary>
        public SpriteFont Font;
        /// <summary>
        /// Text that gets drawn
        /// </summary>
        public string Text;

        /// <summary>
        /// The height of the text
        /// </summary>
        public new Vector2 Size => this.Font.MeasureString(this.Text);

        /// <summary>
        /// Creates a new TextDrawable
        /// </summary>
        /// <param name="font">A byte[] containing the font in ttf form)</param>
        /// <param name="text">What Text to Draw (can be changed later)</param>
        /// <param name="size">The size of the text as a float</param>
        /// <param name="range">The CharacterRange of the SpriteFont</param>
        public TextDrawable(byte[] font, string text, float size, CharacterRange[] range = null) {
            range ??= new[] {
                CharacterRange.BasicLatin, CharacterRange.Hiragana, CharacterRange.Katakana, CharacterRange.Latin1Supplement
            };

            TtfFontBakerResult fontBakeResult = TtfFontBaker.Bake(font, size, (int)(1024f * (size / 25f)), (int)(1024f * (size / 25f)), range);

            this.Font = fontBakeResult.CreateSpriteFont(FurballGame.Instance.GraphicsDevice);
            this.Text = text;
        }
        /// <summary>
        /// Creates a new TextDrawable
        /// </summary>
        /// <param name="font">The SpriteFont to render with</param>
        /// <param name="text">What Text to Draw (can be changed later)</param>
        public TextDrawable(SpriteFont font, string text) {
            this.Font = font;
            this.Text = text;
        }

        public override void Draw(GameTime time, SpriteBatch batch, DrawableManagerArgs args) {
            batch.DrawString(this.Font, this.Text, args.Position, args.Color, args.Rotation, args.Origin, args.Scale, args.Effects, args.LayerDepth);
        }
    }
}

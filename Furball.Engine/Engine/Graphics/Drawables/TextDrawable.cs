using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
        /// Creates a new TextDrawable
        /// </summary>
        /// <param name="font">What SpriteFont to draw with</param>
        /// <param name="text">What Text to Draw (can be changed later)</param>
        public TextDrawable(SpriteFont font, string text) {
            this.Font = font;
            this.Text = text;
        }

        public override void Draw(GameTime time, SpriteBatch batch) {
            batch.DrawString(this.Font, this.Text, this.Position, this.ColorOverride, this.Rotation, Vector2.Zero, this.Scale, this.SpriteEffect, 0f);
        }
    }
}

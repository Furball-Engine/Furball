using FontStashSharp;
using Microsoft.Xna.Framework;
using Furball.Engine.Engine.Graphics.Drawables.Managers;

namespace Furball.Engine.Engine.Graphics.Drawables {
    /// <summary>
    /// Simple way to Draw Text
    /// </summary>
    public class TextDrawable : ManagedDrawable {
        /// <summary>
        /// SpriteFont that gets used during Drawing
        /// </summary>
        public DynamicSpriteFont Font;
        /// <summary>
        /// Text that gets drawn
        /// </summary>
        public string Text;

        /// <summary>
        /// The height of the text
        /// </summary>
        public override Vector2 Size => this.Font.MeasureString(this.Text) * this.Scale;
        
        /// <summary>
        /// Creates a new TextDrawable
        /// </summary>
        /// <param name="position">Where to Draw</param>
        /// <param name="font">A byte[] containing the font in ttf form)</param>
        /// <param name="text">What Text to Draw (can be changed later)</param>
        /// <param name="size">The size of the text as a float</param>
        public TextDrawable(Vector2 position, FontSystem font, string text, int size) {
            this.Position = position;
            
            this.Font = font.GetFont(size);

            this.Text = text;
        }

        public override void Draw(GameTime time, DrawableBatch batch, DrawableManagerArgs args) {
            args.Position *= FurballGame.VerticalRatio;
            args.Scale    *= FurballGame.VerticalRatio;
            
            batch.SpriteBatch.DrawString(
                this.Font, this.Text, args.Position, args.Color, args.Scale, args.Rotation, Vector2.Zero
            );
        }
    }
}

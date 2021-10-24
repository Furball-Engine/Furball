using System;
using System.Linq;
using FontStashSharp;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Helpers.Logger;
using Kettu;
using Microsoft.Xna.Framework;

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

        public TextColorType ColorType = TextColorType.Solid;
        public Color[] Colors = {
            Color.Cyan, Color.Pink, Color.White, Color.Pink, Color.Cyan
        };
        
        /// <summary>
        /// Creates a new TextDrawable
        /// </summary>
        /// <param name="position">Where to Draw</param>
        /// <param name="font">A byte[] containing the font in ttf form)</param>
        /// <param name="text">What Text to Draw (can be changed later)</param>
        /// <param name="size">The size of the text as a float</param>
        public TextDrawable(Vector2 position, FontSystem font, string text, int size) {
            this.Position = position;

            if (!ContentManager.FSS_CACHE.TryGetValue((font, size), out this.Font)) {
                this.Font = font.GetFont(size);
                ContentManager.FSS_CACHE.Add((font, size), this.Font);
                Logger.Log($"Caching DynamicSpriteFont of size {size}", LoggerLevelCacheEvent.Instance);
            }
            
            this.Font = font.GetFont(size);

            this.Text = text;
        }

        public static Color[] StretchColours(Color[] colours, int size) => throw new NotImplementedException();

        public override void Draw(GameTime time, DrawableBatch batch, DrawableManagerArgs args) {
            args.Position *= FurballGame.VerticalRatio;
            args.Scale    *= FurballGame.VerticalRatio;

            switch (this.ColorType) {
                case TextColorType.Solid: {
                    batch.SpriteBatch.DrawString(this.Font, this.Text, args.Position, args.Color, args.Scale, args.Rotation, Vector2.Zero);
                    break;
                }
                case TextColorType.Repeating: {
                    Color[] colors = this.Colors;
                    while (colors.Length < this.Text.Length)
                        colors = colors.Concat(colors).ToArray();

                    batch.SpriteBatch.DrawString(this.Font, this.Text, args.Position, colors, args.Scale, args.Rotation, Vector2.Zero);
                    break;
                }
                case TextColorType.Stretch: {
                    batch.SpriteBatch.DrawString(
                    this.Font,
                    this.Text,
                    args.Position,
                    StretchColours(this.Colors, this.Text.Length),
                    args.Scale,
                    args.Rotation,
                    Vector2.Zero
                    );
                    break;
                }
            }
        }
    }

    public enum TextColorType {
        Solid,
        Repeating,
        Stretch
    }
}

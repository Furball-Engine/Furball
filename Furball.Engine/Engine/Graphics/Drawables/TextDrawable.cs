
using System.Linq;
using System.Numerics;
using FontStashSharp;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Helpers;
using Furball.Engine.Engine.Helpers.Logger;
using Furball.Vixie.Graphics;
using Kettu;


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
        ///     The color type of the text, Solid means a single color, Repeating means the pattern in Colors repeats, and Stretch
        ///     means the colours stretch to fit
        /// </summary>
        public TextColorType ColorType = TextColorType.Solid;
        /// <summary>
        ///     An array of colours for the text drawable to use depending on the TextColorType
        /// </summary>
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

        public override void Draw(double time, DrawableBatch batch, DrawableManagerArgs args) {
            args.Position *= FurballGame.VerticalRatio;
            args.Scale    *= FurballGame.VerticalRatio;

            switch (this.ColorType) {
                case TextColorType.Solid: {
                    //TODO(Eevee): figure out a clean way to manage this
                    //batch.Renderer.DrawString(this.Font, this.Text, args.Position, args.Color, args.Scale, args.Rotation, Vector2.Zero);
                    break;
                }
                case TextColorType.Repeating: {
                    Color[] colors = this.Colors;
                    while (colors.Length < this.Text.Length)
                        colors = colors.Concat(colors).ToArray();

                    //TODO(Eevee): figure out a clean way to manage this
                    //batch.Renderer.DrawString(this.Font, this.Text, args.Position, colors, args.Scale, args.Rotation, Vector2.Zero);
                    break;
                }
                case TextColorType.Stretch: {
                    //TODO(Eevee): figure out a clean way to manage this
                    //batch.Renderer.DrawString(
                    //    this.Font,
                    //    this.Text,
                    //    args.Position,
                    //    ArrayHelper.FitElementsInANewArray(this.Colors, this.Text.Length),
                    //    args.Scale,
                    //    args.Rotation,
                    //    Vector2.Zero
                    //    );
                    break;
                }
            }
        }
    }

    public enum TextColorType {
        /// <summary>
        ///     A single color for the whole text
        /// </summary>
        Solid,
        /// <summary>
        ///     A few colors repeated constantly
        /// </summary>
        Repeating,
        /// <summary>
        ///     A few colors stretched to fit the text
        /// </summary>
        Stretch
    }
}

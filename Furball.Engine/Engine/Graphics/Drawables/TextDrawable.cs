using System.Collections.Generic;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Helpers;
using Furball.Engine.Engine.Helpers.Logger;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteFontPlus;
using MathHelper=Furball.Engine.Engine.Helpers.MathHelper;

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
        public override Vector2 Size => this.Font.MeasureString(this.Text) * this.Scale;

        /// <summary>
        /// Creates a new TextDrawable
        /// </summary>
        /// <param name="position">Where to Draw</param>
        /// <param name="font">A byte[] containing the font in ttf form)</param>
        /// <param name="text">What Text to Draw (can be changed later)</param>
        /// <param name="size">The size of the text as a float</param>
        /// <param name="range">The CharacterRange of the SpriteFont</param>
        public TextDrawable(Vector2 position, byte[] font, string text, float size, CharacterRange[] range = null) {
            this.Position = position;

            range ??= new[] {
                CharacterRange.BasicLatin
            };

            long crc64 = MathHelper.CRC64(font, 5000);
            if (!ContentManager.SPRITEFONTPLUS_CACHE.TryGetValue(new KeyValuePair<long, float>(crc64, size), out this.Font)) {
                TtfFontBakerResult fontBakeResult = TtfFontBaker.Bake(font, size, (int)(1024f * (size / 25f)), (int)(1024f * (size / 25f)), range);

                SpriteFont spriteFont = fontBakeResult.CreateSpriteFont(FurballGame.Instance.GraphicsDevice);
                spriteFont.DefaultCharacter = '?';
                
                ContentManager.SPRITEFONTPLUS_CACHE.Add(new KeyValuePair<long, float>(crc64, size), spriteFont);
                Logger.Log($"Caching SpriteFont with crc8:{crc64}, fontSize:{size}, dataSize:{font.LongLength}", new LoggerLevelCacheEvent());
                
                this.Font = spriteFont;
            }

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

        public override void Draw(GameTime time, DrawableBatch batch, DrawableManagerArgs args) {
            args.Position *= FurballGame.VerticalRatio;
            args.Scale    *= FurballGame.VerticalRatio;
            
            batch.SpriteBatch.DrawString(
                this.Font, 
                this.Text, 
                args
            );
        }
    }
}

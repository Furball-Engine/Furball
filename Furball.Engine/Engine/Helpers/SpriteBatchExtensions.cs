using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Furball.Engine.Engine.Helpers {
    public static class SpriteBatchExtensions {
        /// <summary>
        /// Shorthand for drawing Texture2Ds with DrawableManagerArgs
        /// </summary>
        /// <param name="batch">SpriteBatch</param>
        /// <param name="texture2D">Texture to Draw</param>
        /// <param name="args">DrawableManagerArgs</param>
        /// <param name="cropping">Optional Cropping</param>
        public static void Draw(this SpriteBatch batch, Texture2D texture2D, DrawableManagerArgs args, Rectangle? cropping = null) {
            batch.Draw(texture2D, args.Position, cropping, args.Color, args.Rotation, args.Origin, args.Scale, args.Effects, args.LayerDepth);
        }
        /// <summary>
        /// Shorthand for drawwing Strings with DrawableManagerArgs
        /// </summary>
        /// <param name="batch">SpriteBatch</param>
        /// <param name="font">SpriteFont to draw with</param>
        /// <param name="text">Text to Draw</param>
        /// <param name="args">DrawableManagerArgs</param>
        public static void DrawString(this SpriteBatch batch, SpriteFont font, string text, DrawableManagerArgs args) {
            batch.DrawString(font, text, args.Position, args.Color, args.Rotation, args.Origin, args.Scale, args.Effects, args.LayerDepth);
        }
    }
}

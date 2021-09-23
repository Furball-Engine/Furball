using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Furball.Engine.Engine.Helpers {
    public static class SpriteBatchExtensions {
        public static void Draw(this SpriteBatch batch, Texture2D texture2D, DrawableManagerArgs args, Rectangle? cropping = null) {
            batch.Draw(texture2D, args.Position, cropping, args.Color, args.Rotation, args.Origin, args.Scale, args.Effects, args.LayerDepth);
        }

        public static void DrawString(this SpriteBatch batch, SpriteFont font, string text, DrawableManagerArgs args) {
            batch.DrawString(font, text, args.Position, args.Color, args.Rotation, args.Origin, args.Scale, args.Effects, args.LayerDepth);
        }
    }
}

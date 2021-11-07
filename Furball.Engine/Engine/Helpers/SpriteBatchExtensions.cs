
using System.Drawing;
using System.Numerics;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Vixie.Graphics;
using Furball.Vixie.Graphics.Renderers;

namespace Furball.Engine.Engine.Helpers {
    public static class SpriteBatchExtensions {
        ///// <summary>
        ///// Shorthand for drawing Texture2Ds with DrawableManagerArgs
        ///// </summary>
        ///// <param name="batch">SpriteBatch</param>
        ///// <param name="texture2D">Texture to Draw</param>
        ///// <param name="args">DrawableManagerArgs</param>
        ///// <param name="cropping">Optional Cropping</param>
        //public static void Draw(this SpriteBatch batch, Texture2D texture2D, DrawableManagerArgs args, Rectangle? cropping = null) {
        //    batch.Draw(texture2D, args.Position, cropping, args.Color, args.Rotation, Vector2.Zero, args.Scale, args.Effects, 0f);
        //}

        public static void Draw(this ITextureRenderer renderer, Texture texture, DrawableManagerArgs args, Rectangle? cropping = null) {
            renderer.Draw(texture, args.Position, null, args.Scale, args.Rotation, args.Color, cropping);
        }
    }
}

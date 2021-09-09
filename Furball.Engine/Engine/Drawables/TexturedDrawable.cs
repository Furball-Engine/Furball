using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Furball.Engine.Engine.Drawables {
    /// <summary>
    /// A Basic Managed Drawable that just Draws a Texture to the Screen,
    /// </summary>
    public class TexturedDrawable : ManagedDrawable {
        /// <summary>
        /// The Texture Being drawn
        /// </summary>
        private Texture2D _texture;
        /// <summary>
        /// Crop Rectangle, this basically tells which part of the Texture to Render
        /// Leave null to draw the entire Texture
        /// </summary>
        private Rectangle? _cropped = null;
        /// <summary>
        /// TexturedDrawable Constructor
        /// </summary>
        /// <param name="texture">Texture to Draw</param>
        /// <param name="position">Where to Draw</param>
        public TexturedDrawable(Texture2D texture, Vector2 position) {
            this.Position      = position;
            this.Rotation      = 0f;
            this.ColorOverride = Color.White;
            this.Scale         = new Vector2(1,             1);
            this.Size          = new Vector2(texture.Width, texture.Height);

            this._texture = texture;
        }
        /// <summary>
        /// TexturedDrawable Constructor that allows for Cropping
        /// </summary>
        /// <param name="texture">Texture to Draw</param>
        /// <param name="position">Where to Draw</param>
        /// <param name="cropped">What Part to Draw</param>
        /// <param name="rotation">Rotation in Radians</param>
        public TexturedDrawable(Texture2D texture, Vector2 position, Rectangle cropped, float rotation = 0f) {
            this.Position = position;
            this.Rotation = rotation;

            this._cropped = cropped;
            this._texture = texture;
        }

        public override void Draw(GameTime time, SpriteBatch batch) {
            //TODO: origin
            //TODO: depth
            batch.Draw(this._texture, this.Position, this._cropped, this.ColorOverride, this.Rotation, Vector2.Zero, this.Scale, this.SpriteEffect, 0f);
        }
        /// <summary>
        /// Changes the Cropping of the Texture
        /// </summary>
        /// <param name="crop">New Cropping</param>
        public void ChangeCropping(Rectangle? crop) => this._cropped = crop;
    }
}

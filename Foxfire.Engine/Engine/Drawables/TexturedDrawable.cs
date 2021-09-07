using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Foxfire.Engine.Engine.Drawables {
    public class TexturedDrawable : ManagedDrawable {
        private Texture2D _texture;
        private Rectangle _cropped;

        public TexturedDrawable(Texture2D texture, Vector2 position) {
            this.Position      = position;
            this.Rotation      = 0f;
            this.ColorOverride = Color.White;
            this.Size          = new Vector2(texture.Width, texture.Height);

            this._texture = texture;
        }

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

        public void ChangeCropping(Rectangle crop) => this._cropped = crop;
    }
}

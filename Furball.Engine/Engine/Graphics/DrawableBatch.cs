using Microsoft.Xna.Framework.Graphics;

namespace Furball.Engine.Engine.Graphics {
    /// <summary>
    /// A Basic Abstraction for Sprite and Shape batch
    /// </summary>
    public class DrawableBatch {
        public SpriteBatch SpriteBatch;

        private bool _begun;
        public bool Begun => _begun;
        
        public DrawableBatch(SpriteBatch spriteBatch) {
            this.SpriteBatch = spriteBatch;
        }

        public void Begin() {
            this.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            this._begun = true;
        }

        public void End() {
            this.SpriteBatch.End();
            this._begun = false;
        }
    }
}

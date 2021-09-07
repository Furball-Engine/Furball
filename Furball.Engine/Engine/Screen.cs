using Furball.Engine.Engine.Drawables;
using Microsoft.Xna.Framework;

namespace Furball.Engine.Engine {
    public class Screen : DrawableGameComponent {
        private DrawableManager _manager = new();
        public Screen() : base(FoxfireGame.Instance) {}

        public override void Draw(GameTime gameTime) {
            this._manager.Draw(gameTime, FoxfireGame.SpriteBatch);

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime) {
            this._manager.Update(gameTime);

            base.Update(gameTime);
        }
    }
}

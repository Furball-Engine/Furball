using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Microsoft.Xna.Framework;

namespace Furball.Engine.Engine {
    public class Screen : DrawableGameComponent {
        protected DrawableManager Manager;
        public Screen() : base(FurballGame.Instance) {}

        /// <summary>
        /// You MUST run base.Initialize before adding things to your manager!!!!
        /// </summary>
        public override void Initialize() {
            this.Manager = new();
            
            base.Initialize();
        }
        
        public override void Draw(GameTime gameTime) {
            this.Manager.Draw(gameTime, FurballGame.DrawableBatch);

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime) {
            this.Manager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Dispose(bool disposing) {
            this.Manager.Dispose(disposing);
            
            base.Dispose(disposing);
        }
    }
}

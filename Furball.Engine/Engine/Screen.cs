using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Vixie;


namespace Furball.Engine.Engine {
    public class Screen : GameComponent {
        protected DrawableManager Manager;
        public Screen() {}

        /// <summary>
        /// You MUST run base.Initialize before adding things to your manager!!!!
        /// </summary>
        public override void Initialize() {
            this.Manager = new DrawableManager() {
                Name = this.GetType().Name + "'s DrawableManager"
            };
            
            base.Initialize();
        }
        
        public override void Draw(double gameTime) {
            this.Manager?.Draw(gameTime, FurballGame.DrawableBatch);

            base.Draw(gameTime);
        }

        public override void Update(double gameTime) {
            this.Manager?.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Dispose() {
            this.Manager?.Dispose();
            
            base.Dispose();
        }
    }
}

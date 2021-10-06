using Furball.Engine.Engine.Graphics;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Microsoft.Xna.Framework;

namespace Furball.Engine.Engine.Debug {
    public class DebugCounter : ManagedDrawable {
        private TextDrawable _textDrawable = new TextDrawable(new Vector2(0, 700), FurballGame.DEFAULT_FONT, "a", 24);

        private Vector2 _size;
        public override Vector2 Size => this._size * this.Scale;

        private double _lastUpdatedFramerate          = 0;
        private double _lastUpdatedUpdaterate         = 0;
        private int    _lastUpdatedManagedDrawables   = 0;
        private int    _lastUpdatedUnmanagedDrawables = 0;

        private double _frameDeltaTime  = 0;
        private int    _frames          = 0;
        private double _updateDeltaTime = 0;
        private int    _updates         = 0;

        public DebugCounter() {
            this.Position = new Vector2(0, 720 - this._textDrawable.Size.Y);
        }

        public override void Draw(GameTime time, DrawableBatch batch, DrawableManagerArgs args) {
            this._frames++;
            this._frameDeltaTime += time.ElapsedGameTime.TotalSeconds;

            this._textDrawable.Draw(time, batch, args);
        }

        public override void Update(GameTime time) {
            this._updates++;
            this._updateDeltaTime += time.ElapsedGameTime.TotalSeconds;

            if (this._frameDeltaTime >= 1.0) {
                this._lastUpdatedFramerate = this._frames;
                this._frameDeltaTime       = 0;
                this._frames               = 0;

                this._lastUpdatedManagedDrawables   = 0;
                this._lastUpdatedUnmanagedDrawables = 0;

                for (int i = 0; i != DrawableManager.DrawableManagers.Count; i++) {
                    var current = DrawableManager.DrawableManagers[i];

                    this._lastUpdatedManagedDrawables   += current.CountManaged;
                    this._lastUpdatedUnmanagedDrawables += current.CountUnmanaged;
                }
            }

            if (this._updateDeltaTime >= 1.0) {
                this._lastUpdatedUpdaterate = this._updates;
                this._updateDeltaTime       = 0;
                this._updates               = 0;
            }

            this._textDrawable.Text = string.Format(
                "fps: {0} ({1:N2}ms); ups: {2} ({3:N2}ms); dmi: {4}; ud/md: {5}/{6};",

                this._lastUpdatedFramerate,
                (1000.0 / this._lastUpdatedFramerate),
                this._lastUpdatedUpdaterate,
                (1000.0 / this._lastUpdatedUpdaterate),
                DrawableManager.Instances,
                this._lastUpdatedUnmanagedDrawables,
                this._lastUpdatedManagedDrawables
            );

            this._size = this._textDrawable.Size;
        }
    }
}

using System.Collections.Generic;
using System.Numerics;
using Furball.Engine.Engine.Debug.DebugCounter.Items;
using Furball.Engine.Engine.Graphics;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.Managers;


namespace Furball.Engine.Engine.Debug.DebugCounter {
    /// <summary>
    /// Debug Counter which is displayed on the bottom left corner
    /// </summary>
    public class DebugCounter : ManagedDrawable {
        private TextDrawable _textDrawable = new(new Vector2(0, 700), FurballGame.DEFAULT_FONT, "a", 24);

        private Vector2 _size;
        public override Vector2 Size => this._size * this.Scale;
        /// <summary>
        /// Items which will be displayed on the Counter
        /// </summary>
        public List<DebugCounterItem> Items = new() {
            new FrameRate(),
            new UpdateRate(),
            new DrawableManagerStats(),
            new GameTimeSourceTime(),
            new BoundByDrawUpdate(),
            new ContentCacheItems(),
#if DESKTOP
            //If Android ever gets implemented, add new item called MousePoints and have it display all current cursors
            new MousePosition(),
#endif
            new KeyboardInputs(),
            new DrawableBatchType()
        };

        public DebugCounter() {
            this.Position = new Vector2(0, 720 - this._textDrawable.Size.Y);
        }

        public override void Draw(double time, DrawableBatch batch, DrawableManagerArgs args) {
            for (int i = 0; i != this.Items.Count; i++) {
                this.Items[i].Draw(time);
            }

            this._textDrawable.Draw(time, batch, args);
        }

        public override void Update(double time) {
            string finalText = "";

            for (int i = 0; i != this.Items.Count; i++) {
                DebugCounterItem current = this.Items[i];

                current.Update(time);

                if (i % 3 == 0 && i != 0)
                    finalText += "\n";

                finalText += current.GetAsString(time) + "; ";

                if (current.ForceNewLine)
                    finalText += "\n";
            }

            this._textDrawable.Text = finalText;
            this._size              = this._textDrawable.Size;
            this.Position           = new Vector2(0, 720 - this._textDrawable.Size.Y);
        }
    }
}

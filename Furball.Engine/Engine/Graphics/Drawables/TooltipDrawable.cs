using Furball.Engine.Engine.Graphics.Drawables.Primitives;
using Microsoft.Xna.Framework;

namespace Furball.Engine.Engine.Graphics.Drawables {
    public class TooltipDrawable : CompositeDrawable {
        private readonly TextDrawable               _textDrawable;
        private readonly RectanglePrimitiveDrawable _backgroundRect;

        public TooltipDrawable() {
            this._backgroundRect = new(new(0), new(100, 20), 2, true) {
                ColorOverride = new(0, 0, 0, 155)
            };
            this._textDrawable = new(new(0), FurballGame.DEFAULT_FONT, "", 20) {
                ColorOverride = Color.White
            };

            this.Drawables.Add(this._backgroundRect);
            this.Drawables.Add(this._textDrawable);

            this.Clickable   = false;
            this.CoverClicks = false;
            this.Hoverable   = false;
            this.CoverHovers = false;
        }

        public void SetTooltip(string text) {
            this._textDrawable.Text       = text;
            this._backgroundRect.RectSize = this._textDrawable.Size;
        }
    }
}

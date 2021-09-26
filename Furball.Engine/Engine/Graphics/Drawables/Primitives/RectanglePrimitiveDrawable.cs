using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Furball.Engine.Engine.Graphics.Drawables.Primitives {
    public class RectanglePrimitiveDrawable : ManagedDrawable {
        public bool    Filled;
        public Vector2 RectSize;
        public float   Thickness;
        public override Vector2 Size => this.RectSize;

        public RectanglePrimitiveDrawable(Vector2 position, Vector2 size, float thickness, bool filled) {
            this.Position  = position;
            this.RectSize  = size;
            this.Thickness = thickness;
            this.Filled    = filled;
        }
        
        public override void Draw(GameTime time, SpriteBatch batch, DrawableManagerArgs args) {
            if(this.Filled)
                batch.FillRectangle(args.Position - args.Origin, this.RectSize, args.Color, args.LayerDepth);
            else
                batch.DrawRectangle(args.Position - args.Origin, this.RectSize, args.Color, this.Thickness, args.LayerDepth);
        }
    }
}

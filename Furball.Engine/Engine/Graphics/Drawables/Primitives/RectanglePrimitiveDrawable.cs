using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Furball.Engine.Engine.Graphics.Drawables.Primitives {
    public class RectanglePrimitiveDrawable : ManagedDrawable {
        public bool    Filled;
        public Vector2 RectSize;
        public float   Thickness;
        
        public override void Draw(GameTime time, SpriteBatch batch, DrawableManagerArgs args) {
            if(this.Filled)
                batch.FillRectangle(args.Position - args.Origin, this.RectSize, args.Color, args.LayerDepth);
            else
                batch.DrawRectangle(args.Position - args.Origin, this.RectSize, args.Color, this.Thickness, args.LayerDepth);
        }
    }
}

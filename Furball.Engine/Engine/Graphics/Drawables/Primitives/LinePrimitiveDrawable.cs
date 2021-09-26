using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Furball.Engine.Engine.Graphics.Drawables.Primitives {
    public class LinePrimitiveDrawable : ManagedDrawable {
        public float Length;
        public float Angle;
        public float Thickness;
        
        public LinePrimitiveDrawable(float length, float angle) {
            this.Length = length;
            this.Angle  = angle;
        }
        
        public override void Draw(GameTime time, SpriteBatch batch, DrawableManagerArgs args) {
            batch.DrawLine(args.Position, this.Length, this.Angle, args.Color, this.Thickness, args.LayerDepth);
        }
    }
}

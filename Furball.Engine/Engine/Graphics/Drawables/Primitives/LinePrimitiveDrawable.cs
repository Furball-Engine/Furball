using System;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Furball.Engine.Engine.Graphics.Drawables.Primitives {
    public class LinePrimitiveDrawable : ManagedDrawable {
        public float Length;
        public float Angle;
        public float Thickness;

        public override Vector2 Size => new((float)Math.Cos(this.Angle) * this.Length, (float)Math.Sin(this.Angle) * this.Length);

        public LinePrimitiveDrawable(Vector2 position, float length, float angle, float thickness) {
            this.Position  = position;
            this.Thickness = thickness;
            this.Length    = length;
            this.Angle     = angle;
        }
        
        public override void Draw(GameTime time, SpriteBatch batch, DrawableManagerArgs args) {
            batch.DrawLine(args.Position - args.Origin, this.Length, this.Angle, args.Color, this.Thickness, args.LayerDepth);
        }
    }
}

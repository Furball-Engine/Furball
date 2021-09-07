using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Furball.Engine.Engine.Drawables {
    public abstract class BaseDrawable {
        public Vector2       Position;
        public Vector2       Size;
        public Color         ColorOverride;
        public float         Rotation;
        public float         Scale;
        public SpriteEffects SpriteEffect;
    }
}

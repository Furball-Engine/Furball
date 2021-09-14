using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Furball.Engine.Engine.Graphics.Drawables.Managers {
    public class DrawableManagerArgs {
        public Vector2       Position;
        public Color         Color;
        public float         Rotation;
        public Vector2       Origin;
        public Vector2       Scale;
        public SpriteEffects Effects;
        public float         LayerDepth;
    }
}

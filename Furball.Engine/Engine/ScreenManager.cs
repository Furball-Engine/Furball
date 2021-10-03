using Furball.Engine.Engine.Graphics;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Microsoft.Xna.Framework;

namespace Furball.Engine.Engine {
    public class ScreenManager {
        internal static Transition? Transition;

        internal static void DrawTransition(GameTime time, DrawableBatch batch, DrawableManagerArgs args = null) {
            Transition?.Draw(time, batch, args);
        }

        internal static void UpdateTransition(GameTime time) {
            Transition?.Update(time);
        }

        public static void ChangeScreen(Screen newScreen) {
            Transition?.TransitionBegin();

            FurballGame.Instance.ChangeScreen(newScreen);

            Transition?.TransitionEnd();
        }

        public static void SetTransition(Transition transition) => Transition = transition;
        public static void SetBlankTransition() => Transition = null;
    }
}

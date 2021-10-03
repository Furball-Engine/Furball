using Furball.Engine.Engine.Graphics;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Microsoft.Xna.Framework;

namespace Furball.Engine.Engine {
    public class ScreenManager {
        /// <summary>
        /// Transition that's currently used, by default it is FadeTransition
        /// </summary>
        internal static Transition? Transition;
        /// <summary>
        /// Calls the Transition Draw Method
        /// </summary>
        /// <param name="time"></param>
        /// <param name="batch"></param>
        /// <param name="args"></param>
        internal static void DrawTransition(GameTime time, DrawableBatch batch, DrawableManagerArgs args = null) {
            Transition?.Draw(time, batch, args);
        }
        /// <summary>
        /// Calls the Transition Update Method
        /// </summary>
        /// <param name="time"></param>
        internal static void UpdateTransition(GameTime time) {
            Transition?.Update(time);
        }
        /// <summary>
        /// Changes the Screen to a new Screen
        /// </summary>
        /// <param name="newScreen">Screen to Switch to</param>
        public static void ChangeScreen(Screen newScreen) {
            Transition?.TransitionBegin();

            FurballGame.Instance.ChangeScreen(newScreen);

            Transition?.TransitionEnd();
        }
        /// <summary>
        /// Sets the Transition Effect to something else
        /// </summary>
        /// <param name="transition">New Transition Effect</param>
        public static void SetTransition(Transition transition) => Transition = transition;
        /// <summary>
        /// Sets the Transition effect to literally nothing, No Fading or anything will happen
        /// </summary>
        public static void SetBlankTransition() => Transition = null;
    }
}

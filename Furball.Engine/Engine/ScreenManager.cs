using System.Threading;
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
        /// Current Fade State
        /// </summary>
        public static FadeState CurrentFadeState { get; private set; } = FadeState.None;
        /// <summary>
        /// Lock for CurrentFadeState because Threads...
        /// </summary>
        private static object _fadeLock = new object();
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
            if (Transition != null) {
                lock (_fadeLock) {
                    int fadeInTime = Transition.TransitionBegin();

                    CurrentFadeState = FadeState.FadeIn;

                    Thread fadeWaitThread = new(() => {
                        Thread.Sleep(fadeInTime);

                        FurballGame.Instance.ChangeScreen(newScreen);

                        int fadeOutTime = Transition.TransitionEnd();

                        Thread.Sleep(fadeOutTime);

                        CurrentFadeState = FadeState.FadeOut;
                    }) {
                        Priority = ThreadPriority.BelowNormal
                    };

                    fadeWaitThread.Start();
                }
            } else {
                FurballGame.Instance.ChangeScreen(newScreen);
            }
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

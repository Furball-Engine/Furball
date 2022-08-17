#nullable enable
using System;
using Furball.Engine.Engine.Graphics;
using Furball.Engine.Engine.Graphics.Drawables.Managers;

namespace Furball.Engine.Engine; 

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
    private static object _fadeLock = new();
    /// <summary>
    /// Calls the Transition Draw Method
    /// </summary>
    /// <param name="time"></param>
    /// <param name="batch"></param>
    /// <param name="args"></param>
    internal static void DrawTransition(double time, DrawableBatch batch, DrawableManagerArgs args = null!) {
        Transition?.Draw(time, batch, args);
    }
    /// <summary>
    /// Calls the Transition Update Method
    /// </summary>
    /// <param name="time"></param>
    internal static void UpdateTransition(double time) {
        Transition?.Update(time);
    }
    /// <summary>
    /// Changes the Screen to a new Screen
    /// </summary>
    /// <param name="newScreen">Screen to Switch to</param>
    public static void ChangeScreen(Screen newScreen, bool skipTransition = false) {
        if (Transition != null && !skipTransition) {
            lock (_fadeLock) {
                Transition t = Transition;
                    
                double fadeInTime = Transition.TransitionBegin();

                CurrentFadeState = FadeState.FadeIn;

                FurballGame.GameTimeScheduler.ScheduleMethod(delegate {
                    GC.Collect();
                    
                    FurballGame.Instance.ChangeScreen(newScreen);

                    double fadeOutTime = t.TransitionEnd();
                        
                    CurrentFadeState = FadeState.FadeOut;
                    
                    FurballGame.GameTimeScheduler.ScheduleMethod(delegate {
                        CurrentFadeState = FadeState.None;
                    }, FurballGame.Time + fadeOutTime);
                }, FurballGame.Time + fadeInTime);
            }
        } else {
            FurballGame.Instance.ChangeScreen(newScreen);
        }
    }
    /// <summary>
    /// Sets the Transition Effect to something else
    /// </summary>
    /// <param name="transition">New Transition Effect</param>
    public static void SetTransition(Transition transition) {
        Transition?.Dispose();
            
        Transition = transition;
    }
    /// <summary>
    /// Sets the Transition effect to literally nothing, No Fading or anything will happen
    /// </summary>
    public static void SetBlankTransition() {
        Transition?.Dispose();

        Transition = null;
    }
}
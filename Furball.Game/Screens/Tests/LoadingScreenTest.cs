using System.Diagnostics;
using System.Numerics;
using System.Threading;
using Furball.Engine;
using Furball.Engine.Engine.Graphics.Drawables;

namespace Furball.Game.Screens.Tests; 

public class LoadingScreenTest : TestScreen {
    public override bool RequireLoadingScreen => true;

    public override void BackgroundInitialize() {
        this.LoadingStatus = "Wasting CPU Cycles...";

        Stopwatch stopwatch = new();
        stopwatch.Start();

        const float miliTotal = 5000;
            
        while (stopwatch.Elapsed.TotalMilliseconds < miliTotal) {
            this.LoadingProgress = (float)(stopwatch.Elapsed.TotalMilliseconds / miliTotal);
            Thread.Sleep(1);
        }

        this.LoadingComplete = true;
        stopwatch.Stop();
    }

    public override void Initialize() {
        base.Initialize();

        this.Manager.Add(new TextDrawable(new Vector2(10), FurballGame.DefaultFontStroked, "Loading Complete!", 30));
    }
}
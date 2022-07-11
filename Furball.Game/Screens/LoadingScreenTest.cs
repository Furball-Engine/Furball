using System.Diagnostics;
using System.Numerics;
using Furball.Engine.Engine;
using System.Threading;
using Furball.Engine;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Vixie.Backends.Shared;

namespace Furball.Game.Screens {
    public class LoadingScreenTest : Screen {
        public override bool RequireLoadingScreen => true;

        public override void BackgroundInitialize() {
            LoadingStatus = "Wasting CPU Cycles...";

            Stopwatch stopwatch = new();
            stopwatch.Start();

            const float miliTotal = 5000;
            
            while (stopwatch.Elapsed.TotalMilliseconds < miliTotal) {
                LoadingProgress = (float)(stopwatch.Elapsed.TotalMilliseconds / miliTotal);
                Thread.Sleep(1);
            }
            
            LoadingComplete = true;
            stopwatch.Stop();
        }

        public override void Initialize() {
            base.Initialize();
            
            this.Manager.Add(
            new TexturedDrawable(FurballGame.WhitePixel, Vector2.Zero) {
                ColorOverride = new Color(0x80, 0x00, 0x80),
                Scale         = new Vector2(1280, 720),
                Depth         = 1f
            }
            );

            this.Manager.Add(new TextDrawable(new Vector2(10), FurballGame.DEFAULT_FONT_STROKED, "Loading Complete!", 30));
        }
    }
}

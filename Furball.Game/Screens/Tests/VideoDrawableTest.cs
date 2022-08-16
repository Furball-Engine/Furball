using System.IO;
using System.Numerics;
using Furball.Engine;
using Furball.Engine.Engine.Graphics.Drawables;

namespace Furball.Game.Screens.Tests; 

public class VideoDrawableTest : TestScreen {
    private VideoDrawable _video;
    
    public override void Initialize() {
        base.Initialize();

        string videoPath = Path.Combine(FurballGame.AssemblyPath, "Content", "test.mp4");

        this._video = new VideoDrawable(videoPath, 1, FurballGame.GameTimeSource, Vector2.Zero);
        
        this.Manager.Add(this._video);
    }

    public override void Dispose() {
        base.Dispose();
        
        this._video.Dispose();
    }
}

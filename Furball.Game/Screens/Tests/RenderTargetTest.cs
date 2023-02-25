using System.IO;
using System.Numerics;
using Furball.Engine;
using Furball.Engine.Engine.Graphics;
using Furball.Engine.Engine.Graphics.Drawables;
using sowelipisona;

namespace Furball.Game.Screens.Tests; 

public class RenderTargetTest : TestScreen {
    private WaveformDrawable _waveformDrawable;
    private WaveformDrawable _croppedWaveformDrawable;
    
    public override void Initialize() {
        base.Initialize();

        Waveform waveform = FurballGame.AudioEngine.GetWaveform(new MemoryStream(ContentManager.LoadRawAsset("lulkanto.mp3")));

        const float x      = 10;
        const float height = 100;

        this.Manager.Add(this._waveformDrawable        = new WaveformDrawable(new Vector2(x), waveform, height));
        this.Manager.Add(this._croppedWaveformDrawable = new WaveformDrawable(new Vector2(x, height + 10 + x), waveform, height));

        this._croppedWaveformDrawable.StartCrop = 100;
        this._croppedWaveformDrawable.EndCrop   = 200;
    }

    public override void Draw(double gameTime) {
        base.Draw(gameTime);
    }
}

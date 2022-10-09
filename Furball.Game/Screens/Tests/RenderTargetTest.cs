using System.IO;
using System.Numerics;
using Furball.Engine;
using Furball.Engine.Engine.Graphics;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Vixie;
using Furball.Vixie.Backends.Shared;
using sowelipisona;

namespace Furball.Game.Screens.Tests; 

public class RenderTargetTest : TestScreen {
    private WaveformDrawable _waveformDrawable;
    private WaveformDrawable _croppedWaveformDrawable;
    // private RenderTarget _texture;
    
    public override void Initialize() {
        base.Initialize();

        Waveform waveform = FurballGame.AudioEngine.GetWaveform(new MemoryStream(ContentManager.LoadRawAsset("lulkanto.mp3")));

        const float height = 100;
        
        // RawWaveformDrawable drawable = new RawWaveformDrawable(waveform, height);
        //
        // //Create the render target
        // RenderTarget renderTarget = new RenderTarget((uint)drawable.Size.X, (uint)drawable.Size.Y);
        //
        // //Create the batch we will use for rendering
        // DrawableBatch batch = new DrawableBatch();
        //
        // //Bind the render target
        // renderTarget.Bind();
        //
        // //Clear the render target
        // GraphicsBackend.Current.Clear();
        //
        // //Begin the batch
        // batch.Begin();
        //
        // //Draw the waveform
        // drawable.Draw(0, batch, new DrawableManagerArgs {
        //     Position = Vector2.Zero,
        //     Scale    = Vector2.One,
        //     Rotation = 0,
        //     Color    = Color.White, 
        //     Effects = TextureFlip.None
        // });
        //
        // //End the batch
        // batch.SoftEnd();
        //
        // //Draw the batch
        // batch.ManualDraw();
        //
        // //Unbind the render target
        // renderTarget.Unbind();
        //
        // this._texture = renderTarget;
        
        this.Manager.Add(this._waveformDrawable        = new WaveformDrawable(Vector2.Zero, waveform, height));
        this.Manager.Add(this._croppedWaveformDrawable = new WaveformDrawable(new Vector2(0, height), waveform, height));

        this._croppedWaveformDrawable.StartCrop = 100;
        this._croppedWaveformDrawable.EndCrop   = 200;
    }

    public override void Draw(double gameTime) {
        base.Draw(gameTime);
        
        //Draw the render target
        // FurballGame.DrawableBatch.Draw(this._texture, Vector2.Zero);
    }
}

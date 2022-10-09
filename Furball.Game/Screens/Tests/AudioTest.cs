using System.IO;
using System.Numerics;
using Furball.Engine;
using Furball.Engine.Engine.Graphics;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Graphics.Drawables.UiElements;
using Furball.Engine.Engine.Input.Events;
using Furball.Engine.Engine.Timing;
using Furball.Vixie;
using Furball.Vixie.Backends.Shared;
using Furball.Vixie.Helpers;
using ManagedBass;
using sowelipisona;
using sowelipisona.Effects;

namespace Furball.Game.Screens.Tests; 

public class AudioTest : TestScreen {
    private AudioStream _testingStream;

    private TextDrawable     _topText;
    private RawWaveformDrawable _rawWaveform;

    public override void Initialize() {
        base.Initialize();

        this._topText = new TextDrawable(new Vector2(1280f / 2f, 40), FurballGame.DefaultFont, "Audio testing!", 48) {
            OriginType = OriginType.Center
        };

        this.Manager.Add(this._topText);

        DrawableButton playButton = new(
        new Vector2(50, 50),
        FurballGame.DefaultFont,
        28,
        "Play Audio normally",
        Color.White,
        Color.Black,
        Color.Black,
        new Vector2(350, 40),
        this.PlayButtonOnClick
        );

        this.Manager.Add(playButton);

        TextDrawable effectText = new(new Vector2(190, 100), FurballGame.DefaultFont, "Effects:", 28);

        this.Manager.Add(effectText);

        int currentY = 130;

        DrawableButton lowPassPlayButton = new(
        new Vector2(50, currentY),
        FurballGame.DefaultFont,
        28,
        "Play Audio with Low Pass filter",
        Color.White,
        Color.Black,
        Color.Black,
        new Vector2(350, 40),
        this.LowPassPlayOnClick
        );

        this.Manager.Add(lowPassPlayButton);

        currentY += 60;

        DrawableButton highPassPlayButton = new(
        new Vector2(50, currentY),
        FurballGame.DefaultFont,
        28,
        "Play Audio with High Pass filter",
        Color.White,
        Color.Black,
        Color.Black,
        new Vector2(350, 40),
        this.HighPassPlayOnClick
        );

        this.Manager.Add(highPassPlayButton);

        currentY += 60;

        DrawableButton reverbPlayButton = new(
        new Vector2(50, currentY),
        FurballGame.DefaultFont,
        28,
        "Play Audio with Reverb",
        Color.White,
        Color.Black,
        Color.Black,
        new Vector2(350, 40),
        this.ReverbPlayOnClick
        );

        currentY += 60;

        this.Manager.Add(reverbPlayButton);

        DrawableButton generateWaveform = new DrawableButton(
            new Vector2(50, currentY),
            FurballGame.DefaultFont,
            28,
            "Generate a waveform of the audio",
            Color.White,
            Color.Black,
            Color.Black,
            Vector2.Zero, 
            GenerateWaveform
        );
        
        this.Manager.Add(generateWaveform);
    }
    
    private void GenerateWaveform(object sender, MouseButtonEventArgs e) {
        MemoryStream stream = new MemoryStream(ContentManager.LoadRawAsset("lulkanto.mp3"));

        Waveform waveform = FurballGame.AudioEngine.GetWaveform(stream);
        
        Guard.EnsureNonNull(waveform.Points);

        const int texHeight = 100;
        
        this.Manager.Add(this._rawWaveform = new RawWaveformDrawable(waveform, texHeight) {
            Scale = new Vector2(0.25f, 1)
        });
    }

    private void ReverbPlayOnClick(object sender, MouseButtonEventArgs mouseButtonEventArgs) {
        if (this._testingStream?.PlaybackState == PlaybackState.Playing) {
            this._testingStream.Stop();
        }

        this._testingStream = FurballGame.AudioEngine.CreateStream(ContentManager.LoadRawAsset("lulkanto.mp3"));

        ReverbAudioEffect effect = FurballGame.AudioEngine.CreateReverbEffect(this._testingStream);

        effect.Apply();

        this._testingStream.Volume = 0.4;
        this._testingStream.Play();
    }

    public override void Relayout(float newWidth, float newHeight) {
        base.Relayout(newWidth, newHeight);

        if(this._topText != null)
            this._topText.Position.X = newWidth / 2f;
    }

    private void LowPassPlayOnClick(object sender, MouseButtonEventArgs mouseButtonEventArgs) {
        if (this._testingStream?.PlaybackState == PlaybackState.Playing) {
            this._testingStream.Stop();
        }

        this._testingStream = FurballGame.AudioEngine.CreateStream(ContentManager.LoadRawAsset("lulkanto.mp3"));

        LowPassFilterAudioEffect effect = FurballGame.AudioEngine.CreateLowPassFilterEffect(this._testingStream);

        effect.Apply();

        effect.FrequencyCutoff = 200;

        this._testingStream.Volume = 0.4;
        this._testingStream.Play();
    }

    private void HighPassPlayOnClick(object sender, MouseButtonEventArgs mouseButtonEventArgs) {
        if (this._testingStream?.PlaybackState == PlaybackState.Playing)
            this._testingStream.Stop();

        this._testingStream = FurballGame.AudioEngine.CreateStream(ContentManager.LoadRawAsset("lulkanto.mp3"));

        HighPassFilterAudioEffect effect = FurballGame.AudioEngine.CreateHighPassFilterEffect(this._testingStream);

        effect.Apply();

        this._testingStream.Volume = 0.4;
        this._testingStream.Play();
    }

    private void PlayButtonOnClick(object sender, MouseButtonEventArgs mouseButtonEventArgs) {
        if (this._testingStream?.PlaybackState == PlaybackState.Playing) {
            this._testingStream.Stop();
        }

        this._testingStream        = FurballGame.AudioEngine.CreateStream(ContentManager.LoadRawAsset("lulkanto.mp3"));
        this._testingStream.Volume = 0.4;
        this._testingStream.Play();

        if(this._rawWaveform != null)
            this._rawWaveform.TimeSource = new AudioStreamTimeSource(this._testingStream);
    }
}
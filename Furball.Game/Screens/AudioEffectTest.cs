using System.Drawing;
using System.Numerics;
using Furball.Engine;
using Furball.Engine.Engine;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.UiElements;
using ManagedBass;
using Silk.NET.Input;
using sowelipisona;
using sowelipisona.Effects;
using Color=Furball.Vixie.Backends.Shared.Color;

namespace Furball.Game.Screens; 

public class AudioEffectTest : TestScreen {
    private AudioStream _testingStream;

    private DrawableTextBox _filenameTextBox;

    public override void Initialize() {
        base.Initialize();

        TextDrawable topText = new TextDrawable(new Vector2(1280f / 2f, 40), FurballGame.DEFAULT_FONT, "Audio Effects testing!!!", 48) {
            OriginType = OriginType.Center
        };

        this.Manager.Add(topText);

        DrawableTextBox filenameTextBox = new DrawableTextBox(new Vector2(50, 80), FurballGame.DEFAULT_FONT, 28, 350, "audio.mp3");

        this._filenameTextBox = filenameTextBox;

        this.Manager.Add(filenameTextBox);

        DrawableButton playButton = new DrawableButton(
        new Vector2(50, 130),
        FurballGame.DEFAULT_FONT,
        28,
        "Play Audio normally",
        Color.White,
        Color.Black,
        Color.Black,
        new Vector2(350, 40),
        PlayButtonOnClick
        );

        this.Manager.Add(playButton);

        TextDrawable effectText = new TextDrawable(new Vector2(190, 180), FurballGame.DEFAULT_FONT, "Effects", 28);

        this.Manager.Add(effectText);

        int currentY = 220;

        DrawableButton lowPassPlayButton = new DrawableButton(
        new Vector2(50, currentY),
        FurballGame.DEFAULT_FONT,
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
        FurballGame.DEFAULT_FONT,
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

        DrawableButton reverbPlayButton = new DrawableButton(
        new Vector2(50, currentY),
        FurballGame.DEFAULT_FONT,
        28,
        "Play Audio with Reverb",
        Color.White,
        Color.Black,
        Color.Black,
        new Vector2(350, 40), ReverbPlayOnClick
        );

        this.Manager.Add(reverbPlayButton);
    }

    private void ReverbPlayOnClick(object sender, (MouseButton, Point) e) {
        if (this._testingStream?.PlaybackState == PlaybackState.Playing) {
            this._testingStream.Stop();
        }

        this._testingStream = FurballGame.AudioEngine.CreateStream(this._filenameTextBox.Text);

        ReverbAudioEffect effect = FurballGame.AudioEngine.CreateReverbEffect(this._testingStream);

        effect.Apply();

        this._testingStream.Volume = 0.4;
        this._testingStream.Play();
    }

    private void LowPassPlayOnClick(object sender, (MouseButton, Point) e) {
        if (this._testingStream?.PlaybackState == PlaybackState.Playing) {
            this._testingStream.Stop();
        }

        this._testingStream = FurballGame.AudioEngine.CreateStream(this._filenameTextBox.Text);

        LowPassFilterAudioEffect effect = FurballGame.AudioEngine.CreateLowPassFilterEffect(this._testingStream);

        effect.Apply();

        effect.FrequencyCutoff = 200;

        this._testingStream.Volume = 0.4;
        this._testingStream.Play();
    }

    private void HighPassPlayOnClick(object sender, (MouseButton, Point) e) {
        if (this._testingStream?.PlaybackState == PlaybackState.Playing)
            this._testingStream.Stop();

        this._testingStream = FurballGame.AudioEngine.CreateStream(this._filenameTextBox.Text);

        HighPassFilterAudioEffect effect = FurballGame.AudioEngine.CreateHighPassFilterEffect(this._testingStream);

        effect.Apply();

        this._testingStream.Volume = 0.4;
        this._testingStream.Play();
    }

    private void PlayButtonOnClick(object sender, (MouseButton, Point) e) {
        if (this._testingStream?.PlaybackState == PlaybackState.Playing) {
            this._testingStream.Stop();
        }

        this._testingStream        = FurballGame.AudioEngine.CreateStream(this._filenameTextBox.Text);
        this._testingStream.Volume = 0.4;
        this._testingStream.Play();
    }
}
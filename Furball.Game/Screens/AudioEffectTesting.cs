using System.Drawing;
using System.Numerics;
using Furball.Engine;
using Furball.Engine.Engine;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.UiElements;
using ManagedBass;
using ManagedBass.DirectX8;
using ManagedBass.Fx;
using Silk.NET.Input;
using sowelipisona;
using Color=Furball.Vixie.Backends.Shared.Color;

namespace Furball.Game.Screens {
    public class AudioEffectTesting : Screen {
        private AudioStream _testingStream;

        private DrawableTextBox _filenameTextBox;

        public override void Initialize() {
            base.Initialize();

            TexturedDrawable background = new TexturedDrawable(FurballGame.WhitePixel, Vector2.Zero) {
                ColorOverride = new Color(0x80, 0x00, 0x80),
                Scale         = new Vector2(1280, 720),
                Depth         = 1f
            };

            this.Manager.Add(background);

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
                LowPassPlayOnClick
            );

            this.Manager.Add(lowPassPlayButton);

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

            int reverbFxHandle = Bass.ChannelSetFX(this._testingStream.Handle, EffectType.DXReverb, 1);

            DXReverbParameters reverbParameters = new DXReverbParameters() {
                fHighFreqRTRatio = 0.001f,
                fInGain          = 0,
                fReverbMix       = -5,
                fReverbTime      = 3000,
            };

            bool succeded = Bass.FXSetParameters(reverbFxHandle, reverbParameters);

            if (succeded) {
                this._testingStream.Volume = 0.4;
                this._testingStream.Play();
            }
        }

        private void LowPassPlayOnClick(object sender, (MouseButton, Point) e) {
            if (this._testingStream?.PlaybackState == PlaybackState.Playing) {
                this._testingStream.Stop();
            }

            this._testingStream = FurballGame.AudioEngine.CreateStream(this._filenameTextBox.Text);

            int lowPassHandle = Bass.ChannelSetFX(this._testingStream.Handle, EffectType.BQF, 1);

            BQFParameters lowPassParams = new BQFParameters() {
                fBandwidth = 0,
                fCenter    = 200,
                lFilter    = BQFType.LowPass,
                fQ         = 1f,
            };

            bool succeded = Bass.FXSetParameters(lowPassHandle, lowPassParams);

            if (succeded) {
                this._testingStream.Volume = 0.4;
                this._testingStream.Play();
            }
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
}

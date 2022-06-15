using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using Furball.Engine;
using Furball.Engine.Engine;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.Tweens;
using Furball.Engine.Engine.Graphics.Drawables.Tweens.TweenTypes;
using Furball.Engine.Engine.Graphics.Drawables.UiElements;
using ManagedBass;
using ManagedBass.Fx;
using Silk.NET.Input;
using sowelipisona;
using Color=Furball.Vixie.Backends.Shared.Color;

namespace Furball.Game.Screens {
    public class AudioEffectTesting : Screen {
        private AudioStream _testingStream;
        private int         _lowPassFxHandle;
        private FloatTween       _frequencyTween;

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
        }
        private void LowPassPlayOnClick(object sender, (MouseButton, Point) e) {
            if (this._testingStream?.PlaybackState == PlaybackState.Playing) {
                this._testingStream.Stop();
            }

            this._testingStream = FurballGame.AudioEngine.CreateStream(this._filenameTextBox.Text);

            int lowPassHandle = Bass.ChannelSetFX(this._testingStream.Handle, EffectType.BQF, 1);
            this._lowPassFxHandle = lowPassHandle;

            BQFParameters lowPassParams = new BQFParameters() {
                fBandwidth = 0,
                fCenter    = 22049,
                lFilter    = BQFType.LowPass,
                fQ         = 1f,
            };

            bool succeded = Bass.FXSetParameters(lowPassHandle, lowPassParams);

            if (succeded) {
                this._testingStream.Volume = 0.4;
                this._testingStream.Play();
                this._testingStream.CurrentPosition = 27500;
            }

            this._frequencyTween = new FloatTween(TweenType.Fade, 22049, 200, FurballGame.Time, FurballGame.Time + 2500);
        }

        private void PlayButtonOnClick(object sender, (MouseButton, Point) e) {
            if (this._testingStream?.PlaybackState == PlaybackState.Playing) {
                this._testingStream.Stop();
            }

            this._testingStream        = FurballGame.AudioEngine.CreateStream(this._filenameTextBox.Text);
            this._testingStream.Volume = 0.4;
            this._testingStream.Play();
        }

        public override void Update(double gameTime) {
            this._frequencyTween?.Update(FurballGame.Time);

            if (this._frequencyTween != null && !this._frequencyTween.Terminated) {
                Stopwatch stopwatch = Stopwatch.StartNew();

                BQFParameters lowPassParams = new BQFParameters() {
                    fBandwidth = 0,
                    fCenter    = this._frequencyTween.GetCurrent(),
                    lFilter    = BQFType.LowPass,
                    fQ         = 1f,
                };

                Bass.FXSetParameters(this._lowPassFxHandle, lowPassParams);

                stopwatch.Stop();

                double ms = ((double) stopwatch.ElapsedTicks / (double) Stopwatch.Frequency) * 1000.0;
            }

            base.Update(gameTime);
        }
    }
}

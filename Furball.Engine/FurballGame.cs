using System;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using Furball.Engine.Engine;
using Furball.Engine.Engine.Audio;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Input;
using Furball.Engine.Engine.Input.InputMethods;
using Furball.Engine.Engine.Platform;
using Furball.Engine.Engine.Timing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Furball.Engine {
    public class FurballGame : Game {
        private GraphicsDeviceManager _graphics;
        private IGameComponent        _running;

        public static Game         Instance;
        public static SpriteBatch  SpriteBatch;
        public static InputManager InputManager;
        public static ITimeSource  GameTimeSource;

        public static TextDrawable DebugFpsDraw;
        public static TextDrawable DebugFpsUpdate;

        public static DrawableManager DrawableManager;
        public static DrawableManager DebugOverlayDrawableManager;
        public static bool            DrawDebugOverlay = true;

        public FurballGame(Screen startScreen) {
            this._graphics             = new GraphicsDeviceManager(this);
            this.Content.RootDirectory = "Content";
            this.IsMouseVisible        = true;

            GameTimeSource = new GameTimeSource();
            Instance       = this;

            this.ChangeScreen(startScreen);
        }

        protected override void Initialize() {
            _stopwatch.Start();

            InputManager = new();
            InputManager.RegisterInputMethod(new MonogameMouseInputMethod());
            InputManager.RegisterInputMethod(new MonogameKeyboardInputMethod());

            if (RuntimeInfo.IsDebug()) {
                InputManager.OnKeyDown += delegate(object _, Keys keys) {
                    if (keys == Keys.F11) DrawDebugOverlay = !DrawDebugOverlay;
                };
            }

            AudioEngine.Initialize(this.Window.Handle);

            DrawableManager             = new();
            DebugOverlayDrawableManager = new();

            _graphics.SynchronizeWithVerticalRetrace = false;
            this.IsFixedTimeStep                     = false;
            this._graphics.ApplyChanges();

            base.Initialize();
        }

        public void ChangeScreen(Screen screen) {
            if (this._running != null) {
                this.Components.Remove(this._running);

                ((GameComponent)this._running).Dispose();
                this._running = null;
            }

            this.Components.Add(screen);
        }

        protected override void LoadContent() {
            SpriteBatch = new SpriteBatch(this.GraphicsDevice);

            this._graphics.PreferredBackBufferWidth  = 1280;
            this._graphics.PreferredBackBufferHeight = 720;
            this._graphics.ApplyChanges();

            if (RuntimeInfo.IsDebug()) {
                // TODO: implement a proper ContentReader
                DebugFpsDraw = new TextDrawable(File.ReadAllBytes(Path.Combine(this.Content.RootDirectory, "default-font.ttf")), "", 50);
                DebugOverlayDrawableManager.Add(DebugFpsDraw);

                DebugFpsUpdate = new TextDrawable(File.ReadAllBytes(Path.Combine(this.Content.RootDirectory, "default-font.ttf")), "", 50) {
                    Position = new Vector2(0, DebugFpsDraw.Height)
                };
                DebugOverlayDrawableManager.Add(DebugFpsUpdate);
            }
        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            if (RuntimeInfo.IsDebug())
                DebugFpsUpdate.Text = $"update fps: {Math.Round(1 / gameTime.ElapsedGameTime.TotalSeconds).ToString(CultureInfo.InvariantCulture)}";

            InputManager.Update();

            DrawableManager.Update(gameTime);
            if (RuntimeInfo.IsDebug())
                DebugOverlayDrawableManager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            this.GraphicsDevice.Clear(Color.Black);

            if (RuntimeInfo.IsDebug())
                DebugFpsDraw.Text = $"draw fps: {Math.Round(1 / gameTime.ElapsedGameTime.TotalSeconds).ToString(CultureInfo.InvariantCulture)}";

            DrawableManager.Draw(gameTime, SpriteBatch);
            if (RuntimeInfo.IsDebug() && DrawDebugOverlay)
                DebugOverlayDrawableManager.Draw(gameTime, SpriteBatch);

            base.Draw(gameTime);
        }

        #region Timing

        private static Stopwatch _stopwatch = new();
        public static int Time => (int)_stopwatch.ElapsedMilliseconds;

        #endregion
    }
}

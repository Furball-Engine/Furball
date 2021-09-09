using System;
using System.Diagnostics;
using Furball.Engine.Engine;
using Furball.Engine.Engine.Input;
using Furball.Engine.Engine.Input.InputMethods;
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

        private SpriteFont _debugSpriteFont;

        public FurballGame(Screen startScreen) {
            this._graphics             = new GraphicsDeviceManager(this);
            this.Content.RootDirectory = "Content";
            this.IsMouseVisible           = true;


            GameTimeSource = new GameTimeSource();
            Instance       = this;

            this.ChangeScreen(startScreen);
        }

        protected override void Initialize() {
            _stopwatch.Start();

            InputManager = new();
            InputManager.RegisterInputMethod(new MonogameMouseInputMethod());
            InputManager.RegisterInputMethod(new MonogameKeyboardInputMethod());

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

            this._debugSpriteFont = this.Content.Load<SpriteFont>("Corbel");
        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            InputManager.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            this.GraphicsDevice.Clear(Color.CornflowerBlue);

            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
            SpriteBatch.DrawString(this._debugSpriteFont, $"Time: {GameTimeSource.GetCurrentTime()}", Vector2.Zero, Color.White);
            SpriteBatch.End();

            base.Draw(gameTime);
        }

        #region Timing

        private static Stopwatch _stopwatch = new Stopwatch();
        public static int Time => (int)_stopwatch.ElapsedMilliseconds;

        #endregion
    }
}

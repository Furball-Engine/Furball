using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using FontStashSharp;
using Furball.Engine.Engine;
using Furball.Engine.Engine.Audio;
using Furball.Engine.Engine.Debug.DebugCounter;
using Furball.Engine.Engine.DevConsole;
using Furball.Engine.Engine.Graphics;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Graphics.Drawables.Tweens;
using Furball.Engine.Engine.Graphics.Drawables.Tweens.TweenTypes;
using Furball.Engine.Engine.Helpers;
using Furball.Engine.Engine.Helpers.Logger;
using Furball.Engine.Engine.Input;
using Furball.Engine.Engine.Input.InputMethods;
using Furball.Engine.Engine.Localization;
using Furball.Engine.Engine.Platform;
using Furball.Engine.Engine.Timing;
using Furball.Engine.Engine.Transitions;
using Kettu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Furball.Engine {
    public class FurballGame : Game {
        private GraphicsDeviceManager _graphics;
        private IGameComponent        _running;

        public static Random Random = new();

        public static FurballGame   Instance;
        public static DrawableBatch DrawableBatch;
        public static InputManager  InputManager;
        public static ITimeSource   GameTimeSource;
        public static Scheduler     GameTimeScheduler;

        public static DebugCounter DebugCounter;

        public static DrawableManager DrawableManager;
        public static DrawableManager DebugOverlayDrawableManager;
        public static bool            DrawDebugOverlay = true;

        public const int DEFAULT_WINDOW_WIDTH  = 1280;
        public const int DEFAULT_WINDOW_HEIGHT = 720;

        public static string AssemblyPath       = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new Exception("shits fucked man");
        public static string LocalizationFolder => $"{Instance.Content.RootDirectory}/Localization";

        public static int WindowHeight => Instance.GraphicsDevice.Viewport.Height;
        public static int WindowWidth => Instance.GraphicsDevice.Viewport.Width;

        public static float HorizontalRatio => (float)WindowWidth / DEFAULT_WINDOW_WIDTH;
        public static float VerticalRatio => (float)WindowHeight / DEFAULT_WINDOW_HEIGHT;
        public static Rectangle DisplayRect => new(0, 0, (int)Math.Ceiling(Instance.GraphicsDevice.Viewport.Width / VerticalRatio), (int)Math.Ceiling(Instance.GraphicsDevice.Viewport.Height / VerticalRatio));
        public static Rectangle DisplayRectActual => new(0, 0, Instance.GraphicsDevice.Viewport.Width, Instance.GraphicsDevice.Viewport.Height);

        public static  ConsoleDrawable ConsoleDrawable;
        private static TextDrawable    _ConsoleAutoComplete;
        public static  TooltipDrawable TooltipDrawable;
        
        public static byte[] DefaultFontData;
        public static readonly FontSystem DEFAULT_FONT = new(new FontSystemSettings {
            FontResolutionFactor = 2f,
            KernelWidth          = 2,
            KernelHeight         = 2,
            Effect               = FontSystemEffect.None
        });
        public static readonly FontSystem DEFAULT_FONT_STROKED = new(new FontSystemSettings {
            FontResolutionFactor = 2f,
            KernelWidth          = 2,
            KernelHeight         = 2,
            Effect               = FontSystemEffect.Stroked,
            EffectAmount         = 2
        });

        public static Texture2D WhitePixel;

        public event EventHandler<Screen> BeforeScreenChange; 
        public event EventHandler<Screen> AfterScreenChange;

        private Screen _startScreen;
        public FurballGame(Screen startScreen) {
            this._graphics = new GraphicsDeviceManager(this) {
                GraphicsProfile     = GraphicsProfile.HiDef,
                PreferMultiSampling = true
            };

            this._graphics.PreparingDeviceSettings += (_, e) => e.GraphicsDeviceInformation.PresentationParameters.MultiSampleCount = 4;
            this._graphics.ApplyChanges();
            
            this.Content.RootDirectory = "Content";
            this.IsMouseVisible        = true;

            GameTimeSource    = new GameTimeSource();
            Instance          = this;
            GameTimeScheduler = new Scheduler();
            
            Logger.AddLogger(new ConsoleLogger());
            Logger.AddLogger(new DevConsoleLogger());

            this._startScreen = startScreen;
        }

        protected override void Initialize() {
            this.InitializeLocalizations();
            
            Logger.Log(
                $@"Starting Furball {(Environment.Is64BitProcess ? "64-bit" : "32-bit")}"+
                $@"on {Environment.OSVersion.VersionString} {(Environment.Is64BitOperatingSystem ? "64-bit" : "32-bit")}",
                LoggerLevelEngineInfo.Instance
            );

            DefaultFontData = ContentManager.LoadRawAsset("default-font.ttf");
            DEFAULT_FONT.AddFont(DefaultFontData);
            DEFAULT_FONT_STROKED.AddFont(DefaultFontData);

            _stopwatch.Start();

            InputManager = new InputManager();
            InputManager.RegisterInputMethod(new MonogameMouseInputMethod());
            InputManager.RegisterInputMethod(new MonogameKeyboardInputMethod());

            if (ConVars.DebugOverlay.Value == 1) {
                InputManager.OnKeyDown += delegate(object _, Keys keys) {
                    if (keys == Keys.F11) DrawDebugOverlay = !DrawDebugOverlay;
                };
            }

            AudioEngine.Initialize(this.Window.Handle);

            DrawableManager             = new DrawableManager();
            DebugOverlayDrawableManager = new DrawableManager();

            #region Console result

            ConsoleDrawable = new ConsoleDrawable();
            DebugOverlayDrawableManager.Add(ConsoleDrawable);

            TextDrawable consoleResult = new(new Vector2(DEFAULT_WINDOW_WIDTH / 2f, DEFAULT_WINDOW_HEIGHT * 0.75f), DEFAULT_FONT, "", 30) {
                OriginType    = OriginType.Center,
                ColorOverride = new Color(255, 255, 255, 0),
                Clickable     = false,
                CoverClicks   = false
            };

            ConsoleDrawable.OnCommandFinished += delegate(object _, ConsoleResult result) {
                consoleResult.Tweens.Clear();
                consoleResult.Tweens.Add(new FloatTween(TweenType.Fade, consoleResult.ColorOverride.A / 255f, 1f, Time,        Time + 100));
                consoleResult.Tweens.Add(new FloatTween(TweenType.Fade, 1f,                                   0f, Time + 4100, Time + 5100));

                consoleResult.Text = result.Message;

                consoleResult.ColorOverride = result.Result switch {
                    ExecutionResult.Success => Color.White,
                    ExecutionResult.Error   => Color.OrangeRed,
                    ExecutionResult.Warning => Color.Orange,
                    _                       => throw new ArgumentOutOfRangeException()
                };
            };

            DebugOverlayDrawableManager.Add(consoleResult);

            #endregion

            _ConsoleAutoComplete = new TextDrawable(new Vector2(DEFAULT_WINDOW_WIDTH / 2f, DEFAULT_WINDOW_HEIGHT * 0.4f), DEFAULT_FONT, "", 30) {
                OriginType    = OriginType.BottomCenter,
                ColorOverride = new Color(255, 255, 255, 0),
                Clickable     = false,
                CoverClicks   = false
            };

            ConsoleDrawable.OnLetterTyped   += this.ConsoleOnLetterTyped;
            ConsoleDrawable.OnLetterRemoved += this.ConsoleOnLetterTyped;

            DebugOverlayDrawableManager.Add(_ConsoleAutoComplete);

            WhitePixel = new Texture2D(this.GraphicsDevice, 1, 1);
            Color[] white = { Color.White };
            WhitePixel.SetData(white);

            LocalizationManager.ReadTranslations();

            TooltipDrawable = new();
            DrawableManager.Add(TooltipDrawable);

            base.Initialize();
        }

        protected override void OnExiting(object sender, EventArgs args) {
            DevConsole.Run(":nt_on_exiting", false, true);
            DevConsole.WriteLog();

            base.OnExiting(sender, args);
        }

        public void SetTargetFps(int fps, double unfocusedScale = -1) {
            if (fps != -1) {
                this.TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0 / (double) fps);
                this.IsFixedTimeStep   = true;
            } else {
                this.TargetElapsedTime = TimeSpan.FromTicks(1);
                this.IsFixedTimeStep   = false;
            }

            if (unfocusedScale != -1) {
                double newFps = fps * unfocusedScale;
                double milliseconds = 1000.0 / (double) newFps;

                this.InactiveSleepTime = TimeSpan.FromMilliseconds(milliseconds);
            }
        }

        private void ConsoleOnLetterTyped(object sender, char e) {
            _ConsoleAutoComplete.Tweens.Clear();
            _ConsoleAutoComplete.Tweens.Add(new FloatTween(TweenType.Fade, _ConsoleAutoComplete.ColorOverride.A / 255f, 1f, Time,        Time + 100));
            _ConsoleAutoComplete.Tweens.Add(new FloatTween(TweenType.Fade, 1f,                                          0f, Time + 2100, Time + 3100));

            string input = ConsoleDrawable.Text;

            if (input.StartsWith(':')) {
                input = input.TrimStart(':');

                IEnumerable<KeyValuePair<string, ConFunc>> functions = DevConsole.RegisteredFunctions.Where(x => x.Key.StartsWith(input));

                string text = "";

                int i = 0;
                foreach (KeyValuePair<string, ConFunc> pair in functions) {
                    if (i == 5) break;
                    text += $"{pair.Key}\n";
                    i++;
                }

                _ConsoleAutoComplete.Text = text.Trim();
            } else {
                IEnumerable<KeyValuePair<string, ConVar>> convars = DevConsole.RegisteredConVars.Where(x => x.Key.StartsWith(input));

                string text = "";

                int i = 0;
                foreach (KeyValuePair<string, ConVar> pair in convars) {
                    if (i == 5) break;
                    text += $"{pair.Key}\n";
                    i++;
                }

                _ConsoleAutoComplete.Text = text.Trim();
            }
        }

        protected override void EndRun() {
            GameTimeScheduler.Dispose(Time);

            base.EndRun();
        }

        /// <summary>
        /// Use this function to initialize the default strings for all your localizations
        /// </summary>
        public virtual void InitializeLocalizations() {
            
        }

        protected override void BeginRun() {
            DevConsole.Run(":nt_begin_run", false, true);
            ScreenManager.ChangeScreen(this._startScreen);
        }

        public void ChangeScreen(Screen screen) {
            this.BeforeScreenChange?.Invoke(this, screen);
            
            if (this._running != null) {
                this.Components.Remove(this._running);

                ((GameComponent)this._running).Dispose();
                this._running = null;
            }

            this.Components.Add(screen);
            this._running = screen;
            
            this.AfterScreenChange?.Invoke(this, screen);
        }

        protected override void LoadContent() {
            DrawableBatch = new DrawableBatch(new SpriteBatch(this.GraphicsDevice));

            this.ChangeScreenSize(DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT);

            DebugCounter = new DebugCounter {
                Clickable   = false,
                CoverClicks = false
            };
            DebugOverlayDrawableManager.Add(DebugCounter);

            DevConsole.Initialize();

            ScreenManager.SetTransition(new FadeTransition());
        }

        public void ChangeScreenSize(int width, int height, bool fullscreen = false) {
            this._graphics.PreferredBackBufferWidth  = width;
            this._graphics.PreferredBackBufferHeight = height;

            this._graphics.IsFullScreen = fullscreen;

            this._graphics.SynchronizeWithVerticalRetrace = false;
            this.IsFixedTimeStep                          = false;

            this._graphics.GraphicsDevice.RasterizerState = new RasterizerState {
                CullMode             = CullMode.None,
                MultiSampleAntiAlias = true
            };
            this._graphics.GraphicsDevice.BlendState = new BlendState {
                AlphaSourceBlend      = Blend.SourceAlpha,
                AlphaDestinationBlend = Blend.InverseSourceColor,
                ColorSourceBlend      = Blend.SourceAlpha,
                ColorDestinationBlend = Blend.InverseSourceAlpha
            };
            
            this._graphics.ApplyChanges();
        }

        private Stopwatch _updateWatch    = new ();
        public double    LastUpdateTime { get; private set; } = 0.0;

        protected override void Update(GameTime gameTime) {
            if (RuntimeInfo.IsDebug()) {
                this._updateWatch.Reset();
                this._updateWatch.Start();
            }

            InputManager.Update();

            DrawableManager.Update(gameTime);

            if (ConVars.DebugOverlay.Value == 1)
                DebugOverlayDrawableManager.Update(gameTime);

            if (RuntimeInfo.LoggerEnabled())
                Logger.XnaUpdate(gameTime.ElapsedGameTime.TotalSeconds);

            ScreenManager.UpdateTransition(gameTime);

            GameTimeScheduler.Update(Time);
            
            base.Update(gameTime);

            if (RuntimeInfo.IsDebug()) {
                this._updateWatch.Stop();
                this.LastUpdateTime = this._updateWatch.Elapsed.TotalMilliseconds;
            }
        }

        private Stopwatch _drawWatch = new ();
        public double    LastDrawTime { get; private set; } = 0.0;

        protected override void Draw(GameTime gameTime) {
            if (RuntimeInfo.IsDebug()) {
                this._drawWatch.Reset();
                this._drawWatch.Start();
            }

            this.GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);

            if(DrawableBatch.Begun)
                DrawableBatch.End();

            ScreenManager.DrawTransition(gameTime, DrawableBatch);
            
            DrawableManager.Draw(gameTime, DrawableBatch);

            if (ConVars.DebugOverlay.Value == 1)
                DebugOverlayDrawableManager.Draw(gameTime, DrawableBatch);

            if (RuntimeInfo.IsDebug()) {
                this._drawWatch.Stop();
                this.LastDrawTime = this._drawWatch.Elapsed.TotalMilliseconds;
            }
        }

        #region Timing

        private static Stopwatch _stopwatch = new();
        public static int Time => (int)_stopwatch.ElapsedMilliseconds;

        #endregion
    }
}

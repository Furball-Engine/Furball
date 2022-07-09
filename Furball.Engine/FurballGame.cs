using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Reflection;
using FontStashSharp;
using Furball.Engine.Engine;
using Furball.Engine.Engine.Config;
using Furball.Engine.Engine.Debug.DebugCounter;
using Furball.Engine.Engine.DevConsole;
using Furball.Engine.Engine.ECS;
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
using Furball.Vixie;
using Furball.Vixie.Backends.Shared;
using Furball.Volpe.Evaluation;
using Kettu;
using Silk.NET.Input;
using sowelipisona;
using sowelipisona.ManagedBass;
using Color=System.Drawing.Color;
using Environment=System.Environment;

namespace Furball.Engine {
    public class FurballGame : Game {
        private GameComponent _running;
        public  GameComponent RunningScreen;

        public static Random Random = new();

        public static FurballGame   Instance;
        public static DrawableBatch DrawableBatch;
        public static InputManager  InputManager;
        public static ITimeSource   GameTimeSource;
        public static Scheduler     GameTimeScheduler;
        public static AudioEngine   AudioEngine;
        
        public static DebugCounter DebugCounter;

        public static DrawableManager DrawableManager;
        public static DrawableManager DebugOverlayDrawableManager;

        public static bool DrawInputOverlay;

        public const int DEFAULT_WINDOW_WIDTH  = 1280;
        public const int DEFAULT_WINDOW_HEIGHT = 720;

        public static string AssemblyPath       = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new Exception("shits fucked man");
        public static string LocalizationFolder => $"{AssemblyPath}/Localization";

        public static int WindowWidth => (int) Instance.WindowManager.WindowSize.X;
        public static int WindowHeight => (int) Instance.WindowManager.WindowSize.Y;

        public static float HorizontalRatio => (float)WindowWidth / DEFAULT_WINDOW_WIDTH;
        public static float VerticalRatio => (float)WindowHeight / DEFAULT_WINDOW_HEIGHT;
        //public static float VerticalRatio => 1;
        public static Rectangle DisplayRect => new(0, 0, (int)Math.Ceiling(WindowWidth / VerticalRatio), (int)Math.Ceiling(WindowHeight / VerticalRatio));
        public static Rectangle DisplayRectActual => new(0, 0, WindowWidth, WindowHeight);

        private bool _drawDebugOverlay = true;

        private static TextDrawable    _ConsoleAutoComplete;
        public static  TooltipDrawable TooltipDrawable;

        public static byte[] DefaultFontData;

        public static bool BypassFurballFPSLimit = false;
        
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

        public static Texture WhitePixel;

        public event EventHandler<Screen> BeforeScreenChange; 
        public event EventHandler<Screen> AfterScreenChange;

        private Screen _startScreen;
        public FurballGame(Screen startScreen) : base()  {
            FurballConfig.Instance = new FurballConfig();
            FurballConfig.Instance.Load();

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
            $@"Starting Furball {(Environment.Is64BitProcess ? "64-bit" : "32-bit")} on {Environment.OSVersion.VersionString} {(Environment.Is64BitOperatingSystem ? "64-bit" : "32-bit")}",
                LoggerLevelEngineInfo.Instance
            );

            DefaultFontData = ContentManager.LoadRawAsset("default-font.ttf");
            DEFAULT_FONT.AddFont(DefaultFontData);
            DEFAULT_FONT_STROKED.AddFont(DefaultFontData);

            _stopwatch.Start();

            InputManager = new InputManager();
            InputManager.RegisterInputMethod(new VixieMouseInputMethod());
            InputManager.RegisterInputMethod(new VixieKeyboardInputMethod());

            InputManager.OnKeyDown += delegate(object _, Key keys) {
                switch (keys) {
                    case Key.F11:
                        this._drawDebugOverlay = !this._drawDebugOverlay;
                        break;
                    case Key.F10:
                        DrawInputOverlay = !DrawInputOverlay;
                        break;
                }
            };

            //TODO: Add logic to decide on what audio backend to use, and maybe write some code to help change backend on the fly
            AudioEngine = new ManagedBassAudioEngine();
            AudioEngine.Initialize(this.WindowManager.GetWindowHandle());

            DrawableManager             = new DrawableManager();
            DebugOverlayDrawableManager = new DrawableManager();

            WhitePixel = Resources.CreateTexture();

            LocalizationManager.ReadTranslations();

            if (Enum.TryParse(FurballConfig.Instance.Language, out ISO639_2Code code)) {
                LocalizationManager.CurrentLanguage = LocalizationManager.GetLanguageFromCode(code);
            } else {
                Logger.Log($"Invalid language in config! Resetting to default: {LocalizationManager.DefaultLanguage}.", LoggerLevelLocalizationInfo.Instance);
                LocalizationManager.CurrentLanguage = LocalizationManager.DefaultLanguage;
            }
            
            TooltipDrawable = new TooltipDrawable();
            DrawableManager.Add(TooltipDrawable);

            TooltipDrawable.Visible = false;
            
            DrawableBatch = new DrawableBatch();

            DebugCounter = new DebugCounter {
                Clickable   = false,
                CoverClicks = false
            };
            DebugOverlayDrawableManager.Add(DebugCounter);

            DevConsole.Initialize();
            ImGuiConsole.Initialize();

            ScreenManager.SetTransition(new FadeTransition());

            base.Initialize();

            if (!BypassFurballFPSLimit && FurballConfig.Instance.Values["limit_fps"].ToBoolean().Value)
                SetTargetFps((int) FurballConfig.Instance.Values["target_fps"].ToNumber().Value);

            ChangeScreenSize(
            (int) FurballConfig.Instance.Values["screen_width"].ToNumber().Value,
            (int) FurballConfig.Instance.Values["screen_height"].ToNumber().Value,
            FurballConfig.Instance.Values["fullscreen"].ToBoolean().Value
            );
            
            ScreenManager.ChangeScreen(this._startScreen);
            //Clear the reference
            this._startScreen = null;

            EtoHelper.Initialize();

            if (Assembly.GetExecutingAssembly().GetType("MonoMod.WasHere") != null) {
                GameTimeScheduler.ScheduleMethod(
                delegate {
                    TextDrawable easterEggText = new TextDrawable(
                        new Vector2(5, 5),
                        DEFAULT_FONT_STROKED,
                        "Hello MonoMod user! :3c",
                        36
                    );

                    easterEggText.Hoverable = true;
                    easterEggText.ToolTip   = "Enjoy Modding!";

                    double drawableTime = easterEggText.DrawableTime;

                    easterEggText.Depth = 1.0;

                    easterEggText.Tweens.Add(new FloatTween(TweenType.Fade, 0.0f, 1.0f, drawableTime, drawableTime + 250));
                    easterEggText.Tweens.Add(new FloatTween(TweenType.Fade, 1.0f, 0.0f, drawableTime + 2250, easterEggText.DrawableTime + 2500));

                    DrawableManager.Add(easterEggText);

                    GameTimeScheduler.ScheduleMethod(
                    delegate {
                        DrawableManager.Remove(easterEggText);
                    }, easterEggText.DrawableTime + 2500);

                }, 1500);
            }
        }
        protected override void OnClosing() {
            try {
                DevConsole.WriteLog();
            }
            catch { /* */ }

            GameTimeScheduler.Dispose(Time);
            EtoHelper.Dispose();

            FurballConfig.Instance.Save();
            
            base.OnClosing();
        }

        protected override void DrawLoadingScreen() {
            GraphicsBackend.Current.Clear();

            DrawableBatch b = new();

            FontSystem fontSystem = new();
            fontSystem.AddFont(ContentManager.LoadRawAsset("default-font.ttf"));
            DynamicSpriteFont font = fontSystem.GetFont(80);

            string text = $"Loading {Assembly.GetEntryAssembly()!.GetName().Name}...";

            Vector2 textSize = font.MeasureString(text);

            b.Begin();
            b.DrawString(font, text, new Vector2(DEFAULT_WINDOW_WIDTH / 2f - textSize.X / 2f, DEFAULT_WINDOW_HEIGHT / 2f - textSize.Y / 2f), Color.White);
            b.End();

            fontSystem.Dispose();

            b.Dispose();
        }

        public double UnfocusedFpsScale => this._unfocusedFpsScale;

        public void SetTargetFps(int fps, double unfocusedScale = -1) {
            this.WindowManager.SetTargetFramerate(fps == -1 ? 0 : fps);

            if (!BypassFurballFPSLimit)
                FurballConfig.Instance.Values["target_fps"] = new Value.Number(fps);
            
            if (unfocusedScale < 0)
                unfocusedScale = this._unfocusedFpsScale;

            if (unfocusedScale >= 0) {
                this._unfocusedFpsScale = unfocusedScale;

                double newFps       = fps   * unfocusedScale;
                double milliseconds = 1000d / newFps;

                //TODO post release: emulate this using OnWindowStateChange or smth
                //this.InactiveSleepTime = TimeSpan.FromMilliseconds(milliseconds);
            }
        }

        /// <summary>
        /// Use this function to initialize the default strings for all your localizations
        /// </summary>
        public virtual void InitializeLocalizations() {
            
        }

        public void ChangeScreen(Screen screen) {
            this.BeforeScreenChange?.Invoke(this, screen);

            if (this.RunningScreen != null) {
                this.Components.Remove(this.RunningScreen);

                this.RunningScreen.Dispose();
                this.RunningScreen = null;
            }

            this.Components.Add(screen);
            this.RunningScreen = screen;
            
            this.AfterScreenChange?.Invoke(this, screen);
            TooltipDrawable.Visible = false;
        }

        public void ChangeScreenSize(int width, int height, bool fullscreen) {
            this.ChangeScreenSize(width, height);
            this.WindowManager.Fullscreen = fullscreen;
            
            FurballConfig.Instance.Values["fullscreen"] = new Value.Boolean(fullscreen);
        }

        public void ChangeScreenSize(int width, int height) {
            this.WindowManager.SetWindowSize(width, height);
            FurballConfig.Instance.Values["screen_width"]  = new Value.Number(width);
            FurballConfig.Instance.Values["screen_height"] = new Value.Number(height);
        }

        private Stopwatch _updateWatch    = new ();
        public double    LastUpdateTime { get; private set; } = 0.0;

        protected override void Update(double deltaTime) {
            if (RuntimeInfo.IsDebug()) {
                this._updateWatch.Reset();
                this._updateWatch.Start();
            }

            InputManager.Update();

            base.Update(deltaTime);

            DrawableManager.Update(deltaTime);

            if (this._drawDebugOverlay)
                DebugOverlayDrawableManager.Update(deltaTime);

            Logger.XnaUpdate(deltaTime);

            ScreenManager.UpdateTransition(deltaTime);

            GameTimeScheduler.Update(Time);

            if (RuntimeInfo.IsDebug()) {
                this._updateWatch.Stop();
                this.LastUpdateTime = this._updateWatch.Elapsed.TotalMilliseconds;
            }
        }

        private Stopwatch _drawWatch = new ();
        private double    _unfocusedFpsScale      = 1;
        public double    LastDrawTime { get; private set; } = 0.0;

        protected override void Draw(double gameTime) {
            if (RuntimeInfo.IsDebug()) {
                this._drawWatch.Reset();
                this._drawWatch.Start();
            }

            GraphicsBackend.Current.Clear();

            GraphicsBackend.Current.SetFullScissorRect();
            
            if(DrawableBatch.Begun)
                DrawableBatch.End();

            ImGuiConsole.Draw();

            base.Draw(gameTime);

            DrawableManager.Draw(gameTime, DrawableBatch);

            if (RuntimeInfo.IsDebug()) {
                this._drawWatch.Stop();
                this.LastDrawTime = this._drawWatch.Elapsed.TotalMilliseconds;
            }

            ScreenManager.DrawTransition(gameTime, DrawableBatch);

            if (this._drawDebugOverlay)
                DebugOverlayDrawableManager.Draw(gameTime, DrawableBatch);
        }

        #region Timing

        private static Stopwatch _stopwatch = new();
        public static  double    Time => _stopwatch.Elapsed.TotalMilliseconds;

        #endregion

        #region ECS Stuff

        public void RegisterEntity(Entity entity) => this.Components.Add(entity);
        public void UnregisterEntity(Entity entity) => this.Components.Remove(entity);

        #endregion

    }
}

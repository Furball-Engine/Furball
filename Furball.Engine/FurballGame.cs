using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using FontStashSharp;
using Furball.Engine.Engine;
using Furball.Engine.Engine.Debug.DebugCounter;
using Furball.Engine.Engine.DevConsole;
using Furball.Engine.Engine.Graphics;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Helpers;
using Furball.Engine.Engine.Helpers.Logger;
using Furball.Engine.Engine.Input;
using Furball.Engine.Engine.Input.InputMethods;
using Furball.Engine.Engine.Localization;
using Furball.Engine.Engine.Platform;
using Furball.Engine.Engine.Timing;
using Furball.Engine.Engine.Transitions;
using Furball.Vixie;
using Furball.Vixie.Graphics;
using Kettu;
using sowelipisona;
using sowelipisona.ManagedBass;
using Silk.NET.Input;
using Color=Furball.Vixie.Graphics.Color;

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

        public static Texture WhitePixel;

        public event EventHandler<Screen> BeforeScreenChange; 
        public event EventHandler<Screen> AfterScreenChange;

        private Screen _startScreen;
        public FurballGame(Screen startScreen) : base()  {
            //this._graphics = new GraphicsDeviceManager(this) {
            //    GraphicsProfile     = GraphicsProfile.HiDef,
            //    PreferMultiSampling = true
            //};
//
            //this._graphics.PreparingDeviceSettings += (_, e) => e.GraphicsDeviceInformation.PresentationParameters.MultiSampleCount = 4;
            //this._graphics.ApplyChanges();

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
            InputManager.RegisterInputMethod(new VixieMouseInputMethod());
            InputManager.RegisterInputMethod(new VixieKeyboardInputMethod());

            if (ConVars.DebugOverlay.Value == 1) {
                InputManager.OnKeyDown += delegate(object _, Key keys) {
                    if (keys == Key.F11)
                        ConVars.DebugOverlay.BindableValue.Value = ConVars.DebugOverlay.BindableValue.Value == 0 ? 1 : 0;
                    if (keys == Key.F12 && !ImGuiConsole.Visible)
                        ImGuiConsole.Visible = true;
                };
            }

            //TODO: Add logic to decide on what audio backend to use, and maybe write some code to help change backend on the fly
            AudioEngine = new ManagedBassAudioEngine();
            AudioEngine.Initialize(this.WindowManager.GetWindowHandle());

            DrawableManager             = new DrawableManager();
            DebugOverlayDrawableManager = new DrawableManager();

            //_ConsoleAutoComplete = new TextDrawable(new Vector2(DEFAULT_WINDOW_WIDTH / 2f, DEFAULT_WINDOW_HEIGHT * 0.4f), DEFAULT_FONT, "", 30) {
            //    OriginType    = OriginType.BottomCenter,
            //    ColorOverride = new Color(255, 255, 255, 0),
            //    Clickable     = false,
            //    CoverClicks   = false
            //};

            //ConsoleDrawable.OnLetterTyped   += this.ConsoleOnLetterTyped;
            //ConsoleDrawable.OnLetterRemoved += this.ConsoleOnLetterTyped;

            //DebugOverlayDrawableManager.Add(_ConsoleAutoComplete);

            WhitePixel = new Texture();

            LocalizationManager.ReadTranslations();

            TooltipDrawable = new();
            DrawableManager.Add(TooltipDrawable);

            ScreenManager.ChangeScreen(this._startScreen);

            DrawableBatch = new DrawableBatch(RendererType.Batched);

            this.ChangeScreenSize(DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT);

            DebugCounter = new DebugCounter {
                Clickable   = false,
                CoverClicks = false
            };
            DebugOverlayDrawableManager.Add(DebugCounter);

            DevConsole.Initialize();
            ImGuiConsole.Initialize();

            ScreenManager.SetTransition(new FadeTransition());

            base.Initialize();

            EtoHelper.Initialize();
        }
        protected override void OnClosing() {
            DevConsole.Run(":nt_on_exiting", false, true);
            DevConsole.WriteLog();

            GameTimeScheduler.Dispose(Time);
            EtoHelper.Dispose();

            base.OnClosing();
        }

        public void SetTargetFps(int fps, double unfocusedScale = -1) {
            if (fps != -1) {
                WindowManager.SetTargetFramerate(fps);
            } else {
                //Setting to 0 makes it unlimited
                WindowManager.SetTargetFramerate(0);
            }

            if (unfocusedScale != -1) {
                double newFps       = fps   * unfocusedScale;
                double milliseconds = 1000d / newFps;

                //TODO post release: emulate this using OnWindowStateChange or smth
                //this.InactiveSleepTime = TimeSpan.FromMilliseconds(milliseconds);
            }
        }

        private void ConsoleOnLetterTyped(object sender, char e) {
            //_ConsoleAutoComplete.Tweens.Clear();
            //_ConsoleAutoComplete.Tweens.Add(new FloatTween(TweenType.Fade, _ConsoleAutoComplete.ColorOverride.A / 255f, 1f, Time,        Time + 100));
            //_ConsoleAutoComplete.Tweens.Add(new FloatTween(TweenType.Fade, 1f,                                          0f, Time + 2100, Time + 3100));
//
            //string input = ConsoleDrawable.Text;
//
            //if (input.StartsWith(':')) {
            //    input = input.TrimStart(':');
//
            //    IEnumerable<KeyValuePair<string, ConFunc>> functions = DevConsole.RegisteredFunctions.Where(x => x.Key.StartsWith(input));
//
            //    string text = "";
//
            //    int i = 0;
            //    foreach (KeyValuePair<string, ConFunc> pair in functions) {
            //        if (i == 5) break;
            //        text += $"{pair.Key}\n";
            //        i++;
            //    }
//
            //    _ConsoleAutoComplete.Text = text.Trim();
            //} else {
            //    IEnumerable<KeyValuePair<string, ConVar>> convars = DevConsole.RegisteredConVars.Where(x => x.Key.StartsWith(input));
//
            //    string text = "";
//
            //    int i = 0;
            //    foreach (KeyValuePair<string, ConVar> pair in convars) {
            //        if (i == 5) break;
            //        text += $"{pair.Key}\n";
            //        i++;
            //    }
//
            //    _ConsoleAutoComplete.Text = text.Trim();
            //}
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
        }
        public void ChangeScreenSize(int width, int height, bool fullscreen = false) {
            WindowManager.SetWindowSize(width, height);
        }

        private Stopwatch _updateWatch    = new ();
        public double    LastUpdateTime { get; private set; } = 0.0;

        protected override void Update(double gameTime) {
            if (RuntimeInfo.IsDebug()) {
                this._updateWatch.Reset();
                this._updateWatch.Start();
            }

            InputManager.Update();

            base.Update(gameTime);

            ImGuiConsole.Draw();

            DrawableManager.Update(gameTime);

            if (ConVars.DebugOverlay.Value == 1)
                DebugOverlayDrawableManager.Update(gameTime);

            if (RuntimeInfo.LoggerEnabled())
                Logger.XnaUpdate(gameTime);

            ScreenManager.UpdateTransition(gameTime);

            GameTimeScheduler.Update(Time);

            if (RuntimeInfo.IsDebug()) {
                this._updateWatch.Stop();
                this.LastUpdateTime = this._updateWatch.Elapsed.TotalMilliseconds;
            }
        }

        private Stopwatch _drawWatch = new ();
        public double    LastDrawTime { get; private set; } = 0.0;

        protected override void Draw(double gameTime) {
            if (RuntimeInfo.IsDebug()) {
                this._drawWatch.Reset();
                this._drawWatch.Start();
            }

            this.GraphicsDevice.GlClearColor(Color.Black);
            this.GraphicsDevice.GlClear();

            if(DrawableBatch.Begun)
                DrawableBatch.End();

            ScreenManager.DrawTransition(gameTime, DrawableBatch);
            
            DrawableManager.Draw(gameTime, DrawableBatch);

            base.Draw(gameTime);

            if (RuntimeInfo.IsDebug()) {
                this._drawWatch.Stop();
                this.LastDrawTime = this._drawWatch.Elapsed.TotalMilliseconds;
            }

            if (ConVars.DebugOverlay.Value == 1)
                DebugOverlayDrawableManager.Draw(gameTime, DrawableBatch);
        }

        #region Timing

        private static Stopwatch _stopwatch = new();
        public static int Time => (int)_stopwatch.ElapsedMilliseconds;

        #endregion
    }
}

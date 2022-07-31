using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Threading;
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
using Furball.Engine.Engine.Localization.Languages;
using Furball.Engine.Engine.Platform;
using Furball.Engine.Engine.Timing;
using Furball.Engine.Engine.Transitions;
using Furball.Vixie;
using Furball.Vixie.Backends.Shared;
using Furball.Vixie.Backends.Shared.Backends;
using Furball.Volpe.Evaluation;
using JetBrains.Annotations;
using Kettu;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using sowelipisona;
using sowelipisona.ManagedBass;
using Color=System.Drawing.Color;
using Environment=System.Environment;
using Rectangle=System.Drawing.Rectangle;

namespace Furball.Engine; 

public class FurballGame : Game {
    private GameComponent _running;
    [CanBeNull]
    public Screen RunningScreen;
    private Screen LoadingScreen;

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

    public static string AssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new Exception("shits fucked man");
    public static string LocalizationFolder => $"{AssemblyPath}/Localization";

    public static int RealWindowWidth => (int) Instance.WindowManager.WindowSize.X;
    public static int RealWindowHeight => (int) Instance.WindowManager.WindowSize.Y;

    public static float HorizontalRatio => (float)RealWindowWidth / DEFAULT_WINDOW_WIDTH;
    public static float VerticalRatio => (float)RealWindowHeight / DEFAULT_WINDOW_HEIGHT;

    public static float WindowWidth => RealWindowWidth / (float)RealWindowHeight * 720f;
    public static float WindowHeight => DEFAULT_WINDOW_HEIGHT;

    //public static float VerticalRatio => 1;
    public static RectangleF DisplayRect       => new(0, 0, WindowWidth, WindowHeight);
    public static Rectangle  DisplayRectActual => new(0, 0, RealWindowWidth, RealWindowHeight);

    private bool _drawDebugOverlay = false;

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

    public event EventHandler<Vector2> OnRelayout; 

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
        Profiler.StartProfile("full_furball_initialize");

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

        Profiler.StartProfile("init_audio_engine");
        //TODO: Add logic to decide on what audio backend to use, and maybe write some code to help change backend on the fly
        AudioEngine = new ManagedBassAudioEngine();
        AudioEngine.Initialize(this.WindowManager.GetWindowHandle());
        Profiler.EndProfileAndPrint("init_audio_engine");

        DrawableManager             = new DrawableManager();
        DebugOverlayDrawableManager = new DrawableManager();

        WhitePixel = Resources.CreateTexture();

        LocalizationManager.ReadTranslations();

        Language language = null;
        try {
            if (Enum.TryParse(FurballConfig.Instance.Language, out ISO639_2Code code)) {
                language = LocalizationManager.GetLanguageFromCode(code);

                if (language == null)
                    throw new Exception("Invalid language in config, this is likely due to the language not being registered with the engine, or you mistyped.");
            } else {
                throw new Exception("Invalid language in config!");
            }
        }
        catch (Exception ex) {
            language = LocalizationManager.DefaultLanguage;
            Debugger.Break();
        }
        finally {
            LocalizationManager.CurrentLanguage = language!;
        }
        
        LocalizationManager.LanguageChanged += delegate {
            this.RunningScreen?.UpdateTextStrings();
        };

        TooltipDrawable = new TooltipDrawable();
        DrawableManager.Add(TooltipDrawable);

        TooltipDrawable.Visible = false;

        DrawableBatch = new DrawableBatch();

        DebugCounter = new DebugCounter {
            Position = new Vector2(5, 5),
            OriginType = OriginType.BottomLeft,
            ScreenOriginType = OriginType.BottomLeft
        };
        DebugOverlayDrawableManager.Add(DebugCounter);

        if (RuntimeInfo.IsDebug())
            this._drawDebugOverlay = true;

        DevConsole.Initialize();
        ImGuiConsole.Initialize();
        ContentManager.Initialize();

        ScreenManager.SetTransition(new FadeTransition());

        Profiler.StartProfile("load_content");
        base.Initialize();
        Profiler.EndProfileAndPrint("load_content");

        Profiler.StartProfile("set_window_properties");
        if (!BypassFurballFPSLimit && FurballConfig.Instance.Values["limit_fps"].ToBoolean().Value)
            SetTargetFps((int) FurballConfig.Instance.Values["target_fps"].ToNumber().Value);

        ChangeScreenSize(
        (int) FurballConfig.Instance.Values["screen_width"].ToNumber().Value,
        (int) FurballConfig.Instance.Values["screen_height"].ToNumber().Value,
        FurballConfig.Instance.Values["fullscreen"].ToBoolean().Value
        );
        Profiler.EndProfileAndPrint("set_window_properties");

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

                easterEggText.Tweens.Add(new FloatTween(TweenType.Fade, 0.0f, 1.0f, drawableTime,        drawableTime + 250));
                easterEggText.Tweens.Add(new FloatTween(TweenType.Fade, 1.0f, 0.0f, drawableTime + 2250, easterEggText.DrawableTime + 2500));

                DrawableManager.Add(easterEggText);

                GameTimeScheduler.ScheduleMethod(
                delegate {
                    DrawableManager.Remove(easterEggText);
                }, easterEggText.DrawableTime + 2500);

            }, 1500);
        }
        Profiler.EndProfileAndPrint("full_furball_initialize");
    }

    public void Run(Backend backend = Backend.None) {
        WindowOptions options = WindowOptions.Default;

        options.VSync = false;

        options.Size = new Vector2D<int>(FurballConfig.Instance.ScreenWidth, FurballConfig.Instance.ScreenHeight);

        this.Run(options, backend);
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
        b.DrawString(font, text, new Vector2(WindowWidth / 2f - textSize.X / 2f, WindowHeight / 2f - textSize.Y / 2f), Color.White);
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
    protected virtual void InitializeLocalizations() {
            
    }

    public void ChangeScreen(Screen screen) {
        this.BeforeScreenChange?.Invoke(this, screen);

        if (this.RunningScreen != null) {
            this.Components.Remove(this.RunningScreen);

            this.RunningScreen.Dispose();
            this.RunningScreen = null;
        }

        if((!screen.RequireLoadingScreen) || (screen.RequireLoadingScreen && screen.LoadingComplete)) {
            this.LoadingScreen                 = null;
            this._loadingScreenChangeOffQueued = false;
                
            this.Components.Add(screen);
            screen.UpdateTextStrings();
            screen.Relayout(WindowWidth, WindowHeight);
            this.OnRelayout?.Invoke(this, new(WindowWidth, WindowHeight));
            this.RunningScreen = screen;

            this.AfterScreenChange?.Invoke(this, screen);
            TooltipDrawable.Visible = false;
        } else {
            this.LoadingScreen = screen;

            if (screen.BackgroundThread == null) {
                screen.BackgroundThread = new Thread(
                _ => {
                    screen.BackgroundInitialize();
                }
                );
                    
                screen.BackgroundThread.Start();
            }
        }
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
            
        this.RunningScreen?.Relayout(WindowWidth, WindowHeight);
        this.OnRelayout?.Invoke(this, new(WindowWidth, WindowHeight));
    }

    protected override void OnWindowResize(Vector2D<int> newSize) {
        base.OnWindowResize(newSize);
            
        this.RunningScreen?.Relayout(WindowWidth, WindowHeight);
        this.OnRelayout?.Invoke(this, new(WindowWidth, WindowHeight));

        FurballConfig.Instance.Values["screen_width"]  = new Value.Number(newSize.X);
        FurballConfig.Instance.Values["screen_height"] = new Value.Number(newSize.Y);
    }

    private Stopwatch _updateWatch = new ();
    public double    LastUpdateTime { get; private set; } = 0.0;

    // ReSharper disable once InconsistentNaming
    public static readonly List<FixedTimeStepMethod> TimeStepMethods = new();
        
    private bool _loadingScreenChangeOffQueued = false;
    protected override void Update(double deltaTime) {
        if (RuntimeInfo.IsDebug()) {
            this._updateWatch.Reset();
            this._updateWatch.Start();
        }

        if (this.LoadingScreen is {
                LoadingComplete: true
            } && !this._loadingScreenChangeOffQueued) {
            this._loadingScreenChangeOffQueued = true;
            ScreenManager.ChangeScreen(this.LoadingScreen);
        }

        InputManager.Update();

        base.Update(deltaTime);

        foreach (FixedTimeStepMethod fixedTimeStepMethod in TimeStepMethods) {
            fixedTimeStepMethod.Update(deltaTime);
        }
            
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

    private Stopwatch _drawWatch         = new ();
    private double    _unfocusedFpsScale = 1;
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

        if (this.LoadingScreen != null) {
            DynamicSpriteFont f = DEFAULT_FONT.GetFont(40);

            string text = this.LoadingScreen.LoadingStatus;

            Vector2 textSize = f.MeasureString(text);
                
            if(!DrawableBatch.Begun)
                DrawableBatch.Begin();

            const float gap       = DEFAULT_WINDOW_HEIGHT * 0.05f;
            const float barHeight = 30;
                
            DrawableBatch.DrawString(f, text, new Vector2(WindowWidth / 2f - textSize.X / 2f, WindowHeight * 0.3f), Color.White);
            DrawableBatch.FillRectangle(new Vector2(gap, WindowHeight - gap - barHeight), new((WindowWidth - gap * 2f) * this.LoadingScreen.LoadingProgress, barHeight), Vixie.Backends.Shared.Color.Grey);
            DrawableBatch.DrawRectangle(new Vector2(gap, WindowHeight - gap - barHeight), new(WindowWidth - gap * 2f, barHeight), 1, Vixie.Backends.Shared.Color.White);
                
            DrawableBatch.End();
        }
            
        ScreenManager.DrawTransition(gameTime, DrawableBatch);

        if (this._drawDebugOverlay)
            DebugOverlayDrawableManager.Draw(gameTime, DrawableBatch);

        if (RuntimeInfo.IsDebug()) {
            this._drawWatch.Stop();
            this.LastDrawTime = this._drawWatch.Elapsed.TotalMilliseconds;
        }
    }

    #region Timing

    private static Stopwatch _stopwatch = new();
    public static  double    Time => _stopwatch.Elapsed.TotalMilliseconds;

    #endregion

    #region ECS Stuff

    public void RegisterEntity(Entity   entity) => this.Components.Add(entity);
    public void UnregisterEntity(Entity entity) => this.Components.Remove(entity);

    #endregion

}
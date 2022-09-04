using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
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
using Furball.Vixie.Backends.Shared.Backends;
using Furball.Volpe.Evaluation;
using JetBrains.Annotations;
using Kettu;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using sowelipisona;
using sowelipisona.ManagedBass;
using Environment=System.Environment;
using GraphicsBackend=Furball.Vixie.GraphicsBackend;
using Rectangle=System.Drawing.Rectangle;

namespace Furball.Engine; 

public class FurballGame : Game {
    [CanBeNull]
    public Screen RunningScreen;
    private Screen _loadingScreen;

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
    public static DrawableManager OverlayDrawableManager;

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

    internal static TooltipDrawable TooltipDrawable;

    private DrawableForm _textureDisplayForm;
    private bool        _textureDisplayFormAdded;
    
    public static byte[] DefaultFontData;

    public static bool BypassFurballFpsLimit = false;
    public static bool BypassFurballUpsLimit = false;

    public static readonly FontSystem DefaultFont = new(
    new FontSystemSettings {
        FontResolutionFactor = 2f,
        KernelWidth          = 2,
        KernelHeight         = 2,
        Effect               = FontSystemEffect.None
    }
    );

    public static readonly FontSystem DefaultFontStroked = new(
    new FontSystemSettings {
        FontResolutionFactor = 2f,
        KernelWidth          = 2,
        KernelHeight         = 2,
        Effect               = FontSystemEffect.Stroked,
        EffectAmount         = 2
    }
    );

    public static Texture WhitePixel;

    public event EventHandler<Screen> BeforeScreenChange; 
    public event EventHandler<Screen> AfterScreenChange;

    public event EventHandler<Vector2> OnRelayout; 

    private Screen _startScreen;
    public FurballGame(Screen startScreen) {
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
        DefaultFont.AddFont(DefaultFontData);
        DefaultFontStroked.AddFont(DefaultFontData);

        _stopwatch.Start();

        InputManager = new InputManager();
        //Only use the silk input methods if we are using the `View` event loop!
        if (this.EventLoop is ViewEventLoop) {
            InputManager.RegisterInputMethod(InputManager.SilkWindowingMouseInputMethod    = new SilkWindowingMouseInputMethod());

            bool useSilkKeyboardInput = true;

            //If we are on linux and we are the root user, we can use the EvDev input instead, which can detect multiple keyboards separately
            if (RuntimeInfo.CurrentPlatform() == OSPlatform.Linux && UnixEnvironment.IsRoot()) {
                useSilkKeyboardInput = false;
                InputManager.RegisterInputMethod(new EvDevKeyboardInputMethod());
            }
            
            if(useSilkKeyboardInput)
                InputManager.RegisterInputMethod(InputManager.SilkWindowingKeyboardInputMethod = new SilkWindowingKeyboardInputMethod());
        }

        Profiler.StartProfile("init_audio_engine");
        //TODO: Add logic to decide on what audio backend to use, and maybe write some code to help change backend on the fly
        AudioEngine = new ManagedBassAudioEngine();
        AudioEngine.Initialize(this.EventLoop is ViewEventLoop ? this.WindowManager.GetWindowHandle() : IntPtr.Zero);
        Profiler.EndProfileAndPrint("init_audio_engine");

        DrawableManager             = new DrawableManager();
        DebugOverlayDrawableManager = new DrawableManager();
        OverlayDrawableManager      = new DrawableManager();
        
        WhitePixel      = Texture.CreateWhitePixelTexture();
        WhitePixel.Name = "FurballGame.WhitePixel";
        
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
        catch {
            language = LocalizationManager.DefaultLanguage;
            Debugger.Break();
        }
        finally {
            LocalizationManager.CurrentLanguage = language!;
        }
        
        LocalizationManager.LanguageChanged += delegate {
            this.RunningScreen?.UpdateTextStrings();
        };

        TooltipDrawable = new TooltipDrawable {
            Depth = double.NegativeInfinity
        };
        OverlayDrawableManager.Add(TooltipDrawable);

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
        if (!BypassFurballFpsLimit && FurballConfig.Instance.Values["limit_fps"].ToBoolean().Value) {
            double var = FurballConfig.Instance.Values["target_fps"].ToNumber().Value;

            //If the target fps is less than or equal to 0, then send in `null`, if not, then send the var
            SetTargetFps(var <= 0 ? null : var);
        }

        if (!BypassFurballUpsLimit && FurballConfig.Instance.Values["limit_ups"].ToBoolean().Value) {
            double var = FurballConfig.Instance.Values["target_ups"].ToNumber().Value;
            
            //If the target ups is less than or equal to 0, then send in `null`, if not, then send the var
            SetTargetUps(var <= 0 ? null : var);
        }

        ChangeScreenSize(
        (int) FurballConfig.Instance.Values["screen_width"].ToNumber().Value,
        (int) FurballConfig.Instance.Values["screen_height"].ToNumber().Value,
        FurballConfig.Instance.Values["fullscreen"].ToBoolean().Value
        );

        this.WindowManager.EnableUnfocusCap = FurballConfig.Instance.UnfocusCap;
        Profiler.EndProfileAndPrint("set_window_properties");

        ScreenManager.ChangeScreen(this._startScreen);
        //Clear the reference
        this._startScreen = null;

        EtoHelper.Initialize();
        
        this.RegisterKeybinds();

        MonoModCheck();

        #region Texture debugger

        ScrollableContainer scrollable = new(new Vector2(400, 500));

        scrollable.ScrollSpeed       *= 2;
        scrollable.InfiniteScrolling =  true;

        DebugTextureDisplayDrawable debug = new();

        scrollable.Add(debug);

        this._textureDisplayForm = new DrawableForm("Currently bound textures", scrollable) {
            Depth = -10
        };

        this._textureDisplayForm.OnTryClose += delegate {
            this._textureDisplayFormAdded = false;

            DrawableManager.Remove(this._textureDisplayForm);
        };

        #endregion
        
        Profiler.EndProfileAndPrint("full_furball_initialize");
    }
    
    private static void MonoModCheck() {
        if (Assembly.GetExecutingAssembly().GetType("MonoMod.WasHere") != null) {
            GameTimeScheduler.ScheduleMethod(
            delegate {
                TextDrawable easterEggText = new(new Vector2(5, 5), DefaultFontStroked, "Hello MonoMod user! :3c", 36);

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
                },
                easterEggText.DrawableTime + 2500
                );

            },
            1500
            );
        }
    }

    protected override void OnWindowRecreation() {
        base.OnWindowRecreation();
        
        InputManager.RemoveInputMethod(InputManager.SilkWindowingMouseInputMethod);
        InputManager.RemoveInputMethod(InputManager.SilkWindowingKeyboardInputMethod);
        
        InputManager.RegisterInputMethod(InputManager.SilkWindowingMouseInputMethod    = new SilkWindowingMouseInputMethod());
        InputManager.RegisterInputMethod(InputManager.SilkWindowingKeyboardInputMethod = new SilkWindowingKeyboardInputMethod());
    }

    public new void RunHeadless() {
        base.RunHeadless();
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

        this.UnregisterKeybinds();
        
        GameTimeScheduler.Dispose(Time);
        EtoHelper.Dispose();

        FurballConfig.Instance.Save();
            
        base.OnClosing();
    }

    private enum EngineDebugKeybinds {
        ToggleDebugOverlay,
        ToggleInputOverlay,
        ToggleConsole,
        DispalyDebugTextureviewer
    }
    
    private Keybind _toggleDebugOverlay;
    private Keybind _toggleInputOverlay;
    private Keybind _displayDebugTextureViewer;
    private Keybind _toggleConsole;
    public virtual void RegisterKeybinds() {
        this._toggleDebugOverlay = new Keybind(EngineDebugKeybinds.ToggleDebugOverlay, "Toggle Debug Overlay", Key.F11,
                                               _ => {
                                                   this._drawDebugOverlay = !this._drawDebugOverlay;
                                               }
        );
        this._toggleInputOverlay = new Keybind(EngineDebugKeybinds.ToggleInputOverlay, "Toggle Input Overlay", Key.F10,
                                               _ => {
                                                   DrawInputOverlay = !DrawInputOverlay;
                                               }
        );
        this._displayDebugTextureViewer = new Keybind(EngineDebugKeybinds.DispalyDebugTextureviewer, "Display Debug Texture Viewer", Key.F9,
                                                      _ => {
                                                          if (!this._textureDisplayFormAdded) {
                                                              DrawableManager.Add(this._textureDisplayForm);
                                                              this._textureDisplayFormAdded = true;
                                                          }
                                                      }
        );
        this._toggleConsole = new Keybind(
        EngineDebugKeybinds.ToggleConsole,
        "Toggle Console",
        Key.F12,
        _ => {
            ImGuiConsole.Visible = !ImGuiConsole.Visible;
        }
        );
        
        InputManager.RegisterKeybind(this._toggleDebugOverlay);
        InputManager.RegisterKeybind(this._toggleInputOverlay);
        InputManager.RegisterKeybind(this._displayDebugTextureViewer);
        InputManager.RegisterKeybind(this._toggleConsole);
    }

    public virtual void UnregisterKeybinds() {
        InputManager.UnregisterKeybind(this._toggleDebugOverlay);
        InputManager.UnregisterKeybind(this._toggleInputOverlay);
        InputManager.UnregisterKeybind(this._displayDebugTextureViewer);
        InputManager.UnregisterKeybind(this._toggleConsole);
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

    public void SetTargetFps(double? fps) {
        if (this.EventLoop is not ViewEventLoop)
            return;
        
        this.WindowManager.TargetFramerate = fps ?? 0;

        if (!BypassFurballFpsLimit)
            FurballConfig.Instance.Values["target_fps"] = new Value.Number(fps ?? -1);
    }

    public void SetTargetUps(double? ups) {
        if (this.EventLoop is not ViewEventLoop)
            return;
        
        this.WindowManager.TargetUpdaterate = ups ?? 0;

        if (!BypassFurballUpsLimit)
            FurballConfig.Instance.Values["target_ups"] = new Value.Number(ups ?? -1);
    }

    /// <summary>
    /// Use this function to initialize the default strings for all your localizations
    /// </summary>
    protected virtual void InitializeLocalizations() {
            
    }

    public void ChangeScreen(Screen screen) {
        Logger.Log($"Immediately changing screen to {screen.GetType().Name}", LoggerLevelEngineInfo.Instance);

        this.BeforeScreenChange?.Invoke(this, screen);

        if (this.RunningScreen != null) {
            this.Components.Remove(this.RunningScreen);

            this.RunningScreen = null;
        }

        if((!screen.RequireLoadingScreen) || (screen.RequireLoadingScreen && screen.LoadingComplete)) {
            this._loadingScreen                 = null;
            this._loadingScreenChangeOffQueued = false;
                
            this.Components.Add(screen);
            screen.UpdateTextStrings();
            if (screen.Manager.EffectedByScaling)
                screen.ManagerOnOnScalingRelayoutNeeded(this, screen.Manager.Size);
            else
                screen.Relayout(WindowWidth, WindowHeight);
            this.OnRelayout?.Invoke(this, new Vector2(WindowWidth, WindowHeight));
            this.RunningScreen = screen;

            this.AfterScreenChange?.Invoke(this, screen);
            TooltipDrawable.Visible = false;
            
            Logger.Log($"Change to {screen.GetType().Name} finished!", LoggerLevelEngineInfo.Instance);
        } else {
            this._loadingScreen = screen;

            if (screen.BackgroundThread == null) {
                Logger.Log($"Starting background thread for {screen.GetType().Name}", LoggerLevelEngineInfo.Instance);

                screen.BackgroundThread = new Thread(
                _ => {
                    screen.BackgroundInitialize();
                }
                ) {
                    IsBackground = true
                };

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

        if (this.RunningScreen?.Manager.EffectedByScaling ?? false)
            this.RunningScreen.ManagerOnOnScalingRelayoutNeeded(this, this.RunningScreen.Manager.Size);
        else
            this.RunningScreen?.Relayout(WindowWidth, WindowHeight);
        this.OnRelayout?.Invoke(this, new Vector2(WindowWidth, WindowHeight));
    }

    protected override void OnWindowResize(Vector2D<int> newSize) {
        base.OnWindowResize(newSize);

        if (this.RunningScreen?.Manager.EffectedByScaling ?? false)
            this.RunningScreen.ManagerOnOnScalingRelayoutNeeded(this, this.RunningScreen.Manager.Size);
        else
            this.RunningScreen?.Relayout(WindowWidth, WindowHeight);
        this.OnRelayout?.Invoke(this, new Vector2(WindowWidth, WindowHeight));

        FurballConfig.Instance.Values["screen_width"]  = new Value.Number(newSize.X);
        FurballConfig.Instance.Values["screen_height"] = new Value.Number(newSize.Y);
    }

    private Stopwatch _updateWatch = new ();
    public double    LastUpdateTime { get; private set; } = 0.0;

    // ReSharper disable once InconsistentNaming
    public static readonly List<FixedTimeStepMethod> TimeStepMethods = new();
        
    private bool _loadingScreenChangeOffQueued = false;
    protected override void Update(double deltaTime) {
        Time = _stopwatch.Elapsed.TotalMilliseconds;
        
        if (RuntimeInfo.IsDebug()) {
            this._updateWatch.Reset();
            this._updateWatch.Start();
        }

        if (this._loadingScreen is {
                LoadingComplete: false
            } && !this._loadingScreen.BackgroundThread.IsAlive) {
            throw new Exception("The background loading thread has died!");
        }       
        if (this._loadingScreen is {
                LoadingComplete: true
            } && !this._loadingScreenChangeOffQueued) {
            this._loadingScreenChangeOffQueued = true;
            ScreenManager.ChangeScreen(this._loadingScreen);
        }

        InputManager.Update();

        base.Update(deltaTime);

        foreach (FixedTimeStepMethod fixedTimeStepMethod in TimeStepMethods) {
            fixedTimeStepMethod.Update(deltaTime);
        }
            
        DrawableManager.Update(deltaTime);
        OverlayDrawableManager.Update(deltaTime);

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

    private Stopwatch _drawWatch = new();
    public double LastDrawTime { get; private set; } = 0.0;

    protected override void PreDraw(double deltaTime) {
        base.PreDraw(deltaTime);

        Debug.Assert(DrawableBatch.ScissorStackItemCount == 0, "Scissor Stack not empty at start of frame!");
        
        DrawableBatch.Begin();
    }

    protected override void PostDraw(double deltaTime) {
        base.PostDraw(deltaTime);

        Debug.Assert(DrawableBatch.ScissorStackItemCount == 0, "Scissor Stack not empty at end of frame!");

        DrawableBatch.End();
    }

    protected override void Draw(double gameTime) {
        if (RuntimeInfo.IsDebug()) {
            this._drawWatch.Reset();
            this._drawWatch.Start();
        }

        GraphicsBackend.Current.Clear();

        GraphicsBackend.Current.SetFullScissorRect();

        ImGuiConsole.Draw();

        base.Draw(gameTime);

        DrawableManager.Draw(gameTime, DrawableBatch);

        if (this._loadingScreen != null) {
            DynamicSpriteFont f = DefaultFont.GetFont(40);

            string text = this._loadingScreen.LoadingStatus;

            Vector2 textSize = f.MeasureString(text);

            const float gap       = DEFAULT_WINDOW_HEIGHT * 0.05f;
            const float barHeight = 30;
                
            DrawableBatch.DrawString(f, text, new Vector2(WindowWidth / 2f - textSize.X / 2f, WindowHeight * 0.3f), Color.White);
            DrawableBatch.FillRectangle(new Vector2(gap,                                      WindowHeight - gap - barHeight), new Vector2((WindowWidth - gap * 2f) * this._loadingScreen.LoadingProgress, barHeight), Vixie.Backends.Shared.Color.Grey);
            DrawableBatch.DrawRectangle(new Vector2(gap,                                      WindowHeight - gap - barHeight), new Vector2(WindowWidth - gap * 2f, barHeight), 1, Vixie.Backends.Shared.Color.White);
        }

        OverlayDrawableManager.Draw(gameTime, DrawableBatch);

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
    public static double Time {
        get;
        private set;
    }

    #endregion

    #region ECS Stuff

    public void RegisterEntity(Entity   entity) => this.Components.Add(entity);
    public void UnregisterEntity(Entity entity) => this.Components.Remove(entity);

    #endregion

}
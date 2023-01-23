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
using Furball.Engine.Engine.Debug.DebugCounter.Items;
using Furball.Engine.Engine.DevConsole;
using Furball.Engine.Engine.Graphics;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.Debug;
using Furball.Engine.Engine.Graphics.Drawables.Debug.SceneViewer;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Graphics.Drawables.Tweens;
using Furball.Engine.Engine.Graphics.Drawables.Tweens.TweenTypes;
using Furball.Engine.Engine.Helpers;
using Furball.Engine.Engine.Helpers.Logger;
using Furball.Engine.Engine.Localization;
using Furball.Engine.Engine.Localization.Languages;
using Furball.Engine.Engine.Input;
using Furball.Engine.Engine.Platform;
using Furball.Engine.Engine.Timing;
using Furball.Engine.Engine.Transitions;
using Furball.Vixie;
using Furball.Vixie.Backends.Shared.Backends;
using Furball.Vixie.Backends.Shared.Renderers;
using Furball.Vixie.WindowManagement;
using Furball.Volpe.Evaluation;
using JetBrains.Annotations;
using Kettu;
using Silk.NET.Input;
using Silk.NET.Maths;
using sowelipisona;
using sowelipisona.ManagedBass;
using Environment = System.Environment;
using Rectangle = System.Drawing.Rectangle;

namespace Furball.Engine; 

public class FurballGame : Game {
    [CanBeNull]
    public Screen RunningScreen;
    private Screen _loadingScreen;

    public static Random Random = new();

    public static FurballGame   Instance;
    public static DrawableBatch DrawableBatch;
    public static DrawableBatch CursorDrawableBatch;
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

    public static string AssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? Environment.CurrentDirectory;
    public static string DataFolder   = AssemblyPath;
    public static string LocalizationFolder => $"{AssemblyPath}/Localization";

    public static int RealWindowWidth => (int) Instance.WindowManager.FramebufferSize.X;
    public static int RealWindowHeight => (int) Instance.WindowManager.FramebufferSize.Y;

    public static float HorizontalRatio => (float)RealWindowWidth / DEFAULT_WINDOW_WIDTH;
    public static float VerticalRatio => (float)RealWindowHeight / DEFAULT_WINDOW_HEIGHT;

    public static float WindowWidth => RealWindowWidth / (float)RealWindowHeight * 720f;
    public static float WindowHeight => DEFAULT_WINDOW_HEIGHT;

    //public static float VerticalRatio => 1;
    public static RectangleF DisplayRect       => new(0, 0, WindowWidth, WindowHeight);
    public static Rectangle  DisplayRectActual => new(0, 0, RealWindowWidth, RealWindowHeight);

    private bool _drawDebugOverlay = false;

    public static TooltipDrawable TooltipDrawable;

    private DrawableForm _textureDisplayForm;
    private bool         _textureDisplayFormAdded;
    private DrawableForm _sceneDebuggerForm;
    private bool         _sceneDebuggerFormAdded;
    
    public static byte[] DefaultFontData;

    public static bool BypassFurballFpsLimit = false;
    public static bool BypassFurballUpsLimit = false;

    public static readonly FontSystem DefaultFont = new(
    new FontSystemSettings {
        FontResolutionFactor = 2f,
        KernelWidth          = 2,
        KernelHeight         = 2,
        PremultiplyAlpha     = false
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

        FontSystemDefaults.PremultiplyAlpha = false;
    }

    protected override void Initialize() {
        if(RuntimeInfo.CurrentPlatform() == OSPlatform.Windows)
            Windows.AttachToExistingConsole();
        
        Profiler.StartProfile("full_furball_initialize");

        //If we are in a readonly environment, we should write to another folder which is more likely to not be readonly
        if (new DirectoryInfo(DataFolder).Attributes.HasFlag(FileAttributes.ReadOnly))
            DataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), this.GetType().Assembly.GetName().Name);

        this.InitializeLocalizations();
            
        Logger.Log(
        $@"Starting Furball {(Environment.Is64BitProcess ? "64-bit" : "32-bit")} on {Environment.OSVersion.VersionString} {(Environment.Is64BitOperatingSystem ? "64-bit" : "32-bit")}",
        LoggerLevelEngineInfo.Instance
        );

        DefaultFontData = ContentManager.LoadRawAsset("default-font.ttf");
        DefaultFont.AddFont(DefaultFontData);

        _stopwatch.Start();

        InputManager = new InputManager();
        InputManager.Start();
        
        // bool usingEvDevInput = false;

        // //If we are on linux and we are the root user, we can use the EvDev input instead, which can detect multiple keyboards separately
        // if (RuntimeInfo.CurrentPlatform() == OSPlatform.Linux && UnixEnvironment.IsRoot()) {
        //     usingEvDevInput = true;
        //     InputManager.RegisterInputMethod(new EvDevKeyboardInputMethod());
        // }
        //
        // if (this.WindowManager is SilkWindowManager silkWindowManager) {
        //     if(!usingEvDevInput)
        //         InputManager.RegisterInputMethod(InputManager.SilkWindowingKeyboardInputMethod = new SilkWindowingKeyboardInputMethod(silkWindowManager.InputContext));
        //
        //     InputManager.RegisterInputMethod(InputManager.SilkWindowingMouseInputMethod = new SilkWindowingMouseInputMethod(silkWindowManager.InputContext));
        //     InputManager.SilkWindowingMouseInputMethod.SetRawInputStatus(FurballConfig.Instance.RawMouseInput);
        // }

        Profiler.StartProfile("init_audio_engine");
        //TODO: Add logic to decide on what audio backend to use, and maybe write some code to help change backend on the fly
        AudioEngine = new ManagedBassAudioEngine();
        AudioEngine.Initialize(this.WindowManager.WindowHandle);
        Profiler.EndProfileAndPrint("init_audio_engine");

        DrawableManager             = new DrawableManager();
        DebugOverlayDrawableManager = new DrawableManager();
        OverlayDrawableManager      = new DrawableManager();
        
        WhitePixel      = ResourceFactory.CreateWhitePixelTexture();
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
        CursorDrawableBatch = new DrawableBatch();

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

        this.WindowManager.UnfocusFramerateCap = FurballConfig.Instance.UnfocusCap;
        this.WindowManager.VSync               = FurballConfig.Instance.VerticalSync;
        Profiler.EndProfileAndPrint("set_window_properties");

        ScreenManager.ChangeScreen(this._startScreen);
        //Clear the reference
        this._startScreen = null;

        EtoHelper.Initialize();
        
        this.RegisterKeybinds();

        MonoModCheck();

        if (RuntimeInfo.IsDebug()) {
            // this.CreateTextureDebugger();
            // this.CreateSceneViewer();
        }

        WindowManager.FramebufferResize += this.OnWindowResize;

        MaxTextureSize = WindowManager.GraphicsBackend.MaxTextureSize;
        
        Profiler.EndProfileAndPrint("full_furball_initialize");
    }

    private void CreateSceneViewer() {
        Bindable<Drawable> selected = new Bindable<Drawable>(null);

        DebugSceneViewerTreePane   treePane   = new DebugSceneViewerTreePane(selected);
        DebugSceneViewerEditorPane editorPane = new DebugSceneViewerEditorPane(selected);

        ScrollableContainer treeContainer = new ScrollableContainer(treePane.Size) {
            InvisibleToInput = true
        };
        treeContainer.ScrollSpeed *= 2;
        ScrollableContainer editorContainer = new ScrollableContainer(editorPane.Size) {
            Position = treePane.Size with {
                Y = 0
            },
            InvisibleToInput = true
        };
        editorContainer.ScrollSpeed *= 2;

        treeContainer.Add(treePane);
        editorContainer.Add(editorPane);

        CompositeDrawable composite = new CompositeDrawable {
            InvisibleToInput = true
        };

        composite.Children.Add(treeContainer);
        composite.Children.Add(editorContainer);

        this._sceneDebuggerForm = new DrawableForm("Scene Debugger", composite) {
            Depth = -10
        };

        this._sceneDebuggerForm.OnTryClose += delegate {
            this._sceneDebuggerFormAdded = false;

            DrawableManager.Remove(this._sceneDebuggerForm);
        };
    }
    
    private void CreateTextureDebugger() {
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
    }

    private static void MonoModCheck() {
        if (Assembly.GetExecutingAssembly().GetType("MonoMod.WasHere") != null) {
            GameTimeScheduler.ScheduleMethod(
            delegate {
                TextDrawable easterEggText = new TextDrawable(new Vector2(5, 5), DefaultFont, "Hello MonoMod user! :3c", 36);

                easterEggText.Effect         = FontSystemEffect.Stroked;
                easterEggText.EffectStrength = 1;
                easterEggText.Hoverable      = true;
                easterEggText.ToolTip        = "Enjoy Modding!";

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

    public void Run(Backend backend = Backend.None) {
        base.Run(backend);
    }
        
    protected override void OnClosing() {
        try {
            DevConsole.WriteLog();
        }
        catch { /* */ }

        this.UnregisterKeybinds();
        
        InputManager.End();
        
        GameTimeScheduler.Dispose(Time);
        EtoHelper.Dispose();

        FurballConfig.Instance.Save();
        
        this.RunningScreen?.Dispose();

        Logger.Update().Wait();
        
        base.OnClosing();
    }

    private enum EngineDebugKeybinds {
        ToggleDebugOverlay,
        ToggleInputOverlay,
        ToggleConsole,
        DisplayDebugTextureViewer,
        DisplaySceneDebugger,
        WalkAndPrintCurrentScreen
    }
    
    private Keybind _toggleDebugOverlay;
    private Keybind _toggleInputOverlay;
    private Keybind _displayDebugTextureViewer;
    private Keybind _displaySceneDebugger;
    private Keybind _toggleConsole;
    private Keybind _walkCurrentScreen;
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
        this._displayDebugTextureViewer = new Keybind(
        EngineDebugKeybinds.DisplayDebugTextureViewer,
        "Display Debug Texture Viewer",
        Key.F9,
        _ => {
            if (!this._textureDisplayFormAdded) {
                DrawableManager.Add(this._textureDisplayForm);
                this._textureDisplayFormAdded = true;
            }
        }
        );
        this._displaySceneDebugger = new Keybind(
        EngineDebugKeybinds.DisplaySceneDebugger,
        "Display Debug Texture Viewer",
        Key.F7,
        _ => {
            if (!this._sceneDebuggerFormAdded) {
                DrawableManager.Add(this._sceneDebuggerForm);
                this._sceneDebuggerFormAdded = true;
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
        this._walkCurrentScreen = new Keybind(EngineDebugKeybinds.WalkAndPrintCurrentScreen, "Walk and print current screen", Key.F8,
                                              _ => {
                                                  if (this.RunningScreen is not null) {
                                                      DrawableTreeWalker.PrintWalkedTree(this.RunningScreen.Manager);
                                                  }
                                              }
        );
        
        InputManager.RegisterKeybind(this._toggleDebugOverlay);
        InputManager.RegisterKeybind(this._toggleInputOverlay);
        InputManager.RegisterKeybind(this._displayDebugTextureViewer);
        InputManager.RegisterKeybind(this._displaySceneDebugger);
        InputManager.RegisterKeybind(this._toggleConsole);
        InputManager.RegisterKeybind(this._walkCurrentScreen);
    }

    public virtual void UnregisterKeybinds() {
        InputManager.UnregisterKeybind(this._toggleDebugOverlay);
        InputManager.UnregisterKeybind(this._toggleInputOverlay);
        InputManager.UnregisterKeybind(this._displayDebugTextureViewer);
        InputManager.UnregisterKeybind(this._displaySceneDebugger);
        InputManager.UnregisterKeybind(this._toggleConsole);
        InputManager.UnregisterKeybind(this._walkCurrentScreen);
    }

    protected override void DrawLoadingScreen() {
        Instance.WindowManager.GraphicsBackend.Clear();

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
        this.WindowManager.TargetFramerate = fps ?? 0;

        if (!BypassFurballFpsLimit)
            FurballConfig.Instance.Values["target_fps"] = new Value.Number(fps ?? -1);
    }

    public void SetTargetUps(double? ups) {
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

        //Dispose the currently running screen
        this.RunningScreen?.Dispose();

        this.RunningScreen = null;

        if((!screen.RequireLoadingScreen) || (screen.RequireLoadingScreen && screen.LoadingComplete)) {
            this._loadingScreen                 = null;
            this._loadingScreenChangeOffQueued = false;
                
            screen.Initialize();
            
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

        //do *not* move this to a switch expression, it will break
        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if (!fullscreen && this.WindowManager.WindowState.HasFlag(WindowState.Fullscreen))
            this.WindowManager.WindowState = WindowState.Windowed;
        else if (fullscreen && !this.WindowManager.WindowState.HasFlag(WindowState.Fullscreen))
            this.WindowManager.WindowState = WindowState.Fullscreen;

        FurballConfig.Instance.Values["fullscreen"] = new Value.Boolean(fullscreen);
    }

    public void ChangeScreenSize(int width, int height) {
        this.WindowManager.WindowSize                  = new Vector2D<int>(width, height);
        FurballConfig.Instance.Values["screen_width"]  = new Value.Number(width);
        FurballConfig.Instance.Values["screen_height"] = new Value.Number(height);

        if (this.RunningScreen?.Manager.EffectedByScaling ?? false)
            this.RunningScreen.ManagerOnOnScalingRelayoutNeeded(this, this.RunningScreen.Manager.Size);
        else
            this.RunningScreen?.Relayout(WindowWidth, WindowHeight);
        this.OnRelayout?.Invoke(this, new Vector2(WindowWidth, WindowHeight));
    }

    protected void OnWindowResize(Vector2D<int> newSize) {
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

        this.RunningScreen?.Update(deltaTime);
        base.Update(deltaTime);

        for (int i = 0; i < TimeStepMethods.Count; i++) {
            FixedTimeStepMethod fixedTimeStepMethod = TimeStepMethods[i];
            fixedTimeStepMethod.Update(deltaTime);
        }

        DrawableManager.Update(deltaTime);
        OverlayDrawableManager.Update(deltaTime);

        if (this._drawDebugOverlay)
            DebugOverlayDrawableManager.Update(deltaTime);

        //Kettu uses seconds for delta time, so we need to convert it
        Logger.XnaUpdate(deltaTime / 1000f);

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

        Renderer.VertexCount = 0;
        Renderer.IndexCount  = 0;
        
        DrawableBatch.Begin();
    }

    private float _cursorVerticalRatioCache;
    protected override void PostDraw(double deltaTime) {
        base.PostDraw(deltaTime);

        Debug.Assert(DrawableBatch.ScissorStackItemCount == 0, "Scissor Stack not empty at end of frame!");

        DrawableBatch.End();
        
        // //We want to draw software cursors after everything else in the game, so we do it in here
        // #region Draw software cursors
        //
        // List<FurballMouse> mice = InputManager.Mice;
        //
        // bool needsRecalc = false;
        // foreach (FurballMouse furballMouse in mice) {
        //     if (furballMouse.LastKnownSoftwareCursorPosition != furballMouse.Position)
        //         needsRecalc = true;
        //
        //     furballMouse.LastKnownSoftwareCursorPosition = furballMouse.Position;
        // }
        //
        // // ReSharper disable once CompareOfFloatsByEqualityOperator
        // if (this._cursorVerticalRatioCache != VerticalRatio)
        //     needsRecalc = true;
        //
        // this._cursorVerticalRatioCache = VerticalRatio;
        //
        // if(needsRecalc) {
        //     CursorDrawableBatch.Begin();
        //
        //     unsafe {
        //         foreach (FurballMouse mouse in mice) {
        //             if (mouse.SoftwareCursor) {
        //                 //3 vertices for the head, and 4 for the tail
        //                 //3 indices for the head, 6 for the tail (2 tris)
        //                 MappedData map = CursorDrawableBatch.Reserve(3 + 4, 3 + 6, WhitePixel);
        //
        //                 const float mouseWidth  = 15;
        //                 const float mouseHeight = 25;
        //                 const float tailWidth   = mouseWidth / 3f;
        //
        //                 float mouseScale = (float)(1 / VerticalRatio * FurballConfig.Instance.SoftwareCursorScale);
        //
        //                 map.VertexPtr[0].Position = Vector2.Zero * mouseScale + mouse.Position;
        //                 map.VertexPtr[1].Position = new Vector2(0,          mouseHeight) * mouseScale + mouse.Position;
        //                 map.VertexPtr[2].Position = new Vector2(mouseWidth, mouseHeight) * mouseScale + mouse.Position;
        //
        //                 const float tailLeft         = mouseWidth / 2f - tailWidth / 2f;
        //                 const float tailRight        = mouseWidth / 2f + tailWidth / 2f;
        //                 const float tailLength       = 10f;
        //                 const float tailBottom       = mouseHeight + tailLength;
        //                 const float tailBottomOffset = 2.5f;
        //
        //                 //Top left corner of the tail
        //                 map.VertexPtr[3].Position = new Vector2(tailLeft, mouseHeight) * mouseScale + mouse.Position;
        //                 //Top right corner of the tail
        //                 map.VertexPtr[4].Position = new Vector2(tailRight, mouseHeight) * mouseScale + mouse.Position;
        //                 //Bottom left corner of the tail
        //                 map.VertexPtr[5].Position = new Vector2(tailLeft + tailBottomOffset, tailBottom) * mouseScale + mouse.Position;
        //                 //Bottom right corner of the tail
        //                 map.VertexPtr[6].Position = new Vector2(tailRight + tailBottomOffset, tailBottom) * mouseScale + mouse.Position;
        //                 
        //                 for (int i = 0; i < map.VertexCount; i++) {
        //                     map.VertexPtr[i].Color = new Vixie.Backends.Shared.Color(1f, 1f, 1f, 0.8f);
        //                     map.VertexPtr[i].TexId = map.TextureId;
        //                 }
        //                 
        //                 //Head tri
        //                 map.IndexPtr[0] = (ushort)(0 + map.IndexOffset);
        //                 map.IndexPtr[1] = (ushort)(1 + map.IndexOffset);
        //                 map.IndexPtr[2] = (ushort)(2 + map.IndexOffset);
        //                 
        //                 //Tail tri 1
        //                 map.IndexPtr[3] = (ushort)(3 + map.IndexOffset);
        //                 map.IndexPtr[4] = (ushort)(5 + map.IndexOffset);
        //                 map.IndexPtr[5] = (ushort)(4 + map.IndexOffset);
        //                 //Tail tri 2
        //                 map.IndexPtr[6] = (ushort)(4 + map.IndexOffset);
        //                 map.IndexPtr[7] = (ushort)(5 + map.IndexOffset);
        //                 map.IndexPtr[8] = (ushort)(6 + map.IndexOffset);
        //             }
        //         }
        //     }
        //
        //     CursorDrawableBatch.SoftEnd();
        // }
        // CursorDrawableBatch.ManualDraw();
        //
        // #endregion

        DrawCounts.LastVertexCount = Renderer.VertexCount;
        DrawCounts.LastIndexCount  = Renderer.IndexCount;
    }

    protected override void Draw(double gameTime) {
        if (RuntimeInfo.IsDebug()) {
            this._drawWatch.Reset();
            this._drawWatch.Start();
        }

        Instance.WindowManager.GraphicsBackend.Clear();

        Instance.WindowManager.GraphicsBackend.SetFullScissorRect();

        ImGuiConsole.Draw();

        this.RunningScreen?.Draw(gameTime);
        base.Draw(gameTime);

        DrawableManager.Draw(gameTime, DrawableBatch);

        if (this._loadingScreen != null)
            this.DrawSceneLoadingScreen();

        OverlayDrawableManager.Draw(gameTime, DrawableBatch);

        ScreenManager.DrawTransition(gameTime, DrawableBatch);

        if (this._drawDebugOverlay)
            DebugOverlayDrawableManager.Draw(gameTime, DrawableBatch);

        if (RuntimeInfo.IsDebug()) {
            this._drawWatch.Stop();
            this.LastDrawTime = this._drawWatch.Elapsed.TotalMilliseconds;
        }
    }

    private void DrawSceneLoadingScreen() {
        DynamicSpriteFont f = DefaultFont.GetFont(40);

        string text = this._loadingScreen.LoadingStatus;

        Vector2 textSize = f.MeasureString(text);

        const float gap       = DEFAULT_WINDOW_HEIGHT * 0.05f;
        const float barHeight = 30;

        DrawableBatch.DrawString(f, text, new Vector2(WindowWidth / 2f - textSize.X / 2f, WindowHeight * 0.3f), Color.White);
        DrawableBatch.FillRectangle(
        new Vector2(gap,                                                            WindowHeight - gap - barHeight),
        new Vector2((WindowWidth - gap * 2f) * this._loadingScreen.LoadingProgress, barHeight),
        Vixie.Backends.Shared.Color.Grey
        );
        DrawableBatch.DrawRectangle(
        new Vector2(gap, WindowHeight - gap - barHeight),
        new Vector2(WindowWidth       - gap * 2f, barHeight),
        1,
        Vixie.Backends.Shared.Color.White
        );
    }

    #region Timing

    private static Stopwatch _stopwatch = new();
    public static double Time {
        get;
        private set;
    }
    
    public static Vector2D<int> MaxTextureSize {
        get;
        private set;
    }

    #endregion
}
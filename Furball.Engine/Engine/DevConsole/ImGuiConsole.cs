using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Helpers;
using Furball.Engine.Engine.Platform;
using Furball.Vixie.Backends.Shared;
using ImGuiNET;
using Silk.NET.Input;

namespace Furball.Engine.Engine.DevConsole; 

public static class ImGuiConsole {
    private static string              _consoleBuffer   = "";
    private static List<ConsoleResult> _devConsoleCache = new();

    private static BlankDrawable _consoleInputCoverDrawable;
    public static  bool          Visible = false;

    private static bool _scrollDown = false;
    public static  bool AutoScroll  = true;

    private static int _cursorPosition = 0;
    private static int _selectionStart = 0;
    private static int _selectionEnd   = 0;

    public static string Title;

    public static void UpdateTitle() {
        //TODO: Figure out how to make this change without breaking the window
        Title = $"Console (Press CTRL to toggle auto-scroll!)";
    }

    public static void Initialize() {
        Profiler.StartProfile("init_imgui_console");

        UpdateTitle();

        RefreshCache();

        FurballGame.InputManager.OnKeyDown += (_, key) => {
            if (key == Key.ControlLeft) {
                AutoScroll = !AutoScroll;
                UpdateTitle();
            }
            if (Visible) {
                if (key == Key.V && FurballGame.InputManager.ControlHeld && RuntimeInfo.CurrentPlatform() == OSPlatform.Linux) {
                    _consoleBuffer = _consoleBuffer.Insert(_cursorPosition, FurballGame.InputManager.Clipboard ?? string.Empty);
                }

                if (key == Key.C && FurballGame.InputManager.ControlHeld && RuntimeInfo.CurrentPlatform() == OSPlatform.Linux) {
                    FurballGame.InputManager.Clipboard = _consoleBuffer.Substring(_selectionStart, _selectionEnd);
                }
            }
        };

        DevConsole.ConsoleLog.CollectionChanged += (sender, args) => {
            RefreshCache();
            if (AutoScroll)
                _scrollDown = true;
        };

        ImGuiStylePtr style = ImGui.GetStyle();

        _consoleInputCoverDrawable = new BlankDrawable {
            CoverClicks = true,
            Clickable   = true,
            Visible     = true,
            CoverHovers = true,
            Hoverable   = true,
            Depth       = 0
        };

        FurballGame.DrawableManager.Add(_consoleInputCoverDrawable);

        style.Colors[(int) ImGuiCol.Text]                  = new Vector4(1.00f, 1.00f, 1.00f, 1.00f);
        style.Colors[(int) ImGuiCol.TextDisabled]          = new Vector4(0.50f, 0.50f, 0.50f, 1.00f);
        style.Colors[(int) ImGuiCol.WindowBg]              = new Vector4(0.29f, 0.34f, 0.26f, 1.00f);
        style.Colors[(int) ImGuiCol.ChildBg]               = new Vector4(0.29f, 0.34f, 0.26f, 1.00f);
        style.Colors[(int) ImGuiCol.PopupBg]               = new Vector4(0.24f, 0.27f, 0.20f, 1.00f);
        style.Colors[(int) ImGuiCol.Border]                = new Vector4(0.54f, 0.57f, 0.51f, 0.50f);
        style.Colors[(int) ImGuiCol.BorderShadow]          = new Vector4(0.14f, 0.16f, 0.11f, 0.52f);
        style.Colors[(int) ImGuiCol.FrameBg]               = new Vector4(0.24f, 0.27f, 0.20f, 1.00f);
        style.Colors[(int) ImGuiCol.FrameBgHovered]        = new Vector4(0.27f, 0.30f, 0.23f, 1.00f);
        style.Colors[(int) ImGuiCol.FrameBgActive]         = new Vector4(0.30f, 0.34f, 0.26f, 1.00f);
        style.Colors[(int) ImGuiCol.TitleBg]               = new Vector4(0.24f, 0.27f, 0.20f, 1.00f);
        style.Colors[(int) ImGuiCol.TitleBgActive]         = new Vector4(0.29f, 0.34f, 0.26f, 1.00f);
        style.Colors[(int) ImGuiCol.TitleBgCollapsed]      = new Vector4(0.00f, 0.00f, 0.00f, 0.51f);
        style.Colors[(int) ImGuiCol.MenuBarBg]             = new Vector4(0.24f, 0.27f, 0.20f, 1.00f);
        style.Colors[(int) ImGuiCol.ScrollbarBg]           = new Vector4(0.35f, 0.42f, 0.31f, 1.00f);
        style.Colors[(int) ImGuiCol.ScrollbarGrab]         = new Vector4(0.28f, 0.32f, 0.24f, 1.00f);
        style.Colors[(int) ImGuiCol.ScrollbarGrabHovered]  = new Vector4(0.25f, 0.30f, 0.22f, 1.00f);
        style.Colors[(int) ImGuiCol.ScrollbarGrabActive]   = new Vector4(0.23f, 0.27f, 0.21f, 1.00f);
        style.Colors[(int) ImGuiCol.CheckMark]             = new Vector4(0.59f, 0.54f, 0.18f, 1.00f);
        style.Colors[(int) ImGuiCol.SliderGrab]            = new Vector4(0.35f, 0.42f, 0.31f, 1.00f);
        style.Colors[(int) ImGuiCol.SliderGrabActive]      = new Vector4(0.54f, 0.57f, 0.51f, 0.50f);
        style.Colors[(int) ImGuiCol.Button]                = new Vector4(0.29f, 0.34f, 0.26f, 0.40f);
        style.Colors[(int) ImGuiCol.ButtonHovered]         = new Vector4(0.35f, 0.42f, 0.31f, 1.00f);
        style.Colors[(int) ImGuiCol.ButtonActive]          = new Vector4(0.54f, 0.57f, 0.51f, 0.50f);
        style.Colors[(int) ImGuiCol.Header]                = new Vector4(0.35f, 0.42f, 0.31f, 1.00f);
        style.Colors[(int) ImGuiCol.HeaderHovered]         = new Vector4(0.35f, 0.42f, 0.31f, 0.6f);
        style.Colors[(int) ImGuiCol.HeaderActive]          = new Vector4(0.54f, 0.57f, 0.51f, 0.50f);
        style.Colors[(int) ImGuiCol.Separator]             = new Vector4(0.14f, 0.16f, 0.11f, 1.00f);
        style.Colors[(int) ImGuiCol.SeparatorHovered]      = new Vector4(0.54f, 0.57f, 0.51f, 1.00f);
        style.Colors[(int) ImGuiCol.SeparatorActive]       = new Vector4(0.59f, 0.54f, 0.18f, 1.00f);
        style.Colors[(int) ImGuiCol.ResizeGrip]            = new Vector4(0.19f, 0.23f, 0.18f, 0.00f);
        style.Colors[(int) ImGuiCol.ResizeGripHovered]     = new Vector4(0.54f, 0.57f, 0.51f, 1.00f);
        style.Colors[(int) ImGuiCol.ResizeGripActive]      = new Vector4(0.59f, 0.54f, 0.18f, 1.00f);
        style.Colors[(int) ImGuiCol.Tab]                   = new Vector4(0.35f, 0.42f, 0.31f, 1.00f);
        style.Colors[(int) ImGuiCol.TabHovered]            = new Vector4(0.54f, 0.57f, 0.51f, 0.78f);
        style.Colors[(int) ImGuiCol.TabActive]             = new Vector4(0.59f, 0.54f, 0.18f, 1.00f);
        style.Colors[(int) ImGuiCol.TabUnfocused]          = new Vector4(0.24f, 0.27f, 0.20f, 1.00f);
        style.Colors[(int) ImGuiCol.TabUnfocusedActive]    = new Vector4(0.35f, 0.42f, 0.31f, 1.00f);
        style.Colors[(int) ImGuiCol.DockingPreview]        = new Vector4(0.59f, 0.54f, 0.18f, 1.00f);
        style.Colors[(int) ImGuiCol.DockingEmptyBg]        = new Vector4(0.20f, 0.20f, 0.20f, 1.00f);
        style.Colors[(int) ImGuiCol.PlotLines]             = new Vector4(0.61f, 0.61f, 0.61f, 1.00f);
        style.Colors[(int) ImGuiCol.PlotLinesHovered]      = new Vector4(0.59f, 0.54f, 0.18f, 1.00f);
        style.Colors[(int) ImGuiCol.PlotHistogram]         = new Vector4(1.00f, 0.78f, 0.28f, 1.00f);
        style.Colors[(int) ImGuiCol.PlotHistogramHovered]  = new Vector4(1.00f, 0.60f, 0.00f, 1.00f);
        style.Colors[(int) ImGuiCol.TextSelectedBg]        = new Vector4(0.59f, 0.54f, 0.18f, 1.00f);
        style.Colors[(int) ImGuiCol.DragDropTarget]        = new Vector4(0.73f, 0.67f, 0.24f, 1.00f);
        style.Colors[(int) ImGuiCol.NavHighlight]          = new Vector4(0.59f, 0.54f, 0.18f, 1.00f);
        style.Colors[(int) ImGuiCol.NavWindowingHighlight] = new Vector4(1.00f, 1.00f, 1.00f, 0.70f);
        style.Colors[(int) ImGuiCol.NavWindowingDimBg]     = new Vector4(0.80f, 0.80f, 0.80f, 0.20f);
        style.Colors[(int) ImGuiCol.ModalWindowDimBg]      = new Vector4(0.80f, 0.80f, 0.80f, 0.35f);

        style.FrameBorderSize   = 1.0f;
        style.WindowRounding    = 0.0f;
        style.ChildRounding     = 0.0f;
        style.FrameRounding     = 0.0f;
        style.PopupRounding     = 0.0f;
        style.ScrollbarRounding = 0.0f;
        style.GrabRounding      = 0.0f;
        style.TabRounding       = 0.0f;

        Profiler.EndProfileAndPrint("init_imgui_console");
    }

    public static unsafe void Draw() {
        _consoleInputCoverDrawable.Clickable   = Visible;
        _consoleInputCoverDrawable.CoverClicks = Visible;
        _consoleInputCoverDrawable.Hoverable   = Visible;
        _consoleInputCoverDrawable.CoverHovers = Visible;

        if (Visible) {
            ImGui.SetNextWindowSize(new Vector2(520, 600), ImGuiCond.FirstUseEver);
            ImGui.Begin(Title, ref Visible, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoCollapse);

            _consoleInputCoverDrawable.Position     = ImGui.GetWindowPos() / FurballGame.VerticalRatio;
            _consoleInputCoverDrawable.OverrideSize = ImGui.GetWindowSize() / FurballGame.VerticalRatio;

            float heightReserved = ImGui.GetStyle().ItemSpacing.Y + ImGui.GetFrameHeightWithSpacing();

            ImGui.BeginChild("ScrollingRegion", new Vector2(0,           -heightReserved), true, ImGuiWindowFlags.HorizontalScrollbar);
            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(4, 1));

            lock (_devConsoleCache) {
                for (int i = 0; i != _devConsoleCache.Count; i++) {
                    ConsoleResult result = _devConsoleCache[i];

                    switch (result.Result) {
                        case ExecutionResult.Success:
                        case ExecutionResult.Log:
                        case ExecutionResult.Message:
                            ImGui.PushStyleColor(ImGuiCol.Text, Color.White.ToVector4F());
                            break;
                        case ExecutionResult.Warning:
                            ImGui.PushStyleColor(ImGuiCol.Text, Color.Yellow.ToVector4F());
                            break;
                        case ExecutionResult.Error:
                            ImGui.PushStyleColor(ImGuiCol.Text, Color.Red.ToVector4F());
                            break;
                    }

                    ImGui.TextWrapped(result.Message);
                    ImGui.PopStyleColor();
                }
            }
                
            if (_scrollDown) {
                ImGui.SetScrollY(ImGui.GetScrollMaxY() + 100);
                _scrollDown = false;
            }

            ImGui.PopStyleVar();
            ImGui.EndChild();
            ImGui.Separator();

            const ImGuiInputTextFlags textFlags = ImGuiInputTextFlags.EnterReturnsTrue | ImGuiInputTextFlags.CallbackCompletion | ImGuiInputTextFlags.CallbackHistory;

            ImGui.PushItemWidth(-1);

            //if (ImGui.InputText("", _consoleBuffer, (uint) _consoleBuffer.Length, textFlags, OnTextEdit)) {
            if (ImGui.InputText("", ref _consoleBuffer, 4096, textFlags, OnTextEdit)) {
                DevConsole.Run(_consoleBuffer);

                _consoleBuffer = string.Empty;

                ImGui.SetKeyboardFocusHere(-1);
                _scrollDown = true;
            }

            ImGui.PopItemWidth();
            ImGui.SetItemDefaultFocus();
            ImGui.End();
        }
    }

    private static unsafe int OnTextEdit(ImGuiInputTextCallbackData* data) {
        //Potentially maybe add some sort of autocomplete? i wont add it right now because volpe is gonna make us redo this entire thing anyway, but something for later
        //if (data->EventKey == ImGuiKey.Tab) {
        //
        //}

        _cursorPosition = data->CursorPos;
        _selectionStart = data->SelectionStart;
        _selectionEnd   = data->SelectionEnd;

        return 0;
    }

    private static void RefreshCache() {
        lock (_devConsoleCache) {
            _devConsoleCache.Clear();

            for (int i = 0; i != DevConsole.ConsoleLog.Count; i++) {
                (string input, ConsoleResult result) current = DevConsole.ConsoleLog[i];

                if (current.input != "") {
                    _devConsoleCache.Add(new ConsoleResult(ExecutionResult.Message, $"] {current.input}"));
                    _devConsoleCache.Add(current.result);
                } else {
                    _devConsoleCache.Add(current.result);
                }
            }
        }
    }
}
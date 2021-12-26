using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Furball.Engine.Engine.Graphics.Drawables;
using ImGuiNET;
using Silk.NET.Input;

namespace Furball.Engine.Engine.DevConsole {
    public static class ImGuiConsole {
        private static byte[]              _consoleBuffer = new byte[4096];

        private static BlankDrawable _consoleInputCoverDrawable;

        public static bool Visible = false;

        public static unsafe void Initialize() {
            FurballGame.InputManager.OnKeyDown += (_, key) => {
                if (key == Key.F12 && !Visible)
                    Visible = true;
            };

            var style = ImGui.GetStyle();

            _consoleInputCoverDrawable = new BlankDrawable {
                CoverClicks = true,
                Clickable   = true,
                Visible     = true,
                CoverHovers = true,
                Hoverable   = true,
                Depth       = 0
            };

            FurballGame.DrawableManager.Add(_consoleInputCoverDrawable);

            style.ScrollbarRounding = 0;
            style.ChildRounding     = 0;
            style.FrameRounding     = 0;
            style.PopupRounding     = 0;
            style.WindowRounding    = 0;

            style.Colors[(int) ImGuiCol.WindowBg]         = new Vector4((40.0f / 255.0f), (5.0f / 255.0f),  (50.0f / 255.0f), 0.8f);
            style.Colors[(int) ImGuiCol.FrameBg]          = new Vector4((50.0f / 255.0f), (10.0f / 255.0f), (90.0f / 255.0f), 0.8f);
            style.Colors[(int) ImGuiCol.ChildBg]          = new Vector4((50.0f / 255.0f), (10.0f / 255.0f), (90.0f / 255.0f), 0.8f);
            style.Colors[(int) ImGuiCol.TitleBgActive]    = new Vector4((70.0f / 255.0f), (10.0f / 255.0f), (90.0f / 255.0f), 0.8f);
            style.Colors[(int) ImGuiCol.TitleBgCollapsed] = new Vector4((70.0f / 255.0f), (10.0f / 255.0f), (90.0f / 255.0f), 0.8f);

            style.WindowPadding = new Vector2(8, 8);
            style.ItemSpacing   = new Vector2(8, 8);
            style.FramePadding  = new Vector2(8, 4);
        }

        public static unsafe void Draw() {
            if (Visible) {
                ImGui.SetNextWindowSize(new Vector2(520, 600), ImGuiCond.FirstUseEver);
                ImGui.Begin("Console", ref Visible, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoCollapse);

                _consoleInputCoverDrawable.Position     = ImGui.GetWindowPos() / FurballGame.VerticalRatio;
                _consoleInputCoverDrawable.OverrideSize = ImGui.GetWindowSize() / FurballGame.VerticalRatio;

                float heightReserved = ImGui.GetStyle().ItemSpacing.Y + ImGui.GetFrameHeightWithSpacing();

                ImGui.BeginChild("ScrollingRegion", new Vector2(0,           -heightReserved), true, ImGuiWindowFlags.HorizontalScrollbar);
                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(4, 1));

                for (int i = 0; i != DevConsole.ConsoleLog.Count; i++) {
                    (string input, ConsoleResult result) =  DevConsole.ConsoleLog[i];

                    if (input != "") {
                        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(255, 255, 255, 255));
                        ImGui.TextWrapped($"] {input}");
                        ImGui.PopStyleColor();
                    }

                    switch (result.Result) {
                        case ExecutionResult.Success:
                        case ExecutionResult.Log:
                        case ExecutionResult.Message:
                            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(255, 255, 255, 255));
                            break;
                        case ExecutionResult.Warning:
                            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(255, 255, 0, 255));
                            break;
                        case ExecutionResult.Error:
                            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(255, 0, 0, 255));
                            break;
                    }

                    ImGui.SetScrollY(ImGui.GetScrollMaxY());

                    ImGui.TextWrapped(result.Message);
                    ImGui.PopStyleColor();
                }

                ImGui.PopStyleVar();
                ImGui.EndChild();
                ImGui.Separator();

                ImGuiInputTextFlags textFlags = ImGuiInputTextFlags.EnterReturnsTrue | ImGuiInputTextFlags.CallbackCompletion | ImGuiInputTextFlags.CallbackHistory;

                ImGui.PushItemWidth(-1);

                if (ImGui.InputText("", _consoleBuffer, (uint) _consoleBuffer.Length, textFlags, OnTextEdit)) {
                    string result = Encoding.UTF8.GetString(_consoleBuffer).Trim();
                    DevConsole.Run(result);

                    _consoleBuffer = new byte[4096];

                    ImGui.SetKeyboardFocusHere(-1);
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

            return 0;
        }

        public static void ScrollToBottom() {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Furball.Engine.Engine.Graphics;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Graphics.Drawables.UiElements;
using Furball.Engine.Engine.Input;
using Furball.Vixie.Graphics;
using Furball.Vixie.Input;
using ImGuiNET;
using JetBrains.Annotations;
using Silk.NET.Input;

namespace Furball.Engine.Engine.DevConsole {
    public static class ImGuiConsole {
        private static List<ConsoleResult> _consoleLog    = new();
        private static byte[]              _consoleBuffer = new byte[4096];

        public static unsafe void Draw() {
            ImGui.SetNextWindowSize(new Vector2(520, 600), ImGuiCond.FirstUseEver);
            ImGui.Begin("Console", ImGuiWindowFlags.NoScrollbar);

            float heightReserved = ImGui.GetStyle().ItemSpacing.Y + ImGui.GetFrameHeightWithSpacing();

            ImGui.BeginChild("ScrollingRegion", new Vector2(0,           -heightReserved), true, ImGuiWindowFlags.HorizontalScrollbar);
            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(4, 1));

            for (int i = 0; i != _consoleLog.Count; i++) {
                ConsoleResult current = _consoleLog[i];

                switch (current.Result) {
                    case ExecutionResult.Log:
                    case ExecutionResult.Success:
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

                ImGui.TextUnformatted(current.Message);
                ImGui.PopStyleColor();
            }

            ImGui.PopStyleVar();
            ImGui.EndChild();
            ImGui.Separator();

            ImGuiInputTextFlags textFlags = ImGuiInputTextFlags.EnterReturnsTrue | ImGuiInputTextFlags.CallbackCompletion | ImGuiInputTextFlags.CallbackHistory;

            ImGui.PushItemWidth(-1);

            if (ImGui.InputText("", _consoleBuffer, (uint) _consoleBuffer.Length, textFlags, OnTextEdit)) {
                string result = Encoding.UTF8.GetString(_consoleBuffer).Trim();

                _consoleLog.Add(new ConsoleResult(ExecutionResult.Message, $"] {result}"));

                var consoleResult = DevConsole.Run(result);

                _consoleLog.Add(consoleResult);

                _consoleBuffer = new byte[4096];

                ImGui.SetKeyboardFocusHere(-1);
            }

            ImGui.PopItemWidth();
            ImGui.SetItemDefaultFocus();
            ImGui.End();

        }

        private static unsafe int OnTextEdit(ImGuiInputTextCallbackData* data) {
            return 0;
        }

        public static void ScrollToBottom() {
            if(ImGui.GetScrollY() >= ImGui.GetScrollMaxY())
                ImGui.SetScrollHereY(1.0f);
        }
    }
}

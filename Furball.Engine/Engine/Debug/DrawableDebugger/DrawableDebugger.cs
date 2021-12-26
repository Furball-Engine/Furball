using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using ImGuiNET;
using Silk.NET.Input;

namespace Furball.Engine.Engine.Debug.DrawableDebugger {
    public static class DrawableDebugger {
        public static bool Visible = false;

        public static void Initialize() {
            FurballGame.InputManager.OnKeyDown += (sender, key) => {
                if (key == Key.F1 && (FurballGame.InputManager.HeldKeys.Contains(Key.ControlLeft) || FurballGame.InputManager.HeldKeys.Contains(Key.ControlRight)) &&
                    !Visible) {
                    Visible = true;
                }
            };
        }

        public static void Draw() {
            if (Visible) {
                ImGui.Begin("Drawable Debugger", ref Visible, ImGuiWindowFlags.HorizontalScrollbar);

                ImGui.Text("Warning! Expanding large DrawableManagers may slow down the entire application!");
                ImGui.Separator();

                for (int i = 0; i != DrawableManager.DrawableManagers.Count; i++) {
                    DrawableManager manager = DrawableManager.DrawableManagers[i];

                    if (ImGui.TreeNode(manager.Name, manager.Name)) {
                        for (int j = 0; j != manager.Drawables.Count; j++) {
                            BaseDrawable currentDrawable = manager.Drawables[j];

                            string shownName = currentDrawable.Name == " " ? currentDrawable.GetType().Name : currentDrawable.Name;

                            if (ImGui.TreeNode(shownName)) {
                                currentDrawable.DrawableDebuggerDraw();

                                ImGui.TreePop();
                            }
                        }

                        ImGui.TreePop();
                    }
                }

                ImGui.End();
            }
        }
    }
}

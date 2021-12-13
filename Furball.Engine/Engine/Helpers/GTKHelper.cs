using System;
using System.Threading;
using Furball.Vixie.Graphics;
using Gtk;

namespace Furball.Engine.Engine.Helpers {
    public static class GtkHelper {
        private static Window _Window;

        public static void Initialize() {
            Application.Init();

            _Window = new("dummy window (you should not see this)");

            Thread t = new(
            () => {
                Application.Run();
            }
            );
            t.Start();
        }

        public static void Dispose() {
            _Window.Destroy();
            Application.Quit();
        }

        public static Color ColorChooserDialog(string title) {
            ColorChooserDialog dialog = new(title, _Window);

            dialog.ShowEditor = true;
            dialog.UseAlpha   = true;

            dialog.Run();

            Color color = new() {
                R = (byte)Math.Round(dialog.Rgba.Red   * 255d),
                G = (byte)Math.Round(dialog.Rgba.Green * 255d),
                B = (byte)Math.Round(dialog.Rgba.Blue  * 255d),
                A = (byte)Math.Round(dialog.Rgba.Alpha * 255d)
            };

            dialog.Destroy();

            return color;
        }

        public static void ColorChooserDialogAsync(string title, EventHandler<Color> callback = null) {
            new Thread(
            () => {
                ColorChooserDialog dialog = new(title, _Window);

                dialog.ShowEditor = true;
                dialog.UseAlpha   = true;

                dialog.Run();

                Color color = new() {
                    R = (byte)Math.Round(dialog.Rgba.Red   * 255d),
                    G = (byte)Math.Round(dialog.Rgba.Green * 255d),
                    B = (byte)Math.Round(dialog.Rgba.Blue  * 255d),
                    A = (byte)Math.Round(dialog.Rgba.Alpha * 255d)
                };

                dialog.Destroy();

                callback?.Invoke(null, color);
            }
            ).Start();
        }

        public static void MessageDialogAsync(string title, string contents, MessageType messageType, ButtonsType type, EventHandler<ResponseType> callback = null) {
            new Thread(
            () => {
                MessageDialog dialog = new(_Window, DialogFlags.DestroyWithParent, messageType, type, contents);
                dialog.Title = title;

                ResponseType result = (ResponseType)dialog.Run();

                dialog.Destroy();

                callback?.Invoke(null, result);
            }
            ).Start();
        }

        public static ResponseType MessageDialog(string title, string contents, MessageType messageType, ButtonsType type) {
            MessageDialog dialog = new(_Window, DialogFlags.DestroyWithParent, messageType, type, contents);
            dialog.Title = title;

            ResponseType result = (ResponseType)dialog.Run();

            dialog.Destroy();

            return result;
        }
    }
}

using System.Threading;
using Gtk;

namespace Furball.Engine.Engine.Helpers {
    public static class GtkHelper {
        private static Window _Window;

        private static bool _StopThread = false;

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

        public static void MessageDialogAsync(string title, string contents, MessageType messageType, ButtonsType type) {
            new Thread(
            () => {
                MessageDialog dialog = new(_Window, DialogFlags.DestroyWithParent, messageType, type, contents);
                dialog.Title = title;

                dialog.Run();

                dialog.Destroy();
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

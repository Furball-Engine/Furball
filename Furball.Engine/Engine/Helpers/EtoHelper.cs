using System;
using System.Runtime.InteropServices;
using System.Threading;
using Eto;
using Eto.Forms;
using Furball.Engine.Engine.Helpers.Logger;
using Furball.Engine.Engine.Platform;
using Furball.Vixie.Backends.Shared;

namespace Furball.Engine.Engine.Helpers;

public static class EtoHelper {
    private static Thread      _thread;
    private static Application _app;

    private static bool _initCalled = false;

    public static void Initialize() {
        Profiler.StartProfile("init_eto");

        _initCalled = true;

        _thread = new Thread(
        () => {
            string platform = Platforms.Gtk;

            if (RuntimeInfo.CurrentPlatform() == OSPlatform.Windows) {
                platform = Platforms.WinForms;
            }

            Eto.Platform.Initialize(platform);

            Kettu.Logger.Log("Eto Initialized", LoggerLevelEtoInfo.Instance);

            _app = new Application();
            _app.Run();
        }
        );
        //If we are on Windows, set the apartment state to STA, which is required for WinForms to work
        if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            _thread.SetApartmentState(ApartmentState.STA);

        _thread.Start();

        Profiler.EndProfileAndPrint("init_eto");
    }

    private static bool _QuitInvoked = false;
    public static void Dispose() {
        while (_initCalled && _app == null)
            Thread.Sleep(150);

        if (!_QuitInvoked) {
            _QuitInvoked = true;
            _app?.Invoke(
            () => {
                _app?.Quit();
            }
            );
        }
    }

    public static void MessageDialog(EventHandler<DialogResult> callback, string message, MessageBoxButtons buttons) {
        _app?.InvokeAsync(
        () => {
            DialogResult response = MessageBox.Show(message, buttons);
            callback.Invoke(_app, response);
        }
        );
    }

    public static void OpenColorPicker(EventHandler<(DialogResult result, Color color)> callback, Color existingColor, string title = "Color Picker", bool allowAlpha = true) {
        _app?.InvokeAsync(
        () => {
            ColorDialog dialog = new ColorDialog();
            dialog.AllowAlpha = allowAlpha;
            dialog.Color      = existingColor.ToEto();

            DialogResult result = dialog.ShowDialog(_app.MainForm);

            callback.Invoke(_app, (result, dialog.Color.ToVixie()));
        }
        );
    }

    public static Eto.Drawing.Color ToEto(this Color color) => new Eto.Drawing.Color(color.Rf, color.Gf, color.Bf, color.Af);
    public static Color ToVixie(this Eto.Drawing.Color color) => new Color(color.R, color.G, color.B, color.A);
}

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

                if (RuntimeInformation.FrameworkDescription.Contains("Framework"))
                    platform = Platforms.Gtk;
            }

            Eto.Platform.Initialize(platform);

            Kettu.Logger.Log("Eto Initialized", LoggerLevelEtoInfo.Instance);

            _app = new Application();
            _app.Run();
        }
        );
        //If we are on winows, set the apartment state to STA
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

    public static void OpenColorPicker(EventHandler<Color> callback, Color existingColor, string title = "Color Picker", bool allowAlpha = true) {
        _app?.Invoke(
        () => {
            ColorPickerForm form = new ColorPickerForm(title, allowAlpha);

            form.ColorPicker.Value = new Eto.Drawing.Color(existingColor.R / 255f, existingColor.G / 255f, existingColor.B / 255f, existingColor.A / 255f);

            bool preventClosure = true;

            form.Show();
            form.BringToFront();
            form.ColorPicker.ValueChanged += (_, _) => {
                Eto.Drawing.Color color = form.ColorPicker.Value;

                callback.Invoke(_app, new Color(color.Rb, color.Gb, color.Bb, color.Ab));

                preventClosure = false;
                _app.Invoke(
                () => {
                    form.Close();
                }
                );
            };
            form.Closing += (_, args) => {
                if (preventClosure)
                    args.Cancel = true;
            };
        }
        );
    }

    private class ColorPickerForm : Form {
        public ColorPicker ColorPicker;

        public ColorPickerForm(string title = "Color Picker", bool allowAlpha = true) {
            _app.Invoke(
            () => {
                this.Title = title;
                // ClientSize = new Size(200, 200);
                this.Content = this.ColorPicker = new ColorPicker {
                    AllowAlpha = allowAlpha
                };
                this.Resizable = false;
            }
            );
        }
    }
}

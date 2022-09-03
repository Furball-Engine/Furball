using System;
using System.Threading;
using Eto;
using Eto.Forms;
using Furball.Engine.Engine.Helpers.Logger;
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
            Eto.Platform.Initialize(Platforms.Gtk);

            Kettu.Logger.Log("Eto Initialized", LoggerLevelEtoInfo.Instance);
                
            _app = new Application();
            _app.Run();
        }
        );

        _thread.Start();

        Profiler.EndProfileAndPrint("init_eto");
    }

    public static void Dispose() {
        while (_initCalled && _app == null) {
            Thread.Sleep(150);
        }

        _app?.Invoke(() => {
            _app?.Quit();
        });
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
            ColorPickerForm form = new(title, allowAlpha);

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
                    AllowAlpha = true
                };
                this.Resizable = false;
            }
            );
        }
    }
}
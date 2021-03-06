using System;
using System.Threading;
using Eto;
using Eto.Forms;
using Furball.Engine.Engine.Helpers.Logger;
using Furball.Vixie.Backends.Shared;

namespace Furball.Engine.Engine.Helpers; 

public static class EtoHelper {
    private static Thread      _Thread;
    private static Application App;

    private static bool InitCalled = false;

    public static void Initialize() {
        Profiler.StartProfile("init_eto");

        InitCalled = true;
            
        _Thread = new Thread(
        () => {
            Eto.Platform.Initialize(Platforms.Gtk);

            Kettu.Logger.Log("Eto Initialized", LoggerLevelEtoInfo.Instance);
                
            App = new Application();
            App.Run();
        }
        );

        _Thread.Start();

        Profiler.EndProfileAndPrint("init_eto");
    }

    public static void Dispose() {
        while (InitCalled && App == null) {
            Thread.Sleep(150);
        }

        App?.Invoke(() => App?.Quit());
    }

    public static DialogResult MessageDialog(string message, MessageBoxButtons buttons) {
        DialogResult response = DialogResult.Cancel;

        App?.InvokeAsync(
        () => {
            response = MessageBox.Show(message, buttons);
        }
        ).Wait();

        return response;
    }

    public static void OpenColorPicker(EventHandler<Color> callback, Color existingColor, string title = "Color Picker", bool allowAlpha = true) {
        App?.Invoke(
        () => {
            ColorPickerForm form = new(title, allowAlpha);

            form.ColorPicker.Value = new Eto.Drawing.Color(existingColor.R / 255f, existingColor.G / 255f, existingColor.B / 255f, existingColor.A / 255f);

            bool preventClosure = true;
                
            form.Show();
            form.BringToFront();
            form.ColorPicker.ValueChanged += (_, _) => {
                Eto.Drawing.Color color = form.ColorPicker.Value;

                callback.Invoke(App, new Color(color.Rb, color.Gb, color.Bb, color.Ab));

                preventClosure = false;
                App.Invoke(
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

    public class ColorPickerForm : Form {
        public ColorPicker ColorPicker;

        public ColorPickerForm(string title = "Color Picker", bool allowAlpha = true) {
            App.Invoke(
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
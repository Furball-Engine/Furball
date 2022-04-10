using System;
using System.Threading;
using Eto.Forms;
using Furball.Vixie.Backends.Shared;

namespace Furball.Engine.Engine.Helpers {
    public static class EtoHelper {
        private static Thread      _Thread;
        private static Application App;

        private static bool InitCalled = false;

        public static void Initialize() {
            InitCalled = true;

            Eto.Platform.Initialize(Eto.Platform.Detect);

            _Thread = new(
            () => {
                App = new();
                App.Run();
            }
            );

            _Thread.Start();
        }

        public static void Dispose() {
            while (InitCalled && App == null) {
                Thread.Sleep(150);
            }

            App?.Invoke(() => App?.Quit());
        }

        public static void OpenColorPicker(EventHandler<Color> callback, Color existingColor, string title = "Color Picker", bool allowAlpha = true) {
            App?.Invoke(
            () => {
                ColorPickerForm form = new(title, allowAlpha);

                form.ColorPicker.Value = new(existingColor.R / 255f, existingColor.G / 255f, existingColor.B / 255f, existingColor.A / 255f);

                form.Show();
                form.BringToFront();
                form.ColorPicker.ValueChanged += (sender, args) => {
                    Eto.Drawing.Color color = form.ColorPicker.Value;

                    callback.Invoke(App, new Color(color.Rb, color.Gb, color.Bb, color.Ab));

                    form.Close();
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
                    this.Content = this.ColorPicker = new() {
                        AllowAlpha = true
                    };
                    this.Resizable = false;
                }
                );
            }
        }
    }
}

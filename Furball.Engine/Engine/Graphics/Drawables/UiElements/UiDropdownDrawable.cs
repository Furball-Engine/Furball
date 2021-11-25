using System.Collections.Generic;
using FontStashSharp;
using Furball.Engine.Engine.Helpers;
using Microsoft.Xna.Framework;

namespace Furball.Engine.Engine.Graphics.Drawables.UiElements {
    public class UiDropdownDrawable : CompositeDrawable {
        public List<string> Items;
        public Vector2      ButtonSize;
        public FontSystem   Font;
        public int          FontSize;

        public bool Selected = false;

        public Bindable<string> SelectedItem;

        public UiDropdownDrawable(Vector2 position, List<string> items, Vector2 buttonSize, FontSystem font, int fontSize) {
            this.Position   = position;
            this.Items      = items;
            this.ButtonSize = buttonSize;
            this.Font       = font;
            this.FontSize   = fontSize;

            this.Clickable = false;

            this.SelectedItem = new(items[0]);

            this.Update();
        }

        public void Update() {
            this._drawables.Clear();

            if (this.Selected) {
                UiButtonDrawable element = new(new(0, 0), this.SelectedItem, this.Font, this.FontSize, Color.Blue, Color.White, Color.Black, this.ButtonSize);

                element.OnClick += delegate {
                    this.Selected = false;

                    this.Update();
                };

                this._drawables.Add(element);

                float y = element.Size.Y;
                foreach (string item in this.Items) {
                    if (item == this.SelectedItem) continue;

                    element = new(new(0, y), item, this.Font, this.FontSize, Color.Blue, Color.White, Color.Black, this.ButtonSize);

                    element.OnClick += delegate {
                        this.SelectedItem.Value = item;

                        this.Selected = false;

                        this.Update();
                    };

                    this._drawables.Add(element);

                    y += element.Size.Y;
                }
            } else {
                UiButtonDrawable element = new(new(0, 0), this.SelectedItem, this.Font, this.FontSize, Color.Blue, Color.White, Color.Black, this.ButtonSize);

                element.OnClick += delegate {
                    this.Selected = true;

                    this.Update();
                };

                this._drawables.Add(element);
            }
        }
    }
}

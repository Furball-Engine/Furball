using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FontStashSharp;
using Furball.Engine.Engine.Helpers;
using Furball.Vixie.Backends.Shared;

namespace Furball.Engine.Engine.Graphics.Drawables.UiElements {
    public class UiDropdownDrawable : CompositeDrawable {
        public Dictionary<object, string> Items;
        public Vector2      ButtonSize;
        public FontSystem   Font;
        public int          FontSize;

        public bool Selected = false;

        public Bindable<KeyValuePair<object, string>> SelectedItem;

        public UiDropdownDrawable(Vector2 position, Dictionary<object, string> items, Vector2 buttonSize, FontSystem font, int fontSize) {
            this.Position   = position;
            this.Items      = items;
            this.ButtonSize = buttonSize;
            this.Font       = font;
            this.FontSize   = fontSize;

            this.Clickable = false;

            if (items.Count == 0)
                throw new InvalidOperationException("Your dropdown needs at least one item!");
            
            this.SelectedItem = new(items.First());

            this.Update();
        }

        public void Update() {
            this.Drawables.Clear();

            if (this.Selected) {
                UiButtonDrawable element = new(new(0, 0), this.SelectedItem.Value.Value, this.Font, this.FontSize, Color.Blue, Color.White, Color.Black, this.ButtonSize);

                element.OnClick += delegate {
                    this.Selected = false;

                    this.Update();
                };

                this.Drawables.Add(element);

                float y = element.Size.Y;
                foreach (KeyValuePair<object, string> item in this.Items) {
                    if (item.Key == this.SelectedItem.Value.Key) continue;

                    element = new(new(0, y), item.Value, this.Font, this.FontSize, Color.Blue, Color.White, Color.Black, this.ButtonSize);

                    element.OnClick += delegate {
                        this.SelectedItem.Value = item;

                        this.Selected = false;

                        this.Update();
                    };

                    this.Drawables.Add(element);

                    y += element.Size.Y;
                }
            } else {
                UiButtonDrawable element = new(new(0, 0), this.SelectedItem.Value.Value, this.Font, this.FontSize, Color.Blue, Color.White, Color.Black, this.ButtonSize);

                element.OnClick += delegate {
                    this.Selected = true;

                    this.Update();
                };

                this.Drawables.Add(element);
            }
        }
    }
}

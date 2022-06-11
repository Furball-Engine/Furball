using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FontStashSharp;
using Furball.Engine.Engine.Helpers;
using Furball.Vixie.Backends.Shared;

namespace Furball.Engine.Engine.Graphics.Drawables.UiElements {
    public class DrawableDropdown : CompositeDrawable {
        public Dictionary<object, string> Items;
        public Vector2      ButtonSize;
        public FontSystem   Font;
        public int          FontSize;

        public bool Selected = false;

        public Bindable<KeyValuePair<object, string>> SelectedItem;

        public DrawableDropdown(Vector2 position, FontSystem font, int fontSize, Vector2 buttonSize, Dictionary<object, string> items) {
            this.Position   = position;
            this.Items      = items;
            this.ButtonSize = buttonSize;
            this.Font       = font;
            this.FontSize   = fontSize;

            this.Clickable = false;

            if (items.Count == 0)
                throw new InvalidOperationException("Your dropdown needs at least one item!");
            
            this.SelectedItem = new Bindable<KeyValuePair<object, string>>(items.First());

            this.Update();
        }

        public void Update() {
            this.Drawables.Clear();

            if (this.Selected) {
                DrawableButton element = new(
                new Vector2(0, 0),
                this.Font,
                this.FontSize,
                this.SelectedItem.Value.Value,
                Color.Blue,
                Color.White,
                Color.Black,
                this.ButtonSize
                );

                element.OnClick += delegate {
                    this.Selected = false;

                    this.Update();
                };

                this.Drawables.Add(element);

                float y = element.Size.Y;
                foreach (KeyValuePair<object, string> item in this.Items) {
                    if (item.Key == this.SelectedItem.Value.Key) continue;

                    element = new DrawableButton(new Vector2(0, y), this.Font, this.FontSize, item.Value, Color.Blue, Color.White, Color.Black, this.ButtonSize);

                    element.OnClick += delegate {
                        this.SelectedItem.Value = item;

                        this.Selected = false;

                        this.Update();
                    };

                    this.Drawables.Add(element);

                    y += element.Size.Y;
                }
            } else {
                DrawableButton element = new(
                new Vector2(0, 0),
                this.Font,
                this.FontSize,
                this.SelectedItem.Value.Value,
                Color.Blue,
                Color.White,
                Color.Black,
                this.ButtonSize
                );

                element.OnClick += delegate {
                    this.Selected = true;

                    this.Update();
                };

                this.Drawables.Add(element);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FontStashSharp;
using Furball.Engine.Engine.Helpers;
using Furball.Vixie.Backends.Shared;

namespace Furball.Engine.Engine.Graphics.Drawables.UiElements; 

public class DrawableDropdown : CompositeDrawable {
    public Dictionary<object, string> Items;
    public Vector2                    ButtonSize;
    public FontSystem                 Font;
    public int                        FontSize;

    public bool Selected = false;

    public Bindable<KeyValuePair<object, string>> SelectedItem;

    private float? _outlineThickness;
    public float OutlineThickness {
        get => ((DrawableButton)this.Children.First(x => x is DrawableButton)).OutlineThickness;
        set {
            this._outlineThickness = value;
            foreach (Drawable drawable in this.Children) {
                if (drawable is DrawableButton button) {
                    button.OutlineThickness = this._outlineThickness.Value;
                }
            }
        }
    }

    public DrawableDropdown(Vector2 position, FontSystem font, int fontSize, Vector2 buttonSize, Dictionary<object, string> items) {
        this.Position   = position;
        this.Items      = items;
        this.ButtonSize = buttonSize;
        this.Font       = font;
        this.FontSize   = fontSize;

        this.InvisibleToInput = true;

        if (items.Count == 0)
            throw new InvalidOperationException("Your dropdown needs at least one item!");
            
        this.SelectedItem = new Bindable<KeyValuePair<object, string>>(items.First());

        this.SelectedItem.OnChange += this.OnChange;

        this.Update();
    }

    private void OnChange(object sender, KeyValuePair<object, string> e) {
        this.Update();
    }

    public void Update() {
        foreach (Drawable child in this.Children!) {
            child.Dispose();
        }
        this.Children!.Clear();

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

            if (this._outlineThickness.HasValue) {
                element.OutlineThickness = this._outlineThickness.Value;
            }

            element.OnClick += delegate {
                this.Selected = false;

                this.Update();
            };

            this.Children.Add(element);
            element.RegisterForInput();

            float y = element.Size.Y;
            foreach (KeyValuePair<object, string> item in this.Items) {
                if (item.Key == this.SelectedItem.Value.Key) continue;

                element = new DrawableButton(new Vector2(0, y), this.Font, this.FontSize, item.Value, Color.Blue, Color.White, Color.Black, this.ButtonSize);

                if (this._outlineThickness.HasValue) {
                    element.OutlineThickness = this._outlineThickness.Value;
                }
                
                element.OnClick += delegate {
                    this.Selected = false;
                    
                    this.SelectedItem.Value = item;
                };

                this.Children.Add(element);
                element.RegisterForInput();

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

            if (this._outlineThickness.HasValue) {
                element.OutlineThickness = this._outlineThickness.Value;
            }

            element.OnClick += delegate {
                this.Selected = true;

                this.Update();
            };

            this.Children.Add(element);
            element.RegisterForInput();
        }
    }
}
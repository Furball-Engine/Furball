using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using Furball.Engine.Engine.Helpers;
using Furball.Vixie.Backends.Shared;

namespace Furball.Engine.Engine.Graphics.Drawables.Debug.SceneViewer;

public class DebugSceneViewerEditorPane : CompositeDrawable {
    private readonly Bindable<Drawable>  _selected;
    private readonly FixedTimeStepMethod _update;

    public const float WIDTH = 400;

    public override Vector2 Size => new Vector2(WIDTH, 600);

    public DebugSceneViewerEditorPane(Bindable<Drawable> selected) {
        this._selected = selected;

        this._selected.OnChange += this.SelectedChange;
        
        FurballGame.TimeStepMethods.Add(this._update = new FixedTimeStepMethod(500, this.UpdateUI));
    }

    public override void Dispose() {
        base.Dispose();

        this._selected.OnChange -= this.SelectedChange;
        
        FurballGame.TimeStepMethods.Remove(this._update);
    }

    private void SelectedChange(object sender, Drawable e) {
        this.UpdateUI();
    }

    private void UpdateUI() {
        this.Children.Clear();
        
        //Dont do anything if the selection is now null
        if (this._selected.Value == null) return;

        float y = 0;
        //Iterate over all fields on the selected drawable, and put them in the UI
        foreach (FieldInfo field in this._selected.Value.GetType().GetFields(BindingFlags.Default | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
            //Get the field value
            object? value = field.GetValue(this._selected.Value);

            TextDrawable text = new TextDrawable(new Vector2(0, y), FurballGame.DefaultFont, $"{field.Name}: {GetFancyString(value)}", 20);
            y += text.Size.Y;
            
            this.Children.Add(text);
        }
        
        //Iterate over all properties on the selected drawable, and put them in the UI
        foreach (PropertyInfo property in this._selected.Value.GetType().GetProperties(BindingFlags.Default | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
            //Get the property value
            object? value = property.GetValue(this._selected.Value);

            TextDrawable text = new TextDrawable(new Vector2(0, y), FurballGame.DefaultFont, $"{property.Name}: {GetFancyString(value)}", 20) {
                ColorOverride = Color.LightBlue
            };
            y += text.Size.Y;
            
            this.Children.Add(text);
        }
    }

    public string GetFancyString(object value) {
        if (value == null) return "null";

        if (value is string str) return $"\"{str}\"";
        
        //If the value is a list of any kind, iterate over it and get the fancy string of each item
        if (value is IEnumerable<object> enumerable) {
            string result = "[";
            foreach (object item in enumerable) {
                result += $"{this.GetFancyString(item)}, ";
            }

            return result.TrimEnd(',', ' ') + "]";
        }
        
        //If the value is an array of any kind, iterate over it and get the fancy string of each item
        if (value is Array array) {
            string result = "[";
            foreach (object item in array) {
                result += $"{this.GetFancyString(item)}, ";
            }

            return result.TrimEnd(',', ' ') + "]";
        }
        
        //If the value is a dictionary, iterate over it and get the fancy string of each key and value
        if (value is IDictionary<object, object> dictionary) {
            string result = "{";
            foreach (KeyValuePair<object, object> item in dictionary) {
                result += $"{this.GetFancyString(item.Key)}: {this.GetFancyString(item.Value)}, ";
            }

            return result.TrimEnd(',', ' ') + "}";
        }

        return value.ToString();
    }
}

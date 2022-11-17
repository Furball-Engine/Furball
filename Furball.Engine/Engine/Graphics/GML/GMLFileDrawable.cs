#nullable enable
using System;
using System.Linq;
using System.Numerics;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using GMLSharp;

namespace Furball.Engine.Engine.Graphics.GML;

public class GMLFileDrawable : Drawable, IGMLElement {
    private Vector2 _size = Vector2.Zero;

    public override Vector2 Size => this._size;

    public GMLFile? File {
        get;
        private set;
    }

    public GMLTheme Theme = new GMLTheme();

    public GMLFileDrawable(Vector2 position) {
        this.Position = position;
    }

    public void SetGMLFile(GMLFile file) {
        this.File = file;
        this.Invalidate();
    }

    public void Invalidate() {
        this._size = Vector2.Zero;

        if (this.File == null)
            return;

        this._size = this.ElementSize();
    }

    public Vector2 ElementSize() {
        Vector2 size = Vector2.Zero;
        
        if (this.File == null)
            return size;

        if (this.File.MainClass.Properties.LastOrDefault(
            x => x is KeyValuePair {
                Key: "fixed_width"
            }
            ) is KeyValuePair {
                Value: JsonValueNode {
                    Value: {}
                } fixedWidth
            })
            size.X = Convert.ToSingle(fixedWidth.Value);
        if (this.File.MainClass.Properties.LastOrDefault(
            x => x is KeyValuePair {
                Key: "fixed_height"
            }
            ) is KeyValuePair {
                Value: JsonValueNode {
                    Value: {}
                } fixedHeight
            })
            size.Y = Convert.ToSingle(fixedHeight.Value);
        
        //TODO: parse the other types of width/height, along with recursively going through to find the size of the children

        if (size.X == 0 || size.Y == 0)
            throw new Exception("Unable to determine size!");

        return size;
    }

    public bool FillWithBackgroundColor() {
        if (this.File == null)
            return false;
        
        return this.File.MainClass.Properties.LastOrDefault(
               x => x is KeyValuePair {
                   Key: "fill_with_background_color"
               }
               ) is KeyValuePair {
                   Value: JsonValueNode {
                       Value: {}
                   } fillWithBackgroundColor
               } && Convert.ToBoolean(fillWithBackgroundColor.Value);
    }

    public override void Draw(double time, DrawableBatch batch, DrawableManagerArgs args) {
        if(this.FillWithBackgroundColor())
            batch.Draw(FurballGame.WhitePixel, args.Position, this.Size * args.Scale, this.Theme.BackgroundFillColor);
    }
}

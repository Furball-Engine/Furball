#nullable enable
using System;
using System.Linq;
using System.Numerics;
using Furball.Engine.Engine.Graphics.Drawables;
using GMLSharp;

namespace Furball.Engine.Engine.Graphics.GML;

public class GMLFileDrawable : Drawable, IGMLElement {
    private Vector2 _size = Vector2.Zero;

    public override Vector2 Size => this._size;

    public GMLFile? File {
        get;
        private set;
    }

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

        if (this.File.MainClass.Properties.LastOrDefault(
            x => x is KeyValuePair {
                Key: "fixed_width"
            }
            ) is KeyValuePair {
                Value: JsonValueNode {
                    Value: {}
                } fixedWidth
            })
            this._size.X = Convert.ToSingle(fixedWidth.Value);
        if (this.File.MainClass.Properties.LastOrDefault(
            x => x is KeyValuePair {
                Key: "fixed_height"
            }
            ) is KeyValuePair {
                Value: JsonValueNode {
                    Value: {}
                } fixedHeight
            })
            this._size.Y = Convert.ToSingle(fixedHeight.Value);
        
        //TODO: parse the other types of width/height

        if (this._size == Vector2.Zero)
            throw new Exception("Unable to determine size!");
    }
}

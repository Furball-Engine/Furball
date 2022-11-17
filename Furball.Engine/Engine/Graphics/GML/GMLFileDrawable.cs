#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Graphics.GML.Elements;
using Furball.Vixie.Helpers.Helpers;
using GMLSharp;
using Object=GMLSharp.Object;

namespace Furball.Engine.Engine.Graphics.GML;

public class GMLFileDrawable : CompositeDrawable, IGMLElement {
    private Vector2 _size = Vector2.Zero;

    public override Vector2 Size => this._size;

    public GMLFile? File {
        get;
        private set;
    }

    public GMLTheme Theme = new GMLTheme();

    private List<GMLElementDrawable> CalculatedElements;

    public GMLFileDrawable(Vector2 position) {
        this.Position = position;

        this.InvisibleToInput = true;
    }

    public void SetGMLFile(GMLFile file) {
        this.File = file;
        this.Invalidate();
    }

    public void Invalidate() {
        this.Children.Clear();
        
        this._size = Vector2.Zero;

        if (this.File == null)
            return;

        GMLCalculatedElementSize calcedSize = this.MinimumSize();
        this._size = new Vector2(calcedSize.Width.Size, calcedSize.Height.Size);

        List<GMLElementDrawable> workingElements = new List<GMLElementDrawable>();

        //First pass to create all the working elements, and calculate their minimum sizes
        foreach (Node node in this.File.MainClass.SubObjects) {
            if (node is Object subObject) {
                switch (subObject.Name) {
                    case "GUI::Label": {
                        GMLLabelElement label = new GMLLabelElement(subObject, this.Theme);
                        workingElements.Add(new GMLElementDrawable(label, this.Theme));

                        break;
                    }
                    case "GUI::Button": {
                        GMLButtonElement button = new GMLButtonElement(subObject, this.Theme);
                        workingElements.Add(new GMLElementDrawable(button, this.Theme));

                        break;
                    }
                    default: throw new NotImplementedException($"Unknown GML Object {subObject.Name}");
                }
            }
        }

        //TODO: support multiple layout types
        this.CalculateSizesForVerticallyStackedLayout(workingElements);
        this.CalculatePositionsForVerticallyStackedLayout(workingElements);

        foreach (GMLElementDrawable element in workingElements) {
            element.Invalidate();
        }
        
        this.Children.AddRange(workingElements);
    }

    private void CalculatePositionsForVerticallyStackedLayout(List<GMLElementDrawable> workingElements) {
        //TODO: implement padding, currently elements will be stacked on top of each other
        float y = 0;
        foreach (GMLElementDrawable element in workingElements) {
            element.Position.Y =  y;
            y                  += element.CalculatedSize.Y;
        }
    }

    private void CalculateSizesForVerticallyStackedLayout(List<GMLElementDrawable> workingElements) {
        float remainingHeight = this._size.Y;

        int expandToFitEements = workingElements.Count(x => x.MinimumSize.Height.Type == SizeType.ExpandToFit);

        //Allocate fixed height elements
        foreach (GMLElementDrawable element in workingElements) {
            GMLCalculatedElementSize minSize = element.MinimumSize;

            //If the size is fixed, then it takes up *that* much space, and take it
            if (minSize.Height.Type == SizeType.Fixed) {
                remainingHeight -= minSize.Height.Size;

                element.CalculatedSize.Y = minSize.Height.Size;
            }
        }

        //Allocate the heights of expand-to-fit elements
        foreach (GMLElementDrawable element in workingElements) {
            GMLCalculatedElementSize minSize = element.MinimumSize;

            if (minSize.Height.Type != SizeType.ExpandToFit)
                continue;

            //The size of the element is the remaining height (which is the height after the fixed elements have taken their share)
            //divided by the number of expand-to-fit elements
            element.CalculatedSize.Y = remainingHeight / expandToFitEements;
        }

        //Calculate the widths of all the elements
        foreach (GMLElementDrawable element in workingElements) {
            GMLCalculatedElementSize minSize = element.MinimumSize;

            element.CalculatedSize.X = minSize.Width.Type switch {
                SizeType.Fixed       => minSize.Width.Size,                                           //set the width to the element's fixed width
                SizeType.ExpandToFit => this._size.X.Clamp(minSize.Width.Size, minSize.Width.MaxSize),//clamp to the min/max size
                _                    => throw new NotImplementedException($"Unknown SizeType {minSize.Width.Type} for element width")
            };
        }
    }

    public GMLCalculatedElementSize MinimumSize() {
        Vector2 size = Vector2.Zero;

        if (this.File == null)
            return new GMLCalculatedElementSize();

        float? fixedWidth = this.File.MainClass.FixedWidth();
        if (fixedWidth != null)
            size.X = fixedWidth.Value;

        float? fixedHeight = this.File.MainClass.FixedHeight();
        if (fixedWidth != null)
            size.Y = fixedHeight.Value;

        //TODO: parse the other types of width/height, along with recursively going through to find the size of the children

        if (size.X == 0 || size.Y == 0)
            throw new Exception("Unable to determine size!");

        return new GMLCalculatedElementSize {
            Width = new GMLSize {
                Size = size.X,
                Type = SizeType.Fixed
            },
            Height = new GMLSize {
                Size = size.Y,
                Type = SizeType.Fixed
            }
        };
    }

    public bool FillWithBackgroundColor() {
        return this.File != null && this.File.MainClass.FillWithBackgroundColor();

    }

    public override void Draw(double time, DrawableBatch batch, DrawableManagerArgs args) {
        if (this.FillWithBackgroundColor())
            batch.Draw(FurballGame.WhitePixel, args.Position, this.Size * args.Scale, this.Theme.BackgroundFillColor);
        
        base.Draw(time, batch, args);
    }
}

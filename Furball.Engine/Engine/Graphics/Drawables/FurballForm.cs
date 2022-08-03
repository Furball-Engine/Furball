using System;
using System.Drawing;
using System.Numerics;
using Furball.Engine.Engine.Graphics.Drawables.Primitives;
using Furball.Engine.Engine.Helpers;
using Furball.Engine.Engine.Helpers.Logger;
using Kettu;
using Silk.NET.Input;
using Color=Furball.Vixie.Backends.Shared.Color;

namespace Furball.Engine.Engine.Graphics.Drawables;

public class FurballForm : CompositeDrawable {
    public Drawable Contents {
        get;
    }

    private readonly TextDrawable               _title;
    private readonly TextDrawable               _closeButton;
    private readonly RectanglePrimitiveDrawable _titleBar;
    private readonly RectanglePrimitiveDrawable _background;

    public event EventHandler OnTryClose;

    public FurballForm(string title, Drawable contents, OriginType startPosition = OriginType.Center) {
        this.Contents = contents;

        this._title = new TextDrawable(new Vector2(2), FurballGame.DEFAULT_FONT, title, 20) {
            Clickable   = false,
            CoverClicks = false,
            Hoverable   = false,
            CoverHovers = false
        };
        this._closeButton = new TextDrawable(new Vector2(this.Contents.Size.X + 8, 2), FurballGame.DEFAULT_FONT, "x", 20) {
            OriginType = OriginType.TopRight,
            Depth      = 2f
        };
        this._titleBar = new RectanglePrimitiveDrawable(new Vector2(0), new Vector2(this.Contents.Size.X + 10, 24), 0, true) {
            ColorOverride = new Color(45, 45, 45, 175)
        };

        //We make the background a little bigger so there is margin
        this._background = new RectanglePrimitiveDrawable(new Vector2(0, 24), this.Contents.Size + new Vector2(10), 0, true) {
            ColorOverride = new Color(30, 30, 30, 175),
            Depth         = 0f
        };

        //Center it in the margin
        this.Contents.Position = new Vector2(5, 29);

        //Make sure the contents are above the background
        this.Contents.Depth = 2f;

        this.Drawables.Add(this._titleBar);
        this.Drawables.Add(this._title);
        this.Drawables.Add(this._closeButton);

        this.Drawables.Add(this._background);
        this.Drawables.Add(this.Contents);

        Vector2 size = base.Size;

        //TODO: account of the rest of the members of `OriginType`
        this.Position = startPosition switch {
            OriginType.Center => new Vector2(FurballGame.DEFAULT_WINDOW_WIDTH / 2f - size.X / 2, FurballGame.DEFAULT_WINDOW_HEIGHT / 2f - size.Y / 2f),
            _                 => this.Position
        };

        #region Events

        this._titleBar.OnDragBegin += this.OnTitleBarDragBegin;
        this._titleBar.OnDrag      += this.OnTitleBarDrag;

        this._closeButton.OnClick += this.OnCloseButtonClick;

        #endregion

        Logger.Log($"Created form with title {title}", LoggerLevelFurballFormInfo.Instance);
    }

    private void OnCloseButtonClick(object _, (MouseButton button, Point pos) valueTuple) {
        if (this.OnTryClose == null) {
            Logger.Log("Unhandled FurballForm close!", LoggerLevelFurballFormInfo.Instance);
            return;
        }

        this.OnTryClose.Invoke(this, EventArgs.Empty);
    }

    private Vector2 _startDiffFromPos;
    private Vector2 _startDragMousePos;

    private void OnTitleBarDragBegin(object _, Point startDragPos) {
        this._startDragMousePos = startDragPos.ToVector2();

        this._startDiffFromPos = this._startDragMousePos - this.Position;
    }

    private void OnTitleBarDrag(object _, Point currentDragPos) {
        Vector2 differenceFromStart = currentDragPos.ToVector2() - this._startDragMousePos;

        this.MoveTo(this._startDragMousePos + differenceFromStart - this._startDiffFromPos, 3);
    }
}

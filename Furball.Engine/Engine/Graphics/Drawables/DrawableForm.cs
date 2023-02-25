using System;
using System.Numerics;
using Furball.Engine.Engine.Graphics.Drawables.Primitives;
using Furball.Engine.Engine.Helpers.Logger;
using Furball.Engine.Engine.Input.Events;
using Furball.Vixie.Backends.Shared;
using Kettu;

namespace Furball.Engine.Engine.Graphics.Drawables;

public class DrawableForm : CompositeDrawable {
    public Drawable Contents {
        get;
    }

    private readonly TextDrawable               _title;
    private readonly TextDrawable               _closeButton;
    private readonly RectanglePrimitiveDrawable _titleBar;
    private readonly RectanglePrimitiveDrawable _background;

    public event EventHandler OnTryClose;

    public DrawableForm(string title, Drawable contents, OriginType startPosition = OriginType.Center) {
        this.Contents = contents;

        this._title = new TextDrawable(new Vector2(2), FurballGame.DefaultFont, title, 20) {
            Clickable   = false,
            CoverClicks = false,
            Hoverable   = false,
            CoverHovers = false
        };
        this._closeButton = new TextDrawable(new Vector2(this.Contents.Size.X + 8, 2), FurballGame.DefaultFont, "x", 20) {
            OriginType = OriginType.TopRight,
            Depth      = 0f
        };
        this._titleBar = new RectanglePrimitiveDrawable(new Vector2(0), new Vector2(this.Contents.Size.X + 10, 24), 0, true) {
            ColorOverride = new Color(45, 45, 45, 175),
            Depth         = 2f
        };

        //We make the background a little bigger so there is margin
        this._background = new RectanglePrimitiveDrawable(new Vector2(0, 24), this.Contents.Size + new Vector2(10), 0, true) {
            ColorOverride = new Color(30, 30, 30, 175),
            Depth         = 2f,
            Clickable     = false,
            CoverClicks   = false,
            Hoverable     = false,
            CoverHovers   = false
        };

        //Center it in the margin
        this.Contents.Position = new Vector2(5, 29);

        //Make sure the contents are above the background
        this.Contents.Depth = 0f;

        this.Children!.Add(this._titleBar);
        this.Children.Add(this._title);
        this.Children.Add(this._closeButton);

        this.Children.Add(this._background);
        this.Children.Add(this.Contents);

        Vector2 size = base.Size;

        this.Position = startPosition switch {
            OriginType.Center       => new Vector2(FurballGame.WindowWidth / 2f - size.X / 2, FurballGame.WindowHeight / 2f - size.Y / 2f),
            OriginType.TopLeft      => Vector2.Zero,
            OriginType.TopRight     => new Vector2(FurballGame.WindowWidth - size.X, 0),
            OriginType.BottomLeft   => new Vector2(0, FurballGame.WindowHeight - size.Y),
            OriginType.BottomRight  => new Vector2(FurballGame.WindowWidth, FurballGame.WindowHeight) - size,
            OriginType.TopCenter    => new Vector2(FurballGame.WindowWidth / 2f - size.X / 2, 0),
            OriginType.BottomCenter => new Vector2(FurballGame.WindowWidth / 2f - size.X / 2, FurballGame.WindowHeight - size.Y),
            OriginType.LeftCenter   => new Vector2(0,                                         FurballGame.WindowHeight / 2f - size.Y / 2f),
            OriginType.RightCenter  => new Vector2(FurballGame.WindowWidth - size.X, FurballGame.WindowHeight / 2f - size.Y / 2f),
            _                       => throw new ArgumentOutOfRangeException(nameof (startPosition), startPosition, null)
        };

        #region Events

        this._titleBar.OnDragBegin += this.OnTitleBarDragBegin;
        this._titleBar.OnDrag      += this.OnTitleBarDrag;

        this._closeButton.OnClick += this.OnCloseButtonClick;

        this.RegisterChildrenForInput();

        #endregion

        Logger.Log($"Created form with title {title}", LoggerLevelFurballFormInfo.Instance);
    }

    protected void RegisterChildrenForInput() {
        this._titleBar.RegisterForInput();
        this._closeButton.RegisterForInput();
    }

    protected void UnregisterChildrenForInput() {
        this._titleBar.UnregisterForInput();
        this._closeButton.UnregisterForInput();
    }
    
    private void OnCloseButtonClick(object _, MouseButtonEventArgs mouseButtonEventArgs) {
        if (this.OnTryClose == null) {
            Logger.Log("Unhandled FurballForm close!", LoggerLevelFurballFormInfo.Instance);
            return;
        }

        this.OnTryClose.Invoke(this, EventArgs.Empty);
    }

    private Vector2 _startDiffFromPos;

    private void OnTitleBarDragBegin(object _, MouseDragEventArgs e) {
        this._startDiffFromPos = e.StartPosition - this.Position;
    }

    private void OnTitleBarDrag(object _, MouseDragEventArgs e) {
        Vector2 differenceFromStart = e.Position - e.StartPosition;

        this.MoveTo(e.StartPosition + differenceFromStart - this._startDiffFromPos);
    }
}

using System.Numerics;
using Furball.Engine;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Input.Events;

namespace Furball.Game.Screens.Tests; 

public class CatmullTest : TestScreen {
    private CurveDrawable _pathVisualization;

    private readonly Vector2[] _points = new Vector2[4];

    public override void Initialize() {
        base.Initialize();

        this._points[0] = new Vector2(160,                  -250 + 1280 / 2);
        this._points[1] = new Vector2(160 + ((1280/4) * 1), -250 + (1280/2) - (1280/4));
        this._points[2] = new Vector2(160 + ((1280/4) * 2), -250 + (1280/2) + (1280/4));
        this._points[3] = new Vector2(160 + ((1280/4) * 3), -250 + 1280 / 2);

        TexturedDrawable a1 = new(FurballGame.WhitePixel, this._points[0]) {
            Scale      = new Vector2(16, 16),
            OriginType = OriginType.Center,
            Clickable  = true,
        };
        TexturedDrawable a2 = new(FurballGame.WhitePixel, this._points[1]) {
            Scale      = new Vector2(16, 16),
            OriginType = OriginType.Center,
            Clickable  = true,
        };
        TexturedDrawable a3 = new(FurballGame.WhitePixel, this._points[2]) {
            Scale      = new Vector2(16, 16),
            OriginType = OriginType.Center,
            Clickable  = true,
        };
        TexturedDrawable a4 = new(FurballGame.WhitePixel, this._points[3]) {
            Scale      = new Vector2(16, 16),
            OriginType = OriginType.Center,
            Clickable  = true,
        };

        a1.OnDrag += delegate(object _, MouseDragEventArgs e) {
            this._points[0] = e.Position;
            a1.Position     = e.Position;
        
            this.UpdatePath();
        };
        a2.OnDrag += delegate(object _, MouseDragEventArgs e) {
            this._points[1] = e.Position;
            a2.Position     = e.Position;
        
            this.UpdatePath();
        };
        a3.OnDrag += delegate(object _, MouseDragEventArgs e) {
            this._points[2] = e.Position;
            a3.Position     = e.Position;
            
            this.UpdatePath();
        };
        a4.OnDrag += delegate(object _, MouseDragEventArgs e) {
            this._points[3] = e.Position;
            a4.Position     = e.Position;
            
            this.UpdatePath();
        };

        this.UpdatePath();
        
        this.Manager.Add(a1, a2, a3, a4);
    }

    private void UpdatePath() {
        this.Manager.Remove(this._pathVisualization);
        this._pathVisualization = new CurveDrawable(this._points[0], this._points[1], this._points[2], this._points[3]) {
            Quality = 50, Thickness = 5f, Type = CurveType.CatmullRom
        };
        this.Manager.Add(this._pathVisualization);
    }
}
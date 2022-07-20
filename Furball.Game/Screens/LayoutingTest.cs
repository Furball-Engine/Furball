using System.Numerics;
using Furball.Engine;
using Furball.Engine.Engine.Graphics.Drawables;

namespace Furball.Game.Screens; 

public class LayoutingTest : TestScreen {
    private TexturedDrawable _topLeft;
    private TexturedDrawable _topRight;
    private TexturedDrawable _bottomLeft;
    private TexturedDrawable _bottomRight;
    public override void Initialize() {
        base.Initialize();

        this.Manager.Add(
        this._topLeft = new TexturedDrawable(FurballGame.WhitePixel, Vector2.Zero) {
            ScreenOriginType = OriginType.TopLeft,
            OriginType       = OriginType.TopLeft,
            Scale            = new Vector2(25)
        }
        );
        this.Manager.Add(
        this._topRight = new TexturedDrawable(FurballGame.WhitePixel, Vector2.Zero) {
            ScreenOriginType = OriginType.TopRight,
            OriginType       = OriginType.TopRight,
            Scale            = new Vector2(25)
        }
        );
        this.Manager.Add(
        this._bottomLeft = new TexturedDrawable(FurballGame.WhitePixel, Vector2.Zero) {
            ScreenOriginType = OriginType.BottomLeft,
            OriginType       = OriginType.BottomLeft,
            Scale            = new Vector2(25)
        }
        );
        this.Manager.Add(
        this._bottomRight = new TexturedDrawable(FurballGame.WhitePixel, Vector2.Zero) {
            ScreenOriginType = OriginType.BottomRight,
            OriginType       = OriginType.BottomRight,
            Scale            = new Vector2(25)
        }
        );
    }

    public override void Relayout(float newWidth, float newHeight) {
        base.Relayout(newWidth, newHeight);

        this._topLeft.Position     = new(10);
        this._topRight.Position    = new(10);
        this._bottomLeft.Position  = new(10);
        this._bottomRight.Position = new(10);
        // this._topRight.Position    = new(newWidth - 10, 10);
        // this._bottomLeft.Position  = new(10, newHeight - 10);
        // this._bottomRight.Position = new(newWidth - 10, newHeight - 10);
    }
}
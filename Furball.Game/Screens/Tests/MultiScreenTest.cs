using System.Numerics;
using Furball.Engine.Engine.Graphics.Drawables;

namespace Furball.Game.Screens.Tests;

public class MultiScreenTest : TestScreen {
    private readonly int _recurseAmount;

    private ScreenDrawable _topLeft;
    private ScreenDrawable _topRight;
    private ScreenDrawable _bottomLeft;
    private ScreenDrawable _bottomRight;

    public MultiScreenTest() {
        this._recurseAmount = 0;
    }

    private MultiScreenTest(int recurseAmount) {
        this._recurseAmount = recurseAmount;
    }
    
    public override void Initialize() {
        base.Initialize();

        this.Manager.Add(this._topLeft = new ScreenDrawable(this._recurseAmount < 150 ? new MultiScreenTest(this._recurseAmount + 1) : new LayoutingTest(), Vector2.Zero, Vector2.Zero));
        this.Manager.Add(
        this._topRight = new ScreenDrawable(new FixedTimeStepTest(), Vector2.Zero, Vector2.Zero) {
            OriginType       = OriginType.TopRight,
            ScreenOriginType = OriginType.TopRight
        }
        );
        this.Manager.Add(
        this._bottomLeft = new ScreenDrawable(new AudioEffectTest(), Vector2.Zero, Vector2.Zero) {
            OriginType       = OriginType.BottomLeft,
            ScreenOriginType = OriginType.BottomLeft
        }
        );
        this.Manager.Add(
        this._bottomRight = new ScreenDrawable(new ScreenSelector(), Vector2.Zero, Vector2.Zero) {
            OriginType       = OriginType.BottomRight,
            ScreenOriginType = OriginType.BottomRight
        }
        );
    }

    public override void Relayout(float newWidth, float newHeight) {
        base.Relayout(newWidth, newHeight);

        if(this._topLeft != null) {
            this._topLeft.ScreenSize     = new Vector2(newWidth * 0.8f, newHeight * 0.8f);
            this._topRight.ScreenSize    = new Vector2(newWidth * 0.2f, newHeight * 0.8f);
            this._bottomLeft.ScreenSize  = new Vector2(newWidth * 0.8f, newHeight * 0.2f);
            this._bottomRight.ScreenSize = new Vector2(newWidth * 0.2f, newHeight * 0.2f);
        }
    }
}

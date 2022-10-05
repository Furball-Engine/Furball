using System.Numerics;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Vixie.Backends.Shared;
using Furball.Vixie.Helpers;
using sowelipisona;

namespace Furball.Engine.Engine.Graphics.Drawables;

public class WaveformDrawable : Drawable {
    private readonly Waveform _waveform;
    private readonly float    _height;

    public override Vector2 Size => new Vector2(this._waveform.Points!.Length, this._height) * this.Scale;

    public WaveformDrawable(Waveform waveform, float height) {
        this._waveform = waveform;
        this._height   = height;

        Guard.EnsureNonNull(waveform.Points);
    }

    public override void Draw(double time, DrawableBatch batch, DrawableManagerArgs args) {
        base.Draw(time, batch, args);

        int currPoint = this._waveform.GetPointFromTime(this.TimeSource.GetCurrentTime());

        for (int i = 0; i < this._waveform.Points!.Length; i++) {
            Waveform.Point point = this._waveform.Points![i];

            float leftHeight  = this._height * point.AmplitudeLeft * args.Scale.Y;
            float rightHeight = this._height * point.AmplitudeRight * args.Scale.Y;

            float x      = i * args.Scale.X;
            float leftY  = this._height * args.Scale.Y - leftHeight;
            float rightY = this._height * args.Scale.Y - rightHeight;

            bool curr = currPoint == i;

            batch.Draw(FurballGame.WhitePixel, new Vector2(x, leftY) + args.Position, new Vector2(1 * args.Scale.X, leftHeight),  curr ? Color.Red : new Color(1f, 0f, 0f, 0.5f));
            batch.Draw(FurballGame.WhitePixel, new Vector2(x, rightY)+ args.Position, new Vector2(1 * args.Scale.X, rightHeight), curr ? Color.Blue : new Color(0f, 0f, 1f, 0.5f));
        }
        
        batch.Draw(FurballGame.WhitePixel, new Vector2(currPoint, 0) * args.Scale + args.Position, new Vector2(5, this._height) * args.Scale, Color.White);
    }
}

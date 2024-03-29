using System.Collections.Generic;
using System.Numerics;
using System.Text;
using FontStashSharp;
using Furball.Engine.Engine.Debug.DebugCounter.Items;
using Furball.Engine.Engine.Graphics;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.Managers;

namespace Furball.Engine.Engine.Debug.DebugCounter;

/// <summary>
///     Debug Counter which is displayed on the bottom left corner
/// </summary>
public class DebugCounter : TextDrawable {
    /// <summary>
    ///     Items which will be displayed on the Counter
    /// </summary>
    public readonly List<DebugCounterItem> Items = new List<DebugCounterItem> {
        new FrameRate(),
        new UpdateRate(),
        new InputUpdateRate(),
        new UpdateLoops(),
        new GraphicsBackend(),
        new VramUsage(),
        new TrackedVixieResources(),
        new DrawableManagerStats(),
        new ContentCacheItems(),
        new GameTimeSourceTime(),
        new MemoryUsage(),
        new MousePosition(),
        new KeyboardInputs(),
        new DrawCounts()
    };

    private readonly FixedTimeStepMethod _updateTimeStep;

    public DebugCounter() : base(Vector2.Zero, FurballGame.DefaultFont, "", 24) {
        this.Clickable   = false;
        this.CoverClicks = false;
        this.Hoverable   = false;
        this.CoverHovers = false;

        this.Effect         = FontSystemEffect.Stroked;
        this.EffectStrength = 2;
        
        FurballGame.TimeStepMethods.Add(
        this._updateTimeStep = new FixedTimeStepMethod(
        250,
        () => {
            StringBuilder builder = new();

            for (int i = 0; i != this.Items.Count; i++) {
                DebugCounterItem current = this.Items[i];

                if (current.ForceNewLine || i % 3 == 0 && i != 0)
                    builder.AppendLine();

                builder.Append($"{current.GetAsString(FurballGame.Time)}; ");
            }

            this.Text = builder.ToString();
        }
        )
        );
    }

    public override void Update(double time) {
        for (int i = 0; i < this.Items.Count; i++)
            this.Items[i].Update(time);

        base.Update(time);
    }

    public override void Draw(double time, DrawableBatch batch, DrawableManagerArgs args) {
        for (int i = 0; i != this.Items.Count; i++)
            this.Items[i].Draw(time);

        base.Draw(time, batch, args);
    }

    public override void Dispose() {
        base.Dispose();

        foreach (DebugCounterItem debugCounterItem in this.Items)
            debugCounterItem.Dispose();

        FurballGame.TimeStepMethods.Remove(this._updateTimeStep);
    }
}

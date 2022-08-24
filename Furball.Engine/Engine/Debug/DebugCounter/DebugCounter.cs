using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Furball.Engine.Engine.Debug.DebugCounter.Items;
using Furball.Engine.Engine.Graphics;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.Managers;

namespace Furball.Engine.Engine.Debug.DebugCounter;

/// <summary>
/// Debug Counter which is displayed on the bottom left corner
/// </summary>
public class DebugCounter : TextDrawable {
    /// <summary>
    /// Items which will be displayed on the Counter
    /// </summary>
    public List<DebugCounterItem> Items = new() {
        new FrameRate(),
        new UpdateRate(),
        new BoundByDrawUpdate(),
        new GraphicsBackend(),
        new TrackedVixieResources(),
        new DrawableManagerStats(),
        new ContentCacheItems(),
        new GameTimeSourceTime(),
        new MemoryUsage(),
        new MousePosition(),
        new KeyboardInputs()
    };

    public DebugCounter() : base(Vector2.Zero, FurballGame.DEFAULT_FONT, "", 24) {
        this.Clickable   = false;
        this.CoverClicks = false;
        this.Hoverable   = false;
        this.CoverHovers = false;
    }

    public override void Draw(double time, DrawableBatch batch, DrawableManagerArgs args) {
        for (int i = 0; i != this.Items.Count; i++) {
            this.Items[i].Draw(time);
        }

        base.Draw(time, batch, args);
    }

    public override void Update(double time) {
        StringBuilder builder = new();

        for (int i = 0; i != this.Items.Count; i++) {
            DebugCounterItem current = this.Items[i];

            current.Update(time);

            if (i % 3 == 0 && i != 0)
                builder.AppendLine();

            builder.Append($"{current.GetAsString(time)}; ");

            if (current.ForceNewLine)
                builder.AppendLine();
        }

        this.Text = builder.ToString();
    }

    public override void Dispose() {
        base.Dispose();

        foreach (DebugCounterItem debugCounterItem in this.Items)
            debugCounterItem.Dispose();
    }
}

using Furball.Engine.Engine.Graphics.Drawables.Primitives;
using Furball.Vixie.Backends.Shared;

namespace Furball.Game.Screens.Tests; 

public class CircleDrawableTest : TestScreen {
    public override void Initialize() {
        base.Initialize();
            
        this.Manager.Add(new CirclePrimitiveDrawable(new(100), 50, Color.Red, 2));
    }
}
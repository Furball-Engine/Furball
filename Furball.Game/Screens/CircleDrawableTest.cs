using Furball.Engine.Engine.Graphics.Drawables.Primitives;
using Furball.Vixie.Backends.Shared;
using Screen=Furball.Engine.Engine.Screen;

namespace Furball.Game.Screens; 

public class CircleDrawableTest : TestScreen {
    public override void Initialize() {
        base.Initialize();
            
        this.Manager.Add(new CirclePrimitiveDrawable(new(100), 50, Color.Red, 2));
    }
}
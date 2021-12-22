using Furball.Engine.Engine.Graphics.Drawables.Primitives;
using Color=Furball.Vixie.Graphics.Color;
using Screen=Furball.Engine.Engine.Screen;

namespace Furball.Game.Screens {
    public class CircleDrawableTest : Screen {
        public override void Initialize() {
            base.Initialize();
            
            this.Manager.Add(new CirclePrimitiveDrawable(new(100), 50, Color.Red, 2));
        }
    }
}

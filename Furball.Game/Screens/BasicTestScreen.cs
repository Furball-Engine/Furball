using Furball.Engine;
using Furball.Engine.Engine;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.Tweens.TweenTypes.BezierPathTween;
using Microsoft.Xna.Framework;

namespace Furball.Game.Screens {
    public class BasicTestScreen : Screen {
        public override void Initialize() {
            base.Initialize();
            
            TexturedDrawable background = new TexturedDrawable(FurballGame.WhitePixel, Vector2.Zero) {
                ColorOverride = Color.BlueViolet,
                Scale = new Vector2(1280, 720),
                Depth = 1f
            };
            
            this.Manager.Add(background);

            Vector2 p1 = new Vector2(160,                  -250 + 1280 / 2);
            Vector2 p2 = new Vector2(160 + ((1280/4) * 1), -250 + (1280/2) - (1280/4));
            Vector2 p3 = new Vector2(160 + ((1280/4) * 2), -250 + (1280/2) + (1280/4));
            Vector2 p4 = new Vector2(160 + ((1280/4) * 3), -250 + 1280 / 2);

            BezierCurveDrawable pathVisualization = new BezierCurveDrawable(p1, p2, p3, p4) {
                Thickness = 5f,
                Quality = 50
            };

            TexturedDrawable testDrawable = new TexturedDrawable(FurballGame.WhitePixel, p1) {
                Scale = new Vector2(64, 64),
                OriginType = OriginType.Center
            };

            Path path = new Path(new PathSegment(p1, p2, p3, p4));

            testDrawable.Tweens.Add(new PathTween(path, 2500, 10000));

            this.Manager.Add(pathVisualization);
            this.Manager.Add(testDrawable);
        }
    }
}

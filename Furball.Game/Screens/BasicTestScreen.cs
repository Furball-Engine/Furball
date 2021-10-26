using Furball.Engine;
using Furball.Engine.Engine;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.Tweens.TweenTypes.BezierPathTween;
using Furball.Engine.Engine.Graphics.Drawables.UiElements;
using Furball.Engine.Engine.Helpers;
using Furball.Engine.Engine.Localization;
using Furball.Engine.Engine.Localization.Languages;
using Kettu;
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

            Vector2 p1 = new Vector2(80, 640);
            Vector2 p2 = new Vector2(400, 640);
            Vector2 p3 = new Vector2(640, 160);
            Vector2 p4 = new Vector2(880, 640);
            Vector2 p5 = new Vector2(1200, 640);

            BezierCurveDrawable pathVisualization = new BezierCurveDrawable(p1, p2, p3);
            BezierCurveDrawable pathVisualization2 = new BezierCurveDrawable(p3, p4, p5);

            TexturedDrawable testDrawable = new TexturedDrawable(FurballGame.WhitePixel, p1) {
                Scale = new Vector2(64, 64),
                OriginType = OriginType.Center
            };

            Path path = new Path(new PathSegment(p1, p2, p3), new PathSegment(p3, p4, p5));

            testDrawable.Tweens.Add(new PathTween(path, 2500, 10000));

            this.Manager.Add(pathVisualization);
            this.Manager.Add(pathVisualization2);
            this.Manager.Add(testDrawable);
        }
    }
}

using System.Collections.Generic;
using Furball.Engine;
using Furball.Engine.Engine;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.Tweens.TweenTypes.BezierPathTween;
using Microsoft.Xna.Framework;

namespace Furball.Game.Screens {
    public class BasicTestScreen : Screen {
        private CurveDrawable pathVisualization;

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

            List<Vector2> points = new() {
                p1, p2, p3, p4
            };

            List<BaseDrawable> anchors = new List<BaseDrawable>();

            var a1 = new TexturedDrawable(FurballGame.WhitePixel, p1) {
                Scale      = new Vector2(16, 16),
                OriginType = OriginType.Center,
                Clickable = true,
            };

            a1.OnDrag += delegate(object? sender, Point point) {
                p1          = point.ToVector2();
                a1.Position = point.ToVector2();

                points = new() {
                    p1, p2, p3, p4
                };

                this.Manager.Remove(this.pathVisualization);
                this.pathVisualization = new CurveDrawable(points, CurveType.PeppyCatmullRom) {
                    Quality = 20, Thickness = 5f,
                };
                this.Manager.Add(this.pathVisualization);
            };

            anchors.Add(a1);


            var a2 = new TexturedDrawable(FurballGame.WhitePixel, p2) {
                Scale      = new Vector2(16, 16),
                OriginType = OriginType.Center,
                Clickable  = true,
            };

            a2.OnDrag += delegate(object? sender, Point point) {
                p2          = point.ToVector2();
                a2.Position = point.ToVector2();
                points = new() {
                    p1, p2, p3, p4
                };

                this.Manager.Remove(this.pathVisualization);
                this.pathVisualization = new CurveDrawable(points, CurveType.PeppyCatmullRom) {
                    Quality = 20, Thickness = 5f,
                };
                this.Manager.Add(this.pathVisualization);
            };

            anchors.Add(a2);


            var a3 = new TexturedDrawable(FurballGame.WhitePixel, p3) {
                Scale      = new Vector2(16, 16),
                OriginType = OriginType.Center,
                Clickable  = true,
            };

            a3.OnDrag += delegate(object? sender, Point point) {
                p3          = point.ToVector2();
                a3.Position = point.ToVector2();
                points = new() {
                    p1, p2, p3, p4
                };
                this.Manager.Remove(this.pathVisualization);
                this.pathVisualization = new CurveDrawable(points, CurveType.PeppyCatmullRom) {
                    Quality = 20, Thickness = 5f,
                };
                this.Manager.Add(this.pathVisualization);
            };

            anchors.Add(a3);



            var a4 = new TexturedDrawable(FurballGame.WhitePixel, p4) {
                Scale      = new Vector2(16, 16),
                OriginType = OriginType.Center,
                Clickable  = true,
            };

            a4.OnDrag += delegate(object? sender, Point point) {
                p4          = point.ToVector2();
                a4.Position = point.ToVector2();

                points = new() {
                    p1, p2, p3, p4
                };
                this.Manager.Remove(this.pathVisualization);
                this.pathVisualization = new CurveDrawable(points, CurveType.PeppyCatmullRom) {
                    Quality = 20, Thickness = 5f,
                };
                this.Manager.Add(this.pathVisualization);
            };

            anchors.Add(a4);

            this.pathVisualization = new CurveDrawable(points, CurveType.PeppyCatmullRom) {
                Quality = 20, Thickness = 5f,
            };

            TexturedDrawable testDrawable = new TexturedDrawable(FurballGame.WhitePixel, p1) {
                Scale = new Vector2(64, 64),
                OriginType = OriginType.Center
            };

            Path path = new Path(new PathSegment(p1, p2, p3, p4));

            testDrawable.Tweens.Add(new PathTween(path, 2500, 10000));

            this.Manager.Add(pathVisualization);
            //this.Manager.Add(testDrawable);
            this.Manager.Add(anchors);
        }
    }
}

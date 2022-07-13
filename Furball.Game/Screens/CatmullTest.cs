using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Furball.Engine;
using Furball.Engine.Engine;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.Tweens.TweenTypes.BezierPathTween;
using Furball.Engine.Engine.Helpers;
using Color=Furball.Vixie.Backends.Shared.Color;

namespace Furball.Game.Screens {
    public class CatmullTest : TestScreen {
        private CurveDrawable pathVisualization;

        public override void Initialize() {
            base.Initialize();

            Vector2 p1 = new Vector2(160,                  -250 + 1280 / 2);
            Vector2 p2 = new Vector2(160 + ((1280/4) * 1), -250 + (1280/2) - (1280/4));
            Vector2 p3 = new Vector2(160 + ((1280/4) * 2), -250 + (1280/2) + (1280/4));
            Vector2 p4 = new Vector2(160 + ((1280/4) * 3), -250 + 1280 / 2);

            List<Drawable> anchors = new();

            var a1 = new TexturedDrawable(FurballGame.WhitePixel, p1) {
                Scale      = new Vector2(16, 16),
                OriginType = OriginType.Center,
                Clickable = true,
            };

            a1.OnDrag += delegate(object? sender, Point point) {
                p1          = point.ToVector2();
                a1.Position = point.ToVector2();

                this.Manager.Remove(this.pathVisualization);
                this.pathVisualization = new CurveDrawable(p1, p2, p3, p4) {
                    Quality = 50, Thickness = 5f, Type = CurveType.CatmullRom
                };
                this.Manager.Add(this.pathVisualization);
            };

            a1.OnClick += delegate {
                a1.Scale = new Vector2(100, 100);
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
                this.Manager.Remove(this.pathVisualization);
                this.pathVisualization = new CurveDrawable(p1, p2, p3, p4) {
                    Quality = 50, Thickness = 5f, Type = CurveType.CatmullRom
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
                this.Manager.Remove(this.pathVisualization);
                this.pathVisualization = new CurveDrawable(p1, p2, p3, p4) {
                    Quality = 50, Thickness = 5f, Type = CurveType.CatmullRom
                };
                this.Manager.Add(this.pathVisualization);;
            };

            anchors.Add(a3);



            var a4 = new TexturedDrawable(FurballGame.WhitePixel, p4) {
                Scale      = new Vector2(128, 128),
                OriginType = OriginType.Center,
                Clickable  = true,
            };

            a4.OnDrag += delegate(object? sender, Point point) {
                p4          = point.ToVector2();
                a4.Position = point.ToVector2();
                this.Manager.Remove(this.pathVisualization);
                this.pathVisualization = new CurveDrawable(p1, p2, p3, p4) {
                    Quality = 50, Thickness = 5f, Type = CurveType.CatmullRom
                };
                this.Manager.Add(this.pathVisualization);
            };

            anchors.Add(a4);

            pathVisualization = new CurveDrawable(p1, p2, p3, p4) {
                Thickness = 5f,
                Quality = 50,
                Type = CurveType.CatmullRom
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

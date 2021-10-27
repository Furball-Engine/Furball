using System.Collections.Generic;
using System.Collections.ObjectModel;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Helpers;
using Microsoft.Xna.Framework;
using Xssp.MonoGame.Primitives2D;

namespace Furball.Engine.Engine.Graphics.Drawables {
    public class CurveDrawable : ManagedDrawable {
        public CurveType Type;

        public Bindable<Vector2> P0;
        public Bindable<Vector2> P1;
        public Bindable<Vector2> P2;
        public Bindable<Vector2> P3;

        public  ObservableCollection<Vector2> Points;
        public  ObservableCollection<(Vector2 begin, Vector2 end)> Path;
        private bool                          _requiresRedraw = false;

        public int Quality = 20;

        public float Thickness = 2f;

        public override Vector2 Size => new(100);

        public CurveDrawable(Vector2 p0, Vector2 p1, Vector2 p2) {
            this.P0 = new Bindable<Vector2>(p0);
            this.P1 = new Bindable<Vector2>(p1);
            this.P2 = new Bindable<Vector2>(p2);

            this.Type = CurveType.Quadratic;
        }

        public CurveDrawable(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3) {
            this.P0 = new Bindable<Vector2>(p0);
            this.P1 = new Bindable<Vector2>(p1);
            this.P2 = new Bindable<Vector2>(p2);
            this.P3 = new Bindable<Vector2>(p3);

            this.Type = CurveType.Cubic;
        }

        public CurveDrawable(List<Vector2> points, CurveType type) {
            this.Points          = new ObservableCollection<Vector2>(points);
            this.Path            = new ObservableCollection<(Vector2 begin, Vector2 end)>();

            this.Points.CollectionChanged += delegate { this._requiresRedraw = true; };
            this.Path.CollectionChanged   += delegate { this._requiresRedraw = true; };

            this._requiresRedraw = true;
            this.Type            = type;
        }

        public override void Draw(GameTime time, DrawableBatch batch, DrawableManagerArgs args) {
            if (this.Type != CurveType.PeppyCatmullRom) {
                for (int i = 0; i < this.Quality; i++) {
                    float t = (float)i / this.Quality;
                    float nextT = (float)(i + 1) / this.Quality;

                    switch (this.Type) {
                        case CurveType.Cubic: {
                            (float x, float y)   = BezierHelper.CubicBezier(this.P0, this.P1, this.P2, this.P3, t);
                            (float x1, float y1) = BezierHelper.CubicBezier(this.P0, this.P1, this.P2, this.P3, nextT);

                            x  *= FurballGame.VerticalRatio;
                            y  *= FurballGame.VerticalRatio;
                            x1 *= FurballGame.VerticalRatio;
                            y1 *= FurballGame.VerticalRatio;

                            batch.SpriteBatch.DrawLine(x, y, x1, y1, args.Color, this.Thickness * FurballGame.VerticalRatio, 0);
                            batch.SpriteBatch.DrawLine(x, y, x1, y1, args.Color, this.Thickness * FurballGame.VerticalRatio, 0);

                            break;
                        }
                        case CurveType.Quadratic: {
                            (float x, float y)   = BezierHelper.QuadraticBezier(this.P0, this.P1, this.P2, t);
                            (float x1, float y1) = BezierHelper.QuadraticBezier(this.P0, this.P1, this.P2, nextT);

                            x  *= FurballGame.VerticalRatio;
                            y  *= FurballGame.VerticalRatio;
                            x1 *= FurballGame.VerticalRatio;
                            y1 *= FurballGame.VerticalRatio;

                            batch.SpriteBatch.DrawLine(x, y, x1, y1, args.Color, this.Thickness * FurballGame.VerticalRatio, 0);
                            batch.SpriteBatch.DrawLine(x, y, x1, y1, args.Color, this.Thickness * FurballGame.VerticalRatio, 0);

                            break;
                        }
                        case CurveType.CatmullRom: {
                            (float x, float y)   = Vector2.CatmullRom(this.P0, this.P1, this.P2, this.P3, t);
                            (float x1, float y1) = Vector2.CatmullRom(this.P0, this.P1, this.P2, this.P3, nextT);

                            x  *= FurballGame.VerticalRatio;
                            y  *= FurballGame.VerticalRatio;
                            x1 *= FurballGame.VerticalRatio;
                            y1 *= FurballGame.VerticalRatio;

                            batch.SpriteBatch.DrawLine(x, y, x1, y1, args.Color, this.Thickness * FurballGame.VerticalRatio, 0);
                            batch.SpriteBatch.DrawLine(x, y, x1, y1, args.Color, this.Thickness * FurballGame.VerticalRatio, 0);

                            break;
                        }
                    }
                }
            } else {
                if (this._requiresRedraw) {
                    this.Path.Clear();

                    for (int j = 0; j < this.Points.Count - 1; j++) {
                        Vector2 v1;
                        if (j - 1 >= 0)
                            v1 = this.Points[j - 1];
                        else
                            v1 = this.Points[j];

                        Vector2 v2 = Points[j];

                        Vector2 v3;
                        if (j + 1 < this.Points.Count)
                            v3 = this.Points[j + 1];
                        else
                            v3 = v2 + (v2 - v1);

                        Vector2 v4;

                        if (j + 2 < this.Points.Count)
                            v4 = this.Points[j + 2];
                        else
                            v4 = v3 + (v3 - v2);

                        for (int k = 0; k < this.Quality; k++) {
                            Vector2 q1 = Vector2.CatmullRom(v1, v2, v3, v4, (float)k / (float)this.Quality);
                            Vector2 q2 = Vector2.CatmullRom(v1, v2, v3, v4, (float)(k + 1) / (float)this.Quality);

                            this.Path.Add((q1, q2));
                        }

                        this._requiresRedraw = false;
                    }
                }

                for (int j = 0; j != this.Path.Count; j++) {
                    (Vector2 begin, Vector2 end) = this.Path[j];

                    begin *= FurballGame.VerticalRatio;
                    end   *= FurballGame.VerticalRatio;

                    batch.SpriteBatch.DrawLine(begin.X, begin.Y, end.X, end.Y, args.Color, this.Thickness * FurballGame.VerticalRatio, 0);
                    batch.SpriteBatch.DrawLine(begin.X, begin.Y, end.X, end.Y, args.Color, this.Thickness * FurballGame.VerticalRatio, 0);
                }

            }
        }
    }

    public enum CurveType {
        Quadratic,
        Cubic,
        CatmullRom,
        PeppyCatmullRom,

    }
}

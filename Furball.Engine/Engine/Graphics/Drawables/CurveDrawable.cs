using System.Numerics;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Helpers;

namespace Furball.Engine.Engine.Graphics.Drawables {
    public class CurveDrawable : ManagedDrawable {
        /// <summary>
        ///     The type of curve
        /// </summary>
        public CurveType Type;

        /// <summary>
        ///     p0, the first point, or starting position TODO: could this be merged with this.Position?
        /// </summary>
        public Bindable<Vector2> P0;
        /// <summary>
        ///     p1, the second point
        /// </summary>
        public Bindable<Vector2> P1;
        /// <summary>
        ///     p2, the third point (last point on Quadratic bezier curve)
        /// </summary>
        public Bindable<Vector2> P2;
        /// <summary>
        ///     p3, the last point on Cubic and Catmull-Rom curves
        /// </summary>
        public Bindable<Vector2> P3;

        /// <summary>
        ///     How many line segments the drawable uses to approximate the curve
        /// </summary>
        public int Quality = 20;

        /// <summary>
        ///     The thickness of the line
        /// </summary>
        public float Thickness = 2f;

        public override Vector2 Size => new(100);

        /// <summary>
        ///     The constructor for a Quadratic Bezier curve
        /// </summary>
        public CurveDrawable(Vector2 p0, Vector2 p1, Vector2 p2) {
            this.P0 = new Bindable<Vector2>(p0);
            this.P1 = new Bindable<Vector2>(p1);
            this.P2 = new Bindable<Vector2>(p2);

            this.Type = CurveType.Quadratic;
        }

        /// <summary>
        ///     The constructor for a Cubic Bezier curve
        /// </summary>
        public CurveDrawable(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3) {
            this.P0 = new Bindable<Vector2>(p0);
            this.P1 = new Bindable<Vector2>(p1);
            this.P2 = new Bindable<Vector2>(p2);
            this.P3 = new Bindable<Vector2>(p3);

            this.Type = CurveType.Cubic;
        }

        public override void Draw(double time, DrawableBatch batch, DrawableManagerArgs args) {
            for (int i = 0; i < this.Quality; i++) {
                float t     = (float)i       / this.Quality;
                float nextT = (float)(i + 1) / this.Quality;

                switch (this.Type) {
                    case CurveType.Cubic: {
                        Vector2 p0 = BezierHelper.CubicBezier(this.P0, this.P1, this.P2, this.P3, t);
                        Vector2 p1 = BezierHelper.CubicBezier(this.P0,   this.P1, this.P2, this.P3, nextT);

                        p0.X *= FurballGame.VerticalRatio;
                        p0.Y *= FurballGame.VerticalRatio;
                        p1.X *= FurballGame.VerticalRatio;
                        p1.Y *= FurballGame.VerticalRatio;
                        //TODO(Eevee): DrawableBatch renderer switching and stuff
                        //batch.Renderer.DrawLine(p0.X, p0.Y, p1.X, p1.Y, args.Color, this.Thickness * FurballGame.VerticalRatio, 0);
                        //batch.Renderer.DrawLine(p0.X, p0.Y, p1.X, p1.Y, args.Color, this.Thickness * FurballGame.VerticalRatio, 0);

                        break;
                    }
                    case CurveType.Quadratic: {
                        Vector2 p0 = BezierHelper.QuadraticBezier(this.P0, this.P1, this.P2, t);
                        Vector2 p1 = BezierHelper.QuadraticBezier(this.P0, this.P1, this.P2, nextT);

                        p0.X *= FurballGame.VerticalRatio;
                        p0.Y *= FurballGame.VerticalRatio;
                        p1.X *= FurballGame.VerticalRatio;
                        p1.Y *= FurballGame.VerticalRatio;
                        //TODO(Eevee): DrawableBatch renderer switching and stuff
                        //batch.Renderer.DrawLine(p0.X, p0.Y, p1.X, p1.Y, args.Color, this.Thickness * FurballGame.VerticalRatio, 0);
                        //batch.Renderer.DrawLine(p0.X, p0.Y, p1.X, p1.Y, args.Color, this.Thickness * FurballGame.VerticalRatio, 0);
                        
                        break;
                    }
                    case CurveType.CatmullRom: {
                        //(float x, float y)   = Vector2.CatmullRom(this.P0, this.P1, this.P2, this.P3, t);
                        //(float x1, float y1) = Vector2.CatmullRom(this.P0, this.P1, this.P2, this.P3, nextT);
//
                        //x  *= FurballGame.VerticalRatio;
                        //y  *= FurballGame.VerticalRatio;
                        //x1 *= FurballGame.VerticalRatio;
                        //y1 *= FurballGame.VerticalRatio;
//                      //TODO(Eevee): DrawableBatch renderer switching and stuff
                        //batch.SpriteBatch.DrawLine(x, y, x1, y1, args.Color, this.Thickness * FurballGame.VerticalRatio, 0);
                        //batch.SpriteBatch.DrawLine(x, y, x1, y1, args.Color, this.Thickness * FurballGame.VerticalRatio, 0);

                        break;
                    }
                }
            }
        }
    }

    public enum CurveType {
        Quadratic,
        Cubic,
        CatmullRom
    }
}

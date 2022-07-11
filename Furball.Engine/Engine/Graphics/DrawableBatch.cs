using System;
using System.Drawing;
using System.Numerics;
using FontStashSharp;
using Furball.Engine.Engine.Helpers;
using Furball.Vixie;
using Furball.Vixie.Backends.Shared;
using Furball.Vixie.Backends.Shared.Renderers;
using Color=Furball.Vixie.Backends.Shared.Color;

namespace Furball.Engine.Engine.Graphics {
    /// <summary>
    /// A Basic Abstraction for Vixie's different types of renderers
    /// </summary>
    public class DrawableBatch : IDisposable {
        private readonly IQuadRenderer _textureRenderer;
        private readonly ILineRenderer    _lineRenderer;

        private bool _begun;
        public bool Begun => _begun;

        public DrawableBatch() {
            Profiler.StartProfile("create_drawable_batch");
            this._textureRenderer = GraphicsBackend.Current.CreateTextureRenderer();
            this._lineRenderer    = GraphicsBackend.Current.CreateLineRenderer();
            Profiler.EndProfileAndPrint("create_drawable_batch");
        }

        public void Begin() {
            this._textureRenderer.Begin();
            this._begun = true;
        }

        public void End() {
            if (this._textureRenderer.IsBegun)
                this._textureRenderer.End();
            if (this._lineRenderer.IsBegun)
                this._lineRenderer.End();

            this._begun = false;
        }

        public void Draw(Texture texture, Vector2 position, Vector2 scale, float rotation, Color colorOverride, TextureFlip texFlip = TextureFlip.None, Vector2 rotOrigin = default) {
            if (this._lineRenderer.IsBegun)
                this._lineRenderer.End();
            if (!this._textureRenderer.IsBegun)
                this._textureRenderer.Begin();

            this._textureRenderer.Draw(texture, position, scale, rotation, colorOverride, texFlip, rotOrigin);
        }

        public void Draw(Texture texture, Vector2 position, Vector2 scale, float rotation, Color colorOverride, Rectangle sourceRect, TextureFlip texFlip = TextureFlip.None, Vector2 rotOrigin = default) {
            if (this._lineRenderer.IsBegun)
                this._lineRenderer.End();
            if (!this._textureRenderer.IsBegun)
                this._textureRenderer.Begin();

            this._textureRenderer.Draw(texture, position, scale, rotation, colorOverride, sourceRect, texFlip, rotOrigin);
        }
        public void Draw(Texture texture, Vector2 position, float rotation = 0, TextureFlip flip = TextureFlip.None, Vector2 rotOrigin = default) {
            if (this._lineRenderer.IsBegun)
                this._lineRenderer.End();
            if (!this._textureRenderer.IsBegun)
                this._textureRenderer.Begin();

            this._textureRenderer.Draw(texture, position, rotation, flip, rotOrigin);
        }
        public void Draw(Texture texture, Vector2 position, Vector2 scale, float rotation = 0, TextureFlip flip = TextureFlip.None, Vector2 rotOrigin = default) {
            if (this._lineRenderer.IsBegun)
                this._lineRenderer.End();
            if (!this._textureRenderer.IsBegun)
                this._textureRenderer.Begin();

            this._textureRenderer.Draw(texture, position, scale, rotation, Color.White, flip, rotOrigin);
        }

        public void Draw(Texture texture, Vector2 position, Vector2 scale, Color colorOverride, float rotation = 0, TextureFlip texFlip = TextureFlip.None, Vector2 rotOrigin = default) {
            if (this._lineRenderer.IsBegun)
                this._lineRenderer.End();
            if (!this._textureRenderer.IsBegun)
                this._textureRenderer.Begin();

            this._textureRenderer.Draw(texture, position, scale, rotation, colorOverride, texFlip, rotOrigin);
        }


        public void DrawLine(Vector2 begin, Vector2 end, float thickness, Color color) {
            if (this._textureRenderer.IsBegun)
                this._textureRenderer.End();

            if (!this._lineRenderer.IsBegun)
                this._lineRenderer.Begin();

            this._lineRenderer.Draw(begin, end, thickness, color);
        }

        public void DrawLine(float x0, float y0, float x1, float y1, float thickness, Color color) {
            this.DrawLine(new Vector2(x0, y0), new Vector2(x1, y1), thickness, color);
        }

        //TODO: add rotations
        public void DrawRectangle(Vector2 position, Vector2 size, float thickness, Color color) {
            Vector2 p0 = position; //TL
            Vector2 p1 = new Vector2(position.X + size.X, position.Y); //TR
            Vector2 p2 = new Vector2(position.X + size.X, position.Y + size.Y); //BR
            Vector2 p3 = new Vector2(position.X,          position.Y + size.Y); //BL

            this.DrawLine(p0, p1, thickness, color);
            this.DrawLine(p1, p2, thickness, color);
            this.DrawLine(p2, p3, thickness, color);
            this.DrawLine(p3, p0, thickness, color);

            //drawn with lines to not break batch
            this.DrawLine(new Vector2(p0.X - thickness / 2, p0.Y), new Vector2(p0.X + thickness / 2, p0.Y), thickness, color);
            this.DrawLine(new Vector2(p1.X - thickness / 2, p1.Y), new Vector2(p1.X + thickness / 2, p1.Y), thickness, color);
            this.DrawLine(new Vector2(p2.X - thickness / 2, p2.Y), new Vector2(p2.X + thickness / 2, p2.Y), thickness, color);
            this.DrawLine(new Vector2(p3.X - thickness / 2, p3.Y), new Vector2(p3.X + thickness / 2, p3.Y), thickness, color);
        }

        public void FillRectangle(Vector2 position, Vector2 size, Color color, float rotation = 0f) {
            this.Draw(FurballGame.WhitePixel, position, size, rotation, color);
        }

        public void DrawString(DynamicSpriteFont font, string text, Vector2 position, Color color, float rotation = 0f, Vector2? scale = null, Vector2 origin = default) {
            if (this._lineRenderer.IsBegun)
                this._lineRenderer.End();
            if(!this._textureRenderer.IsBegun)
                this._textureRenderer.Begin();

            this._textureRenderer.DrawString(font, text, position, color, rotation, scale, origin);
        }

        public void DrawString(DynamicSpriteFont font, string text, Vector2 position, System.Drawing.Color color, float rotation = 0f, Vector2? scale = null, Vector2 origin = default) {
            if (this._lineRenderer.IsBegun)
                this._lineRenderer.End();
            if(!this._textureRenderer.IsBegun)
                this._textureRenderer.Begin();

            this._textureRenderer.DrawString(font, text, position, color, rotation, scale, origin);
        }

        public void DrawString(DynamicSpriteFont font, string text, Vector2 position, System.Drawing.Color[] colors, float rotation = 0f, Vector2? scale = null, Vector2 origin = default ) {
            if (this._lineRenderer.IsBegun)
                this._lineRenderer.End();
            if(!this._textureRenderer.IsBegun)
                this._textureRenderer.Begin();

            this._textureRenderer.DrawString(font, text, position, colors, rotation, scale, origin);
        }


        public void Dispose() {
            this._lineRenderer.Dispose();
            this._textureRenderer.Dispose();
        }
    }
}

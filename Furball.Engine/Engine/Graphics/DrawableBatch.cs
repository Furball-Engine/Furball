using System;
using System.Drawing;
using System.Numerics;
using FontStashSharp;
using Furball.Vixie.Graphics;
using Furball.Vixie.Graphics.Renderers;
using Furball.Vixie.Graphics.Renderers.OpenGL;
using Color=Furball.Vixie.Graphics.Color;

namespace Furball.Engine.Engine.Graphics {
    /// <summary>
    /// A Basic Abstraction for Vixie's different types of renderers
    /// </summary>
    public class DrawableBatch : IDisposable {
        private readonly ITextureRenderer _textureRenderer;
        private readonly ILineRenderer    _lineRenderer;
        private readonly ITextRenderer    _textRenderer;

        private bool _begun;
        public bool Begun => _begun;

        public DrawableBatch(ITextureRenderer textureRenderer, ILineRenderer lineRenderer, ITextRenderer renderer) {
            this._textureRenderer = textureRenderer;
            this._lineRenderer    = lineRenderer;
        }

        public DrawableBatch() {
            this._textureRenderer = new QuadRenderer();
            this._lineRenderer    = new LineRenderer();
            this._textRenderer    = (ITextRenderer) this._textureRenderer;
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
            if (this._textRenderer.IsBegun)
                this._textRenderer.End();

            this._begun = false;
        }

        public void Draw(Texture texture, Vector2 position, Vector2 scale, float rotation, Color colorOverride, TextureFlip texFlip = TextureFlip.None, Vector2 rotOrigin = default) {
            if (this._lineRenderer.IsBegun)
                this._lineRenderer.End();
            if (this._textureRenderer != this._textRenderer && this._textRenderer.IsBegun)
                this._textRenderer.End();

            if (!this._textureRenderer.IsBegun)
                this._textureRenderer.Begin();

            this._textureRenderer.Draw(texture, position, scale, rotation, colorOverride, texFlip, rotOrigin);
        }

        public void Draw(Texture texture, Vector2 position, Vector2 scale, float rotation, Color colorOverride, Rectangle sourceRect, TextureFlip texFlip = TextureFlip.None, Vector2 rotOrigin = default) {
            if (this._lineRenderer.IsBegun)
                this._lineRenderer.End();
            if (this._textureRenderer != this._textRenderer && this._textRenderer.IsBegun)
                this._textRenderer.End();

            if (!this._textureRenderer.IsBegun)
                this._textureRenderer.Begin();

            this._textureRenderer.Draw(texture, position, scale, rotation, colorOverride, sourceRect, texFlip, rotOrigin);
        }
        public void Draw(Texture texture, Vector2 position, float rotation = 0, TextureFlip flip = TextureFlip.None, Vector2 rotOrigin = default) {
            if (this._lineRenderer.IsBegun)
                this._lineRenderer.End();
            if (this._textureRenderer != this._textRenderer && this._textRenderer.IsBegun)
                this._textRenderer.End();

            if (!this._textureRenderer.IsBegun)
                this._textureRenderer.Begin();
        }
        public void Draw(Texture texture, Vector2 position, Vector2 scale, float rotation = 0, TextureFlip flip = TextureFlip.None, Vector2 rotOrigin = default) {
            if (this._lineRenderer.IsBegun)
                this._lineRenderer.End();
            if (this._textureRenderer != this._textRenderer && this._textRenderer.IsBegun)
                this._textRenderer.End();

            if (!this._textureRenderer.IsBegun)
                this._textureRenderer.Begin();

            this._textureRenderer.Draw(texture, position, texture.Size, Vector2.One, rotation, Color.White, flip, rotOrigin);
        }

        public void Draw(Texture texture, Vector2 position, Vector2 scale, Color colorOverride, float rotation = 0, TextureFlip texFlip = TextureFlip.None, Vector2 rotOrigin = default) {
            if (this._lineRenderer.IsBegun)
                this._lineRenderer.End();
            if (this._textureRenderer != this._textRenderer && this._textRenderer.IsBegun)
                this._textRenderer.End();

            if (!this._textureRenderer.IsBegun)
                this._textureRenderer.Begin();

            this._textureRenderer.Draw(texture, position, texture.Size, scale, rotation, Color.White, texFlip, rotOrigin);
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
            this.Draw(FurballGame.WhitePixel, position, size, Vector2.One, rotation, color);
        }

        public void DrawString(DynamicSpriteFont font, string text, Vector2 position, Color color, float rotation = 0f, Vector2? scale = null) {
            if (this._lineRenderer.IsBegun)
                this._lineRenderer.End();
            if (this._textRenderer == this._textureRenderer) {
                if (!this._textRenderer.IsBegun)
                    this._textRenderer.Begin();
            } else this._textureRenderer.End();

            this._textRenderer.DrawString(font, text, position, color, rotation, scale);
        }

        public void DrawString(DynamicSpriteFont font, string text, Vector2 position, System.Drawing.Color color, float rotation = 0f, Vector2? scale = null) {
            if (this._lineRenderer.IsBegun)
                this._lineRenderer.End();
            if (this._textRenderer == this._textureRenderer) {
                if (!this._textRenderer.IsBegun)
                    this._textRenderer.Begin();
            } else this._textureRenderer.End();

            this._textRenderer.DrawString(font, text, position, color, rotation, scale);
        }

        public void DrawString(DynamicSpriteFont font, string text, Vector2 position, System.Drawing.Color[] colors, float rotation = 0f, Vector2? scale = null) {
            if (this._lineRenderer.IsBegun)
                this._lineRenderer.End();
            if (this._textRenderer == this._textureRenderer) {
                if (!this._textRenderer.IsBegun)
                    this._textRenderer.Begin();
            } else this._textureRenderer.End();

            this._textRenderer.DrawString(font, text, position, colors, rotation, scale);
        }


        public void Dispose() {
            this._lineRenderer.Dispose();
            this._textureRenderer.Dispose();

            if(this._textRenderer != this._textureRenderer)
                this._textRenderer.Dispose();
        }
    }
}

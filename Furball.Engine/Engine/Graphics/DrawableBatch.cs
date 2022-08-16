using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using FontStashSharp;
using Furball.Engine.Engine.Helpers;
using Furball.Vixie;
using Furball.Vixie.Backends.Shared;
using Color=Furball.Vixie.Backends.Shared.Color;

namespace Furball.Engine.Engine.Graphics; 

/// <summary>
/// A Basic Abstraction for Vixie's different types of renderers
/// </summary>
public class DrawableBatch : IDisposable {
    private readonly Renderer _renderer;

    public bool Begun {
        get;
        private set;
    }

    public DrawableBatch() {
        Profiler.StartProfile("create_drawable_batch");
        this._renderer = new Renderer();
        Profiler.EndProfileAndPrint("create_drawable_batch");
    }

    public void Begin() {
        if (this.Begun)
            throw new Exception("DrawableBatch already begun!");
        
        this._renderer.Begin();
        this.Begun = true;
    }

    public void End() {
        this._renderer.End();
        this.Begun = false;
    }

    public void Draw(Texture texture, Vector2 position, Vector2 scale, float rotation, Color colorOverride, TextureFlip texFlip = TextureFlip.None, Vector2 rotOrigin = default) {
        this._renderer.Draw(texture, position, scale, rotation, colorOverride, texFlip, rotOrigin);
    }

    public void Draw(Texture texture, Vector2 position, Vector2 scale, float rotation, Color colorOverride, Rectangle sourceRect, TextureFlip texFlip = TextureFlip.None, Vector2 rotOrigin = default) {
        this._renderer.Draw(texture, position, scale, rotation, colorOverride, sourceRect, texFlip, rotOrigin);
    }
    public void Draw(Texture texture, Vector2 position, float rotation = 0, TextureFlip flip = TextureFlip.None, Vector2 rotOrigin = default) {
        this._renderer.Draw(texture, position, rotation, flip, rotOrigin);
    }
    public void Draw(Texture texture, Vector2 position, Vector2 scale, float rotation = 0, TextureFlip flip = TextureFlip.None, Vector2 rotOrigin = default) {
        this._renderer.Draw(texture, position, scale, rotation, Color.White, flip, rotOrigin);
    }

    public void Draw(Texture texture, Vector2 position, Vector2 scale, Color colorOverride, float rotation = 0, TextureFlip texFlip = TextureFlip.None, Vector2 rotOrigin = default) {
        this._renderer.Draw(texture, position, scale, rotation, colorOverride, texFlip, rotOrigin);
    }


    public void DrawLine(Vector2 begin, Vector2 end, float thickness, Color color) {
        this._renderer.Draw(FurballGame.WhitePixel, begin, new Vector2((end - begin).Length(), thickness), (float)Math.Atan2(end.Y - begin.Y, end.X - begin.X), color, TextureFlip.None, new Vector2(0, thickness / 2f));
    }

    public void DrawLine(float x0, float y0, float x1, float y1, float thickness, Color color) {
        this.DrawLine(new Vector2(x0, y0), new Vector2(x1, y1), thickness, color);
    }

    //TODO: add rotations
    public void DrawRectangle(Vector2 position, Vector2 size, float thickness, Color color) {
        Vector2 p0 = position;                                             //TL
        Vector2 p1 = new(position.X + size.X, position.Y);         //TR
        Vector2 p2 = new(position.X + size.X, position.Y + size.Y);//BR
        Vector2 p3 = new(position.X, position.Y + size.Y);//BL

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
        this._renderer.DrawString(font, text, position, color, rotation, scale, origin);
    }

    public void DrawString(DynamicSpriteFont font, string text, Vector2 position, System.Drawing.Color color, float rotation = 0f, Vector2? scale = null, Vector2 origin = default) {
        this._renderer.DrawString(font, text, position, color, rotation, scale, origin);
    }

    public void DrawString(DynamicSpriteFont font, string text, Vector2 position, System.Drawing.Color[] colors, float rotation = 0f, Vector2? scale = null, Vector2 origin = default ) {
        this._renderer.DrawString(font, text, position, colors, rotation, scale, origin);
    }

    private readonly Stack<(object badge, Rectangle rect)> _scissorStack = new();

    public int ScissorStackItemCount => this._scissorStack.Count;

    private void UpdateScissorFromStack() {
        if (this._scissorStack.Count == 0) {
            this.ScissorRect = new Rectangle(0, 0, (int)FurballGame.WindowWidth, (int)FurballGame.WindowHeight);
            return;
        }

        this.ScissorRect = this._scissorStack.Peek().rect;
    }
    
    public void ScissorPush(object badge, Rectangle rect) {
        this._scissorStack.Push((badge, rect));
        
        this.UpdateScissorFromStack();
    }

    public void ScissorPop(object badge) {
        System.Diagnostics.Debug.Assert(this._scissorStack.Count != 0, "this._scissorStack.Count != 0");

        (object o, Rectangle rect) = this._scissorStack.Pop();
        
        System.Diagnostics.Debug.Assert(o == badge, $"{nameof(badge)} != popped item!");
        
        this.UpdateScissorFromStack();
    }
    
    private Rectangle ScissorRect {
        get => new(
        (int)(GraphicsBackend.Current.ScissorRect.X / FurballGame.VerticalRatio),
        (int)(GraphicsBackend.Current.ScissorRect.Y / FurballGame.VerticalRatio),
        (int)(GraphicsBackend.Current.ScissorRect.Width / FurballGame.VerticalRatio),
        (int)(GraphicsBackend.Current.ScissorRect.Height / FurballGame.VerticalRatio)
        );
        set {
            if (value == this.ScissorRect)
                return;
            
            this.End();
            this.Begin();
            GraphicsBackend.Current.ScissorRect = new SixLabors.ImageSharp.Rectangle(
            (int)(value.X * FurballGame.VerticalRatio),
            (int)(value.Y * FurballGame.VerticalRatio),
            (int)(value.Width * FurballGame.VerticalRatio),
            (int)(value.Height * FurballGame.VerticalRatio)
            );
        }
    }

    public void Dispose() {
        this._renderer.Dispose();
    }
}
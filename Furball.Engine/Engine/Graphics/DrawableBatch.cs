using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using FontStashSharp;
using FontStashSharp.RichText;
using Furball.Engine.Engine.Helpers;
using Furball.Vixie;
using Furball.Vixie.Backends.Shared;
using Furball.Vixie.Backends.Shared.Renderers;
using Furball.Vixie.Helpers;
using Color = Furball.Vixie.Backends.Shared.Color;

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

    public void SoftEnd() {
        this._renderer.End();
        this.Begun = false;
    }

    public void ManualDraw() {
        this._renderer.Draw();
    }
    
    public void End() {
        this._renderer.End();
        this._renderer.Draw();
        this.Begun = false;
    }

    public MappedData Reserve(ushort vtxCount, uint idxCount) {
        return this._renderer.Reserve(vtxCount, idxCount);
    }
    public long GetTextureIdForReserve(Texture whitePixel) {
        return this._renderer.GetTextureId(whitePixel);
    }
    
    public void Draw(Texture texture, Vector2 position, Vector2 scale, float rotation, Color colorOverride, TextureFlip texFlip = TextureFlip.None, Vector2 rotOrigin = default) {
        this._renderer.AllocateRotatedTexturedQuad(texture, position, scale, rotation, rotOrigin, colorOverride, texFlip);
    }

    public void Draw(Texture texture, Vector2 position, Vector2 scale, float rotation, Color colorOverride, Rectangle sourceRect, TextureFlip texFlip = TextureFlip.None, Vector2 rotOrigin = default) {
        this._renderer.AllocateRotatedTexturedQuadWithSourceRect(texture, position, scale, rotation, rotOrigin, sourceRect, colorOverride, texFlip);
    }
    public void Draw(Texture texture, Vector2 position, float rotation = 0, TextureFlip flip = TextureFlip.None, Vector2 rotOrigin = default) {
        this._renderer.AllocateRotatedTexturedQuad(texture, position, Vector2.One, rotation, rotOrigin, Color.White, flip);
    }
    public void Draw(Texture texture, Vector2 position, Vector2 scale, float rotation = 0, TextureFlip flip = TextureFlip.None, Vector2 rotOrigin = default) {
        this._renderer.AllocateRotatedTexturedQuad(texture, position, scale, rotation, rotOrigin, Color.White, flip);
    }

    public void Draw(Texture texture, Vector2 position, Vector2 scale, Color colorOverride, float rotation = 0, TextureFlip texFlip = TextureFlip.None, Vector2 rotOrigin = default) {
        this._renderer.AllocateRotatedTexturedQuad(texture, position, scale, rotation, rotOrigin, colorOverride, texFlip);
    }

    public void DrawLine(Vector2 begin, Vector2 end, float thickness, Color color) {
        this._renderer.AllocateRotatedTexturedQuad(FurballGame.WhitePixel, begin, new Vector2((end - begin).Length(), thickness), (float)Math.Atan2(end.Y - begin.Y, end.X - begin.X), new Vector2(0, thickness / 2f), color);
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

    public void DrawString(
        DynamicSpriteFont font, string text, Vector2 position, Color color, float rotation = 0f, Vector2? scale = null, Vector2 origin = default(Vector2),
        TextStyle         style = TextStyle.None, FontSystemEffect effect = FontSystemEffect.None, int effectAmount = 0
    ) {
        scale ??= Vector2.One;
        this._renderer.DrawString(font, text, position, color, rotation, scale.Value, origin, style, effect, effectAmount);
    }

    public void DrawString(
        DynamicSpriteFont font, string text, Vector2 position, FSColor[] colors, float rotation = 0f, Vector2? scale = null, Vector2 origin = default(Vector2),
        TextStyle         style = TextStyle.None, FontSystemEffect effect = FontSystemEffect.None, int effectAmount = 0
    ) {
        this._renderer.DrawString(font, text, position, colors, rotation, scale, origin, style, effect, effectAmount);
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
        Guard.Assert(this._scissorStack.Count != 0, "this._scissorStack.Count != 0");

        (object o, Rectangle _) = this._scissorStack.Pop();
        
        Guard.Assert(o == badge, $"{nameof(badge)} != popped item!");
        
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
    public void DrawRichString(RichTextLayout layout, Vector2 position, Color color, Vector2? sourceScale, float rotation, Vector2 origin) {
        layout.Draw(this._renderer.FontRenderer, position, color, sourceScale, rotation, origin);
    }
}
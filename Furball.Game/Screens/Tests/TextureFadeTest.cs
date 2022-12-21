using System.Numerics;
using Furball.Engine;
using Furball.Engine.Engine.Graphics;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.UiElements;
using Furball.Engine.Engine.Input.Events;
using Furball.Vixie;
using Furball.Vixie.Backends.Shared;
using Furball.Vixie.Backends.Shared.TextureEffects.Blur;

namespace Furball.Game.Screens.Tests;

public class TextureFadeTest : TestScreen {
    private TexturedDrawable     _textureDrawable;
    private Texture              _source;
    private BoxBlurTextureEffect _blur;
    private Texture         _dest;

    public override void Initialize() {
        base.Initialize();
        
        this._source = ContentManager.LoadTextureFromFileCached("inviswater.png");
        this._blur = FurballGame.Instance.WindowManager.GraphicsBackend.CreateBoxBlurTextureEffect(this._source);
        this._dest   = new Texture(this._blur.Texture);
        this._blur.UpdateTexture();

        this.Manager.Add(this._textureDrawable = new TexturedDrawable(this._source, new Vector2(10)));

        this.Manager.Add(
        new DrawableButton(new Vector2(10), FurballGame.DefaultFont, 24, "Fade texture", Color.Blue, Color.White, Color.Black, Vector2.Zero, FadeTexture) {
            ScreenOriginType = OriginType.TopRight, 
            OriginType = OriginType.TopRight
        }
        );
    }
    
    private void FadeTexture(object sender, MouseButtonEventArgs e) {
        this._textureDrawable.FadeTexture(this._textureDrawable.Texture == this._source ? this._dest : this._source, 1000);
    }
}

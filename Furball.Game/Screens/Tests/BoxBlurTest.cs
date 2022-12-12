using System.Numerics;
using Furball.Engine;
using Furball.Engine.Engine.Graphics;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Vixie;
using Furball.Vixie.Backends.Shared.TextureEffects.Blur;

namespace Furball.Game.Screens.Tests; 

public class BoxBlurTest : TestScreen {
    private Texture              _source;
    private BoxBlurTextureEffect _blur;
    public override void Initialize() {
        base.Initialize();

        this._source            = ContentManager.LoadTextureFromFileCached("inviswater.png");
        this._blur              = FurballGame.Instance.WindowManager.GraphicsBackend.CreateBoxBlurTextureEffect(this._source);
        this._blur.KernelRadius = 4;
        this._blur.Passes       = 5;
        this._blur.UpdateTexture();

        this.Manager.Add(new TexturedDrawable(new Texture(this._blur.Texture), new Vector2(100, 100)) {
            Scale = new Vector2(4)
        });
        this.Manager.Add(new TexturedDrawable(this._source, new Vector2(100 + (this._source.Width * 4), 100)) {
            Scale = new Vector2(4)
        });
    }
}

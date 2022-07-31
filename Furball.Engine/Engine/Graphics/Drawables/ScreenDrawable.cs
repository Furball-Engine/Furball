using System.Numerics;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Localization;
using Furball.Engine.Engine.Localization.Languages;

namespace Furball.Engine.Engine.Graphics.Drawables;

public class ScreenDrawable : Drawable {
    private Screen  _screen;
    public  Vector2 ScreenSize;

    public override Vector2 Size => this.ScreenSize * this.Scale;

    public ScreenDrawable(Screen screen, Vector2 position, Vector2 size) {
        this._screen = screen;
        
        screen.Manager.EffectedByScaling = true;

        this.Clickable   = false;
        this.CoverClicks = false;
        this.Hoverable   = false;
        this.CoverHovers = false;

        screen.Initialize();

        this.Position   = position;
        this.ScreenSize = size;

        FurballGame.Instance.OnRelayout     += this.OnRelayout;
        LocalizationManager.LanguageChanged += this.LanguageChanged;
    }
    
    private void LanguageChanged(object sender, Language e) {
        this._screen.UpdateTextStrings();
    }

    private void OnRelayout(object sender, Vector2 e) {
        this._screen.Relayout(e.X, e.Y);
    }

    public override void Update(double time) {
        base.Update(time);
        
        this._screen.Manager.Position = this.RealPosition;
        this._screen.Manager.Size     = this.ScreenSize * this.RealScale;
        
        this._screen.Update(time);
    }

    public override void Draw(double time, DrawableBatch batch, DrawableManagerArgs args) {
        base.Draw(time, batch, args);
        
        this._screen.Draw(time);
    }

    public override void Dispose() {
        base.Dispose();

        FurballGame.Instance.OnRelayout     -= this.OnRelayout;
        LocalizationManager.LanguageChanged -= this.LanguageChanged;

        this._screen.Unload();
        this._screen.Dispose();
    }
}

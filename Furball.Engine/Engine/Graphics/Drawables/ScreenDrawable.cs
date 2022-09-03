using System.Numerics;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Localization;
using Furball.Engine.Engine.Localization.Languages;

namespace Furball.Engine.Engine.Graphics.Drawables;

public class ScreenDrawable : Drawable {
    private Screen  _screen;
    private Vector2 _screenSize;

    public Vector2 ScreenSize {
        get => this._screenSize;
        set {
            this._screenSize = value;
            this.OnRelayout();
        }
    }

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

        LocalizationManager.LanguageChanged += this.LanguageChanged;
    }
    
    private void LanguageChanged(object sender, Language e) {
        this._screen.UpdateTextStrings();
    }

    private void OnRelayout() {
        this._screen.ManagerOnOnScalingRelayoutNeeded(this, this._screen.Manager.Size);
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

        LocalizationManager.LanguageChanged -= this.LanguageChanged;

        this._screen.Unload();
        this._screen.Dispose();
    }
}

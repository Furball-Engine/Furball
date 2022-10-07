using System.Numerics;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Localization;
using Furball.Engine.Engine.Localization.Languages;

namespace Furball.Engine.Engine.Graphics.Drawables;

public class ScreenDrawable : Drawable {
    public readonly Screen  Screen;
    private         Vector2 _screenSize;

    public Vector2 ScreenSize {
        get => this._screenSize;
        set {
            this._screenSize = value;
            this.OnRelayout();
        }
    }

    public override Vector2 Size => this.ScreenSize * this.Scale;

    public ScreenDrawable(Screen screen, Vector2 position, Vector2 size) {
        this.Screen = screen;
        
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
        this.Screen.UpdateTextStrings();
    }

    private void OnRelayout() {
        this.Screen.ManagerOnOnScalingRelayoutNeeded(this, this.Screen.Manager.Size);
    }

    public override void Update(double time) {
        base.Update(time);
        
        this.Screen.Manager.Position = this.RealPosition;
        this.Screen.Manager.Size     = this.ScreenSize * this.RealScale;
        
        this.Screen.Update(time);
    }

    public override void Draw(double time, DrawableBatch batch, DrawableManagerArgs args) {
        base.Draw(time, batch, args);
        
        this.Screen.Draw(time);
    }

    public override void Dispose() {
        base.Dispose();

        LocalizationManager.LanguageChanged -= this.LanguageChanged;

        this.Screen.Unload();
        this.Screen.Dispose();
    }
}

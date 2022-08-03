using System;
using Furball.Engine.Engine.Localization;
using Furball.Volpe.Evaluation;
using Silk.NET.Input;

namespace Furball.Engine.Engine.Config; 

public class FurballConfig : VolpeConfig {
    public override string Name => "furball";

    public int ScreenWidth  => (int)this.Values["screen_width"].ToNumber().Value;
    public int ScreenHeight => (int)this.Values["screen_height"].ToNumber().Value;


    public bool LimitFPS  => this.Values["limit_fps"].ToBoolean().Value;
    public int  TargetFPS => (int)this.Values["target_fps"].ToNumber().Value;
    public bool UnfocusCap => this.Values["unfocus_cap"].ToBoolean().Value;

    public bool Fullscreen => this.Values["fullscreen"].ToBoolean().Value;
    public string Language => this.Values["language"].ToStringValue().Value;

    public void SetKeybind(object identifier, Key key) {
        this.Values[$"keybind_{identifier}"] = new Value.String(key.ToString());
    }

    public bool GetKeybind(object identifier, out Key key) {
        if (!this.Values.ContainsKey($"keybind_{identifier}")) {
            key = 0;
            return false;
        }

        return Enum.TryParse(this.Values[$"keybind_{identifier}"].ToStringValue().Value, out key);
    }
    
    public FurballConfig() {
        this.Values["screen_width"]  = new Value.Number(FurballGame.DEFAULT_WINDOW_WIDTH);
        this.Values["screen_height"] = new Value.Number(FurballGame.DEFAULT_WINDOW_HEIGHT);

        this.Values["limit_fps"]   = new Value.Boolean(true);
        this.Values["target_fps"]  = new Value.Number(240);
        this.Values["limit_ups"]   = new Value.Boolean(false);
        this.Values["target_ups"]  = new Value.Number(1000);
        this.Values["unfocus_cap"] = new Value.Boolean(true);

        this.Values["fullscreen"] = new Value.Boolean(false);

        this.Values["language"] = new Value.String(LocalizationManager.DefaultLanguage.Iso6392Code().ToString());
    }

    public static FurballConfig Instance;
}
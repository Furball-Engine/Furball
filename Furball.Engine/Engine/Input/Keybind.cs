using Furball.Engine.Engine.Config;
using Furball.Engine.Engine.OldInput;
using Silk.NET.Input;

namespace Furball.Engine.Engine.Input; 

public class Keybind {
    public delegate void Pressed(FurballKeyboard keyboard);

    public Keybind(object identifier, string name, Key @default, Pressed onPressed) {
        if (!FurballConfig.Instance.GetKeybind(identifier, out Key key)) {
            FurballConfig.Instance.SetKeybind(identifier, @default);
            FurballConfig.Instance.GetKeybind(identifier, out key);
        }

        //If we cannot parse the config key, then set to default
        this._key       = key;
        this.DefaultKey = @default;
        this.OnPressed  = onPressed;

        this.Name       = name;
        this.Identifier = identifier;
    }
    
    public bool   Enabled = true;
    public Key    DefaultKey;

    private Key _key;
    public Key Key {
        get => this._key;
        set {
            this._key = value;
            FurballConfig.Instance.SetKeybind(this.Identifier, value);
        }
    }

    public object Identifier;
    public string Name;
    public string Tooltip;
    
    public Pressed OnPressed;
}

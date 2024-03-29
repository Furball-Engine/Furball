using Furball.Engine.Engine.Config;
using Furball.Engine.Engine.Input.Events;
using Silk.NET.Input;

namespace Furball.Engine.Engine.Input; 

public class Keybind {
    public delegate void Pressed(KeyEventArgs keyboard);

    public Keybind(object identifier, string name, Key @default, Key[] modifiers, Pressed onPressed) {
        //If we cannot parse the config key, then set to default
        if (!FurballConfig.Instance.GetKeybind(identifier, out Key key)) {
            FurballConfig.Instance.SetKeybind(identifier, @default);
            FurballConfig.Instance.GetKeybind(identifier, out key);
        }

        this._key       = key;
        this.Modifiers  = modifiers;
        this.DefaultKey = @default;
        this.OnPressed  = onPressed;

        this.Name       = name;
        this.Identifier = identifier;
    }
    
    public bool  Enabled = true;
    public Key[] Modifiers;
    public Key   DefaultKey;

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

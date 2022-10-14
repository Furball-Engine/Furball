using System.Collections.Generic;
using Silk.NET.Input;

namespace Furball.Engine.Engine.Input.InputMethods;

public class SilkWindowingKeyboardInputMethod : InputMethod {
    private readonly IInputContext            _inputContext;
    private          IReadOnlyList<IKeyboard> _silkKeyboards;
    
    public SilkWindowingKeyboardInputMethod(IInputContext inputContext) {
        this._inputContext = inputContext;

    }

    public override void Update() {}

    public override void Dispose() {
        this.Keyboards.Clear();
    }

    public override void Initialize() {
        this._silkKeyboards = this._inputContext.Keyboards;

        this.Keyboards.Capacity += this._silkKeyboards.Count;

        for (int i = 0; i < this._silkKeyboards.Count; i++) {
            IKeyboard silkKeyboard = this._silkKeyboards[i];

            int j = i;
            FurballKeyboard keyboard = new() {
                Name         = silkKeyboard.Name,
                GetClipboard = () => this._silkKeyboards[j].ClipboardText,
                SetClipboard = s => this._silkKeyboards[j].ClipboardText = s,
                BeginInput   = () => this._silkKeyboards[j].BeginInput(),
                EndInput     = () => this._silkKeyboards[j].EndInput()
            };

            silkKeyboard.KeyChar += (_, c) => this.OnKeyboardChar(c, j);
            silkKeyboard.KeyDown += (_, key, _) => this.OnKeyboardDown(key, j);
            silkKeyboard.KeyUp   += (_, key, _) => this.OnKeyboardUp(key, j);

            //Dont let the list from currKeyboards carry over to here
            keyboard.QueuedTextInputs = new List<char>();
            this.Keyboards.Add(keyboard);
        }
    }

    private void OnKeyboardDown(Key key, int i) {
        this.Keyboards[i].PressedKeys.Add(key);
        this.Keyboards[i].QueuedKeyPresses.Add(key);
    }

    private void OnKeyboardUp(Key key, int i) {
        this.Keyboards[i].PressedKeys.Remove(key);
        this.Keyboards[i].QueuedKeyReleases.Add(key);
    }

    private void OnKeyboardChar(char c, int i) {
        this.Keyboards[i].QueuedTextInputs.Add(c);
    }
}

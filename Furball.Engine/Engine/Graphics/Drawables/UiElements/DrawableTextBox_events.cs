using System;

namespace Furball.Engine.Engine.Graphics.Drawables.UiElements;

public partial class DrawableTextBox {
    /// <summary>
    ///     Called when a letter is typed in the text box
    /// </summary>
    public event EventHandler<char> OnLetterTyped;
    /// <summary>
    ///     Called when a letter is removed from the text box
    /// </summary>
    public event EventHandler<char> OnLetterRemoved;
    /// <summary>
    ///     Called when the user "commits" the text in the text box, aka when they press enter
    /// </summary>
    public event EventHandler<string> OnCommit;
    /// <summary>
    ///     Called when focus changes on the textbox
    /// </summary>
    public event EventHandler<bool> OnFocusChange;
}

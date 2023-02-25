#nullable enable
using System;

namespace Furball.Engine.Engine.Input.InputMethods; 

public abstract class InputMethod : IDisposable {
    /// <summary>
    /// Whether or not the InputMethod has been initialized
    /// </summary>
    internal bool IsInitialized = false;
    /// <summary>
    /// Whether the InputMethod is queued to be removed
    /// </summary>
    internal bool Remove = false;
    
    public event EventHandler<FurballKeyboard>? KeyboardAdded;
    protected void OnKeyboardAdded(FurballKeyboard keyboard) => this.KeyboardAdded?.Invoke(this, keyboard);
    public event EventHandler<FurballKeyboard>? KeyboardRemoved;
    protected void OnKeyboardRemoved(FurballKeyboard keyboard) => this.KeyboardRemoved?.Invoke(this, keyboard);

    public event EventHandler<FurballMouse>? MouseAdded;
    protected void OnMouseAdded(FurballMouse mouse) => this.MouseAdded?.Invoke(this, mouse);
    public event EventHandler<FurballMouse>? MouseRemoved;
    protected void OnMouseRemoved(FurballMouse mouse) => this.MouseRemoved?.Invoke(this, mouse);
    
    /// <summary>
    /// Used if the InputMethod needs to constantly poll a source
    /// </summary>
    public abstract void Update();
    /// <summary>
    /// Used if the InputMethod needs to destruct itself safely
    /// </summary>
    public abstract void Dispose();
    /// <summary>
    /// Used if the InputMethod needs to Initialize itself before being updated
    /// </summary>
    /// <param name="inputManager"></param>
    public abstract void Initialize(InputManager inputManager);
}
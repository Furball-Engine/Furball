using System.Collections.Generic;
using Silk.NET.Input;

namespace Furball.Engine.Engine.Input; 

public abstract class InputMethod {
    /// <summary>
    /// The registered mice
    /// </summary>B
    // public List<IMouse> Mice = new();
    public List<FurballMouse> Mice = new();
    /// <summary>
    /// The registered keyboards
    /// </summary>
    // public List<IKeyboard> Keyboards = new();
    public List<FurballKeyboard> Keyboards = new();

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
    public abstract void Initialize();
}
using Furball.Engine.Engine.Input.Events;

namespace Furball.Engine.Engine.Input; 

public interface ICharInputHandler {
    public bool SaveInStack { get; set; }
    
    public void HandleChar(CharInputEvent ev);
    public void HandleFocus();
    public void HandleDefocus();
}

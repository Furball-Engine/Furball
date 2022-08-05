namespace Furball.Engine.Engine.Input; 

public interface ICharInputHandler {
    public void HandleChar(char c);
    public void HandleFocus();
    public void HandleDefocus();
}

namespace Furball.Engine.Engine.Input {
    public class MouseState {
        public Microsoft.Xna.Framework.Input.MouseState State;
        public string                                   Name;

        public MouseState(Microsoft.Xna.Framework.Input.MouseState state, string name) {
            this.State = state;
            this.Name  = name;
        }
    }
}

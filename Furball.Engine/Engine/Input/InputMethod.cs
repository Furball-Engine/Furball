using System.Collections.Generic;
using Silk.NET.Input;

namespace Furball.Engine.Engine.Input {
    public abstract class InputMethod {
        /// <summary>
        /// The registered mice
        /// </summary>
        public List<IMouse> Mice = new();
        public List<FurballMouseState> MouseStates = new();
        /// <summary>
        /// The registered keyboards
        /// </summary>
        public List<IKeyboard> Keyboards = new();
        public List<Key> HeldKeys = new();

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
}

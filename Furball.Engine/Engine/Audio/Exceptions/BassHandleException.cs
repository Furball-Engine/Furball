using System;

namespace Furball.Engine.Engine.Audio.Exceptions {
    public class BassHandleException : Exception {
        public override string Message => "The handle is invalid!";
    }
}

using System;

namespace Furball.Engine.Engine.Audio.Exceptions {
    public class BassFlagSetException : Exception {
        public override string Message => "Error while settings a flag";
    }
}

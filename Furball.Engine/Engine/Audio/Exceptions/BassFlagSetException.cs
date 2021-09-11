using System;

namespace ManagedManagedBass.Exceptions {
    public class BassFlagSetException : Exception {
        public override string Message => "Error while settings a flag";
    }
}

using System;

namespace ManagedManagedBass.Exceptions {
	public class BassHandleException : Exception {
		public override string Message => "The handle is invalid!";
	}
}

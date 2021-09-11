using Furball.Engine.Engine.Audio;
using ManagedBass;

namespace Furball.Engine.Engine.Timing {
	public class AudioTimeSource : ITimeSource {
		public int GetCurrentTime() {
			if (!AudioEngine.ActiveStream.IsValidHandle) return 0;
			if (AudioEngine.ActiveStream.PlaybackState != PlaybackState.Playing) return 0;
			
			return (int)(AudioEngine.ActiveStream.CurrentTime * 1000d);
		}
	}
}

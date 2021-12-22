using sowelipisona;

namespace Furball.Engine.Engine.Timing {
    public class AudioStreamTimeSource : ITimeSource {
        public AudioStream Stream;

        public AudioStreamTimeSource(AudioStream stream) => this.Stream = stream;

        public int GetCurrentTime() => (int)this.Stream.CurrentPosition;
    }
}

using System.Runtime.InteropServices;
using Furball.Engine.Engine.Audio.Exceptions;
using ManagedBass;

namespace Furball.Engine.Engine.Audio {
    public class SoundEffect {
        public static int MaxSimultaneousPlaybacks = 32;// chosen due to hardware limitations 
        
        private GCHandle _memoryHandle;

        private int _sampleHandle;

        public float Volume = 1f;
        
        public void Load(byte[] sampleData) {
            this._memoryHandle = GCHandle.Alloc(sampleData, GCHandleType.Pinned);

            this._sampleHandle = Bass.SampleLoad(this._memoryHandle.AddrOfPinnedObject(), 0, sampleData.Length, MaxSimultaneousPlaybacks, BassFlags.SampleOverrideLongestPlaying);

            if(this._sampleHandle == 0)
                throw Bass.LastError switch {
                    Errors.Init         => new BassInitException(),
                    Errors.NotAvailable => new BassNotAvailableException(),
                    Errors.Parameter    => new BassParameterException(),
                    Errors.FileOpen     => new BassFileOpenException(),
                    Errors.FileFormat   => new BassFileFormatException(),
                    Errors.Codec        => new BassCodecException(),
                    Errors.SampleFormat => new BassSampleFormatException(),
                    Errors.Memory       => new BassMemoryException(),
                    Errors.No3D         => new BassNo3DException(),
                    _                   => new BassUnknownException()
                };
        }

        public void Play() {
            int handle = Bass.SampleGetChannel(this._sampleHandle);

            if (handle == 0) 
                throw Bass.LastError switch {
                    Errors.Handle    => new BassHandleException(),
                    Errors.NoChannel => new BassNoChannelException(),
                    Errors.Timeout   => new BassTimeoutException(),
                    _                => new BassUnknownException()
                };

            if(!Bass.ChannelPlay(handle)) {
                throw Bass.LastError switch {
                    Errors.Handle => new BassHandleException(),
                    Errors.Start  => new BassStartException(),
                    Errors.Decode => new BassDecodeException(),
                    _             => new BassUnknownException()
                };
            }

            Bass.ChannelSetAttribute(handle, ChannelAttribute.Volume, this.Volume);
        }
    }
}

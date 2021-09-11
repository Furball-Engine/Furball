using System.Runtime.InteropServices;
using Furball.Engine.Engine.Timing;
using ManagedBass;
using ManagedBass.Fx;
using ManagedManagedBass.Exceptions;

namespace Furball.Engine.Engine.Audio {
	public class AudioStream : ITimeSource {
		private int      _rawAudioHandle;
		private int      _audioHandle;
		private int      _reverseFxAudioHandle;
		private GCHandle _memoryHandle;

		public AudioStream() { }

		public AudioStream(byte[] audioData, BassFlags extraFlags = BassFlags.Default) {
			this.Load(audioData, extraFlags);
		}

		public bool IsValidHandle {
			get {
				try {
					Bass.ChannelGetInfo(this._audioHandle);
				}
				catch {
					// ignored
				}

				return Bass.LastError != Errors.Handle;
			}
		}

		public PlaybackState PlaybackState => Bass.ChannelIsActive(this._audioHandle);

		public float Volume {
			get {
				bool success = Bass.ChannelGetAttribute(this._audioHandle, ChannelAttribute.Volume, out float volume);

				if (success)
					return volume;

				throw Bass.LastError switch {
					Errors.Handle => new BassHandleException(),
					Errors.Type   => new BassAttributeNotValidException(),
					_             => new BassUnknownException()
				};
			}
			set {
				bool success = Bass.ChannelSetAttribute(this._audioHandle, ChannelAttribute.Volume, value);

				if (success) return;

				throw Bass.LastError switch {
					Errors.Handle    => new BassHandleException(),
					Errors.Type      => new BassAttributeNotValidException(),
					Errors.Parameter => new BassParameterException(),
					_                => new BassUnknownException()
				};
			}
		}

		public double CurrentTime {
			get {
				long audioTimeRaw = Bass.ChannelGetPosition(this._audioHandle);

				if (audioTimeRaw != -1)
					return Bass.ChannelBytes2Seconds(this._audioHandle, audioTimeRaw);

				throw Bass.LastError switch {
					Errors.Handle       => new BassHandleException(),
					Errors.NotAvailable => new BassNotAvailableException(),
					_                   => new BassUnknownException()
				};
			}
		}

		public bool Loop {
			get {
				BassFlags flags = Bass.ChannelFlags(this._audioHandle, 0, 0);

				if ((int)flags == -1 && Bass.LastError == Errors.Handle) throw new BassHandleException();

				return (flags & BassFlags.Loop) > 0;
			}
			set => this.SetFlag(value, BassFlags.Loop);
		}

		public float Pitch {
			get {
				bool success = Bass.ChannelGetAttribute(this._audioHandle, ChannelAttribute.Pitch, out float pitch);
		
				if (success) return pitch;
				
				throw Bass.LastError switch {
					Errors.Handle => new BassHandleException(),
					Errors.Type   => new BassAttributeNotValidException(),
					_             => new BassUnknownException()
				};
			}
			set {
				bool success = Bass.ChannelSetAttribute(this._audioHandle, ChannelAttribute.Pitch, value);
		
				if (success) return;
				
				throw Bass.LastError switch {
					Errors.Handle    => new BassHandleException(),
					Errors.Type      => new BassAttributeNotValidException(),
					Errors.Parameter => new BassParameterException(),
					_                => new BassUnknownException()
				};
			}
		}
		
		public float Frequency {
			get {
				bool success = Bass.ChannelGetAttribute(this._audioHandle, ChannelAttribute.Frequency, out float frequency);

				if (success) return frequency;

				throw Bass.LastError switch {
					Errors.Handle => new BassHandleException(),
					Errors.Type   => new BassAttributeNotValidException(),
					_             => new BassUnknownException()
				};
			}
			set {
				bool success = Bass.ChannelSetAttribute(this._audioHandle, ChannelAttribute.Frequency, value);

				if (success) return;

				throw Bass.LastError switch {
					Errors.Handle    => new BassHandleException(),
					Errors.Type      => new BassAttributeNotValidException(),
					Errors.Parameter => new BassParameterException(),
					_                => new BassUnknownException()
				};
			}
		}

		public void SeekTo(int milliseconds) {
			bool success = Bass.ChannelSetPosition(this._audioHandle, milliseconds / 1000);

			if (success) return;

			throw Bass.LastError switch {
				Errors.Handle       => new BassHandleException(),
				Errors.NotFile      => new BassNotFileException(),
				Errors.Position     => new BassPositionException(),
				Errors.NotAvailable => new BassNotAvailableException(),
				_                   => new BassUnknownException()
			};
		}

		public void Pause() {
			bool success = Bass.ChannelPause(this._audioHandle);

			if (success) return;

			throw Bass.LastError switch {
				Errors.Handle     => new BassHandleException(),
				Errors.NotPlaying => new BassNotPlayingException(),
				Errors.Decode     => new BassDecodeException(),
				Errors.Already    => new BassAlreadyPausedException(),
				_                 => new BassUnknownException()
			};
		}

		public void Play() {
			bool success = Bass.ChannelPlay(this._audioHandle);

			if (success) return;

			throw Bass.LastError switch {
				Errors.Handle => new BassHandleException(),
				Errors.Decode => new BassDecodeException(),
				_             => new BassUnknownException()
			};
		}

		public void Stop(bool reset = true) {
			bool success = Bass.ChannelStop(this._audioHandle);

			if (!success)
				throw Bass.LastError switch {
					Errors.Handle => new BassHandleException(),
					_             => new BassUnknownException()
				};

			if (reset) this.SeekTo(0);
		}

		private void SetFlag(bool status, BassFlags flag) {
			BassFlags flags = Bass.ChannelFlags(this._audioHandle, status ? flag : BassFlags.Default, flag);

			if ((int)flags == -1 && Bass.LastError == Errors.Handle) throw new BassHandleException();

			bool isSet = (flags & flag) == flag;

			switch (isSet) {
				case true when status:
					break;
				case false when !status:
					break;
				default:
					throw new BassFlagSetException();
			}
		}

		public void Load(byte[] audioData, BassFlags extraFlags = BassFlags.Default) {
			this._memoryHandle = GCHandle.Alloc(audioData, GCHandleType.Pinned);

			int tempAudioHandle = Bass.CreateStream(
				this._memoryHandle.AddrOfPinnedObject(),
				0,
				audioData.Length,
				BassFlags.Prescan | BassFlags.Decode | extraFlags
			);

			if (tempAudioHandle == 0)
				throw Bass.LastError switch {
					Errors.Init         => new BassInitException(),
					Errors.NotAvailable => new BassNotAvailableException(),
					Errors.Parameter    => new BassParameterException(),
					Errors.FileOpen     => new BassFileOpenException(),
					Errors.FileFormat   => new BassFileFormatException(),
					Errors.Codec        => new BassCodecException(),
					Errors.SampleFormat => new BassSampleFormatException(),
					Errors.Speaker      => new BassSpeakerException(),
					Errors.Memory       => new BassMemoryException(),
					Errors.No3D         => new BassNo3DException(),
					_                   => new BassUnknownException()
				};

			this._rawAudioHandle = tempAudioHandle;

			int tempAudioHandle2 = BassFx.TempoCreate(tempAudioHandle, BassFlags.Prescan | extraFlags);

			if (tempAudioHandle2 == 0)
				throw Bass.LastError switch {
					Errors.Handle       => new BassHandleException(),
					Errors.Decode       => new BassDecodeException(),
					Errors.SampleFormat => new BassSampleFormatException(),
					_                   => new BassUnknownException()
				};

			this._audioHandle = tempAudioHandle2;
			
			tempAudioHandle = BassFx.ReverseCreate(tempAudioHandle, 5f, BassFlags.Prescan | extraFlags);

			if (tempAudioHandle == 0)
				throw Bass.LastError switch {
					Errors.Handle       => new BassHandleException(),
					Errors.Decode       => new BassDecodeException(),
					Errors.SampleFormat => new BassSampleFormatException(),
					_                   => new BassUnknownException()
				};

			this._reverseFxAudioHandle = tempAudioHandle;
		}
		public int GetCurrentTime() => (int)(this.CurrentTime * 1000d);
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Furball.Engine.Engine.Platform.Linux;
using ManagedBass;
using ManagedBass.Fx;

namespace Furball.Engine.Engine.Audio {
	public static class AudioEngine {
		public static int DefaultAudioDevice = -1;

		public static void Initialize(IntPtr windowId = default) {
			if (windowId == default) windowId = IntPtr.Zero;

			if (Platform.RuntimeInfo.CurrentPlatform() == OSPlatform.Linux) {
				Console.WriteLine("Loading Linux Bass Libraries!");

				Library.Load("libbass.so", Library.LoadFlags.RTLD_LAZY | Library.LoadFlags.RTLD_GLOBAL);
				
				// I should be loading this manually too but it seems fine without
				// Library.Load("/usr/lib/libbass_fx.so", Library.LoadFlags.RTLD_LAZY | Library.LoadFlags.RTLD_GLOBAL);
			}

			Bass.Init(DefaultAudioDevice, 44100, DeviceInitFlags.Default, windowId);
			Bass.PluginLoad("/usr/lib/libbass_fx.so");
			
			Console.WriteLine($"Bass Version: {Bass.Version}\nBassFx Version: {BassFx.Version}");
		}

		public static AudioStream LoadFile(string filename, BassFlags flags = BassFlags.Default) {
			byte[] audioData = File.ReadAllBytes(filename);

			return new AudioStream(audioData, flags);
		}
	}
}

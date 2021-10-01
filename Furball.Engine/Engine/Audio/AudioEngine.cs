using System;
using System.IO;
using ManagedBass;
using ManagedBass.Fx;
using System.Runtime.InteropServices;
using Furball.Engine.Engine.Platform.Linux;
using Furball.Engine.Engine.Audio.Exceptions;
using Furball.Engine.Engine.Helpers.Logger;

namespace Furball.Engine.Engine.Audio {
    public static class AudioEngine {
        public static int DefaultAudioDevice = -1;

        public static void Initialize(IntPtr windowId = default) {
            if (windowId == default) windowId = IntPtr.Zero;

            if (Platform.RuntimeInfo.CurrentPlatform() == OSPlatform.Linux) {
                Logger.Log("Loading Linux Bass Libraries", new LoggerLevelEngineInfo());

                Library.Load("libbass.so", Library.LoadFlags.RTLD_LAZY | Library.LoadFlags.RTLD_GLOBAL);

                // I should be loading this manually too but it seems fine without
                // Library.Load("/usr/lib/libbass_fx.so", Library.LoadFlags.RTLD_LAZY | Library.LoadFlags.RTLD_GLOBAL);
            }

            bool success = Bass.Init(DefaultAudioDevice, 44100, DeviceInitFlags.Latency | DeviceInitFlags.Default | DeviceInitFlags.Device3D, windowId);

            if (!success) {
                switch (Bass.LastError) {
                    case Errors.Device:
                        throw new BassInvalidDeviceException();
                    case Errors.Already:
                        throw new BassAlreadyInitializedException();
                    case Errors.Driver:
                        throw new BassNoAvailableDriveException();
                    case Errors.SampleFormat:
                        throw new BassSampleFormatNotSupportedException();
                    case Errors.Memory:
                        throw new InsufficientMemoryException();
                    case Errors.No3D:
                        throw new BassNo3DException();
                    case Errors.Unknown:
                        throw new BassUnknownException();
                }
            }

            Logger.Log($"Bass Version: {Bass.Version} BassFx Version: {BassFx.Version}", new LoggerLevelEngineInfo());
        }

        public static AudioStream LoadFile(string filename, BassFlags flags = BassFlags.Default) {
            byte[] audioData = File.ReadAllBytes(filename);

            return new AudioStream(audioData, flags);
        }

        public static AudioStream LoadFile(byte[] audioData, BassFlags flags = BassFlags.Default) => new(audioData, flags);
    }
}

using System.Runtime.InteropServices;

namespace Furball.Engine.Engine.Platform; 

public static class RuntimeInfo {
    public static OSPlatform CurrentPlatform() {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return OSPlatform.Windows;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return OSPlatform.Linux;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return OSPlatform.OSX;
        return OSPlatform.Windows;
    }

    public static bool IsDebug() {
#if DEBUG
        return true;
#else
		return false;
#endif
    }
}
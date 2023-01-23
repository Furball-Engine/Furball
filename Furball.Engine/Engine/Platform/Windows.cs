using System.Runtime.InteropServices;

namespace Furball.Engine.Engine.Platform; 

public static class Windows {
    [DllImport("kernel32.dll")]
    private static extern bool AttachConsole(int dwProcessId);

    public static void AttachToExistingConsole() {
        AttachConsole(-1);
    }
}

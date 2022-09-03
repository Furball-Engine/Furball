using System.Runtime.InteropServices;

namespace Furball.Engine.Engine.Helpers; 

public static class UnixEnvironment {
    private const long ROOT_USER_ID = 0;
    
    [DllImport("libc")]
    private static extern uint getuid();

    [DllImport("libc")]
    private static extern uint geteuid();

    public static uint GetUserId() {
        return getuid();
    }

    public static uint GetEffectiveUserId() {
        return geteuid();
    }

    /// <summary>
    /// Checks if the effective user id is the root user id
    /// </summary>
    /// <returns></returns>
    public static bool IsRoot() => geteuid() == ROOT_USER_ID;
}
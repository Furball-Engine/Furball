using System.Diagnostics;

namespace Furball.Engine.Engine.Helpers {
    public class LinkHelper {
        public static void OpenLink(string url) {
            ProcessStartInfo processInfo = new() {
                FileName        = url,
                UseShellExecute = true

            };
            Process.Start(processInfo);
        }
    }
}

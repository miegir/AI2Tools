using System.Diagnostics;

namespace AI2Tools;

public partial class Game
{
    public void Launch()
    {
        var path = File.Exists(launcherPath) ? launcherPath : gamePath;

        var startInfo = new ProcessStartInfo(path)
        {
            WorkingDirectory = Path.GetDirectoryName(gamePath),
        };

        Process.Start(startInfo)?.Dispose();
    }
}

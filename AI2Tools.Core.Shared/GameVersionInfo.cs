using System.Diagnostics;

namespace AI2Tools;

public class GameVersionInfo
{
    public GameVersionInfo(string path)
    {
        var fileVersionInfo = FileVersionInfo.GetVersionInfo(path);
        LastWriteTimeUtc = File.GetLastWriteTimeUtc(path);
        GameVersion = new GameVersion
        {
            FileVersionString = fileVersionInfo.FileVersion,
            ProductVersionString = fileVersionInfo.ProductVersion,
        };
    }

    public DateTime LastWriteTimeUtc { get; }
    public GameVersion GameVersion { get; }
}

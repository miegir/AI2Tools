using Microsoft.Extensions.Logging;

namespace AI2Tools;

internal partial class GameVersionResource
{
    public IEnumerable<Action> BeginExport(ExportArguments arguments)
    {
        return Enumerable.Empty<Action>();
    }

    public IEnumerable<Action> BeginImport(ImportArguments arguments)
    {
        return Enumerable.Empty<Action>();
    }

    public IEnumerable<Action> BeginMuster(MusterArguments arguments)
    {
        yield return () =>
        {
            logger.LogInformation("mustering game version...");

            var versionInfo = game.FindVersionInfo();
            if (versionInfo is null)
            {
                logger.LogError("game version info not found.");
                return;
            }

            arguments.Sink.ReportObject(
                GameVersionStatics.Path,
                new GameVersionStreamSource(versionInfo));
        };
    }

    private class GameVersionStreamSource : IObjectStreamSource
    {
        private readonly GameVersionInfo info;
        public GameVersionStreamSource(GameVersionInfo info) => this.info = info;
        public bool Exists => true;
        public DateTime LastWriteTimeUtc => info.LastWriteTimeUtc;
        public Stream OpenRead() => ObjectSerializer.SerializeToStream(info.GameVersion);
    }
}

using Microsoft.Extensions.Logging;

namespace AI2Tools;

public partial class Game
{
    private readonly ILogger logger;
    private readonly string gamePath;
    private readonly string launcherPath;
    private readonly string textLanguagesDir;
    private readonly string bundlesDir;
    private readonly string metadataDir;

    public Game(ILogger logger, string gamePath)
    {
        this.logger = logger;
        this.gamePath = gamePath;
        var gameDir = Path.GetDirectoryName(gamePath) ?? string.Empty;
        var gameName = Path.GetFileNameWithoutExtension(gamePath);
        var dataDir = Path.Combine(gameDir, gameName + "_Data");
        var assetsDir = Path.Combine(dataDir, "StreamingAssets");
        launcherPath = Path.Combine(dataDir, "Launcher", "AI2ProfileSelector.exe");
        textLanguagesDir = Path.Combine(assetsDir, "Text");
        bundlesDir = Path.Combine(assetsDir, "aa", "StandaloneWindows64");
        metadataDir = Path.Combine(dataDir, "il2cpp_data", "Metadata");
    }

    public GamePipeline CreatePipeline()
    {
        return new GamePipeline(logger, EnumerateResources());

        IEnumerable<IResource> EnumerateResources()
        {
            foreach (var source in FileSource.EnumerateFiles(metadataDir, "*.dat"))
            {
                yield return new Il2CppMetadataResource(logger, source);
            }

            foreach (var path in Directory.EnumerateDirectories(textLanguagesDir))
            {
                var languageName = Path.GetFileName(path);

                foreach (var source in FileSource.EnumerateFiles(path, "*."))
                {
                    yield return new TextMapResource(logger, source, languageName);
                }
            }

            foreach (var source in FileSource.EnumerateFiles(bundlesDir, BundleResource.SearchPatterns))
            {
                yield return new BundleResource(logger, source);
            }
        }
    }
}

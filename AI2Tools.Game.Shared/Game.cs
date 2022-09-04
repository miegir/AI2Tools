using Microsoft.Extensions.Logging;

namespace AI2Tools;

public partial class Game
{
    private readonly ILogger logger;
    private readonly string textLanguagesDir;
    private readonly string bundlesDir;

    public Game(ILogger logger, string gamePath)
    {
        this.logger = logger;
        var gameDir = Path.GetDirectoryName(gamePath) ?? string.Empty;
        var gameName = Path.GetFileNameWithoutExtension(gamePath);
        var assetsDir = Path.Combine(gameDir, gameName + "_Data", "StreamingAssets");
        textLanguagesDir = Path.Combine(assetsDir, "Text");
        bundlesDir = Path.Combine(assetsDir, "aa", "StandaloneWindows64");
    }

    public GamePipeline CreatePipeline()
    {
        return new GamePipeline(logger, EnumerateResources());

        IEnumerable<IResource> EnumerateResources()
        {
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

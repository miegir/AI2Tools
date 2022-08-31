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

    public GamePipeline CreatePipeline(string textLanguage)
    {
        var textMapsDir = Path.Combine(textLanguagesDir, textLanguage);

        return new GamePipeline(logger, EnumerateResources());

        IEnumerable<IResource> EnumerateResources()
        {
            foreach (var source in FileSource.EnumerateFiles(textMapsDir, "*."))
            {
                yield return new TextMapResource(logger, source, textLanguage);
            }

            foreach (var source in FileSource.EnumerateFiles(bundlesDir, BundleResource.SearchPatterns))
            {
                yield return new BundleResource(logger, source);
            }
        }
    }
}

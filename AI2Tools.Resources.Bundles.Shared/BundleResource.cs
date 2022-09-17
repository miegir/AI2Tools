using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace AI2Tools;

public partial class BundleResource : IResource
{
    public static readonly string[] SearchPatterns =
    {
        "video*.bundle",
        "scene-bonus_bake_scenes_all*.bundle",
        "scene-search_scenes_all*.bundle",
        "scene-title_scenes_all*.bundle",
        "fonts*.bundle",
        "image*.bundle",
        "etc*.bundle",
        "ui*.bundle",
        "item-md_iib???_assets_all*.bundle"
    };

    private readonly ILogger logger;
    private readonly FileSource source;
    private readonly string name;

    public BundleResource(ILogger logger, FileSource source)
    {
        this.logger = logger;
        this.source = source;
        name = Regex.Replace(source.FileNameWithoutExtension, @"_[\da-fA-F]+$", "");
    }

    public IEnumerable<Action> BeginUnpack(UnpackArguments arguments)
    {
        var directory = ObjectPath.Root.Append("aa", name);
        if (!arguments.Container.HasDirectory(directory)) return BeginUnroll();
        return Enumerate();
        IEnumerable<Action> Enumerate()
        {
            yield return () =>
            {
                logger.LogInformation("unpacking bundle {name}...", name);
                using (logger.BeginScope("bundle {name}", name))
                {
                    var manager = new BundleManager(logger, source);
                    manager.Unpack(arguments, directory);
                }
            };
        }
    }

    public IEnumerable<Action> BeginUnroll()
    {
        if (source.CanUnroll())
        {
            yield return () =>
            {
                logger.LogInformation("unrolling bundle {name}...", name);
                source.Unroll();
            };
        }
    }
}

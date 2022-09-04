using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace AI2Tools;

public partial class BundleResource : IResource
{
    public static readonly string[] SearchPatterns =
    {
        "scene-bonus_bake_scenes_all*.bundle",
        "scene-title_scenes_all*.bundle",
        "fonts*.bundle",
        "image*.bundle",
        "etc*.bundle",
        "ui*.bundle",
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

    public Action? BeginUnpack(UnpackArguments arguments)
    {
        var directory = ObjectPath.Root.Append("aa", name);
        if (!arguments.Container.HasDirectory(directory)) return BeginUnroll();
        return () =>
        {
            logger.LogInformation("unpacking bundle {name}...", name);
            using (logger.BeginScope("bundle {name}", name))
            {
                var manager = new BundleManager(logger, source);
                manager.Unpack(arguments, directory);
            }
        };
    }

    public Action? BeginUnroll()
    {
        return source.CanUnroll() ? () =>
        {
            logger.LogInformation("unrolling bundle {name}...", name);
            source.Unroll();
        }
        : null;
    }
}

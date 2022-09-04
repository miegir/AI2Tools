using Microsoft.Extensions.Logging;

namespace AI2Tools;

public partial class TextMapResource : IResource
{
    private readonly ILogger logger;
    private readonly FileSource source;
    private readonly string languageName;
    private readonly string name;
    private readonly string fullName;

    public TextMapResource(ILogger logger, FileSource source, string languageName)
    {
        this.logger = logger;
        this.source = source;
        this.languageName = languageName;
        name = source.FileNameWithoutExtension;
        fullName = languageName + "/" + name;
    }

    public Action? BeginUnpack(UnpackArguments arguments)
    {
        var directory = ObjectPath.Root.Append("Text", languageName);
        if (!arguments.Container.HasDirectory(directory)) return BeginUnroll();
        var path = directory.Append(name + ".pak");
        if (!arguments.Container.TryGetEntry(path, out var entry) && !arguments.Debug) return BeginUnroll();
        if (!arguments.Debug && entry is null) return BeginUnroll();
        var manager = new TextMapManager(logger, source);
        if (manager.IsEmpty) return BeginUnroll();
        return () =>
        {
            logger.LogInformation("unpacking text map {name}...", fullName);
            using (logger.BeginScope("text map {name}", fullName))
            {
                var objectSource = entry is not null
                    ? entry.AsObjectSource<Dictionary<string, TextMapTranslation>>()
                    : DelegateObjectSource.Create<Dictionary<string, TextMapTranslation>>();
                manager.Import(objectSource, arguments.Debug ? name : null);
            }

            logger.LogInformation("saving text map {name}...", fullName);
            using var target = source.CreateTarget();
            manager.Save(target.Stream);
            target.Commit();
        };
    }

    public Action? BeginUnroll()
    {
        return source.CanUnroll() ? () =>
        {
            logger.LogInformation("unrolling text map {name}...", fullName);
            source.Unroll();
        }
        : null;
    }
}

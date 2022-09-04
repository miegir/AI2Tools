using Microsoft.Extensions.Logging;

namespace AI2Tools;

public partial class TextMapResource : IResource
{
    private readonly ILogger logger;
    private readonly FileSource source;
    private readonly string textLanguage;
    private readonly string name;

    public TextMapResource(ILogger logger, FileSource source, string textLanguage)
    {
        this.logger = logger;
        this.source = source;
        this.textLanguage = textLanguage;
        name = source.FileNameWithoutExtension;
    }

    public Action? BeginUnpack(UnpackArguments arguments)
    {
        var path = ObjectPath.Root.Append("Text", name + ".pak");
        if (!arguments.Container.TryGetEntry(path, out var entry) && !arguments.Debug) return BeginUnroll();
        if (!arguments.Debug && entry is null) return BeginUnroll();
        var manager = new TextMapManager(logger, source);
        if (manager.IsEmpty) return BeginUnroll();
        return () =>
        {
            logger.LogInformation("unpacking text map {name}...", name);
            using (logger.BeginScope("text map {name}", name))
            {
                var objectSource = entry is not null
                    ? entry.AsObjectSource<Dictionary<string, TextMapTranslation>>()
                    : DelegateObjectSource.Create<Dictionary<string, TextMapTranslation>>();
                manager.Import(objectSource, arguments.Debug ? name : null);
            }

            logger.LogInformation("saving text map {name}...", name);
            using var target = source.CreateTarget();
            manager.Save(target.Stream);
            target.Commit();
        };
    }

    public Action? BeginUnroll()
    {
        return source.CanUnroll() ? () =>
        {
            logger.LogInformation("unrolling text map {name}...", name);
            source.Unroll();
        }
        : null;
    }
}

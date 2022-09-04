using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace AI2Tools;

public partial class TextMapResource : IResource
{
    public Action? BeginExport(ExportArguments arguments)
    {
        var path = Path.Combine(arguments.ExportDirectory, "Text", textLanguage, name + ".txt");
        if (!arguments.Force && File.Exists(path)) return null;
        return () =>
        {
            logger.LogInformation("Exporting text map {name}...", name);

            var manager = new TextMapManager(logger, source);
            using var target = new FileTarget(path);
            manager.Export(target.Stream);
            target.Commit();
        };
    }

    public Action? BeginImport(ImportArguments arguments)
    {
        var sourcePath = Path.Combine(arguments.SourceDirectory, "src", "Text", name + ".txt");
        var sourceExists = File.Exists(sourcePath);
        if (!arguments.Debug && !sourceExists)
        {
            return BeginUnroll();
        }

        var manager = new TextMapManager(logger, source);
        if (manager.IsEmpty)
        {
            return BeginUnroll();
        }

        var statePath = Path.Combine(arguments.ObjectDirectory, "Text", name + ".importstate");
        var stateChangeTracker = new SourceChangeTracker(
            source.Destination, statePath, JsonSerializer.Serialize(arguments.Debug));

        if (!arguments.ForceTargets && !stateChangeTracker.HasChanges())
        {
            return null;
        }

        return () =>
        {
            stateChangeTracker.RegisterSource(sourcePath);

            logger.LogInformation("importing text map {name}...", name);
            using (logger.BeginScope("text map {name}", name))
            {
                var objectSource = sourceExists
                    ? new TranslationSource(sourcePath)
                    : DelegateObjectSource.Create<Dictionary<string, TextMapTranslation>>();

                manager.Import(objectSource, arguments.Debug ? name : null);
            }

            logger.LogInformation("saving text map {name}...", name);
            using var target = source.CreateTarget();
            manager.Save(target.Stream);
            target.Commit();

            if (!manager.HasWarnings)
            {
                stateChangeTracker.Commit();
            }
        };
    }

    public Action? BeginMuster(MusterArguments arguments)
    {
        var sourcePath = Path.Combine(arguments.SourceDirectory, "src", "Text", name + ".txt");
        if (!File.Exists(sourcePath)) return null;
        return () =>
        {
            logger.LogInformation("mustering text map {name}...", name);

            var objectPath = Path.Combine(arguments.ObjectDirectory, "Text", name + ".pak");
            var builder = new ObjectBuilder(sourcePath, objectPath, arguments.ForceObjects);

            TextMapManager.BuildObject(builder);

            var musterPath = ObjectPath.Root.Append("Text", name + ".pak");
            arguments.Sink.ReportObject(musterPath, objectPath);
        };
    }
}

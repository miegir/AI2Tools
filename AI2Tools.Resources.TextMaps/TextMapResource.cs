using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace AI2Tools;

public partial class TextMapResource : IResource
{
    public IEnumerable<Action> BeginExport(ExportArguments arguments)
    {
        var path = Path.Combine(arguments.ExportDirectory, "Text", languageName, name + ".txt");
        if (!arguments.Force && File.Exists(path))
        {
            yield break;
        }

        yield return () =>
        {
            logger.LogInformation("Exporting text map {name}...", fullName);

            var manager = new TextMapManager(logger, source);
            using var target = new FileTarget(path);
            manager.Export(target.Stream);
            target.Commit();
        };
    }

    public IEnumerable<Action> BeginImport(ImportArguments arguments)
    {
        var directoryPath = Path.Combine(arguments.SourceDirectory, "src", "Text", languageName);
        if (!Directory.Exists(directoryPath))
        {
            return BeginUnroll();
        }

        var sourcePath = Path.Combine(directoryPath, name + ".txt");
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

        return Enumerate();

        IEnumerable<Action> Enumerate()
        {
            var statePath = Path.Combine(arguments.ObjectDirectory, "Text", languageName, name + ".importstate");
            var stateChangeTracker = new SourceChangeTracker(
                source.Destination, statePath, JsonSerializer.Serialize(arguments.Debug));

            if (!arguments.ForceTargets && !stateChangeTracker.HasChanges())
            {
                yield break;
            }

            yield return () =>
            {
                stateChangeTracker.RegisterSource(sourcePath);

                logger.LogInformation("importing text map {name}...", fullName);
                using (logger.BeginScope("text map {name}", fullName))
                {
                    var objectSource = sourceExists
                        ? new TranslationSource(sourcePath)
                        : DelegateObjectSource.Create<Dictionary<string, TextMapTranslation>>();

                    manager.Import(objectSource, arguments.Debug ? name : null);
                }

                logger.LogInformation("saving text map {name}...", fullName);
                using var target = source.CreateTarget();
                manager.Save(target.Stream);
                target.Commit();

                if (!manager.HasWarnings)
                {
                    stateChangeTracker.Commit();
                }
            };
        }
    }

    public IEnumerable<Action> BeginMuster(MusterArguments arguments)
    {
        var directoryPath = Path.Combine(arguments.SourceDirectory, "src", "Text", languageName);
        if (!Directory.Exists(directoryPath))
        {
            yield break;
        }

        var sourcePath = Path.Combine(directoryPath, name + ".txt");
        if (!File.Exists(sourcePath))
        {
            yield break;
        }

        yield return () =>
        {
            logger.LogInformation("mustering text map {name}...", fullName);

            var directory = ObjectPath.Root.Append("Text", languageName);
            arguments.Sink.ReportDirectory(directory);

            var objectPath = Path.Combine(arguments.ObjectDirectory, "Text", languageName, name + ".pak");
            var builder = new ObjectBuilder(sourcePath, objectPath, arguments.ForceObjects);

            TextMapManager.BuildObject(builder);

            var musterPath = directory.Append(name + ".pak");
            arguments.Sink.ReportObject(musterPath, objectPath);
        };
    }
}

using Microsoft.Extensions.Logging;

namespace AI2Tools;

public partial class BundleResource : IResource
{
    public Action? BeginExport(ExportArguments arguments)
    {
        var path = Path.Combine(arguments.ExportDirectory, "aa", name);
        return () =>
        {
            logger.LogInformation("exporting bundle {name}...", name);
            using (logger.BeginScope("bundle {name}", name))
            {
                using var manager = new BundleManager(logger, source);
                manager.Export(arguments with { ExportDirectory = path });
            }
        };
    }

    public Action? BeginMuster(MusterArguments arguments)
    {
        var sourceDirectory = Path.Combine(arguments.SourceDirectory, "src", "aa", name);
        var bundleFileSource = new BundleFileSource(
            logger, Path.Combine(arguments.SourceDirectory, "bundles", name));

        if (!Directory.Exists(sourceDirectory) && !bundleFileSource.Exists)
        {
            return null;
        }

        return () =>
        {
            var objectDirectory = Path.Combine(arguments.ObjectDirectory, "aa", name);
            logger.LogInformation("mustering bundle {name}...", name);
            using (logger.BeginScope("bundle {name}", name))
            {
                var directoryName = ObjectPath.Root.Append("aa", name);
                using var manager = new BundleManager(logger, source);
                if (manager.Muster(directoryName, bundleFileSource, arguments with
                {
                    SourceDirectory = sourceDirectory,
                    ObjectDirectory = objectDirectory,
                }))
                {
                    arguments.Sink.ReportDirectory(directoryName);
                }
            }
        };
    }

    public Action? BeginImport(ImportArguments arguments)
    {
        var sourceDirectory = Path.Combine(arguments.SourceDirectory, "src", "aa", name);
        var bundleFileSource = new BundleFileSource(
            logger, Path.Combine(arguments.SourceDirectory, "bundles", name));

        if (!Directory.Exists(sourceDirectory) && !bundleFileSource.Exists)
        {
            return null;
        }

        return () =>
        {
            var objectDirectory = Path.Combine(arguments.ObjectDirectory, "aa", name);
            logger.LogInformation("importing bundle {name}...", name);
            using (logger.BeginScope("bundle {name}", name))
            {
                var statePath = objectDirectory + ".importstate";
                var sourceChangeTracker = new SourceChangeTracker(source.Destination, statePath);

                using var manager = new BundleManager(logger, source);

                var shouldCommit = manager.Import(arguments with
                {
                    SourceDirectory = sourceDirectory,
                    ObjectDirectory = objectDirectory,
                }, bundleFileSource, sourceChangeTracker);

                if (shouldCommit)
                {
                    sourceChangeTracker.Commit();
                }
            }
        };
    }
}

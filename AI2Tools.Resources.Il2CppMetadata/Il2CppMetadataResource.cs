using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace AI2Tools;

public partial class Il2CppMetadataResource
{
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web)
        {
            WriteIndented = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

    public IEnumerable<Action> BeginExport(ExportArguments arguments)
    {
        var path = Path.Combine(arguments.ExportDirectory, "metadata", name + ".txt");
        if (File.Exists(path))
        {
            yield break;
        }

        var manager = new Il2CppMetadataManager(logger, source);
        yield return () =>
        {
            logger.LogInformation("exporting il2cpp metadata file {name}...", name);

            var translations = manager.StringLiterals
                .ToDictionary(src => src, src => default(string));

            using var target = new FileTarget(path);
            JsonSerializer.Serialize(target.Stream, translations, JsonOptions);
            target.Commit();
        };
    }

    public IEnumerable<Action> BeginImport(ImportArguments arguments)
    {
        var sourcePath = Path.Combine(arguments.SourceDirectory, "metadata", name + ".txt");
        if (!File.Exists(sourcePath)) return BeginUnroll();
        return Enumerate();

        IEnumerable<Action> Enumerate()
        {
            var objectPath = Path.Combine(arguments.ObjectDirectory, "metadata", name + ".pak");
            var sourceChangeTracker = new SourceChangeTracker(source.Destination, objectPath + ".state");

            sourceChangeTracker.RegisterSource(sourcePath);

            if (!sourceChangeTracker.HasChanges())
            {
                yield break;
            }

            yield return () =>
            {
                var manager = new Il2CppMetadataManager(logger, source);
                
                using var stream = File.OpenRead(sourcePath);
                
                var translations = JsonSerializer.Deserialize<Dictionary<string, string?>>(stream, JsonOptions);
                var hasWarnings = Translate(manager.StringLiterals, translations);

                Save(manager);

                if (!hasWarnings)
                {
                    sourceChangeTracker.Commit();
                }
            };
        }
    }

    public IEnumerable<Action> BeginMuster(MusterArguments arguments)
    {
        var sourcePath = Path.Combine(arguments.SourceDirectory, "metadata", name + ".txt");
        if (!File.Exists(sourcePath)) yield break;

        yield return () =>
        {
            logger.LogInformation("mustering il2cpp metadata file {name}...", name);

            var objectPath = Path.Combine(arguments.ObjectDirectory, "metadata", name + ".pak");
            var builder = new ObjectBuilder(sourcePath, objectPath, arguments.ForceObjects);

            builder.Build(stream =>
            {
                logger.LogInformation("building il2cpp metadata file {name}...", name);

                var translations =
                    JsonSerializer.Deserialize<Dictionary<string, string?>>(stream, JsonOptions)
                        ?? new();

                // compress
                var untranslated = new HashSet<string>();
                foreach (var entry in translations)
                {
                    if (entry.Value == null || entry.Value == entry.Key)
                    {
                        untranslated.Add(entry.Key);
                    }
                }

                foreach (var item in untranslated)
                {
                    translations.Remove(item);
                }

                return translations;
            });

            var musterPath = ObjectPath.Root.Append("metadata", name + ".pak");
            arguments.Sink.ReportObject(musterPath, objectPath);
        };
    }
}

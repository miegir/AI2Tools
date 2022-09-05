using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace AI2Tools;

public partial class Il2CppMetadataResource : IResource
{
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web)
        {
            WriteIndented = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

    private readonly ILogger logger;
    private readonly FileSource source;
    private readonly string name;

    public Il2CppMetadataResource(ILogger logger, FileSource source)
    {
        this.logger = logger;
        this.source = source;
        name = source.FileNameWithoutExtension;
    }

    public IEnumerable<Action> BeginUnpack(UnpackArguments arguments)
    {
        var path = ObjectPath.Root.Append("metadata", name + ".pak");
        if (!arguments.Container.TryGetEntry(path, out var entry)) return BeginUnroll();
        return Enumerate();

        IEnumerable<Action> Enumerate()
        {
            yield return () =>
            {
                logger.LogInformation("unpacking il2cpp metadata {name}...", name);

                var manager = new Il2CppMetadataManager(logger, source);
                var translations = entry.AsObjectSource<Il2CppMetadataTranslation[]>().Deserialize();

                Translate(manager.StringLiterals, translations);
                Save(manager);
            };
        }
    }

    public IEnumerable<Action> BeginUnroll()
    {
        if (source.CanUnroll())
        {
            yield return () =>
            {
                logger.LogInformation("unrolling il2cpp metadata {name}...", name);
                source.Unroll();
            };
        }
    }

    private bool Translate(string[] strings, Il2CppMetadataTranslation[]? translations)
    {
        logger.LogInformation("importing il2cpp metadata file {name}...", name);
        using (logger.BeginScope("importing metadata file {name}", name))
        {
            var hasWarnings = false;

            if (translations == null || translations.Length == 0)
            {
                return hasWarnings;
            }

            var i = 0;
            for (; i < strings.Length; i++)
            {
                var s = strings[i];

                if (translations.Length <= i)
                {
                    hasWarnings = true;
                    logger.LogWarning("Missing translation for index {index}. Expected: '{expected}'.", i, s);
                    continue;
                }

                var t = translations[i];

                if (s != t.Src)
                {
                    hasWarnings = true;
                    logger.LogWarning("Invalid source for index {index}. Expected: '{expected}'. Actual: '{actual}'.", i, s, t.Src);
                    continue;
                }

                if (t.Trx == null || t.Trx == t.Src)
                {
                    continue;
                }

                strings[i] = t.Trx;
            }

            for (; i < translations.Length; i++)
            {
                hasWarnings = true;
                logger.LogWarning("Extra translation for index {index}. Actual: '{actual}'.", i, translations[i].Src);
            }

            return hasWarnings;
        }
    }

    private void Save(Il2CppMetadataManager manager)
    {
        logger.LogInformation("saving il2cpp metadata {name}...", name);
        using var target = source.CreateTarget();
        manager.Save(target.Stream);
        target.Commit();
    }
}

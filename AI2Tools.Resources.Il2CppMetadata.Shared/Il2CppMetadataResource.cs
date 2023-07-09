using Microsoft.Extensions.Logging;

namespace AI2Tools;

public partial class Il2CppMetadataResource : IResource
{
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
                var translations = entry.AsObjectSource<Dictionary<string, string?>>().Deserialize();

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

    private bool Translate(string[] strings, Dictionary<string, string?>? translations)
    {
        logger.LogInformation("importing il2cpp metadata file {name}...", name);
        using (logger.BeginScope("importing metadata file {name}", name))
        {
            var hasWarnings = false;

            if (translations == null || translations.Count == 0)
            {
                return hasWarnings;
            }

            var unapplied = translations.Keys.ToHashSet();
            for (var i = 0; i < strings.Length; i++)
            {
                var s = strings[i];
                if (translations.TryGetValue(s, out var translated) && translated != null)
                {
                    strings[i] = translated;
                    unapplied.Remove(s);
                }
            }

            foreach (var s in unapplied)
            {
                hasWarnings = true;
                logger.LogWarning("Unapplied translation: {expected}.", s);
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

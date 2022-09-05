using AssetsTools.NET.Extra;
using MessagePack;
using Microsoft.Extensions.Logging;

namespace AI2Tools;

internal class BundleResolver
{
    private readonly ILogger logger;
    private readonly string directory;
    private readonly string objectPath;
    private Dictionary<string, string>? pathMap;

    public BundleResolver(ILogger logger, string directory, string objectPath)
    {
        this.logger = logger;
        this.directory = directory;
        this.objectPath = objectPath;
    }

    public FileStream? OpenBundle(string name)
    {
        if (!GetPathMap().TryGetValue(name, out var path)) return null;
        var bundleSource = new FileSource(path);
        return bundleSource.OpenRead();
    }

    Dictionary<string, string> GetPathMap()
    {
        return pathMap ??= GetPathMapCore();

        Dictionary<string, string> GetPathMapCore()
        {
            var bundlePaths = Directory.GetFiles(directory, "*.bundle");
            if (bundlePaths.Length == 0)
            {
                return new();
            }

            Dictionary<string, (string Name, DateTime)>? entries;
            var objectInfo = new FileInfo(objectPath);
            if (objectInfo.Exists)
            {
                entries = ReadEntries();

                if (entries != null)
                {
                    return entries
                        .GroupBy(e => e.Value.Name)
                        .ToDictionary(e => e.Key, e => e.First().Key);
                }
            }

            entries = new();
            var map = new Dictionary<string, string>();

            logger.LogInformation("building bundle map...");

            foreach (var bundlePath in bundlePaths)
            {
                logger.LogInformation("parsing bundle {name}...", Path.GetFileNameWithoutExtension(bundlePath));

                var bundleSource = new FileSource(bundlePath);
                using var stream = bundleSource.OpenRead();
                var bundleFile = new BundleFileInstance(stream, "", false).file;
                bundleFile.UnpackInfoOnly();
                if (bundleFile.bundleInf6.directoryCount == 0) continue;
                var fileName = bundleFile.bundleInf6.dirInf[0].name;
                var name = $"archive:/{fileName}/{fileName}";
                entries[map[name] = bundlePath] = (name, bundleSource.LastWriteTimeUtc);
            }

            using var target = new FileTarget(objectInfo.FullName);
            ObjectSerializer.Serialize(target.Stream, entries);
            target.Commit();

            return map;

            Dictionary<string, (string, DateTime)>? ReadEntries()
            {
                using var stream = objectInfo.OpenRead();
                try
                {
                    var entries = ObjectSerializer.Deserialize<Dictionary<string, (string, DateTime LastWriteTimeUtc)>>(stream);

                    if (entries.Count < bundlePaths.Length)
                    {
                        return null;
                    }

                    foreach (var bundlePath in bundlePaths)
                    {
                        var bundleSource = new FileSource(bundlePath);

                        if (!entries.TryGetValue(bundlePath, out var entry)
                            || entry.LastWriteTimeUtc != bundleSource.LastWriteTimeUtc)
                        {
                            return null;
                        }
                    }

                    if (entries.Count > bundlePaths.Length)
                    {
                        foreach (var key in entries.Keys.Except(bundlePaths))
                        {
                            entries.Remove(key);
                        }
                    }

                    return entries;
                }
                catch (MessagePackSerializationException)
                {
                }

                return null;
            }
        }
    }
}

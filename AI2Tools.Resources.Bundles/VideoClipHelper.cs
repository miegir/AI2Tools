namespace AI2Tools;

internal static class VideoClipHelper
{
    private static readonly Dictionary<string, string[]> ExtensionMap =
        new(StringComparer.InvariantCultureIgnoreCase)
        {
            [".m4v"] = new[] { ".mov" },
        };

    public static ITrackableStreamSource GetStreamSource(string path)
    {
        foreach (var candidate in GetBasicCandidates(path))
        {
            var zippedPath = candidate + ".zip";
            if (File.Exists(zippedPath + ".001"))
            {
                return new StreamSourceZippedFileMultipart(zippedPath);
            }

            if (File.Exists(zippedPath))
            {
                return new StreamSourceZippedFile(zippedPath);
            }

            if (File.Exists(candidate))
            {
                return new StreamSourcePlainFile(candidate);
            }
        }

        return new StreamSourcePlainFile(path);
    }

    private static IEnumerable<string> GetBasicCandidates(string path)
    {
        yield return path;

        var extension = Path.GetExtension(path);
        if (ExtensionMap.TryGetValue(extension, out var candidates))
        {
            foreach (var candidate in candidates)
            {
                yield return Path.ChangeExtension(path, candidate);
            }
        }
    }
}

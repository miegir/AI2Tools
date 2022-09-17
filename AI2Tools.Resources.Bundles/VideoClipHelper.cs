namespace AI2Tools;

internal static class VideoClipHelper
{
    private static readonly Dictionary<string, string[]> ExtensionMap =
        new(StringComparer.InvariantCultureIgnoreCase)
        {
            [".m4v"] = new[] { ".mov" },
        };

    public static string ConvertPath(string path)
    {
        if (File.Exists(path))
        {
            return path;
        }

        var extension = Path.GetExtension(path);
        if (ExtensionMap.TryGetValue(extension, out var candidates))
        {
            foreach (var candidate in candidates)
            {
                var candidatePath = Path.ChangeExtension(path, candidate);
                if (File.Exists(candidatePath))
                {
                    return candidatePath;
                }
            }
        }

        return path;
    }
}

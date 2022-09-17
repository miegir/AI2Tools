namespace AI2Tools;

internal static class BundleFileExtensions
{
    public static byte[]? ReadResourceFromPath(this BundleFile? bundleFile, string? path, long offset, long length)
    {
        if (string.IsNullOrEmpty(path) || length == 0)
        {
            return null;
        }

        if (path.StartsWith("archive:/"))
        {
            if (bundleFile is null) return null;
            var index = path.LastIndexOf('/');
            var name = path[(index + 1)..];
            return bundleFile.ReadResource(name, (int)offset, (int)length);
        }

        if (!File.Exists(path))
        {
            return null;
        }

        using var stream = File.OpenRead(path);
        stream.Position = offset;
        var data = new byte[length];
        stream.Read(data, 0, data.Length);
        return data;
    }
}

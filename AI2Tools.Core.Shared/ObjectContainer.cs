using System.Collections.Immutable;
using System.IO.Compression;

namespace AI2Tools;

public sealed class ObjectContainer : IDisposable
{
    private readonly ImmutableHashSet<string> directories;
    private readonly ZipArchive archive;

    public ObjectContainer(Stream stream)
    {
        archive = new ZipArchive(stream, ZipArchiveMode.Read);

        var entry = archive.GetEntry(".dirs");
        if (entry != null)
        {
            using var entryStream = entry.Open();
            directories = ObjectSerializer.Deserialize<ImmutableHashSet<string>>(entryStream);
        }
        else
        {
            directories = ImmutableHashSet<string>.Empty;
        }
    }

    public void Dispose()
    {
        archive.Dispose();
    }

    public bool HasDirectory(ObjectPath name) => directories.Contains(name.Name);

    public ObjectEntry? GetEntry(ObjectPath name)
    {
        var entry = archive.GetEntry(name.Name);
        return entry is not null ? new ObjectEntry(entry) : null;
    }
}

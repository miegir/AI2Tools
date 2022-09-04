using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;

namespace AI2Tools;

public sealed class ObjectContainer : IDisposable
{
    private readonly ImmutableHashSet<string> directories;
    private readonly ZipArchive archive;

    public ObjectContainer(Stream stream)
    {
        archive = new ZipArchive(stream, ZipArchiveMode.Read);

        var entry = archive.GetEntry(".dir");
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

    public bool TryGetEntry(ObjectPath name, [NotNullWhen(true)] out ObjectEntry? entry)
    {
        var archiveEntry = archive.GetEntry(name.Name);
        if (archiveEntry != null)
        {
            entry = new ObjectEntry(archiveEntry);
            return true;
        }
        else
        {
            entry = null;
            return false;
        }
    }
}

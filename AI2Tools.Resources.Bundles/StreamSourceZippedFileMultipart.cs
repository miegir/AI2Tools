using System.Collections.Immutable;

namespace AI2Tools;

internal class StreamSourceZippedFileMultipart : ITrackableStreamSource
{
    private readonly ImmutableArray<string> parts;

    public StreamSourceZippedFileMultipart(string path)
    {
        var partsBuilder = ImmutableArray.CreateBuilder<string>();

        for (var i = 1; i <= 999; i++)
        {
            var partInfo = new FileInfo($"{path}.{i:D3}");

            if (!partInfo.Exists)
            {
                break;
            }

            partsBuilder.Add(partInfo.FullName);

            if (LastWriteTimeUtc < partInfo.LastWriteTimeUtc)
            {
                LastWriteTimeUtc = partInfo.LastWriteTimeUtc;
            }
        }

        parts = partsBuilder.ToImmutable();
    }

    public bool Exists => !parts.IsDefaultOrEmpty;

    public DateTime LastWriteTimeUtc { get; }

    public Stream OpenRead() => new ArchiveEntryStream(new ArchiveSourceStream(parts));

    public void Register(SourceChangeTracker tracker)
    {
        foreach (var part in parts)
        {
            tracker.RegisterSource(part);
        }
    }
}

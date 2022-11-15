namespace AI2Tools;

internal class StreamSourceZippedFile : ITrackableStreamSource
{
    private readonly FileInfo info;

    public StreamSourceZippedFile(string path) => info = new FileInfo(path);

    public bool Exists => info.Exists;

    public DateTime LastWriteTimeUtc => info.LastWriteTime;

    public Stream OpenRead() => new ArchiveEntryStream(info.OpenRead());

    public void Register(SourceChangeTracker tracker) => tracker.RegisterSource(info.FullName);
}

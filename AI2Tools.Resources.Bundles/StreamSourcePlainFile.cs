namespace AI2Tools;

internal class StreamSourcePlainFile : ITrackableStreamSource
{
    private readonly FileInfo info;

    public StreamSourcePlainFile(string path) => info = new FileInfo(path);

    public bool Exists => info.Exists;

    public DateTime LastWriteTimeUtc => info.LastWriteTime;

    public Stream OpenRead() => info.OpenRead();

    public void Register(SourceChangeTracker tracker) => tracker.RegisterSource(info.FullName);
}

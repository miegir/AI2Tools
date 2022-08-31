using System.IO.Compression;

namespace AI2Tools;

public class ObjectEntry
{
    private readonly ZipArchiveEntry entry;

    public ObjectEntry(ZipArchiveEntry entry) => this.entry = entry;

    public DateTime LastWriteTimeUtc => entry?.LastWriteTime.UtcDateTime ?? DateTime.MinValue;

    public IObjectSource<T> AsObjectSource<T>() => new ObjectSource<T>(entry);

    private class ObjectSource<T> : IObjectSource<T>
    {
        private readonly ZipArchiveEntry entry;

        public ObjectSource(ZipArchiveEntry entry) => this.entry = entry;

        public T Deserialize()
        {
            using var stream = entry.Open();
            return ObjectSerializer.Deserialize<T>(stream);
        }
    }
}

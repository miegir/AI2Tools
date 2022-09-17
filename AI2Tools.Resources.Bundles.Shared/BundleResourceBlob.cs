namespace AI2Tools;

internal class BundleResourceBlob
{
    private readonly List<byte[]> chunks = new();

    public long Length { get; private set; }

    public void AddChunk(byte[] data)
    {
        chunks.Add(data);
        Length += data.Length;
    }

    public byte[] ToArray()
    {
        if (Length == 0)
        {
            return Array.Empty<byte>();
        }

        if (chunks.Count == 1)
        {
            return chunks[0];
        }

        var target = new byte[Length];
        var p = 0;

        foreach (var chunk in chunks)
        {
            Buffer.BlockCopy(chunk, 0, target, p, chunk.Length);
            p += chunk.Length;
        }

        return target;
    }
}

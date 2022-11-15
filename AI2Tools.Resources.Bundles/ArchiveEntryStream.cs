using System.IO.Compression;

namespace AI2Tools;

internal class ArchiveEntryStream : Stream
{
    private readonly Stream archiveSource;
    private readonly ZipArchive archive;
    private readonly Stream entryStream;

    public ArchiveEntryStream(Stream archiveSource)
    {
        this.archiveSource = archiveSource;
        archive = new ZipArchive(archiveSource, ZipArchiveMode.Read);
        entryStream = archive.Entries.Single().Open();
    }

    public override bool CanRead => entryStream.CanRead;

    public override bool CanSeek => entryStream.CanSeek;

    public override bool CanWrite => entryStream.CanWrite;

    public override long Length => entryStream.Length;

    public override long Position
    {
        get => entryStream.Position;
        set => entryStream.Position = value;
    }

    public override void Flush()
    {
        entryStream.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return entryStream.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return entryStream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        entryStream.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        entryStream.Write(buffer, offset, count);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            entryStream.Dispose();
            archive.Dispose();
            archiveSource.Dispose();
        }
    }
}

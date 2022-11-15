using System.Collections.Immutable;

namespace AI2Tools;

internal class ArchiveSourceStream : Stream
{
    private readonly ImmutableArray<FileStream> streams;
    private readonly ImmutableArray<long> startPositions;
    private int streamIndex;

    public ArchiveSourceStream(ImmutableArray<string> parts)
    {
        var streamsBuilder = ImmutableArray.CreateBuilder<FileStream>(parts.Length);
        var startPositionsBulder = ImmutableArray.CreateBuilder<long>(parts.Length);

        foreach (var part in parts)
        {
            var stream = File.OpenRead(part);
            streamsBuilder.Add(stream);
            startPositionsBulder.Add(Length);
            Length += stream.Length;
        }

        streams = streamsBuilder.MoveToImmutable();
        startPositions = startPositionsBulder.MoveToImmutable();
    }

    public override bool CanRead => true;

    public override bool CanSeek => true;

    public override bool CanWrite => false;

    public override long Length { get; }

    public override long Position
    {
        get
        {
            return streamIndex < 0
                ? 0
                : streamIndex < streams.Length
                ? streams[streamIndex].Position + startPositions[streamIndex]
                : Length;
        }

        set => Seek(value, SeekOrigin.Begin);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            foreach (var stream in streams)
            {
                stream.Dispose();
            }
        }
    }

    public override void Flush() => throw new NotSupportedException();

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (streamIndex >= streams.Length)
        {
            return 0;
        }

        if (streamIndex < 0)
        {
            streamIndex = 0;
            streams[0].Position = 0;
        }

        while (true)
        {
            var read = streams[streamIndex].Read(buffer, offset, count);
            if (read > 0)
            {
                return read;
            }

            if (++streamIndex == streams.Length)
            {
                return 0;
            }

            streams[streamIndex].Position = 0;
        }
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        switch (origin, offset)
        {
            case (SeekOrigin.Begin, <= 0):
                return streams[streamIndex = 0].Seek(0, origin);

            case (SeekOrigin.Begin, > 0):
                for (streamIndex = 0; streamIndex < streams.Length; streamIndex++)
                {
                    var stream = streams[streamIndex];
                    var streamLength = stream.Length;

                    if (offset <= streamLength)
                    {
                        return stream.Seek(offset, origin) + startPositions[streamIndex];
                    }

                    offset -= streamLength;
                }

                return Length;

            case (SeekOrigin.End, >= 0):
                streamIndex = streams.Length;
                if (--streamIndex >= 0)
                {
                    return streams[streamIndex].Seek(0, origin) + startPositions[streamIndex];
                }

                return 0;

            case (SeekOrigin.End, < 0):
                streamIndex = streams.Length;

                while (--streamIndex >= 0)
                {
                    var stream = streams[streamIndex];
                    var streamLength = stream.Length;

                    if (-offset <= streamLength)
                    {
                        return stream.Seek(offset, origin) + startPositions[streamIndex];
                    }

                    offset += streamLength;
                }

                return 0;

            case (SeekOrigin.Current, > 0):
                if (streamIndex < 0)
                {
                    return Seek(offset, SeekOrigin.Begin);
                }

                if (streamIndex >= streams.Length)
                {
                    return Length;
                }

                {
                    var stream = streams[streamIndex];
                    if (stream.Position + offset <= stream.Length)
                    {
                        return stream.Seek(offset, origin) + startPositions[streamIndex];
                    }

                    offset -= stream.Length - stream.Position;
                }

                while (++streamIndex < streams.Length)
                {
                    var stream = streams[streamIndex];
                    var streamLength = stream.Length;

                    if (offset <= streamLength)
                    {
                        return stream.Seek(offset, origin) + startPositions[streamIndex];
                    }

                    offset -= streamLength;
                }

                throw new NotSupportedException();

            case (SeekOrigin.Current, 0):
                return Position;

            case (SeekOrigin.Current, < 0):
                if (streamIndex < 0)
                {
                    return 0;
                }

                if (streamIndex >= streams.Length)
                {
                    return Seek(offset, SeekOrigin.End);
                }

                {
                    var stream = streams[streamIndex];
                    if (stream.Position + offset >= 0)
                    {
                        return stream.Seek(offset, origin) + startPositions[streamIndex];
                    }

                    offset += stream.Position;
                }

                while (--streamIndex >= 0)
                {
                    var stream = streams[streamIndex];
                    var streamLength = stream.Length;

                    if (-offset <= streamLength)
                    {
                        return stream.Seek(offset, origin) + startPositions[streamIndex];
                    }

                    offset += streamLength;
                }

                return 0;

            default:
                throw new NotSupportedException();
        }
    }

    public override void SetLength(long value) => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
}

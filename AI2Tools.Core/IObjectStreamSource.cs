namespace AI2Tools;

public interface IObjectStreamSource : IStreamSource
{
    bool Exists { get; }
    DateTime LastWriteTimeUtc { get; }
}

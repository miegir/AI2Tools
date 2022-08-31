namespace AI2Tools;

public sealed class FileDestination : IStreamSource
{
    private readonly string path;

    public FileDestination(string path)
    {
        this.path = path;
    }

    public FileState FileState => FileState.FromPath(path);
    public FileStream OpenRead() => File.OpenRead(path);

    public static implicit operator FileDestination(string path) => new(path);
}

namespace AI2Tools;

public record FileState(long Length, DateTime LastWriteTimeUtc)
{
	public static FileState FromPath(string path)
	{
        var info = new FileInfo(path);

        return new FileState(
            Length: info.Length,
            LastWriteTimeUtc: info.LastWriteTimeUtc);
    }
}

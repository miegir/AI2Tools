namespace AI2Tools;

public sealed class FileSource : IFileStreamSource, IStreamSource
{
    public record State(long Length, DateTime LastWriteTimeUtc);

    private readonly string sourcePath;
    private readonly string backupPath;

    public FileSource(string path)
    {
        sourcePath = path;
        backupPath = path + ".bak";
    }

    private string ReadPath => File.Exists(backupPath) ? backupPath : sourcePath;
    public string FileName => Path.GetFileName(sourcePath);
    public string FileNameWithoutExtension => Path.GetFileNameWithoutExtension(sourcePath);
    public FileDestination Destination => new(sourcePath);
    public DateTime LastWriteTimeUtc => File.GetLastWriteTimeUtc(ReadPath);

    public FileStream OpenRead() => File.OpenRead(ReadPath);
    public bool CanUnroll() => File.Exists(backupPath);
    public FileTarget CreateTarget() => new(sourcePath, createBackupIfNotExists: true);

    public void Unroll()
    {
        if (File.Exists(backupPath))
        {
            File.Move(backupPath, sourcePath, overwrite: true);
        }
    }

    public static IEnumerable<FileSource> EnumerateFiles(string path, string searchPattern)
    {
        foreach (var filePath in Directory.EnumerateFiles(path, searchPattern))
        {
            yield return new FileSource(filePath);
        }
    }

    public static IEnumerable<FileSource> EnumerateFiles(string path, params string[] searchPatterns)
    {
        foreach (var searchPattern in searchPatterns)
        {
            foreach (var source in EnumerateFiles(path, searchPattern))
            {
                yield return source;
            }
        }
    }

    Stream IStreamSource.OpenRead() => OpenRead();
}

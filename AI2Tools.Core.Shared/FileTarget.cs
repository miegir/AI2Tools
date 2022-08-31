namespace AI2Tools;

public sealed class FileTarget : IDisposable
{
    private readonly string path;
    private readonly bool createBackupIfNotExists;
    private readonly string temporaryPath;
    private bool disposed;

    public FileTarget(string path, bool createBackupIfNotExists = false)
    {
        this.path = path;
        this.createBackupIfNotExists = createBackupIfNotExists;
        new FileInfo(path).Directory?.Create();
        temporaryPath = path + ".tmp";
        Stream = File.Create(temporaryPath);
    }

    public FileStream Stream { get; }
    public string Extension => Path.GetExtension(path);

    public void Commit()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;
        Stream.Dispose();

        if (createBackupIfNotExists)
        {
            var backupPath = path + ".bak";
            if (!File.Exists(backupPath))
            {
                File.Replace(temporaryPath, path, backupPath);
                return;
            }
        }

        File.Move(temporaryPath, path, overwrite: true);
    }

    public void Dispose()
    {
        if (!disposed)
        {
            disposed = true;
            Stream.Dispose();
        }

        if (File.Exists(temporaryPath))
        {
            File.Delete(temporaryPath);
        }
    }
}

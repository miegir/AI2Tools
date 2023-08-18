namespace AI2Tools;

public class FileTargetCollector : IFileTargetCollector
{
    private readonly List<FileTarget> targets = new();

    public void AddTarget(FileTarget target)
    {
        targets.Add(target);
    }

    public void Commit()
    {
        foreach (var target in targets)
        {
            target.Commit();
            target.Dispose();
        }

        targets.Clear();
    }
}

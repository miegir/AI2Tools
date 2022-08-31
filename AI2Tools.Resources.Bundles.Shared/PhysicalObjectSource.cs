namespace AI2Tools;

internal class PhysicalObjectSource<T> : IObjectSource<T>
{
    private readonly string path;

    public PhysicalObjectSource(string path) => this.path = path;

    public T Deserialize()
    {
        using var stream = File.OpenRead(path);
        return ObjectSerializer.Deserialize<T>(stream);
    }
}

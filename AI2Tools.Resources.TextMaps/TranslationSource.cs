namespace AI2Tools;

internal class TranslationSource : IObjectSource<Dictionary<string, TextMapTranslation>>
{
    private readonly string path;

    public TranslationSource(string path) => this.path = path;

    public Dictionary<string, TextMapTranslation> Deserialize()
    {
        using var stream = File.OpenRead(path);
        return TranslationSerializer.Deserialize(stream);
    }
}

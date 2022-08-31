namespace AI2Tools;

internal partial class TextMapManager
{
    public void Export(Stream stream)
    {
        var translations = messages.ToDictionary(
            e => e.Key,
            e => new TextMapTranslation
            {
                Src = e.Value,
                Trx = e.Value,
            });

        TranslationSerializer.Serialize(stream, translations);
    }

    public static void BuildObject(ObjectBuilder builder)
    {
        builder.Build(stream => TranslationSerializer.Deserialize(stream));
    }
}

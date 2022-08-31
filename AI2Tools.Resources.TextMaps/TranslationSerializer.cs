using System.Text.Encodings.Web;
using System.Text.Json;

namespace AI2Tools;

internal static class TranslationSerializer
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        ReadCommentHandling = JsonCommentHandling.Skip,
    };

    public static Dictionary<string, TextMapTranslation> Deserialize(Stream stream)
    {
        return JsonSerializer.Deserialize<Dictionary<string, TextMapTranslation>>(stream, JsonOptions) ?? new();
    }

    public static void Serialize(Stream stream, Dictionary<string, TextMapTranslation> value)
    {
        JsonSerializer.Serialize(stream, value, JsonOptions);
    }
}

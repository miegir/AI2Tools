using System.Text.Encodings.Web;
using System.Text.Json;

namespace AI2Tools;

internal class ScenesManager
{
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web)
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            ReadCommentHandling = JsonCommentHandling.Skip,
        };

    private readonly Dictionary<string, string> textMap = new();

    public bool IsEmpty => textMap.Count == 0;

    public void AddText(string path, string text) => textMap[path] = text;

    public void Export(Stream stream)
    {
        var items = textMap
            .Select(e => new Item { Path = e.Key, Text = e.Value })
            .ToArray();

        JsonSerializer.Serialize(stream, items, JsonOptions);
    }

    private class Item
    {
        public string? Path { get; set; }
        public string? Text { get; set; }
    }
}

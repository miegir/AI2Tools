using System.Collections.Immutable;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace AI2Tools;

internal class GameObjectSource
{
    public record Entry(string Path, string Text);

    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web)
        {
            ReadCommentHandling = JsonCommentHandling.Skip,
        };

    private readonly ILogger logger;
    private readonly string path;

    public GameObjectSource(ILogger logger, string path)
    {
        this.logger = logger;
        this.path = path;

        if (File.Exists(path))
        {
            using var stream = File.OpenRead(path);
            Entries = JsonSerializer.Deserialize<ImmutableArray<Entry>>(stream, JsonOptions);
        }
        else
        {
            Entries = ImmutableArray<Entry>.Empty;
        }
    }

    public bool Exists => !Entries.IsDefaultOrEmpty;

    public ImmutableArray<Entry> Entries { get; }

    public bool IsChanged(SourceChangeTracker sourceChangeTracker) => sourceChangeTracker.IsChanged(path);
}

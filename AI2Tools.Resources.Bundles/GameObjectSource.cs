using System.Collections.Immutable;
using System.Text.Json;

namespace AI2Tools;

internal class GameObjectSource
{
    public record Entry(string Path, string Text, GameObjectData? Data);

    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web)
        {
            ReadCommentHandling = JsonCommentHandling.Skip,
        };

    private readonly string path;

    public GameObjectSource(string path)
    {
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

    public FileDestination Destination => new(path);

    public ImmutableArray<Entry> Entries { get; }

    public void Register(SourceChangeTracker sourceChangeTracker) => sourceChangeTracker.RegisterSource(path);
}

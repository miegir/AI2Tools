using System.Reflection;
using System.Text.Json;

namespace AI2Tools;

public class SourceChangeTracker
{
    private static readonly JsonSerializerOptions JsonOptions = new();

    private readonly Dictionary<string, FileState?> updatedStates = new();
    private readonly Dictionary<string, FileState?> originalStates;
    private readonly FileDestination destination;
    private readonly string statePath;
    private readonly string? callerState;
    private readonly Guid mvid;
    private readonly bool accepted;

    public SourceChangeTracker(FileDestination destination, string statePath, string? callerState = null)
    {
        this.destination = destination;
        this.statePath = statePath;
        this.callerState = callerState;
        mvid = Assembly.GetCallingAssembly().ManifestModule.ModuleVersionId;
        var destinationState = destination.FileState;
        var stateInfo = new FileInfo(statePath);

        if (stateInfo.Exists)
        {
            using var stream = stateInfo.OpenRead();
            try
            {
                var state = JsonSerializer.Deserialize<State>(stream, JsonOptions);

                accepted = state != null
                    && state.Mvid == mvid
                    && state.CallerState == callerState
                    && state.DestinationState == destinationState;

                originalStates = state?.SourceStates ?? new();

                return;
            }
            catch (JsonException)
            {
            }
        }

        originalStates = new();
    }

    public bool HasChanges()
    {
        if (!accepted)
        {
            return true;
        }

        foreach (var (sourcePath, originalState) in originalStates)
        {
            if (GetCurrentState(sourcePath) != originalState)
            {
                return true;
            }
        }

        return false;
    }

    public void RegisterSource(string sourcePath)
    {
        updatedStates[sourcePath] = GetCurrentState(sourcePath);
    }

    public void Commit()
    {
        using var target = new FileTarget(statePath);

        JsonSerializer.Serialize(
            target.Stream, new State
            {
                Mvid = mvid,
                DestinationState = destination.FileState,
                CallerState = callerState,
                SourceStates = updatedStates,
            },
            JsonOptions);

        target.Commit();
    }

    private static FileState? GetCurrentState(string path) => File.Exists(path) ? FileState.FromPath(path) : null;

    private class State
    {
        public Guid Mvid { get; set; }
        public FileState? DestinationState { get; set; }
        public string? CallerState { get; set; }
        public Dictionary<string, FileState?>? SourceStates { get; set; }
    }
}

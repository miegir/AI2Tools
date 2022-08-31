using System.Reflection;
using System.Text.Json;

namespace AI2Tools;

public class SourceChangeTracker
{
    private static readonly JsonSerializerOptions JsonOptions = new();

    private readonly Dictionary<string, bool> isChangedCache = new();
    private readonly Dictionary<string, SourceState> states;
    private readonly FileDestination destination;
    private readonly string statePath;
    private readonly string? callerState;
    private bool accepted;

    public SourceChangeTracker(FileDestination destination, string statePath, string? callerState = null)
    {
        this.destination = destination;
        this.statePath = statePath;
        this.callerState = callerState;
        var destinationState = destination.FileState;
        var stateInfo = new FileInfo(statePath);

        if (stateInfo.Exists)
        {
            using var stream = stateInfo.OpenRead();
            try
            {
                var state = JsonSerializer.Deserialize<State>(stream, JsonOptions);

                accepted = state != null
                    && state.CallerState == callerState
                    && state.DestinationState == destinationState;

                states = state?.SourceStates ?? new();

                foreach (var (key, sourceState) in states)
                {
                    if (sourceState is null)
                    {
                        states.Remove(key);
                    }
                }

                foreach (var (key, sourceState) in states)
                {
                    if (sourceState.FileState is not null &&
                        sourceState.FileState.LastWriteTimeUtc > destinationState.LastWriteTimeUtc)
                    {
                        states.Remove(key);
                    }
                }

                return;
            }
            catch (JsonException)
            {
            }
        }

        states = new();
    }

    public bool IsChanged(string sourcePath)
    {
        var assembly = Assembly.GetCallingAssembly();

        if (!isChangedCache.TryGetValue(sourcePath, out var changed))
        {
            isChangedCache[sourcePath] = changed = IsChangedCore();
        }

        return changed;

        bool IsChangedCore()
        {
            sourcePath = Path.GetFullPath(sourcePath);

            var fileState = File.Exists(sourcePath)
                ? FileState.FromPath(sourcePath)
                : null;

            var currentState = new SourceState(
                Mvid: assembly.ManifestModule.ModuleVersionId,
                FileState: fileState);

            if (accepted
                && states.TryGetValue(sourcePath, out var state)
                && state == currentState)
            {
                return false;
            }

            accepted = false;
            states[sourcePath] = currentState;
            return true;
        }
    }

    public void Commit()
    {
        using var target = new FileTarget(statePath);

        JsonSerializer.Serialize(
            target.Stream, new State
            {
                DestinationState = destination.FileState,
                CallerState = callerState,
                SourceStates = states,
            },
            JsonOptions);

        target.Commit();
    }

    private class State
    {
        public FileState? DestinationState { get; set; }
        public string? CallerState { get; set; }
        public Dictionary<string, SourceState>? SourceStates { get; set; }
    }

    private record SourceState(Guid Mvid, FileState? FileState);
}

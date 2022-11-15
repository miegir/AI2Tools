namespace AI2Tools;

internal interface ITrackableStreamSource : IObjectStreamSource
{
    void Register(SourceChangeTracker tracker);
}

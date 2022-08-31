using MessagePack;

namespace AI2Tools;

public class ObjectBuilder
{
    private readonly FileDestination source;
    private readonly string objectPath;
    private readonly bool forceObjects;

    public ObjectBuilder(FileDestination source, string objectPath, bool forceObjects)
    {
        this.source = source;
        this.objectPath = objectPath;
        this.forceObjects = forceObjects;
    }

    public T Build<T>(Func<Stream, T> factory)
    {
        var statePath = objectPath + ".state";
        var sourceState = source.FileState;

        if (!forceObjects && !IsChanged())
        {
            try
            {
                using var existingStream = File.OpenRead(objectPath);
                return ObjectSerializer.Deserialize<T>(existingStream);
            }
            catch (MessagePackSerializationException)
            {
            }
        }

        using var stream = source.OpenRead();
        var result = factory(stream);
        using var target = new FileTarget(objectPath);
        ObjectSerializer.Serialize(target.Stream, result);
        target.Commit();
        return result;

        bool IsChanged()
        {
            if (File.Exists(statePath))
            {
                using var stream = File.OpenRead(statePath);
                using var reader = new BinaryReader(stream);

                if (reader.ReadInt64() == sourceState.Length &&
                    reader.ReadInt64() == sourceState.LastWriteTimeUtc.Ticks)
                {
                    var objectInfo = new FileInfo(objectPath);
                    return !objectInfo.Exists || objectInfo.LastWriteTimeUtc < sourceState.LastWriteTimeUtc;
                }
            }

            using var target = new FileTarget(statePath);
            using var writer = new BinaryWriter(target.Stream);

            writer.Write(sourceState.Length);
            writer.Write(sourceState.LastWriteTimeUtc.Ticks);

            target.Commit();

            return true;
        }
    }
}

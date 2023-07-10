using MessagePack;

namespace AI2Tools;

public static class ObjectSerializer
{
    private static readonly MessagePackSerializerOptions MessageOptions =
        MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4BlockArray);

    public static void Serialize<T>(Stream stream, T value)
    {
        MessagePackSerializer.Serialize(stream, value, MessageOptions);
    }

    public static Stream SerializeToStream<T>(T value)
    {
        return new MemoryStream(MessagePackSerializer.Serialize(value, MessageOptions), writable: false);
    }

    public static T Deserialize<T>(Stream stream)
    {
        return MessagePackSerializer.Deserialize<T>(stream, MessageOptions);
    }
}

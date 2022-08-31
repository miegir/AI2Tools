using MessagePack;

namespace AI2Tools;

public static class ObjectSerializer
{
    private static readonly MessagePackSerializerOptions MessageOptions =
        MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4BlockArray);

    public static void Serialize<T>(FileStream stream, T value)
    {
        MessagePackSerializer.Serialize(stream, value, MessageOptions);
    }

    public static T Deserialize<T>(Stream stream)
    {
        return MessagePackSerializer.Deserialize<T>(stream, MessageOptions);
    }
}

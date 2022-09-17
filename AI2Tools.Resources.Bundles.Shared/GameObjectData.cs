using MessagePack;

namespace AI2Tools;

[MessagePackObject]
public record GameObjectData(
    [property: Key(0)] bool? Active);

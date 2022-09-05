using MessagePack;

namespace AI2Tools;

[MessagePackObject]
public class Il2CppMetadataTranslation
{
    [Key(0)]
    public string? Src { get; set; }
    [Key(1)]
    public string? Trx { get; set; }
}

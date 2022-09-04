using AssetsTools.NET;
using MessagePack;

namespace AI2Tools;

[MessagePackObject]
public class TextMeshProUGUIData : IWriteTo
{
    [Key(0)] public string m_text;

    public TextMeshProUGUIData(string text) => m_text = text;

    public void WriteTo(AssetTypeValueField baseField)
    {
        baseField["m_text"].Write(m_text);
    }
}

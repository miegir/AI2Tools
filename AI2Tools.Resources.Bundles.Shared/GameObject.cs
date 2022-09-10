using System.Diagnostics;
using AssetsTools.NET;
using AssetsTools.NET.Extra;

namespace AI2Tools;

[DebuggerDisplay("{Name}")]
internal class GameObject
{
    public GameObject(AssetFileInfoEx asset, AssetTypeValueField field)
    {
        Asset = asset;
        Field = field;

        var nameField = field["m_Name"];
        if (!nameField.IsDummy())
        {
            Name = nameField.GetValue().AsString();
        }
        else
        {
            Name = string.Empty;
        }
    }

    public AssetFileInfoEx Asset { get; }
    public AssetTypeValueField Field { get; }
    public string Name { get; }
    public GameObjectCollection Children { get; } = new();
    public GameObject? Parent { get; set; }
    public AssetClassID TypeId => (AssetClassID)Asset.curFileType;

    public string GetPath()
    {
        var segments = new Stack<string>();

        for (var obj = this; obj != null; obj = obj.Parent)
        {
            segments.Push(obj.Name);
        }

        return string.Join("/", segments);
    }
}

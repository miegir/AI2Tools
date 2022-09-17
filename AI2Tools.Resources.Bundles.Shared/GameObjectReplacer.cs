using AssetsTools.NET;
using AssetsTools.NET.Extra;

namespace AI2Tools;

internal class GameObjectReplacer : SerializingAssetReplacer
{
    private readonly IObjectSource<GameObjectData> source;

    public GameObjectReplacer(
        AssetsManager manager,
        AssetsFileInstance assetsFile,
        AssetFileInfoEx asset,
        IObjectSource<GameObjectData> source) : base(manager, assetsFile, asset)
    {
        this.source = source;
    }

    protected override void Modify(AssetTypeValueField baseField)
    {
        var data = source.Deserialize();
        if (data.Active is bool active)
        {
            baseField["m_IsActive"].GetValue().Set(active);
        }
    }
}

using AssetsTools.NET;
using AssetsTools.NET.Extra;

namespace AI2Tools;

internal class FontAssetReplacer : SerializingAssetReplacer
{
    private readonly IObjectSource<FontAssetData> source;

    public FontAssetReplacer(
        AssetsManager manager,
        AssetsFileInstance assetsFile,
        AssetFileInfoEx asset,
        IObjectSource<FontAssetData> source) : base(manager, assetsFile, asset)
    {
        this.source = source;
    }

    protected override void Modify(AssetTypeValueField baseField)
    {
        source.Deserialize().WriteTo(baseField);
    }
}

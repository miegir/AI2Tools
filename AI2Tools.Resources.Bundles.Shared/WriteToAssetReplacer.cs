using AssetsTools.NET;
using AssetsTools.NET.Extra;

namespace AI2Tools;

internal class WriteToAssetReplacer<TData> : SerializingAssetReplacer where TData : IWriteTo
{
    private readonly IObjectSource<TData> source;

    public WriteToAssetReplacer(
        AssetsManager manager,
        AssetsFileInstance assetsFile,
        AssetFileInfoEx asset,
        IObjectSource<TData> source) : base(manager, assetsFile, asset)
    {
        this.source = source;
    }

    protected override void Modify(AssetTypeValueField baseField)
    {
        source.Deserialize().WriteTo(baseField);
    }
}

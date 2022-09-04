using AssetsTools.NET;
using AssetsTools.NET.Extra;

namespace AI2Tools;

internal class TextAssetReplacer : SerializingAssetReplacer
{
    private readonly IObjectSource<string> source;

    public TextAssetReplacer(
        AssetsManager manager,
        AssetsFileInstance assetsFile,
        AssetFileInfoEx asset,
        IObjectSource<string> source) : base(manager, assetsFile, asset)
    {
        this.source = source;
    }

    protected override void Modify(AssetTypeValueField baseField)
    {
        var script = source.Deserialize();
        baseField["m_Script"].GetValue().Set(TextCompressor.Compress(script));
    }
}

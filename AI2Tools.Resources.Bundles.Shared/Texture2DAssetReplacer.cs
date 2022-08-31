using AssetsTools.NET;
using AssetsTools.NET.Extra;

namespace AI2Tools;

internal class Texture2DAssetReplacer : SerializingAssetReplacer
{
    private readonly IObjectSource<Texture2DData> source;

    public Texture2DAssetReplacer(
        AssetsManager manager,
        AssetsFileInstance assetsFile,
        AssetFileInfoEx asset,
        IObjectSource<Texture2DData> source) : base(manager, assetsFile, asset)
    {
        this.source = source;
    }

    protected override void Modify(AssetTypeValueField baseField)
    {
        var textureData = source.Deserialize();
        var textureFile = TextureFile.ReadTextureFile(baseField);
        textureFile.SetTextureDataRaw(textureData.EncodedData, textureData.Width, textureData.Height);
        textureFile.m_TextureFormat = (int)textureData.Format;
        textureFile.m_MipCount = textureData.MipCount;
        textureFile.WriteTo(baseField);
    }
}

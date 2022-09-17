using AssetsTools.NET;
using AssetsTools.NET.Extra;
using Microsoft.Extensions.Logging;

namespace AI2Tools;

internal class BundleFileSource : IDisposable
{
    private readonly BundleFile? bundleFile;
    private readonly string path;

    public BundleFileSource(ILogger logger, string path)
    {
        if (File.Exists(path))
        {
            bundleFile = new BundleFile(logger, File.OpenRead(path));
        }

        this.path = path;
    }

    public bool Exists => bundleFile is not null;

    public FileDestination Destination => new(path);

    public void Dispose() => bundleFile?.Dispose();

    public void Register(SourceChangeTracker sourceChangeTracker) => sourceChangeTracker.RegisterSource(path);

    public IObjectSource<Texture2DData>? FindTexture2D(string name, string defaultExtension)
    {
        if (bundleFile is null)
        {
            return null;
        }

        var asset = bundleFile.FindAssets(AssetClassID.Texture2D, name, defaultExtension).FirstOrDefault();
        if (asset is null)
        {
            return null;
        }

        var baseField = bundleFile.GetBaseField(asset);
        var textureFile = TextureFile.ReadTextureFile(baseField);
        var textureData = GetTextureData(textureFile);

        if (textureData is null)
        {
            return null;
        }

        return ObjectSource.Create(() => new Texture2DData(
            Format: (TextureFormat)textureFile.m_TextureFormat,
            Width: textureFile.m_Width,
            Height: textureFile.m_Height,
            MipCount: textureFile.m_MipCount,
            EncodedData: textureData));
    }

    public IObjectSource<T>? FindMonoBehavior<T>(
        BundleResolver bundleResolver,
        string name,
        ScriptName scriptName,
        string defaultExtension,
        Func<MonoBehaviorContext, T> factory,
        Func<T, bool>? validator = null)
    {
        if (bundleFile is null)
        {
            return null;
        }

        foreach (var asset in bundleFile.FindAssets(AssetClassID.MonoBehaviour, name, defaultExtension))
        {
            var baseField = bundleFile.GetBaseField(asset);
            var assetScriptName = bundleFile.ReadScriptName(bundleResolver, baseField);
            if (assetScriptName != scriptName)
            {
                continue;
            }

            var context = new MonoBehaviorContext(asset, scriptName, baseField);

            if (validator == null)
            {
                return ObjectSource.Create(() => factory(context));
            }

            var value = factory(context);
            if (!validator(value))
            {
                continue;
            }

            return ObjectSource.Create(() => value);
        }

        return null;
    }

    private byte[]? GetTextureData(TextureFile textureFile)
    {
        if (textureFile.pictureData is { Length: > 0 })
        {
            return textureFile.pictureData;
        }

        return bundleFile.ReadResourceFromPath(
            textureFile.m_StreamData.path,
            (long)textureFile.m_StreamData.offset,
            textureFile.m_StreamData.size);
    }
}

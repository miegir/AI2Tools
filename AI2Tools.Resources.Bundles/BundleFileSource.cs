using AssetsTools.NET;
using AssetsTools.NET.Extra;
using Microsoft.Extensions.Logging;

namespace AI2Tools;

internal class BundleFileSource : IDisposable
{
    private readonly BundleFile? file;
    private readonly string path;

    public BundleFileSource(ILogger logger, string path)
    {
        if (File.Exists(path))
        {
            file = new BundleFile(logger, File.OpenRead(path));
        }

        this.path = path;
    }

    public bool Exists => file is not null;

    public FileDestination Destination => new(path);

    public void Dispose() => file?.Dispose();

    public bool IsChanged(SourceChangeTracker sourceChangeTracker) => sourceChangeTracker.IsChanged(path);

    public IObjectSource<Texture2DData>? FindTexture2D(string name, string defaultExtension)
    {
        if (file is null)
        {
            return null;
        }

        var asset = file.FindAssets(AssetClassID.Texture2D, name, defaultExtension).FirstOrDefault();
        if (asset is null)
        {
            return null;
        }

        var baseField = file.GetBaseField(asset);
        var textureFile = TextureFile.ReadTextureFile(baseField);
        var textureData = GetTextureData(textureFile);

        if (textureData is null)
        {
            return null;
        }

        return DelegateObjectSource.Create(() => new Texture2DData(
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
        if (file is null)
        {
            return null;
        }

        foreach (var asset in file.FindAssets(AssetClassID.MonoBehaviour, name, defaultExtension))
        {
            var baseField = file.GetBaseField(asset);
            var assetScriptName = file.ReadScriptName(bundleResolver, baseField);
            if (assetScriptName != scriptName)
            {
                continue;
            }

            var context = new MonoBehaviorContext(asset, scriptName, baseField);

            if (validator == null)
            {
                return DelegateObjectSource.Create(() => factory(context));
            }

            var value = factory(context);
            if (!validator(value))
            {
                continue;
            }

            return DelegateObjectSource.Create(() => value);
        }

        return null;
    }

    private byte[]? GetTextureData(TextureFile textureFile)
    {
        if (textureFile.pictureData is { Length: > 0 })
        {
            return textureFile.pictureData;
        }

        if (!string.IsNullOrEmpty(textureFile.m_StreamData.path)
            && textureFile.m_StreamData.size > 0)
        {
            var path = textureFile.m_StreamData.path;
            if (path.StartsWith("archive:/"))
            {
                if (file is null) return null;
                var index = path.LastIndexOf('/');
                var name = path[(index + 1)..];
                return file.ReadResource(
                    name,
                    (int)textureFile.m_StreamData.offset,
                    (int)textureFile.m_StreamData.size);
            }

            if (!File.Exists(path))
            {
                return null;
            }

            using var stream = File.OpenRead(path);
            stream.Position = (long)textureFile.m_StreamData.offset;
            var data = new byte[textureFile.m_StreamData.size];
            stream.Read(data, 0, data.Length);
            return data;
        }

        return null;
    }
}

using AssetsTools.NET;
using AssetsTools.NET.Extra;
using Microsoft.Extensions.Logging;

namespace AI2Tools;

internal partial class BundleManager : BundleFile
{
    private const string DefaultTextExtension = ".txt";
    private const string DefaultTexture2DExtension = ".png";
    private const string DefaultAssetExtension = ".asset";

    private readonly FileSource source;

    public BundleManager(ILogger logger, FileSource source, int index = 0) : base(logger, source.OpenRead(), index)
    {
        this.source = source;
    }

    public void Unpack(UnpackArguments arguments, ObjectPath root)
    {
        var assetReplacers = new List<AssetsReplacer>();

        Enumerate()
            .Scoped(logger, "asset")
            .Run();

        var compression = arguments.BundleCompression switch
        {
            BundleCompressionType.Default => GetCompressionType(source),
            BundleCompressionType.LZ4 => AssetBundleCompressionType.LZ4,
            BundleCompressionType.LZMA => AssetBundleCompressionType.LZMA,
            _ => AssetBundleCompressionType.NONE,
        };

        var writer = new BundleWriter(logger, bundleFileInstance.file, source);

        writer.Replacers.Add(new BundleReplacerFromAssets(
            oldName: assetsFileInstance.name,
            newName: null,
            assetsFile: assetsFileInstance.file,
            assetReplacers: assetReplacers));

        writer.Write(compression);

        IEnumerable<Action> Enumerate()
        {
            foreach (var asset in GetAssets(AssetClassID.Texture2D))
            {
                var name = ReadAssetName(asset, DefaultTexture2DExtension);
                var path = root.Append(name + ".pak");
                var entry = arguments.Container.GetEntry(path);

                if (entry is null)
                {
                    continue;
                }

                yield return () =>
                {
                    logger.LogInformation("importing texture {name}...", name);
                    assetReplacers.Add(CreateReplacer(asset, entry.AsObjectSource<Texture2DData>()));
                };
            }

            foreach (var asset in GetAssets(AssetClassID.TextAsset))
            {
                var name = ReadAssetName(asset, DefaultTextExtension);
                var path = root.Append(name + ".pak");
                var entry = arguments.Container.GetEntry(path);

                if (entry is null)
                {
                    continue;
                }

                yield return () =>
                {
                    logger.LogInformation("importing text {name}...", name);
                    assetReplacers.Add(CreateReplacer(asset, entry.AsObjectSource<string>()));
                };
            }

            foreach (var asset in GetAssets(AssetClassID.MonoBehaviour))
            {
                var name = ReadAssetName(asset, DefaultAssetExtension);
                var path = root.Append(name + ".f.pak");
                var entry = arguments.Container.GetEntry(path);

                if (entry is null)
                {
                    continue;
                }

                yield return () =>
                {
                    logger.LogInformation("importing font {name}...", name);
                    assetReplacers.Add(CreateReplacer(asset, entry.AsObjectSource<FontAssetData>()));
                };
            }
        }
    }

    private AssetsReplacer CreateReplacer(AssetFileInfoEx asset, IObjectSource<Texture2DData> source)
    {
        return new Texture2DAssetReplacer(assetsManager, assetsFileInstance, asset, source);
    }

    private AssetsReplacer CreateReplacer(AssetFileInfoEx asset, IObjectSource<string> source)
    {
        return new TextAssetReplacer(assetsManager, assetsFileInstance, asset, source);
    }

    private AssetsReplacer CreateReplacer(AssetFileInfoEx asset, IObjectSource<FontAssetData> source)
    {
        return new FontAssetReplacer(assetsManager, assetsFileInstance, asset, source);
    }

    private static AssetBundleCompressionType GetCompressionType(IStreamSource source)
    {
        using var stream = source.OpenRead();

        var targetAssetsManager = new AssetsManager();
        var targetBundle = targetAssetsManager.LoadBundleFile(stream, unpackIfPacked: false);

        return (targetBundle.file.bundleHeader6?.GetCompressionType()) switch
        {
            1 => AssetBundleCompressionType.LZMA,
            2 or 3 => AssetBundleCompressionType.LZ4,
            _ => AssetBundleCompressionType.NONE,
        };
    }
}

using AssetsTools.NET;
using AssetsTools.NET.Extra;
using Microsoft.Extensions.Logging;

namespace AI2Tools;

internal partial class BundleManager
{
    private const string DefaultTextExtension = ".txt";
    private const string DefaultTexture2DExtension = ".png";
    private const string DefaultAssetExtension = ".asset";
    private readonly ILogger logger;
    private readonly FileSource source;

    public BundleManager(ILogger logger, FileSource source)
    {
        this.logger = logger;
        this.source = source;
    }

    public void Unpack(UnpackArguments arguments, ObjectPath root)
    {
        using var bundleFile = new BundleFile(logger, source.OpenRead());
        var resourceCollector = bundleFile.CreateResourceCollector();

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

        resourceCollector.Write(source, compression);

        IEnumerable<Action> Enumerate()
        {
            foreach (var asset in bundleFile.GetAssets(AssetClassID.GameObject))
            {
                var gameObject = bundleFile.ResolveGameObject(asset);
                if (gameObject == null)
                {
                    continue;
                }

                var name = gameObject.GetPath();
                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }

                if (arguments.Container.TryGetEntry(root.Append(name + ".god"), out var entry))
                {
                    yield return () =>
                    {
                        logger.LogInformation("importing game object {name}...", name);
                        resourceCollector.AddReplacer(gameObject.Asset, entry.AsObjectSource<GameObjectData>());
                    };
                }
            }

            foreach (var asset in bundleFile.GetAssets(AssetClassID.Texture2D))
            {
                var name = bundleFile.ReadAssetName(asset, DefaultTexture2DExtension);
                if (arguments.Container.TryGetEntry(root.Append(name + ".pak"), out var entry))
                {
                    yield return () =>
                    {
                        logger.LogInformation("importing texture {name}...", name);
                        resourceCollector.AddReplacer(asset, entry.AsObjectSource<Texture2DData>());
                    };
                }
            }

            foreach (var asset in bundleFile.GetAssets(AssetClassID.TextAsset))
            {
                var name = bundleFile.ReadAssetName(asset, DefaultTextExtension);
                if (arguments.Container.TryGetEntry(root.Append(name + ".pak"), out var entry))
                {
                    yield return () =>
                    {
                        logger.LogInformation("importing text {name}...", name);
                        resourceCollector.AddReplacer(asset, entry.AsObjectSource<string>());
                    };
                }
            }

            foreach (var asset in bundleFile.GetAssets(AssetClassID.MonoBehaviour))
            {
                var name = bundleFile.ReadAssetName(asset, DefaultAssetExtension);

                if (arguments.Container.TryGetEntry(root.Append(name + ".fnt"), out var fntEntry))
                {
                    yield return () =>
                    {
                        logger.LogInformation("importing font {name}...", name);
                        resourceCollector.AddReplacer(asset, fntEntry.AsObjectSource<FontAssetData>());
                    };
                }

                if (arguments.Container.TryGetEntry(root.Append(name + ".tmprougui.pak"), out var tmpEntry))
                {
                    yield return () =>
                    {
                        logger.LogInformation("importing text mesh pro {name}...", name);
                        resourceCollector.AddReplacer(asset, tmpEntry.AsObjectSource<TextMeshProUGUIData>());
                    };
                }
            }

            foreach (var asset in bundleFile.GetAssets(AssetClassID.VideoClip))
            {
                var baseField = bundleFile.GetBaseField(asset);
                var fileData = new VideoClipFileData(baseField);
                if (!fileData.IsValid) continue;
                var name = fileData.OriginalPath;
                if (arguments.Container.TryGetEntry(root.Append(name), out var entry))
                {
                    yield return () =>
                    {
                        logger.LogInformation("importing video clip {name}...", name);
                        resourceCollector.AddResourceReplacer(
                            asset,
                            fileData.ResourceName,
                            entry.AsStreamSource(),
                            fileData.Write);
                    };
                }
            }
        }
    }

    private static AssetBundleCompressionType GetCompressionType(IFileStreamSource source)
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

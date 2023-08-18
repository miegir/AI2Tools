using AssetsTools.NET;
using AssetsTools.NET.Extra;
using Microsoft.Extensions.Logging;

namespace AI2Tools;

internal class BundleResourceCollector
{
    private readonly ILogger logger;
    private readonly AssetsManager assetsManager;
    private readonly BundleFileInstance bundleFileInstance;
    private readonly AssetsFileInstance assetsFileInstance;
    private readonly List<AssetsReplacer> assetReplacers = new();
    private readonly Dictionary<string, BundleResourceBlob> resourceBlobs = new();

    public BundleResourceCollector(
        ILogger logger,
        AssetsManager assetsManager,
        BundleFileInstance bundleFileInstance,
        AssetsFileInstance assetsFileInstance)
    {
        this.logger = logger;
        this.assetsManager = assetsManager;
        this.bundleFileInstance = bundleFileInstance;
        this.assetsFileInstance = assetsFileInstance;
    }

    public void AddReplacer(AssetFileInfoEx asset, Action<AssetTypeValueField> modify)
    {
        assetReplacers.Add(
            new LambdaAssetReplacer(
                assetsManager, assetsFileInstance, asset, modify));
    }

    public void AddReplacer(AssetFileInfoEx asset, IObjectSource<Texture2DData> source)
    {
        assetReplacers.Add(
            new Texture2DAssetReplacer(
                assetsManager, assetsFileInstance, asset, source));
    }

    public void AddReplacer(AssetFileInfoEx asset, IObjectSource<string> source)
    {
        assetReplacers.Add(
            new TextAssetReplacer(
                assetsManager, assetsFileInstance, asset, source));
    }

    public void AddReplacer(AssetFileInfoEx asset, IObjectSource<GameObjectData> source)
    {
        assetReplacers.Add(
            new GameObjectReplacer(
                assetsManager, assetsFileInstance, asset, source));
    }

    public void AddReplacer<TData>(AssetFileInfoEx asset, IObjectSource<TData> source) where TData : IWriteTo
    {
        assetReplacers.Add(
            new WriteToAssetReplacer<TData>(
                assetsManager, assetsFileInstance, asset, source));
    }

    public void AddReplacer<TData>(AssetFileInfoEx asset, TData data) where TData : IWriteTo
    {
        AddReplacer(asset, ObjectSource.Create(() => data));
    }

    public void AddResourceReplacer(
        AssetFileInfoEx asset,
        string resourceName,
        IStreamSource source,
        Action<ResourceReplacerContext> replacerAction)
    {
        if (!resourceBlobs.TryGetValue(resourceName, out var blob))
        {
            resourceBlobs[resourceName] = blob = new();

            var data = BundleHelper.LoadAssetDataFromBundle(bundleFileInstance.file, resourceName);
            if (data == null)
            {
                logger.LogWarning("Cannot find resource named '{resourceName}'.", resourceName);
            }
            else
            {
                blob.AddChunk(data);
            }
        }

        using var sourceStream = source.OpenRead();
        using var targetStream = new MemoryStream();

        sourceStream.CopyTo(targetStream);

        var offset = blob.Length;
        var size = targetStream.Length;

        blob.AddChunk(targetStream.ToArray());

        assetReplacers.Add(new LambdaAssetReplacer(assetsManager, assetsFileInstance, asset, (baseField) =>
        {
            replacerAction(new ResourceReplacerContext(baseField, offset, size));
        }));
    }

    public void Write(FileSource source, IFileTargetCollector fileTargetCollector, AssetBundleCompressionType compression)
    {
        if (assetReplacers.Count == 0)
        {
            if (source.CanUnroll())
            {
                logger.LogInformation("unrolling bundle...");

                source.Unroll();
            }

            return;
        }

        var writer = new BundleWriter(logger, fileTargetCollector, bundleFileInstance.file, source);

        writer.Replacers.Add(new BundleReplacerFromAssets(
            oldName: assetsFileInstance.name,
            newName: null,
            assetsFile: assetsFileInstance.file,
            assetReplacers: assetReplacers));

        foreach (var (key, value) in resourceBlobs)
        {
            writer.Replacers.Add(new BundleReplacerFromMemory(
                oldName: key,
                newName: null,
                hasSerializedData: false,
                buffer: value.ToArray(),
                size: value.Length));
        }

        writer.Write(compression);
    }
}

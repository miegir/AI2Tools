using AssetsTools.NET;
using AssetsTools.NET.Extra;
using Microsoft.Extensions.Logging;

namespace AI2Tools;

internal partial class BundleFile : IDisposable
{
    private readonly Dictionary<long, string> paths = new();
    protected readonly ILogger logger;
    protected readonly AssetsManager assetsManager;
    protected readonly BundleFileInstance bundleFileInstance;
    protected readonly AssetsFileInstance assetsFileInstance;
    
    public BundleFile(ILogger logger, FileStream stream, int index = 0)
	{
        this.logger = logger;
        assetsManager = new AssetsManager();
        bundleFileInstance = assetsManager.LoadBundleFile(stream);
        assetsFileInstance = assetsManager.LoadAssetsFileFromBundle(bundleFileInstance, index);
        LoadPaths();
        Initialize();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            assetsManager.UnloadAll();
        }
    }

    public IEnumerable<AssetFileInfoEx> GetAssets(AssetClassID typeId)
    {
        return assetsFileInstance.table.GetAssetsOfType((int)typeId);
    }

    public AssetTypeValueField GetBaseField(AssetFileInfoEx asset)
    {
        return assetsManager.GetTypeInstance(assetsFileInstance.file, asset).GetBaseField();
    }

    public string ReadAssetName(AssetFileInfoEx asset, string defaultExtension)
    {
        if (paths.TryGetValue(asset.index, out var name))
        {
            return name;
        }
        if (asset.ReadName(assetsFileInstance.file, out name))
        {
            return $"{name}{defaultExtension}";
        }
        else
        {
            return $"{asset.index}{defaultExtension}";
        }
    }

    private void LoadPaths()
    {
        foreach (var inf in GetAssets(AssetClassID.AssetBundle))
        {
            var baseField = GetBaseField(inf);
            foreach (var data in baseField["m_Container"]["Array"].GetChildrenList())
            {
                var path = data[0].GetValue().AsString();
                var pathId = data[1]["asset"]["m_PathID"].GetValue().AsInt64();
                paths[pathId] = path;
            }
        }
    }

    partial void Initialize();
}

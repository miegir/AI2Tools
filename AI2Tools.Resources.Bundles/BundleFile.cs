using AssetsTools.NET;
using AssetsTools.NET.Extra;

namespace AI2Tools;

internal partial class BundleFile
{
    private ILookup<string, long> pathLookup = default!;

    partial void Initialize()
    {
        pathLookup = paths.ToLookup(e => e.Value, e => e.Key, StringComparer.OrdinalIgnoreCase);
    }

    public IEnumerable<AssetFileInfoEx> FindAssets(AssetClassID typeId, string name, string defaultExtension)
    {
        foreach (var pathId in pathLookup[name])
        {
            var asset = assetsFileInstance.table.GetAssetInfo(pathId);
            if (asset is null)
            {
                continue;
            }

            if (asset.curFileType != (int)typeId)
            {
                continue;
            }

            var assetName = ReadAssetName(asset, defaultExtension);
            if (!string.Equals(name, assetName, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            yield return asset;
        }
    }

    public ScriptName? ReadScriptName(BundleResolver resolver, AssetTypeValueField baseField)
    {
        var atvf = baseField["m_Script"];
        if (atvf.IsDummy())
        {
            return null;
        }

        var fileId = atvf["m_FileID"].GetValue().AsInt();
        var pathId = atvf["m_PathID"].GetValue().AsInt64();

        if (fileId == 0)
        {
            return ReadScriptName(assetsFileInstance);
        }
        else
        {
            fileId--;

            var dependency = assetsFileInstance.GetDependency(assetsManager, fileId);
            if (dependency != null)
            {
                return ReadScriptName(dependency);
            }

            var path = assetsFileInstance.file.dependencies.dependencies[fileId].assetPath;

            var dependentFileInstance = Resolve(path);
            if (dependentFileInstance == null)
            {
                return null;
            }

            return ReadScriptName(dependentFileInstance);
        }

        ScriptName? ReadScriptName(AssetsFileInstance assetsFileInstance)
        {
            var asset = assetsFileInstance.table.GetAssetInfo(pathId);
            if (asset == null) return null;
            var baseField = assetsManager.GetTypeInstance(assetsFileInstance, asset).GetBaseField();
            return ScriptName.Read(baseField);
        }

        AssetsFileInstance? Resolve(string path)
        {
            using var bundleStream = resolver.OpenBundle(path);
            if (bundleStream == null) return null;
            var bundleFileInstance = assetsManager.LoadBundleFile(bundleStream);
            return assetsManager.LoadAssetsFileFromBundle(bundleFileInstance, 0);
        }
    }

    public byte[]? ReadResource(string name, int offset, int length)
    {
        var data = BundleHelper.LoadAssetDataFromBundle(bundleFileInstance.file, name);
        if (data == null) return null;
        return data.AsSpan().Slice(offset, length).ToArray();
    }
}

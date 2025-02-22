﻿using AssetsTools.NET;
using AssetsTools.NET.Extra;
using Microsoft.Extensions.Logging;

namespace AI2Tools;

internal partial class BundleFile : IDisposable
{
    private readonly Dictionary<long, string> paths = new();
    private readonly Dictionary<long, GameObject> gameObjectMap = new();
    private readonly GameObjectCollection gameObjects = new();
    private readonly FileTargetCollector fileTargetCollector = new();
    private readonly ILogger logger;
    private readonly AssetsManager assetsManager;
    private readonly BundleFileInstance bundleFileInstance;
    private readonly AssetsFileInstance assetsFileInstance;

    public BundleFile(ILogger logger, FileStream stream)
    {
        this.logger = logger;
        assetsManager = new AssetsManager();
        bundleFileInstance = assetsManager.LoadBundleFile(stream);
        assetsFileInstance = assetsManager.LoadAssetsFileFromBundle(bundleFileInstance, GetAssetsFileIndex());
        LoadPaths();
        LoadGameObjects();
        Initialize();
    }

    public IFileTargetCollector FileTargetCollector => fileTargetCollector;

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

        fileTargetCollector.Commit();
    }

    public BundleResourceCollector CreateResourceCollector()
    {
        return new BundleResourceCollector(logger, assetsManager, bundleFileInstance, assetsFileInstance);
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
            return $"#{asset.index}{defaultExtension}";
        }
    }

    public GameObjectCollection GameObjects => gameObjects;

    public IEnumerable<GameObject> GetComponents(GameObject parent)
    {
        foreach (var data in parent.Field["m_Component"]["Array"].GetChildrenList())
        {
            var component = ResolveGameObject(data[0]);
            if (component != null)
            {
                yield return component;
            }
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

    private void LoadGameObjects()
    {
        gameObjectMap.Clear();
        gameObjects.Clear();

        var parents = new Dictionary<long, long>();

        foreach (var inf in GetAssets(AssetClassID.GameObject))
        {
            var baseField = GetBaseField(inf);

            gameObjectMap.TryAdd(inf.index, new GameObject(inf, baseField));

            foreach (var data in baseField["m_Component"]["Array"].GetChildrenList())
            {
                var component = ResolveGameObject(data[0]);
                if (component == null || !IsTransform(component.TypeId))
                {
                    continue;
                }

                var father = ResolveGameObject(component.Field["m_Father"]);
                if (father == null)
                {
                    continue;
                }

                var gameObject = ResolveGameObject(father.Field["m_GameObject"]);
                if (gameObject == null)
                {
                    continue;
                }

                parents[inf.index] = gameObject.Asset.index;
            }
        }

        foreach (var (childId, parentId) in parents)
        {
            var child = gameObjectMap[childId];
            var parent = gameObjectMap[parentId];
            child.Parent = parent;
            parent.Children.Add(child);
        }

        foreach (var value in gameObjectMap.Values)
        {
            if (value.Parent == null)
            {
                gameObjects.Add(value);
            }

            value.Children.BuildLookupTable();
        }

        static bool IsTransform(AssetClassID typeId) => typeId switch
        {
            AssetClassID.Transform or
            AssetClassID.RectTransform => true,
            _ => false,
        };
    }

    public GameObject? ResolveGameObject(AssetTypeValueField pptr)
    {
        if (pptr.IsDummy()) return default;
        if (pptr["m_FileID"].GetValue().AsInt() != 0) return default;
        var pathId = pptr["m_PathID"].GetValue().AsInt64();
        if (gameObjectMap.TryGetValue(pathId, out var gameObject)) return gameObject;
        var asset = assetsManager.GetExtAsset(assetsFileInstance, 0, pathId);
        if (asset.instance == null) return default;
        gameObject = new GameObject(asset.info, asset.instance.GetBaseField());
        gameObjectMap.Add(pathId, gameObject);
        return gameObject;
    }

    public GameObject? ResolveGameObject(AssetFileInfoEx asset)
    {
        if (asset.curFileType != (int)AssetClassID.GameObject) return null;
        var pathId = asset.index;
        if (gameObjectMap.TryGetValue(pathId, out var gameObject)) return gameObject;
        var baseField = GetBaseField(asset);
        gameObject = new GameObject(asset, baseField);
        gameObjectMap.Add(pathId, gameObject);
        return gameObject;
    }

    private int GetAssetsFileIndex()
    {
        var inf = bundleFileInstance.file.bundleInf6.dirInf;

        for (var i = 0; i < inf.Length; i++)
        {
            if (inf[i].flags == 0)
            {
                continue;
            }

            if (inf[i].name.EndsWith(".sharedAssets"))
            {
                continue;
            }

            return i;
        }

        return 0;
    }

    partial void Initialize();
}

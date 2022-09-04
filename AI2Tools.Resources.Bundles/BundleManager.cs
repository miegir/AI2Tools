using System.Text;
using AssetsTools.NET;
using AssetsTools.NET.Extra;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace AI2Tools;

internal partial class BundleManager
{
    public void Export(ExportArguments arguments)
    {
        using var bundleFile = new BundleFile(logger, source.OpenRead());

        Enumerate()
            .Scoped(logger, "asset")
            .Run();

        IEnumerable<Action> Enumerate()
        {
            foreach (var asset in bundleFile.GetAssets(AssetClassID.Texture2D))
            {
                var name = bundleFile.ReadAssetName(asset, DefaultTexture2DExtension);
                var path = Path.Combine(arguments.ExportDirectory, name);
                if (!arguments.Force && File.Exists(path)) continue;
                var baseField = bundleFile.GetBaseField(asset);
                var textureFile = TextureFile.ReadTextureFile(baseField);
                if (textureFile.m_Width == 0 || textureFile.m_Height == 0) continue;
                yield return () =>
                {
                    logger.LogInformation(message: "exporting texture {name}...", name);

                    var textureData = bundleFile.GetTextureData(textureFile);

                    using var image = Image.LoadPixelData<Bgra32>(
                        textureData, textureFile.m_Width, textureFile.m_Height);

                    image.Mutate(m => m.Flip(FlipMode.Vertical));

                    using var target = new FileTarget(path);
                    image.Save(target, PngFormat.Instance);
                    target.Commit();
                };
            }

            foreach (var asset in bundleFile.GetAssets(AssetClassID.TextAsset))
            {
                var name = bundleFile.ReadAssetName(asset, DefaultTextExtension);
                var path = Path.Combine(arguments.ExportDirectory, name);
                if (!arguments.Force && File.Exists(path)) continue;
                yield return () =>
                {
                    logger.LogInformation(message: "exporting text {name}...", name);

                    var baseField = bundleFile.GetBaseField(asset);
                    var script = baseField["m_Script"].GetValue().AsString();

                    using var target = new FileTarget(path);
                    using (var writer = new StreamWriter(target.Stream, Encoding.UTF8))
                    {
                        writer.Write(script);
                    }

                    target.Commit();
                };
            }
        }
    }

    public bool Muster(
        ObjectPath root,
        MusterArguments arguments,
        BundleResolverFactory bundleResolverFactory,
        BundleFileSource bundleFileSource,
        GameObjectSource gameObjectSource)
    {
        using var bundleFile = new BundleFile(logger, source.OpenRead());

        return Enumerate()
            .Scoped(logger, "asset")
            .Run();

        IEnumerable<Action> Enumerate()
        {
            var bundleResolver = bundleFile.CreateBundleResolver(bundleResolverFactory);

            foreach (var entry in gameObjectSource.Entries)
            {
                var gameObject = bundleFile.FindGameObject(entry.Path);
                if (gameObject == null) continue;

                foreach (var component in bundleFile.GetComponents(gameObject))
                {
                    if (component.TypeId != AssetClassID.MonoBehaviour)
                    {
                        continue;
                    }

                    var scriptName = bundleFile.ReadScriptName(bundleResolver, component.Field);
                    if (scriptName?.FullName != "TMPro.TextMeshProUGUI")
                    {
                        continue;
                    }

                    yield return () =>
                    {
                        var name = bundleFile.ReadAssetName(component.Asset, DefaultAssetExtension);
                        var objectPath = Path.Combine(arguments.ObjectDirectory, name + ".tmprougui.pak");
                        var builder = new ObjectBuilder(
                            gameObjectSource.Destination, objectPath, arguments.ForceObjects);

                        builder.Build(_ => BuildTextMeshProUGUIData(entry));

                        arguments.Sink.ReportObject(root.Append(name + ".tmprougui.pak"), objectPath);
                    };
                }
            }

            foreach (var asset in bundleFile.GetAssets(AssetClassID.Texture2D))
            {
                var name = bundleFile.ReadAssetName(asset, DefaultTexture2DExtension);

                var assetSource = bundleFileSource.FindTexture2D(name, DefaultTexture2DExtension);
                if (assetSource is not null)
                {
                    yield return () =>
                    {
                        var objectPath = Path.Combine(arguments.ObjectDirectory, name + ".texture2d");

                        var builder = new ObjectBuilder(
                            bundleFileSource.Destination, objectPath, arguments.ForceObjects);

                        builder.Build(_ => assetSource.Deserialize());

                        arguments.Sink.ReportObject(root.Append(name + ".pak"), objectPath);
                    };

                    continue;
                }

                var sourcePath = Path.Combine(arguments.SourceDirectory, name);

                if (!File.Exists(sourcePath)) continue;

                yield return () =>
                {
                    var baseField = bundleFile.GetBaseField(asset);
                    var textureFile = TextureFile.ReadTextureFile(baseField);
                    var textureArguments = Texture2DArguments.Create(textureFile, arguments.BC7Compression);
                    var objectPath = Path.Combine(arguments.ObjectDirectory, name, textureArguments.Name);
                    var builder = new ObjectBuilder(sourcePath, objectPath, arguments.ForceObjects);

                    BuildTexture2DObject(builder, textureArguments, name);

                    arguments.Sink.ReportObject(root.Append(name + ".pak"), objectPath);
                };
            }

            foreach (var asset in bundleFile.GetAssets(AssetClassID.TextAsset))
            {
                var name = bundleFile.ReadAssetName(asset, DefaultTextExtension);
                var sourcePath = Path.Combine(arguments.SourceDirectory, name);

                if (!File.Exists(sourcePath)) continue;

                yield return () =>
                {
                    arguments.Sink.ReportObject(root.Append(name), sourcePath);
                };
            }

            if (bundleFileSource.Exists)
            {
                foreach (var asset in bundleFile.GetAssets(AssetClassID.MonoBehaviour))
                {
                    var assetSource = FindFontAsset(bundleFile, bundleResolver, bundleFileSource, asset);
                    if (assetSource == null) continue;
                    yield return () =>
                    {
                        var name = bundleFile.ReadAssetName(asset, DefaultAssetExtension);
                        var objectPath = Path.Combine(arguments.ObjectDirectory, name + ".fnt");

                        var builder = new ObjectBuilder(
                            bundleFileSource.Destination, objectPath, arguments.ForceObjects);

                        builder.Build(_ => assetSource.Deserialize());

                        arguments.Sink.ReportObject(root.Append(name + ".fnt"), objectPath);
                    };
                }
            }
        }
    }

    public bool Import(
        ImportArguments arguments,
        BundleResolverFactory bundleResolverFactory,
        BundleFileSource bundleFileSource,
        GameObjectSource gameObjectSource,
        SourceChangeTracker sourceChangeTracker)
    {
        var hasChanges = arguments.ForceTargets || sourceChangeTracker.HasChanges();
        var assetReplacers = new List<AssetsReplacer>();

        var compression = arguments.BundleCompression switch
        {
            BundleCompressionType.Default => GetCompressionType(source),
            BundleCompressionType.LZ4 => AssetBundleCompressionType.LZ4,
            BundleCompressionType.LZMA => AssetBundleCompressionType.LZMA,
            _ => AssetBundleCompressionType.NONE,
        };

        if (!hasChanges)
        {
            var targetCompression = GetCompressionType(source.Destination);
            if (targetCompression == compression)
            {
                return false;
            }
        }

        using var bundleFile = new BundleFile(logger, source.OpenRead());

        Enumerate()
            .Scoped(logger, "asset")
            .Run();

        if (assetReplacers.Count == 0)
        {
            if (source.CanUnroll())
            {
                logger.LogInformation("unrolling bundle...");
                source.Unroll();
            }

            return true;
        }

        bundleFile.Write(source, assetReplacers, compression);

        return true;

        IEnumerable<Action> Enumerate()
        {
            var bundleResolver = bundleFile.CreateBundleResolver(bundleResolverFactory);

            foreach (var entry in gameObjectSource.Entries)
            {
                var gameObject = bundleFile.FindGameObject(entry.Path);
                if (gameObject == null) continue;

                foreach (var component in bundleFile.GetComponents(gameObject))
                {
                    if (component.TypeId != AssetClassID.MonoBehaviour)
                    {
                        continue;
                    }

                    var scriptName = bundleFile.ReadScriptName(bundleResolver, component.Field);
                    if (scriptName?.FullName != "TMPro.TextMeshProUGUI")
                    {
                        continue;
                    }

                    yield return () =>
                    {
                        logger.LogInformation("importing text mesh pro {name}...", entry.Path);
                        gameObjectSource.Register(sourceChangeTracker);
                        assetReplacers.Add(bundleFile.CreateReplacer(component.Asset, BuildTextMeshProUGUIData(entry)));
                    };
                }
            }

            foreach (var asset in bundleFile.GetAssets(AssetClassID.Texture2D))
            {
                var name = bundleFile.ReadAssetName(asset, DefaultTexture2DExtension);

                if (bundleFileSource.Exists)
                {
                    var assetSource = bundleFileSource.FindTexture2D(name, DefaultTexture2DExtension);
                    if (assetSource is not null)
                    {
                        yield return () =>
                        {
                            logger.LogInformation("importing bundled texture {name}...", name);
                            bundleFileSource.Register(sourceChangeTracker);
                            assetReplacers.Add(bundleFile.CreateReplacer(asset, assetSource));
                        };

                        continue;
                    }
                }

                var sourcePath = Path.Combine(arguments.SourceDirectory, name);

                if (!File.Exists(sourcePath)) continue;

                yield return () =>
                {
                    logger.LogInformation("importing texture {name}...", name);

                    var baseField = bundleFile.GetBaseField(asset);
                    var textureFile = TextureFile.ReadTextureFile(baseField);
                    var textureArguments = Texture2DArguments.Create(textureFile, arguments.BC7Compression);
                    var objectPath = Path.Combine(arguments.ObjectDirectory, name, textureArguments.Name);
                    var builder = new ObjectBuilder(sourcePath, objectPath, arguments.ForceObjects);

                    BuildTexture2DObject(builder, textureArguments, name);

                    sourceChangeTracker.RegisterSource(objectPath);
                    var objectSource = new PhysicalObjectSource<Texture2DData>(objectPath);
                    assetReplacers.Add(bundleFile.CreateReplacer(asset, objectSource));
                };
            }

            foreach (var asset in bundleFile.GetAssets(AssetClassID.TextAsset))
            {
                var name = bundleFile.ReadAssetName(asset, DefaultTextExtension);
                var sourcePath = Path.Combine(arguments.SourceDirectory, name);

                if (!File.Exists(sourcePath)) continue;

                yield return () =>
                {
                    logger.LogInformation("import text asset {name}...", name);

                    var objectPath = Path.Combine(arguments.ObjectDirectory, name + ".pak");
                    var builder = new ObjectBuilder(sourcePath, objectPath, arguments.ForceObjects);

                    BuildTextObject(builder, name);

                    sourceChangeTracker.RegisterSource(sourcePath);
                    var objectSource = new PhysicalObjectSource<string>(objectPath);
                    assetReplacers.Add(bundleFile.CreateReplacer(asset, objectSource));
                };
            }

            if (bundleFileSource.Exists)
            {
                foreach (var asset in bundleFile.GetAssets(AssetClassID.MonoBehaviour))
                {
                    var assetSource = FindFontAsset(bundleFile, bundleResolver, bundleFileSource, asset);
                    if (assetSource == null) continue;
                    yield return () =>
                    {
                        var name = bundleFile.ReadAssetName(asset, DefaultAssetExtension);
                        logger.LogInformation("import font {name}...", name);
                        bundleFileSource.Register(sourceChangeTracker);
                        assetReplacers.Add(bundleFile.CreateReplacer(asset, assetSource));
                    };
                }
            }
        }
    }

    private static IObjectSource<FontAssetData>? FindFontAsset(
        BundleFile bundleFile,
        BundleResolver bundleResolver,
        BundleFileSource bundleFileSource,
        AssetFileInfoEx asset)
    {
        var baseField = bundleFile.GetBaseField(asset);
        var scriptName = bundleFile.ReadScriptName(bundleResolver, baseField);
        if (scriptName?.FullName != "TMPro.TMP_FontAsset")
        {
            return null;
        }

        var name = bundleFile.ReadAssetName(asset, DefaultAssetExtension);
        return bundleFileSource.FindMonoBehavior(
            bundleResolver,
            name,
            scriptName,
            DefaultAssetExtension,
            ReadFontAsset,
            data => data.m_UsedGlyphRects?.Length > 0);
    }

    private static FontAssetData ReadFontAsset(MonoBehaviorContext context)
    {
        var data = FontAssetData.ReadFontAsset(context.BaseField);

        if (data.m_FaceInfo is not null and var info)
        {
            info.m_LineHeight = info.m_PointSize * 2;
        }

        return data;
    }

    private void BuildTexture2DObject(ObjectBuilder builder, Texture2DArguments arguments, string name)
    {
        builder.Build(stream =>
        {
            logger.LogInformation("building texture {name}...", name);

            return new Texture2DEncoder(logger, arguments, stream, name).Encode();
        });
    }

    private void BuildTextObject(ObjectBuilder builder, string name)
    {
        builder.Build(stream =>
        {
            logger.LogInformation("building text {name}...", name);

            using var reader = new StreamReader(stream, Encoding.UTF8);
            return reader.ReadToEnd();
        });
    }

    private static TextMeshProUGUIData BuildTextMeshProUGUIData(GameObjectSource.Entry entry)
    {
        return new TextMeshProUGUIData(TextCompressor.Compress(entry.Text));
    }
}

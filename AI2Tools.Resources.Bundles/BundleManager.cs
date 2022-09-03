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
        Enumerate()
            .Scoped(logger, "asset")
            .Run();

        IEnumerable<Action> Enumerate()
        {
            foreach (var asset in GetAssets(AssetClassID.Texture2D))
            {
                var name = ReadAssetName(asset, DefaultTexture2DExtension);
                var path = Path.Combine(arguments.ExportDirectory, name);
                if (!arguments.Force && File.Exists(path)) continue;
                var baseField = GetBaseField(asset);
                var textureFile = TextureFile.ReadTextureFile(baseField);
                if (textureFile.m_Width == 0 || textureFile.m_Height == 0) continue;
                yield return () =>
                {
                    logger.LogInformation(message: "exporting texture {name}...", name);

                    var textureData = textureFile.GetTextureData(null, bundleFileInstance.file);

                    using var image = Image.LoadPixelData<Bgra32>(
                        textureData, textureFile.m_Width, textureFile.m_Height);

                    image.Mutate(m => m.Flip(FlipMode.Vertical));

                    using var target = new FileTarget(path);
                    image.Save(target, PngFormat.Instance);
                    target.Commit();
                };
            }

            foreach (var asset in GetAssets(AssetClassID.TextAsset))
            {
                var name = ReadAssetName(asset, DefaultTextExtension);
                var path = Path.Combine(arguments.ExportDirectory, name);
                if (!arguments.Force && File.Exists(path)) continue;
                yield return () =>
                {
                    logger.LogInformation(message: "exporting text {name}...", name);

                    var baseField = GetBaseField(asset);
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
        BundleFileSource bundleFileSource,
        MusterArguments arguments)
    {
        var bundleResolver = CreateBundleResolver(arguments.ObjectDirectory);

        return Enumerate()
            .Scoped(logger, "asset")
            .Run();

        IEnumerable<Action> Enumerate()
        {
            foreach (var asset in GetAssets(AssetClassID.Texture2D))
            {
                var name = ReadAssetName(asset, DefaultTexture2DExtension);

                var assetSource = bundleFileSource.FindTexture2D(name, DefaultTexture2DExtension);
                if (assetSource is not null)
                {
                    yield return () =>
                    {
                        var objectPath = Path.Combine(arguments.ObjectDirectory, name + ".t.pak");

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
                    var baseField = GetBaseField(asset);
                    var textureFile = TextureFile.ReadTextureFile(baseField);
                    var textureArguments = Texture2DArguments.Create(textureFile, arguments.BC7Compression);
                    var objectPath = Path.Combine(arguments.ObjectDirectory, name, textureArguments.Name);
                    var builder = new ObjectBuilder(sourcePath, objectPath, arguments.ForceObjects);

                    BuildTexture2DObject(builder, textureArguments, name);

                    arguments.Sink.ReportObject(root.Append(name + ".pak"), objectPath);
                };
            }

            foreach (var asset in GetAssets(AssetClassID.TextAsset))
            {
                var name = ReadAssetName(asset, DefaultTextExtension);
                var sourcePath = Path.Combine(arguments.SourceDirectory, name);

                if (!File.Exists(sourcePath)) continue;

                yield return () =>
                {
                    arguments.Sink.ReportObject(root.Append(name), sourcePath);
                };
            }

            if (bundleFileSource.Exists)
            {
                foreach (var asset in GetAssets(AssetClassID.MonoBehaviour))
                {
                    var assetSource = FindFontAsset(bundleResolver, bundleFileSource, asset);
                    if (assetSource == null) continue;
                    yield return () =>
                    {
                        var name = ReadAssetName(asset, DefaultAssetExtension);
                        var objectPath = Path.Combine(arguments.ObjectDirectory, name + ".f.pak");

                        var builder = new ObjectBuilder(
                            bundleFileSource.Destination, objectPath, arguments.ForceObjects);

                        builder.Build(_ => assetSource.Deserialize());

                        arguments.Sink.ReportObject(root.Append(name + ".f.pak"), objectPath);
                    };
                }
            }
        }
    }

    public bool Import(
        ImportArguments arguments,
        BundleFileSource bundleFileSource,
        SourceChangeTracker sourceChangeTracker)
    {
        var hasChanges = false;
        var assetReplacers = new List<AssetsReplacer>();
        var bundleResolver = CreateBundleResolver(arguments.ObjectDirectory);

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

        var writer = new BundleWriter(logger, bundleFileInstance.file, source);

        writer.Replacers.Add(new BundleReplacerFromAssets(
            oldName: assetsFileInstance.name,
            newName: null,
            assetsFile: assetsFileInstance.file,
            assetReplacers: assetReplacers));

        writer.Write(compression);

        return true;

        IEnumerable<Action> Enumerate()
        {
            foreach (var asset in GetAssets(AssetClassID.Texture2D))
            {
                var name = ReadAssetName(asset, DefaultTexture2DExtension);

                var assetSource = bundleFileSource.FindTexture2D(name, DefaultTexture2DExtension);
                if (assetSource is not null)
                {
                    yield return () =>
                    {
                        if (arguments.ForceTargets || bundleFileSource.IsChanged(sourceChangeTracker))
                        {
                            hasChanges = true;
                        }

                        assetReplacers.Add(CreateReplacer(asset, assetSource));
                    };

                    continue;
                }

                var sourcePath = Path.Combine(arguments.SourceDirectory, name);

                if (!File.Exists(sourcePath)) continue;

                yield return () =>
                {
                    var baseField = GetBaseField(asset);
                    var textureFile = TextureFile.ReadTextureFile(baseField);
                    var textureArguments = Texture2DArguments.Create(textureFile, arguments.BC7Compression);
                    var objectPath = Path.Combine(arguments.ObjectDirectory, name, textureArguments.Name);
                    var builder = new ObjectBuilder(sourcePath, objectPath, arguments.ForceObjects);

                    BuildTexture2DObject(builder, textureArguments, name);

                    if (arguments.ForceTargets || sourceChangeTracker.IsChanged(objectPath))
                    {
                        hasChanges = true;
                    }

                    var objectSource = new PhysicalObjectSource<Texture2DData>(objectPath);
                    assetReplacers.Add(CreateReplacer(asset, objectSource));
                };
            }

            foreach (var asset in GetAssets(AssetClassID.TextAsset))
            {
                var name = ReadAssetName(asset, DefaultTextExtension);
                var sourcePath = Path.Combine(arguments.SourceDirectory, name);

                if (!File.Exists(sourcePath)) continue;

                yield return () =>
                {
                    var objectPath = Path.Combine(arguments.ObjectDirectory, name + ".pak");
                    var builder = new ObjectBuilder(sourcePath, objectPath, arguments.ForceObjects);

                    BuildTextObject(builder, name);

                    if (arguments.ForceTargets || sourceChangeTracker.IsChanged(sourcePath))
                    {
                        hasChanges = true;
                    }

                    var objectSource = new PhysicalObjectSource<string>(objectPath);
                    assetReplacers.Add(CreateReplacer(asset, objectSource));
                };
            }

            if (bundleFileSource.Exists)
            {
                foreach (var asset in GetAssets(AssetClassID.MonoBehaviour))
                {
                    var assetSource = FindFontAsset(bundleResolver, bundleFileSource, asset);
                    if (assetSource == null) continue;
                    yield return () =>
                    {
                        if (arguments.ForceTargets || bundleFileSource.IsChanged(sourceChangeTracker))
                        {
                            hasChanges = true;
                        }

                        assetReplacers.Add(CreateReplacer(asset, assetSource));
                    };
                }
            }
        }
    }

    private BundleResolver CreateBundleResolver(string objectDirectory) => new(
        logger: logger,
        assetsManager: assetsManager,
        directory: Path.GetDirectoryName(bundleFileInstance.path) ?? string.Empty,
        objectPath: objectDirectory + ".resolver");

    private IObjectSource<FontAssetData>? FindFontAsset(
        BundleResolver bundleResolver,
        BundleFileSource bundleFileSource,
        AssetFileInfoEx asset)
    {
        var baseField = GetBaseField(asset);
        var scriptName = ReadScriptName(bundleResolver, baseField);
        if (scriptName?.FullName != "TMPro.TMP_FontAsset")
        {
            return null;
        }

        var name = ReadAssetName(asset, DefaultAssetExtension);
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
}

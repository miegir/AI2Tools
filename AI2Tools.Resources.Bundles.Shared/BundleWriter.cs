using AssetsTools.NET;
using Microsoft.Extensions.Logging;

namespace AI2Tools;

internal class BundleWriter
{
    private readonly ILogger logger;
    private readonly IFileTargetCollector fileTargetCollector;
    private readonly AssetBundleFile bundleFile;
    private readonly FileSource source;

    public BundleWriter(ILogger logger, IFileTargetCollector fileTargetCollector, AssetBundleFile bundleFile, FileSource source)
    {
        this.logger = logger;
        this.fileTargetCollector = fileTargetCollector;
        this.bundleFile = bundleFile;
        this.source = source;
    }

    public List<BundleReplacer> Replacers { get; } = new();

    public void Write(AssetBundleCompressionType compressionType)
    {
        if (compressionType == AssetBundleCompressionType.NONE)
        {
            var target = source.CreateTarget();
            try
            {
                if (WriteUncompressed(target.Stream))
                {
                    fileTargetCollector.AddTarget(target);
                    target = null;
                }
            }
            finally
            {
                target?.Dispose();
            }
        }
        else
        {
            WriteCompressed(compressionType);
        }
    }

    private bool WriteUncompressed(Stream stream)
    {
        using var writer = new AssetsFileWriter(stream);

        logger.LogInformation("writing bundle...");

        if (!bundleFile.Write(writer, Replacers))
        {
            logger.LogError("error writing bundle.");
            return false;
        }

        return true;
    }

    private Stream? GetUncompressedStream()
    {
        using var ms = new MemoryStream();
        return WriteUncompressed(ms)
            ? new MemoryStream(ms.ToArray())
            : null;
    }

    private void WriteCompressed(AssetBundleCompressionType compressionType)
    {
        using var uncompressedStream = GetUncompressedStream();
        if (uncompressedStream == null)
        {
            return;
        }

        using var uncompressedReader = new AssetsFileReader(uncompressedStream);
        using var target = source.CreateTarget();
        using var bundleWriter = new AssetsFileWriter(target.Stream);

        logger.LogInformation("compressing bundle [{compression}]...", compressionType);

        if (bundleFile.Pack(uncompressedReader, bundleWriter, compressionType))
        {
            target.Commit();
        }
        else
        {
            logger.LogError("error compressing bundle.");
        }
    }
}

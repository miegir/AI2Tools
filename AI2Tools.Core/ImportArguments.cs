namespace AI2Tools;

public record ImportArguments(
    string SourceDirectory,
    string ObjectDirectory,
    bool ForceObjects = false,
    bool ForceTargets = false,
    bool Debug = false,
    BC7CompressionType BC7Compression = 0,
    BundleCompressionType BundleCompression = 0);

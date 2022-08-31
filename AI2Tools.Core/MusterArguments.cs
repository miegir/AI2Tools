namespace AI2Tools;

public record MusterArguments(
    MusterSink Sink,
    string SourceDirectory,
    string ObjectDirectory,
    bool ForceObjects = false,
    BC7CompressionType BC7Compression = 0);

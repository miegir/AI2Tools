using System.Text;
using Microsoft.Extensions.Logging;

namespace AI2Tools;

internal class Il2CppMetadataManager
{
    private const uint MetadataSignature = 0xFAB11BAF;
    private static readonly Encoding Encoding = Encoding.UTF8;
    private readonly FileSource source;

    public Il2CppMetadataManager(ILogger logger, FileSource source)
    {
        this.source = source;

        using var stream = source.OpenRead();
        using var reader = new BinaryReader(stream);

        var signature = reader.ReadUInt32();
        if (signature != MetadataSignature)
        {
            logger.LogError("Invalid metadata signature. Expected: {expected}. Actual: {actual}", MetadataSignature, signature);
            StringLiterals = Array.Empty<string>();
            return;
        }

        var version = reader.ReadInt32();
        var stringLiteralOffset = reader.ReadInt32();
        var stringLiteralCount = reader.ReadInt32();
        var stringLiteralDataOffset = reader.ReadInt32();
        var stringLiteralDataCount = reader.ReadInt32();

        // read string literal data span
        stream.Position = stringLiteralDataOffset;
        var stringLiteralData = reader.ReadBytes(stringLiteralDataCount).AsSpan();

        // read string literal data
        var stringLiteralLength = stringLiteralCount / 8;
        StringLiterals = new string[stringLiteralLength];
        stream.Position = stringLiteralOffset;
        for (var i = 0; i < stringLiteralLength; i++)
        {
            var length = reader.ReadInt32();
            var dataIndex = reader.ReadInt32();
            StringLiterals[i] = Encoding.GetString(
                stringLiteralData.Slice(dataIndex, length));
        }
    }

    public string[] StringLiterals { get; }

    public void Save(FileStream targetStream)
    {
        using var sourceStream = source.OpenRead();
        sourceStream.CopyTo(targetStream);

        if (StringLiterals.Length == 0)
        {
            return;
        }

        using var reader = new BinaryReader(targetStream);
        using var writer = new BinaryWriter(targetStream);

        // write string literals
        var newStringLiteralDataOffset = (int)targetStream.Position;
        var stringLiteralBytes = StringLiterals.Select(Encoding.GetBytes).ToArray();
        foreach (var value in stringLiteralBytes)
        {
            writer.Write(value);
        }

        // write string literal data
        writer.Flush();
        targetStream.Position = 8;
        targetStream.Position = reader.ReadInt32();
        var index = 0;
        for (var i = 0; i < stringLiteralBytes.Length; i++)
        {
            var length = stringLiteralBytes[i].Length;
            writer.Write(length);
            writer.Write(index);
            index += length;
        }

        writer.Flush();
        var offset = (int)targetStream.Position;
        targetStream.Position = 16;
        writer.Write(newStringLiteralDataOffset);
        writer.Write(index);
        targetStream.Position = offset;
    }
}

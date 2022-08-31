using MessagePack;
using Microsoft.Extensions.Logging;

namespace AI2Tools;

internal partial class TextMapManager
{
    private static readonly MessagePackSerializerOptions MessageOptions =
        MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4BlockArray);

    private readonly Dictionary<string, string> messages;
    private readonly ILogger logger;
    private readonly FileSource source;

    public TextMapManager(ILogger logger, FileSource source)
    {
        this.logger = logger;
        this.source = source;

        try
        {
            using var stream = source.OpenRead();
            messages = MessagePackSerializer.Deserialize<Dictionary<string, string>>(stream, MessageOptions);
        }
        catch (MessagePackSerializationException)
        {
            messages = new();
        }
    }

    public bool IsEmpty => messages.Count == 0;
    public bool HasWarnings { get; private set; }

    public void Save(Stream stream)
    {
        MessagePackSerializer.Serialize(stream, messages, MessageOptions);
    }

    public void Import(IObjectSource<Dictionary<string, TextMapTranslation>> source, string? debugName)
    {
        var translations = source.Deserialize();

        foreach (var (key, msg) in messages)
        {
            if (translations.TryGetValue(key, out var obj))
            {
                translations.Remove(key);
            }

            if (obj != null && obj.Trx != obj.Src)
            {
                if (obj.Trx == null)
                {
                    continue;
                }

                if (obj.Src != msg)
                {
                    logger.LogWarning("translation source incorrect: '{key}' (was '{old}' but now '{new}').", key, obj.Src, msg);
                    HasWarnings = true;
                    continue;
                }

                messages[key] = TextCompressor.Compress(obj.Trx);
            }
            else
            {
                if (translations.Count > 0)
                {
                    logger.LogWarning("translation missing: '{key}'.", key);
                    HasWarnings = true;
                }

                if (debugName != null)
                {
                    messages[key] = $"{msg} [{debugName}:{key}]";
                }
            }
        }

        foreach (var key in translations.Keys)
        {
            logger.LogWarning("unused translation: '{key}'.", key);
            HasWarnings = true;
        }
    }
}

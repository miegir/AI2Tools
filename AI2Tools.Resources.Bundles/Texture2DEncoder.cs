using System.Diagnostics;
using AssetsTools.NET;
using BCnEncoder.Encoder;
using BCnEncoder.ImageSharp;
using BCnEncoder.Shared;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace AI2Tools;

internal class Texture2DEncoder
{
    private readonly ILogger logger;
    private readonly Texture2DArguments arguments;
    private readonly Stream stream;
    private readonly string name;

    public Texture2DEncoder(ILogger logger, Texture2DArguments arguments, Stream stream, string name)
    {
        this.logger = logger;
        this.arguments = arguments;
        this.stream = stream;
        this.name = name;
    }

    public Texture2DData Encode()
    {
        return arguments.Format switch
        {
            TextureFormat.DXT1 => EncodeBCn(CompressionFormat.Bc1WithAlpha),
            TextureFormat.DXT5 => EncodeBCn(CompressionFormat.Bc3),
            TextureFormat.BC7 => EncodeBCn(CompressionFormat.Bc7),
            _ => EncodeBasic(),
        };
    }

    private Texture2DData EncodeBasic()
    {
        using var image = Image.Load<Bgra32>(stream);
        image.Mutate(m => m.Flip(FlipMode.Vertical));
        var pixels = new byte[4 * image.Width * image.Height];
        image.CopyPixelDataTo(pixels);
        var encodedData = TextureFile.Encode(pixels, arguments.Format, image.Width, image.Height);
        if (encodedData == null) throw new NotSupportedException($"Texture format not supported: '{arguments.Format}'.");
        return new Texture2DData(arguments.Format, image.Width, image.Height, 1, encodedData);
    }

    private Texture2DData EncodeBCn(CompressionFormat compressionFormat)
    {
        using var image = Image.Load<Rgba32>(stream);
        image.Mutate(m => m.Flip(FlipMode.Vertical));
        var encoder = new BcEncoder(compressionFormat);

        encoder.OutputOptions.Quality = CompressionQuality.Balanced;
        encoder.OutputOptions.MaxMipMapLevel = arguments.MipCount;

        var stopwatch = Stopwatch.StartNew();
        var prevPercent = 0;

        encoder.Options.Progress = new Progress<ProgressElement>(e =>
        {
            if (stopwatch.Elapsed.TotalSeconds >= 3)
            {
                var percent = (int)(e.Percentage * 100);
                if (prevPercent < percent)
                {
                    prevPercent = percent;
                    LogPercent(percent);
                    stopwatch.Restart();
                }
            }
        });

        var bytes = encoder.EncodeToRawBytes(image);
        using var ms = new MemoryStream();
        foreach (var level in bytes)
        {
            ms.Write(level, 0, level.Length);
        }

        if (prevPercent is > 0 and < 100)
        {
            LogPercent(100);
        }

        return new Texture2DData(arguments.Format, image.Width, image.Height, bytes.Length, ms.ToArray());

        void LogPercent(int percent)
        {
            logger.LogDebug("encoding texture {name} [{format}]: {percent}%...", name, compressionFormat, percent);
        }
    }
}

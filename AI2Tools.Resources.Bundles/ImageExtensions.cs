using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

namespace AI2Tools;

internal static class ImageExtensions
{
    public static void Save(this Image image, FileTarget target, IImageFormat fallbackFormat)
    {
        var manager = image.Configuration.ImageFormatsManager;

        if (!manager.TryFindFormatByFileExtension(target.Extension, out var format))
        {
            throw new InvalidOperationException($"File format not supported: {target.Extension}");
        }

        image.Save(target.Stream, format ?? fallbackFormat);
    }
}

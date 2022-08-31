using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Formats;

namespace AI2Tools;

internal static class ImageExtensions
{
    public static void Save(this Image image, FileTarget target, IImageFormat fallbackFormat)
    {
        var manager = image.GetConfiguration().ImageFormatsManager;
        var format = manager.FindFormatByFileExtension(target.Extension);
        image.Save(target.Stream, format ?? fallbackFormat);
    }
}

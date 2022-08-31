using AssetsTools.NET;

namespace AI2Tools;

internal record Texture2DArguments(TextureFormat Format, int MipCount)
{
    public static Texture2DArguments Create(TextureFile textureFile, BC7CompressionType bc7Compression)
    {
        var format = (TextureFormat)textureFile.m_TextureFormat;

        if (format == TextureFormat.BC7)
        {
            switch (bc7Compression)
            {
                case BC7CompressionType.DXT1:
                    format = TextureFormat.DXT1;
                    break;

                case BC7CompressionType.DXT5:
                    format = TextureFormat.DXT5;
                    break;
            }
        }

        return new Texture2DArguments(format, textureFile.m_MipCount);
    }

    public string Name => $"{Format}.{MipCount}.pak";
}

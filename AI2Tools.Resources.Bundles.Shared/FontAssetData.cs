using AssetsTools.NET;
using MessagePack;

namespace AI2Tools;

[MessagePackObject]
public class FontAssetData
{
    [Key(00)] public Glyph[]? m_GlyphTable;
    [Key(01)] public Character[]? m_CharacterTable;
    [Key(02)] public GlyphRect[]? m_UsedGlyphRects;
    [Key(03)] public GlyphRect[]? m_FreeGlyphRects;
    [Key(04)] public int m_AtlasWidth;
    [Key(05)] public int m_AtlasHeight;
    [Key(06)] public int m_AtlasPadding;
    [Key(07)] public int m_AtlasRenderMode;
    [Key(08)] public float normalStyle;
    [Key(09)] public float normalSpacingOffset;
    [Key(10)] public float boldStyle;
    [Key(11)] public float boldSpacing;
    [Key(12)] public byte italicStyle;
    [Key(13)] public byte tabSize;

    [MessagePackObject]
    public class Glyph
    {
        [Key(0)] public uint m_Index;
        [Key(1)] public GlyphMetrics? m_Metrics;
        [Key(2)] public GlyphRect? m_GlyphRect;
        [Key(3)] public float m_Scale;
        [Key(4)] public int m_AtlasIndex;
    }

    [MessagePackObject]
    public class GlyphMetrics
    {
        [Key(0)] public float m_Width;
        [Key(1)] public float m_Height;
        [Key(2)] public float m_HorizontalBearingX;
        [Key(3)] public float m_HorizontalBearingY;
        [Key(4)] public float m_HorizontalAdvance;
    }

    [MessagePackObject]
    public class GlyphRect
    {
        [Key(0)] public int m_X;
        [Key(1)] public int m_Y;
        [Key(2)] public int m_Width;
        [Key(3)] public int m_Height;
    }

    [MessagePackObject]
    public class Character
    {
        [Key(0)] public byte m_ElementType;
        [Key(1)] public uint m_Unicode;
        [Key(2)] public int m_GlyphIndex;
        [Key(3)] public float m_Scale;
    }

    public static FontAssetData ReadFontAsset(AssetTypeValueField baseField)
    {
        var data = new FontAssetData();
        baseField["m_GlyphTable"].Read(ref data.m_GlyphTable, ReadGlyph);
        baseField["m_CharacterTable"].Read(ref data.m_CharacterTable, ReadCharacter);
        baseField["m_UsedGlyphRects"].Read(ref data.m_UsedGlyphRects, ReadGlyphRect);
        baseField["m_FreeGlyphRects"].Read(ref data.m_FreeGlyphRects, ReadGlyphRect);
        baseField["m_AtlasWidth"].Read(ref data.m_AtlasWidth);
        baseField["m_AtlasHeight"].Read(ref data.m_AtlasHeight);
        baseField["m_AtlasPadding"].Read(ref data.m_AtlasPadding);
        baseField["m_AtlasRenderMode"].Read(ref data.m_AtlasRenderMode);
        baseField["normalStyle"].Read(ref data.normalStyle);
        baseField["normalSpacingOffset"].Read(ref data.normalSpacingOffset);
        baseField["boldStyle"].Read(ref data.boldStyle);
        baseField["boldSpacing"].Read(ref data.boldSpacing);
        baseField["italicStyle"].Read(ref data.italicStyle);
        baseField["tabSize"].Read(ref data.tabSize);
        return data;
    }

    private static Glyph ReadGlyph(AssetTypeValueField baseField)
    {
        var data = new Glyph();
        baseField["m_Index"].Read(ref data.m_Index);
        baseField["m_Metrics"].Read(ref data.m_Metrics, ReadGlyphMetrics);
        baseField["m_GlyphRect"].Read(ref data.m_GlyphRect, ReadGlyphRect);
        baseField["m_Scale"].Read(ref data.m_Scale);
        baseField["m_AtlasIndex"].Read(ref data.m_AtlasIndex);
        return data;
    }

    private static GlyphMetrics ReadGlyphMetrics(AssetTypeValueField baseField)
    {
        var data = new GlyphMetrics();
        baseField["m_Width"].Read(ref data.m_Width);
        baseField["m_Height"].Read(ref data.m_Height);
        baseField["m_HorizontalBearingX"].Read(ref data.m_HorizontalBearingX);
        baseField["m_HorizontalBearingY"].Read(ref data.m_HorizontalBearingY);
        baseField["m_HorizontalAdvance"].Read(ref data.m_HorizontalAdvance);
        return data;
    }

    private static GlyphRect ReadGlyphRect(AssetTypeValueField baseField)
    {
        var data = new GlyphRect();
        baseField["m_X"].Read(ref data.m_X);
        baseField["m_Y"].Read(ref data.m_Y);
        baseField["m_Width"].Read(ref data.m_Width);
        baseField["m_Height"].Read(ref data.m_Height);
        return data;
    }

    private static Character ReadCharacter(AssetTypeValueField baseField)
    {
        var data = new Character();
        baseField["m_ElementType"].Read(ref data.m_ElementType);
        baseField["m_Unicode"].Read(ref data.m_Unicode);
        baseField["m_GlyphIndex"].Read(ref data.m_GlyphIndex);
        baseField["m_Scale"].Read(ref data.m_Scale);
        return data;
    }

    public void WriteTo(AssetTypeValueField baseField) => WriteTo(baseField, this);

    private static void WriteTo(AssetTypeValueField baseField, FontAssetData data)
    {
        baseField["m_GlyphTable"].Write(data.m_GlyphTable, WriteTo);
        baseField["m_CharacterTable"].Write(data.m_CharacterTable, WriteTo);
        baseField["m_UsedGlyphRects"].Write(data.m_UsedGlyphRects, WriteTo);
        baseField["m_FreeGlyphRects"].Write(data.m_FreeGlyphRects, WriteTo);
        baseField["m_AtlasWidth"].Write(data.m_AtlasWidth);
        baseField["m_AtlasHeight"].Write(data.m_AtlasHeight);
        baseField["m_AtlasPadding"].Write(data.m_AtlasPadding);
        baseField["m_AtlasRenderMode"].Write(data.m_AtlasRenderMode);
        baseField["normalStyle"].Write(data.normalStyle);
        baseField["normalSpacingOffset"].Write(data.normalSpacingOffset);
        baseField["boldStyle"].Write(data.boldStyle);
        baseField["boldSpacing"].Write(data.boldSpacing);
        baseField["italicStyle"].Write(data.italicStyle);
        baseField["tabSize"].Write(data.tabSize);
    }

    private static void WriteTo(AssetTypeValueField baseField, Glyph data)
    {
        baseField["m_Index"].Write(data.m_Index);
        baseField["m_Metrics"].Write(data.m_Metrics, WriteTo);
        baseField["m_GlyphRect"].Write(data.m_GlyphRect, WriteTo);
        baseField["m_Scale"].Write(data.m_Scale);
        baseField["m_AtlasIndex"].Write(data.m_AtlasIndex);
    }

    private static void WriteTo(AssetTypeValueField baseField, GlyphMetrics data)
    {
        baseField["m_Width"].Write(data.m_Width);
        baseField["m_Height"].Write(data.m_Height);
        baseField["m_HorizontalBearingX"].Write(data.m_HorizontalBearingX);
        baseField["m_HorizontalBearingY"].Write(data.m_HorizontalBearingY);
        baseField["m_HorizontalAdvance"].Write(data.m_HorizontalAdvance);
    }

    private static void WriteTo(AssetTypeValueField baseField, GlyphRect data)
    {
        baseField["m_X"].Write(data.m_X);
        baseField["m_Y"].Write(data.m_Y);
        baseField["m_Width"].Write(data.m_Width);
        baseField["m_Height"].Write(data.m_Height);
    }

    private static void WriteTo(AssetTypeValueField baseField, Character data)
    {
        baseField["m_ElementType"].Write(data.m_ElementType);
        baseField["m_Unicode"].Write(data.m_Unicode);
        baseField["m_GlyphIndex"].Write(data.m_GlyphIndex);
        baseField["m_Scale"].Write(data.m_Scale);
    }
}

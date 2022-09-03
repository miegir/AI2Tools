using AssetsTools.NET;

namespace AI2Tools;

public partial class FontAssetData
{
    public static FontAssetData ReadFontAsset(AssetTypeValueField baseField)
    {
        var data = new FontAssetData();
        baseField["m_FaceInfo"].Read(ref data.m_FaceInfo, ReadFaceInfo);
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

    private static FaceInfo ReadFaceInfo(AssetTypeValueField baseField)
    {
        var data = new FaceInfo();
        baseField["m_FaceIndex"].Read(ref data.m_FaceIndex);
        baseField["m_FamilyName"].Read(ref data.m_FamilyName);
        baseField["m_StyleName"].Read(ref data.m_StyleName);
        baseField["m_PointSize"].Read(ref data.m_PointSize);
        baseField["m_Scale"].Read(ref data.m_Scale);
        baseField["m_LineHeight"].Read(ref data.m_LineHeight);
        baseField["m_AscentLine"].Read(ref data.m_AscentLine);
        baseField["m_CapLine"].Read(ref data.m_CapLine);
        baseField["m_MeanLine"].Read(ref data.m_MeanLine);
        baseField["m_Baseline"].Read(ref data.m_Baseline);
        baseField["m_DescentLine"].Read(ref data.m_DescentLine);
        baseField["m_SuperscriptOffset"].Read(ref data.m_SuperscriptOffset);
        baseField["m_SuperscriptSize"].Read(ref data.m_SuperscriptSize);
        baseField["m_SubscriptOffset"].Read(ref data.m_SubscriptOffset);
        baseField["m_SubscriptSize"].Read(ref data.m_SubscriptSize);
        baseField["m_UnderlineOffset"].Read(ref data.m_UnderlineOffset);
        baseField["m_UnderlineThickness"].Read(ref data.m_UnderlineThickness);
        baseField["m_StrikethroughOffset"].Read(ref data.m_StrikethroughOffset);
        baseField["m_StrikethroughThickness"].Read(ref data.m_StrikethroughThickness);
        baseField["m_TabWidth"].Read(ref data.m_TabWidth);
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
}

using System.Diagnostics.CodeAnalysis;
using AssetsTools.NET;

namespace AI2Tools;

internal class VideoClipFileData
{
    public VideoClipFileData(AssetTypeValueField baseField)
    {
        var resource = baseField["m_ExternalResources"];
        if (resource.IsDummy()) return;
        OriginalPath = baseField["m_OriginalPath"].GetValue().AsString();
        if (string.IsNullOrEmpty(OriginalPath)) return;
        var path = resource["m_Source"].GetValue().AsString();
        if (string.IsNullOrEmpty(path)) return;
        if (!path.StartsWith("archive:/")) return;
        var index = path.LastIndexOf('/');
        ResourceName = path[(index + 1)..];
        Offset = resource["m_Offset"].GetValue().AsInt64();
        Size = resource["m_Size"].GetValue().AsInt64();
    }

    [MemberNotNullWhen(true, nameof(OriginalPath))]
    [MemberNotNullWhen(true, nameof(ResourceName))]
    public bool IsValid => ResourceName != null;
    public string? OriginalPath { get; }
    public string? ResourceName { get; }
    public long Offset { get; }
    public long Size { get; }

    public void Write(ResourceReplacerContext context)
    {
        var baseField = context.BaseField;
        var resource = baseField["m_ExternalResources"];
        resource["m_Offset"].GetValue().Set(context.Offset);
        resource["m_Size"].GetValue().Set(context.Size);
    }
}

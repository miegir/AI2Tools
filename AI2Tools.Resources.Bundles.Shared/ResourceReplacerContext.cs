using AssetsTools.NET;

namespace AI2Tools;

internal record ResourceReplacerContext(
    AssetTypeValueField BaseField, long Offset, long Size);

using AssetsTools.NET;

namespace AI2Tools;

internal record MonoBehaviorContext(
    AssetFileInfoEx Asset, ScriptName ScriptName, AssetTypeValueField BaseField);

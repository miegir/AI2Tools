using AssetsTools.NET;

namespace AI2Tools;

internal static partial class AssetTypeValueFieldExtensions
{
    public static void Read(this AssetTypeValueField field, ref byte value)
    {
        if (!field.IsDummy())
        {
            value = (byte)field.GetValue().AsUInt();
        }
    }

    public static void Read(this AssetTypeValueField field, ref int value)
    {
        if (!field.IsDummy())
        {
            value = field.GetValue().AsInt();
        }
    }

    public static void Read(this AssetTypeValueField field, ref long value)
    {
        if (!field.IsDummy())
        {
            value = field.GetValue().AsInt64();
        }
    }

    public static void Read(this AssetTypeValueField field, ref ulong value)
    {
        if (!field.IsDummy())
        {
            value = field.GetValue().AsUInt64();
        }
    }

    public static void Read(this AssetTypeValueField field, ref uint value)
    {
        if (!field.IsDummy())
        {
            value = field.GetValue().AsUInt();
        }
    }

    public static void Read(this AssetTypeValueField field, ref float value)
    {
        if (!field.IsDummy())
        {
            value = field.GetValue().AsFloat();
        }
    }

    public static void Read(this AssetTypeValueField field, ref string? value)
    {
        if (!field.IsDummy())
        {
            value = field.GetValue().AsString();
        }
    }

    public static void Read<T>(this AssetTypeValueField field, ref T? value, Func<AssetTypeValueField, T> reader)
    {
        if (!field.IsDummy())
        {
            value = reader(field);
        }
    }

    public static void Read<T>(this AssetTypeValueField field, ref T[]? value, Func<AssetTypeValueField, T> reader)
    {
        if (!field.IsDummy())
        {
            var array = field["Array"];

            if (!array.IsDummy())
            {
                value = array.GetChildrenList().Select(reader).ToArray();
            }
        }
    }
}

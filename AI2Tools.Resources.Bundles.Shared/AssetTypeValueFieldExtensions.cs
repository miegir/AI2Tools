using AssetsTools.NET;

namespace AI2Tools;

internal static class AssetTypeValueFieldExtensions
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

    public static void Write(this AssetTypeValueField field, byte value)
    {
        if (!field.IsDummy())
        {
            field.GetValue().Set(value);
        }
    }

    public static void Write(this AssetTypeValueField field, int value)
    {
        if (!field.IsDummy())
        {
            field.GetValue().Set(value);
        }
    }

    public static void Write(this AssetTypeValueField field, uint value)
    {
        if (!field.IsDummy())
        {
            field.GetValue().Set(value);
        }
    }

    public static void Write(this AssetTypeValueField field, float value)
    {
        if (!field.IsDummy())
        {
            field.GetValue().Set(value);
        }
    }

    public static void Write<T>(this AssetTypeValueField field, T? value, Action<AssetTypeValueField, T> writer)
    {
        if (value is not null && !field.IsDummy())
        {
            writer(field, value);
        }
    }

    public static void Write<T>(this AssetTypeValueField field, T[]? value, Action<AssetTypeValueField, T> writer)
    {
        if (value is not null && !field.IsDummy())
        {
            var array = field["Array"];
            if (!array.IsDummy())
            {
                var oldChildren = array.GetChildrenList();
                var newChildren = new AssetTypeValueField[value.Length];
                var prototype = array.templateField.children[1];

                for (var i = 0; i < value.Length; i++)
                {
                    var child = newChildren[i] = i < oldChildren.Length
                        ? oldChildren[i]
                        : CreateField(prototype);

                    writer(child, value[i]);
                }

                array.SetChildrenList(newChildren);
                array.GetValue().Set(new AssetTypeArray(newChildren.Length));
            }
        }
    }

    private static AssetTypeValueField CreateField(AssetTypeTemplateField prototype)
    {
        var field = new AssetTypeValueField
        {
            templateField = prototype,
            children = Array.Empty<AssetTypeValueField>(),
        };

        if (prototype.hasValue)
        {
            field.value = new AssetTypeValue(prototype.valueType, null);
        }
        else if (!prototype.isArray && prototype.childrenCount > 0)
        {
            field.childrenCount = prototype.childrenCount;
            field.children = new AssetTypeValueField[prototype.childrenCount];

            for (var i = 0; i < field.children.Length; i++)
            {
                field.children[i] = CreateField(prototype.children[i]);
            }
        }

        return field;
    }
}

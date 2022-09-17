using AssetsTools.NET;

namespace AI2Tools;

internal static partial class AssetTypeValueFieldExtensions
{
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

    public static void Write(this AssetTypeValueField field, long value)
    {
        if (!field.IsDummy())
        {
            field.GetValue().Set(value);
        }
    }

    public static void Write(this AssetTypeValueField field, ulong value)
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

    public static void Write(this AssetTypeValueField field, string? value)
    {
        if (!field.IsDummy())
        {
            field.GetValue().Set(value);
        }
    }

    public static void Write<T>(this AssetTypeValueField field, T? value) where T : IWriteTo
    {
        Write(field, value, (f, v) => v.WriteTo(f));
    }

    public static void Write<T>(this AssetTypeValueField field, T? value, Action<AssetTypeValueField, T> writer)
    {
        if (value is not null && !field.IsDummy())
        {
            writer(field, value);
        }
    }

    public static void Write<T>(this AssetTypeValueField field, T[]? value) where T : IWriteTo
    {
        Write(field, value, (f, v) => v.WriteTo(f));
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

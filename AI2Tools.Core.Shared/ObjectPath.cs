namespace AI2Tools;

public record ObjectPath
{
    public static readonly ObjectPath Root = new(string.Empty);
    private const char ValidSeparator = '/';
    private static readonly HashSet<char> InvalidSeparators = new();

    static ObjectPath()
    {
        InvalidSeparators.Add(Path.DirectorySeparatorChar);
        InvalidSeparators.Add(Path.AltDirectorySeparatorChar);
        InvalidSeparators.Remove(ValidSeparator);
    }

    private ObjectPath(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public override string ToString() => Name;

    public ObjectPath Append(params string[] names)
    {
        var newName = Name;

        for (var i = 0; i < names.Length; i++)
        {
            var name = Normalize(names[i]).TrimEnd(ValidSeparator);

            if (name.StartsWith(ValidSeparator))
            {
                newName = name;
            }
            else
            {
                newName += ValidSeparator + name;
            }
        }

        return new ObjectPath(newName.TrimStart(ValidSeparator));
    }

    private static string Normalize(string name)
    {
        name = name.ToLowerInvariant().Trim();

        foreach (var ch in InvalidSeparators)
        {
            name = name.Replace(ch, ValidSeparator);
        }

        return name;
    }
}

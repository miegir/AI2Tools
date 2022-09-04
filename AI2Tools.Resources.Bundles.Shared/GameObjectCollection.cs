using System.Diagnostics.CodeAnalysis;

namespace AI2Tools;

internal class GameObjectCollection
{
    private readonly List<GameObject> items = new();
    private ILookup<string, GameObject>? lookupTable;

    public bool TryGetValue(string name, [NotNullWhen(true)] out GameObject? value)
    {
        var candidates = lookupTable?[name] ?? items.Where(x => x.Name == name);
        using var candidatesEnumerator = candidates.GetEnumerator();
        if (!candidatesEnumerator.MoveNext())
        {
            value = null;
            return false;
        }

        value = candidatesEnumerator.Current;
        return !candidatesEnumerator.MoveNext();
    }

    public GameObject? Find(string path)
    {
        GameObject? item = null;

        var container = this;
        var segments = path.Split('/');

        foreach (var segment in segments)
        {
            if (!container.TryGetValue(segment, out item))
            {
                break;
            }

            container = item.Children;
        }

        return item;
    }

    public void Add(GameObject child) => items.Add(child);

    public void Clear() => items.Clear();

    public void BuildLookupTable()
    {
        lookupTable = items
            .Where(x => !string.IsNullOrEmpty(x.Name))
            .ToLookup(x => x.Name!);
    }
}

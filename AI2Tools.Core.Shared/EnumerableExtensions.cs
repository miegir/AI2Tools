using Microsoft.Extensions.Logging;

namespace AI2Tools;

public static class EnumerableExtensions
{
    public static IEnumerable<T> Scoped<T>(this IEnumerable<T> source, ILogger logger, string name)
    {
        IEnumerable<T> collection;

        if (source.TryGetNonEnumeratedCount(out var total))
        {
            collection = source;
        }
        else
        {
            var list = source.ToList();
            collection = list;
            total = list.Count;
        }

        var index = 0;
        var totalLength = total.ToString().Length;
        var messageFormat = $"{{name}} [{{index:D{totalLength}}}/{{total}}]";

        foreach (var item in collection)
        {
#pragma warning disable CA2254 // Шаблон должен быть статическим выражением
            using (logger.BeginScope(messageFormat, name, ++index, total))
            {
                yield return item;
            }
#pragma warning restore CA2254 // Шаблон должен быть статическим выражением
        }
    }

    public static IEnumerable<U> Choose<T, U>(this IEnumerable<T> source, Func<T, U?> selector)
    {
        foreach (var item in source)
        {
            if (selector(item) is U target)
            {
                yield return target;
            }
        }
    }

    public static bool Run(this IEnumerable<Action> source)
    {
        var result = false;

        foreach (var item in source)
        {
            item();
            result = true;
        }

        return result;
    }
}

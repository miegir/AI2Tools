using System.Text;

namespace AI2Tools;

internal static class TextCompressor
{
    public static string Compress(string text)
    {
        var builder = new StringBuilder();

        foreach (var ch in text)
        {
            builder.Append(Compress(ch));
        }

        return builder.ToString();
    }

    private static char Compress(char ch) => ch switch
    {
        'А' => 'A',
        'В' => 'B',
        'С' => 'C',
        'Е' => 'E',
        'Н' => 'H',
        'К' => 'K',
        'М' => 'M',
        'О' => 'O',
        'Р' => 'P',
        'Т' => 'T',
        'Х' => 'X',
        'а' => 'a',
        'с' => 'c',
        'е' => 'e',
        'о' => 'o',
        'р' => 'p',
        'х' => 'x',
        'у' => 'y',
        _ => ch
    };
}

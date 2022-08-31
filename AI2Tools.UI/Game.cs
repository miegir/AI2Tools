namespace AI2Tools;

public partial class Game
{
    public IEnumerable<string> EnumerateTextLanguages()
    {
        if (Directory.Exists(textLanguagesDir))
        {
            foreach (var path in Directory.EnumerateDirectories(textLanguagesDir))
            {
                yield return Path.GetFileName(path);
            }
        }
    }
}

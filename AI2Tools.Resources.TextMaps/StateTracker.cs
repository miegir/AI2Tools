using System.Text;

namespace AI2Tools;

internal static class StateTracker
{
    private static readonly Encoding StateEncoding = Encoding.UTF8;

    public static bool IsChanged(string path, string state)
    {
        if (File.Exists(path))
        {
            using var stream = File.OpenRead(path);
            using var reader = new BinaryReader(stream, StateEncoding);

            if (reader.ReadString() == state)
            {
                return false;
            }
        }

        using var target = new FileTarget(path);
        using (var writer = new BinaryWriter(target.Stream, StateEncoding))
        {
            writer.Write(state);
        }

        target.Commit();

        return true;
    }
}

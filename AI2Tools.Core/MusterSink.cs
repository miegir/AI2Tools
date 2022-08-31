using System.IO.Compression;
using MessagePack;
using Microsoft.Extensions.Logging;

namespace AI2Tools;

public class MusterSink
{
    private static readonly MessagePackSerializerOptions MessageOptions =
        MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4BlockArray);

    private record ObjectEntry(string Name, string Path);

    private readonly HashSet<string> directories = new();
    private readonly List<ObjectEntry> objects = new();
    private readonly ILogger logger;
    private DateTime lastWriteTimeUtc;

    public MusterSink(ILogger logger)
    {
        this.logger = logger;
    }

    public void ReportDirectory(ObjectPath name)
    {
        directories.Add(name.Name);
    }

    public void ReportObject(ObjectPath name, string path)
    {
        var info = new FileInfo(path);

        if (!info.Exists)
        {
            return;
        }

        objects.Add(new ObjectEntry(name.Name, path));

        if (lastWriteTimeUtc < info.LastWriteTimeUtc)
        {
            lastWriteTimeUtc = info.LastWriteTimeUtc;
        }
    }

    public void Pack(PackArguments arguments)
    {
        var archiveInfo = new FileInfo(arguments.ArchivePath);

        if (objects.Count == 0)
        {
            if (archiveInfo.Exists)
            {
                logger.LogInformation("removing archive...");
                archiveInfo.Delete();
            }

            return;
        }

        if (!arguments.Force && archiveInfo.LastWriteTimeUtc >= lastWriteTimeUtc)
        {
            return;
        }

        logger.LogInformation("writing archive...");

        using var target = new FileTarget(arguments.ArchivePath);
        WriteArchive();
        target.Commit();

        void WriteArchive()
        {
            using var archive = new ZipArchive(target.Stream, ZipArchiveMode.Create);

            if (directories.Count > 0)
            {
                var entry = archive.CreateEntry(".dirs");
                using var stream = entry.Open();
                MessagePackSerializer.Serialize(stream, directories, MessageOptions);
            }

            foreach (var (name, path) in objects)
            {
                archive.CreateEntryFromFile(path, name);
            }
        }
    }
}

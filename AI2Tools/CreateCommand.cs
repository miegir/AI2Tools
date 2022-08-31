using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace AI2Tools;

[Command("create")]
internal class CreateCommand
{
    private readonly ILogger<CreateCommand> logger;

    public CreateCommand(ILogger<CreateCommand> logger)
    {
        this.logger = logger;
    }

#nullable disable
    [Required]
    [FileExists]
    [Option("-g|--game-path")]
    public string GamePath { get; }

    [Required]
    [LegalCultureName]
    [Option("-t|--text-language")]
    public string TextLanguage { get; }

    [Required]
    [DirectoryExists]
    [Option("-s|--source-directory")]
    public string SourceDirectory { get; }

    [Required]
    [LegalFilePath]
    [Option("-j|--object-directory")]
    public string ObjectDirectory { get; }

    [Option("-7|--bc7-compression")]
    public BC7CompressionType BC7Compression { get; }

    [Required]
    [LegalFilePath]
    [Option("-a|--archive-path")]
    public string ArchivePath { get; }

    [Option("-f|--force")]
    public bool Force { get; }

    [Option("--force-objects")]
    public bool ForceObjects { get; }

    [Option("--force-pack")]
    public bool ForcePack { get; }
#nullable restore

    public void OnExecute()
    {
        logger.LogInformation("executing...");

        var sink = new MusterSink(logger);

        new Game(logger, GamePath)
            .CreatePipeline(TextLanguage)
            .Muster(new MusterArguments(
                Sink: sink,
                SourceDirectory: SourceDirectory,
                ObjectDirectory: ObjectDirectory,
                ForceObjects: Force || ForceObjects,
                BC7Compression: BC7Compression));

        sink.Pack(new PackArguments(
            ArchivePath: ArchivePath,
            Force: Force || ForcePack));

        logger.LogInformation("executed.");
    }
}

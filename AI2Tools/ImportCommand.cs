using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace AI2Tools;

[Command("import")]
internal class ImportCommand
{
    private readonly ILogger<ImportCommand> logger;

    public ImportCommand(ILogger<ImportCommand> logger)
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

    [Option("-c|--bundle-compression")]
    public BundleCompressionType BundleCompression { get; }

    [Option("-f|--force")]
    public bool Force { get; }

    [Option("--force-objects")]
    public bool ForceObjects { get; }

    [Option("--force-targets")]
    public bool ForceTargets { get; }

    [Option("-d|--debug")]
    public bool Debug { get; }

    [Option("-l|--launch")]
    public bool Launch { get; }
#nullable restore

    public void OnExecute()
    {
        logger.LogInformation("executing...");

        new Game(logger, GamePath)
            .CreatePipeline(TextLanguage)
            .Import(new ImportArguments(
                SourceDirectory: SourceDirectory,
                ObjectDirectory: ObjectDirectory,
                ForceObjects: Force || ForceObjects,
                ForceTargets: Force || ForceTargets,
                Debug: Debug,
                BC7Compression: BC7Compression,
                BundleCompression: BundleCompression));

        logger.LogInformation("executed.");

        if (Launch)
        {
            Process.Start(GamePath).Dispose();
        }
    }
}

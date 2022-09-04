using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace AI2Tools;

[Command("export")]
internal class ExportCommand
{
    private readonly ILogger<ExportCommand> logger;

    public ExportCommand(ILogger<ExportCommand> logger)
    {
        this.logger = logger;
    }

#nullable disable
    [Required]
    [FileExists]
    [Option("-g|--game-path")]
    public string GamePath { get; }

    [Required]
    [LegalFilePath]
    [Option("-e|--export-directory")]
    public string ExportDirectory { get; }

    [Option("-f|--force")]
    public bool Force { get; }

    [Option("--force-export")]
    public bool ForceExport { get; }
#nullable restore

    public void OnExecute()
    {
        logger.LogInformation("executing...");

        new Game(logger, GamePath)
            .CreatePipeline()
            .Export(new ExportArguments(ExportDirectory, Force: Force || ForceExport));

        logger.LogInformation("executed.");
    }
}

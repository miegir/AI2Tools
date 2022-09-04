using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace AI2Tools;

[Command("unpack")]
internal class UnpackCommand
{
	private readonly ILogger<UnpackCommand> logger;

	public UnpackCommand(ILogger<UnpackCommand> logger)
	{
		this.logger = logger;
    }

#nullable disable
    [Required]
    [FileExists]
    [Option("-g|--game-path")]
    public string GamePath { get; }

    [Required]
    [FileExists]
    [Option("-a|--archive-path")]
    public string ArchivePath { get; }

    [Option("-c|--bundle-compression")]
    public BundleCompressionType BundleCompression { get; }

    [Option("-d|--debug")]
    public bool Debug { get; }

    [Option("-l|--launch")]
    public bool Launch { get; }
#nullable restore

    public void OnExecute()
    {
        logger.LogInformation("executing...");

        using var stream = File.OpenRead(ArchivePath);
        using var container = new ObjectContainer(stream);

        new Game(logger, GamePath)
            .CreatePipeline()
            .Unpack(new UnpackArguments(
                Container: container,
                BundleCompression: BundleCompression,
                Debug: Debug));

        logger.LogInformation("executed.");

        if (Launch)
        {
            Process.Start(GamePath).Dispose();
        }
    }
}

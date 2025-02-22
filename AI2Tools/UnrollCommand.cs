﻿using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace AI2Tools;

[Command("unroll")]
internal class UnrollCommand
{
	private readonly ILogger<UnrollCommand> logger;

	public UnrollCommand(ILogger<UnrollCommand> logger)
	{
		this.logger = logger;
    }

#nullable disable
    [Required]
    [FileExists]
    [Option("-g|--game-path")]
    public string GamePath { get; }
#nullable restore

    public void OnExecute()
    {
        logger.LogInformation("executing...");

        new Game(logger, GamePath)
            .CreatePipeline()
            .Unroll();

        logger.LogInformation("executed.");
    }
}

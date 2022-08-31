using McMaster.Extensions.CommandLineUtils;

namespace AI2Tools;

[Command]
[Subcommand(
    typeof(ExportCommand),
    typeof(ImportCommand),
    typeof(CreateCommand),
    typeof(UnpackCommand),
    typeof(UnrollCommand))]
internal class RootCommand
{
    private readonly CommandLineApplication application;

    public RootCommand(CommandLineApplication application)
    {
        this.application = application;
    }

    public void OnExecute()
    {
        application.ShowHelp();
    }
}

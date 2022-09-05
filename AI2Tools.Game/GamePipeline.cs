namespace AI2Tools;

public partial class GamePipeline
{
    public void Export(ExportArguments arguments) => resources
        .SelectMany(r => r.BeginExport(arguments))
        .Scoped(logger, "resource")
        .Run();

    public void Import(ImportArguments arguments) => resources
        .SelectMany(r => r.BeginImport(arguments))
        .Scoped(logger, "resource")
        .Run();

    public void Muster(MusterArguments arguments) => resources
        .SelectMany(r => r.BeginMuster(arguments))
        .Scoped(logger, "resource")
        .Run();
}

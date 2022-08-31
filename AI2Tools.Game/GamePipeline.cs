namespace AI2Tools;

public partial class GamePipeline
{
    public void Export(ExportArguments arguments) => resources
        .Choose(r => r.BeginExport(arguments))
        .Scoped(logger, "resource")
        .Run();

    public void Import(ImportArguments arguments) => resources
        .Choose(r => r.BeginImport(arguments))
        .Scoped(logger, "resource")
        .Run();

    public void Muster(MusterArguments arguments) => resources
        .Choose(r => r.BeginMuster(arguments))
        .Scoped(logger, "resource")
        .Run();
}

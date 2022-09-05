namespace AI2Tools;

public partial interface IResource
{
    IEnumerable<Action> BeginExport(ExportArguments arguments);
    IEnumerable<Action> BeginImport(ImportArguments arguments);
    IEnumerable<Action> BeginMuster(MusterArguments arguments);
}

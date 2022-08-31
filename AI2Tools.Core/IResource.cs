namespace AI2Tools;

public partial interface IResource
{
    Action? BeginExport(ExportArguments arguments);
    Action? BeginImport(ImportArguments arguments);
    Action? BeginMuster(MusterArguments arguments);
}

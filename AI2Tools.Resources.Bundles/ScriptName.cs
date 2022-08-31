using System.Diagnostics;
using AssetsTools.NET;

namespace AI2Tools;

[DebuggerDisplay("{FullName,nq}")]
internal record ScriptName(string Name, string ClassName, string Namespace, string AssemblyName)
{
	public static ScriptName Read(AssetTypeValueField baseField)
	{
        var name = baseField["m_Name"].GetValue().AsString();
        var className = baseField["m_ClassName"].GetValue().AsString();
        var namespaceName = baseField["m_Namespace"].GetValue().AsString();
        var assemblyName = baseField["m_AssemblyName"].GetValue().AsString();
        return new ScriptName(name, className, namespaceName, assemblyName);
	}

    public string FullName => $"{Namespace}.{ClassName}";
}

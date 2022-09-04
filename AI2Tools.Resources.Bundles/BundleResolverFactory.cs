using AssetsTools.NET.Extra;
using Microsoft.Extensions.Logging;

namespace AI2Tools;

internal class BundleResolverFactory
{
	private readonly ILogger logger;
	private readonly string objectPath;

	public BundleResolverFactory(ILogger logger, string objectPath)
	{
		this.logger = logger;
		this.objectPath = objectPath;
	}

	public BundleResolver CreateBundleResolver(BundleFileInstance bundleFileInstance)
	{
		var directory = Path.GetDirectoryName(bundleFileInstance.path) ?? string.Empty;
		return new BundleResolver(logger, directory, objectPath);
	}
}

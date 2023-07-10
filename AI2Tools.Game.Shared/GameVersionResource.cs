using Microsoft.Extensions.Logging;

namespace AI2Tools;

internal partial class GameVersionResource : IResource
{
    private readonly ILogger logger;
    private readonly Func<GameVersionInfo> versionGetter;

    public GameVersionResource(ILogger logger, Func<GameVersionInfo> versionGetter)
    {
        this.logger = logger;
        this.versionGetter = versionGetter;
    }

    public IEnumerable<Action> BeginUnpack(UnpackArguments arguments)
    {
        return Enumerable.Empty<Action>();
    }

    public IEnumerable<Action> BeginUnroll()
    {
        return Enumerable.Empty<Action>();
    }
}

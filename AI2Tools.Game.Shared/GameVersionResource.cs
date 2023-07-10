using Microsoft.Extensions.Logging;

namespace AI2Tools;

internal partial class GameVersionResource : IResource
{
    private readonly ILogger logger;
    private readonly Game game;

    public GameVersionResource(ILogger logger, Game game)
    {
        this.logger = logger;
        this.game = game;
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

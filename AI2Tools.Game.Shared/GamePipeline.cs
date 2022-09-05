using Microsoft.Extensions.Logging;

namespace AI2Tools;

public partial class GamePipeline
{
    private readonly ILogger logger;
    private readonly IEnumerable<IResource> resources;

    public GamePipeline(ILogger logger, IEnumerable<IResource> resources)
    {
        this.logger = logger;
        this.resources = resources;
    }

    public void Unpack(UnpackArguments arguments) => resources
        .SelectMany(r => r.BeginUnpack(arguments))
        .Scoped(logger, "resource")
        .Run();

    public void Unroll() => resources
        .SelectMany(r => r.BeginUnroll())
        .Scoped(logger, "resource")
        .Run();
}

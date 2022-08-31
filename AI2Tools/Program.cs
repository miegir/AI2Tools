using AI2Tools;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

return await Host
    .CreateDefaultBuilder()
    .ConfigureLogging(builder =>
    {
        builder.AddFilter(nameof(AI2Tools), LogLevel.Debug);
        builder.AddSimpleConsole(options =>
        {
            options.IncludeScopes = true;
        });
    })
    .RunCommandLineApplicationAsync<RootCommand>(args);

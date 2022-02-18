using Orleans.Hosting;

namespace RealtimeMap.Orleans;

public class SiloHostedService : IHostedService
{
    private readonly ISiloHost _siloHost;

    public SiloHostedService(ISiloHost siloHost)
    {
        _siloHost = siloHost;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _siloHost.StartAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _siloHost.StopAsync(cancellationToken);
    }
}
namespace Client;

using Orleans.Runtime;
using Orleans.Runtime.Messaging;

internal sealed class ClientConnectionRetryFilter : IClientConnectionRetryFilter
{
    public async Task<bool> ShouldRetryConnectionAttempt(Exception exception, CancellationToken cancellationToken)
    {
        if (!cancellationToken.IsCancellationRequested
            && exception is SiloUnavailableException
                or ConnectionFailedException // needed for client streams to reconnect to cluster when cluster was shut down
           )
        {
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
            return true;
        }

        return false;
    }
}
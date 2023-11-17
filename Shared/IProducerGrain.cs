namespace Shared;

using Orleans.Concurrency;

public interface IProducerGrain : IGrainWithIntegerKey
{
    [OneWay]
    Task Start();
}
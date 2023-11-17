namespace Server;

using Orleans.Runtime;
using Orleans.Streams;
using Shared;

public class ProducerGrain : Grain, IProducerGrain
{



    public async Task Start()
    {
        var stream = this.GetStreamProvider(Constants.SMSProvider).GetStream<Message>(StreamId.Create(Constants.StreamName, 0));
        while (true)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(400));
            await stream.OnNextAsync(new Message(DateTime.Now));
            Console.WriteLine($"{DateTime.Now} - Sent message");
        }
    }
}
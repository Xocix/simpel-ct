using SimpelTransport.CodeTest.Application.Interfaces;
using SimpelTransport.CodeTest.Infrastructure.Contexts;

namespace SimpelTransport.CodeTest.Workers;

public class OrderWorker(IMessageQueue queue, MyDbContext dbContext) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var message = await queue.ReceiveMessageAsync();
            
            if (message == null)
            {
                continue;
            }

            var order = await dbContext.Orders.FindAsync([message.OrderId], cancellationToken);
            
            if (order is { Status: "Pending" })
            {
                await queue.SendMessageAsync(message);
                continue;
            }

            Console.WriteLine($"Processed order {message.OrderId}");
        }
    }
}
using Microsoft.EntityFrameworkCore;

using SimpelTransport.CodeTest.Application.Interfaces;
using SimpelTransport.CodeTest.Infrastructure.Contexts;

namespace SimpelTransport.CodeTest.Workers;

public class OrderWorker(IMessageQueue queue, IDbContextFactory<MyDbContext> dbContextFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var message = await queue.ReceiveMessageAsync();

            if (message == null)
            {
                await Task.Delay(500, cancellationToken);
                continue;
            }

            await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            var order = await dbContext.Orders.FindAsync([message.OrderId], cancellationToken);

            // Alot of complex business logic here, but we will just check the status for simplicity

            if (order is { Status: "Pending" })
            {
                await queue.SendMessageAsync(message);
                continue;
            }

            Console.WriteLine($"Processed order {message.OrderId}");
        }
    }
}
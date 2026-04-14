using Microsoft.EntityFrameworkCore;
using SimpelTransport.CodeTest.Application.Dto;
using SimpelTransport.CodeTest.Application.Interfaces;
using SimpelTransport.CodeTest.Domain.Entities;
using SimpelTransport.CodeTest.Infrastructure.Contexts;
using SimpelTransport.CodeTest.Infrastructure.Services;
using SimpelTransport.CodeTest.Workers;
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContextFactory<MyDbContext>(options => options.UseInMemoryDatabase("TestDb"));
builder.Services.AddSingleton<IMessageQueue, MockQueue>();
builder.Services.AddHostedService<OrderWorker>();

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<MyDbContext>>();
    await using var db = await dbFactory.CreateDbContextAsync();
    var queue = scope.ServiceProvider.GetRequiredService<IMessageQueue>();

    var order = new Order { Id = 1, Details = "Test Order", Price = 150m, Status = "Pending" };
    db.Orders.Add(order);
    await db.SaveChangesAsync();

    await queue.SendMessageAsync(new OrderMessageDto(order.Id));
}

Console.WriteLine("Worker started.");

await host.RunAsync();

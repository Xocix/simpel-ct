using System.Collections.Concurrent;
using SimpelTransport.CodeTest.Application.Dto;
using SimpelTransport.CodeTest.Application.Interfaces;

namespace SimpelTransport.CodeTest.Infrastructure.Services;

public class MockQueue : IMessageQueue
{
    private readonly ConcurrentQueue<OrderMessageDto> _messages = new();

    public Task<OrderMessageDto?> ReceiveMessageAsync()
    {
        _messages.TryDequeue(out var msg);
        return Task.FromResult(msg);
    }

    public Task SendMessageAsync(OrderMessageDto message)
    {
        _messages.Enqueue(message);
        return Task.CompletedTask;
    }
}
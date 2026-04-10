using SimpelTransport.CodeTest.Application.Dto;

namespace SimpelTransport.CodeTest.Application.Interfaces;

public interface IMessageQueue
{
    Task<OrderMessageDto?> ReceiveMessageAsync();
    Task SendMessageAsync(OrderMessageDto message);
}
using MediatR;
using MicroRabbit.Domain.Core.Bus;
using MicroRabbit.Domain.Core.Commands;
using MicroRabbit.Domain.Core.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace MicroRabbit.Infrastructure.Bus;
public sealed class RabbitMQBus : IEventBus
{
    private readonly IMediator _mediator;
    private readonly Dictionary<string, List<Type>> _handlers;
    private readonly List<Type> _eventTypes;

    public RabbitMQBus(IMediator mediator)
    {
        _mediator = mediator;
        _handlers = [];
        _eventTypes = [];
    }

    public Task SendCommand<T>(T command) where T : Command
    {
        return _mediator.Send(command);
    }

    public async Task PublishAsync<T>(T @event) where T : Event
    {
        var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672 };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        var eventName = @event.GetType().Name;

        await channel.QueueDeclareAsync(eventName, false, false, false);

        var message = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(message);

        await channel.BasicPublishAsync("", eventName, body);
    }

    public Task SubscribeAsync<T, TH>()
        where T : Event
        where TH : IEventHandler
    {
        var eventName = typeof(T).Name;
        var handlerType = typeof(TH);

        if (!_eventTypes.Contains(typeof(T)))
        {
            _eventTypes.Add(typeof(T));
        }

        if (!_handlers.ContainsKey(eventName))
        {
            _handlers.Add(eventName, new List<Type>());
        }

        if (_handlers[eventName].Any(s => s.GetType() == handlerType))
        {
            throw new ArgumentException($"Handler Type {handlerType.Name} is already registered for {eventName}", nameof(handlerType));
        }

        _handlers[eventName].Add(handlerType);

        StartBasicConsumeAsync<T>();
    }
}

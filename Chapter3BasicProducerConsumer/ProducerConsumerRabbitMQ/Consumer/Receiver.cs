using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var connectionFactory = new ConnectionFactory() { HostName = "localhost", Port = 5672 };
using var connection = await connectionFactory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();
await channel.QueueDeclareAsync("BasicTest", false, false, false, null);

var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += async (model, ea) =>
{
    try
    {
        var body = ea.Body;
        var message = Encoding.UTF8.GetString(body.Span);
        Console.WriteLine($"Received message: {message}");

        await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error processing message: {ex.Message}");

        await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true);
    }
};

await channel.BasicConsumeAsync("BasicTest", autoAck: false, consumer);
Console.ReadKey();
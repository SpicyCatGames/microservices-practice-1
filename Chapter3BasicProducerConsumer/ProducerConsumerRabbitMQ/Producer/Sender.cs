using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672 };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();
await channel.QueueDeclareAsync("BasicTest", false, false, false, null);

string message = "Getting  started with .Net RabbitMQ";
var body = Encoding.UTF8.GetBytes(message);

await channel.BasicPublishAsync("", "BasicTest", body);
Console.WriteLine($"Sent message {message}");

Console.ReadKey();
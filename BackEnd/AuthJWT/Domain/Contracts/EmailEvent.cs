using System.Text.Json;
using Confluent.Kafka;

public class EmailEvent
{
    public string To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
}
public interface IKafkaProducerService
{
    Task PublishEmailEventAsync(EmailEvent emailEvent);
}

public class KafkaProducerService : IKafkaProducerService
{
    private readonly string _bootstrapServers;
    private readonly string _topic;

    public KafkaProducerService(IConfiguration config)
    {
        _bootstrapServers = config["Kafka:BootstrapServers"];
        _topic = config["Kafka:EmailTopic"];
    }

    public async Task PublishEmailEventAsync(EmailEvent emailEvent)
    {
        var config = new ProducerConfig { BootstrapServers = _bootstrapServers };
        using var producer = new ProducerBuilder<Null, string>(config).Build();
        var message = JsonSerializer.Serialize(emailEvent);
        await producer.ProduceAsync(_topic, new Message<Null, string> { Value = message });
    }
}

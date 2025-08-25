using System.Text.Json;
using Confluent.Kafka;
using Confluent.Kafka.Admin;

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
    private readonly ILogger<KafkaProducerService> _logger;

    public KafkaProducerService(IConfiguration config, ILogger<KafkaProducerService> logger)
    {
        _bootstrapServers = config["Kafka:BootstrapServers"];
        _topic = config["Kafka:EmailTopic"] ?? "email-events";
        _logger = logger;
    }

    public async Task PublishEmailEventAsync(EmailEvent emailEvent)
    {
        try
        {
            // Ensure topic exists before producing
            await EnsureTopicExists();

            var config = new ProducerConfig { BootstrapServers = _bootstrapServers };
            using var producer = new ProducerBuilder<Null, string>(config).Build();
            
            var message = JsonSerializer.Serialize(emailEvent);
            var result = await producer.ProduceAsync(_topic, new Message<Null, string> { Value = message });
            
            _logger.LogInformation("Email event published to Kafka: {Subject} for {To}", emailEvent.Subject, emailEvent.To);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish email event to Kafka for {To}", emailEvent.To);
            throw;
        }
    }

    private async Task EnsureTopicExists()
    {
        try
        {
            using var adminClient = new AdminClientBuilder(new AdminClientConfig 
            { 
                BootstrapServers = _bootstrapServers 
            }).Build();

            var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(5));
            var topicExists = metadata.Topics.Any(t => t.Topic == _topic);

            if (!topicExists)
            {
                await adminClient.CreateTopicsAsync(new[]
                {
                    new TopicSpecification
                    {
                        Name = _topic,
                        NumPartitions = 1,
                        ReplicationFactor = 1
                    }
                });

                _logger.LogInformation("Created Kafka topic: {Topic}", _topic);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not ensure Kafka topic exists: {Topic}", _topic);
        }
    }
}
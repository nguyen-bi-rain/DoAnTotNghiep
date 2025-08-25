using System.Text.Json;
using AuthJWT.Domain.Contracts;
using AuthJWT.Services.Interfaces;
using Confluent.Kafka;
using Confluent.Kafka.Admin;

namespace AuthJWT.Services.Implements
{
    public class EmailKafkaConsumer : BackgroundService
    {
        private readonly string _bootstrapServers;
        private readonly string _topic;
        private readonly ISendEmailService _sendEmailService;
        private readonly ILogger<EmailKafkaConsumer> _logger;

        public EmailKafkaConsumer(
            IConfiguration configuration, 
            ISendEmailService sendEmailService,
            ILogger<EmailKafkaConsumer> logger)
        {
            _bootstrapServers = configuration["Kafka:BootstrapServers"];
            _topic = configuration["Kafka:EmailTopic"] ?? "email-events";
            _sendEmailService = sendEmailService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                // Create topic if it doesn't exist
                await CreateTopicIfNotExists();

                var config = new ConsumerConfig
                {
                    BootstrapServers = _bootstrapServers,
                    GroupId = "email-consumer-group",
                    AutoOffsetReset = AutoOffsetReset.Earliest,
                    EnableAutoCommit = false
                };

                using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
                consumer.Subscribe(_topic);

                _logger.LogInformation("Kafka consumer started, listening to topic: {Topic}", _topic);

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumerResult = consumer.Consume(stoppingToken);
                        if (consumerResult != null)
                        {
                            var emailEvent = JsonSerializer.Deserialize<EmailEvent>(consumerResult.Message.Value);
                            
                            await _sendEmailService.SendEmailAsync(new MailRequest
                            {
                                ToEmail = emailEvent.To,
                                Subject = emailEvent.Subject,
                                Body = emailEvent.Body
                            });

                            // Commit the message after successful processing
                            consumer.Commit(consumerResult);
                            
                            _logger.LogInformation("Email sent successfully to {To}", emailEvent.To);
                        }
                    }
                    catch (ConsumeException ex)
                    {
                        _logger.LogError(ex, "Error consuming message from Kafka");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing email message");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kafka consumer failed to start");
            }
        }

        private async Task CreateTopicIfNotExists()
        {
            try
            {
                using var adminClient = new AdminClientBuilder(new AdminClientConfig 
                { 
                    BootstrapServers = _bootstrapServers 
                }).Build();

                var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
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
                else
                {
                    _logger.LogInformation("Kafka topic already exists: {Topic}", _topic);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not create Kafka topic. Make sure Kafka is running and accessible.");
                throw;
            }
        }
    }
}
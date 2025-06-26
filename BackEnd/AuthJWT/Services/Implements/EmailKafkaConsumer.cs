
using System.Text.Json;
using AuthJWT.Domain.Contracts;
using AuthJWT.Services.Interfaces;
using Confluent.Kafka;
using Org.BouncyCastle.Crypto.Signers;

namespace AuthJWT.Services.Implements
{
    public class EmailKafkaConsumer : BackgroundService
    {
        private readonly string _bootstrapServers;
        private readonly string _topic;
        private readonly ISendEmailService _sendEmailService;

        public EmailKafkaConsumer(IConfiguration configuration, ISendEmailService sendEmailService)
        {
            _bootstrapServers = configuration["Kafka:BootstrapServers"];
            _topic = configuration["Kafka:EmailTopic"];
            _sendEmailService = sendEmailService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _bootstrapServers,
                GroupId = "email-consumer-group",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };
            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe(_topic);

            while (!stoppingToken.IsCancellationRequested)
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
                }
            }

        }
    }
}
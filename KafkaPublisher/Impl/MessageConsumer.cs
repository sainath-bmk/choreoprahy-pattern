using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaPublisher.Impl
{
    public class MessageConsumer : IHostedService
    {
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;

        public MessageConsumer(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            var bootStrapServer = AppSettings.GetConfig(_configuration, "Consumer");

            var config = new ConsumerConfig
            {
                BootstrapServers = bootStrapServer?.Where(x => x.Key.Equals("BootStrapServer")).FirstOrDefault().Value,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };


            try
            {
                using (var consumerBuilder = new ConsumerBuilder<Ignore, string>(config).Build())
                {
                    consumerBuilder.Subscribe(topic);
                    var cancelToken = new CancellationTokenSource();

                    try
                    {
                        while (true)
                        {
                            var consumer = consumerBuilder.Consume
                               (cancelToken.Token);
                            var orderRequest = JsonSerializer.Deserialize<OrderProcessingRequest>(consumer.Message.Value);
                            Debug.WriteLine($"Processing Order Id: 
                                { orderRequest.OrderId}
                            ");
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        consumerBuilder.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}

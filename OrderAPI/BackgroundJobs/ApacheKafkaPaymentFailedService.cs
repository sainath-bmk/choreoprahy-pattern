using Confluent.Kafka;
using KafkaPublisher.Impl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using OrderAPI.Controllers;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentAPI.BackgroundJobs
{

    public class ApacheKafkaPaymentFailedService : IHostedService
    {
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly string topic = "payment-rejected";
        private readonly string groupId = "group1";

        public ApacheKafkaPaymentFailedService(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            var consumerAppSettings = AppSettings.GetConfig(_configuration, "Consumer");

            var config = new ConsumerConfig
            {
               
                GroupId = groupId,
                BootstrapServers = consumerAppSettings?.Where(x => x.Key.Equals("BootStrapServer")).FirstOrDefault().Value,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = consumerAppSettings?.Where(x => x.Key.Equals("ApiKey")).FirstOrDefault().Value,
                SaslPassword = consumerAppSettings?.Where(x => x.Key.Equals("ApiSecret")).FirstOrDefault().Value,
                SslCaLocation = Path.Combine(_hostingEnvironment.ContentRootPath, consumerAppSettings?.Where(x => x.Key.Equals("SslCertificatePath")).FirstOrDefault().Value),
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            try
            {
                using (var consumerBuilder = new ConsumerBuilder
                <Ignore, string>(config).Build())
                {
                    consumerBuilder.Subscribe(topic);
                    var cancelToken = new CancellationTokenSource();

                    try
                    {
                        while (true)
                        {
                            var consumer = consumerBuilder.Consume
                               (cancelToken.Token);
                            var orderRequest = JsonConvert.DeserializeObject<Order>(consumer.Message.Value);
                            Debug.WriteLine($"Processing Order Id: { orderRequest.Id} ");
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

            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}

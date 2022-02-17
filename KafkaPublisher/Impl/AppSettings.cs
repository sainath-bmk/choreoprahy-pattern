using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace KafkaPublisher.Impl
{
    public static class AppSettings
    {
        private const string keyPrefix = "kafkaSettings";

        public static List<KeyValuePair<string,string>> GetConfig(IConfiguration configuration, string Key)
        {
            return configuration.GetSection($"{keyPrefix}:{Key}").GetChildren().ToDictionary(x => x.Key, x => x.Value).ToList();
        }

        public static string GetTopicName(IConfiguration configuration, string key)
        {
            var result = configuration.GetSection($"{keyPrefix}:Producer:{key}").GetChildren()
                .ToDictionary(x => x.Key, x => x.Value).ToList()?.Where(x => x.Key.Equals("TopicName")).FirstOrDefault().Value;

            return result;
        }
    }
}

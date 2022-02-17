using System;
using System.Text.Json.Serialization;

namespace KafkaPublisher.Models
{
    public class ExceptionMessage
    {
        public string Message { get; set; }

        [JsonIgnore]
        public DateTime PostedDate { get; set; }
    }
}

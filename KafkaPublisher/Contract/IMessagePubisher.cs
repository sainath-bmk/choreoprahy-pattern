using KafkaPublisher.Models;
using System.Threading.Tasks;

namespace KafkaPublisher.Contract
{
    public interface IMessagePubisher
    {
        /// <summary>
        /// produces message to given topic
        /// </summary>
        /// <param name="message">message</param>
        /// <returns></returns>
        Task<bool> PublishAsync(string topic ,string message);
    }
}

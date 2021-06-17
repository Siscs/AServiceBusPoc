namespace Siscs.Api.ServiceBusSender
{
    public class ServiceBusConfig
    {
        public string QueueConnection { get; set; }
        public string QueueName { get; set; }
        public string TopicConnection { get; set; }
        public string TopicName { get; set; }

    }
}
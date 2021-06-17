namespace Siscs.Api.ServiceBusTopicReceiver
{
    public class ServiceBusConfig
    {
        public string QueueConnection { get; set; }
        public string QueueName { get; set; }
        public string TopicConnection { get; set; }
        public string TopicName { get; set; }
        public string SubscriptionName { get; set; }

    }
}
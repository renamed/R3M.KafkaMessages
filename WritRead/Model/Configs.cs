namespace WritRead.Model
{
    public class Configs
    {
        public string MicroserviceName { get; set; }
        public string ConsumerTopicName { get; set; }
        public string ProducerTopicName { get; set; }
        public string GroupId { get; set; }
        public string BootstrapServers { get; set; }
        public string Text { get; set; }
        public int IntervalBetweenMessagesInSeconds { get; set; }
    }
}
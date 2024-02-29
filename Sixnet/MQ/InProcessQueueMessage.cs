namespace Sixnet.MQ
{
    /// <summary>
    /// Defines internal queue message
    /// </summary>
    public abstract class InProcessQueueMessage : ISixnetQueueMessage
    {
        /// <summary>
        /// Queue name
        /// </summary>
        public string QueueName { get; set; }

        /// <summary>
        /// Execute
        /// </summary>
        public abstract void Execute();
    }
}

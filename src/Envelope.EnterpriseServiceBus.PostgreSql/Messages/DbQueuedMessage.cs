namespace Envelope.EnterpriseServiceBus.PostgreSql.Messages;

#nullable disable

public class DbQueuedMessage
{
	public Guid MessageId { get; set; }
	public QueuedMessageDto QueuedMessage { get; set; }
}

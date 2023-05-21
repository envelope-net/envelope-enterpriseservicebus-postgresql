using Envelope.EnterpriseServiceBus.Orchestrations.Model;

namespace Envelope.EnterpriseServiceBus.PostgreSql.Messages;

#nullable disable

public class DbOrchestrationEvent
{
	public string OrchestrationKey { get; set; }
	public Guid MessageId { get; set; }
	public OrchestrationEvent OrchestrationEventMessage { get; set; }
}

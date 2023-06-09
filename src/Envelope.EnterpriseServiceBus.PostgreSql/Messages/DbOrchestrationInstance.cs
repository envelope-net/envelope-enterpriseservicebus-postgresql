﻿using Envelope.EnterpriseServiceBus.Orchestrations;
using Envelope.EnterpriseServiceBus.Orchestrations.Dto;

namespace Envelope.EnterpriseServiceBus.PostgreSql.Messages;

#nullable disable

public class DbOrchestrationInstance
{
	public Guid IdOrchestrationInstance { get; set; }
	public OrchestrationInstanceDto OrchestrationInstance { get; set; }

	public DbOrchestrationInstance Initialize(IOrchestrationInstance orchestrationInstance)
	{
		if (orchestrationInstance == null)
			throw new ArgumentNullException(nameof(orchestrationInstance));

		IdOrchestrationInstance = orchestrationInstance.IdOrchestrationInstance;
		OrchestrationInstance = new OrchestrationInstanceDto(orchestrationInstance);
		return this;
	}
}

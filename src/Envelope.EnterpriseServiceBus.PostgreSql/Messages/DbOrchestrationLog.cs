﻿using Envelope.Logging;

namespace Envelope.EnterpriseServiceBus.PostgreSql.Messages;

public class DbOrchestrationLog
{
	public Guid IdLogMessage { get; set; }
	public ILogMessage LogMessage { get; set; }
	public int IdLogLevel { get; set; }

	public DbOrchestrationLog(ILogMessage logMessage)
	{
		if (logMessage == null)
			throw new ArgumentNullException(nameof(logMessage));

		IdLogMessage = logMessage.IdLogMessage;
		LogMessage = logMessage;
		IdLogLevel = logMessage.IdLogLevel;
	}
}

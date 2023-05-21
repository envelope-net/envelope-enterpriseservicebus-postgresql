using Envelope.EnterpriseServiceBus.PostgreSql.Messages;
using Marten.Linq;
using System.Linq.Expressions;

namespace Envelope.EnterpriseServiceBus.PostgreSql.Exchange.Internal;

public class ExecutionPointerByIdStepQuery : ICompiledQuery<DbExecutionPointer, DbExecutionPointer?>
{
	public Guid IdOrchestrationInstance { get; set; }
	public Guid IdStep { get; set; }

	public Expression<Func<IMartenQueryable<DbExecutionPointer>, DbExecutionPointer?>> QueryIs()
	{
		return q => q.FirstOrDefault(x => x.IdOrchestrationInstance == IdOrchestrationInstance && x.ExecutionPointer.IdStep == IdStep);
	}
}

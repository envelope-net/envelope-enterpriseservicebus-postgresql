﻿using Envelope.Enums;
using Envelope.ServiceBus.Jobs;
using Envelope.ServiceBus.Queries;

namespace Envelope.EnterpriseServiceBus.PostgreSql.Messages;

public class DbJob : IDbJob
{
	public Guid JobInstanceId { get; set; }
	public Guid HostInstanceId { get; set; }
	public string HostName { get; set; }
	public string Name { get; set; }
	public string? Description { get; set; }
	public bool Disabled { get; set; }
	public int Mode { get; set; }
	public TimeSpan? DelayedStart { get; set; }
	public TimeSpan? IdleTimeout { get; set; }
	public string? CronExpression { get; set; }
	public bool CronExpressionIncludeSeconds { get; set; }
	public DateTime? NextExecutionRunUtc { get; set; }
	public int Status { get; set; }
	public IReadOnlyDictionary<int, string>? JobExecutionOperations { get; set; }
	public IReadOnlyList<int>? AssociatedJobMessageTypes { get; set; }
	public int CurrentExecuteStatus { get; set; }
	public int ExecutionEstimatedTimeInSeconds { get; set; }
	public int DeclaringAsOfflineAfterMinutesOfInactivity { get; set; }
	public DateTime LastUpdateUtc { get; set; }
	public DateTime? LastExecutionStartedUtc { get; set; }

	public static DbJob Create(IJob job, JobExecuteResult executeResult)
	{
		if (job == null)
			throw new ArgumentNullException(nameof(job));

		if (executeResult == null)
			throw new ArgumentNullException(nameof(executeResult));

		return new DbJob
		{
			JobInstanceId = job.JobInstanceId,
			HostInstanceId = job.HostInfo.InstanceId,
			HostName = job.HostInfo.HostName,
			Name = job.Name,
			Description = job.Description,
			Disabled = job.Disabled,
			Mode = (int)job.Mode,
			DelayedStart = job.DelayedStart,
			IdleTimeout = job.IdleTimeout,
			CronExpression = job.CronTimerSettings?.Expression,
			CronExpressionIncludeSeconds = job.CronTimerSettings?.IncludeSeconds ?? false,
			NextExecutionRunUtc = job.NextExecutionRunUtc,
			Status = (int)job.Status,
			JobExecutionOperations = job.JobExecutionOperations,
			AssociatedJobMessageTypes = job.AssociatedJobMessageTypes,
			CurrentExecuteStatus = (int)executeResult.ExecuteStatus,
			ExecutionEstimatedTimeInSeconds = job.ExecutionEstimatedTimeInSeconds,
			DeclaringAsOfflineAfterMinutesOfInactivity = job.DeclaringAsOfflineAfterMinutesOfInactivity,
			LastUpdateUtc = job.LastUpdateUtc,
			LastExecutionStartedUtc = job.LastExecutionStartedUtc
		};
	}

	public string ToJson()
		=> Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);

	public JobStatus GetJobActivityStatus()
		=> JobStatusHelper.GetJobActivityStatus(this);

	public override string ToString()
		=> $"{Name} | {GetJobActivityStatus()}";
}
